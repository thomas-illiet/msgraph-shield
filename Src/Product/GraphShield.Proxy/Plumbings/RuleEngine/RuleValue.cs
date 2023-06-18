using GraphShield.Data.Model.Entities;

namespace GraphShield.Proxy.Plumbings.RuleEngine
{
    /// <summary>
    /// Represents a rule value with associated rules.
    /// </summary>
    /// <typeparam name="T">The type of the rule value.</typeparam>
    public class RuleValue<T>
    {
        /// <summary>
        /// Gets or sets the value of the rule.
        /// </summary>
        public T Value { get; set; }

        /// <summary>
        /// Gets or sets the list of associated rules.
        /// </summary>
        public List<RuleContent> Rules { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RuleValue{T}"/> class.
        /// </summary>
        public RuleValue()
        {
            Rules = new List<RuleContent>();
        }
    }
}