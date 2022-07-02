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
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace HammyBot_Sharp.Bot.Modules
{
    [Group("emojitools")]
    class EmojiTools : ModuleBase<SocketCommandContext>
    {
        public DiscordSocketClient? Client { get; set; }

        [Command("clone")]
        [RequireUserPermission(GuildPermission.ManageGuild)]
        [Summary("Takes all the emojis from a certain guild (using it's ID) and copies them into the current server. The bot must be in the specified server.")]
        public async Task Clone(ulong guildId)
        {
            await ReplyAsync(embed: Embeds.Embed(Color.Blue, "This may take a while."));

            var httpClient = new HttpClient();
            var guild = Client!.Guilds.ToList().FirstOrDefault(x => x.Id == guildId);

            foreach (Emote emote in guild!.Emotes)
            {
                await Context.Guild.CreateEmoteAsync(emote.Name, new Image(await httpClient.GetStreamAsync(emote.Url)));
            }

            await ReplyAsync(embed: Embeds.Embed(Color.Green, "Added emojis", "Added emojis from the other server."));
        }

        [Command("clear")]
        [RequireUserPermission(GuildPermission.ManageGuild)]
        [Summary("Clears all emojis in a server")]
        public async Task Clear()
        {
            await ReplyAsync(embed: Embeds.Embed(Color.Blue, "This may take a while."));

            foreach (Emote emote in Context.Guild.Emotes)
            {
                await Context.Guild.DeleteEmoteAsync((GuildEmote)emote);
            }

            await ReplyAsync(embed: Embeds.Embed(Color.Green, "Cleared emojis", "Cleared all the emojis from this server."));
        }
    }
}
