namespace RuleWeaver.Abstractions
{
    /// <summary>
    /// Contract that every validation rule must follow.
    /// 'RuleWeaver' scans the assembly looking for implementations of this interface.
    /// </summary>
    public interface IValidationRule
    {
        /// <summary>
        /// The name of the rule used in JSON (e.g. "Required", "Min", "IsCpf").
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Executes the validation.
        /// </summary>
        /// <param name="value">The value of the property being validated.</param>
        /// <param name="args">Optional arguments (e.g. in "Min:5", args[0] is "5").</param>
        /// <param name="errorMessage">Return message in case of validation failure.</param>
        /// <returns>True if valid, False if invalid.</returns>
        bool Validate(object? value, string[] args, out string errorMessage);
    }
}
