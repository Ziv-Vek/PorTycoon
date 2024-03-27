using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using JetBrains.Annotations;

namespace SupersonicWisdomSDK
{
    public static class SwJsonParser
    {
        #region --- Public Methods ---

        public static object Deserialize(string json)
        {
            if (json == null)
            {
                return null;
            }

            return Parser.Parse(json);
        }

        [NotNull]
        public static Dictionary<string, object> DeserializeToDictionary(string json)
        {
            return Deserialize(json) as Dictionary<string, object> ?? new Dictionary<string, object>();
        }

        public static string Serialize(object obj)
        {
            return Serializer.Serialize(obj);
        }

        #endregion


        #region --- Inner Classes ---

        private sealed class Parser : IDisposable
        {
            #region --- Constants ---

            private const string WhiteSpace = " \t\n\r";
            private const string WordBreak = " \t\n\r{}[],:\"";

            #endregion


            #region --- Members ---

            private StringReader _json;

            #endregion


            #region --- Properties ---

            private char NextChar
            {
                get { return Convert.ToChar(_json.Read()); }
            }

            private char PeekChar
            {
                get { return Convert.ToChar(_json.Peek()); }
            }

            private string NextWord
            {
                get
                {
                    var word = new StringBuilder();
                    char ch;

                    try
                    {
                        ch = PeekChar;
                    }
                    catch (Exception)
                    {
                        return word.ToString();
                    }

                    while (WordBreak.IndexOf(ch) == -1)
                    {
                        word.Append(NextChar);

                        if (_json.Peek() == -1)
                        {
                            break;
                        }

                        try
                        {
                            ch = PeekChar;
                        }
                        catch (Exception)
                        {
                            return word.ToString();
                        }
                    }

                    return word.ToString();
                }
            }

            private Token NextToken
            {
                get
                {
                    EatWhitespace();

                    if (_json.Peek() == -1)
                    {
                        return Token.None;
                    }

                    var c = PeekChar;

                    switch (c)
                    {
                        case '{':
                            return Token.CurlyOpen;
                        case '}':
                            _json.Read();

                            return Token.CurlyClose;
                        case '[':
                            return Token.SquaredOpen;
                        case ']':
                            _json.Read();

                            return Token.SquaredClose;
                        case ',':
                            _json.Read();

                            return Token.Comma;
                        case '"':
                            return Token.String;
                        case ':':
                            return Token.Colon;
                        case '0':
                        case '1':
                        case '2':
                        case '3':
                        case '4':
                        case '5':
                        case '6':
                        case '7':
                        case '8':
                        case '9':
                        case '-':
                            return Token.Number;
                    }

                    var word = NextWord;

                    switch (word)
                    {
                        case "false":
                            return Token.False;
                        case "true":
                            return Token.True;
                        case "null":
                            return Token.Null;
                    }

                    return Token.None;
                }
            }

            #endregion


            #region --- Construction ---

            private Parser(string jsonString)
            {
                _json = new StringReader(jsonString);
            }

            #endregion


            #region --- Public Methods ---

            public static object Parse(string jsonString)
            {
                using (var instance = new Parser(jsonString))
                {
                    return instance.ParseValue();
                }
            }

            public void Dispose ()
            {
                _json.Dispose();
                _json = null;
            }

            #endregion


            #region --- Private Methods ---

            private void EatWhitespace ()
            {
                char ch;

                try
                {
                    ch = PeekChar;
                }
                catch (Exception)
                {
                    return;
                }

                while (WhiteSpace.IndexOf(ch) != -1)
                {
                    _json.Read();

                    if (_json.Peek() == -1)
                    {
                        break;
                    }

                    try
                    {
                        ch = PeekChar;
                    }
                    catch (Exception)
                    {
                        break;
                    }
                }
            }

            private List<object> ParseArray ()
            {
                var array = new List<object>();

                // ditch opening bracket
                _json.Read();

                // [
                var parsing = true;

                while (parsing)
                {
                    var nextToken = NextToken;

                    switch (nextToken)
                    {
                        case Token.None:
                            return null;
                        case Token.Comma:
                            continue;
                        case Token.SquaredClose:
                            parsing = false;

                            break;
                        default:
                            var value = ParseByToken(nextToken);

                            array.Add(value);

                            break;
                    }
                }

                return array;
            }

            private object ParseByToken(Token token)
            {
                switch (token)
                {
                    case Token.String:
                        return ParseString();
                    case Token.Number:
                        return ParseNumber();
                    case Token.CurlyOpen:
                        return ParseObject();
                    case Token.SquaredOpen:
                        return ParseArray();
                    case Token.True:
                        return true;
                    case Token.False:
                        return false;
                    case Token.Null:
                        return null;
                    default:
                        return null;
                }
            }

            private object ParseNumber ()
            {
                var number = NextWord;

                if (number.IndexOf('.') == -1)
                {
                    long parsedInt;
                    long.TryParse(number, out parsedInt);

                    return parsedInt;
                }

                double parsedDouble;
                double.TryParse(number, out parsedDouble);

                return parsedDouble;
            }

