using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.Json;

namespace AcadeGameMaker.Profile
{
    // REQ-M5D5-001..006. Engine-free RFC 8785 inner binding JSON canonicalization only.
    public readonly struct ProfileBindingOverridesJson
    {
        private readonly string _canonicalText;

        private ProfileBindingOverridesJson(string canonicalText)
        {
            _canonicalText = canonicalText;
        }

        public static ProfileBindingOverridesJson Parse(string source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (source.Length == 0) return new ProfileBindingOverridesJson(string.Empty);
            try
            {
                return new ProfileBindingOverridesJson(ProfileBindingOverridesJsonCanonicalizer.CanonicalizeNonEmpty(source));
            }
            catch (JsonException exception)
            {
                throw new ArgumentException("Binding overrides JSON is invalid.", nameof(source), exception);
            }
            catch (FormatException exception)
            {
                throw new ArgumentException("Binding overrides JSON is invalid.", nameof(source), exception);
            }
        }

        public bool IsEmpty
        {
            get
            {
                Validate();
                return _canonicalText.Length == 0;
            }
        }

        public string CanonicalText
        {
            get
            {
                Validate();
                return _canonicalText;
            }
        }

        // REQ-M5D5-004. Reparse/recanonicalize every non-empty public representation.
        public void Validate()
        {
            if (_canonicalText == null) throw new InvalidOperationException("Binding overrides representation is absent.");
            if (_canonicalText.Length == 0) return;
            try
            {
                var recanonicalized = ProfileBindingOverridesJsonCanonicalizer.CanonicalizeNonEmpty(_canonicalText);
                if (!string.Equals(_canonicalText, recanonicalized, StringComparison.Ordinal))
                    throw new InvalidOperationException("Binding overrides representation is noncanonical.");
            }
            catch (JsonException exception)
            {
                throw new InvalidOperationException("Binding overrides representation is malformed.", exception);
            }
            catch (FormatException exception)
            {
                throw new InvalidOperationException("Binding overrides representation is malformed.", exception);
            }
        }
    }

    internal static class ProfileBindingOverridesJsonCanonicalizer
    {
        private static readonly JsonDocumentOptions StrictOptions = new JsonDocumentOptions
        {
            AllowTrailingCommas = false,
            CommentHandling = JsonCommentHandling.Disallow,
            MaxDepth = 64
        };

        internal static string CanonicalizeNonEmpty(string source)
        {
            if (string.IsNullOrEmpty(source)) throw new FormatException("Non-empty JSON is required.");
            ValidateRawStringSurrogates(source);
            using (var document = JsonDocument.Parse(source, StrictOptions))
            {
                var builder = new StringBuilder();
                WriteElement(document.RootElement, builder);
                return builder.ToString();
            }
        }

        private static void WriteElement(JsonElement element, StringBuilder builder)
        {
            switch (element.ValueKind)
            {
                case JsonValueKind.Object:
                    WriteObject(element, builder);
                    return;
                case JsonValueKind.Array:
                    builder.Append('[');
                    var first = true;
                    foreach (var item in element.EnumerateArray())
                    {
                        if (!first) builder.Append(',');
                        first = false;
                        WriteElement(item, builder);
                    }
                    builder.Append(']');
                    return;
                case JsonValueKind.String:
                    WriteString(element.GetString(), builder);
                    return;
                case JsonValueKind.Number:
                    builder.Append(FormatNumber(element.GetDouble()));
                    return;
                case JsonValueKind.True:
                    builder.Append("true");
                    return;
                case JsonValueKind.False:
                    builder.Append("false");
                    return;
                case JsonValueKind.Null:
                    builder.Append("null");
                    return;
                default:
                    throw new FormatException("JSON value kind is unsupported.");
            }
        }

        private static void WriteObject(JsonElement element, StringBuilder builder)
        {
            var names = new HashSet<string>(StringComparer.Ordinal);
            var properties = new List<KeyValuePair<string, JsonElement>>();
            foreach (var property in element.EnumerateObject())
            {
                var name = property.Name;
                ValidateDecodedText(name);
                if (!names.Add(name)) throw new FormatException("Decoded JSON property name is duplicated.");
                properties.Add(new KeyValuePair<string, JsonElement>(name, property.Value));
            }
            properties.Sort((left, right) => StringComparer.Ordinal.Compare(left.Key, right.Key));
            builder.Append('{');
            for (var index = 0; index < properties.Count; index++)
            {
                if (index != 0) builder.Append(',');
                WriteString(properties[index].Key, builder);
                builder.Append(':');
                WriteElement(properties[index].Value, builder);
            }
            builder.Append('}');
        }

        private static void WriteString(string value, StringBuilder builder)
        {
            ValidateDecodedText(value);
            builder.Append('"');
            for (var index = 0; index < value.Length; index++)
            {
                var current = value[index];
                switch (current)
                {
                    case '"': builder.Append("\\\""); break;
                    case '\\': builder.Append("\\\\"); break;
                    case '\b': builder.Append("\\b"); break;
                    case '\t': builder.Append("\\t"); break;
                    case '\n': builder.Append("\\n"); break;
                    case '\f': builder.Append("\\f"); break;
                    case '\r': builder.Append("\\r"); break;
                    default:
                        if (current <= 0x1f)
                        {
                            builder.Append("\\u00");
                            builder.Append("0123456789abcdef"[(current >> 4) & 0x0f]);
                            builder.Append("0123456789abcdef"[current & 0x0f]);
                        }
                        else builder.Append(current);
                        break;
                }
            }
            builder.Append('"');
        }

