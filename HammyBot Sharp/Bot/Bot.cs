// HammyBot Sharp - HammyBot Sharp
//     Copyright (C) 2021 Thomas Duckworth <tduck973564@gmail.com>
// 
//     This program is free software: you can redistribute it and/or modify
//     it under the terms of the GNU General Public License as published by
//     the Free Software Foundation, either version 3 of the License, or
//     (at your option) any later version.
// 
//     This program is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU General Public License for more details.
// 
//     You should have received a copy of the GNU General Public License
//     along with this program.  If not, see <https://www.gnu.org/licenses/>.

using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using HammyBot_Sharp.Bot.Modules;
using Microsoft.Extensions.DependencyInjection;

namespace HammyBot_Sharp.Bot
{
    public class Bot
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandHandler _commandHandler;
        private readonly CommandService _commands;

        private readonly Config _config;
        private readonly Storage _storage;

        public Bot(Config config, Storage storage)
        {
            AppDomain.CurrentDomain.ProcessExit += OnProcessExit;
            Console.CancelKeyPress += OnProcessExit;

            _config = config;
            _storage = storage;

            _client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Info
            });
            _commands = new CommandService(new CommandServiceConfig
            {
                LogLevel = LogSeverity.Info
            });
            _commandHandler = new CommandHandler(
                _client, _commands, _config,
                BuildServiceProvider()
            );
            _client.SetActivityAsync(new Game(_config.Status ?? "with matches"));
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

        private IServiceProvider BuildServiceProvider()
        {
            return new ServiceCollection()
                .AddSingleton(_client)
                .AddSingleton(_commands)
                .AddSingleton(_storage)
                .AddSingleton(_config)
                .BuildServiceProvider();
        }

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
        private readonly Config _config;
        private readonly IServiceProvider _services;

        // Retrieve client and CommandService instance via ctor
        public CommandHandler(DiscordSocketClient client, CommandService commands, Config config,
            IServiceProvider services)
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
            await _commands.AddModuleAsync<Base>(_services);
            await _commands.AddModuleAsync<Facts>(_services);
            await _commands.AddModuleAsync<Settings>(_services);
            await _commands.AddModuleAsync<Moderation>(_services);

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
            var argPos = 0;

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
                await context.Channel.SendMessageAsync(
                    embed: Embeds.Embed(Color.Red, "An error occured!", result.ErrorReason)
                );
        }
    }
}