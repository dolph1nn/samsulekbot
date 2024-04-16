﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Discord;
using AngouriMath;
using Discord.WebSocket;
using Newtonsoft.Json;

namespace SSB.Discord
{
    public static class DiscordHandler
    {
        public static DiscordSocketClient SocketClient;
        private static SSBConfig Config;

        //public static DiscordSocketClient GetClient() {  return SocketClient; }
        public static SSBConfig GetConfig() {  return Config; }

        /// <summary>
        /// starts the discord handler
        /// </summary>
        /// <returns>task stuff idk</returns>
        public static async Task Init()
        {
            SocketClient = new DiscordSocketClient(new DiscordSocketConfig { /*MessageCacheSize = 100, */GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.MessageContent | GatewayIntents.GuildMembers });
            Config = JsonConvert.DeserializeObject<SSBConfig>(File.ReadAllText(@"D:\Projects\samsulekbot\cfg.json"));
            await SocketClient.LoginAsync(TokenType.Bot, Config.Token);
            await SocketClient.StartAsync();
            SocketClient.UserJoined += UserJoinGuildEvent;
            SocketClient.GuildMemberUpdated += GuildMemberUpdatedEvent;
            SocketClient.SlashCommandExecuted += Commands.SlashCommandHandler;
            SocketClient.MessageReceived += MessageEvent;
            SocketClient.Ready += ReadyEvent;

            //await SendStartupMessage("<@1140006616800956529> <@217097093226037249> https://tenor.com/view/sam-sulek-sam-sulek-pump-pound-town-gif-11308748596026110159");
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
            await Database.DBHandler.OpenConnection();
            await SendStartupMessage("Connected to Database!", 430528035247095818);
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
            if (Database.DBHandler.CheckUserRolesExists(User.Id, User.Guild.Id))
            {
                await User.AddRolesAsync(Database.DBHandler.FetchGuildUserRoles(User.Id, User.Guild.Id));
            } 
            else
            {
                await Database.DBHandler.InsertGuildUserRoles(User.Id, User.Guild.Id, new List<ulong>());
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
                if (Database.DBHandler.CheckUserRolesExists(UserAfter.Id, UserAfter.Guild.Id))
                {
                    BeforeRoles = Database.DBHandler.FetchGuildUserRoles(UserAfter.Id, UserAfter.Guild.Id);
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
                await Database.DBHandler.UpdateUserRoles(UserAfter.Id, UserAfter.Guild.Id, AfterRoles);
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
                await Message.Channel.SendMessageAsync(((float)((Entity)Message.Content).EvalNumerical()).ToString());
            }
        }
    }
    /// <summary>
    /// This is the class for the Config file.
    /// </summary>
    public class SSBConfig
    {
        public string Token = String.Empty;
    }
}
