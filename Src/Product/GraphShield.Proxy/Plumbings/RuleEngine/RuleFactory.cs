using System.Collections;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using GraphShield.Proxy.Exceptions;

namespace GraphShield.Proxy.Plumbings.RuleEngine
{
    public class RuleFactory
    {
        private static readonly ExpressionType[] _nestedOperators = new ExpressionType[]
            {ExpressionType.And, ExpressionType.AndAlso, ExpressionType.Or, ExpressionType.OrElse};

        private static readonly Lazy<MethodInfo> _miRegexIsMatch = new Lazy<MethodInfo>(() =>
            typeof(Regex).GetMethod("IsMatch", new[] { typeof(string), typeof(string), typeof(RegexOptions) }));

        private static readonly Lazy<MethodInfo> _miGetItem = new Lazy<MethodInfo>(() =>
            typeof(System.Data.DataRow).GetMethod("get_Item", new Type[] { typeof(string) }));

        private static readonly Lazy<MethodInfo> _miListContains = new Lazy<MethodInfo>(() =>
            typeof(IList).GetMethod("Contains", new[] { typeof(object) }));

        private static readonly Tuple<string, Lazy<MethodInfo>>[] _enumrMethodsByName =
            new Tuple<string, Lazy<MethodInfo>>[]
            {
                Tuple.Create("Any", new Lazy<MethodInfo>(() => GetLinqMethod("Any", 2))),
                Tuple.Create("All", new Lazy<MethodInfo>(() => GetLinqMethod("All", 2))),
            };

        private static readonly Lazy<MethodInfo> _miIntTryParse = new Lazy<MethodInfo>(() =>
            typeof(Int32).GetMethod("TryParse", new Type[] { typeof(string), Type.GetType("System.Int32&") }));

        private static readonly Lazy<MethodInfo> _miFloatTryParse = new Lazy<MethodInfo>(() =>
            typeof(Single).GetMethod("TryParse", new Type[] { typeof(string), Type.GetType("System.Single&") }));

        private static readonly Lazy<MethodInfo> _miDoubleTryParse = new Lazy<MethodInfo>(() =>
            typeof(Double).GetMethod("TryParse", new Type[] { typeof(string), Type.GetType("System.Double&") }));

        private static readonly Lazy<MethodInfo> _miDecimalTryParse = new Lazy<MethodInfo>(() =>
            typeof(Decimal).GetMethod("TryParse", new Type[] { typeof(string), Type.GetType("System.Decimal&") }));

        public Func<T, bool> CompileRule<T>(Rule r)
        {
            var paramUser = Expression.Parameter(typeof(T));
            Expression expr = GetExpressionForRule(typeof(T), r, paramUser);

            return Expression.Lambda<Func<T, bool>>(expr, paramUser).Compile();
        }

        public static Expression<Func<T, bool>> ToExpression<T>(Rule r, bool useTryCatchForNulls = true)
        {
            var paramUser = Expression.Parameter(typeof(T));
            Expression expr = GetExpressionForRule(typeof(T), r, paramUser, useTryCatchForNulls);

            return Expression.Lambda<Func<T, bool>>(expr, paramUser);
        }

        public static Func<T, bool> ToFunc<T>(Rule r, bool useTryCatchForNulls = true)
        {
            return ToExpression<T>(r, useTryCatchForNulls).Compile();
        }

        public static Expression<Func<object, bool>> ToExpression(Type type, Rule r)
        {
            var paramUser = Expression.Parameter(typeof(object));
            Expression expr = GetExpressionForRule(type, r, paramUser);

            return Expression.Lambda<Func<object, bool>>(expr, paramUser);
        }

        public static Func<object, bool> ToFunc(Type type, Rule r)
        {
            return ToExpression(type, r).Compile();
        }

        public Func<object, bool> CompileRule(Type type, Rule r)
        {
            var paramUser = Expression.Parameter(typeof(object));
            Expression expr = GetExpressionForRule(type, r, paramUser);

            return Expression.Lambda<Func<object, bool>>(expr, paramUser).Compile();
        }

