using Discord;
using Discord.Commands;
using System;
using System.Threading.Tasks;
using System.Net.Http;
using System.IO;
using Microsoft.Extensions.Configuration;
public class LunaNode : ModuleBase<SocketCommandContext>
{
    private static string apiid = "";
    private static string apikey = "";
    // private IConfigurationRoot Configuration;
    private HttpClient httpclient;
    
    public LunaNode()// (IConfigurationRoot config)
    {
        // Configuration = config;
        httpclient = new HttpClient
        {
            BaseAddress = new Uri("https://dynamic.lunanode.com/")
        };
    }

    private async Task<byte[]> postAction(string category, string action, params string[] optionalparams)
    {
        string request = category + "&" + action;
        foreach (string parameter in optionalparams)
        {
            request += "&" + parameter;
        }
        var response = await httpclient.GetAsync("api?api_id=" + apiid + "&api_key=" + apikey + "&" + request);
        var stream = await response.Content.ReadAsStreamAsync();
        byte[] streamresult = new byte[stream.Length];
        await stream.ReadAsync(streamresult, 0, unchecked((int)stream.Length));

        return streamresult;
    }

    #if DEBUG
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

        // await ReplyAsync(Configuration.ToString());
    }
    #endif

    [Command("lunanode")]
    [Alias("luna", "server")]
    [Summary("Commands for LunaNode integrations")]
    // [RequireUserPermission(GuildPermission.Administrator)]
    public async Task lunanode()
    {
        await ReplyAsync(System.Text.Encoding.Default.GetString(await postAction("vm", "list")));
    }
}