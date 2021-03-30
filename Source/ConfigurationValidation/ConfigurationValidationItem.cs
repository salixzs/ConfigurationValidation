using System;

namespace ConfigurationValidation
{
    [Serializable]
    public class ConfigurationValidationItem
    {
        public ConfigurationValidationItem()
        {
        }

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


        public override string ToString() => $"{this.ConfigurationSection}:{this.ConfigurationItem} = {this.ConfigurationValue} ({this.ValidationMessage})";
    }
}
