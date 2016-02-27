using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Assignment1
{
    class JsonImporter
    {
        static string startLineSymbol = "{";
        static string endLineSymbol = "}";
        static string startParseSymbol = "\"rows\" : [";
        static string endParseSymbol = "]";
        static string[] stringsToRemove = { "\",", "\"" };

        /// <summary>
        /// Opens the input file and transforms it into a set of key value pairs for each pased line
        /// </summary>
        /// <param name="filePath">filePath to source</param>
        /// <returns>dict with [[lineNumber] , [[Column], [Entry]]]</returns>
        public static Dictionary<int, Dictionary<string, string>> ImportFile(string filePath)
        {
            StreamReader r = File.OpenText(filePath);

            Dictionary<int, Dictionary<string, string>> parsedLines =
                new Dictionary<int, Dictionary<string, string>>();

            Dictionary<string, string> columnEntry = null;

            string line;
            bool parseInput = false;

            for (int lineNum = 0; (line = r.ReadLine()) != null; lineNum++)
            {
                line = line.Trim();

                if (line.StartsWith(startParseSymbol))
                    parseInput = true;
                else
                {
                    if (parseInput && (!line.StartsWith(endParseSymbol)))
                    {
                        for (int i = 0; i < stringsToRemove.Length; i++)
                            line = line.Replace(stringsToRemove[i], "");

                        if (line == startLineSymbol)
                        {
                            columnEntry = new Dictionary<string, string>();
                            parsedLines.Add(lineNum, columnEntry);
                        }
                        else
                            if (!line.StartsWith(endLineSymbol))
                                columnEntry.Add(line.Substring(0, line.IndexOf(':')).Trim(),
                                                line.Substring(line.IndexOf(':') + 1).Trim());
                    }
                }
            }

            return parsedLines;
        }
    }
}
