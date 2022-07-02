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
using HammyBot_Sharp;

namespace HammyBot_Sharp.Bot.Modules
{
    [Group("covid")]
    class Covid19 : ModuleBase<SocketCommandContext>
    {
        [Command("overview")]
        [Summary("Gives a short overview of COVID-19 in Victoria")]
        public async Task Overview()
        {
            var embed = new EmbedBuilder();
            await ReplyAsync(embed: Embeds.Embed(Color.Blue,
                "COVID-19 overview for Victoria",
                $"**Active cases:** {Covid19Service.GetActiveCases()}",
                $"**New cases:** {Covid19Service.GetNewCases()}"));

            await Context.Channel.SendFileAsync(Covid19Service.ModellingImage);
        }

        [Command("overviewByPostcode")]
        [Summary("Gives an overview of COVID-19 in your suburb by postcode")]
        public async Task OverviewByPostcode(int postcode)
        {
            await ReplyAsync(embed: Embeds.Embed(Color.Blue,
                $"COVID-19 overview for {postcode}",
                $"**Active cases:** {Covid19Service.GetActiveCasesPostcode(postcode)}",
                $"**New cases:** {Covid19Service.GetNewCasesPostcode(postcode)}"));
        }
    }
}
