using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace ConfigurationValidation
{
    /// <summary>
    /// A utility <see cref="Exception"/> that indicates a strong typed configuration model was not configured correctly.
    /// Used by ASP.NET App builder custom filter.
    /// </summary>
    [Serializable]
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class ConfigurationValidationException : Exception
    {
        private readonly ConfigurationValidationCollection _validations = new();

        /// <summary>
        /// Validation data collection - all validations.
        /// </summary>
        public ConfigurationValidationCollection ValidationData => _validations;

        /// <summary>
        /// A utility <see cref="Exception"/> that indicates a strong typed configuration model was not configured correctly.
        /// Used by ASP.NET App builder custom filter.
        /// </summary>
        public ConfigurationValidationException() : this("Configuration validation threw exception.")
        {
        }

        /// <summary>
        /// A utility <see cref="Exception"/> that indicates a strong typed configuration model was not configured correctly.
        /// Used by ASP.NET App builder custom filter.
        /// </summary>
        /// <param name="message">A message that describes the validation problem.</param>
        public ConfigurationValidationException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// A utility <see cref="Exception"/> that indicates a strong typed configuration model was not configured correctly.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the validation to fail.</param>
        /// <param name="innerException">
        /// The exception that is the cause of the current exception.
        /// If the innerException parameter is not a null reference,
        /// the current exception is raised in a catch block that handles the inner exception.
        /// </param>
        public ConfigurationValidationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// A utility <see cref="Exception"/> that indicates a strong typed configuration model was not configured correctly.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the validation to fail.</param>
        /// <param name="failures">A collection of found configuration failures.</param>
        public ConfigurationValidationException(string message, ConfigurationValidationCollection failures) : this(message) =>
            _validations = failures;

        /// <summary>
        /// A utility <see cref="Exception"/> that indicates a strong typed configuration model was not configured correctly.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the validation to fail.</param>
        /// <param name="failures">A collection of found configuration failure items.</param>
        public ConfigurationValidationException(string message, List<ConfigurationValidationItem> failures) : this(message) =>
            _validations.AddRange(failures.ToArray());

        /// <summary>
        /// A utility <see cref="Exception"/> that indicates a strong typed configuration model was not configured correctly.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the validation to fail.</param>
        /// <param name="failures">An array of found configuration failure items.</param>
        public ConfigurationValidationException(string message, ConfigurationValidationItem[] failures) : this(message) =>
            _validations.AddRange(failures);

        /// <summary>
        /// A utility <see cref="Exception"/> that indicates a strong typed configuration model was not configured correctly.
        /// </summary>
        /// <param name="failures">A collection of found configuration failures.</param>
        public ConfigurationValidationException(ConfigurationValidationCollection failures)
            : this("Configuration validation threw exception.", failures)
        {
        }

        /// <summary>
        /// A utility <see cref="Exception"/> that indicates a strong typed configuration model was not configured correctly.
        /// </summary>
        /// <param name="failures">A collection of found configuration failure items.</param>
        public ConfigurationValidationException(List<ConfigurationValidationItem> failures)
            : this("Configuration validation threw exception.", failures)
        {
        }

        /// <summary>
        /// A utility <see cref="Exception"/> that indicates a strong typed configuration model was not configured correctly.
        /// </summary>
        /// <param name="failures">An array of found configuration failure items.</param>
        public ConfigurationValidationException(ConfigurationValidationItem[] failures)
            : this("Configuration validation threw exception.", failures)
        {
        }

        /// <summary>
        /// A utility <see cref="Exception"/> that indicates a strong typed configuration model was not configured correctly.
        /// </summary>
        protected ConfigurationValidationException(SerializationInfo info, StreamingContext context) =>
            _validations = (ConfigurationValidationCollection)info.GetValue("ValidationData", typeof(ConfigurationValidationCollection));

        /// <summary>
        /// Deserialization of the exception data.
        /// </summary>
        /// <param name="info">Serialization information.</param>
        /// <param name="context">A data stream context.</param>
        /// <exception cref="ArgumentNullException">Serialization information is null.</exception>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }

            info.AddValue("ValidationData", _validations, typeof(ConfigurationValidationCollection));
            base.GetObjectData(info, context);
        }

        /// <summary>
        /// A string representation of the exception.
        /// </summary>
        public override string ToString()
        {
            // Weird occasion when exception is thrown, but no validation problems recorded.
            if (this.Message == "Configuration validation threw exception." && this.ValidationData.Count == 0)
            {
                return this.Message;
            }

            // Change default message to more appropriate when validations failed and recorded.
            if (this.Message == "Configuration validation threw exception." && this.ValidationData.Count > 0)
            {
                return $"Cofiguration validation found problems with configured values. {_validations.Count} validations failed.";
            }

            // Return custom exception.
            return $"{this.Message}{(this.Message.EndsWith(".") ? string.Empty : ".")} {_validations.Count} validations failed.";
        }

        /// <summary>
        /// Displays object main properties in Debug screen. (Only for development purposes).
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay => this.ToString();
    }
}
