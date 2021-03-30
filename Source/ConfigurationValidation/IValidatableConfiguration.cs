using System.Collections.Generic;

namespace ConfigurationValidation
{
    /// <summary>
    /// Enforcing method for strongly typed Configuration objects validations.
    /// </summary>
    public interface IValidatableConfiguration
    {
        /// <summary>
        /// Performs the validation of this configuration object.
        /// Returns empty list if no problems found, otherwise list contains validation problems.
        /// </summary>
        IEnumerable<ConfigurationValidationItem> Validate();
    }
}
