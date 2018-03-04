using Discord;
using Discord.Commands;
using System;
using System.Threading.Tasks;
using System.Net.Http;
public class LunaNode : ModuleBase<SocketCommandContext>
{
    private string apikey;

    [Command("testhttp")]
    [Summary("Test bot HTTP code. ")]
    [RequireUserPermission(GuildPermission.Administrator)]
    public async Task testhttp()
    {
        var client = new HttpClient
        {
            BaseAddress = new Uri("http://httpbin.org/get")
        };

        var response = await client.GetAsync("");
        var stream = await response.Content.ReadAsStreamAsync();
        byte[] streamresult = new byte[2000];
        await stream.ReadAsync(streamresult, 0, unchecked((int)stream.Length));
        await ReplyAsync(System.Text.Encoding.Default.GetString(streamresult));
    }
}