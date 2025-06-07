using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;

namespace DrawnUi.Draw
{
    public static partial class FluentExtensions
    {
        // Single property WhenAnyValue
        /// <summary>
        /// Observes a single property using expression-based syntax similar to ReactiveUI WhenAnyValue
        /// iOS AOT compatible - uses reflection instead of expression compilation
        /// </summary>
        public static T WhenAnyValue<T, TViewModel, TProperty>(
            this T control,
            Expression<Func<TViewModel, TProperty>> propertyExpression,
            Action<TProperty> onValueChanged)
            where T : SkiaControl
            where TViewModel : class, INotifyPropertyChanged
        {
            var propertyName = ExtractPropertyName(propertyExpression);

            return control.ObserveBindingContext<T, TViewModel>((ctrl, vm, prop) =>
            {
                bool attached = prop == nameof(BindableObject.BindingContext);
                if (attached || prop == propertyName)
                {
                    var value = GetPropertyValueViaReflection<TViewModel, TProperty>(propertyExpression, vm);
                    onValueChanged(value);
                }
            });
        }

        // Two properties WhenAnyValue with selector
        /// <summary>
        /// Observes two properties and combines them using a selector function
        /// iOS AOT compatible
        /// </summary>
        public static T WhenAnyValue<T, TViewModel, T1, T2, TResult>(
            this T control,
            Expression<Func<TViewModel, T1>> property1,
            Expression<Func<TViewModel, T2>> property2,
            Func<T1, T2, TResult> selector,
            Action<TResult> onValueChanged)
            where T : SkiaControl
            where TViewModel : class, INotifyPropertyChanged
        {
            var prop1Name = ExtractPropertyName(property1);
            var prop2Name = ExtractPropertyName(property2);

            return control.ObserveBindingContext<T, TViewModel>((ctrl, vm, prop) =>
            {
                bool attached = prop == nameof(BindableObject.BindingContext);
                if (attached || prop == prop1Name || prop == prop2Name)
                {
                    var value1 = GetPropertyValueViaReflection<TViewModel, T1>(property1, vm);
                    var value2 = GetPropertyValueViaReflection<TViewModel, T2>(property2, vm);
                    var result = selector(value1, value2);
                    onValueChanged(result);
                }
            });
        }

        // Three properties WhenAnyValue with selector
        /// <summary>
        /// Observes three properties and combines them using a selector function
        /// iOS AOT compatible
        /// </summary>
        public static T WhenAnyValue<T, TViewModel, T1, T2, T3, TResult>(
            this T control,
            Expression<Func<TViewModel, T1>> property1,
            Expression<Func<TViewModel, T2>> property2,
            Expression<Func<TViewModel, T3>> property3,
            Func<T1, T2, T3, TResult> selector,
            Action<TResult> onValueChanged)
            where T : SkiaControl
            where TViewModel : class, INotifyPropertyChanged
        {
            var prop1Name = ExtractPropertyName(property1);
            var prop2Name = ExtractPropertyName(property2);
            var prop3Name = ExtractPropertyName(property3);

            return control.ObserveBindingContext<T, TViewModel>((ctrl, vm, prop) =>
            {
                bool attached = prop == nameof(BindableObject.BindingContext);
                if (attached || prop == prop1Name || prop == prop2Name || prop == prop3Name)
                {
                    var value1 = GetPropertyValueViaReflection<TViewModel, T1>(property1, vm);
                    var value2 = GetPropertyValueViaReflection<TViewModel, T2>(property2, vm);
                    var value3 = GetPropertyValueViaReflection<TViewModel, T3>(property3, vm);
                    var result = selector(value1, value2, value3);
                    onValueChanged(result);
                }
            });
        }

        // Alternative syntax without selector for single property
        /// <summary>
        /// Observes a single property and applies it directly to a control property
        /// iOS AOT compatible
        /// </summary>
        public static T WhenAnyValue<T, TViewModel, TProperty>(
            this T control,
            Expression<Func<TViewModel, TProperty>> sourceProperty,
            Expression<Func<T, TProperty>> targetProperty)
            where T : SkiaControl
            where TViewModel : class, INotifyPropertyChanged
        {
            var sourcePropName = ExtractPropertyName(sourceProperty);
            var targetPropInfo = ExtractPropertyInfo(targetProperty);

            return control.ObserveBindingContext<T, TViewModel>((ctrl, vm, prop) =>
            {
                bool attached = prop == nameof(BindableObject.BindingContext);
                if (attached || prop == sourcePropName)
                {
                    var value = GetPropertyValueViaReflection<TViewModel, TProperty>(sourceProperty, vm);
                    targetPropInfo.SetValue(ctrl, value);
                }
            });
        }

        // WhenAny with nested property support (like WhenAnyValue but with null propagation)
        /// <summary>
        /// Observes a property chain with automatic null propagation handling
        /// iOS AOT compatible
        /// </summary>
        public static T WhenAny<T, TViewModel, TProperty>(
            this T control,
            Expression<Func<TViewModel, TProperty>> propertyExpression,
            Action<TProperty> onValueChanged,
            TProperty defaultValue = default(TProperty))
            where T : SkiaControl
            where TViewModel : class, INotifyPropertyChanged
        {
            var propertyChain = ExtractPropertyChain(propertyExpression);

            return control.ObserveBindingContext<T, TViewModel>((ctrl, vm, prop) =>
            {
                bool attached = prop == nameof(BindableObject.BindingContext);
                if (attached || propertyChain.Any(p => p.Name == prop))
                {
                    try
                    {
                        var value = GetNestedPropertyValueSafe(propertyChain, vm, defaultValue);
                        onValueChanged(value);
                    }
                    catch
                    {
                        onValueChanged(defaultValue);
                    }
                }
            });
        }

