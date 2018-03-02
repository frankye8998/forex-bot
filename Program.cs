using Discord;
using Discord.WebSocket;
using Discord.Commands;
using System;
using System.Threading.Tasks;
using System.Reflection;
using System.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace stro_bot
{
    class Program
    {
        public IConfigurationRoot Configuration { get; set; }
        private CommandService _commands;
        private DiscordSocketClient _client;
        private IServiceProvider _services;
        private string prefix;

        static void Main(string[] args)
        {
            new Program().MainAsync().GetAwaiter().GetResult();
        }

        public async Task MainAsync()
		{
            #if DEBUG
            Console.WriteLine("Hello World!");
            #endif

            _client = new DiscordSocketClient();
            _commands = new CommandService();

        	_client.Log += Log;

            _services = new ServiceCollection()
            .AddSingleton(_client)
            .AddSingleton(_commands)
            .BuildServiceProvider();

            var builder = new ConfigurationBuilder();  // Create a new instance of the config builder
            #if DEBUG
            builder.SetBasePath(AppContext.BaseDirectory + "../../..");
            #endif
            #if RELEASE
            builder.SetBasePath(AppContext.BaseDirectory);
            #endif
            Microsoft.Extensions.Configuration.JsonConfigurationExtensions.AddJsonFile(builder, "_configuration.json");
            Configuration = builder.Build(); // Build the configuration

            #if DEBUG
            await Log(new LogMessage(LogSeverity.Debug, "Config Dump", Configuration.ToString()));
            #endif

        	await InstallCommandsAsync();
            await _client.LoginAsync(TokenType.Bot, Configuration.GetSection("api_token").Value);
            // Set bot status
            await _client.SetStatusAsync(UserStatus.Online);
	        await _client.StartAsync();

	        // Block this task until the program is closed.
	        await Task.Delay(-1);
        }

        public async Task InstallCommandsAsync()
        {
            // Hook the MessageReceived Event into our Command Handler
            _client.MessageReceived += HandleCommandAsync;
            // Discover all of the commands in this assembly and load them.
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly());
            prefix = Configuration.GetSection("bot_prefix").Value;
        }

        private async Task HandleCommandAsync(SocketMessage messageParam)
        {
            // Don't process the command if it was a System Message
            var message = messageParam as SocketUserMessage;
            if (message == null) return;
            // Also do not process other bots (bot loop would be bad)
            else if (message.Author.IsBot) return;
            // Create a number to track where the prefix ends and the command begins
            int argPos = 0;
            // Determine if the message is a command, based on if it starts with '!' or a mention prefix
            // TODO: MAKE PROPER PREFIX IMPLEMENTATION
            if (!(message.HasStringPrefix(prefix, ref argPos) || message.HasMentionPrefix(_client.CurrentUser, ref argPos))) return;
            // Create a Command Context
            var context = new SocketCommandContext(_client, message);
            // Execute the command. (result does not indicate a return value, 
            // rather an object stating if the command executed successfully)
            var result = await _commands.ExecuteAsync(context, argPos, _services);
            if (!result.IsSuccess)
                await context.Channel.SendMessageAsync(result.ErrorReason);
        }

        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());

            #if DEBUG
	        if (_client.ConnectionState == ConnectionState.Connected)
		        _client.SetGameAsync(msg.ToString(), "https://github.com/varGeneric/stro-bot/");
            #endif
            return Task.CompletedTask;
        }
    }
}