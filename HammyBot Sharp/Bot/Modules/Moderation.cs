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

using System.Data;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace HammyBot_Sharp.Bot.Modules
{
    [Group("mod")]
    public class Moderation : ModuleBase<SocketCommandContext>
    {
        public Storage? Storage { get; set; }

        public IRole? GetMuterole(ulong muteroleId)
        {
            IRole? muterole = null;
            
            foreach (IRole role in Context.Guild.Roles)
                if (role.Id == (ulong) muteroleId)
                    muterole = role;

            return muterole;
        }
        
        [RequireUserPermission(GuildPermission.ManageMessages)]
        [Command("mute")]
        public async Task Mute(IGuildUser member)
        {
            var storage = Storage!.Get(Context.Guild.Id);
            ulong muteroleId = storage.Muterole;

            await member.AddRoleAsync(GetMuterole(muteroleId));

            await Context.Channel.SendMessageAsync(embed: Embeds.Embed(Color.Green, "Muted member",
                $"Successfully muted member {member}."));
        }

        [RequireUserPermission(GuildPermission.ManageMessages)]
        [Command("unmute")]
        public async Task Unmute(IGuildUser member)
        {
            var storage = Storage!.Get(Context.Guild.Id);
            ulong muteroleId = storage.Muterole;

            await member.RemoveRoleAsync(GetMuterole(muteroleId));
            
            await Context.Channel.SendMessageAsync(embed: Embeds.Embed(Color.Green, "Unmuted member",
                $"Successfully unmuted member {member}."));
        }
    }
}