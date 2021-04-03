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

        public static readonly Parser<char, string> StatementEnd = Tok(";");
        public static readonly Parser<char, string> OpenParen = Tok("(");
        public static readonly Parser<char, string> CloseParen = Tok(")");
        public static readonly Parser<char, string> OpenBracket = Tok("[");
        public static readonly Parser<char, string> CloseBracket = Tok("]");
        public static readonly Parser<char, string> OpenBrace = Tok("{");
        public static readonly Parser<char, string> CloseBrace = Tok("}");

        public static Parser<char, T> AddParens<T>(Parser<char, T> parser)
            => parser.Between(OpenParen, CloseParen);
        public static Parser<char, T> AddBrackets<T>(Parser<char, T> parser)
            => parser.Between(OpenBracket, CloseBracket);
        public static Parser<char, T> AddBraces<T>(Parser<char, T> parser)
            => parser.Between(OpenBrace, CloseBrace);
    }

    public static class ParserExtensions
    {
        public static Parser<char, T> Parens<T>(this Parser<char, T> parser)
            => TokenParser.AddParens(parser);
        public static Parser<char, T> Brackets<T>(this Parser<char, T> parser)
            => TokenParser.AddBrackets(parser);
        public static Parser<char, T> Braces<T>(this Parser<char, T> parser)
            => TokenParser.AddBraces(parser);
    }
}
