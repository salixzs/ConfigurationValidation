using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Xunit;

namespace ConfigurationValidation.Tests
{
    public class ConfigurationValidationCollectorTests
    {
        [Fact]
        public void Collector_Full_Collected()
        {
            var cfg = new TestConfig { SomeValue = 0, SomeName = "", SomeEmail = "@.", SomeEndpoint = "Crap", SomeIp = "what?" };
            var vals = cfg.Validate().ToList();
            vals.Should().NotBeNull();
            vals.Should().HaveCount(11);
        }

        [Fact]
        public void AddCustom_Validation()
        {
            var cfg = new TestConfig { SomeName = "Wrong" };
            var coll = new ConfigurationValidationCollector<TestConfig>(cfg);
            coll.ValidateAddCustom(c => c.SomeName, "This is wrong!");
            coll.Result.Should().NotBeNull();
            coll.Result.Count.Should().Be(1);
            coll.Result[0].ConfigurationSection.Should().Be("TestConfig");
            coll.Result[0].ConfigurationItem.Should().Be("SomeName");
            coll.Result[0].ConfigurationValue.Should().Be("Wrong");
            coll.Result[0].ValidationMessage.Should().Be("This is wrong!");
        }

        [Fact]
        public void IntNotZero_Validation()
        {
            var cfg = new TestConfig { SomeValue = 0 };
            var coll = new ConfigurationValidationCollector<TestConfig>(cfg);
            coll.ValidateNotZero(c => c.SomeValue, "No Zeroes!");
            coll.Result.Should().NotBeNull();
            coll.Result.Count.Should().Be(1);
            coll.Result[0].ConfigurationSection.Should().Be("TestConfig");
            coll.Result[0].ConfigurationItem.Should().Be("SomeValue");
            coll.Result[0].ConfigurationValue.Should().Be(0);
            coll.Result[0].ValidationMessage.Should().Be("No Zeroes!");
        }

        [Fact]
        public void IntNotZero_Success_NoValidation()
        {
            var cfg = new TestConfig { SomeValue = 1 };
            var coll = new ConfigurationValidationCollector<TestConfig>(cfg);
            coll.ValidateNotZero(c => c.SomeValue, "No Zeroes!");
            coll.Result.Should().NotBeNull();
            coll.Result.Count.Should().Be(0);
        }

        [Fact]
        public void ShortNotZero_Validation()
        {
            var cfg = new TestConfig { SomeShortValue = 0 };
            var coll = new ConfigurationValidationCollector<TestConfig>(cfg);
            coll.ValidateNotZero(c => c.SomeShortValue, "No Zeroes!");
            coll.Result.Should().NotBeNull();
            coll.Result.Count.Should().Be(1);
            coll.Result[0].ConfigurationSection.Should().Be("TestConfig");
            coll.Result[0].ConfigurationItem.Should().Be("SomeShortValue");
            coll.Result[0].ConfigurationValue.Should().Be(0);
            coll.Result[0].ValidationMessage.Should().Be("No Zeroes!");
        }

        [Fact]
        public void ShortNotZero_Success_NoValidation()
        {
            var cfg = new TestConfig { SomeShortValue = 1 };
            var coll = new ConfigurationValidationCollector<TestConfig>(cfg);
            coll.ValidateNotZero(c => c.SomeShortValue, "No Zeroes!");
            coll.Result.Should().NotBeNull();
            coll.Result.Count.Should().Be(0);
        }

        [Fact]
        public void LongNotZero_Validation()
        {
            var cfg = new TestConfig { SomeLongValue = 0 };
            var coll = new ConfigurationValidationCollector<TestConfig>(cfg);
            coll.ValidateNotZero(c => c.SomeLongValue, "No Zeroes!");
            coll.Result.Should().NotBeNull();
            coll.Result.Count.Should().Be(1);
            coll.Result[0].ConfigurationSection.Should().Be("TestConfig");
            coll.Result[0].ConfigurationItem.Should().Be("SomeLongValue");
            coll.Result[0].ConfigurationValue.Should().Be(0);
            coll.Result[0].ValidationMessage.Should().Be("No Zeroes!");
        }

