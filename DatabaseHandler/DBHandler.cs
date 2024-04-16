using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SSB.Database
{
    public class DBHandler
    {
        private static string hostname = "2019-SRV14.lunarcolony.local";
        private static string dbname = "ssbd";
        private static SqlConnection SqlConn;

        public static async Task OpenConnection()
        {
            string ConnStr = "Data Source=" + hostname + ";Initial Catalog=" + dbname + ";Integrated Security=SSPI"/*+ ";Encrypt=true"*/;
            SqlConn = new SqlConnection(ConnStr);
            await SqlConn.OpenAsync();
            return;
        }

        public static async Task InsertGuildUserRoles(ulong UserID, ulong GuildID, List<ulong> RoleIDs)
        {
            const string Query = "INSERT INTO roles (userid, guildid, userroles) VALUES (@userid, @guildid, @userroles)";
            SqlCommand Cmd = new SqlCommand(Query, SqlConn);
            Cmd.Parameters.AddWithValue("@userid", UserID);
            Cmd.Parameters.AddWithValue("@guildid", GuildID);
            Cmd.Parameters.AddWithValue("@userroles", JsonConvert.SerializeObject(RoleIDs));
            await Cmd.ExecuteNonQueryAsync();
            return;
        }
        public static List<ulong> FetchGuildUserRoles(ulong UserID, ulong GuildID)
        {
            const string Query = "SELECT userroles FROM roles WHERE userid = @userid AND guildid = @guildid LIMIT 1";
            string JsonString = "{}";
            SqlCommand Cmd = new SqlCommand(Query, SqlConn);
            Cmd.Parameters.AddWithValue("@userid", UserID);
            Cmd.Parameters.AddWithValue("@guildid", GuildID);
            using (SqlDataReader reader = Cmd.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    JsonString = reader.GetString(0);
                }
            }
            return JsonConvert.DeserializeObject<List<ulong>>(JsonString);
        }

        public static async Task UpdateUserRoles(ulong UserID, ulong GuildID, List<ulong> RoleIDs)
        {
            const string Query = "UPDATE roles SET userroles = @userroles WHERE userid = @userid AND guildid = @guildid)";
            SqlCommand Cmd = new SqlCommand(Query, SqlConn);
            Cmd.Parameters.AddWithValue("@userid", UserID);
            Cmd.Parameters.AddWithValue("@guildid", GuildID);
            Cmd.Parameters.AddWithValue("@userroles", JsonConvert.SerializeObject(RoleIDs));
            await Cmd.ExecuteNonQueryAsync();
            return;
        }

        public static bool CheckUserRolesExists(ulong UserID, ulong GuildID)
        {
            const string Query = "SELECT COUNT(*) FROM roles WHERE userid = @userid AND guildid = @guildid LIMIT 1";
            SqlCommand Cmd = new SqlCommand(Query, SqlConn);
            Cmd.Parameters.AddWithValue("@userid", UserID);
            Cmd.Parameters.AddWithValue("@guildid", GuildID);
            using (SqlDataReader reader = Cmd.ExecuteReader())
            {
                return reader.HasRows;
            }
        }

        public static async Task InsertNewGuild(ulong GuildID)
        {
            const string Query = "INSERT INTO guilds (guildid, status) VALUES (@guildid, 1)";
            SqlCommand Cmd = new SqlCommand(Query, SqlConn);
            Cmd.Parameters.AddWithValue("@guildid", GuildID);
            await Cmd.ExecuteNonQueryAsync();
            return;
        }

        public static bool CheckGuildExists(ulong GuildID)
        {
            string Query = "SELECT COUNT(*) FROM guilds WHERE guildid = @guildid LIMIT 1";
            SqlCommand Cmd = new SqlCommand(Query, SqlConn);
            Cmd.Parameters.AddWithValue("@guildid", GuildID);
            using (SqlDataReader reader = Cmd.ExecuteReader())
            {
                return reader.HasRows;
            }
        }
    }
}