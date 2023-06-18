using GraphShield.Data.Model.Entities;

namespace GraphShield.Proxy.Plumbings.RuleEngine
{
    /// <summary>
    /// Represents a data rule used in the rule engine.
    /// </summary>
    public class DataRule : Rule
    {
        /// <summary>
        /// Gets or sets the type of the data rule.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Creates a data rule with the specified member, operator, and target value.
        /// </summary>
        /// <typeparam name="T">The type of the target value.</typeparam>
        /// <param name="member">The name of the member.</param>
        /// <param name="oper">The rule operator.</param>
        /// <param name="target">The target value.</param>
        /// <returns>The created data rule.</returns>
        public static DataRule Create<T>(string member, RuleOperator oper, T target)
            => new (member, oper, target!, typeof(T));

        /// <summary>
        /// Creates a data rule with the specified member, operator, and target value as a string.
        /// </summary>
        /// <typeparam name="T">The type of the target value.</typeparam>
        /// <param name="member">The name of the member.</param>
        /// <param name="oper">The rule operator.</param>
        /// <param name="target">The target value as a string.</param>
        /// <returns>The created data rule.</returns>
        public static DataRule Create<T>(string member, RuleOperator oper, string target)
            => new (member, oper, target, typeof(T));

        /// <summary>
        /// Creates a data rule with the specified member, operator, target value, and member type.
        /// </summary>
        /// <param name="member">The name of the member.</param>
        /// <param name="oper">The rule operator.</param>
        /// <param name="target">The target value.</param>
        /// <param name="memberType">The type of the member.</param>
        /// <returns>The created data rule.</returns>
        public static DataRule Create(string member, RuleOperator oper, object target, Type memberType)
            => new (member, oper, target, memberType);

        /// <summary>
        /// Initializes a new instance of the <see cref="DataRule"/> class.
        /// </summary>
        /// <param name="member">The name of the member.</param>
        /// <param name="oper">The rule operator.</param>
        /// <param name="target">The target value.</param>
        /// <param name="memberType">The type of the member.</param>
        private DataRule(string member, RuleOperator oper, object target, Type memberType)
        {
            MemberName = member;
            TargetValue = target;
            Operator = oper.ToString();
            Type = memberType.FullName!;
        }
    }
}