        [Fact]
        public void LongNotZero_Success_NoValidation()
        {
            var cfg = new TestConfig { SomeLongValue = 1000000 };
            var coll = new ConfigurationValidationCollector<TestConfig>(cfg);
            coll.ValidateNotZero(c => c.SomeLongValue, "No Zeroes!");
            coll.Result.Should().NotBeNull();
            coll.Result.Count.Should().Be(0);
        }

        [Fact]
        public void NotNullOrEmpty_Null_Validation()
        {
            var cfg = new TestConfig { SomeName = null };
            var coll = new ConfigurationValidationCollector<TestConfig>(cfg);
            coll.ValidateNotNullOrEmpty(c => c.SomeName, "Wow!");
            coll.Result.Should().NotBeNull();
            coll.Result.Count.Should().Be(1);
            coll.Result[0].ConfigurationSection.Should().Be("TestConfig");
            coll.Result[0].ConfigurationItem.Should().Be("SomeName");
            coll.Result[0].ConfigurationValue.Should().BeNull();
            coll.Result[0].ValidationMessage.Should().Be("Wow!");
        }

        [Fact]
        public void NotNullOrEmpty_Empty_Validation()
        {
            var cfg = new TestConfig { SomeName = string.Empty };
            var coll = new ConfigurationValidationCollector<TestConfig>(cfg);
            coll.ValidateNotNullOrEmpty(c => c.SomeName, "Wow!");
            coll.Result.Should().NotBeNull();
            coll.Result.Count.Should().Be(1);
            coll.Result[0].ConfigurationSection.Should().Be("TestConfig");
            coll.Result[0].ConfigurationItem.Should().Be("SomeName");
            coll.Result[0].ConfigurationValue.Should().Be(string.Empty);
            coll.Result[0].ValidationMessage.Should().Be("Wow!");
        }

        [Fact]
        public void NotNullOrEmpty_Filled_NoValidation()
        {
            var cfg = new TestConfig { SomeName = "Anrijs" };
            var coll = new ConfigurationValidationCollector<TestConfig>(cfg);
            coll.ValidateNotNullOrEmpty(c => c.SomeName, "Wow!");
            coll.Result.Should().NotBeNull();
            coll.Result.Count.Should().Be(0);
        }

        [Fact]
        public void Contains_Null_Validation()
        {
            var cfg = new TestConfig { SomeName = null };
            var coll = new ConfigurationValidationCollector<TestConfig>(cfg);
            coll.ValidateContains(c => c.SomeName, "domain", "Should have domain!");
            coll.Result.Should().NotBeNull();
            coll.Result.Count.Should().Be(1);
            coll.Result[0].ConfigurationSection.Should().Be("TestConfig");
            coll.Result[0].ConfigurationItem.Should().Be("SomeName");
            coll.Result[0].ConfigurationValue.Should().BeNull();
            coll.Result[0].ValidationMessage.Should().Be("Should have domain!");
        }

        [Fact]
        public void Contains_Empty_Validation()
        {
            var cfg = new TestConfig { SomeName = string.Empty };
            var coll = new ConfigurationValidationCollector<TestConfig>(cfg);
            coll.ValidateContains(c => c.SomeName, "domain", "Should have domain!");
            coll.Result.Should().NotBeNull();
            coll.Result.Count.Should().Be(1);
            coll.Result[0].ConfigurationSection.Should().Be("TestConfig");
            coll.Result[0].ConfigurationItem.Should().Be("SomeName");
            coll.Result[0].ConfigurationValue.Should().Be(string.Empty);
            coll.Result[0].ValidationMessage.Should().Be("Should have domain!");
        }

        [Fact]
        public void Contains_String_NoValidation()
        {
            var cfg = new TestConfig { SomeName = "Test for Domain features." };
            var coll = new ConfigurationValidationCollector<TestConfig>(cfg);
            coll.ValidateContains(c => c.SomeName, "domain", "Should have domain!");
            coll.Result.Should().NotBeNull();
            coll.Result.Count.Should().Be(0);
        }

        [Fact]
        public void StartsWith_Null_Validation()
        {
            var cfg = new TestConfig { SomeEndpoint = null };
            var coll = new ConfigurationValidationCollector<TestConfig>(cfg);
            coll.ValidateStartsWith(c => c.SomeEndpoint, "HTTP", "Should start with protocol!");
            coll.Result.Should().NotBeNull();
            coll.Result.Count.Should().Be(1);
            coll.Result[0].ConfigurationSection.Should().Be("TestConfig");
            coll.Result[0].ConfigurationItem.Should().Be("SomeEndpoint");
            coll.Result[0].ConfigurationValue.Should().BeNull();
            coll.Result[0].ValidationMessage.Should().Be("Should start with protocol!");
        }

