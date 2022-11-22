using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Mail;
using System.Threading;

#pragma warning disable IDE0008 // Use explicit type - here var is better for readability
namespace ConfigurationValidation
{
    /// <summary>
    /// Helper class to provide configuration class (DTO) properties and collect validation problems.
    /// </summary>
    /// <typeparam name="TCfg">Type of strongly typed configuration class.</typeparam>
    public class ConfigurationValidationCollector<TCfg> where TCfg : class
    {
        private readonly string _validationSectionName;
        private readonly TCfg _configurationSection;

        /// <summary>
        /// A resulting list of configuration validations.
        /// </summary>
        public List<ConfigurationValidationItem> Result { get; } = new List<ConfigurationValidationItem>();

        /// <summary>
        /// Helper class to provide configuration class (DTO) properties and collect validation problems.
        /// </summary>
        /// <param name="configurationSection">Instance of configuration class being validated.</param>
        public ConfigurationValidationCollector(TCfg configurationSection)
        {
            _validationSectionName = typeof(TCfg).Name;
            _configurationSection = configurationSection;
        }

        /// <summary>
        /// Generic validator taking valid LINQ expression, which evaluates to boolean result.
        /// <code>
        /// var validations = new ConfigurationValidationCollector&lt;SampleConfig&gt;(this);
        /// validations.ValidateMust(c =&gt; c.Property &gt; 2000, "Property", "Should be greater than 2000");
        /// </code>
        /// </summary>
        /// <param name="validationExpression">Linq expression resulting in boolean value.</param>
        /// <param name="configurationItenName">Name of configuration property under question (usually used in <paramref name="validationExpression"/>).</param>
        /// <param name="message">Validation message to display when validation fails.</param>
        public void ValidateMust(Expression<Func<TCfg, bool>> validationExpression, string configurationItenName, string message)
        {
            var propertyValue = validationExpression.Compile().Invoke(_configurationSection);
            if (!propertyValue)
            {
                this.Result.Add(new ConfigurationValidationItem(_validationSectionName, configurationItenName, propertyValue, message));
            }
        }

        /// <summary>
        /// Add custom validation problem, validated in own logic to validations collection.
        /// <code>
        /// var validations = new ConfigurationValidationCollector&lt;SampleConfig&gt;(this);
        /// validations.ValidateAddCustom(c =&gt; c.Property, "This is wrong, I checked myself.");
        /// </code>
        /// </summary>
        /// <param name="configurationProperty">Expression to point to property.</param>
        /// <param name="message">Validation message to display when validation fails.</param>
        public void ValidateAddCustom(Expression<Func<TCfg, string>> configurationProperty, string message)
        {
            var (propertyName, propertyValue) = this.GetNameAndValue(configurationProperty);
            this.Result.Add(new ConfigurationValidationItem(_validationSectionName, propertyName, propertyValue, message));
        }

        /// <summary>
        /// Validates whether configuration property (of type string) is not null or empty.
        /// <code>
        /// var validations = new ConfigurationValidationCollector&lt;SampleConfig&gt;(this);
        /// validations.ValidateNotNullOrEmpty(c =&gt; c.Property, "Validation failed.");
        /// </code>
        /// </summary>
        /// <param name="configurationProperty">Expression to point to property.</param>
        /// <param name="message">Validation message to display when validation fails.</param>
        public void ValidateNotNullOrEmpty(Expression<Func<TCfg, string>> configurationProperty, string message)
        {
            var (propertyName, propertyValue) = this.GetNameAndValue(configurationProperty);
            if (string.IsNullOrEmpty(propertyValue))
            {
                this.Result.Add(new ConfigurationValidationItem(_validationSectionName, propertyName, propertyValue, message));
            }
        }

        /// <summary>
        /// Validates whether configuration property (of type string) contains necessary string as part of it.
        /// Validation is case insensitive (based on current culture).
        /// <code>
        /// var validations = new ConfigurationValidationCollector&lt;SampleConfig&gt;(this);
        /// validations.ValidateContains(c =&gt; c.Property, "mydomain", "Validation failed.");
        /// </code>
        /// </summary>
        /// <param name="configurationProperty">Expression to point to property.</param>
        /// <param name="containing">string, which should be a part of given configuration property.</param>
        /// <param name="message">Validation message to display when validation fails.</param>
        public void ValidateContains(Expression<Func<TCfg, string>> configurationProperty, string containing, string message)
        {
            var (propertyName, propertyValue) = this.GetNameAndValue(configurationProperty);
            if (string.IsNullOrEmpty(propertyValue) || Thread.CurrentThread.CurrentCulture.CompareInfo.IndexOf(propertyValue, containing, CompareOptions.IgnoreCase) == 0)
            {
                this.Result.Add(new ConfigurationValidationItem(_validationSectionName, propertyName, propertyValue, message));
            }

        }

