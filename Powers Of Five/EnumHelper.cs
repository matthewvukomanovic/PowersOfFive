using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Ross.Infrastructure.Attributes;

namespace Ross.Infrastructure.Helpers
{
    public static class EnumHelper
    {

        public static TEnum TryParseEnum<TEnum>(string value, TEnum defaultValue) where TEnum : struct
        {
            return TryParseEnum(value, false, defaultValue);
        }

        public static TEnum TryParseEnum<TEnum>(string value, bool ignoreCase, TEnum defaultValue) where TEnum : struct
        {
            TEnum result;
            if (!Enum.TryParse(value, ignoreCase, out result) || !Enum.IsDefined(typeof(TEnum), result))
                return defaultValue;

            return result;
        }

        public static TEnum ParseEnum<TEnum>(string value, bool ignoreCase) where TEnum : struct
        {
            TEnum returnValue;
            if (!Enum.TryParse(value, ignoreCase, out returnValue) || !Enum.IsDefined(typeof(TEnum), returnValue))
            {
                throw new InvalidCastException(string.Format("{0} is not valid for type of '{1}'", value, typeof(TEnum)));
            }

            return returnValue;
        }

        /// <summary>
        ///used for parsing database value to enum using the description of the enum .
        ///this is for when there are spaces in the db value.
        ///if no description then it falls back to using the value.
        /// </summary>
        /// <typeparam name="TEnum">The type of the enum.</typeparam>
        /// <param name="description">The description.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>The value converted to an enumeration</returns>
        /// <exception cref="System.ArgumentException">If the type isn't an enumeration</exception>
        public static TEnum TryParseEnumFromDescription<TEnum>(string description, TEnum defaultValue) where TEnum : struct
        {
            var type = typeof(TEnum);
            if (!type.IsEnum)
                throw new ArgumentException();
            FieldInfo[] fields = type.GetFields();
            var field = fields.SelectMany(f => f.GetCustomAttributes(typeof(DescriptionAttribute), false), (f, a) => new { Field = f, Att = a }).SingleOrDefault(a => ((DescriptionAttribute)a.Att).Description == description);
            return field == null ? TryParseEnum(description, defaultValue) : (TEnum)field.Field.GetRawConstantValue();
        }

        /// <summary>
        ///used for parsing database value to enum using the description of the enum .
        ///this is for when there are spaces in the db value.
        ///if no description then it falls back to using the value.
        /// </summary>
        /// <typeparam name="TEnum">The type of the enum.</typeparam>
        /// <param name="representation">The description.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>The value converted to an enumeration</returns>
        /// <exception cref="System.ArgumentException">If the type isn't an enumeration</exception>
        public static TEnum TryParseEnumFromDatabaseRepresentation<TEnum>(string representation, TEnum defaultValue) where TEnum : struct
        {
            var type = typeof(TEnum);
            if (!type.IsEnum)
                throw new ArgumentException();
            FieldInfo[] fields = type.GetFields();
            var field = fields.SelectMany(f => f.GetCustomAttributes(typeof(DatabaseRepresentationAttribute), false), (f, a) => new { Field = f, Att = a }).SingleOrDefault(a => ((DatabaseRepresentationAttribute)a.Att).Representation == representation);
            return field == null ? TryParseEnum(representation, defaultValue) : (TEnum)field.Field.GetRawConstantValue();
        }

        public static dynamic ParseEnum(Type toType, string value, bool ignoreCase)
        {
            dynamic result = Enum.Parse(toType, value, ignoreCase);
            if (!Enum.IsDefined(toType, result))
            {
                throw new InvalidCastException(string.Format("{0} ({2}) is not valid for type of '{1}'", value, toType, typeof(string)));
            }
            return result;
        }

        public static dynamic ToEnumType(Type toType, string value, bool ignoreCase)
        {
            dynamic returnValue = Enum.Parse(toType, value, ignoreCase);
            if (!Enum.IsDefined(toType, returnValue))
            {
                throw new InvalidCastException(string.Format("{0} ({2}) is not valid for type of '{1}'", value, toType, typeof(string)));
            }
            return returnValue;
        }

        public static dynamic ToEnumType(Type toType, Type fromType, object value)
        {
            return ToEnumType(toType, fromType, value, ThrowWrongType);
        }

        private static dynamic ThrowWrongType(Type toType, Type fromType, object value)
        {
            throw new InvalidCastException(string.Format("{0} ({2}) is not valid for type of '{1}'", value, toType, fromType));
        }

