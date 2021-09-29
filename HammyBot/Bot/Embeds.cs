using System;
using Discord;

namespace HammyBot.Bot
{
    public static class Embeds
    {
        public static Embed Embed(Color colour, string title, params string[] descriptions)
        {
            return new EmbedBuilder
            {
                Title = title,
                Description = String.Join('\n', descriptions),
                Color = colour,
            }.Build();
        }
    }
}