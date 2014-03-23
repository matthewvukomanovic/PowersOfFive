using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Ross.Infrastructure.Interfaces;

namespace System
{
    public static class Extensions
    {
        /// <summary>
        /// Throws an <see cref="ArgumentNullException"/> if the object is null
        /// </summary>
        /// <param name="obj">The object to test</param>
        /// <param name="paramName">Name of the parameter.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        public static void IfNullThrow(this object obj, string paramName)
        {
            if (obj == null)
                throw new ArgumentNullException(paramName);
        }

        public static T GetAttributeValue<T>(this Enum enumeration) where T : Attribute
        {
            return enumeration.GetType().GetMember(enumeration.ToString())[0].GetCustomAttributes(typeof(T), false).Cast<T>().SingleOrDefault();
        }

        public static string AppendString(this string original, string appendValue, string separator)
        {
            if (!String.IsNullOrEmpty(original))
            {
                return original + separator + appendValue;
            }
            return appendValue;
        }

        public static void Append(this StringBuilder builder, string value, string separator)
        {
            if (builder.Length != 0)
            {
                builder.Append(separator);
            }
            builder.Append(value);
        }

        public static bool ToBoolean(this string s)
        {
            switch (s.ToLower(CultureInfo.InvariantCulture))
            {
                case "y":
                case "yes":
                case "1":
                case "t":
                case "true":
                    return true;
                default:
                    return false;
            }
        }

        public static bool ToBoolean(this char s)
        {
            switch (s)
            {
                case 'y':
                case 'Y':
                case '1':
                case 't':
                case 'T':
                    return true;
                default:
                    return false;
            }
        }

        public static string ToDbString(this bool b)
        {
            if( b)
                return "Y";
            return "N";
        }

        public static string ToNormalString(this bool b)
        {
            if (b)
                return "Yes";
            return "No";
        }

        public static string ToDbString(this bool? b)
        {
            if (b == null)
                return null;
            if (b.Value)
                return "Y";
            return "N";
        }

        public static int ExtensionFindIndex<T>(this IList<T> list, Predicate<T> match)
        {
            return ExtensionFindIndex(list, 0, list.Count, match);
        }

        public static int ExtensionFindIndex<T>(this IList<T> list, int startIndex, Predicate<T> match)
        {
            return ExtensionFindIndex(list, startIndex, list.Count - startIndex, match);
        }

        public static int ExtensionFindIndex<T>(this IList<T> list, int startIndex, int count, Predicate<T> match)
        {
            if (startIndex > list.Count)
            {
                throw new ArgumentOutOfRangeException("startIndex");
            }
            if ((count < 0) || (startIndex > (list.Count - count)))
            {
                throw new ArgumentOutOfRangeException("count");
            }
            if (match == null)
            {
                throw new ArgumentNullException("match");
            }
            int num = startIndex + count;
            for (int i = startIndex; i < num; i++)
            {
                if (match(list[i]))
                {
                    return i;
                }
            }
            return -1;
        }

        public static T ReplaceItemAtIndex<T>(this IList<T> list, int index, T item)
        {
            var original = list[index];
            list[index] = item;
            return original;
        }

        public static bool ReplaceItemAtMatch<T>(this IList<T> list, T item, Predicate<T> match)
        {
            T throwAway;
            return ReplaceItemAtMatch(list, item, match, out throwAway);
        }

        public static bool ReplaceItemAtMatch<T>(this IList<T> list, T item, Predicate<T> match, out T original)
        {
            var index = list.ExtensionFindIndex(match);
            if (index >= 0)
            {
                original = ReplaceItemAtIndex(list, index, item);
                return true;
            }
            original = default(T);
            return false;
        }

        public static bool AddOrReplaceItemAtMatch<T>(this IList<T> list, T item, Predicate<T> match)
        {
            T original;
            return AddOrReplaceItemAtMatch(list, item, match, out original);
        }

        public static bool AddOrReplaceItemAtMatch<T>(this IList<T> list, T item, Predicate<T> match, out T original)
        {
            var index = list.ExtensionFindIndex(match);
            if (index >= 0)
            {
                original = ReplaceItemAtIndex(list, index, item);
                return true;
            }

            list.Add(item);
            original = default(T);
            return false;
        }

        public static DateTime TrimSeconds(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, date.Day, date.Hour, date.Minute, 0);
        }

