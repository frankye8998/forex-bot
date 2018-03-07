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
    // TODO: Global config that doesnt leave this hardcoded
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
        string request = "category=" + category + "&action=" + action;
        foreach (string parameter in optionalparams)
        {
            request += "&" + parameter;
        }
        var response = await httpclient.GetAsync("api?api_id=" + apiid + "&api_key=" + apikey + "&" + request);
        #if DEBUG
        await System.Console.Out.WriteLineAsync("Requested " + response.RequestMessage.RequestUri.AbsoluteUri);
        #endif
        var stream = await response.Content.ReadAsStreamAsync();

        #if DEBUG
        StreamReader sr = new StreamReader(stream);
        string srread = await sr.ReadToEndAsync();
        await System.Console.Out.WriteLineAsync(srread);
        stream = new MemoryStream(System.Text.Encoding.Default.GetBytes(srread));
        #endif
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

    private async Task<string> postActionString(string category, string action, params string[] optionalparams)
    {
        StreamReader sr = new StreamReader(await postAction(category, action, optionalparams));
        // read the json from a stream
        // json size doesn't matter because only a small piece is read at a time from the HTTP request
        return await sr.ReadToEndAsync(); 
    }

    private async Task<VmList> getVms()
    {
        StreamReader sr = new StreamReader(await postAction("vm", "list"));
        JsonReader reader = new JsonTextReader(sr);
        
        JsonSerializer serializer = new JsonSerializer();

        return serializer.Deserialize<VmList>(reader); 
    }

    private async Task<VmState> getVmInfo(string vm_id)
    {
        StreamReader sr = new StreamReader(await postAction("vm", "info", "vm_id=" + vm_id));
        JsonReader reader = new JsonTextReader(sr);
        
        JsonSerializer serializer = new JsonSerializer();

        return serializer.Deserialize<VmState>(reader);
    }

    private async Task<ApiRequest> shelveVm(string vm_id)
    {
        StreamReader sr = new StreamReader(await postAction("vm", "shelve", "vm_id=" + vm_id));
        JsonReader reader = new JsonTextReader(sr);
        
        JsonSerializer serializer = new JsonSerializer();

        return serializer.Deserialize<ApiRequest>(reader);
    }

    private async Task<ApiRequest> unshelveVm(string vm_id)
    {
        StreamReader sr = new StreamReader(await postAction("vm", "unshelve", "vm_id=" + vm_id));
        JsonReader reader = new JsonTextReader(sr);
        
        JsonSerializer serializer = new JsonSerializer();

        return serializer.Deserialize<ApiRequest>(reader);
    }

    private async Task<ApiRequest> startVm(string vm_id)
    {
        StreamReader sr = new StreamReader(await postAction("vm", "start", "vm_id=" + vm_id));
        JsonReader reader = new JsonTextReader(sr);
        
        JsonSerializer serializer = new JsonSerializer();

        return serializer.Deserialize<ApiRequest>(reader);
    }

    private async Task<ApiRequest> stopVm(string vm_id)
    {
        StreamReader sr = new StreamReader(await postAction("vm", "stop", "vm_id=" + vm_id));
        JsonReader reader = new JsonTextReader(sr);
        
        JsonSerializer serializer = new JsonSerializer();

        return serializer.Deserialize<ApiRequest>(reader);
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
    public async Task lunanode(params string[] args)
    {
        // TODO: Better command handling, switch feels a bit messy

        EmbedBuilder embed = new EmbedBuilder();
        switch (args[0])
        {
            case "list":
                embed.Title = "Virtual Machine List";
                VmList vms = await getVms();
                if (vms.successful)
                {
                    EmbedFieldBuilder field;
                    foreach (VirtualMachine vm in vms.vms)
                    {
                        // message += vm.name + ": " + vm.vm_id + "\n";
                        field = new EmbedFieldBuilder();
                        field.Name = vm.name;
                        field.Value = vm.vm_id;
                        field.IsInline = true;
                        embed.Fields.Add(field);
                        embed.Color = Color.Blue;
                    }
                }

                else
                {
                    embed.Title = "Error Getting VMs!";
                    embed.ImageUrl = "https://upload.wikimedia.org/wikipedia/commons/7/74/Feedbin-Icon-error.svg";
                    embed.Description = "Error getting VM list: " + vms.error;
                    embed.Color = Color.Red;
                }
                // await ReplyAsync(message);
                await ReplyAsync("", false, embed.Build());
                return;
            case "query":
                if (args.Length > 1)
                {
                    VmState state = await getVmInfo(args[1]);
                    if (state.successful)
                    {
                        embed.Color = Color.Blue;
                        EmbedFieldBuilder field = new EmbedFieldBuilder();
                        embed.Title = state.extra.name;
                        field.Name = "Status";
                        field.Value = state.info.status_nohtml;
                        field.IsInline = true;
                        embed.Fields.Add(field);
                        /*field.Name = "Task State";
                        field.Value = state.info.task_state;
                        field.IsInline = true;
                        embed.Fields.Add(field);*/

                        field = new EmbedFieldBuilder();
                        field.Name = "Plan ID";
                        field.Value = state.extra.plan_id;
                        field.IsInline = true;
                        embed.Fields.Add(field);

                        field = new EmbedFieldBuilder();
                        field.Name = "Hostname";
                        field.Value = state.extra.hostname;
                        field.IsInline = true;
                        embed.Fields.Add(field);

                        field = new EmbedFieldBuilder();
                        field.Name = "Primary IP";
                        field.Value = state.extra.primaryip;
                        field.IsInline = true;
                        embed.Fields.Add(field);

                        field = new EmbedFieldBuilder();
                        field.Name = "RAM";
                        field.Value = (double)(state.extra.ram / 1024) + "GB";
                        field.IsInline = true;
                        embed.Fields.Add(field);

                        field = new EmbedFieldBuilder();
                        field.Name = "vCPUs";
                        field.Value = state.extra.vcpu;
                        field.IsInline = true;
                        embed.Fields.Add(field);

                        field = new EmbedFieldBuilder();
                        field.Name = "Storage";
                        field.Value = state.extra.storage + "GB";
                        field.IsInline = true;
                        embed.Fields.Add(field);

                        field = new EmbedFieldBuilder();
                        field.Name = "Bandwidth";
                        field.Value = state.extra.bandwith + "GB";
                        field.IsInline = true;
                        embed.Fields.Add(field);
                        
                        field = new EmbedFieldBuilder();
                        field.Name = "Region";
                        field.Value = state.extra.region;
                        field.IsInline = true;
                        embed.Fields.Add(field);

                        field = new EmbedFieldBuilder();
                        field.Name = "OS Status";
                        field.Value = state.extra.os_status;
                        field.IsInline = true;
                        embed.Fields.Add(field);
                    }

                    else
                    {
                        embed.Title = "Error Getting State!";
                        embed.Description = "Error getting VM state: " + state.error;
                    }
                }

                else
                {
                    embed.Title = "Missing arguments";
                    embed.Description = "You must specify the ID of the VM to perform this action on. ";
                    embed.Color = Color.Red;
                }

                await ReplyAsync("", false, embed.Build());
                return;
            case "start":
                if (args.Length > 1)
                {
                    ApiRequest startrequest = await startVm(args[1]);
                    embed.Title = "Start VM";
                    if (startrequest.successful)
                    {
                        embed.Color = Color.Green;
                        embed.Description = "Startup VM successful";
                    }
                    else
                    {
                        embed.Color = Color.Red;
                        embed.Description = "Startup VM failed: " + startrequest.error;
                    }
                    await ReplyAsync("", false, embed.Build());
                }
                return;
            case "stop":
                if (args.Length > 1)
                {
                    ApiRequest shutdownRequest = await stopVm(args[1]);
                    embed.Title = "Shutdown VM";
                    if (shutdownRequest.successful)
                    {
                        embed.Color = Color.Green;
                        embed.Description = "Shutdown VM successful";
                    }
                    else
                    {
                        embed.Color = Color.Red;
                        embed.Description = "Shutdown VM failed: " + shutdownRequest.error;
                    }
                    await ReplyAsync("", false, embed.Build());
                }
                return;
            case "shelve":
                if (args.Length > 1)
                {
                    ApiRequest shelveRequest = await shelveVm(args[1]);
                    embed.Title = "Shelve VM";
                    if (shelveRequest.successful)
                    {
                        embed.Color = Color.Green;
                        embed.Description = "Shelve VM successful";
                    }
                    else
                    {
                        embed.Color = Color.Red;
                        embed.Description = "Shelve VM failed: " + shelveRequest.error;
                    }
                    await ReplyAsync("", false, embed.Build());
                }
                return;
            case "unshelve":
                if (args.Length > 1)
                {
                    ApiRequest unshelveRequest = await unshelveVm(args[1]);
                    embed.Title = "Unshelve VM";
                    if (unshelveRequest.successful)
                    {
                        embed.Color = Color.Green;
                        embed.Description = "Unshelve VM successful";
                    }
                    else
                    {
                        embed.Color = Color.Red;
                        embed.Description = "Unshelve VM failed: " + unshelveRequest.error;
                    }
                    await ReplyAsync("", false, embed.Build());
                }
                return;
            case "rename":
                throw new System.NotImplementedException();
            case "help":
            default:
                embed.Title = "Help";
                embed.Description = "TODO! (SOON™)\nPM @CurrentlyQuestioning#1234 for assistance";
            return;
        }
    }

    // TODO: This is VERY hacky, short term fix, make JSON
    // deserializer and reintegrate into main command at later date (SOON™)
    /*
    [Command("timcpt")]
    [Alias()]
    [Summary("")]
    public async Task timcpt(string command)
    {
        switch (command)
        {
            
        }
    }
    */
}