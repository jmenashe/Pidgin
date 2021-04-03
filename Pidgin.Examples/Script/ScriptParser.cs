using System;
using System.Collections.Generic;
using System.Text;
using Pidgin;
using Pidgin.Examples.Expression;
using static Pidgin.Parser;
using static Pidgin.Parser<char>;

namespace Pidgin.Examples.Script
{
    using IExpr = Pidgin.Examples.Expression.IExpr;
    using ExprParser = Pidgin.Examples.Expression.ExprParser;
    public interface IStatement { }
    public interface IScript { }
    public class ScriptParser : TokenParser
    {
        public static readonly Parser<char, VarScope> StorageParser = Tok(Lowercase.ManyString())
            .Map(scopeId => (VarScope)Enum.Parse(typeof(VarScope), scopeId, true))
        ;
        public static readonly Parser<char, BinaryOperatorType> BinOpParser = Tok(Symbol)
            .Map(symbol =>
            {
                switch (symbol)
                {
                    case '=':
                        return BinaryOperatorType.Assign;
                    case '+':
                        return BinaryOperatorType.Add;
                    case '*':
                        return BinaryOperatorType.Mul;
                }
                throw new Exception($"Expected: BinaryOperatorType; Got: {symbol}");
            })
        ;
        public static readonly Parser<char, BinaryOperatorType> AssignOperator = BinOpParser.Where(x => x == BinaryOperatorType.Assign);
        public static Parser<char, Decl> DeclParser => StorageParser
            .Bind(scope => ExprParser.Identifier.Select(ident => new Decl(scope, ident)))
        ;
        public static Parser<char, DeclAssign> DeclAssignParser => DeclParser
            .Bind(scopedId => AssignOperator.Select(binOp => scopedId))
            .Bind(scopedId => ExprParser.Expr.Select(expr => new DeclAssign(scopedId.Scope, scopedId.Identifier, expr)))
        ;

        // TODO: Finish in implementation details.
        public static Parser<char, IStatement> StatementParser => DeclParser.Cast<IStatement>().Or(DeclAssignParser.Cast<IStatement>());
        public static Parser<char, Block> BlockParser => StatementParser.Many().Select(x => new Block(x));
        public static Parser<char, Module> ModuleParser => BlockParser.Many().Select(x => new Module(x));
    }
}
