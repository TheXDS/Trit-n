using System;
using System.Linq;
using TheXDS.MCART.Resources;
using TheXDS.MCART.Types;
using TheXDS.Triton.ViewModels;

namespace TermClient
{
    class Program
    {
        static void Main(string[] args)
        {
            
        }
    }

    class Screen : HostViewModel
    {
        private readonly ConsoleColor _bgColor = Console.BackgroundColor;
        private readonly ConsoleColor _fgColor = Console.ForegroundColor;



        public Screen()
        {
        }

        private void DrawBg()
        {
            Console.BackgroundColor = _bgColor;
            Console.ForegroundColor = _fgColor;
            Console.CursorLeft = 0;
            Console.CursorTop = 0;
            Console.Write("Tritón demo".PadRight(Console.BufferWidth));
        }
        private void DrawTabs()
        {
            var t = 1;
            Console.CursorTop = 1;
            Console.CursorLeft = 0;
            foreach (var j in Pages.Take(Console.BufferWidth / 4))
            {
                Console.BackgroundColor = j.AccentColor.HasValue ? (ConsoleColor)(Color.To<byte, VGAAttributeByte>(j.AccentColor.Value) & 15) : _bgColor;
                Console.Write(t++.ToString().PadLeft(4));
            }
        }

        private void DrawActiveTab()
        {
            var j = Pages.FirstOrDefault();
            if (j is null) return;
            Console.BackgroundColor = j.AccentColor.HasValue ? (ConsoleColor)(Color.To<byte, VGAAttributeByte>(j.AccentColor.Value) & 15) : _bgColor;
            Console.CursorTop = 2;
            Console.CursorLeft = 0;
            Console.Write(j.Title.PadRight(Console.BufferWidth));

        }
    }
}
