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
using Discord.WebSocket;
using HammyBot_Sharp.Bot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace HammyBot_Sharp
{
    public class PerspectiveApiConfig
    {
        public decimal Threshold { get; set; }
        public ulong ReportChannelId { get; set; }

        public async Task<bool> CheckAndReport(string contents, string url, string apiKey, DiscordSocketClient client)
        {
            var toxicityRating = await PerspectiveApiHttp.CheckText(contents, apiKey);
            var reportChannel = (IMessageChannel)client.GetChannel(ReportChannelId);
            
            if (toxicityRating > Threshold)
            {
                await reportChannel.SendMessageAsync(embed: Embeds.Embed(Color.Gold, "Possible toxicity found", $"**Toxicity rating:** {await PerspectiveApiHttp.CheckText(contents, apiKey):P}", $"**Message:** {contents}", $"**URL:** {url}"));
                return true;
            }
            return false;
        }
    }

    static class PerspectiveApiHttp
    {
        public static async Task<decimal> CheckText(string text, string apiKey)
        {
            using var httpClient = new HttpClient();

            var response = await httpClient.PostAsync(
                $"https://commentanalyzer.googleapis.com/v1alpha1/comments:analyze?key={apiKey}",
                new StringContent($"{{comment: {{text: \"{text}\"}}, languages:[\"en\"], requestedAttributes: {{ TOXICITY: {{ }} }}}}", Encoding.UTF8, "application/json"));

            var responseDeserialized = JsonSerializer.Deserialize<dynamic>(await response.Content.ReadAsStringAsync());
            try
            {
                return responseDeserialized!
                .GetProperty("attributeScores")
                .GetProperty("TOXICITY")
                .GetProperty("summaryScore")
                .GetProperty("value")
                .GetDecimal();
            } catch { return 0.00m; }
        }
    }
}