            private Dictionary<string, object> ParseObject ()
            {
                var table = new Dictionary<string, object>();

                // ditch opening brace
                _json.Read();

                // {
                while (true)
                {
                    switch (NextToken)
                    {
                        case Token.None:
                            return null;
                        case Token.Comma:
                            continue;
                        case Token.CurlyClose:
                            return table;
                        default:
                            // name
                            var name = ParseString();

                            if (name == null)
                            {
                                return null;
                            }

                            // :
                            if (NextToken != Token.Colon)
                            {
                                return null;
                            }

                            // ditch the colon
                            _json.Read();

                            // value
                            table[name] = ParseValue();

                            break;
                    }
                }
            }

            private string ParseString ()
            {
                var s = new StringBuilder();
                char c;

                // ditch opening quote
                _json.Read();

                var parsing = true;

                while (parsing)
                {
                    if (_json.Peek() == -1)
                    {
                        parsing = false;

                        break;
                    }

                    c = NextChar;

                    switch (c)
                    {
                        case '"':
                            parsing = false;

                            break;
                        case '\\':
                            if (_json.Peek() == -1)
                            {
                                parsing = false;

                                break;
                            }

                            c = NextChar;

                            switch (c)
                            {
                                case '"':
                                case '\\':
                                case '/':
                                    s.Append(c);

                                    break;
                                case 'b':
                                    s.Append('\b');

                                    break;
                                case 'f':
                                    s.Append('\f');

                                    break;
                                case 'n':
                                    s.Append('\n');

                                    break;
                                case 'r':
                                    s.Append('\r');

                                    break;
                                case 't':
                                    s.Append('\t');

                                    break;
                                case 'u':
                                    var hex = new StringBuilder();

                                    for (var i = 0; i < 4; i++)
                                    {
                                        hex.Append(NextChar);
                                    }

                                    s.Append((char)Convert.ToInt32(hex.ToString(), 16));

                                    break;
                            }

                            break;
                        default:
                            s.Append(c);

                            break;
                    }
                }

                return s.ToString();
            }

            private object ParseValue ()
            {
                var nextToken = NextToken;

                return ParseByToken(nextToken);
            }

            #endregion


            #region --- Enums ---

            private enum Token
            {
                None,
                CurlyOpen,
                CurlyClose,
                SquaredOpen,
                SquaredClose,
                Colon,
                Comma,
                String,
                Number,
                True,
                False,
                Null
            }

            #endregion
        }

        private sealed class Serializer
        {
            #region --- Members ---

            private readonly StringBuilder _builder;

            #endregion


            #region --- Construction ---

            private Serializer ()
            {
                _builder = new StringBuilder();
            }

            #endregion


            #region --- Public Methods ---

            public static string Serialize(object obj)
            {
                var instance = new Serializer();

                instance.SerializeValue(obj);

                return instance._builder.ToString();
            }

            #endregion


            #region --- Private Methods ---

            private void SerializeArray(IList anArray)
            {
                _builder.Append('[');

                var first = true;

                foreach (var obj in anArray)
                {
                    if (!first)
                    {
                        _builder.Append(',');
                    }

                    SerializeValue(obj);

                    first = false;
                }

                _builder.Append(']');
            }

            private void SerializeObject(IDictionary obj)
            {
                var first = true;

                _builder.Append('{');

                foreach (var e in obj.Keys)
                {
                    if (!first)
                    {
                        _builder.Append(',');
                    }

                    SerializeString(e.ToString());
                    _builder.Append(':');

                    SerializeValue(obj[e]);

                    first = false;
                }

                _builder.Append('}');
            }

            private void SerializeOther(object value)
            {
                if (value is float || value is int || value is uint || value is long || value is double || value is sbyte || value is byte || value is short || value is ushort || value is ulong || value is decimal)
                {
                    _builder.Append(value);
                }
                else
                {
                    SerializeString(value.ToString());
                }
            }

            private void SerializeString(string str)
            {
                _builder.Append('\"');

                var charArray = str.ToCharArray();

                foreach (var c in charArray)
                {
                    switch (c)
                    {
                        case '"':
                            _builder.Append("\\\"");

                            break;
                        case '\\':
                            _builder.Append("\\\\");

                            break;
                        case '\b':
                            _builder.Append("\\b");

                            break;
                        case '\f':
                            _builder.Append("\\f");

                            break;
                        case '\n':
                            _builder.Append("\\n");

                            break;
                        case '\r':
                            _builder.Append("\\r");

                            break;
                        case '\t':
                            _builder.Append("\\t");

                            break;
                        default:
                            var codepoint = Convert.ToInt32(c);

                            if (codepoint >= 32 && codepoint <= 126)
                            {
                                _builder.Append(c);
                            }
                            else
                            {
                                _builder.Append("\\u" + Convert.ToString(codepoint, 16).PadLeft(4, '0'));
                            }

                            break;
                    }
                }

                _builder.Append('\"');
            }

            private void SerializeValue(object value)
            {
                IList asList;
                IDictionary asDict;
                string asStr;

                if (value == null)
                {
                    _builder.Append("null");
                }
                else if ((asStr = value as string) != null)
                {
                    SerializeString(asStr);
                }
                else if (value is bool)
                {
                    _builder.Append(value.ToString().ToLower());
                }
                else if ((asList = value as IList) != null)
                {
                    SerializeArray(asList);
                }
                else if ((asDict = value as IDictionary) != null)
                {
                    SerializeObject(asDict);
                }
                else if (value is char)
                {
                    SerializeString(value.ToString());
                }
                else
                {
                    SerializeOther(value);
                }
            }

            #endregion
        }

        #endregion
    }
}