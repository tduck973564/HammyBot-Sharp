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
using System.Data;
using System.Linq;
using System.Threading;
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
                if (role.Id == muteroleId)
                    muterole = role;

            return muterole;
        }

        [RequireUserPermission(GuildPermission.ManageMessages)]
        [Command("mute")]
        [Summary("Assigns the muterole to a user.")]
        public async Task Mute(IGuildUser member, string reason)
        {
            var storage = Storage!.Get(Context.Guild.Id);
            ulong muteroleId = storage.Muterole;

            await member.AddRoleAsync(GetMuterole(muteroleId));

            await ReplyAsync(embed: Embeds.Embed(Color.Green, "Muted member",
                $"Successfully muted member {member}."));
        }

        [RequireUserPermission(GuildPermission.ManageMessages)]
        [Command("unmute")]
        [Summary("Removes the muterole from a user.")]
        public async Task Unmute(IGuildUser member, string reason)
        {
            var storage = Storage!.Get(Context.Guild.Id);
            ulong muteroleId = storage.Muterole;

            await member.RemoveRoleAsync(GetMuterole(muteroleId));

            await ReplyAsync(embed: Embeds.Embed(Color.Green, "Unmuted member",
                $"Successfully unmuted member {member}."));
        }

        [RequireUserPermission(GuildPermission.ManageGuild)]
        [Command("muterole")]
        [Summary("Sets the muterole to the role specified.")]
        public async Task Muterole(IRole role)
        {
            var storage = Storage!.Get(Context.Guild.Id);

            storage.Muterole = role.Id;

            await ReplyAsync(embed: Embeds.Embed(Color.Green, "Set muterole", $"Set muterole to {role}."));
        }

        [RequireUserPermission(GuildPermission.ManageRoles)]
        [Command("muteAll")]
        [Summary("Mutes all users in the server")]
        public async Task MuteAll(string reason)
        {
            var storage = Storage!.Get(Context.Guild.Id);
            ulong muteroleId = storage.Muterole;
            var muterole = GetMuterole(muteroleId);

            await ReplyAsync(embed: Embeds.Embed(Color.Blue, "Muting all users", "This may take a while."));

            string outErr = "";

            await foreach (IGuildUser user in Context.Guild.GetUsersAsync())
            {
                try { await user.AddRoleAsync(muterole); }
                catch (Exception e)
                {
                    outErr += $"`{user}:".PadRight(15) + $"{e.Message}\n`";
                }
            }

            await ReplyAsync(embed: Embeds.Embed(Color.Green, "Muted members", $"Could not mute members:\n{outErr}"));
        }

        [RequireUserPermission(GuildPermission.ManageNicknames)]
        [Command("unmuteAll")]
        [Summary("Unmutes all users in the server")]
        public async Task UnmuteAll(string reason)
        {
            var storage = Storage!.Get(Context.Guild.Id);
            ulong muteroleId = storage.Muterole;
            var muterole = GetMuterole(muteroleId);

            await ReplyAsync(embed: Embeds.Embed(Color.Blue, "Unmuting all users", "This may take a while."));

            string outErr = "";

            await foreach (IGuildUser user in Context.Guild.GetUsersAsync())
            {
                try { await user.RemoveRoleAsync(muterole); }
                catch (Exception e)
                {
                    outErr += $"`{user}:".PadRight(15) + $"{e.Message}\n`";
                }
            }

            await ReplyAsync(embed: Embeds.Embed(Color.Green, "Unmuted members", $"Could not unmute members:\n{outErr}"));
        }

        [RequireUserPermission(GuildPermission.ManageNicknames)]
        [Command("setAllNicks")]
        [Summary("Sets or resets the nickname of everyone in the server")]
        public async Task SetAllNicks(string reason, string? nick = null)
        {
            await ReplyAsync(embed: Embeds.Embed(Color.Blue, "Setting nick of all users", "This may take a while."));

            string outErr = "";
            
            foreach (IGuildUser user in await Context.Guild.GetUsersAsync().FlattenAsync())
            {
                try 
                {
                    await user.ModifyAsync(x => x.Nickname = nick);
                }
                catch (Exception e)
                {
                    outErr += $"`{user}:".PadRight(15) + $"{e.Message}\n`";
                }
            }

            await ReplyAsync(embed: Embeds.Embed(
                Color.Green, "Set all users' nick", $"Could not set nick of members:\n{outErr}"));
        }

        [RequireUserPermission(GuildPermission.ManageMessages)]
        [Command("purge")]
        [Summary("Deletes a set amount of messages")]
        public async Task Purge(string reason, int amount = 1)
        {
            await Context.Channel.DeleteMessageAsync(Context.Message);

            var messages = (await Context.Channel.GetMessagesAsync(Context.Message, Direction.Before, amount)
                .FlattenAsync())
                .ToList()
                .Where(x => (DateTimeOffset.UtcNow - x.Timestamp).TotalDays <= 14);

            await (Context.Channel as ITextChannel)!.DeleteMessagesAsync(messages);

            var msg = await ReplyAsync(embed: Embeds.Embed(Color.Green, $"Purged {amount} messages."));
        }

        [RequireUserPermission(GuildPermission.ManageMessages)]
        [Command("purge")]
        [Summary("Deletes a set amount of messages")]
        public async Task Purge(IGuildUser member, string reason, int amount = 1)
        {
            await Context.Channel.DeleteMessageAsync(Context.Message);

            var messages = (await Context.Channel.GetMessagesAsync(Context.Message, Direction.Before, amount)
                .FlattenAsync())
                .ToList()
                .Where(x => (DateTimeOffset.UtcNow - x.Timestamp).TotalDays <= 14 && member == x.Author);

            await (Context.Channel as ITextChannel)!.DeleteMessagesAsync(messages);

            var msg = await ReplyAsync(embed: Embeds.Embed(Color.Green, $"Purged {amount} messages from {member}."));
        }
    }
}