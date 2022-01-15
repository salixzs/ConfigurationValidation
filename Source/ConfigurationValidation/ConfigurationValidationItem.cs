using System;

namespace ConfigurationValidation
{
    /// <summary>
    /// One configuation item definition.
    /// </summary>
    [Serializable]
    public class ConfigurationValidationItem
    {
        /// <summary>
        /// One configuation item definition.
        /// </summary>
        public ConfigurationValidationItem()
        {
        }

        /// <summary>
        /// One configuation item definition.
        /// </summary>
        public ConfigurationValidationItem(string section, string item, object value, string message)
        {
            this.ConfigurationSection = section;
            this.ConfigurationItem = item;
            this.ConfigurationValue = value;
            this.ValidationMessage = message;
        }

        /// <summary>
        /// Name of the configuration section where problem was found.
        /// </summary>
        public string ConfigurationSection { get; set; }

        /// <summary>
        /// Name of configuration item (key).
        /// </summary>
        public string ConfigurationItem { get; set; }

        /// <summary>
        /// Received faulty configuration value.
        /// </summary>
        public object ConfigurationValue { get; set; }

        /// <summary>
        /// Validation message, describing problem with given <see cref="ConfigurationValue"/>.
        /// </summary>
        public string ValidationMessage { get; set; }

        /// <summary>
        /// String representation of configution item.
        /// </summary>
        public override string ToString() => $"{this.ConfigurationSection}:{this.ConfigurationItem} = {this.ConfigurationValue} ({this.ValidationMessage})";
    }
}
