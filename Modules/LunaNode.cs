using Discord;
using Discord.Commands;
using System;
using System.Threading.Tasks;
using System.Net.Http;
using System.IO;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
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

    private async Task<Stream> postAction(string category, string action, params string[] optionalparams)
    {
        string request = category + "&" + action;
        foreach (string parameter in optionalparams)
        {
            request += "&" + parameter;
        }
        var response = await httpclient.GetAsync("api?api_id=" + apiid + "&api_key=" + apikey + "&" + request);
        var stream = await response.Content.ReadAsStreamAsync();
        // byte[] streamresult = new byte[stream.Length];
        // await stream.ReadAsync(streamresult, 0, unchecked((int)stream.Length));
        // StreamReader sr = new StreamReader(stream);
        // JsonReader reader = new JsonTextReader(sr);
        
        // JsonSerializer serializer = new JsonSerializer();

        // read the json from a stream
        // json size doesn't matter because only a small piece is read at a time from the HTTP request
        // Object obj = serializer.Deserialize<Object>(reader);

        return stream;
    }

    private async Task<LunaNodeStructs.VmList> getVms()
    {
        StreamReader sr = new StreamReader(await postAction("vm", "list"));
        JsonReader reader = new JsonTextReader(sr);
        
        JsonSerializer serializer = new JsonSerializer();

        // read the json from a stream
        // json size doesn't matter because only a small piece is read at a time from the HTTP request
        return serializer.Deserialize<LunaNodeStructs.VmList>(reader); 
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
        LunaNodeStructs.VmList vms = await getVms();
        await ReplyAsync(vms.vms[1].name);
    }
}