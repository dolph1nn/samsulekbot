/* This file is part of SamSulekBot.
 *
 * SamSulekBot is free software: you can redistribute it and/or modify it
 * under the terms of the GNU General Public License as published by the
 * Free Software Foundation, either version 3 of the License, or (at your
 * option) any later version.
 *
 * SamSulekBot is distributed in the hope that it will be useful, but WITHOUT
 * ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or 
 * FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for
 * more details.
 *
 * You should have received a copy of the GNU General Public License along
 * with SamSulekBot. If not, see <https://www.gnu.org/licenses/>.
 */

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

        /// <summary>
        /// This method opens the connection to the database.
        /// </summary>
        /// <param name="Config"></param>
        /// <returns></returns>
        public static async Task OpenConnection(SSBConfig Config)
        {
            SqlConn = new SqlConnection("Data Source=" + Config.DBHostname + ";Initial Catalog=" + Config.DBDatabase + ";Integrated Security=SSPI" + ";Encrypt=" + Config.DBSSL.ToString().ToLower());
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
            const string Query = "SELECT COUNT(*) FROM roles WHERE userid = @userid AND guildid = @guildid";
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
    }
}
