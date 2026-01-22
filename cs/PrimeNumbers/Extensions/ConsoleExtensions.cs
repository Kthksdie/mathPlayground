using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PrimeNumbers.Extensions {
    public static class ConsoleExtensions {
        private const int STD_OUTPUT_HANDLE = -11;
        private const uint ENABLE_VIRTUAL_TERMINAL_PROCESSING = 0x0004;
        private const uint DISABLE_NEWLINE_AUTO_RETURN = 0x0008;

        private static bool _enabled = false;

        /// <summary>
        /// Automatically determines whether or not this is running as a Console Application in ConsoleExtensions()
        /// _enabled = true if running as Console
        /// _enabled = false if running as something else, like a Windows Service in a Relay Agent
        /// </summary>
        static ConsoleExtensions() {
            try {
                var iStdOut = GetStdHandle(STD_OUTPUT_HANDLE);
                uint outConsoleMode = 0;

                if (GetConsoleMode(iStdOut, out outConsoleMode)) {
                    outConsoleMode |= ENABLE_VIRTUAL_TERMINAL_PROCESSING | DISABLE_NEWLINE_AUTO_RETURN;
                    if (SetConsoleMode(iStdOut, outConsoleMode)) {
                        _enabled = true;
                    }
                }
            }
            catch { }
        }

        [DllImport("kernel32.dll")]
        private static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);

        [DllImport("kernel32.dll")]
        private static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetStdHandle(int nStdHandle);

        [DllImport("kernel32.dll")]
        public static extern uint GetLastError();

        /// <summary>
        /// Returns a string wrapped in an ANSI foreground color code using the specified color.
        /// </summary>
        /// <param name="input">The string to color.</param>
        /// <param name="color">The color to use on the specified string.</param>
        public static string Pastel(this string input, Color color) {
            if (_enabled) {
                return $"\u001b[38;2;{color.R};{color.G};{color.B}m{input}\u001b[0m";
            }
            else {
                return input;
            }
        }

        public static string Pastel(this object input, Color color) {
            return Pastel($"{input}", color);
        }

        /// <summary>
        /// Returns a string wrapped in an ANSI foreground color code using the specified color.
        /// </summary>
        /// <param name="input">The string to color.</param>
        /// <param name="hexColor">The color to use on the specified string.<para>Supported format: [#]RRGGBB.</para></param>
        public static string Pastel(this string input, string hexColor) {
            if (_enabled) {
                var color = Color.FromArgb(int.Parse(hexColor.Replace("#", ""), NumberStyles.HexNumber));

                return Pastel(input, color);
            }
            else {
                return input;
            }
        }

        public static string Pastel(this object input, string hexColor) {
            return Pastel($"{input}", hexColor);
        }

        /// <summary>
        /// Returns a string wrapped in an ANSI background color code using the specified color.
        /// </summary>
        /// <param name="input">The string to color.</param>
        /// <param name="color">The color to use on the specified string.</param>
        public static string PastelBg(this string input, Color color) {
            if (_enabled) {
                return $"\u001b[48;2;{color.R};{color.G};{color.B}m{input}\u001b[0m";
            }
            else {
                return input;
            }
        }

        public static string PastelBg(this object input, Color color) {
            return PastelBg($"{input}", color);
        }

        /// <summary>
        /// Returns a string wrapped in an ANSI background color code using the specified color.
        /// </summary>
        /// <param name="input">The string to color.</param>
        /// <param name="hexColor">The color to use on the specified string.<para>Supported format: [#]RRGGBB.</para></param>
        public static string PastelBg(this string input, string hexColor) {
            if (_enabled) {
                var color = Color.FromArgb(int.Parse(hexColor.Replace("#", ""), NumberStyles.HexNumber));

                return PastelBg(input, color);
            }
            else {
                return input;
            }
        }

        public static string PastelBg(this object input, string hexColor) {
            return PastelBg($"{input}", hexColor);
        }
    }

    public static class Colors {
        public static List<Color> All() {
            return typeof(Color).GetProperties(BindingFlags.Static | BindingFlags.DeclaredOnly | BindingFlags.Public)
                .Select(c => (Color)c.GetValue(null, null))
                .ToList();
        }
    }
}
