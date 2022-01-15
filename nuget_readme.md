# ConfigurationValidation
Provides means to validate strongly typed configuration class properties on common types (IP address, URL, e-mail address, numbers and strings) of configuration values being valid.

Validation for configuration objects are performed entirely for all class properties (does not throw exception on first failure). Handling validation outcome is upon application business logic. There is special exception type provided, if throwing exception is required.

Package is very lightweight, targeting .Net Standard 2.0 (works with .Net Framework 4.7.2+ and .Net Core 2+) and does not have any other external dependencies.

## Usage
Configuration validation works with strongly typed POCO classes where entire configuration or configuration sections (several configuration classes) are loaded from configuration files (json/xml/yaml/whatever) or environment variables into these class objects.

Imagine you have this POCO class, holding configuration values, loaded from some configuration files (e.g. Microsoft.Extensions.Configuration.* IOptions usage).

```csharp
public class SampleConfig
{
    // Here are properties of configuration values, filled from some configuration.
    public int SomeValue { get; set; }
    public short SomeShortValue { get; set; }
    public long SomeLongValue { get; set; }
    public string SomeName { get; set; }
    public string SomeEndpoint { get; set; }
    public string SomeEmail { get; set; }
    public string SomeIp { get; set; }
}
```

To add validation for loaded values during runtime, you have to slightly extend these classes - they should implement `IValidatableConfiguration` interface, which demands only `IEnumerable<ConfigurationValidationItem> Validate();` method to be added for this class. This method then performs validations, collects them and returns all found misconfigurations in a collection of `ConfigurationValidationItem`s.

Package contains helper class `ConfigurationValidationCollector`, which can considerably ease performing such validations and collecting outcome.

Here is sample of such strongly typed configuration/section class after modifications:

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

### Usage in AspNetCore Web API/app

In ASP.NET Core you can act on configuration validation results in several ways, starting from preventing app startup with IStartupFilter and least intrusive - as HealthCheck.

Three approaches are shown in another repository Sample project: [Salix.AspNetCore.Utilities](https://github.com/salixzs/AspNetCore.Utilities) and are briefly described below.

#### Registering validatable configurations with IoC

First register all your strongly typed configuration class objects with IoC (services):

```csharp
// With configuration as IOptions
services.Configure<SampleConfig>(_configuration.GetSection("SampleConfig"));
// As normal singleton instance for injection
services.AddSingleton(ctx => ctx.GetRequiredService<IOptions<SampleConfig>>().Value);
// As IValidatableConfiguration instance for "automatic" validations
services.AddSingleton<IValidatableConfiguration>(ctx => ctx.GetRequiredService<IOptions<SampleConfig>>().Value);
```

#### IStartupFilter

This is most invasive approach as there are no apparent visible display for reasons of application "crash" during startup.

Create class, implementing `IStartupFilter`, which takes all `IValidatableConfiguration` instances, validates them and throws exception if any misconfigurations are found.

```csharp
public class ConfigurationValidationStartupFilter : IStartupFilter
{
    private readonly IEnumerable<IValidatableConfiguration> _cfgs;

    // Constructor gets injected all instances of validatable condifuration objects
    public ConfigurationValidationStartupFilter(IEnumerable<IValidatableConfiguration> cfgs)
        => _cfgs = cfgs;

    public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
    {
        var fails = new List<ConfigurationValidationItem>();
        foreach (IValidatableConfiguration cfg in _cfgs)
        {
            // Runs Validation on all instances and collects outcomes
            fails.AddRange(cfg.Validate());
        }

        if (fails.Count > 0)
        {
            // If any found - throws special exception
            throw new ConfigurationValidationException("There are issues with configuration.", fails);
        }

        return next;
    }
}
```

Then register this filter as well in Startup ConfigureServices method.

```csharp
services.AddTransient<IStartupFilter, ConfigurationValidationStartupFilter>();
```

Upon Asp.Net Core application startup - this filter will be invoked and will end up in application "crash". You will have to dig up reasons for crash via application monitoring and environment logs.

This filter is provided in [Salix.AspNetCore.Utilities](https://github.com/salixzs/AspNetCore.Utilities) repository/package.

#### Error information (page) middleware

Less intrusive approach as it allows you to get some visible response from your application when changes are deployed. Application itself will not work, but you will get whatever you set to return in this middleware component.

It is quite similar to Developer Error page, you can create something, which returns HTML contents to requesting party (e.g. browser) via this middleware component.
It is quite a code sample to be shown here. See example in [Salix.AspNetCore.Utilities](https://github.com/salixzs/AspNetCore.Utilities) repository (or use it :-) ). 

In essence you should do the same as in IStartupFilter implementation above, just in case of validations - change `Response` to some information, invoke its WriteAsync with content and do not call `await next`.

#### HealthCheck

Standard approach with [Microsoft.Extensions.Diagnostics.HealthChecks](https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/health-checks?view=aspnetcore-5.0) package implementation.
This is least intrusive approach and will not prevent your application from starting up (unless misconfiguration itself prevents it).
As with middleware - see example for such HealthCheck in [Salix.AspNetCore.Utilities](https://github.com/salixzs/AspNetCore.Utilities) repository (or use it :-) ).

When using HealthCheck-ing approach - make sure you actually check the health of your app deployment with it.

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