        [Fact]
        public void StartsWith_Empty_Validation()
        {
            var cfg = new TestConfig { SomeEndpoint = string.Empty };
            var coll = new ConfigurationValidationCollector<TestConfig>(cfg);
            coll.ValidateStartsWith(c => c.SomeEndpoint, "HTTP", "Should start with protocol!");
            coll.Result.Should().NotBeNull();
            coll.Result.Count.Should().Be(1);
            coll.Result[0].ConfigurationSection.Should().Be("TestConfig");
            coll.Result[0].ConfigurationItem.Should().Be("SomeEndpoint");
            coll.Result[0].ConfigurationValue.Should().Be(string.Empty);
            coll.Result[0].ValidationMessage.Should().Be("Should start with protocol!");
        }

        [Fact]
        public void StartsWith_Wrong_Validation()
        {
            var cfg = new TestConfig { SomeEndpoint = "ftp://domain.com" };
            var coll = new ConfigurationValidationCollector<TestConfig>(cfg);
            coll.ValidateStartsWith(c => c.SomeEndpoint, "HTTP", "Should start with web protocol!");
            coll.Result.Should().NotBeNull();
            coll.Result.Count.Should().Be(1);
            coll.Result[0].ConfigurationSection.Should().Be("TestConfig");
            coll.Result[0].ConfigurationItem.Should().Be("SomeEndpoint");
            coll.Result[0].ConfigurationValue.Should().Be("ftp://domain.com");
            coll.Result[0].ValidationMessage.Should().Be("Should start with web protocol!");
        }

        [Fact]
        public void StartsWith_Correct_Validation()
        {
            var cfg = new TestConfig { SomeEndpoint = "https://domain.com" };
            var coll = new ConfigurationValidationCollector<TestConfig>(cfg);
            coll.ValidateStartsWith(c => c.SomeEndpoint, "HTTP", "Should start with web protocol!");
            coll.Result.Should().NotBeNull();
            coll.Result.Count.Should().Be(0);
        }

        [Fact]
        public void EndsWith_Null_Validation()
        {
            var cfg = new TestConfig { SomeEndpoint = null };
            var coll = new ConfigurationValidationCollector<TestConfig>(cfg);
            coll.ValidateEndsWith(c => c.SomeEndpoint, "COM", "Should be COM domain!");
            coll.Result.Should().NotBeNull();
            coll.Result.Count.Should().Be(1);
            coll.Result[0].ConfigurationSection.Should().Be("TestConfig");
            coll.Result[0].ConfigurationItem.Should().Be("SomeEndpoint");
            coll.Result[0].ConfigurationValue.Should().BeNull();
            coll.Result[0].ValidationMessage.Should().Be("Should be COM domain!");
        }

        [Fact]
        public void EndsWith_Empty_Validation()
        {
            var cfg = new TestConfig { SomeEndpoint = string.Empty };
            var coll = new ConfigurationValidationCollector<TestConfig>(cfg);
            coll.ValidateEndsWith(c => c.SomeEndpoint, "COM", "Should be COM domain!");
            coll.Result.Should().NotBeNull();
            coll.Result.Count.Should().Be(1);
            coll.Result[0].ConfigurationSection.Should().Be("TestConfig");
            coll.Result[0].ConfigurationItem.Should().Be("SomeEndpoint");
            coll.Result[0].ConfigurationValue.Should().Be(string.Empty);
            coll.Result[0].ValidationMessage.Should().Be("Should be COM domain!");
        }

        [Fact]
        public void EndsWith_Wrong_Validation()
        {
            var cfg = new TestConfig { SomeEndpoint = "ftp://domain.lv" };
            var coll = new ConfigurationValidationCollector<TestConfig>(cfg);
            coll.ValidateStartsWith(c => c.SomeEndpoint, "COM", "Should be COM domain!");
            coll.Result.Should().NotBeNull();
            coll.Result.Count.Should().Be(1);
            coll.Result[0].ConfigurationSection.Should().Be("TestConfig");
            coll.Result[0].ConfigurationItem.Should().Be("SomeEndpoint");
            coll.Result[0].ConfigurationValue.Should().Be("ftp://domain.lv");
            coll.Result[0].ValidationMessage.Should().Be("Should be COM domain!");
        }

