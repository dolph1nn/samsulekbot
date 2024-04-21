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

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
using System.ServiceProcess;

namespace SSB.Service
{
    public partial class SSBService : ServiceBase
    {
        public SSBService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            DiscordBot.DiscordHandler.Init();
        }

        protected override void OnStop()
        {
            DiscordBot.DiscordHandler.Stop();
        }
    }
}