        public Func<T, bool> CompileRules<T>(IEnumerable<Rule> rules)
        {
            var paramUser = Expression.Parameter(typeof(T));
            var expr = BuildNestedExpression(typeof(T), rules, paramUser, ExpressionType.And);
            return Expression.Lambda<Func<T, bool>>(expr, paramUser).Compile();
        }

        public Func<object, bool> CompileRules(Type type, IEnumerable<Rule> rules)
        {
            var paramUser = Expression.Parameter(type);
            var expr = BuildNestedExpression(type, rules, paramUser, ExpressionType.And);
            return Expression.Lambda<Func<object, bool>>(expr, paramUser).Compile();
        }

        // Build() in some forks
        protected static Expression GetExpressionForRule(Type type, Rule rule, ParameterExpression parameterExpression, bool useTryCatchForNulls = true)
        {
            ExpressionType nestedOperator;
            if (ExpressionType.TryParse(rule.Operator, out nestedOperator) &&
                _nestedOperators.Contains(nestedOperator) && rule.Rules != null && rule.Rules.Any())
                return rule.Negate
                    ? Expression.Not(BuildNestedExpression(type, rule.Rules, parameterExpression, nestedOperator, useTryCatchForNulls))
                    : BuildNestedExpression(type, rule.Rules, parameterExpression, nestedOperator, useTryCatchForNulls);
            else
            {
                return rule.Negate
                    ? Expression.Not(BuildExpr(type, rule, parameterExpression, useTryCatchForNulls))
                    : BuildExpr(type, rule, parameterExpression, useTryCatchForNulls);
            }
        }

        protected static Expression BuildNestedExpression(Type type, IEnumerable<Rule> rules, ParameterExpression param,
            ExpressionType operation, bool useTryCatchForNulls = true)
        {
            var expressions = rules.Select(r => GetExpressionForRule(type, r, param, useTryCatchForNulls));
            return BinaryExpression(expressions, operation);
        }

        protected static Expression BinaryExpression(IEnumerable<Expression> expressions, ExpressionType operationType)
        {
            Func<Expression, Expression, Expression> methodExp;
            switch (operationType)
            {
                case ExpressionType.Or:
                    methodExp = Expression.Or;
                    break;

                case ExpressionType.OrElse:
                    methodExp = Expression.OrElse;
                    break;

                case ExpressionType.AndAlso:
                    methodExp = Expression.AndAlso;
                    break;

                default:
                case ExpressionType.And:
                    methodExp = Expression.And;
                    break;
            }

            return expressions.Aggregate(methodExp);
        }

        private static readonly Regex _regexIndexed =
            new Regex(@"(?'Collection'\w+)\[(?:(?'Index'\d+)|(?:['""](?'Key'\w+)[""']))\]", RegexOptions.Compiled);

        private static Expression GetProperty(Expression param, string propname)
        {
            Expression propExpression = param;
            String[] childProperties = propname.Split('.');
            var propertyType = param.Type;

            foreach (var childprop in childProperties)
            {
                var isIndexed = _regexIndexed.Match(childprop);
                if (isIndexed.Success)
                {
                    var indexType = typeof(int);
                    var collectionname = isIndexed.Groups["Collection"].Value;
                    var collectionProp = propertyType.GetProperty(collectionname);
                    if (collectionProp == null)
                        throw new RuleException(
                            $"Cannot find collection property {collectionname} in class {propertyType.Name} (\"{propname}\")");
                    var collexpr = Expression.PropertyOrField(propExpression, collectionname);

                    Expression expIndex;
                    if (isIndexed.Groups["Index"].Success)
                    {
                        var index = Int32.Parse(isIndexed.Groups["Index"].Value);
                        expIndex = Expression.Constant(index);
                    }
                    else
                    {
                        expIndex = Expression.Constant(isIndexed.Groups["Key"].Value);
                        indexType = typeof(string);
                    }

                    var collectionType = collexpr.Type;
                    if (collectionType.IsArray)
                    {
                        propExpression = Expression.ArrayAccess(collexpr, expIndex);
                        propertyType = propExpression.Type;
                    }
                    else
                    {
                        var getter = collectionType.GetMethod("get_Item", new Type[] { indexType });
                        if (getter == null)
                            throw new RuleException($"'{collectionname} ({collectionType.Name}) cannot be indexed");
                        propExpression = Expression.Call(collexpr, getter, expIndex);
                        propertyType = getter.ReturnType;
                    }
                }
                else
                {
                    var property = propertyType.GetProperty(childprop);
                    if (property == null)
                        throw new RuleException(
                                $"Cannot find property {childprop} in class {propertyType.Name} (\"{propname}\")");
                    propExpression = Expression.PropertyOrField(propExpression, childprop);
                    propertyType = property.PropertyType;
                }
            }

            return propExpression;
        }

