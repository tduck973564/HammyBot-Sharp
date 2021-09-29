using System.Threading.Tasks;
using System;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;

namespace HammyBot_Sharp.Bot
{
    public class Bot
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commands;
        private readonly CommandHandler _commandHandler;

        private Config _config;
        private Storage _storage;
        
        public Bot(Config config, Storage storage)
        {
            AppDomain.CurrentDomain.ProcessExit += OnProcessExit;
            Console.CancelKeyPress += OnProcessExit;
            
            _config = config;
            _storage = storage;
            
            _client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Info,
            });
            _commands = new CommandService(new CommandServiceConfig
            {
                LogLevel = LogSeverity.Info
            });
            _commandHandler = new CommandHandler(
                _client, _commands, _config,
                BuildServiceProvider()
            );
        }
        
        public async Task MainAsync()
        {
            _client.Log += Log;
            _commands.Log += Log;

            await _client.LoginAsync(TokenType.Bot, _config.Token);
            await _client.StartAsync();
            await _commandHandler.InitializeAsync();
            
            // Block until process exit
            await Task.Delay(-1);
        }

        private Task Log(LogMessage msg)
        {
            switch (msg.Severity)
            {
                case LogSeverity.Verbose:
                    Program.Logger.Trace(msg);
                    break;
                case LogSeverity.Debug:
                    Program.Logger.Debug(msg);
                    break;
                case LogSeverity.Info:
                    Program.Logger.Info(msg);
                    break;
                case LogSeverity.Warning:
                    Program.Logger.Warn(msg);
                    break;
                case LogSeverity.Error:
                    Program.Logger.Error(msg);
                    break;
                case LogSeverity.Critical:
                    Program.Logger.Fatal(msg);
                    break;
            }

            return Task.CompletedTask;
        }
        
        private IServiceProvider BuildServiceProvider() => new ServiceCollection()
            .AddSingleton(_client)
            .AddSingleton(_commands)
            .AddSingleton(_storage)
            .AddSingleton(_config)
            .BuildServiceProvider();
        
        private void OnProcessExit(object? obj, EventArgs eventArgs)
        {
            Program.Logger.Info("Exiting program. Saving current storage state.");
            _storage.Save();
        }
    }
    
    public class CommandHandler
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commands;
        private readonly IServiceProvider _services;
        private readonly Config _config;

        // Retrieve client and CommandService instance via ctor
        public CommandHandler(DiscordSocketClient client, CommandService commands, Config config, IServiceProvider services)
        {
            _commands = commands;
            _client = client;
            _config = config;
            _services = services;
        }
        
        public async Task InitializeAsync()
        {
            // Here we discover all of the command modules in the entry 
            // assembly and load them. Starting from Discord.NET 2.0, a
            // service provider is required to be passed into the
            // module registration method to inject the 
            // required dependencies.
            //
            // If you do not use Dependency Injection, pass null.
            // See Dependency Injection guide for more information.
            await _commands.AddModuleAsync<Modules.Base>(services: _services);
            
            // Hook the MessageReceived event into our command handler
            _client.MessageReceived += HandleCommandAsync;
            
            // Hook the CommandExecuted event into OnCommandExecutedAsync
            _commands.CommandExecuted += OnCommandExecutedAsync;
        }

        private async Task HandleCommandAsync(SocketMessage messageParam)
        {
            // Don't process the command if it was a system message
            var message = messageParam as SocketUserMessage;
            if (message == null) return;

            // Create a number to track where the prefix ends and the command begins
            int argPos = 0;

            // Determine if the message is a command based on the prefix and make sure no bots trigger commands
            if (!(message.HasCharPrefix(_config.Prefix?.ToCharArray()[0] ?? ';', ref argPos) || 
                message.HasMentionPrefix(_client.CurrentUser, ref argPos)) ||
                message.Author.IsBot)
                return;

            // Create a WebSocket-based command context based on the message
            var context = new SocketCommandContext(_client, message);

            // Execute the command with the command context we just
            // created, along with the service provider for precondition checks.
            await _commands.ExecuteAsync(context, argPos, _services);
        }

        private async Task OnCommandExecutedAsync(Optional<CommandInfo> command, ICommandContext context,
            IResult result)
        {
            if (!string.IsNullOrEmpty(result?.ErrorReason))
            {
                await context.Channel.SendMessageAsync(
                    embed: Embeds.Embed(Color.Red, "An error occured!", result.ErrorReason)
                );
            }
        }
    }
}