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
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Discord;
using AngouriMath;
using Discord.WebSocket;
using Newtonsoft.Json;
using SSB.Database;
using SSB.Core;

namespace SSB.Discord
{
    public static class DiscordHandler
    {
        public static DiscordSocketClient SocketClient { get; private set; }
        public static SSBConfig Config { get; private set; }

        /// <summary>
        /// starts the discord handler
        /// </summary>
        /// <returns>task stuff idk</returns>
        public static async Task Init()
        {
            SocketClient = new DiscordSocketClient(new DiscordSocketConfig { /*MessageCacheSize = 100, */GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.MessageContent | GatewayIntents.GuildMembers | GatewayIntents.Guilds });
            Config = JsonConvert.DeserializeObject<SSBConfig>(File.ReadAllText(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"\cfg.json")); //File.ReadAllText(@"c:\ssb\cfg.json")
            await SocketClient.LoginAsync(TokenType.Bot, Config.Token);
            await SocketClient.StartAsync();
            SocketClient.UserJoined += UserJoinGuildEvent;
            SocketClient.GuildMemberUpdated += GuildMemberUpdatedEvent;
            SocketClient.SlashCommandExecuted += Commands.SlashCommandHandler;
            SocketClient.MessageReceived += MessageEvent;
            SocketClient.Ready += ReadyEvent;
            await Task.Delay(-1);
        }

        /// <summary>
        /// Stops the discord handler
        /// </summary>
        /// <returns>task stuff idk</returns>
        public static async Task Stop()
        {
            await SocketClient.LogoutAsync();
            await SocketClient.StopAsync();
        }

        /// <summary>
        /// a little function i wrote to make sending messages to a specific channel easier
        /// </summary>
        /// <param name="message">a string containing the message you want to send</param>
        /// <returns>task stuff idk</returns>
        private static async Task SendStartupMessage(string message)
        {
            var channel = await SocketClient.GetChannelAsync(200075640739725312) as IMessageChannel;
            await channel.SendMessageAsync(message);
        }

        /// <summary>
        /// a little function i wrote to make sending messages to a specific channel easier
        /// </summary>
        /// <param name="message">a string containing the message you want to send</param>
        /// <param name="channelid">a ulong of the channel ID you want to send the message to</param>
        /// <returns>task stuff idk</returns>
        private static async Task SendStartupMessage(string message, ulong channelid)
        {
            var channel = await SocketClient.GetChannelAsync(channelid) as IMessageChannel;
            await channel.SendMessageAsync(message);
        }

        private static async Task ReadyEvent()
        {
            Console.WriteLine("Bot is connected!");
            await DBHandler.OpenConnection(Config);
            await SendStartupMessage("Connected to Database!", 430528035247095818);
            await SocketClient.GetGuild(170683612092432387).DownloadUsersAsync();
            // Uncomment this line to provision any commands you need/want to.
            //await Commands.BuildCommands();
            return;
        }

        /// <summary>
        /// This function is fired whenever a GuildMember joins the server.
        /// It sees if they already have a database entry; if so it gives them their old roles. If not, it makes them an entry.
        /// </summary>
        /// <param name="User"></param>
        /// <returns></returns>
        private static async Task UserJoinGuildEvent(SocketGuildUser User)
        {
            if (await DBHandler.CheckUserRolesExists(User.Id, User.Guild.Id))
            {
                await User.AddRolesAsync(await DBHandler.FetchGuildUserRoles(User.Id, User.Guild.Id));
            } 
            else
            {
                await DBHandler.InsertGuildUserRoles(User.Id, User.Guild.Id, new List<ulong>());
            }
        }

        /// <summary>
        /// This function is fired whenever a GuildMember is updated.
        /// It tries to keep a cache of the old GuildMember object but if it can't then we look at the database to see what their old roles might be. On this part in particular I need to add safeguards if there is no DB entry.
        /// At that point it compares the roles to see if they are different; if they are then we update the database.
        /// </summary>
        /// <param name="UserBefore"></param>
        /// <param name="UserAfter"></param>
        /// <returns></returns>
        private static async Task GuildMemberUpdatedEvent(Cacheable<SocketGuildUser,ulong> UserBefore, SocketGuildUser UserAfter)
        {
            List<ulong> BeforeRoles = new List<ulong>(), AfterRoles = new List<ulong>();
            foreach (SocketRole Role in UserAfter.Roles)
            {
                AfterRoles.Add(Role.Id);
            }
            if (!UserBefore.HasValue)
            {
                if (await DBHandler.CheckUserRolesExists(UserAfter.Id, UserAfter.Guild.Id))
                {
                    BeforeRoles = await DBHandler.FetchGuildUserRoles(UserAfter.Id, UserAfter.Guild.Id);
                }
            }
            else
            {
                foreach (SocketRole Role in UserBefore.Value.Roles)
                {
                    BeforeRoles.Add(Role.Id);
                }
            }
            if (!BeforeRoles.Equals(AfterRoles))
            {
                await DBHandler.UpdateGuildUserRoles(UserAfter.Id, UserAfter.Guild.Id, AfterRoles);
            }
        }

        /// <summary>
        /// This function technically handles every single message seen by the bot.
        /// But it only triggers on that regex statement which is looking for mathematical expressions or statements of a somewhat simple degree.
        /// It then solves the expression as best it can, using the AngouriMath (FOSS) library and responds with the result.
        /// In the future this may be used for more complex things but for now this is all it does. I may also move this out into its own binary at some point.
        /// </summary>
        /// <param name="Message">The message object to process</param>
        /// <returns>task stuff idk</returns>
        private static async Task MessageEvent(SocketMessage Message)
        {
            if (Regex.IsMatch(Message.Content, "^[0-9+-/().,* ]+$") && !Message.Author.IsBot)
            {
                float Result = (float)((Entity)Message.Content).EvalNumerical();
                if (!Message.Content.Equals(Result.ToString()))
                {
                    await Message.Channel.SendMessageAsync(Result.ToString());
                }
            }
        }
    }
}
