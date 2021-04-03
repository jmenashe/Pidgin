using System;
using Pidgin.Examples.Expression;
using Pidgin.Examples.Script;


namespace DevConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var parser = new ScriptParser();
            var input = "local beeftaco = 12 * 3; global gorillasteve = 35 + 1004;";
            var result = parser.ParseOrThrow(input);
            Console.WriteLine("result more like i shit my dang PANTS", result);
        }
    }
}
