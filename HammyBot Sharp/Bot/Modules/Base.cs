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

        [Command("echo")]
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
                await Context.Channel.SendMessageAsync(
                    embed: Embeds.Embed(Color.Green, "Sent DM to user!")
                );
            }
        }

        [Command("ping")]
        public async Task Ping()
        {
            var message = await Context.Channel.SendMessageAsync("_ _");
            await message.DeleteAsync();
            
            long messageLatency = message.Timestamp.ToUnixTimeMilliseconds() - Context.Message.Timestamp.ToUnixTimeMilliseconds();
            
            await Context.Channel.SendMessageAsync(
                embed: Embeds.Embed(
                    Color.Blue,
                    "Pong!",
                    "`API Latency:".PadRight(15) + $"{Client!.Latency.ToString()}ms`",
                    "`Msg Latency:".PadRight(15) + $"{messageLatency.ToString()}ms`"
                )
            );
        }
    }
}