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
using System.Net.Http;
using System.Text.Json;

namespace HammyBot_Sharp.Bot.Modules
{
    public class Facts : ModuleBase<SocketCommandContext>
    {
        static class FactsHttp
        {
            public class Fact
            {
                public string Contents;
                public string Source;

                public Fact(string contents, string source)
                {
                    Contents = contents;
                    Source = source;
                }
            }
            
            private static readonly HttpClient client = new HttpClient();

            public static async Task<Fact> GetFact()
            {
                string responseJson =
                    await client.GetStringAsync("https://uselessfacts.jsph.pl/random.json?language=en");

                var responseDict = JsonSerializer.Deserialize<Dictionary<String, Object>>(responseJson);

                if (responseDict == null || responseDict["text"] == null || responseDict["source_url"] == null)
                    throw new NullReferenceException("Deserialised JSON from HTTP request was Null. Maybe the facts API is offline?");

                return new Fact(responseDict["text"].ToString()!, responseDict["source_url"].ToString()!);
            }

            public static async Task<Fact> GetFunny()
            {
                string responseJson =
                    await client.GetStringAsync("https://geek-jokes.sameerkumar.website/api?format=json");

                var responseDict = JsonSerializer.Deserialize<Dictionary<String, Object>>(responseJson);

                if (responseDict == null || responseDict["joke"] == null)
                    throw new NullReferenceException("Deserialised JSON from HTTP request was Null. Maybe the joke API is offline?");

                return new Fact(responseDict["joke"].ToString()!, "https://github.com/sameerkumar18/geek-joke-api");
            }
        }

        [Command("fact")]
        public async Task Fact()
        {
            var fact = await FactsHttp.GetFact();
            await Context.Channel.SendMessageAsync(
                embed: Embeds.Embed(Color.Green, "", $"**{fact.Contents}**", $"Source: {fact.Source}")
            );
        }
        
        [Command("funny")]
        public async Task Funny()
        {
            var fact = await FactsHttp.GetFunny();
            await Context.Channel.SendMessageAsync(
                embed: Embeds.Embed(Color.Green, "", $"**{fact.Contents}**", $"Source: {fact.Source}")
            );
        }
    }
}