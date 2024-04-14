﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Newtonsoft.Json;

namespace SSB.Discord
{
    public static class DiscordHandler
    {
        private static DiscordSocketClient SocketClient;
        private static SSBConfig Config;

        public static async Task Init()
        {
            SocketClient = new DiscordSocketClient(new DiscordSocketConfig { /*MessageCacheSize = 100, */GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.MessageContent | GatewayIntents.GuildMembers });
            Config = JsonConvert.DeserializeObject<SSBConfig>(File.ReadAllText(@"D:\Projects\samsulekbot\cfg.json"));
            await SocketClient.LoginAsync(TokenType.Bot, Config.Token);
            await SocketClient.StartAsync();
            SocketClient.UserJoined += UserJoinGuildEvent;
            SocketClient.GuildMemberUpdated += GuildMemberUpdatedEvent;
            SocketClient.Ready += ReadyEvent;

            //await SendStartupMessage("<@1140006616800956529> <@217097093226037249> https://tenor.com/view/sam-sulek-sam-sulek-pump-pound-town-gif-11308748596026110159");
            await Task.Delay(-1);
        }

        public static async Task Stop()
        {
            await SocketClient.LogoutAsync();
            await SocketClient.StopAsync();
        }

        private static async Task SendStartupMessage(string message)
        {
            var channel = await SocketClient.GetChannelAsync(200075640739725312) as IMessageChannel;
            await channel.SendMessageAsync(message);
        }
        private static async Task SendStartupMessage(string message, ulong channelid)
        {
            var channel = await SocketClient.GetChannelAsync(channelid) as IMessageChannel;
            await channel.SendMessageAsync(message);
        }

        private static async Task InventoryGuild(SocketGuild Guild)
        {
            await SendStartupMessage("Inventorying Guild ID " + Guild.Id, 430528035247095818);
            await Guild.DownloadUsersAsync();
            foreach(SocketGuildUser guildUser in Guild.Users) 
            {
                if (!guildUser.IsBot)
                {
                    await SendStartupMessage("User " + guildUser.Id + " Name " + guildUser.DisplayName, 430528035247095818);
                    await SendStartupMessage("Has Roles:", 430528035247095818);
                    foreach (SocketRole Role in guildUser.Roles)
                    {
                        if (!Role.IsEveryone)
                        {
                            await SendStartupMessage(Role.Id + " Name: " + Role.Name, 430528035247095818);
                        }
                    }
                }

            }
            return;
        }

        private static async Task ReadyEvent()
        {
            Console.WriteLine("Bot is connected!");
            //await InventoryGuild(SocketClient.GetGuild(170683612092432387));
            await Database.DBHandler.OpenConnection();
            await SendStartupMessage("Connected to Database!", 430528035247095818);
            return;
        }

        private static async Task UserJoinGuildEvent(SocketGuildUser SGU)
        {
            if (Database.DBHandler.CheckUserRolesExists(SGU.Id, SGU.Guild.Id))
            {
                await SGU.AddRolesAsync(Database.DBHandler.FetchGuildUserRoles(SGU.Id, SGU.Guild.Id));
            } 
            else
            {
                await Database.DBHandler.InsertGuildUserRoles(SGU.Id, SGU.Guild.Id, new List<ulong>());
            }
        }

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
            if (BeforeRoles.Equals(AfterRoles))
            {
                await Database.DBHandler.UpdateUserRoles(UserAfter.Id, UserAfter.Guild.Id, AfterRoles);
            }
        }
    }

    class SSBConfig
    {
        public String Token;
    }
}