        private static Expression BuildEnumerableOperatorExpression(Type type, Rule rule,
            ParameterExpression parameterExpression)
        {
            var collectionPropertyExpression = BuildExpr(type, rule, parameterExpression);

            var itemType = GetCollectionItemType(collectionPropertyExpression.Type);
            var expressionParameter = Expression.Parameter(itemType);

            var genericFunc = typeof(Func<,>).MakeGenericType(itemType, typeof(bool));

            var innerExp = BuildNestedExpression(itemType, rule.Rules, expressionParameter, ExpressionType.And);
            var predicate = Expression.Lambda(genericFunc, innerExp, expressionParameter);

            var body = Expression.Call(typeof(Enumerable), rule.Operator, new[] { itemType },
                collectionPropertyExpression, predicate);

            return body;
        }

        private static Type GetCollectionItemType(Type collectionType)
        {
            if (collectionType.IsArray)
                return collectionType.GetElementType();

            if ((collectionType.GetInterface("IEnumerable") != null))
                return collectionType.GetGenericArguments()[0];

            return typeof(object);
        }

        private static MethodInfo IsEnumerableOperator(string oprator)
        {
            return (from tup in _enumrMethodsByName
                    where string.Equals(oprator, tup.Item1, StringComparison.CurrentCultureIgnoreCase)
                    select tup.Item2.Value).FirstOrDefault();
        }

