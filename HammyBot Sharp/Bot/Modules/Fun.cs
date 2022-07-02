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

using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace HammyBot_Sharp.Bot.Modules
{
    class Fun : ModuleBase<SocketCommandContext>
    {
        public DiscordSocketClient? Client { get; set; }

        [Command("ship")]
        [Summary("Combines two people's names and figures out how good of a match they are.")]
        public async Task Ship(string person1, string person2)
        {
            var random = new Random();

            string output = person1.Substring(0, person1.Length / 2) + person2.Substring(person2.Length / 2);
            int randomNumber = random.Next(0, 100);

            await ReplyAsync(
                embed: Embeds.Embed(Color.Magenta, "sussy baka", $"**{output}**", $"Match percentage: **{randomNumber}%**"));
        }

        [Command("chrissy")]
        [Summary("chrissy, the most winny of winny and a very refined one too.")]
        public async Task Chrissy(string chrissy = "chrissy", string winny = "winny")
        {
            await ReplyAsync($"{chrissy}, the most {winny} of {winny} and a very refined one too. No words can describe or convey how {winny} {chrissy} is. {chrissy} has such superior refinement that he is able to {winny} at everything he does.");
        }

        [Command("emoji")]
        [Summary("Gets an emoji with the same name from any server the bot is in.")]
        public async Task Emoji(string emoteName)
        {
            string emote = "<:perhaps:880067352639733760>";

            foreach (SocketGuild guild in Client!.Guilds)
                foreach(Emote emoteLoop in guild.Emotes)
                {
                    if (emoteLoop.Name.ToLower() == emoteName.ToLower())
                    {
                        emote = emoteLoop.ToString();
                        goto Breakpoint;
                    }
                }
            Breakpoint:

            await ReplyAsync(emote);
        }

        [Command("aaa")]
        [Summary("Sends the aaa emoji.")]
        public async Task Aaa()
        {
            await Context.Message.DeleteAsync();
            await ReplyAsync("<:aaa:893439758716514314>");
        }

        [Command("fesd")]
        [Summary("Sends the fesd emoji.")]
        public async Task Fesd()
        {
            await Context.Message.DeleteAsync();
            await ReplyAsync("<:fesd:886560001110458378>");
        }
    }
}
