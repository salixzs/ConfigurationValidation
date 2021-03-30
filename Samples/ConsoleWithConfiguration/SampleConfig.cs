using System;
using System.Collections.Generic;
using ConfigurationValidation;

namespace ConsoleWithConfiguration
{
    public class SampleConfig
    {
        public int SomeValue { get; set; }
        public short SomeShortValue { get; set; }
        public long SomeLongValue { get; set; }
        public string SomeName { get; set; }
        public string SomeEndpoint { get; set; }
        public string SomeEmail { get; set; }
        public string SomeIp { get; set; }

        /// <summary>
        /// Performs the validation of this configuration object.
        /// Returns empty list if no problems found, otherwise list contains validation problems.
        /// </summary>
        public IEnumerable<ConfigurationValidationItem> Validate()
        {
            // Helper to collect and perform validations
            var validations = new ConfigurationValidationCollector<SampleConfig>(this);

            // Here are validations
            validations.ValidateNotZero(c => c.SomeValue, "Configuration should not contain default value (=0).");
            validations.ValidateNotNullOrEmpty(c => c.SomeName, "Configuration should specify value for Name.");
            validations.ValidateUri(c => c.SomeEndpoint, "External API endpoint is incorrect");
            validations.ValidateEmail(c => c.SomeEmail, "E-mail address is wrong.");
            validations.ValidateIpV4Address(c => c.SomeIp, "IP address is not valid.");
            validations.ValidatePublicIpV4Address(c => c.SomeIp, "IP address is not a public IP address.");

            // Generic methods, expecting boolean outcome of Linq expression
            validations.ValidateMust(c => c.SomeEndpoint.StartsWith("https", StringComparison.OrdinalIgnoreCase), nameof(this.SomeEndpoint), "Enpoint is no SSL secured.");
            validations.ValidateMust(c => c.SomeEndpoint.EndsWith("/", StringComparison.OrdinalIgnoreCase), nameof(this.SomeEndpoint), "Enpoint should end with shash /.");
            validations.ValidateMust(c =>
                c.SomeName.Contains("sparta", StringComparison.OrdinalIgnoreCase)
                && c.SomeValue > 10,
                $"{nameof(this.SomeName)} and {nameof(this.SomeValue)}",
                "Combined validations failed.");

            // Syntactic sugar
            validations.ValidateStartsWith(c => c.SomeEndpoint, "https", "Enpoint is no SSL secured.");
            validations.ValidateEndsWith(c => c.SomeEndpoint, "/", "Enpoint is no SSL secured.");

            // Returning all found validation problems
            return validations.Result;
        }
    }
}
