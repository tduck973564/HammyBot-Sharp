// HammyBot Sharp - HammyBot Sharp
//     Copyright (C) 2021 Thomas Duckworth <tduck973564@gmail.com>
// 
//     This program is free software: you can redistribute it and/or modify
//     it under the terms of the GNU General Public License as published by
//     the Free Software Foundation, either version 3 of the License, or
//     (at your option) any later version.
// 
//     This program is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU General Public License for more details.
// 
//     You should have received a copy of the GNU General Public License
//     along with this program.  If not, see <https://www.gnu.org/licenses/>.

using Jint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HammyBot_Sharp
{
    class InteractiveConsole
    {
        public static void Start()
        {
            var engine = new Engine(cfg => cfg.AllowClr(typeof(Console).Assembly)).SetValue("bot", typeof(ConsoleFunctions));

            for (; ;)
            {
                Console.Write("js# \x1b[31m>\x1b[0m ");

                var input = Console.ReadLine();

                if (input == ".startblock")
                {
                    input = "";

                    for (; ; )
                    {
                        Console.Write("block \x1b[36m>\x1b[0m ");
                        var toAdd = Console.ReadLine();

                        if (toAdd == ".endblock")
                            break;

                        input += toAdd;
                    }
                }

                try 
                {
                    var completionValue = engine.Evaluate(input);
                    Console.WriteLine($" \x1b[32m=>\x1b[0m \x1b[36m{completionValue}\x1b[0m");
                } 
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
                Console.Write("\n");
            }
        }
    }

    static class ConsoleFunctions
    {
        public static void Lorber()
        {
            Console.WriteLine("harf");
        }
    }
}
