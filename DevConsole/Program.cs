using System;
using Pidgin.Examples.Expression;


namespace DevConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var input = "12 * 3 + foo(-3, x)() * (2 + 1)";
            var result = ExprParser.ParseOrThrow(input);
            Console.WriteLine("result more like i shit my dang PANTS", result);
        }
    }
}
