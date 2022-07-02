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

namespace HammyBot_Sharp.Bot
{
    public static class Embeds
    {
        public static Embed Embed(Color colour, string title, params string[] descriptions)
        {
            return new EmbedBuilder
            {
                Title = title,
                Description = string.Join('\n', descriptions),
                Color = colour,
            }
            .WithFooter(footer => footer.Text = "HammyBot Sharp")
            .WithCurrentTimestamp()
            .Build();
        }
    }
}