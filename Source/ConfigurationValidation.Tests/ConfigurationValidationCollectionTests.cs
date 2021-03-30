using System.Collections.Generic;
using FluentAssertions;
using Xunit;

namespace ConfigurationValidation.Tests
{
    public class ConfigurationValidationCollectionTests
    {
        [Fact]
        public void Create_New_Empty()
        {
            var vals = new ConfigurationValidationCollection();
            vals.Count.Should().Be(0);
        }

        [Fact]
        public void Add_New_ContainsIt()
        {
            var vals = new ConfigurationValidationCollection();
            vals.Add(new ConfigurationValidationItem("Sect", "cfg", 22, "Something"));

            vals.Count.Should().Be(1);
            vals[0].ConfigurationSection.Should().Be("Sect");
            vals[0].ConfigurationItem.Should().Be("cfg");
            vals[0].ConfigurationValue.Should().Be(22);
            vals[0].ValidationMessage.Should().Be("Something");
        }

        [Fact]
        public void Add_Two_ContainsBoth()
        {
            var vals = new ConfigurationValidationCollection();
            vals.Add(new ConfigurationValidationItem("Sect", "cfg", 22, "Something"));
            vals.Add(new ConfigurationValidationItem("Sect", "set", true, "bool"));

            vals.Count.Should().Be(2);
        }

        [Fact]
        public void AddRange_Two_ContainsBoth()
        {
            var valarr = new List<ConfigurationValidationItem>
            {
                new ConfigurationValidationItem("Sect", "cfg", 22, "Something"),
                new ConfigurationValidationItem("Sect", "set", true, "bool")
            };
            var vals = new ConfigurationValidationCollection();
            vals.AddRange(valarr.ToArray());

            vals.Count.Should().Be(2);
            vals[0].ConfigurationSection.Should().Be("Sect");
            vals[1].ConfigurationSection.Should().Be("Sect");
        }

        [Fact]
        public void ConfigurationItem_Filled_ToString()
        {
            var item = new ConfigurationValidationItem
            {
                ConfigurationSection = "Sect",
                ConfigurationItem = "Key",
                ConfigurationValue = true,
                ValidationMessage = "Failure"
            };

            item.ToString().Should().Be("Sect:Key = True (Failure)");
        }
    }
}
