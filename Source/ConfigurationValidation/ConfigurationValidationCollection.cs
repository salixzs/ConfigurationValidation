using System;
using System.Collections;

namespace ConfigurationValidation
{
    /// <summary>
    /// Collection for validations found with their location and values.
    /// </summary>
    [Serializable]
    public class ConfigurationValidationCollection
    {
        private readonly ArrayList _validations = new();

        /// <summary>
        /// Collection for validations found with their location and values.
        /// </summary>
        public ConfigurationValidationCollection()
        {
        }

        /// <summary>
        /// Total count of validation failures found.
        /// </summary>
        public int Count => _validations.Count;

        /// <summary>
        /// Returns validation by its indexer.
        /// </summary>
        /// <param name="index">Location within collection.</param>
        public ConfigurationValidationItem this[int index] =>
            (ConfigurationValidationItem)_validations[index];

        /// <summary>
        /// Enumeration (foreach) capability.
        /// </summary>
        /// <returns></returns>
        public IEnumerator GetEnumerator() => _validations.GetEnumerator();

        /// <summary>
        /// Adds a new validation failure to collection.
        /// </summary>
        /// <param name="validation">Validation failutre.</param>
        public void Add(ConfigurationValidationItem validation) => _ = _validations.Add(validation);

        /// <summary>
        /// Adds multiple validation failures to collectiom.
        /// </summary>
        /// <param name="validations">Array of found validation failures.</param>
        public void AddRange(ConfigurationValidationItem[] validations) => _validations.AddRange(validations);
    }
}
