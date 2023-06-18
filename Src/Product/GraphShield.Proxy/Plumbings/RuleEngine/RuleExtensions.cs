using GraphShield.Data.Model.Entities;

namespace GraphShield.Proxy.Plumbings.RuleEngine
{
    internal static class RuleExtensions
    {
        /// <summary>
        /// Adds a range of values to the collection.
        /// </summary>
        /// <typeparam name="T">The type of the collection elements.</typeparam>
        /// <param name="collection">The collection to add the values to.</param>
        /// <param name="newValues">The values to add to the collection.</param>
        public static void AddRange<T>(this IList<T> collection, IEnumerable<T> newValues)
        {
            foreach (var item in newValues)
            {
                collection.Add(item);
            }
        }

        public static Rule ToRule(this RuleContent source)
        {
            var rules = new List<Rule>();
            if (source.Rules != null)
                rules = source.Rules.ConvertAll(x => x != default ? ToRule(x) : default);

            return new Rule
            {
                MemberName = source.MemberName,
                Operator = source.Operator,
                TargetValue = source.TargetValue,
                Rules = rules,
                Inputs = source.Inputs != null ? source.Inputs : Enumerable.Empty<Rule>(),
                Negate = source.Negate,
            };
        }
    }
}