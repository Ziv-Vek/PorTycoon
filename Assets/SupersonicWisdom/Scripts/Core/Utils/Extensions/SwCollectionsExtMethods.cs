using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace SupersonicWisdomSDK
{
    internal static class SwCollectionsExtMethods
    {
        #region --- Public Methods ---

        /// <summary>
        /// This method modifies the stack and sorts it, the first element popped will be the smallest by default (controlled by ascending)
        /// </summary>
        /// <param name="stack">any stack to sort</param>
        /// <param name="ascending">if true the first element popped will be the smallest</param>
        /// <typeparam name="T">could be anything as long as it's comparable</typeparam>
        internal static void SortStack<T>(this Stack<T> stack, bool ascending = true) where T : IComparable<T>
        {
            List<T> list = stack.ToList();
            list.Sort();

            if (ascending)
            {
                list.Reverse();
            }
            
            stack.Clear();
            foreach (T element in list)
            {
                stack.Push(element);
            }
        }
        
        internal static void SwAdd<TValue>(this HashSet<TValue> self, TValue addition)
        {
            self.Add(addition);
        }

        internal static void SwForEach<TValue>(this IEnumerable<TValue> self, Action<TValue> action)
        {
            foreach (var item in self)
            {
                action(item);
            }
        }
        
        internal static void SwAddAll<TValue>(this IList<TValue> self, IEnumerable<TValue> other)
        {
            other.SwForEach(self.Add);
        }

        internal static bool SwIsEmpty(this ICollection self)
        {
            return self.Count == 0;
        }
        
        internal static TSource FirstOr<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate, TSource defaultValue)
        {
            var result = defaultValue;

            try
            {
                result = source.First(predicate);
            }
            catch (Exception)
            {
                //Console.WriteLine(e);
                // Ignore the exception and return the default value...
            }

            return result;
        }

        /// Returns the `defaultValue` in case: the key doesn't exists / it holds to a null value.
        internal static bool SwAddOrReplace<TKey, TValue>(this Dictionary<TKey, TValue> self, TKey key, TValue value)
        {
            if (key == null) return false;

            if (self.ContainsKey(key))
            {
                self.Remove(key);
            }

            self.Add(key, value);

            return true;
        }

        /// <summary>
        ///     Merge dictionaries extension
        ///     The last source keys overrides the first source keys
        /// </summary>
        /// <param name="self"></param>
        /// <param name="overwriteValue"></param>
        /// <param name="sources"></param>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <returns></returns>
        internal static Dictionary<TKey, TValue> SwMerge<TKey, TValue>(this Dictionary<TKey, TValue> self, bool overwriteValue, params Dictionary<TKey, TValue>[] sources)
        {
            foreach (var source in sources)
            {
                if (source != null)
                {
                    foreach (var keyValuePair in source)
                    {
                        if (overwriteValue || !self.ContainsKey(keyValuePair.Key))
                        {
                            self[keyValuePair.Key] = keyValuePair.Value;
                        }
                    }
                }
            }

            return self;
        }
        
        internal static bool SwContains<TKey, TValue>(this Dictionary<TKey, TValue> self, TKey key)
        {
            return key != null && self != null && self.ContainsKey(key);
        }
        
        /// Returns the `defaultValue` in case: the key doesn't exists / it holds to a null value.
        internal static TValue SwSafelyGet<TKey, TValue>(this IDictionary<TKey, TValue> self, TKey key, [CanBeNull] TValue defaultValue)
        {
            if (self == null || key == null) return defaultValue;
            if (!self.ContainsKey(key)) return defaultValue;
            self.TryGetValue(key, out var value);

            return value == null ? defaultValue : value;
        }

        internal static string SwToString<TSource>(this IEnumerable<TSource> source)
        {
            return JsonConvert.SerializeObject(source);
        }
        
        internal static HashSet<TSource> SwToHashSet<TSource>(this IEnumerable<TSource> source)
        {
            return new HashSet<TSource>(source);
        }
        
        internal static TSource SwSafelyGet<TSource>(this IEnumerable<TSource> source, int index, TSource defaultValue)
        {
            if (source == null) return defaultValue;
            var arrOrNull = source as TSource[];
            var arr = arrOrNull ?? source.ToArray();

            return arr.Length > index ? arr[index] : defaultValue;
        }

        internal static string SwToJsonString(this Dictionary<string, object> self)
        {
            return SwJsonParser.Serialize(self);
        }

        public static void FillWith<TEnum, S>(this IDictionary<TEnum, S> dict, S value) where TEnum : struct, Enum
        {
            foreach (TEnum enumValue in Enum.GetValues(typeof(TEnum)))
            {
                try
                {
                    dict[enumValue] = value;
                }
                catch (Exception e)
                {
                    SwInfra.Logger.LogError(EWisdomLogType.Utils, e);
                }
            }
        }

        /// <summary>
        /// Converts the value of this instance to its equivalent string representation using the formatting conventions of the invariant culture.
        /// Applicable for all types implementing IConvertible.
        /// </summary>
        /// <param name="value">The object to be converted to string</param>
        /// <returns>A string representation of value, formatted by the format specification of the invariant culture.</returns>
        public static string SwToString<T>(this T value) where T : IConvertible
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }

        #endregion
    }
}