        private static Expression BuildExpr(Type type, Rule rule, Expression param, bool useTryCatch = true)
        {
            Expression propExpression;
            Type propType;

            if (param.Type == typeof(object))
            {
                param = Expression.TypeAs(param, type);
            }
            var drule = rule as DataRule;

            if (string.IsNullOrEmpty(rule.MemberName)) //check is against the object itself
            {
                propExpression = param;
                propType = propExpression.Type;
            }
            else if (drule != null)
            {
                if (type != typeof(System.Data.DataRow))
                    throw new RuleException("Bad rule");
                propExpression = GetDataRowField(param, drule.MemberName, drule.Type);
                propType = propExpression.Type;
            }
            else
            {
                propExpression = GetProperty(param, rule.MemberName);
                propType = propExpression.Type;
            }
            if (useTryCatch)
            {
                propExpression = Expression.TryCatch(
                    Expression.Block(propExpression.Type, propExpression),
                    Expression.Catch(typeof(NullReferenceException), Expression.Default(propExpression.Type))
                );
            }

            // is the operator a known .NET operator?
            ExpressionType tBinary;

            if (ExpressionType.TryParse(rule.Operator, out tBinary))
            {
                Expression right;
                var txt = rule.TargetValue as string;
                if (txt != null && txt.StartsWith("*."))
                {
                    txt = txt.Substring(2);
                    right = GetProperty(param, txt);
                }
                else
                    right = StringToExpression(rule.TargetValue, propType);

                return Expression.MakeBinary(tBinary, propExpression, right);
            }

            switch (rule.Operator)
            {
                case "IsMatch":
                    return Expression.Call(
                        _miRegexIsMatch.Value,
                        propExpression,
                        Expression.Constant(rule.TargetValue, typeof(string)),
                        Expression.Constant(RegexOptions.IgnoreCase, typeof(RegexOptions))
                    );

                case "IsInteger":
                    return Expression.Call(
                        _miIntTryParse.Value,
                        propExpression,
                        Expression.MakeMemberAccess(null, typeof(Placeholder).GetField("Int"))
                    );

                case "IsSingle":
                    return Expression.Call(
                        _miFloatTryParse.Value,
                        propExpression,
                        Expression.MakeMemberAccess(null, typeof(Placeholder).GetField("Float"))
                    );

                case "IsDouble":
                    return Expression.Call(
                        _miDoubleTryParse.Value,
                        propExpression,
                        Expression.MakeMemberAccess(null, typeof(Placeholder).GetField("Double"))
                    );

                case "IsDecimal":
                    return Expression.Call(
                        _miDecimalTryParse.Value,
                        propExpression,
                        Expression.MakeMemberAccess(null, typeof(Placeholder).GetField("Decimal"))
                    );

                case "IsInInput":
                    return Expression.Call(Expression.Constant(rule.Inputs.ToList()),
                                           _miListContains.Value,
                                           propExpression);

                default:
                    break;
            }

            var enumrOperation = IsEnumerableOperator(rule.Operator);
            if (enumrOperation != null)
            {
                var elementType = ElementType(propType);
                var lambdaParam = Expression.Parameter(elementType, "lambdaParam");
                return rule.Rules?.Any() == true
                    ? Expression.Call(enumrOperation.MakeGenericMethod(elementType),
                        propExpression,
                        Expression.Lambda(
                            BuildNestedExpression(elementType, rule.Rules, lambdaParam, ExpressionType.AndAlso),
                            lambdaParam)

                    )
                    : Expression.Call(enumrOperation.MakeGenericMethod(elementType), propExpression);
            }
            else //Invoke a method on the Property
            {
                var inputs = rule.Inputs.Select(x => x.GetType()).ToArray();
                var methodInfo = propType.GetMethod(rule.Operator, inputs);
                List<Expression> expressions = new List<Expression>();

                if (methodInfo == null)
                {
                    methodInfo = propType.GetMethod(rule.Operator);
                    if (methodInfo != null)
                    {
                        var parameters = methodInfo.GetParameters();
                        int i = 0;
                        foreach (var item in rule.Inputs)
                        {
                            expressions.Add(RuleFactory.StringToExpression(item, parameters[i].ParameterType));
                            i++;
                        }
                    }
                }
                else
                    expressions.AddRange(rule.Inputs.Select(Expression.Constant));
                if (methodInfo == null)
                    throw new RuleException($"'{rule.Operator}' is not a method of '{propType.Name}");

                if (!methodInfo.IsGenericMethod)
                    inputs = null; //Only pass in type information to a Generic Method
                var callExpression = Expression.Call(propExpression, rule.Operator, inputs, expressions.ToArray());
                if (useTryCatch)
                {
                    return Expression.TryCatch(
                    Expression.Block(typeof(bool), callExpression),
                    Expression.Catch(typeof(NullReferenceException), Expression.Constant(false))
                    );
                }
                else
                    return callExpression;
            }
        }

        private static MethodInfo GetLinqMethod(string name, int numParameter)
        {
            return typeof(Enumerable).GetMethods(BindingFlags.Static | BindingFlags.Public)
                .FirstOrDefault(m => m.Name == name && m.GetParameters().Length == numParameter);
        }

        private static Expression GetDataRowField(Expression prm, string member, string typeName)
        {
            var expMember = Expression.Call(prm, _miGetItem.Value, Expression.Constant(member, typeof(string)));
            var type = Type.GetType(typeName);
            Debug.Assert(type != null);

            if (type.IsClass || typeName.StartsWith("System.Nullable"))
            {
                //  equals "return  testValue == DBNull.Value  ? (typeName) null : (typeName) testValue"
                return Expression.Condition(Expression.Equal(expMember, Expression.Constant(DBNull.Value)),
                    Expression.Constant(null, type),
                    Expression.Convert(expMember, type));
            }
            else
                // equals "return (typeName) testValue"
                return Expression.Convert(expMember, type);
        }

        private static Expression StringToExpression(object value, Type propType)
        {
            Debug.Assert(propType != null);

            object safevalue;
            Type valuetype = propType;
            var txt = value as string;
            if (value == null)
            {
                safevalue = null;
            }
            else if (txt != null)
            {
                if (txt.ToLower() == "null")
                    safevalue = null;
                else if (propType.IsEnum)
                    safevalue = Enum.Parse(propType, txt);
                else
                {
                    if (propType.Name == "Nullable`1")
                        valuetype = Nullable.GetUnderlyingType(propType);

                    safevalue = IsTime(txt, propType) ?? Convert.ChangeType(value, valuetype);
                }
            }
            else
            {
                if (propType.Name == "Nullable`1")
                    valuetype = Nullable.GetUnderlyingType(propType);
                safevalue = Convert.ChangeType(value, valuetype);
            }

            return Expression.Constant(safevalue, propType);
        }

