using System;
using System.IO;
using CommandLine;

#nullable enable
namespace HammyBot
{
    internal class Options
    {
        [Option("config_file", Default = "./config.json",
            HelpText = "The path to the config JSON file (includes the bot token and such).")]
        public string ConfigPath { get; set; } = "./config.json";

        [Verb("init", HelpText = "Initialises the directory with the default files and paths.")]
        public class InitOptions {};
    }
    internal static class Program
    {
        static int Main(string[] args)
        {
            return Parser.Default.ParseArguments<Options, Options.InitOptions>(args)
                .MapResult(
                    (Options opts) => RunBot(opts),
                    (Options.InitOptions opts) => Initialise(opts),
                    errs => 1);
        }

        static int RunBot(Options opts)
        {
            Config config = JsonConfigMethods.Load<Config>(opts.ConfigPath);
            Console.WriteLine(config.Token);
            config.Save(opts.ConfigPath);
            // Run bot here.
            return 1;
        }

        static int Initialise(Options.InitOptions opts)
        {
            Console.WriteLine("Initialising directory...");
            Directory.CreateDirectory("./guilds");
            
            if (!File.Exists("./config.json")) 
                File.Create("./config.json");
            File.WriteAllText("./config.json", "{}");
            
            // Puts the config options in the JSON file.
            JsonConfigMethods.Load<Config>("./config.json").Save("./config.json");
            
            Console.WriteLine("Initialised directory.");
            
            return 1;
        }
    }
}