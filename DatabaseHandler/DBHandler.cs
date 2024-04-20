using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SSB.Core;

namespace SSB.Database
{
    public static class DBHandler
    {
        private static SqlConnection SqlConn { get; set; }

        public static async Task OpenConnection(SSBConfig Config)
        {
            string ConnStr = "Data Source=" + Config.DBHostname + ";Initial Catalog=" + Config.DBDatabase + ";Integrated Security=SSPI"+ ";Encrypt=" + Config.DBSSL.ToString().ToLower();
            SqlConn = new SqlConnection(ConnStr);
            await SqlConn.OpenAsync();
            return;
        }

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
        public static async Task<List<ulong>> FetchGuildUserRoles(ulong UserID, ulong GuildID)
        {
            const string Query = "SELECT userroles FROM roles WHERE userid = @userid AND guildid = @guildid";
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

        public static async Task UpdateUserRoles(ulong UserID, ulong GuildID, List<ulong> RoleIDs)
        {
            const string Query = "UPDATE roles SET userroles = @userroles WHERE userid = @userid AND guildid = @guildid)";
            SqlCommand Cmd = new SqlCommand(Query, SqlConn);
            Cmd.Parameters.AddWithValue("@userid", (long)UserID);
            Cmd.Parameters.AddWithValue("@guildid", (long)GuildID);
            Cmd.Parameters.AddWithValue("@userroles", JsonConvert.SerializeObject(RoleIDs));
            await Cmd.ExecuteNonQueryAsync();
            return;
        }

        public static bool CheckUserRolesExists(ulong UserID, ulong GuildID)
        {
            const string Query = "SELECT COUNT(*) FROM roles WHERE userid = @userid AND guildid = @guildid";
            SqlCommand Cmd = new SqlCommand(Query, SqlConn);
            Cmd.Parameters.AddWithValue("@userid", (long)UserID);
            Cmd.Parameters.AddWithValue("@guildid", (long)GuildID);
            using (SqlDataReader reader = Cmd.ExecuteReader())
            {
                return reader.HasRows;
            }
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
    }
}