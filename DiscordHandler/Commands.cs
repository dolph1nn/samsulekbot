using Discord;
using Discord.Net;
using Discord.WebSocket;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;

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
                .AddOption("code", ApplicationCommandOptionType.String, "The code to evaluate", isRequired: true);
            //EvalCommand.WithDefaultMemberPermissions(GuildPermission.Administrator);
            //EvalCommand.WithContextTypes(InteractionContextType.Guild | InteractionContextType.PrivateChannel | InteractionContextType.BotDm);
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
        public static async Task SlashCommandHandler(SocketSlashCommand command)
        {
            // Let's add a switch statement for the command name so we can handle multiple commands in one event.
            switch (command.Data.Name)
            {
                case "eval":
                    await EvaluateCode(command);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// THIS IS A VERY DANGEROUS COMMAND/FUNCTION!!!
        /// </summary>
        /// <param name="Code"></param>
        /// <returns></returns>
        private static async Task EvaluateCode(SocketSlashCommand command)
        {
            string code = command.Data.Options.First().Value.ToString();
            var chan = await command.GetChannelAsync();
            await chan.SendMessageAsync(code);
        }
    }
}
