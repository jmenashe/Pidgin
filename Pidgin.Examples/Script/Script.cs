using System;
using System.Collections.Generic;
using System.Text;
using Pidgin.Examples.Expression;
using System.ComponentModel;

namespace Pidgin.Examples.Script
{
    public enum VarScope { Local, Class, Global };
    public class Decl : IStatement
    {
        public VarScope Scope { get; set; }
        public Identifier Identifier { get; set; }
        public Decl(VarScope scope, Identifier identifier)
        {
            this.Scope = scope;
            this.Identifier = identifier;
        }
    }
    public class DeclAssign : Decl
    {
        public IExpr Value { get; set; }
        public DeclAssign(VarScope scope, Identifier identifier, IExpr value)
            : base(scope, identifier)
        {
            this.Value = value;
        }
        public DeclAssign(Decl decl, IExpr value)
            : this(decl.Scope, decl.Identifier, value)
        { }
    }

    // TODO: Finish in implementation details.
    public class Block : IScript
    {
        public Block(IEnumerable<IStatement> statements) { }
    }
    public class Sub : IScript
    {
        public Sub(IEnumerable<IStatement> statements) { }
    }
    public class Module : IScript
    {
        public Module(IEnumerable<Block> blocks) { }
    }
}