        /// <summary>
        /// Validates whether configuration property (of type string) starts with given string.
        /// Validation is case insensitive (based on current culture).
        /// <code>
        /// var validations = new ConfigurationValidationCollector&lt;SampleConfig&gt;(this);
        /// validations.ValidateStartsWith(c =&gt; c.Property, "http", "Validation failed.");
        /// </code>
        /// </summary>
        /// <param name="configurationProperty">Expression to point to property.</param>
        /// <param name="starts">string, which should be a starting part of given configuration property.</param>
        /// <param name="message">Validation message to display when validation fails.</param>
        public void ValidateStartsWith(Expression<Func<TCfg, string>> configurationProperty, string starts, string message)
        {
            var (propertyName, propertyValue) = this.GetNameAndValue(configurationProperty);
            if (propertyValue == null || !propertyValue.StartsWith(starts, StringComparison.CurrentCultureIgnoreCase))
            {
                this.Result.Add(new ConfigurationValidationItem(_validationSectionName, propertyName, propertyValue, message));
            }

        }

        /// <summary>
        /// Validates whether configuration property (of type string) ends with given string.
        /// Validation is case insensitive (based on current culture).
        /// <code>
        /// var validations = new ConfigurationValidationCollector&lt;SampleConfig&gt;(this);
        /// validations.ValidateEndsWith(c =&gt; c.Property, "/", "Validation failed.");
        /// </code>
        /// </summary>
        /// <param name="configurationProperty">Expression to point to property.</param>
        /// <param name="ends">string, which should be a ending part of given configuration property.</param>
        /// <param name="message">Validation message to display when validation fails.</param>
        public void ValidateEndsWith(Expression<Func<TCfg, string>> configurationProperty, string ends, string message)
        {
            var (propertyName, propertyValue) = this.GetNameAndValue(configurationProperty);
            if (propertyValue == null || !propertyValue.EndsWith(ends, StringComparison.CurrentCultureIgnoreCase))
            {
                this.Result.Add(new ConfigurationValidationItem(_validationSectionName, propertyName, propertyValue, message));
            }

        }

        /// <summary>
        /// Validates whether configuration property (of type integer) is not zero (0).
        /// <code>
        /// var validations = new ConfigurationValidationCollector&lt;SampleConfig&gt;(this);
        /// validations.ValidateNotZero(c =&gt; c.Property, "Validation failed.");
        /// </code>
        /// </summary>
        /// <param name="configurationProperty">Expression to point to property.</param>
        /// <param name="message">Validation message to display when validation fails.</param>
        public void ValidateNotZero(Expression<Func<TCfg, int>> configurationProperty, string message)
        {
            var (propertyName, propertyValue) = this.GetNameAndValue(configurationProperty);
            if (propertyValue == 0)
            {
                this.Result.Add(new ConfigurationValidationItem(_validationSectionName, propertyName, propertyValue, message));
            }

        }

        /// <summary>
        /// Validates whether configuration property (of type short) is not zero (0).
        /// <code>
        /// var validations = new ConfigurationValidationCollector&lt;SampleConfig&gt;(this);
        /// validations.ValidateNotZero(c =&gt; c.Property, "Validation failed.");
        /// </code>
        /// </summary>
        /// <param name="configurationProperty">Expression to point to property.</param>
        /// <param name="message">Validation message to display when validation fails.</param>
        public void ValidateNotZero(Expression<Func<TCfg, short>> configurationProperty, string message)
        {
            var (propertyName, propertyValue) = this.GetNameAndValue(configurationProperty);
            if (propertyValue == 0)
            {
                this.Result.Add(new ConfigurationValidationItem(_validationSectionName, propertyName, propertyValue, message));
            }

        }

