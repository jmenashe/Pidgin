using System;
using System.IO;
using System.Linq;

namespace Pidgin.CodeGen
{
    static class MapGenerator
    {
        public static void Generate()
        {
            var filePath = "Pidgin/Parser.Map.Generated.cs";
            var dirPath = Path.GetDirectoryName(filePath);
            Directory.CreateDirectory(dirPath);
            File.WriteAllText(filePath, GenerateFile());
        }

        private static string GenerateFile()
        {
            var methods = Enumerable.Range(1, 8).Select(n => GenerateMethod(n));
            var classes = Enumerable.Range(1, 8).Select(n => GenerateClass(n));

            return $@"#region GeneratedCode
using System;

namespace Pidgin
{{
    // Generated by Pidgin.CodeGen.
    // Each of these methods is equivalent to
    //     return
    //         from x1 in p1
    //         from x2 in p2
    //         ...
    //         from xn in pn
    //         select func(x1, x2, ..., xn)
    // but this lower-level approach saves on allocations
    public static partial class Parser
    {{{string.Join(Environment.NewLine, methods)}
    }}
    internal abstract class MapParserBase<TToken, T> : Parser<TToken, T>
    {{
        internal new abstract MapParserBase<TToken, U> Map<U>(Func<T, U> func);
    }}
    {string.Join(Environment.NewLine, classes)}
}}
#endregion
";
        }


        private static string GenerateMethod(int num)
        {
            var nums = Enumerable.Range(1, num);
            var parserParams = nums.Select(n => $"Parser<TToken, T{n}> parser{n}");
            var parserFields = nums.Select(n => $"private readonly Parser<TToken, T{n}> _p{n};");
            var parserParamNames = nums.Select(n => $"parser{n}");
            var types = string.Join(", ", nums.Select(n => "T" + n));
            var checkArgsForNull = string.Concat(parserParamNames.Select(x => $@"
            if ({x} == null)
            {{
                throw new ArgumentNullException(nameof({x}));
            }}"));
            var mapReturnExpr = num == 1
                ? $@"parser1 is MapParserBase<TToken, T1> p
                ? p.Map(func)
                : new Map{num}Parser<TToken, {types}, R>(func, {string.Join(", ", parserParamNames)})"
                : $"new Map{num}Parser<TToken, {types}, R>(func, {string.Join(", ", parserParamNames)})";

            var typeParamDocs = nums.Select(n => $"<typeparam name=\"T{n}\">The return type of the {EnglishNumber(n)} parser</typeparam>");
            var paramDocs = nums.Select(n => $"<param name=\"parser{n}\">The {EnglishNumber(n)} parser</param>");


            return $@"
        /// <summary>
        /// Creates a parser that applies the specified parsers sequentially and applies the specified transformation function to their results.
        /// </summary>
        /// <param name=""func"">A function to apply to the return values of the specified parsers</param>
        /// {string.Join($"{Environment.NewLine}        /// ", paramDocs)}
        /// <typeparam name=""TToken"">The type of tokens in the parser's input stream</typeparam>
        /// {string.Join($"{Environment.NewLine}        ///", typeParamDocs)}
        /// <typeparam name=""R"">The return type of the resulting parser</typeparam>
        public static Parser<TToken, R> Map<TToken, {types}, R>(
            Func<{types}, R> func,
            {string.Join($",{Environment.NewLine}            ", parserParams)}
        )
        {{
            if (func == null)
            {{
                throw new ArgumentNullException(nameof(func));
            }}{checkArgsForNull}

            return {mapReturnExpr};
        }}";
        }


        private static string GenerateClass(int num)
        {
            var nums = Enumerable.Range(1, num);
            var parserParams = nums.Select(n => $"Parser<TToken, T{n}> parser{n}");
            var parserFields = nums.Select(n => $"private readonly Parser<TToken, T{n}> _p{n};");
            var parserParamNames = nums.Select(n => $"parser{n}");
            var parserFieldNames = nums.Select(n => $"_p{n}");
            var parserFieldAssignments = nums.Select(n => $"_p{n} = parser{n};");
            var results = nums.Select(n => $"result{n}");
            var types = string.Join(", ", nums.Select(n => "T" + n));
            var parts = nums.Select(GenerateMethodBodyPart);
            var funcArgNames = nums.Select(n => "x" + n);

            return $@"
    internal sealed class Map{num}Parser<TToken, {types}, R> : MapParserBase<TToken, R>
    {{
        private readonly Func<{types}, R> _func;
        {string.Join($"{Environment.NewLine}        ", parserFields)}

        public Map{num}Parser(
            Func<{types}, R> func,
            {string.Join($",{Environment.NewLine}            ", parserParams)}
        )
        {{
            _func = func;
            {string.Join($"{Environment.NewLine}            ", parserFieldAssignments)}
        }}

        internal sealed override bool TryParse(ref ParseState<TToken> state, ref ExpectedCollector<TToken> expecteds, out R result)
        {{
            {string.Join(Environment.NewLine, parts)}

            result = _func(
                {string.Join($",{Environment.NewLine}                ", results)}
            );
            return true;
        }}

        internal override MapParserBase<TToken, U> Map<U>(Func<R, U> func)
            => new Map{num}Parser<TToken, {types}, U>(
                ({string.Join(", ", funcArgNames)}) => func(_func({string.Join(", ", funcArgNames)})),
                {string.Join($",{Environment.NewLine}                ", parserFieldNames)}
            );
    }}";
        }

        private static string GenerateMethodBodyPart(int num)
            => $@"
            var success{num} = _p{num}.TryParse(ref state, ref expecteds, out var result{num});
            if (!success{num})
            {{
                result = default;
                return false;
            }}";
        
        private static string EnglishNumber(int num)
        {
            switch (num)
            {
                case 1: return "first";
                case 2: return "second";
                case 3: return "third";
                case 4: return "fourth";
                case 5: return "fifth";
                case 6: return "sixth";
                case 7: return "seventh";
                case 8: return "eighth";
            }
            throw new ArgumentOutOfRangeException(nameof(num));
        }
    }
}
