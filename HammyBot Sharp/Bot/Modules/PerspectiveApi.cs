
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

using Discord.Commands;
using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HammyBot_Sharp.Bot.Modules
{
    [Group("toxic")]
    [RequireUserPermission(GuildPermission.ManageGuild)]
    class PerspectiveApi : ModuleBase<SocketCommandContext>
    {
        public Storage? Storage { get; set; }

        [Command("setup")]
        [RequireUserPermission(GuildPermission.ManageGuild)]
        [Summary("Sets up the configuration for the Perspective API. Takes the reporting channel and the threshold.")]
        public async Task Setup(IMessageChannel channel, int threshold)
        {
            var conf = Storage!.Get(Context.Guild.Id);

            conf.PerspectiveApi = new PerspectiveApiConfig
            {
                Threshold = threshold/100.0m, 
                ReportChannelId = channel.Id, 
            };

            await ReplyAsync(embed: Embeds.Embed(Color.Green, "Set up the Perspective AI to track toxicity", $"Reporting channel: {channel}", $"Threshold: {threshold/100:P}"));

            Storage!.Set(Context.Guild.Id, conf);
        }

        [Command("setChannel")]
        [RequireUserPermission(GuildPermission.ManageGuild)]
        [Summary("Sets the reporting channel.")]
        public async Task SetChannel(IMessageChannel channel)
        {
            var conf = Storage!.Get(Context.Guild.Id);

            await ReplyAsync(embed: Embeds.Embed(Color.Green, "Set the AI's reporting channel"));

            Storage!.Set(Context.Guild.Id, conf);
        }

        [Command("setThreshold")]
        [RequireUserPermission(GuildPermission.ManageGuild)]
        [Summary("Sets the threshold.")]
        public async Task SetThreshold(int threshold)
        {
            var conf = Storage!.Get(Context.Guild.Id);

            conf.PerspectiveApi!.Threshold = threshold/100.0m;

            await ReplyAsync(embed: Embeds.Embed(Color.Green, "Set the AI's threshold"));

            Storage!.Set(Context.Guild.Id, conf);
        }

        [Command("disable")]
        [RequireUserPermission(GuildPermission.ManageGuild)]
        [Summary("Disables the Perspective API for your guild.")]
        public async Task Disable()
        {
            var conf = Storage!.Get(Context.Guild.Id);

            conf.PerspectiveApi = null;

            await ReplyAsync(embed: Embeds.Embed(Color.Green, "Disabled the AI", "You will need to set it up again if you want to use it again"));

            Storage!.Set(Context.Guild.Id, conf);
        }

        [Command("getConfig")]
        public async Task GetConfig()
        {
            var conf = Storage!.Get(Context.Guild.Id);

            if (conf.PerspectiveApi == null)
                await ReplyAsync("Perspective API not set up");
            else
                await ReplyAsync(embed: Embeds.Embed(Color.Green, "Config options for Perspective AI", $"**Threshold**: {conf.PerspectiveApi!.Threshold:P}", $"**Report Channel ID**: {conf.PerspectiveApi.ReportChannelId}"));
        }
    }
}
