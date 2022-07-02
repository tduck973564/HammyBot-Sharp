// HammyBot Sharp - HammyBot Sharp
//     Copyright (C) 2021 Thomas Duckworth <tduck973564@gmail.com>
// 
//     This program is free software: you can redistribute it and/or modify
//     it under the terms of the GNU General Public License as published by
//     the Free Software Foundation, either version 3 of the License, or
//     (at your option) any later version.
// 
//     This program is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU General Public License for more details.
// 
//     You should have received a copy of the GNU General Public License
//     along with this program.  If not, see <https://www.gnu.org/licenses/>.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace HammyBot_Sharp.Bot.Modules
{
    public class Base : ModuleBase<SocketCommandContext>
    {
        public Config? Config { get; set; }
        public DiscordSocketClient? Client { get; set; }
        public CommandService? _commands { get; set; }

        [Command("help")]
        [Summary("Lists the possible commands.")]
        public async Task Help()
        {
            var modules = _commands!.Modules.ToList();
            string text = "Modules:\n";

            foreach (ModuleInfo module in modules)
            {
                text += $" {module.Name}\n";
            }

            await ReplyAsync($"```{text}```");
        }

        [Command("help")]
        [Summary("Lists the possible commands.")]
        public async Task Help(string moduleName)
        {
            var modules = _commands!.Modules.ToList();
            string text = $"Commands in module {moduleName}:\n";

            var module = modules.First(x => x.Name == moduleName);

            if (module != null)
            {
                foreach (CommandInfo command in module.Commands.ToList())
                {
                    string summary = command.Summary ?? "No description available\n";

                    text += $"\t{command.Name}: {summary}\n";
                }

                await ReplyAsync($"```{text}```");
            }
            else await ReplyAsync("Module not found. Use help command without arguments to see list of modules.");
        }

        [Command("echo")]
        [Summary("Sends a message in the channel it was called from or to a user.")]
        public async Task Echo(string echo, IUser? user = null)
        {
            if (user == null)
            {
                await Context.Message.DeleteAsync();
                await Context.Channel.SendMessageAsync(echo);
            }
            else
            {
                await user.SendMessageAsync(echo);
                await ReplyAsync(
                    embed: Embeds.Embed(Color.Green, "Sent DM to user!")
                );
            }
        }

        [Command("ping")]
        [Summary("Gets the message and API latency of the bot.")]
        public async Task Ping()
        {
            var message = await Context.Channel.SendMessageAsync("_ _");
            await message.DeleteAsync();

            long messageLatency = message.Timestamp.ToUnixTimeMilliseconds() - Context.Message.Timestamp.ToUnixTimeMilliseconds();

            await ReplyAsync(
                embed: Embeds.Embed(
                    Color.Blue,
                    "Pong!",
                    "`API Latency:".PadRight(15) + $"{Client!.Latency}ms`",
                    "`Msg Latency:".PadRight(15) + $"{messageLatency}ms`"
                )
            );
        }

        [Command("avatar")]
        [Summary("Sends your (or the user you specify's) avatar.")]
        public async Task Avatar(IUser? user = null)
        {
            user ??= Context.User;

            await ReplyAsync(user.GetAvatarUrl());
        }
    }
}