        [Fact]
        public void EndsWith_Correct_Validation()
        {
            var cfg = new TestConfig { SomeEndpoint = "https://domain.com" };
            var coll = new ConfigurationValidationCollector<TestConfig>(cfg);
            coll.ValidateEndsWith(c => c.SomeEndpoint, "COM", "Should be COM domain!");
            coll.Result.Should().NotBeNull();
            coll.Result.Count.Should().Be(0);
        }

        [Theory(DisplayName = "Valid")]
        [InlineData("me@my.desk")]
        [InlineData("name.surname@company.com")]
        [InlineData("surname@business.lv")]
        [InlineData("some-strange-thing@sci.fi")]
        [InlineData("big.boss@corporation.co.uk")]
        public void Email_ValidVariations_NoProblem(string email)
        {
            var cfg = new TestConfig { SomeEmail = email };
            var coll = new ConfigurationValidationCollector<TestConfig>(cfg);
            coll.ValidateEmail(c => c.SomeEmail, "Incorrect email!");
            coll.Result.Should().NotBeNull();
            coll.Result.Count.Should().Be(0);
        }

        [Theory(DisplayName = "Invalid")]
        [InlineData((string)null)]
        [InlineData("")]
        [InlineData("@")]
        [InlineData(".")]
        [InlineData("@.")]
        [InlineData("Just some text")]
        [InlineData("this@is@interesting.not")]
        [InlineData("anrijs vitolins@inbox.com")]
        public void Email_InvalidVariations_Validates(string email)
        {
            var cfg = new TestConfig { SomeEmail = email };
            var coll = new ConfigurationValidationCollector<TestConfig>(cfg);
            coll.ValidateEmail(c => c.SomeEmail, "Incorrect email!");
            coll.Result.Should().NotBeNull();
            coll.Result.Count.Should().Be(1);
            coll.Result[0].ConfigurationSection.Should().Be("TestConfig");
            coll.Result[0].ConfigurationItem.Should().Be("SomeEmail");
            coll.Result[0].ConfigurationValue.Should().Be(email);
            coll.Result[0].ValidationMessage.Should().Be("Incorrect email!");
        }

        [Theory(DisplayName = "Valid")]
        [InlineData("1.1.1.1")]
        [InlineData("255.255.255.255")]
        [InlineData("192.168.1.1")]
        [InlineData("10.0.0.0")]
        public void IP_ValidVariations_NoProblem(string ip)
        {
            var cfg = new TestConfig { SomeIp = ip };
            var coll = new ConfigurationValidationCollector<TestConfig>(cfg);
            coll.ValidateIpV4Address(c => c.SomeIp, "Incorrect IP!");
            coll.Result.Should().NotBeNull();
            coll.Result.Count.Should().Be(0);
        }

        [Theory(DisplayName = "Invalid")]
        [InlineData((string)null)]
        [InlineData("")]
        [InlineData("...")]
        [InlineData(".")]
        [InlineData("0.0.0.0")]
        [InlineData("1.1.1.260")]
        [InlineData("260.1.2.3")]
        [InlineData("156.12.92.")]
        public void IP_InvalidVariations_Validates(string ip)
        {
            var cfg = new TestConfig { SomeIp = ip };
            var coll = new ConfigurationValidationCollector<TestConfig>(cfg);
            coll.ValidateIpV4Address(c => c.SomeIp, "Incorrect IP!");
            coll.Result.Should().NotBeNull();
            coll.Result.Count.Should().Be(1);
            coll.Result[0].ConfigurationSection.Should().Be("TestConfig");
            coll.Result[0].ConfigurationItem.Should().Be("SomeIp");
            coll.Result[0].ConfigurationValue.Should().Be(ip);
            coll.Result[0].ValidationMessage.Should().Be("Incorrect IP!");
        }