        public static List<T> ToList<T>(this Dictionary<int,T> dictionary)
        {
            var list = new List<T>(dictionary.Count);
            foreach (var pair in dictionary)
                list.Add(pair.Value);
            return list;
        }

        public static Dictionary<int, T> ToDictionary<T>(this IEnumerable<T> list) where T : IIdentifiable
        {
            return ToDictionary<int, T>(list);
        }

        public static Dictionary<TKey, T> ToDictionary<TKey,T>(this IEnumerable<T> list) where T : IIdentifiable<TKey>
        {
            var dictionary = new Dictionary<TKey, T>();
            foreach (T item in list)
                dictionary.Add(item.Id, item);
            return dictionary;
        }

        public static List<int> ToIdList<T>(this IEnumerable<T> list) where T : IIdentifiable
        {
            return ToIdList<int, T>(list);
        }

        public static List<TKey> ToIdList<TKey,T>(this IEnumerable<T> list) where T : IIdentifiable<TKey>
        {
            if( list == null)
                return new List<TKey>();

            var ret = new List<TKey>();
            foreach (T item in list)
                ret.Add(item.Id);
            return ret;
        }

        public static string ToDelimiteredString<T>(this IEnumerable<T> list, string delimiter = ",")
        {
            var b = new StringBuilder();
            foreach (var item in list)
            {
                b.Append(item.ToString(), delimiter);
            }

            return b.ToString();
        }

        public static string ToDelimiteredString<T>(this IList<T> list, int offset, int length = -1, string delimiter = ",")
        {
            if (list.Count <= offset || offset < 0)
            {
                throw new IndexOutOfRangeException("Can not create delimetered string out of the range of the collection");
            }

            if (length < 0)
            {
                length = list.Count - offset;
            }
            else if( offset + length > list.Count)
            {
                throw new IndexOutOfRangeException("Can not create delimetered string out of the range of the collection, length is too long for the collection");
            }

            int finalOffset = offset + length;


            var b = new StringBuilder();
            for (int i = offset; i < finalOffset; i++)
            {

                b.Append(list[i].ToString(), delimiter);
            }

            return b.ToString();
        }

        public static bool ContainsItemFromList<T>(this IEnumerable<T> compareList, IList<T> list)
        {
            if (list == null || compareList == null)
                return false;
            foreach (var id in compareList)
            {
                if (list.Contains(id))
                {
                    return true;
                }
            }
            return false;
        }

        public static string GetPropertyName<T>(Expression<Func<T>> expression)
        {
            return ((MemberExpression)expression.Body).Member.Name;
        }

        public static string GetPropertyName<T, TReturn>(Expression<Func<T, TReturn>> expression)
        {
            return ((MemberExpression)expression.Body).Member.Name;
        }

        public static string ToSafeSql(this string sql)
        {
            if (sql != null)
            {
                return sql.Replace("'", "''");
            }
            return sql;
        }
        
        public static List<PropertyDescriptor> GetDescriptorListForProperty(Type collectionType, string propertyName)
        {
            var descriptors = new List<PropertyDescriptor>();
            var propertyNames = propertyName.Split('.');

            for (int i = 0; i < propertyNames.Length; i++)
            {
                if (i == 0)
                    descriptors.Add(GetPropertyDescriptor(collectionType, propertyNames[0]));
                else
                    descriptors.Add(GetChildPropertyDescriptor(descriptors[i - 1], propertyNames[i]));
            }

            return descriptors;
        }

        public static object GetValueFromDescriptorList(object item, List<PropertyDescriptor> descriptors)
        {
            //	var value = filterChildDescriptor != null && filterPropDesc.GetValue(item) != null
            //		? filterChildDescriptor.GetValue(filterPropDesc.GetValue(item)) : filterPropDesc.GetValue(item);

            object currentValue = item;
            foreach (var descriptor in descriptors)
            {
                currentValue = descriptor.GetValue(currentValue);

                if (currentValue == null)
                    break;
            }

            return currentValue;
        }

        private static PropertyDescriptor GetPropertyDescriptor(Type collectionItemType, string propertyName)
        {
            return TypeDescriptor.GetProperties(collectionItemType)[propertyName];
        }

        private static PropertyDescriptor GetChildPropertyDescriptor(PropertyDescriptor property, string propertyName)
        {
            if (property == null || propertyName == null || propertyName == string.Empty)
                return null;
            return property.GetChildProperties()[propertyName];
        }

        public static int IdentifiableToId(IIdentifiable e)
        {
            return e.Id;
        }
    }
}