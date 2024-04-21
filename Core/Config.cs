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

namespace SSB.Core
{
    /// <summary>
    /// This is the class for the Config file.
    /// </summary>
    public class SSBConfig
    {
        public string Token { get; set; } = String.Empty;
        public ulong GlobalChatChannel { get; set; } = 0;
        public string DBDriver { get; set; } = String.Empty;
        public string DBHostname { get; set; } = String.Empty;
        public string DBDatabase { get; set; } = String.Empty;
        public string DBPort { get; set; } = String.Empty;
        public bool DBSSL { get; set; } = false;
        public bool DBAuthType { get; set; } = false;
        public string DBUsername { get; set; } = String.Empty;
        public string DBPassword { get; set; } = String.Empty;
    }
}