        public static dynamic ToEnumType(Type toType, object value)
        {
            if (value == null)
                throw new InvalidCastException("Trying to create enum from null");
            return ToEnumType(toType, value.GetType(), value);
        }

        public static TEnum ToEnum<TEnum>(object value)
            where TEnum : struct
        {
            return (TEnum)ToEnumType(typeof(TEnum), value);
        }

        public static TEnum ToEnum<TEnum, TFromType>(TFromType value)
            where TEnum : struct
        {
            return (TEnum)ToEnumType(typeof(TEnum), typeof(TFromType), value);
        }

        //get description attribute (if set) from the enum - otherwise just return tostring
        public static string GetEnumDescription(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());
            var attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes.Length > 0 ? attributes[0].Description : value.ToString();
        }

        //get description attribute (if set) from the enum - otherwise just return tostring
        public static string GetEnumDatabaseRepresentation(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());
            var attributes = (DatabaseRepresentationAttribute[])fi.GetCustomAttributes(typeof(DatabaseRepresentationAttribute), false);
            return attributes.Length > 0 ? attributes[0].Representation : value.ToString();
        }

        public static dynamic ToEnumType(Type toType, Type fromType, object value, object defaultValue)
        {
            return ToEnumType(toType, fromType, value, (t, f, v) =>
            {
                if (Enum.IsDefined(toType, defaultValue))
                {
                    return Enum.ToObject(toType, defaultValue);
                }
                else
                {
                    throw new InvalidCastException(string.Format("{0} ({2}) and {3} ({4}) is not valid for type of '{1}'", value, toType, fromType, defaultValue, defaultValue.GetType()));
                }
            });
        }

        public static dynamic ToEnumType(Type toType, object value, object defaultValue)
        {
            if (value == null)
                return defaultValue;
            return ToEnumType(toType, value.GetType(), value, defaultValue);
        }

        public static TEnum ToEnum<TEnum>(object value, object defaultValue)
            where TEnum : struct
        {
            return (TEnum)ToEnumType(typeof(TEnum), value, defaultValue);
        }

        public static TEnum ToEnum<TEnum, TFromType>(TFromType value, TEnum defaultValue)
            where TEnum : struct
        {
            return (TEnum)ToEnumType(typeof(TEnum), typeof(TFromType), value, defaultValue);
        }

        public static dynamic ToUnderlyingType(object value)
        {
            if (!(value is Enum)) return value;

            var underlying = Enum.GetUnderlyingType(value.GetType());
            return Convert.ChangeType(value, underlying);
        }

        public static dynamic ToEnumType(Type toType, Type fromType, object value, Func<Type, Type, object,dynamic> invalidValueAction)
        {
            Type underlying = Enum.GetUnderlyingType(toType);

            bool straightAssign = false;

            if (underlying == fromType)
            {
                straightAssign = true;
            }
            else if (value is IConvertible)
            {
                if (value is string)
                {
                }
                else
                {
                    value = Convert.ChangeType(value, underlying);
                    straightAssign = true;
                }
            }

            if (straightAssign)
            {
                if (Enum.IsDefined(toType, value))
                {
                    value = Enum.ToObject(toType, value);
                }
                else
                {
                    return invalidValueAction(toType, fromType, value);
                }
                return value;
            }
            try
            {
                return ParseEnum(toType, value.ToString(), true);
            }
            catch (Exception)
            {
                return invalidValueAction(toType, typeof (string), value);
            }
        }

        public static dynamic ToEnumType(Type toType, object value, Func<Type, Type, object, dynamic> invalidValueAction)
        {
            if (value == null)
                throw new InvalidCastException("Trying to create enum from null");
            return ToEnumType(toType, value.GetType(), value, invalidValueAction);
        }

        public static TEnum ToEnum<TEnum>(object value, Func<Type, Type, object, dynamic> invalidValueAction)
            where TEnum : struct
        {
            return (TEnum)ToEnumType(typeof(TEnum), value, invalidValueAction);
        }

        public static TEnum ToEnum<TEnum, TFromType>(TFromType value, Func<Type, Type, object, dynamic> invalidValueAction)
            where TEnum : struct
        {
            return (TEnum)ToEnumType(typeof(TEnum), typeof(TFromType), value, invalidValueAction);
        }

    }
}
