using System.Threading.Tasks;
using System;
using Discord;
using Discord.WebSocket;

#nullable enable
namespace HammyBot.Bot
{
    public class Bot
    {
        private DiscordSocketClient? _client;
        public Config Config;
        public Storage Storage;

        public Bot(Config config, Storage storage)
        {
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(OnProcessExit);
            this.Config = config;
            this.Storage = storage;
        }
        
        public async Task MainAsync()
        {
            _client = new DiscordSocketClient();
            _client.Log += Log;

            await _client.LoginAsync(TokenType.Bot, Config.Token);
            await _client.StartAsync();

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

        private void OnProcessExit(object? obj, EventArgs eventArgs)
        {
            Program.Logger.Info("Exiting program. Saving current storage state.");
            Storage.Save();
        }
    }
}