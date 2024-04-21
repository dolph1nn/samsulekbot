using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SSB.Core.Database
{
    public static class DBHandler
    {
        private static SqlConnection SqlConn { get; set; }

        /// <summary>
        /// This method opens the connection to the database.
        /// </summary>
        /// <param name="Config"></param>
        /// <returns></returns>
        public static async Task OpenConnection(SSBConfig Config)
        {
            SqlConn = new SqlConnection("Data Source=" + Config.DBHostname + ";Initial Catalog=" + Config.DBDatabase + ";Integrated Security=SSPI" + ";Encrypt=" + Config.DBSSL.ToString().ToLower() + ";MultipleActiveResultSets=true");
            await SqlConn.OpenAsync();
            return;
        }

        /// <summary>
        /// Inserts a new GuildUserRole object into the database.
        /// This stores a JSON-Serialized string of their RoleIDs which gets converted back into a List<ulong> to do things with.
        /// </summary>
        /// <param name="UserID"></param>
        /// <param name="GuildID"></param>
        /// <param name="RoleIDs"></param>
        /// <returns></returns>
        public static async Task InsertGuildUserRoles(ulong UserID, ulong GuildID, List<ulong> RoleIDs)
        {
            const string Query = "INSERT INTO roles (userid, guildid, roles) VALUES (@UserID, @GuildID, @UserRoles)";
            SqlCommand Cmd = new SqlCommand(Query, SqlConn);
            Cmd.Parameters.AddWithValue("@UserID", (long)UserID);
            Cmd.Parameters.AddWithValue("@GuildID", (long)GuildID);
            Cmd.Parameters.AddWithValue("@UserRoles", JsonConvert.SerializeObject(RoleIDs));
            await Cmd.ExecuteNonQueryAsync();
            return;
        }

        /// <summary>
        /// This returns a List<ulong> from the JSON string of their roles. See the method above.
        /// </summary>
        /// <param name="UserID"></param>
        /// <param name="GuildID"></param>
        /// <returns></returns>
        public static async Task<List<ulong>> FetchGuildUserRoles(ulong UserID, ulong GuildID)
        {
            const string Query = "SELECT roles FROM roles WHERE userid = @userid AND guildid = @guildid";
            string JsonString = "{}";
            SqlCommand Cmd = new SqlCommand(Query, SqlConn);
            Cmd.Parameters.AddWithValue("@userid", (long)UserID);
            Cmd.Parameters.AddWithValue("@guildid", (long)GuildID);
            using (SqlDataReader reader = await Cmd.ExecuteReaderAsync())
            {
                if (reader.HasRows)
                {
                    await reader.ReadAsync();
                    JsonString = reader.GetString(0);
                }
            }
            return JsonConvert.DeserializeObject<List<ulong>>(JsonString);
        }

        /// <summary>
        /// Updates a GuildUserRoles entry. See two above methods.
        /// </summary>
        /// <param name="UserID"></param>
        /// <param name="GuildID"></param>
        /// <param name="RoleIDs"></param>
        /// <returns></returns>
        public static async Task UpdateGuildUserRoles(ulong UserID, ulong GuildID, List<ulong> RoleIDs)
        {
            const string Query = "UPDATE roles SET userroles = @userroles WHERE userid = @userid AND guildid = @guildid)";
            SqlCommand Cmd = new SqlCommand(Query, SqlConn);
            Cmd.Parameters.AddWithValue("@userid", (long)UserID);
            Cmd.Parameters.AddWithValue("@guildid", (long)GuildID);
            Cmd.Parameters.AddWithValue("@userroles", JsonConvert.SerializeObject(RoleIDs));
            await Cmd.ExecuteNonQueryAsync();
            return;
        }

        public static async Task<bool> CheckUserRolesExists(ulong UserID, ulong GuildID)
        {
            bool exists = false;
            const string Query = "SELECT * FROM roles WHERE userid = @userid AND guildid = @guildid";
            SqlCommand Cmd = new SqlCommand(Query, SqlConn);
            Cmd.Parameters.AddWithValue("@userid", (long)UserID);
            Cmd.Parameters.AddWithValue("@guildid", (long)GuildID);
            try
            {
                using (SqlDataReader reader = await Cmd.ExecuteReaderAsync())
                {
                    exists = reader.HasRows;
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
            return exists;
        }

        public static async Task InsertNewGuild(ulong GuildID)
        {
            const string Query = "INSERT INTO guilds (guildid, status) VALUES (@GuildID, 1)";
            SqlCommand Cmd = new SqlCommand(Query, SqlConn);
            Cmd.Parameters.AddWithValue("@GuildID", (long)GuildID);
            await Cmd.ExecuteNonQueryAsync();
            return;
        }

        public static async Task<bool> CheckGuildExists(ulong GuildID)
        {
            bool exists = false;
            string Query = "SELECT * FROM guilds WHERE guildid = @GuildID";
            SqlCommand Cmd = new SqlCommand(Query, SqlConn);
            Cmd.Parameters.AddWithValue("@GuildID", (Int64)GuildID);
            try
            {
                using (SqlDataReader reader = await Cmd.ExecuteReaderAsync())
                {
                    exists = reader.HasRows;
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
            return exists;
        }

        public static async Task CheckNewGlobalChatMessages(DateTime LastChecked)
        {
            List<GlobalChatMessage> messages = new List<GlobalChatMessage>();
            string Query = "SELECT id, timestamp, source, author, message FROM globalchat WHERE timestamp >= @lastchecked AND source != 0 AND history = 0";
            SqlCommand Cmd = new SqlCommand(Query, SqlConn);
            Cmd.Parameters.AddWithValue("@lastchecked", LastChecked);
            using (SqlDataReader reader = await Cmd.ExecuteReaderAsync())
            {
                if (reader.HasRows)
                {
                    while (await reader.ReadAsync())
                    {
                        messages.Add(new GlobalChatMessage { ID = reader.GetInt32(0), Timestamp = reader.GetDateTime(1), Source = reader.GetString(2), Author = reader.GetString(3), Message = reader.GetString(4) });
                    }

                    foreach (GlobalChatMessage message in messages)
                    {
                        await Discord.DiscordHandler.ProcessGlobalChatMessage_in(message);
                        await ArchiveGlobalChatMessage(message);
                    }
                }
            }
            return;
        }

        private static async Task ArchiveGlobalChatMessage(GlobalChatMessage message)
        {
            string Query = "UPDATE globalchat SET history = 1 WHERE id = @uid";
            SqlCommand Cmd = new SqlCommand(Query, SqlConn);
            Cmd.Parameters.AddWithValue("@uid", message.ID);
            await Cmd.ExecuteNonQueryAsync();
        }

        public static async Task InsertNewGlobalChatMessage(GlobalChatMessage message)
        {
            string Query = "INSERT INTO globalchat (timestamp, source, author, message) VALUES (@timestamp, @source, @author, @message)";
            SqlCommand Cmd = new SqlCommand(Query, SqlConn);
            Cmd.Parameters.AddWithValue("@timestamp", message.Timestamp);
            Cmd.Parameters.AddWithValue("@source", message.Source);
            Cmd.Parameters.AddWithValue("@author", message.Author);
            Cmd.Parameters.AddWithValue("@message", message.Message);
            await Cmd.ExecuteNonQueryAsync();
        }
    }
}
