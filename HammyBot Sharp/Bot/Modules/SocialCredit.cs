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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HammyBot_Sharp.Bot.Modules
{
    [Group("socialcr")]
    class SocialCreditCommands : ModuleBase<SocketCommandContext>
    {
        public Storage? Storage { get; set; }

        private void AddIfNotExists(ulong guild, ulong user)
        {
            var conf = Storage!.Get(guild);

            if (conf.SocialCredit == null)
                conf.SocialCredit = new Dictionary<ulong, decimal>();

            decimal _;

            if (!conf.SocialCredit!.TryGetValue(user, out _))
                conf.SocialCredit!.Add(user, 1000);

            Storage.Set(guild, conf);
        }

        private void AddIfNotExists(ulong guild)
        {
            var conf = Storage!.Get(guild);

            if (conf.SocialCredit == null)
                conf.SocialCredit = new Dictionary<ulong, decimal>();

            Storage.Set(guild, conf);
        }

        private string CreditRating(decimal amount)
        {
            return amount switch
            {
                >= 1050 => "AAA",
                >= 1030 => "AA",
                >= 1000 => "A+",
                >= 960  => "A-",
                >= 850  => "B",
                >= 600  => "C",
                 < 600  => "D",
            };
        }

        [Command("changeBal")]
        [Summary("Changes a user's Tesco Clubcard point balance.")]
        [RequireUserPermission(GuildPermission.ManageMessages)]
        public async Task ChangeBal(decimal amount, IGuildUser user)
        {
            var conf = Storage!.Get(Context.Guild.Id);

            AddIfNotExists(Context.Guild.Id, user.Id);

            conf.SocialCredit![user.Id] += amount;

            await ReplyAsync(
                embed: Embeds.Embed(Color.Red, 
                $":flag_gb: Modified {user}'s tesco clubcard poin' scawe. :flag_gb:", 
                $"Changed 'heir scawe by {amount}.", 
                $"Their scawe is nah {conf.SocialCredit![user.Id]} an' 'heir ra'in is nah {CreditRating(conf.SocialCredit![user.Id])}."));

            Storage.Set(Context.Guild.Id, conf);
        }

        [Command("bal")]
        [Summary("Gets the point balance of a user.")]
        public async Task GetBal(IGuildUser? user = null)
        {
            user ??= (IGuildUser)Context.Message.Author;

            var conf = Storage!.Get(Context.Guild.Id);

            AddIfNotExists(Context.Guild.Id, user.Id);

            await ReplyAsync(
                embed: Embeds.Embed(Color.Red,
                $":flag_gb: Go' {user}'s tesco clubcard poin' scawe :flag_gb:",
                $"Their scawe is {conf.SocialCredit![user.Id]}.",
                $"Their ra'in is {CreditRating(conf.SocialCredit![user.Id])}."));

            Storage.Set(Context.Guild.Id, conf);
        }

        [Command("bestLeaderboard")]
        [Summary("Gets the leaderboard of best Tesco Clubcard point earners.")]
        public async Task BestLeaderboard(int amount = 10)
        {
            AddIfNotExists(Context.Guild.Id);
            var conf = Storage!.Get(Context.Guild.Id);

            string output = "";

            var sortedDict = conf.SocialCredit!.OrderBy(x => x.Value).Reverse().ToList();
            sortedDict = sortedDict.GetRange(0, amount > sortedDict.Count() ? sortedDict.Count() : amount);

            foreach (var item in sortedDict)
            {
                output += $"`{sortedDict.IndexOf(item) + 1}.".PadRight(7) + $"{Context.Guild.GetUser(item.Key)}".PadRight(30) + $" {item.Value}`\n";
            }

            await ReplyAsync(
                embed: Embeds.Embed(Color.Red,
                ":flag_gb: Bes' tesco clubcard poin' leaderboard :flag_gb:",
                output));
        }

        [Command("worstLeaderboard")]
        [Summary("Gets the leaderboard of the worst Tesco Clubcard point earners.")]
        public async Task WorstLeaderboard(int amount = 10)
        {
            AddIfNotExists(Context.Guild.Id);
            var conf = Storage!.Get(Context.Guild.Id);

            string output = "";

            var sortedDict = conf.SocialCredit!.OrderBy(x => x.Value).ToList();
            sortedDict = sortedDict.GetRange(0, amount > sortedDict.Count() ? sortedDict.Count() : amount);

            foreach (var item in sortedDict)
            {
                output += $"`{sortedDict.IndexOf(item) + 1}.".PadRight(7) + $"{Context.Guild.GetUser(item.Key)}".PadRight(30) + $" {item.Value}`\n";
            }

            await ReplyAsync(
                embed: Embeds.Embed(Color.Red,
                ":flag_gb: Waws' tesco clubcard poin' leaderboard :flag_gb:",
                output));
        }

        [Command("position")]
        [Summary("Gets a user's position on the leaderboard, with extra information.")]
        public async Task GetPosition(IGuildUser? user = null)
        {
            user ??= (IGuildUser)Context.Message.Author;

            AddIfNotExists(Context.Guild.Id);
            var conf = Storage!.Get(Context.Guild.Id);

            var id = user.Id;

            var sortedDict = conf.SocialCredit!.OrderBy(x => x.Value).Reverse();

            var item = sortedDict.First(x => x.Key == id);
            var index = sortedDict.Select(x => x.Key).ToList().FindIndex(x => x == item.Key);

            string ahead = "They are las'! :rage: :flag_gb:";
            string behind = "They are firs'! :smiley: :flag_gb:";

            if (index != 0)
                behind = $"They are behind {Context.Guild.GetUser(sortedDict.ElementAt(index - 1).Key)}, by {sortedDict.ElementAt(index - 1).Value - item.Value} points.";

            if (!(index >= sortedDict.Count()))
                ahead = $"They are ahead {Context.Guild.GetUser(sortedDict.ElementAt(index + 1).Key)}, by {item.Value - sortedDict.ElementAt(index + 1).Value} points.";

            await ReplyAsync(embed: Embeds.Embed(Color.Red, $":flag_gb: {user}'s posi'ion on 'he poin's leaderboard :flag_gb:", $"They are a' posi'ion {index + 1}.\n", ahead, behind));
        }

        [Command("reset")]
        [Summary("Resets everyones' Tesco Clubcard point score.")]
        [RequireUserPermission(GuildPermission.ManageGuild)]
        public async Task Reset()
        {
            var conf = Storage!.Get(Context.Guild.Id);
            conf.SocialCredit = null;

            await ReplyAsync(embed: Embeds.Embed(Color.Red, ":flag_gb: Rese' all tesco clubcard poin' scawes :flag_gb:"));
        }
    }
}
