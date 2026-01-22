using Newtonsoft.Json;
using PrimeNumbers.Extensions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrimeNumbers.Utilities {
    public static class Logging {
        private static object _fileLock = new object();

        public static void Log(string message, bool rewrite = false) {
            var currentTime = DateTime.Now;
            var longTimeString = currentTime.ToLongTimeString().PadLeft(11);

            var loggingMessage = $"[{longTimeString.Pastel(Color.Orange)}] {message}";

            lock (_fileLock) {
                if (rewrite) {
                    var cursorTop = Console.CursorTop > 0 ? (Console.CursorTop - 1) : 0;
                    Console.SetCursorPosition(0, cursorTop);
                    Console.WriteLine(loggingMessage.PadRight(Console.WindowWidth, ' '));

                    //if (Console.CursorTop + 1 < 9000) {
                    //    Console.SetCursorPosition(0, Console.CursorTop + 1);
                    //}
                    //else {
                    //    Console.Clear();
                    //    Console.SetCursorPosition(0, 0);
                    //}
                }
                else {
                    Console.SetCursorPosition(0, Console.CursorTop);
                    Console.WriteLine(loggingMessage.PadRight(Console.WindowWidth, ' '));
                }
            }
        }

        public static void Log(string message, Color fontColor, bool rewrite = false) {
            Log(message.Pastel(fontColor), rewrite);
        }

        public static void ResetCursor() {
            if (Console.CursorTop == 9000) {
                Console.Clear();
                Console.SetCursorPosition(0, 0);
            }
            else {
                Console.SetCursorPosition(0, Console.CursorTop + 1);
            }
        }

        public static T GetJsonFromFile<T>(string filePath) where T : new() {
            TextReader reader = null;
            try {
                if (!File.Exists(filePath)) {
                    return default(T);
                }
                var jsonSerializerSettings = new JsonSerializerSettings {
                    NullValueHandling = NullValueHandling.Include,
                    DateParseHandling = DateParseHandling.None
                };
                reader = new StreamReader(filePath);
                var fileContents = reader.ReadToEnd();
                return JsonConvert.DeserializeObject<T>(fileContents, jsonSerializerSettings);
            }
            finally {
                if (reader != null) {
                    reader.Close();
                }
            }
        }

        public static void SaveJsonAsFile<T>(string filePath, T objectToWrite, bool append = false) where T : new() {
            lock (_fileLock) {
                TextWriter writer = null;
                try {
                    var jsonSerializerSettings = new JsonSerializerSettings {
                        Formatting = Formatting.None,
                        NullValueHandling = NullValueHandling.Include
                    };
                    var contentsToWriteToFile = JsonConvert.SerializeObject(objectToWrite, Formatting.Indented, jsonSerializerSettings);

                    writer = new StreamWriter(filePath, append);
                    writer.Write(contentsToWriteToFile);
                }
                finally {
                    if (writer != null) {
                        writer.Close();
                    }
                }
            }

        }
    }
}
