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
using System.Threading.Tasks;
using SSB.Core;
using SSB.Core.Database;

namespace GlobalChatTester
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            await DBHandler.OpenConnection(new SSBConfig { DBHostname = "2019-SRV14.lunarcolony.local", DBDatabase = "ssbd" });
            await DBHandler.InsertNewGlobalChatMessage(new GlobalChatMessage { Author = "dolphin", Message = "test", Source = "Test", Timestamp = DateTime.Now });
        }
    }
}
