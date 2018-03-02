#if DEBUG
using Discord;
using Discord.Commands;
using System.Threading.Tasks;
public class TestModule : ModuleBase<SocketCommandContext>
{
    [Command("ping")]
    [Summary("Replies pong. ")]
    public async Task ping()
    => await ReplyAsync("pong");

    [Command("whoami")]
    [Summary("Returns info of the invoker. ")]
    public async Task whoami(IUser user)
    {
        if (user != null)
        {
            await ReplyAsync(user.ToString());
            return;
        }
        else
            await ReplyAsync(Context.Message.Author.ToString());
    }
}
#endif