        [Theory(DisplayName = "Valid")]
        [InlineData("192.168.0.0")]
        [InlineData("10.0.0.0")]
        [InlineData("172.16.0.0")]
        [InlineData("192.168.100.1")]
        [InlineData("10.10.128.11")]
        [InlineData("172.21.201.199")]
        [InlineData("192.168.255.255")]
        [InlineData("10.255.255.255")]
        [InlineData("172.31.255.255")]
        public void IpPrivate_ValidVariations_NoProblem(string ip)
        {
            var cfg = new TestConfig { SomeIp = ip };
            var coll = new ConfigurationValidationCollector<TestConfig>(cfg);
            coll.ValidatePrivateIpV4Address(c => c.SomeIp, "Not private IP!");
            coll.Result.Should().NotBeNull();
            coll.Result.Count.Should().Be(0);
        }

        [Theory(DisplayName = "Invalid")]
        [InlineData((string)null)]
        [InlineData("")]
        [InlineData("...")]
        [InlineData(".")]
        [InlineData("0.0.0.0")]
        [InlineData("1.1.1.260")]
        [InlineData("260.1.2.3")]
        [InlineData("156.12.92.")]
        public void IpPrivate_InvalidIpVariations_Validates(string ip)
        {
            var cfg = new TestConfig { SomeIp = ip };
            var coll = new ConfigurationValidationCollector<TestConfig>(cfg);
            coll.ValidatePrivateIpV4Address(c => c.SomeIp, "Not private IP!");
            coll.Result.Should().NotBeNull();
            coll.Result.Count.Should().Be(1);
            coll.Result[0].ConfigurationSection.Should().Be("TestConfig");
            coll.Result[0].ConfigurationItem.Should().Be("SomeIp");
            coll.Result[0].ConfigurationValue.Should().Be(ip);
            coll.Result[0].ValidationMessage.Should().Be("IP address is missing or it is invalid.");
        }

        [Theory(DisplayName = "Not in range")]
        [InlineData("9.255.255.255")]
        [InlineData("11.0.0.0")]
        [InlineData("192.167.255.255")]
        [InlineData("192.169.0.0")]
        [InlineData("172.15.255.255")]
        [InlineData("172.32.0.0")]
        public void IpPrivate_InvalidIpRangeVariations_Validates(string ip)
        {
            var cfg = new TestConfig { SomeIp = ip };
            var coll = new ConfigurationValidationCollector<TestConfig>(cfg);
            coll.ValidatePrivateIpV4Address(c => c.SomeIp, "Not private IP!");
            coll.Result.Should().NotBeNull();
            coll.Result.Count.Should().Be(1);
            coll.Result[0].ConfigurationSection.Should().Be("TestConfig");
            coll.Result[0].ConfigurationItem.Should().Be("SomeIp");
            coll.Result[0].ConfigurationValue.Should().Be(ip);
            coll.Result[0].ValidationMessage.Should().Be("Not private IP!");
        }

        [Theory(DisplayName = "Valid")]
        [InlineData("9.255.255.255")]
        [InlineData("11.0.0.0")]
        [InlineData("192.167.255.255")]
        [InlineData("192.169.0.0")]
        [InlineData("172.15.255.255")]
        [InlineData("172.32.0.0")]
        [InlineData("8.8.8.8")]
        [InlineData("128.12.54.205")]
        public void IpPublic_ValidVariations_NoProblem(string ip)
        {
            var cfg = new TestConfig { SomeIp = ip };
            var coll = new ConfigurationValidationCollector<TestConfig>(cfg);
            coll.ValidatePublicIpV4Address(c => c.SomeIp, "Not public IP!");
            coll.Result.Should().NotBeNull();
            coll.Result.Count.Should().Be(0);
        }

        [Theory(DisplayName = "Invalid")]
        [InlineData((string)null)]
        [InlineData("")]
        [InlineData("...")]
        [InlineData(".")]
        [InlineData("0.0.0.0")]
        [InlineData("1.1.1.260")]
        [InlineData("260.1.2.3")]
        [InlineData("156.12.92.")]
        public void IpPublic_InvalidIpVariations_Validates(string ip)
        {
            var cfg = new TestConfig { SomeIp = ip };
            var coll = new ConfigurationValidationCollector<TestConfig>(cfg);
            coll.ValidatePublicIpV4Address(c => c.SomeIp, "Not public IP!");
            coll.Result.Should().NotBeNull();
            coll.Result.Count.Should().Be(1);
            coll.Result[0].ConfigurationSection.Should().Be("TestConfig");
            coll.Result[0].ConfigurationItem.Should().Be("SomeIp");
            coll.Result[0].ConfigurationValue.Should().Be(ip);
            coll.Result[0].ValidationMessage.Should().Be("IP address is missing or it is invalid.");
        }

