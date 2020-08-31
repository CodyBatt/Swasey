using System;
using System.ComponentModel;
using System.Linq;

namespace Swasey.Normalization
{
    internal class SimpleNormalizationApiDataType : NormalizationApiDataType
    {

        public SimpleNormalizationApiDataType(dynamic jObject)
        {
            JObject = jObject;
        }

        #region Utilities

        internal static SimpleNormalizationApiDataType ParseFromJObject(object obj)
        {
            dynamic prop = obj;
            var type = new SimpleNormalizationApiDataType(prop);

            type.TryParseTypeFromJObject(obj);
            type.TryParseEnumFromJObject(obj);

            if(prop.ContainsKey("schema"))
            {
                prop = prop.schema;
                ApplyPropertyAttributes(prop, type);
            }
            else
            {
                ApplyPropertyAttributes(prop, type);
            }

            if (string.IsNullOrWhiteSpace(type.TypeName))
            {
                type.TypeName = "object";
            }

            return type;
        }

        private static void ApplyPropertyAttributes(dynamic prop, SimpleNormalizationApiDataType type)
        {
            if (prop.ContainsKey("defaultValue"))
            {
                type.DefaultValue = (string)prop.defaultValue;
            }
            if (prop.ContainsKey("minimum"))
            {
                type.MinimumValue = prop.minimum.ToString();
            }
            if (prop.ContainsKey("maximum"))
            {
                type.MaximumValue = prop.maximum.ToString();
            }
            if (prop.ContainsKey("nullable"))
            {
                type.IsNullable = (bool)prop.nullable;
            }
        }

        public void TryParseTypeFromJObject(object obj)
        {
            dynamic prop = obj;

            prop = prop.ContainsKey("schema") ? prop.schema : prop;

            var propType = (string) prop.type;
            if (!String.IsNullOrWhiteSpace(propType)) { propType = propType.ToLowerInvariant(); }

            switch (propType)
            {
                case "array":
                    ParseArrayFromJObject(obj);
                    break;
                case "boolean":
                    IsPrimitive = true;
                    TypeName = propType;
                    break;
                case "integer":
                    ParseIntegerFromJObject(obj);
                    break;
                case "number":
                    ParseNumberFromJObject(obj);
                    break;
                case "string":
                    ParseStringFromJObject(obj);
                    break;
                case "void":
                    TypeName = propType;
                    break;
            }

            if (prop.ContainsKey("$ref"))
            {
                TypeName = SplitType((string)prop["$ref"]);
                IsModelType = true;
            }
            else if (string.IsNullOrEmpty(TypeName) && !string.IsNullOrWhiteSpace(propType))
            {
                // Assume that the property is a model type
                TypeName = SplitType((string) prop.type);
                IsModelType = true;
            }           
        }

        //for Swagger 2.0 we need to split the $ref
        private string SplitType(string type)
        {
            if (type[0] == '#')
            {
                var splitType = type.Split('/');
                return splitType[splitType.Length - 1];
            }
            return type;
        }

        public void TryParseEnumFromJObject(object obj)
        {
            dynamic prop = obj;
            if (!prop.ContainsKey("enum")) return;

            var count = prop["enum"].Count;
            var values = new string[count];
            for (var i = 0; i < count; i++)
            {
                values[i] = (string) prop["enum"][i];
            }

            IsEnum = true;
            EnumValues = values;
        }

        public void ParseIntegerFromJObject(dynamic prop)
        {
            const string int32Format = "int32";
            const string int64Format = "int64";

            var format = prop.ContainsKey("format") ? (string) prop.format : int64Format;

            format = int32Format.Equals(format, StringComparison.InvariantCultureIgnoreCase)
                ? "int"
                : "long";

            TypeName = format;
            IsPrimitive = true;
        }

        public void ParseNumberFromJObject(dynamic prop)
        {
            const string floatFormat = "float";
            const string doubleFormat = "double";

            var format = prop.ContainsKey("format") ? (string) prop.format : doubleFormat;

            format = floatFormat.Equals(format, StringComparison.InvariantCultureIgnoreCase)
                ? "float"
                : "double";

            TypeName = format;
            IsPrimitive = true;
        }

        public void ParseStringFromJObject(dynamic prop)
        {
            var propType = (string) prop.type;
            var normalPropType = propType;
            if (!string.IsNullOrWhiteSpace(normalPropType)) { normalPropType = normalPropType.ToLowerInvariant(); }

            var isPrimitive = false;

            if (!"string".Equals(normalPropType) && !prop.ContainsKey("format"))
            {
                TypeName = propType;
            }
            if (!prop.ContainsKey("format"))
            {
                TypeName = "string";
            }
            else
            {
                var format = (string) prop.format;
                if (!string.IsNullOrWhiteSpace(format)) { format = format.ToLowerInvariant(); }

                switch (format)
                {
                    case "date":
                    case "date-time":
                        format = Constants.DataType_DateTime;
                        isPrimitive = true;
                        break;
                    case "date-span":
                        format = Constants.DataType_TimeSpan;
                        isPrimitive = true;
                        break;
                    case "byte":
                        isPrimitive = true;
                        break;
                    default:
                        // Otherwise just use the format
                        break;
                }
                TypeName = format;
            }

            IsPrimitive = isPrimitive;
        }

        public void ParseArrayFromJObject(dynamic prop)
        {
            IsEnumerable = true;
            IsEnumerableUnique = prop.ContainsKey("uniqueItems") && ((bool) prop.uniqueItems);

            //for Swagger 2.0; may need to break out to new class
            if (prop.ContainsKey("schema"))
                prop = prop.schema;

            if (!prop.ContainsKey("items")) return;

            object items = prop.items;
            var type = ParseFromJObject(items);

            TypeName = type.TypeName;
            IsPrimitive = type.IsPrimitive;
            IsNullable = type.IsNullable;
        }

        #endregion
    }
}
