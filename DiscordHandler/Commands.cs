using Discord;
using Discord.Net;
using Discord.WebSocket;
using Newtonsoft.Json;
using System;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.CodeAnalysis.Scripting;
using System.Collections.Generic;

namespace SSB.Discord
{
    public static class Commands
    {
        public static async Task BuildCommands()
        {
            Console.WriteLine("Start");
            SocketGuild guild = DiscordHandler.SocketClient.GetGuild(170683612092432387);
            Console.WriteLine(guild.Name);
            SlashCommandBuilder EvalCommand = new SlashCommandBuilder()
                .WithName("evaluate")
                .WithDescription("Evaluates C# code using the Roslyn engine.")
                .AddOption("code", ApplicationCommandOptionType.String, "The code to evaluate", isRequired: true)
                .WithDefaultMemberPermissions(GuildPermission.Administrator)
                .WithContextTypes(InteractionContextType.Guild & InteractionContextType.PrivateChannel & InteractionContextType.BotDm);
            SlashCommandBuilder ProvisionCommand = new SlashCommandBuilder()
                .WithName("provision")
                .WithDescription("Provisions a Guild for use with the bot")
                .AddOption("guild", ApplicationCommandOptionType.Integer, "The guild to provision (default: this one)", isRequired: true)
                .WithDefaultMemberPermissions(GuildPermission.Administrator)
                .WithContextTypes(InteractionContextType.Guild & InteractionContextType.PrivateChannel);
            Console.WriteLine("Before Create");
            try
            {
                await DiscordHandler.SocketClient.Rest.CreateGlobalCommand(ProvisionCommand.Build());

            }
            catch (HttpException exception)
            {
                // If our command was invalid, we should catch an ApplicationCommandException. This exception contains the path of the error as well as the error message. You can serialize the Error field in the exception to get a visual of where your error is.
                var json = JsonConvert.SerializeObject(exception.Errors, Formatting.Indented);

                // You can send this error somewhere or just print it to the console, for this example we're just going to print it.
                Console.WriteLine(json);
            }
            Console.WriteLine("Done!");
        }
        public static async Task SlashCommandHandler(SocketSlashCommand command)
        {
            // Let's add a switch statement for the command name so we can handle multiple commands in one event.
            switch (command.Data.Name)
            {
                case "evaluate":
                    await EvaluateCode(command);
                    break;
                case "provision":
                    await ProvisionGuild(command);
                    break;
                default:
                    break;
            }
        }

        private static async Task ProvisionGuild(SocketSlashCommand command)
        {
            ulong GuildID = (ulong)command.Data.Options.First().Value;
            SocketGuild Guild = DiscordHandler.SocketClient.GetGuild(GuildID);
            await command.RespondAsync("Provisioning guild ID " + GuildID + "...");
            if (Guild == null) 
            {
                if (!Database.DBHandler.CheckGuildExists(GuildID))
                {
                    await Database.DBHandler.InsertNewGuild(GuildID);
                    const string ProvMsg = "Guild provisioned, now provisioning users...";
                    string ProvMsg2 = ProvMsg;
                    await command.ModifyOriginalResponseAsync(msg => msg.Content = ProvMsg2);
                    int UserCount = Guild.MemberCount, Progress = 0;
                    float ProgressPct;
                    foreach(SocketGuildUser User in Guild.Users)
                    {
                        if (!User.IsBot)
                        {
                            List<ulong> Roles = new List<ulong>();
                            foreach (SocketRole UserRole in User.Roles)
                            {
                                if (!UserRole.IsEveryone)
                                {
                                    Roles.Add(UserRole.Id);
                                }
                            } 
                            await Database.DBHandler.InsertGuildUserRoles(User.Id, User.Guild.Id, Roles);
                        }
                        Progress++; ProgressPct = Progress/UserCount; ProvMsg2 = ProvMsg + " " + UpdateProgressBar(ProgressPct) + "("+Progress+" out of " + UserCount + ")";
                        await command.ModifyOriginalResponseAsync(msg => msg.Content = ProvMsg2);
                    }
                    ProvMsg2 = ProvMsg2 + "... Finished with Guild!";
                    await command.ModifyOriginalResponseAsync(msg => msg.Content = ProvMsg2);
                }
                else
                {
                    await command.ModifyOriginalResponseAsync(msg => msg.Content = "That guild already exists!");
                }
            }
            else
            {
                await command.ModifyOriginalResponseAsync(msg => msg.Content = "Could not find Guild ID " + GuildID + "!");
            }
        }

        private static string UpdateProgressBar(float pct)
        {
            string prog1 = "=", prog2 = "=", prog3 = "=", prog4 = "=", prog5 = "=", prog6 = "=", prog7 = "=",prog8 = "=", prog9 = "=", prog10 = "=";
            if (pct < .1)
            {
                prog1 = " "; prog2 = " "; prog3 = " "; prog4 = " "; prog5 = " "; prog6 = " "; prog7 = " "; prog8 = " "; prog9 = " "; prog10 = " ";
            }
            else if (pct >= .1 && pct < .2)
            {
                prog2 = " "; prog3 = " "; prog4 = " "; prog5 = " "; prog6 = " "; prog7 = " "; prog8 = " "; prog9 = " "; prog10 = " ";
            }
            else if (pct >= .2 && pct < .3)
            {
                prog3 = " "; prog4 = " "; prog5 = " "; prog6 = " "; prog7 = " "; prog8 = " "; prog9 = " "; prog10 = " ";
            }
            else if (pct >= .3 && pct < .4)
            {
                prog4 = " "; prog5 = " "; prog6 = " "; prog7 = " "; prog8 = " "; prog9 = " "; prog10 = " ";
            }
            else if (pct >= .4 && pct < .5)
            {
                prog5 = " "; prog6 = " "; prog7 = " "; prog8 = " "; prog9 = " "; prog10 = " ";
            }
            else if (pct >= .5 && pct < .6)
            {
                prog6 = " "; prog7 = " "; prog8 = " "; prog9 = " "; prog10 = " ";
            }
            else if (pct >= .6 && pct < .7)
            {
                prog7 = " "; prog8 = " "; prog9 = " "; prog10 = " ";
            }
            else if (pct >= .7 && pct < .8)
            {
                prog8 = " "; prog9 = " "; prog10 = " ";
            }
            else if (pct >= .8 && pct < .9)
            {
                prog9 = " "; prog10 = " ";
            }
            else if (pct >= .9 && pct < 1)
            {
                prog10 = " ";
            }
            string final = "["+prog1+prog2+prog3+prog4+prog5+prog6+prog7+prog8+prog9+prog10+"]"; // [==========]

            return final;
        }

        /// <summary>
        /// THIS IS A VERY DANGEROUS COMMAND/FUNCTION!!!
        /// </summary>
        /// <param name="Code"></param>
        /// <returns></returns>
        private static async Task EvaluateCode(SocketSlashCommand command)
        {
            if (command.User.Id == 170679185650614272)
            {
                string[] Imports = new string[] { "System", "Discord", "Discord.Net", "Discord.Websocket"};
                //await command.RespondAsync("Evaluating...");
                string Code = command.Data.Options.First().Value.ToString();
                string fin = await CSharpScript.EvaluateAsync<string>(Code, ScriptOptions.Default.WithImports("System"));
                await command.RespondAsync(fin);
            }
            else
            {
                await command.RespondAsync("WARNING: You have attempted to use a dangerous command only intended for developers." +
                    "I'll give you the benefit of the doubt that this was a genuine mistake. If you do this again, you will permanently lose access to ALL" +
                    "of my commands.");
            }

        }
    }
}