        private static string FormatNumber(double value)
        {
            if (double.IsNaN(value) || double.IsInfinity(value)) throw new FormatException("JSON number is not finite binary64.");
            if (value == 0d) return "0";
            var shortest = ShortestRoundTrip(value);
            var negative = shortest[0] == '-';
            var unsigned = negative ? shortest.Substring(1) : shortest;
            var exponentIndex = unsigned.IndexOf('E');
            if (exponentIndex < 0) exponentIndex = unsigned.IndexOf('e');
            var mantissa = exponentIndex < 0 ? unsigned : unsigned.Substring(0, exponentIndex);
            var exponent = exponentIndex < 0 ? 0 : int.Parse(unsigned.Substring(exponentIndex + 1), NumberStyles.Integer, CultureInfo.InvariantCulture);
            var dot = mantissa.IndexOf('.');
            var fractionCount = dot < 0 ? 0 : mantissa.Length - dot - 1;
            var digits = dot < 0 ? mantissa : mantissa.Remove(dot, 1);
            var shift = exponent - fractionCount;
            var absolute = Math.Abs(value);
            var body = absolute >= 1e-6d && absolute < 1e21d
                ? DecimalForm(digits, shift)
                : ScientificForm(digits, shift);
            return negative ? "-" + body : body;
        }

        private static string ShortestRoundTrip(double value)
        {
            for (var precision = 1; precision <= 17; precision++)
            {
                var candidate = value.ToString("G" + precision, CultureInfo.InvariantCulture);
                double reparsed;
                if (double.TryParse(candidate, NumberStyles.Float, CultureInfo.InvariantCulture, out reparsed)
                    && BitConverter.DoubleToInt64Bits(reparsed) == BitConverter.DoubleToInt64Bits(value))
                    return candidate;
            }
            throw new FormatException("Binary64 value has no shortest round-trip representation.");
        }

        private static string DecimalForm(string digits, int shift)
        {
            var point = digits.Length + shift;
            if (point >= digits.Length) return digits + new string('0', point - digits.Length);
            if (point > 0) return digits.Substring(0, point) + "." + digits.Substring(point);
            return "0." + new string('0', -point) + digits;
        }

        private static string ScientificForm(string digits, int shift)
        {
            var exponent = digits.Length + shift - 1;
            var coefficient = digits.Length == 1 ? digits : digits[0] + "." + digits.Substring(1);
            return coefficient + "e" + (exponent >= 0 ? "+" : string.Empty) + exponent.ToString(CultureInfo.InvariantCulture);
        }

        private static void ValidateDecodedText(string value)
        {
            if (!HasNoUnpairedSurrogates(value) || !string.Equals(value, value.Normalize(NormalizationForm.FormC), StringComparison.Ordinal))
                throw new FormatException("Decoded JSON text is not surrogate-safe NFC.");
        }

        private static bool HasNoUnpairedSurrogates(string value)
        {
            if (value == null) return false;
            for (var index = 0; index < value.Length; index++)
            {
                if (char.IsHighSurrogate(value[index]))
                {
                    if (index + 1 >= value.Length || !char.IsLowSurrogate(value[index + 1])) return false;
                    index++;
                }
                else if (char.IsLowSurrogate(value[index])) return false;
            }
            return true;
        }

        // Detect escaped and literal UTF-16 surrogate errors before JsonDocument may decode them.
        private static void ValidateRawStringSurrogates(string source)
        {
            var inString = false;
            for (var index = 0; index < source.Length; index++)
            {
                var current = source[index];
                if (!inString)
                {
                    if (current == '"') inString = true;
                    continue;
                }
                if (current == '"') { inString = false; continue; }
                if (current != '\\')
                {
                    if (char.IsHighSurrogate(current))
                    {
                        if (index + 1 >= source.Length || !char.IsLowSurrogate(source[index + 1])) throw new FormatException("Unpaired surrogate.");
                        index++;
                    }
                    else if (char.IsLowSurrogate(current)) throw new FormatException("Unpaired surrogate.");
                    continue;
                }
                if (++index >= source.Length) throw new FormatException("Invalid escape.");
                if (source[index] != 'u') continue;
                if (index + 4 >= source.Length) throw new FormatException("Invalid unicode escape.");
                var codeUnit = ParseHex(source, index + 1);
                index += 4;
                if (char.IsHighSurrogate((char)codeUnit))
                {
                    if (index + 6 >= source.Length || source[index + 1] != '\\' || source[index + 2] != 'u'
                        || !char.IsLowSurrogate((char)ParseHex(source, index + 3))) throw new FormatException("Unpaired surrogate escape.");
                    index += 6;
                }
                else if (char.IsLowSurrogate((char)codeUnit)) throw new FormatException("Unpaired surrogate escape.");
            }
        }

        private static int ParseHex(string value, int index)
        {
            var result = 0;
            for (var offset = 0; offset < 4; offset++)
            {
                var current = value[index + offset];
                var digit = current >= '0' && current <= '9' ? current - '0'
                    : current >= 'a' && current <= 'f' ? current - 'a' + 10
                    : current >= 'A' && current <= 'F' ? current - 'A' + 10 : -1;
                if (digit < 0) throw new FormatException("Invalid unicode escape.");
                result = (result << 4) | digit;
            }
            return result;
        }
    }
}
