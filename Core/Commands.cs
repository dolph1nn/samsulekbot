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
using System.IO;
using System.Net;
using SSB.Core.Database;

namespace SSB.Discord
{
    public static class Commands
    {
        /// <summary>
        /// This function builds the slash-commands the bot uses. You only need to run this function once per command, usually.
        /// </summary>
        /// <returns>task stuff idk</returns>
        public static async Task BuildCommands()
        {
            Console.WriteLine("Start");
            SocketGuild guild = DiscordHandler.SocketClient.GetGuild(170683612092432387);
            Console.WriteLine(guild.Name);
            SlashCommandBuilder EvalCommand = new SlashCommandBuilder()
                .WithName("evaluate")
                .WithDescription("Evaluates C# code using the Roslyn engine.")
                .AddOption("code", ApplicationCommandOptionType.String, "The code to evaluate", isRequired: true)
                .AddOption("hide", ApplicationCommandOptionType.Boolean, "Hide this command/its result from others?", isRequired: true)
                .WithDefaultMemberPermissions(GuildPermission.Administrator)
                .WithContextTypes(InteractionContextType.Guild & InteractionContextType.PrivateChannel & InteractionContextType.BotDm);
            SlashCommandBuilder ProvisionCommand = new SlashCommandBuilder()
                .WithName("provision")
                .WithDescription("Provisions a Guild for use with the bot")
                //.AddOption("guild", ApplicationCommandOptionType.String, "The guild to provision (default: this one)", isRequired: false)
                .WithDefaultMemberPermissions(GuildPermission.Administrator)
                .WithContextTypes(InteractionContextType.Guild & InteractionContextType.PrivateChannel);
            SlashCommandBuilder AddEmoteCommand = new SlashCommandBuilder()
                .WithName("addemote")
                .WithDescription("Adds an emote easily")
                .AddOption("guild", ApplicationCommandOptionType.String, "The guild to make the emote in (default: this one)", isRequired: false)
                .AddOption("name", ApplicationCommandOptionType.String, "name of the emote", isRequired: true)
                .AddOption("url", ApplicationCommandOptionType.String, "url of the emote", isRequired: false)
                .AddOption("file", ApplicationCommandOptionType.Attachment, "attachment image", isRequired: false)
                .WithDefaultMemberPermissions(GuildPermission.Administrator)
                .WithContextTypes(InteractionContextType.Guild & InteractionContextType.PrivateChannel);
            Console.WriteLine("Before Create");
            try
            {
                await DiscordHandler.SocketClient.Rest.CreateGlobalCommand(EvalCommand.Build());

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

        /// <summary>
        /// Slash Command Handler
        /// This is what determines which command function to run based on the command name
        /// </summary>
        /// <param name="command">This is the actual command object; if we didn't need to pass it onto the command function itself we could only care about the Data.Name Property to determine which command is being ran</param>
        /// <returns>task stuff idk</returns>
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
                case "addemote":
                    await AddEmoteURL(command);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// This is the function for the /provision command
        /// this provisions a guild for the bot to use certain features
        /// </summary>
        /// <param name="command">the command object; here we really only need the param that was ran. we strongly type it as a ulong because the command sees it as a string, and we need it as a ulong to do things</param>
        /// <returns>task stuff idk</returns>
        private static async Task ProvisionGuild(SocketSlashCommand command)
        {
            await command.DeferAsync();
            //Dictionary<string, SocketSlashCommandDataOption> args = command.Data.Options.ToDictionary(arg => arg.Name);
            //ulong GuildID = Convert.ToUInt64(args["guild"].Value);
            ulong GuildID = (ulong)command.GuildId;
            SocketGuild Guild = DiscordHandler.SocketClient.GetGuild((ulong)command.GuildId);
            await command.ModifyOriginalResponseAsync(msg => msg.Content = "Provisioning guild ID " + GuildID + "...");
            if (Guild is null)
            {
                await command.ModifyOriginalResponseAsync(msg => msg.Content = "Could not find Guild ID " + GuildID + "!");
            }
            else
            {
                Task<bool> tasdasd = DBHandler.CheckGuildExists(GuildID);
                if (!tasdasd.Result)
                {
                    await DBHandler.InsertNewGuild(GuildID);
                    const string ProvMsg = "Guild provisioned, now provisioning users...";
                    string ProvMsg2 = ProvMsg;
                    await command.ModifyOriginalResponseAsync(msg => msg.Content = ProvMsg2);
                    int UserCount = Guild.MemberCount, Progress = 0;
                    float ProgressPct;
                    foreach (SocketGuildUser User in Guild.Users)
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
                            await DBHandler.InsertGuildUserRoles(User.Id, User.Guild.Id, Roles);
                        }
                        if (++Progress > UserCount) { Progress = UserCount; } ProgressPct = Progress/UserCount;
                        ProvMsg2 = ProvMsg + " " + UpdateProgressBar(ProgressPct) + "("+Progress+" out of " + UserCount + ")";
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
        }

        /// <summary>
        /// this function produces a progress bar that looks like [=   ] [==  ] [=== ] [====]
        /// based on a float between 0 and 1 - if its over 1 somehow we divide by 100
        /// any improvements to this function are greatly welcomed
        /// </summary>
        /// <param name="pct">the string of the progress bar</param>
        /// <returns>a string </returns>
        private static string UpdateProgressBar(float pct)
        {
            if (pct > 1) { pct /= 100; }

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

            return "[" + prog1 + prog2 + prog3 + prog4 + prog5 + prog6 + prog7 + prog8 + prog9 + prog10 + "]";
        }


        /// <summary>
        /// This method creates an emote from a URL.
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        private static async Task AddEmoteURL(SocketSlashCommand command)
        {
            await command.DeferAsync();
            Dictionary<string, SocketSlashCommandDataOption> args = command.Data.Options.ToDictionary(arg => arg.Name);
            await DiscordHandler.SocketClient.GetGuild((ulong)command.GuildId).CreateEmoteAsync((string)args["name"].Value, new Image(new MemoryStream(new WebClient().DownloadData((string)args["url"].Value))));
            await command.ModifyOriginalResponseAsync(msg => msg.Content = "Added emote " + (string)args["name"].Value + " from URL " + (string)args["url"].Value);
        }

        /// <summary>
        /// THIS IS A VERY DANGEROUS COMMAND/METHOD!!!
        /// This is the Method for the Evaulate command.
        /// This creates a C# script using the Roslyn engine using the command's input and runs it, then replies to the user based on the script's output or a compilation error if there is one.
        /// </summary>
        /// <param name="command">The commnand object.</param>
        /// <returns>task stuff idk</returns>
        private static async Task EvaluateCode(SocketSlashCommand command)
        {
            //Converts arguments into Dictionary so we can just args["argname"] to get an argument without having to know what order its in or anything like that. Maybe this is not necessary but it's nice.
            Dictionary<string, SocketSlashCommandDataOption> args = command.Data.Options.ToDictionary(arg => arg.Name);
            //Defers the response, if they want to make it ephemeral or only shown to them, they answer true to the hide arg
            await command.DeferAsync((bool)args["hide"].Value);
            //right now this checks for my userID, i need to make a database table or something for it
            if (command.User.Id == 170679185650614272)
            {
                //Edits original response
                await command.ModifyOriginalResponseAsync(msg => msg.Content = "Evaluating...");
                //Declares our string variable that will be either filled with the result of the script or CompilationErrorException
                string fin;
                //Surround this in a try/catch block in case there's a CompilationErrorException
                try
                {
                    //evaluate code and store the result in a string object, we pass in the SocketClient object so the script can interact with the bot's API
                    fin = await CSharpScript.EvaluateAsync<string>((string)args["code"].Value, ScriptOptions.Default.WithImports(new string[] { "System", "Discord", "Discord.Net", "Discord.Websocket" }), DiscordHandler.SocketClient);
                }
                catch (CompilationErrorException Ex)
                {
                    fin = Ex.Message;
                }
                //Edit original response with the evaluated code or CompilationErrorException
                await command.ModifyOriginalResponseAsync(msg => msg.Content = fin);
            }
            else
            {
                await command.ModifyOriginalResponseAsync(msg => msg.Content = "WARNING: You have attempted to use a dangerous command only intended for developers." +
                    "I'll give you the benefit of the doubt that this was a genuine mistake. If you do this again, you will permanently lose access to ALL of my commands.");
            }

        }
    }
}
