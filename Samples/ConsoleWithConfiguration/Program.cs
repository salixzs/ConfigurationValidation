using System;
using System.Linq;
using ConfigurationValidation;
using Microsoft.Extensions.Configuration;

namespace ConsoleWithConfiguration
{
    /// <summary>
    /// Console application on .Net 5 with Microsoft.Extensions.Configuration usage.
    /// Demonstrates used configuration validation.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Main entry point for console application.
        /// </summary>
        /// <param name="args">Command-line arguments.</param>
        /// <returns>0, when ended gracefully, >0 - error happened.</returns>
        public static int Main(string[] args)
        {
            Console.WriteLine("========= Demonstration of configuration validation =========");
            Console.WriteLine("Change values in appsettings.json for validations to show up.");
            Console.WriteLine();

            IConfiguration configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true, true)
                .Build();
            Console.WriteLine("\u2713 Configuration preloaded from appsettings.json");

            SampleConfig sampleConfig = configuration.GetSection("SampleConfig").Get<SampleConfig>();
            if (sampleConfig == null)
            {
                Console.WriteLine("Could not load configuration section into strongly typed object.");
                return WaitExitWithCode(1);
            }

            Console.WriteLine("\u2713 Configuration loaded into SampleConfig strogly typed object.");

            // As strongly typed config has validations, invoke them and get result.
            var validations = sampleConfig.Validate().ToList();

            if (validations.Count == 0)
            {
                Console.WriteLine("\u2713 No problems found in configuration.");
                return WaitExitWithCode(0);
            }

            Console.WriteLine();
            Console.WriteLine("--- These values failed validation set in SampleConfig class ---");
            foreach (ConfigurationValidationItem validation in validations)
            {
                Console.WriteLine($"X {validation.ConfigurationSection} : {validation.ConfigurationItem} failed: \"{validation.ValidationMessage}\" (Value: {validation.ConfigurationValue})");
            }

            // --- You can also throw special exception:
            // throw new ConfigurationValidationException("Configuration is incorrect.", validations);

            return WaitExitWithCode(1);
        }

        private static int WaitExitWithCode(int exitCode)
        {
            Console.WriteLine();
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
            return exitCode;
        }
    }
}