        private static readonly Regex reNow = new Regex(@"#NOW([-+])(\d+)([SMHDY])", RegexOptions.IgnoreCase
                                                                              | RegexOptions.Compiled
                                                                              | RegexOptions.Singleline);

        private static DateTime? IsTime(string text, Type targetType)
        {
            if (targetType != typeof(DateTime) && targetType != typeof(DateTime?))
                return null;

            var match = reNow.Match(text);
            if (!match.Success)
                return null;

            var amt = Int32.Parse(match.Groups[2].Value);
            if (match.Groups[1].Value == "-")
                amt = -amt;

            switch (Char.ToUpperInvariant(match.Groups[3].Value[0]))
            {
                case 'S':
                    return DateTime.Now.AddSeconds(amt);

                case 'M':
                    return DateTime.Now.AddMinutes(amt);

                case 'H':
                    return DateTime.Now.AddHours(amt);

                case 'D':
                    return DateTime.Now.AddDays(amt);

                case 'Y':
                    return DateTime.Now.AddYears(amt);
            }
            // it should not be possible to reach here.
            throw new ArgumentException();
        }

        private static Type ElementType(Type seqType)
        {
            Type ienum = FindIEnumerable(seqType);
            if (ienum == null) return seqType;
            return ienum.GetGenericArguments()[0];
        }

        private static Type FindIEnumerable(Type seqType)
        {
            if (seqType == null || seqType == typeof(string))
                return null;
            if (seqType.IsArray)
                return typeof(IEnumerable<>).MakeGenericType(seqType.GetElementType());
            if (seqType.IsGenericType)
            {
                foreach (Type arg in seqType.GetGenericArguments())
                {
                    Type ienum = typeof(IEnumerable<>).MakeGenericType(arg);
                    if (ienum.IsAssignableFrom(seqType))
                    {
                        return ienum;
                    }
                }
            }

            Type[] ifaces = seqType.GetInterfaces();
            foreach (Type iface in ifaces)
            {
                Type ienum = FindIEnumerable(iface);
                if (ienum != null)
                    return ienum;
            }

            if (seqType.BaseType != null && seqType.BaseType != typeof(object))
            {
                return FindIEnumerable(seqType.BaseType);
            }

            return null;
        }

        public enum OperatorType
        {
            InternalString = 1,
            ObjectMethod = 2,
            Comparison = 3,
            Logic = 4
        }

        public class Operator
        {
            public string Name { get; set; }
            public OperatorType Type { get; set; }
            public int NumberOfInputs { get; set; }
            public bool SimpleInputs { get; set; }
        }

        public class Member
        {
            public string Name { get; set; }
            public string Type { get; set; }
            public List<Operator> PossibleOperators { get; set; }

            public static bool IsSimpleType(Type type)
            {
                return
                    type.IsPrimitive ||
                    new Type[] {
                        typeof(Enum),
                        typeof(String),
                        typeof(Decimal),
                        typeof(DateTime),
                        typeof(DateTimeOffset),
                        typeof(TimeSpan),
                        typeof(Guid)
                    }.Contains(type) ||
                    Convert.GetTypeCode(type) != TypeCode.Object ||
                    (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>) && IsSimpleType(type.GetGenericArguments()[0]))
                    ;
            }

            public static BindingFlags flags = BindingFlags.Instance | BindingFlags.Public;

            public static List<Member> GetFields(Type type, string memberName = null, string parentPath = null)
            {
                List<Member> toReturn = new List<Member>();
                var fi = new Member
                {
                    Name = string.IsNullOrEmpty(parentPath) ? memberName : $"{parentPath}.{memberName}",
                    Type = type.ToString()
                };
                fi.PossibleOperators = Member.Operators(type, string.IsNullOrEmpty(fi.Name));
                toReturn.Add(fi);
                if (!Member.IsSimpleType(type))
                {
                    var fields = type.GetFields(Member.flags);
                    var properties = type.GetProperties(Member.flags);
                    foreach (var field in fields)
                    {
                        string useParentName = null;
                        var name = Member.ValidateName(field.Name, type, memberName, fi.Name, parentPath, out useParentName);
                        toReturn.AddRange(GetFields(field.FieldType, name, useParentName));
                    }
                    foreach (var prop in properties)
                    {
                        string useParentName = null;
                        var name = Member.ValidateName(prop.Name, type, memberName, fi.Name, parentPath, out useParentName);
                        toReturn.AddRange(GetFields(prop.PropertyType, name, useParentName));
                    }
                }
                return toReturn;
            }

