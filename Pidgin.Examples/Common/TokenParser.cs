using System;
using System.Collections.Generic;
using System.Text;
using Pidgin;
using Pidgin.Expression;
using static Pidgin.Parser;
using static Pidgin.Parser<char>;

namespace Pidgin.Examples
{
    public class TokenParser
    {
        public static Parser<char, T> Tok<T>(Parser<char, T> token) 
            => Try(token).Before(SkipWhitespaces);
        public static Parser<char, string> Tok(string token)
            => Tok(String(token));
    }
}
