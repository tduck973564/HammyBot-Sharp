using System;
using System.IO;
using System.Xml.Schema;
using CommandLine;
using NLog;

#nullable enable
namespace HammyBot
{ 
    static class Options
    {
        [Verb("run", HelpText = "Initialises the directory with the default files and paths. This is run by default when the directory is uninitialised.")]
        public class RunOptions
        {
            [Option("config_file", Default = "./config.json",
                HelpText = "The path to the config JSON file (includes the bot token and such).")]
            public string ConfigPath { get; set; } = "./config.json";
        }

        [Verb("init", HelpText = "Initialises the directory with the default files and paths.")]
        public class InitOptions {};
    }
    class Program
    {
        public static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        static int Main(string[] args)
        {
            LoggingSetup();
            return Parser.Default.ParseArguments<Options.RunOptions, Options.InitOptions>(args)
                .MapResult(
                    (Options.RunOptions opts) =>
                    {
                        if (!IsInitialised())
                        {
                            Logger.Warn("Directory not fully initialised, reinitialising.");
                            Initialise(new Options.InitOptions());
                        }
                            
                        return RunBot(opts);
                    },
                    (Options.InitOptions opts) => Initialise(opts),
                    errs => 1);
        }

        static int RunBot(Options.RunOptions opts)
        {
            Config config = JsonConfigMethods.Load<Config>(opts.ConfigPath);
            Storage storage = Storage.Load("./storage");
            
            new Bot.Bot(config, storage).MainAsync().GetAwaiter().GetResult();
            
            return 0;
        }

        static int Initialise(Options.InitOptions opts)
        {
            Logger.Info("Initialising directory...");
            
            if (!Directory.Exists("./storage"))
                Directory.CreateDirectory("./storage");
            
            if (!File.Exists("./config.json")) 
                File.Create("./config.json");
            File.WriteAllText("./config.json", "{}");
            
            // Puts the config options in the JSON file.
            JsonConfigMethods.Load<Config>("./config.json").Save("./config.json");
            
            if (!File.Exists("./log.txt")) 
                File.Create("./log.txt");
            
            Logger.Info("Initialised directory.");
            
            return 0;
        }

        static bool IsInitialised()
        {
            return File.Exists("./config.json") && File.Exists("./log.txt") && Directory.Exists("./guilds");
        }
        
        static void LoggingSetup()
        {
            var config = new NLog.Config.LoggingConfiguration();
            
            var logfile = new NLog.Targets.FileTarget("logfile") { FileName = "log.txt" };
            var logconsole = new NLog.Targets.ConsoleTarget("logconsole");
            
            config.AddRule(LogLevel.Info, LogLevel.Fatal, logconsole);
            config.AddRule(LogLevel.Debug, LogLevel.Fatal, logfile);
            
            NLog.LogManager.Configuration = config;
        }
    }
}