            private static string ValidateName(string name, Type parentType, string parentName, string parentPath, string grandparentPath, out string useAsParentPath)
            {
                if (name == "Item" && IsGenericList(parentType))
                {
                    useAsParentPath = grandparentPath;
                    return parentName + "[0]";
                }
                else
                {
                    useAsParentPath = parentPath;
                    return name;
                }
            }

            public static bool IsGenericList(Type type)
            {
                if (type == null)
                {
                    throw new ArgumentNullException("type");
                }
                foreach (Type @interface in type.GetInterfaces())
                {
                    if (@interface.IsGenericType)
                    {
                        if (@interface.GetGenericTypeDefinition() == typeof(ICollection<>))
                        {
                            // if needed, you can also return the type used as generic argument
                            return true;
                        }
                    }
                }
                return false;
            }

            private static string[] logicOperators = new string[] {
                    RuleOperator.And.ToString("g"),
                    RuleOperator.AndAlso.ToString("g"),
                    RuleOperator.Or.ToString("g"),
                    RuleOperator.OrElse.ToString("g")
                };

            private static string[] comparisonOperators = new string[] {
                    RuleOperator.Equal.ToString("g"),
                    RuleOperator.GreaterThan.ToString("g"),
                    RuleOperator.GreaterThanOrEqual.ToString("g"),
                    RuleOperator.LessThan.ToString("g"),
                    RuleOperator.LessThanOrEqual.ToString("g"),
                    RuleOperator.NotEqual.ToString("g"),
                };

            private static string[] hardCodedStringOperators = new string[] {
                    RuleOperator.IsMatch.ToString("g"),
                    RuleOperator.IsInteger.ToString("g"),
                    RuleOperator.IsSingle.ToString("g"),
                    RuleOperator.IsDouble.ToString("g"),
                    RuleOperator.IsDecimal.ToString("g"),
                    RuleOperator.IsInInput.ToString("g"),
                };

            public static List<Operator> Operators(Type type, bool addLogicOperators = false, bool noOverloads = true)
            {
                List<Operator> operators = new List<Operator>();
                if (addLogicOperators)
                {
                    operators.AddRange(logicOperators.Select(x => new Operator() { Name = x, Type = OperatorType.Logic }));
                }

                if (type == typeof(String))
                {
                    operators.AddRange(hardCodedStringOperators.Select(x => new Operator() { Name = x, Type = OperatorType.InternalString }));
                }
                else if (Member.IsSimpleType(type))
                {
                    operators.AddRange(comparisonOperators.Select(x => new Operator() { Name = x, Type = OperatorType.Comparison }));
                }
                var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance);
                foreach (var method in methods)
                {
                    if (method.ReturnType == typeof(Boolean) && !method.Name.StartsWith("get_") && !method.Name.StartsWith("set_") && !method.Name.StartsWith("_op"))
                    {
                        var paramaters = method.GetParameters();
                        var op = new Operator()
                        {
                            Name = method.Name,
                            Type = OperatorType.ObjectMethod,
                            NumberOfInputs = paramaters.Length,
                            SimpleInputs = paramaters.All(x => Member.IsSimpleType(x.ParameterType))
                        };
                        if (noOverloads)
                        {
                            var existing = operators.FirstOrDefault(x => x.Name == op.Name && x.Type == op.Type);
                            if (existing == null)
                                operators.Add(op);
                            else if (existing.NumberOfInputs > op.NumberOfInputs)
                            {
                                operators[operators.IndexOf(existing)] = op;
                            }
                        }
                        else
                            operators.Add(op);
                    }
                }
                return operators;
            }
        }
    }

}
