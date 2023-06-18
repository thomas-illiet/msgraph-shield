namespace GraphShield.Proxy.Plumbings.RuleEngine
{
    /// <summary>
    /// Describes the node types for the nodes of an expression tree.
    /// </summary>
    public enum RuleOperator
    {
        /// <summary>
        /// An addition operation, such as a + b, without overflow checking, for numeric operands.
        /// </summary>
        Add = 0,

        /// <summary>
        /// A bitwise or logical AND operation, such as (a &amp; b) in C# and (a And b) in Visual Basic.
        /// </summary>
        And = 2,

        /// <summary>
        /// A conditional AND operation that evaluates the second operand only if the first operand evaluates to true.
        /// It corresponds to (a &amp;&amp; b) in C# and (a AndAlso b) in Visual Basic.
        /// </summary>
        AndAlso = 3,

        /// <summary>
        /// A node that represents an equality comparison, such as (a == b) in C# or (a = b) in Visual Basic.
        /// </summary>
        Equal = 13,

        /// <summary>
        /// A "greater than" comparison, such as (a &gt; b).
        /// </summary>
        GreaterThan = 15,

        /// <summary>
        /// A "greater than or equal to" comparison, such as (a &gt;= b).
        /// </summary>
        GreaterThanOrEqual = 16,

        /// <summary>
        /// A "less than" comparison, such as (a &lt; b).
        /// </summary>
        LessThan = 20,

        /// <summary>
        /// A "less than or equal to" comparison, such as (a &lt;= b).
        /// </summary>
        LessThanOrEqual = 21,

        /// <summary>
        /// An inequality comparison, such as (a != b) in C# or (a &lt;&gt; b) in Visual Basic.
        /// </summary>
        NotEqual = 35,

        /// <summary>
        /// A bitwise or logical OR operation, such as (a | b) in C# or (a Or b) in Visual Basic.
        /// </summary>
        Or = 36,

        /// <summary>
        /// A short-circuiting conditional OR operation, such as (a || b) in C# or (a OrElse b) in Visual Basic.
        /// </summary>
        OrElse = 37,

        /// <summary>
        /// Checks that a string value matches a Regex expression.
        /// </summary>
        IsMatch = 100,

        /// <summary>
        /// Checks that a value can be 'TryParsed' to an Int32.
        /// </summary>
        IsInteger = 101,

        /// <summary>
        /// Checks that a value can be 'TryParsed' to a Single.
        /// </summary>
        IsSingle = 102,

        /// <summary>
        /// Checks that a value can be 'TryParsed' to a Double.
        /// </summary>
        IsDouble = 103,

        /// <summary>
        /// Checks that a value can be 'TryParsed' to a Decimal.
        /// </summary>
        IsDecimal = 104,

        /// <summary>
        /// Checks if the value of the property is in the input list.
        /// </summary>
        IsInInput = 105
    }
}