        #region WhenAnyValue Helper Methods - iOS AOT Safe

        private static readonly ConcurrentDictionary<Expression, string> _reactivePropertyNameCache = new();
        private static readonly ConcurrentDictionary<Expression, PropertyInfo> _reactivePropertyInfoCache = new();
        private static readonly ConcurrentDictionary<Type, ConcurrentDictionary<string, PropertyInfo>> _typePropertyCache = new();

        /// <summary>
        /// Extracts property name from expression - iOS AOT safe
        /// </summary>
        private static string ExtractPropertyName<T, TProperty>(Expression<Func<T, TProperty>> expression)
        {
            return _reactivePropertyNameCache.GetOrAdd(expression, expr =>
            {
                var lambdaExpr = (LambdaExpression)expr;

                if (lambdaExpr.Body is MemberExpression memberExpr)
                {
                    return memberExpr.Member.Name;
                }

                // Handle property access on converted expressions like (int)enumValue
                if (lambdaExpr.Body is UnaryExpression unaryExpr && unaryExpr.Operand is MemberExpression memberOperand)
                {
                    return memberOperand.Member.Name;
                }

                throw new ArgumentException($"Expression must be a property access. Got: {lambdaExpr.Body.GetType().Name}");
            });
        }

        /// <summary>
        /// Gets PropertyInfo from expression - iOS AOT safe
        /// </summary>
        private static PropertyInfo ExtractPropertyInfo<T, TProperty>(Expression<Func<T, TProperty>> expression)
        {
            return _reactivePropertyInfoCache.GetOrAdd(expression, expr =>
            {
                var lambdaExpr = (LambdaExpression)expr;

                if (lambdaExpr.Body is MemberExpression memberExpr && memberExpr.Member is PropertyInfo propInfo)
                {
                    return propInfo;
                }

                throw new ArgumentException("Expression must be a property access");
            });
        }

        /// <summary>
        /// Gets property value using reflection - iOS AOT compatible
        /// Much safer than Expression.Compile() for AOT scenarios
        /// </summary>
        private static TProperty GetPropertyValueViaReflection<TSource, TProperty>(
            Expression<Func<TSource, TProperty>> expression,
            TSource source)
        {
            if (source == null)
                return default(TProperty);

            var propertyInfo = ExtractPropertyInfo(expression);
            var value = propertyInfo.GetValue(source);

            // Handle type conversion if needed
            if (value == null)
                return default(TProperty);

            if (value is TProperty directValue)
                return directValue;

            // Handle conversions (like enum to int, etc.)
            try
            {
                return (TProperty)Convert.ChangeType(value, typeof(TProperty));
            }
            catch
            {
                return default(TProperty);
            }
        }

        /// <summary>
        /// Gets PropertyInfo for a type and property name - cached for performance
        /// iOS AOT safe
        /// </summary>
        private static PropertyInfo GetCachedPropertyInfo(Type type, string propertyName)
        {
            var typeCache = _typePropertyCache.GetOrAdd(type, _ => new ConcurrentDictionary<string, PropertyInfo>());
            return typeCache.GetOrAdd(propertyName, name => type.GetProperty(name));
        }

        /// <summary>
        /// Extracts property chain for nested properties - iOS AOT safe
        /// </summary>
        private static PropertyChainInfo[] ExtractPropertyChain<T, TProperty>(Expression<Func<T, TProperty>> expression)
        {
            var properties = new List<PropertyChainInfo>();
            var lambdaExpr = (LambdaExpression)expression;
            var current = lambdaExpr.Body;
            var currentType = typeof(T);

            while (current is MemberExpression memberExpr)
            {
                if (memberExpr.Member is PropertyInfo propInfo)
                {
                    properties.Add(new PropertyChainInfo
                    {
                        Name = propInfo.Name,
                        PropertyInfo = propInfo,
                        DeclaringType = currentType
                    });
                    currentType = propInfo.PropertyType;
                }
                current = memberExpr.Expression;
            }

            properties.Reverse();
            return properties.ToArray();
        }

        /// <summary>
        /// Gets nested property value with null safety - iOS AOT safe
        /// </summary>
        private static TProperty GetNestedPropertyValueSafe<TProperty>(
            PropertyChainInfo[] propertyChain,
            object source,
            TProperty defaultValue)
        {
            if (source == null || propertyChain.Length == 0)
                return defaultValue;

            object current = source;

            try
            {
                foreach (var property in propertyChain)
                {
                    if (current == null)
                        return defaultValue;

                    current = property.PropertyInfo.GetValue(current);
                }

                if (current is TProperty result)
                    return result;

                // Handle conversions
                if (current != null)
                {
                    return (TProperty)Convert.ChangeType(current, typeof(TProperty));
                }
            }
            catch (NullReferenceException)
            {
                return defaultValue;
            }
            catch (ArgumentNullException)
            {
                return defaultValue;
            }
            catch (InvalidCastException)
            {
                return defaultValue;
            }

            return defaultValue;
        }

        /// <summary>
        /// Helper class for property chain information
        /// </summary>
        private class PropertyChainInfo
        {
            public string Name { get; set; }
            public PropertyInfo PropertyInfo { get; set; }
            public Type DeclaringType { get; set; }
        }

        #endregion
    }
}
