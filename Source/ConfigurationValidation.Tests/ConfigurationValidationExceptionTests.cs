using System.Collections.Generic;
using FluentAssertions;
using Xunit;

namespace ConfigurationValidation.Tests
{
    public class ConfigurationValidationExceptionTests
    {
        [Fact]
        public void ValidationException_Construct_EmptyValidations()
        {
            var ex = new ConfigurationValidationException();

            ex.Message.Should().Be("Configuration validation threw exception.");
            ex.ToString().Should().Be("Configuration validation threw exception.");
            ex.ValidationData.Should().NotBeNull();
            ex.ValidationData.Count.Should().Be(0);
        }

        [Fact]
        public void ValidationException_ConstructColl_NotEmptyValidations()
        {
            var vals = new ConfigurationValidationCollection();
            vals.Add(new ConfigurationValidationItem("Sect", "cfg", 22, "Something"));
            vals.Add(new ConfigurationValidationItem("Sect", "set", true, "bool"));
            var ex = new ConfigurationValidationException(vals);


            ex.Message.Should().Be("Configuration validation threw exception.");
            ex.ToString().Should().Be("Cofiguration validation found problems with configured values. 2 validations failed.");
            ex.ValidationData.Should().NotBeNull();
            ex.ValidationData.Count.Should().Be(2);
        }

        [Fact]
        public void ValidationException_ConstructList_EmptyValidations()
        {
            var vals = new List<ConfigurationValidationItem>
            {
                new ConfigurationValidationItem("Sect", "cfg", 22, "Something"),
                new ConfigurationValidationItem("Sect", "set", true, "bool")
            };
            var ex = new ConfigurationValidationException("Test", vals);

            ex.Message.Should().Be("Test");
            ex.ToString().Should().Be("Test. 2 validations failed.");
            ex.ValidationData.Should().NotBeNull();
            ex.ValidationData.Count.Should().Be(2);
        }

        [Fact]
        public void ValidationException_ConstructArr_EmptyValidations()
        {
            var vals = new ConfigurationValidationItem[2];
            vals[0] = new ConfigurationValidationItem("Sect", "cfg", 22, "Something");
            vals[1] = new ConfigurationValidationItem("Sect", "set", true, "bool");
            var ex = new ConfigurationValidationException("Test", vals);

            ex.Message.Should().Be("Test");
            ex.ToString().Should().Be("Test. 2 validations failed.");
            ex.ValidationData.Should().NotBeNull();
            ex.ValidationData.Count.Should().Be(2);
        }
    }
}