        /// <summary>
        /// Validates whether configuration property (of type long) is not zero (0).
        /// <code>
        /// var validations = new ConfigurationValidationCollector&lt;SampleConfig&gt;(this);
        /// validations.ValidateNotZero(c =&gt; c.Property, "Validation failed.");
        /// </code>
        /// </summary>
        /// <param name="configurationProperty">Expression to point to property.</param>
        /// <param name="message">Validation message to display when validation fails.</param>
        public void ValidateNotZero(Expression<Func<TCfg, long>> configurationProperty, string message)
        {
            var (propertyName, propertyValue) = this.GetNameAndValue(configurationProperty);
            if (propertyValue == 0)
            {
                this.Result.Add(new ConfigurationValidationItem(_validationSectionName, propertyName, propertyValue, message));
            }

        }

        /// <summary>
        /// Validates whether configuration property (of type string) is valid e-mail address (Does not check if it is a real address).
        /// <code>
        /// var validations = new ConfigurationValidationCollector&lt;SampleConfig&gt;(this);
        /// validations.ValidateEmail(c =&gt; c.Property, "Validation failed.");
        /// </code>
        /// </summary>
        /// <param name="configurationProperty">Expression to point to property.</param>
        /// <param name="message">Validation message to display when validation fails.</param>
        public void ValidateEmail(Expression<Func<TCfg, string>> configurationProperty, string message)
        {
            var (propertyName, propertyValue) = this.GetNameAndValue(configurationProperty);

            // solution allows empty strings in email (discards everything before it), but it is not valid.
            if (string.IsNullOrEmpty(propertyValue) || propertyValue.Contains(" "))
            {
                this.Result.Add(new ConfigurationValidationItem(_validationSectionName, propertyName, propertyValue, message));
                return;
            }

            try
            {
                // Will throw exception if string does not contain valid e-mail address.
                var email = new MailAddress(propertyValue);
            }
            catch
            {
                this.Result.Add(new ConfigurationValidationItem(_validationSectionName, propertyName, propertyValue, message));
            }
        }

        /// <summary>
        /// Validates whether configuration property (of type string) is valid absolute URI (Web address).
        /// To be valid it should include protocol (http:// or any other).
        /// <code>
        /// var validations = new ConfigurationValidationCollector&lt;SampleConfig&gt;(this);
        /// validations.ValidateUri(c =&gt; c.Property, "Validation failed.");
        /// </code>
        /// </summary>
        /// <param name="configurationProperty">Expression to point to property.</param>
        /// <param name="message">Validation message to display when validation fails.</param>
        public void ValidateUri(Expression<Func<TCfg, string>> configurationProperty, string message)
        {
            var (propertyName, propertyValue) = this.GetNameAndValue(configurationProperty);
            if (!Uri.IsWellFormedUriString(propertyValue, UriKind.Absolute))
            {
                this.Result.Add(new ConfigurationValidationItem(_validationSectionName, propertyName, propertyValue, message));
            }
        }

        /// <summary>
        /// Validates whether configuration property (of type string) is valid IPv4 address (nnn.nnn.nnn.nnn).
        /// <code>
        /// var validations = new ConfigurationValidationCollector&lt;SampleConfig&gt;(this);
        /// validations.ValidateIpAddress(c =&gt; c.Property, "Validation failed.");
        /// </code>
        /// </summary>
        /// <param name="configurationProperty">Expression to point to property.</param>
        /// <param name="message">Validation message to display when validation fails.</param>
        public void ValidateIpV4Address(Expression<Func<TCfg, string>> configurationProperty, string message)
        {
            var (propertyName, propertyValue) = this.GetNameAndValue(configurationProperty);
            var (isValid, _) = this.IsIpV4Valid(propertyValue);
            if (!isValid || propertyValue == "0.0.0.0")
            {
                this.Result.Add(new ConfigurationValidationItem(_validationSectionName, propertyName, propertyValue, message));
            }
        }

        /// <summary>
        /// Validates whether configuration property (of type string) is valid IP address (nnn.nnn.nnn.nnn) and does not fall into private IP address range (10.x.x.x; 192.168.x.x; 172.16.x.x).
        /// <code>
        /// var validations = new ConfigurationValidationCollector&lt;SampleConfig&gt;(this);
        /// validations.ValidateIpAddress(c =&gt; c.Property, "Validation failed.");
        /// </code>
        /// </summary>
        /// <param name="configurationProperty">Expression to point to property.</param>
        /// <param name="message">Validation message to display when validation fails.</param>
        public void ValidatePublicIpV4Address(Expression<Func<TCfg, string>> configurationProperty, string message)
        {
            var (propertyName, propertyValue) = this.GetNameAndValue(configurationProperty);
            var (isValid, ipAddress) = this.IsIpV4Valid(propertyValue);
            if (!isValid || ipAddress == null || propertyValue == "0.0.0.0")
            {
                this.Result.Add(new ConfigurationValidationItem(_validationSectionName, propertyName, propertyValue, "IP address is missing or it is invalid."));
                return;
            }

            if (this.IsInPrivateNetwork(ipAddress))
            {
                this.Result.Add(new ConfigurationValidationItem(_validationSectionName, propertyName, propertyValue, message));
            }
        }

