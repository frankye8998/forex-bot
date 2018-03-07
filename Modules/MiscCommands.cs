using Discord;
using Discord.Commands;
using System.Threading.Tasks;
public class MiscCommands : ModuleBase<SocketCommandContext>
{
    [Command("saychannel")]
    [Summary("Bot says contents to specified channel. ")]
    [RequireUserPermission(GuildPermission.Administrator)]
    public async Task saychannel(ITextChannel channel, string content)
    {
        await channel.SendMessageAsync(content);
    }

    [Command("version")]
    [Summary("")]
    [Alias("ver", "build")]
    public async Task version()
    {
        System.Reflection.Assembly thisAssem = typeof(stro_bot.Program).Assembly;
        System.Reflection.AssemblyName thisAssemName = thisAssem.GetName();
        await ReplyAsync(thisAssemName.Version.ToString());
    }
}