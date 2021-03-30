# ConfigurationValidation
Provides means to validate configuration objects, which are normally loaded from any configuration sources. 

Validation for configuration objects are performed entirely (does not throw exceptions on first failure). Handling validated values is upon application business logic - you. There is special exception type provided, if throwing exception is required.

## Usage
Configuration validation expects configuration (or configuration sections for multiple configuration objects) in files (json/xml/yaml) or environment variables loaded into strongly typed configuration objects.

These strongly typed configuration objects should implement `IValidatableConfiguration` interface, which demands only `IEnumerable<ConfigurationValidationItem> Validate();` method to be added for this class. This method then performs validations, collects them and returns all found mis-configurations in a collection of `ConfigurationValidationItem`s.

Package contains helper class `ConfigurationValidationCollector`, which can considerably ease performing such validations and collecting outcome.

Here is sample of such strongly typed configuration section class:

```csharp
public class SampleConfig : IValidatableConfiguration
{
    // Here are properties of configuration values, filled from some configuration.
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
        validations.ValidateEndsWith(c => c.SomeEndpoint, "/", "Enpoint should end in slash character.");

        // Returning all found validation problems
        return validations.Result;
    }
}
```
as shown on example - helper class contains many ready-made routines to validate class properties for some widely used configuration value correctness.

### Usage in console (simple) app
Samples folder contains an example of usage of this functionality in Console application.

Here I omit code on how to load configuration into strongly typed object `sampleConfig` (see Sample project on usage of Microsoft.Extensions.Configuration for this purpose).

```csharp
var validations = sampleConfig.Validate().ToList();
if (validations.Count == 0)
{
    // All is fine
}
```

if validation method returned some validations, this means there are problems in configured values and they can be enumerated to handle:

```csharp
foreach (ConfigurationValidationItem validation in validations)
{
    Console.WriteLine($"{validation.ConfigurationSection} : {validation.ConfigurationItem} failed: {validation.ValidationMessage} (Value: {validation.ConfigurationValue})");
}
```

or you can throw exception:
```csharp
throw new ConfigurationValidationException("Configuration is incorrect.", validations);
```

and then handle its property `ex.ValidationData` in some general exception handler.

Using interface on configuration with IoC container allows to do some magic of finding all these classes and performing validations in centralized manner.

## How to install
You add `ConfigurationValidation` package to all projects, where your strongly typed configuration classes are either with Visual Studio NuGet manager or from command line:
```
PM> Install-Package ConfigurationValidation
```

In your project(s) csproj this should appear:

```xml
<PackageReference Include="ConfigurationValidation" Version="1.0.0" />
```

Then in your code files add on top 
```csharp
using ConfigurationValidation;
...
```
to get access to all provided functionality.

## Provided validations
There are number of ready-made validations provided for values in your configuration objects as methods on `ConfigurationValidationCollector` class.

#### `ValidateNotNullOrEmpty`
For string configuration properties this is simple validation to check whether value is not NULL or empty string.
```csharp
var validations = new ConfigurationValidationCollector<SampleConfig>(this);
validations.ValidateNotNullOrEmpty(c => c.Property, "Validation failed message.");
```

#### `ValidateContains`
For string configuration properties this validates whether configured value contains given string as part of it. Validation is case insensitive and uses current Culture set in application.
```csharp
var validations = new ConfigurationValidationCollector<SampleConfig>(this);
validations.ValidateContains(c => c.Property, "mydomain", "Validation failed message.");
```

#### `ValidateStartsWith`
For string configuration properties this validates whether configured value starts with given string. Validation is case insensitive and uses current Culture set in application.
```csharp
var validations = new ConfigurationValidationCollector<SampleConfig>(this);
validations.ValidateStartsWith(c => c.Property, "https", "API endpoint should be SSL secured.");
```

#### `ValidateEndsWith`
For string configuration properties this validates whether configured value ends with given string. Validation is case insensitive and uses current Culture set in application.
```csharp
var validations = new ConfigurationValidationCollector<SampleConfig>(this);
validations.ValidateEndsWith(c => c.Property, "/", "API endpoint should end in slash /.");
```

#### `ValidateNotZero`
For `short`, `integer` and `long` configuration properties this validates whether configured value is not zero (0).
```csharp
var validations = new ConfigurationValidationCollector<SampleConfig>(this);
validations.ValidateNotZero(c => c.Property, "Validation failed message.");
```

#### `ValidateEmail`
For string configuration values this will check whether configuration property contains valid e-mail address. It does not check whether such address really exists, just validates if it is in correct e-mail address format.
```csharp
var validations = new ConfigurationValidationCollector<SampleConfig>(this);
validations.ValidateEmail(c => c.Property, "Not correct e-mail address.");
```

#### `ValidateUri`
For string configuration values this will check whether configuration property contains absolute internet address (with protocol). Example: `https://data.organization.com/api`
```csharp
var validations = new ConfigurationValidationCollector<SampleConfig>(this);
validations.ValidateUri(c => c.Property, "Not correct API address.");
```

#### `ValidateIpV4Address`
For string configuration values this will check whether configuration property contains correct IP address. Example: `182.23.1.0`
```csharp
var validations = new ConfigurationValidationCollector<SampleConfig>(this);
validations.ValidateIpV4Address(c => c.Property, "Not correct IP address.");
```

#### `ValidatePublicIpV4Address`
For string configuration values in addition to checking whether value is correct IP address, it will also check whether it belongs to public IP address range (not internal address ranges 10.0.0.0/8; 192.168.0.0/16; 172.16.0.0/12).
```csharp
var validations = new ConfigurationValidationCollector<SampleConfig>(this);
validations.ValidatePublicIpV4Address(c => c.Property, "Should not be internal IP address.");
```

#### `ValidatePrivateIpV4Address`
For string configuration values in addition to checking whether value is correct IP address, it will also check whether it belongs to private IP address range (10.0.0.0/8; 192.168.0.0/16; 172.16.0.0/12).
```csharp
var validations = new ConfigurationValidationCollector<SampleConfig>(this);
validations.ValidatePrivateIpV4Address(c => c.Property, "Should not be internal IP address.");
```

#### `ValidateMust`
For one or more configuration values will allow to supply custom validation as Linq expression resulting into boolean value.
Second parameter should contain name(s) of configuration class properties, which are responsible for failing this validation.
```csharp
var validations = new ConfigurationValidationCollector<SampleConfig>(this);
validations.ValidateMust(c => c.Property1 > 2000 && c.Property2 == "year", "Property1&2", "Combined validation failed.");
```