        [Theory(DisplayName = "Not in range")]
        [InlineData("192.168.0.0")]
        [InlineData("10.0.0.0")]
        [InlineData("172.16.0.0")]
        [InlineData("192.168.100.1")]
        [InlineData("10.10.128.11")]
        [InlineData("172.21.201.199")]
        [InlineData("192.168.255.255")]
        [InlineData("10.255.255.255")]
        [InlineData("172.31.255.255")]
        public void IpPublic_InvalidIpRangeVariations_Validates(string ip)
        {
            var cfg = new TestConfig { SomeIp = ip };
            var coll = new ConfigurationValidationCollector<TestConfig>(cfg);
            coll.ValidatePublicIpV4Address(c => c.SomeIp, "Not public IP!");
            coll.Result.Should().NotBeNull();
            coll.Result.Count.Should().Be(1);
            coll.Result[0].ConfigurationSection.Should().Be("TestConfig");
            coll.Result[0].ConfigurationItem.Should().Be("SomeIp");
            coll.Result[0].ConfigurationValue.Should().Be(ip);
            coll.Result[0].ValidationMessage.Should().Be("Not public IP!");
        }

        [Theory(DisplayName = "Invalid")]
        [InlineData((string)null)]
        [InlineData("")]
        [InlineData("..")]
        [InlineData("//")]
        [InlineData("http://")]
        [InlineData("http://.com")]
        [InlineData("\\asa")]
        [InlineData("api.local.dev")]
        [InlineData("data.warehouse.co.br/")]
        public void Url_InvalidVariations_Validates(string ep)
        {
            var cfg = new TestConfig { SomeEndpoint = ep };
            var coll = new ConfigurationValidationCollector<TestConfig>(cfg);
            coll.ValidateUri(c => c.SomeEndpoint, "Not valid URL!");
            coll.Result.Should().NotBeNull();
            coll.Result.Count.Should().Be(1);
            coll.Result[0].ConfigurationSection.Should().Be("TestConfig");
            coll.Result[0].ConfigurationItem.Should().Be("SomeEndpoint");
            coll.Result[0].ConfigurationValue.Should().Be(ep);
            coll.Result[0].ValidationMessage.Should().Be("Not valid URL!");
        }

        [Theory(DisplayName = "Valid")]
        [InlineData("http://domain.com")]
        [InlineData("https://api.services.lv")]
        [InlineData("ftp://files.corp.co.uk")]
        [InlineData("http://data.endpoint.info/")]
        public void Url_ValidVariations_Validates(string ep)
        {
            var cfg = new TestConfig { SomeEndpoint = ep };
            var coll = new ConfigurationValidationCollector<TestConfig>(cfg);
            coll.ValidateUri(c => c.SomeEndpoint, "Not valid URL!");
            coll.Result.Should().NotBeNull();
            coll.Result.Count.Should().Be(0);
        }
    }

    public class TestConfig
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
            var validations = new ConfigurationValidationCollector<TestConfig>(this);

            // Here are validations
            validations.ValidateNotZero(c => c.SomeValue, "Configuration should not contain default value (=0).");
            validations.ValidateNotNullOrEmpty(c => c.SomeName, "Configuration should specify value for Name.");
            validations.ValidateUri(c => c.SomeEndpoint, "External API endpoint is incorrect");
            validations.ValidateEmail(c => c.SomeEmail, "E-mail address is wrong.");
            validations.ValidateIpV4Address(c => c.SomeIp, "IP address is not valid.");
            validations.ValidatePublicIpV4Address(c => c.SomeIp, "IP address is not a public IP address.");

            // Generic methods, expecting boolean outcome of Linq expression
            validations.ValidateMust(c => c.SomeEndpoint.StartsWith("https", System.StringComparison.OrdinalIgnoreCase), nameof(this.SomeEndpoint), "Enpoint is no SSL secured.");
            validations.ValidateMust(c => c.SomeEndpoint.EndsWith("/", System.StringComparison.OrdinalIgnoreCase), nameof(this.SomeEndpoint), "Enpoint should end with shash /.");
            validations.ValidateMust(c =>
                c.SomeName.Contains("sparta", System.StringComparison.OrdinalIgnoreCase)
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
