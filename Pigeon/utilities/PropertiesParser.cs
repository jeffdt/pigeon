using System;

namespace pigeon.utilities {
    public abstract class PropertiesParser {
        public static Tuple<string, string> ParseObjectIntoType(string propertyObject) {
            string[] strings = propertyObject.Split(new[] { ':' });

            if (strings.Length != 2) {
                throw new FormatException(string.Format("could not parse property object '{0}' into type and values. expecting format 'type:value1/valu2/value3'", propertyObject));
            }

            return new Tuple<string, string>(strings[0], strings[1]);
        }

        public static string[] ParseObjectValues(string propertyObject, int expectedParameterCount) {
            string[] parameters = propertyObject.Split(new[] { '/' });

            if (parameters.Length != expectedParameterCount) {
                throw new FormatException(string.Format("expected {0} parameters for type, but found {1}", expectedParameterCount, parameters.Length));
            }

            return parameters;
        }
    }
}