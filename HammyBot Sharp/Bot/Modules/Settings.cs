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

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace HammyBot_Sharp.Bot.Modules
{
    [Group("settings")]
    public class Settings : ModuleBase<SocketCommandContext>
    {
        public Storage? Storage { get; set; }

        [RequireUserPermission(GuildPermission.ManageGuild)]
        [Command("set")]
        [Summary("Sets a setting specified by the key to the value you input. This probably doesn't work properly, and is a horrible abuse of the type system.")]
        public async Task Set(string key, string value)
        {
            var storage = Storage!.Get(Context.Guild.Id);
            object convertedValue = Convert.ChangeType(value, storage.Get(key)!.GetType());

            storage.Set(key, convertedValue);
            Storage.Set(Context.Guild.Id, storage);
                
            await Context.Channel.SendMessageAsync(
                embed: Embeds.Embed(Color.Green, $"Set {key} to {value}.")
            );
        }

        [Command("get")]
        [Summary("Gets a setting by key.")]
        public async Task Get(string key)
        {
            await Context.Channel.SendMessageAsync(
                embed: Embeds.Embed(
                    Color.Blue, 
                    "Setting got!", 
                    Storage!.Get(Context.Guild.Id).Get(key)?.ToString() ?? $"Setting \"{key}\" does not exist or it is not set."
                )
            );
        }
    }
}