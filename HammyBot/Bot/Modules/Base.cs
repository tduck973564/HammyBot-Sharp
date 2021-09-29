using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace HammyBot.Bot.Modules
{
    public class Base : ModuleBase<SocketCommandContext>
    {
        public Config? Config { get; set; }
        public Storage? Storage { get; set; }
        public DiscordSocketClient? Client { get; set; }

        [Command("echo")]
        public async Task Echo(string echo, IUser? user = null)
        {
            if (user == null)
            {
                await Context.Channel.SendMessageAsync(echo);
                
            }
            else
            {
                await user.SendMessageAsync(echo);
                await Context.Channel.SendMessageAsync(
                    embed: Embeds.Embed(Color.Green, "Sent DM to user!")
                );
            }
        }

        [Command("ping")]
        public async Task Ping()
        {
            await Context.Channel.SendMessageAsync(
                embed: Embeds.Embed(
                    Color.Blue, 
                    "Pong!", 
                    "`API Latency:".PadRight(15) + $"{Client!.Latency.ToString()}ms`"
                )
            );
        }
    }
}