        /// <summary>
        /// Validates whether configuration property (of type string) is valid IP address (nnn.nnn.nnn.nnn) and is part of private IP address range (10.x.x.x; 192.168.x.x; 172.16.x.x).
        /// <code>
        /// var validations = new ConfigurationValidationCollector&lt;SampleConfig&gt;(this);
        /// validations.ValidateIpAddress(c =&gt; c.Property, "Validation failed.");
        /// </code>
        /// </summary>
        /// <param name="configurationProperty">Expression to point to property.</param>
        /// <param name="message">Validation message to display when validation fails.</param>
        public void ValidatePrivateIpV4Address(Expression<Func<TCfg, string>> configurationProperty, string message)
        {
            var (propertyName, propertyValue) = this.GetNameAndValue(configurationProperty);
            var (isValid, ipAddress) = this.IsIpV4Valid(propertyValue);
            if (!isValid || ipAddress == null || propertyValue == "0.0.0.0")
            {
                this.Result.Add(new ConfigurationValidationItem(_validationSectionName, propertyName, propertyValue, "IP address is missing or it is invalid."));
                return;
            }

            if (!this.IsInPrivateNetwork(ipAddress))
            {
                this.Result.Add(new ConfigurationValidationItem(_validationSectionName, propertyName, propertyValue, message));
            }
        }

        /// <summary>
        /// Retrieves property name and value from passed LINQ expression.
        /// </summary>
        /// <typeparam name="T">Type of property.</typeparam>
        /// <param name="expression">LINQ expression, pointing to configuration class property.</param>
        /// <returns>Tuple of both values.</returns>
        private (string PropertyName, T PropertyValue) GetNameAndValue<T>(Expression<Func<TCfg, T>> expression)
        {
            if (expression.Body is not MemberExpression body)
            {
                throw new ApplicationException("Can validate only configuration member expressions. Please use configuration property in form c => c.Property");
            }

            T propertyValue = expression.Compile().Invoke(_configurationSection);
            return (body.Member.Name, propertyValue);
        }

        /// <summary>
        /// Validates if string contains valid IPv4 address.
        /// </summary>
        /// <param name="ipV4address">IP address as string.</param>
        private (bool IsValid, IPAddress? Address) IsIpV4Valid(string ipV4address)
        {
            // First check whether it cotains 3 dots, as "1" also will be true with if below.
            if (string.IsNullOrEmpty(ipV4address) || ipV4address.Count(c => c == '.') != 3)
            {
                return (false, null);
            }

            if (!IPAddress.TryParse(ipV4address, out IPAddress ipAddress))
            {
                return (false, null);
            }

            return (true, ipAddress);
        }

        /// <summary>
        /// For IP address validations to be part of local/public networks
        /// </summary>
        /// <param name="ipAddress">IP adress to check against network segments</param>
        private bool IsInPrivateNetwork(IPAddress ipAddress)
        {
            var privateRanges = new Dictionary<byte[], int>
            {
                { new byte[] { 10, 0, 0, 0 }, 8 },
                { new byte[] { 172, 16, 0, 0 }, 12 },
                { new byte[] { 192, 168, 0, 0 }, 16 },
            };

            int cidrAddr = BitConverter.ToInt32(ipAddress.GetAddressBytes(), 0);
            foreach (KeyValuePair<byte[], int> range in privateRanges)
            {
                int cidrRange = BitConverter.ToInt32(range.Key, 0);
                int cidrMask = IPAddress.HostToNetworkOrder(-1 << (32 - range.Value));
                if ((cidrAddr & cidrMask) == (cidrRange & cidrMask))
                {
                    return true;
                }
            }

            return false;
        }
    }
#pragma warning restore IDE0008 // Use explicit type
}
