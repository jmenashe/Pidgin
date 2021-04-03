using System;
using System.Collections.Immutable;
using Pidgin;
using Pidgin.Expression;
using static Pidgin.Parser;
using static Pidgin.Parser<char>;

namespace Pidgin.Examples.Expression
{
    public interface IParser<T> where T : class, IEquatable<T>
    {
        public Result<char, T> Parse(string input);
        public T ParseOrThrow(string input);
    }
    public interface IExprParser : IParser<IExpr> { }

    public class ExprParser : TokenParser, IExprParser
    {


        public static Parser<char, T> Parenthesised<T>(Parser<char, T> parser)
            => parser.Between(Tok("("), Tok(")"));

        public static Parser<char, Func<IExpr, IExpr, IExpr>> Binary(Parser<char, BinaryOperatorType> op)
            => op.Select<Func<IExpr, IExpr, IExpr>>(type => (l, r) => new BinaryOp(type, l, r));
        public static Parser<char, Func<IExpr, IExpr>> Unary(Parser<char, UnaryOperatorType> op)
            => op.Select<Func<IExpr, IExpr>>(type => o => new UnaryOp(type, o));

        public static readonly Parser<char, Func<IExpr, IExpr, IExpr>> Assign
            = Binary(Tok("=").ThenReturn(BinaryOperatorType.Assign));
        public static readonly Parser<char, Func<IExpr, IExpr, IExpr>> Add
            = Binary(Tok("+").ThenReturn(BinaryOperatorType.Add));
        public static readonly Parser<char, Func<IExpr, IExpr, IExpr>> Mul
            = Binary(Tok("*").ThenReturn(BinaryOperatorType.Mul));
        public static readonly Parser<char, Func<IExpr, IExpr>> Neg
            = Unary(Tok("-").ThenReturn(UnaryOperatorType.Neg));
        public static readonly Parser<char, Func<IExpr, IExpr>> Complement
            = Unary(Tok("~").ThenReturn(UnaryOperatorType.Complement));

        public static readonly Parser<char, Identifier> Identifier
            = Tok(Letter.Then(LetterOrDigit.ManyString(), (h, t) => h + t))
                .Select(name => new Identifier(name))
                .Labelled("identifier");
        public static readonly Parser<char, IExpr> Literal
            = Tok(Num)
                .Select<IExpr>(value => new Literal(value))
                .Labelled("integer literal");

        public static Parser<char, Func<IExpr, IExpr>> Call(Parser<char, IExpr> subExpr)
            => Parenthesised(subExpr.Separated(Tok(",")))
                .Select<Func<IExpr, IExpr>>(args => method => new Call(method, args.ToImmutableArray()))
                .Labelled("function call");

        public static readonly Parser<char, IExpr> Expr = ExpressionParser.Build<char, IExpr>(
            expr => (
                OneOf(
                    Identifier.Cast<IExpr>(),
                    Literal,
                    Parenthesised(expr).Labelled("parenthesised expression")
                ),
                new[]
                {
                    Operator.PostfixChainable(Call(expr)),
                    Operator.Prefix(Neg).And(Operator.Prefix(Complement)),
                    Operator.InfixL(Mul),
                    Operator.InfixL(Add)
                }
            )
        ).Labelled("expression");

        public Result<char, IExpr> Parse(string input)
            => Expr.Parse(input);
        public IExpr ParseOrThrow(string input)
            => Expr.ParseOrThrow(input);
    }
}