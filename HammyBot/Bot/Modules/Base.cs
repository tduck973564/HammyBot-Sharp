using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace HammyBot.Bot.Modules
{
    public class Base : ModuleBase<SocketCommandContext>
    {
        [Command("echo")]
        public async Task Echo(string echo, IUser user = null)
        {
            if (user == null)
            {
                await Context.Channel.SendMessageAsync(echo);
                
            }
            else
            {
                await user.SendMessageAsync(echo);
            }
        }
    }
}