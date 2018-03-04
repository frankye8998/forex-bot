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
    public async Task whoami(IUser user = null)
    {
        if (user == null)
            user = Context.User;
        await ReplyAsync(user.ToString());
    }
}
#endif