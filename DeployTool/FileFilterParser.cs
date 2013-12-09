﻿// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace DeployTool
{
    public static class FileFilterParser
    {
        public static FileFilter Parse(string path, IEnumerable<string> filenames)
        {
            var result = new FileFilter(filenames);
            var isSlashed = false;
            Action init = () =>
            {
                isSlashed = false;
            };
            Func<char, bool> splitter = c =>
            {
                if (c == '\\')
                {
                    isSlashed = true;
                    return false;
                }

                if (c == ' ' && !isSlashed)
                    return true;

                isSlashed = false;
                return false;
            };
            using (var reader = new StreamReader(path))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    if (line.TrimStart().StartsWith("#") || line.Trim() == "")
                        continue;
                    var mode = line.Split(splitter, 2).ToStringArray()[0];
                    switch (mode)
                    {
                        case "include":
                            result.ApplyInclude(line.Init(init).Split(splitter, 2).ToStringArray()[1]);
                            break;
                        case "exclude":
                            result.ApplyExclude(line.Init(init).Split(splitter, 2).ToStringArray()[1]);
                            break;
                        case "rewrite":
                            result.ApplyRewrite(line.Init(init).Split(splitter, 3).ToStringArray()[1], line.Split(splitter, 3).ToStringArray()[2]);
                            break;
                    }
                }
            }
            return result;
        }
        // Helper function to do some initialization before
        public static IEnumerable<T> Init<T>(this IEnumerable<T> enumerable, Action init)
        {
            init();
            return enumerable;
        }
        // Helper function for dynamically splitting strings.
        // From http://stackoverflow.com/questions/298830/split-string-containing-command-line-parameters-into-string-in-c-sharp/298990#298990.
        public static IEnumerable<IEnumerable<T>> Split<T>(this IEnumerable<T> input, Func<T, bool> controller, int max = 0)
        {
            var nextPiece = 0;
            var count = 0;

            for (var c = 0; c < input.Count() && (count < max || max == 0); c++)
            {
                if (controller(input.ElementAt(c)))
                {
                    yield return input.Where((value, row) => row >= nextPiece && row < c);
                    nextPiece = c + 1;
                }
            }

            yield return input.Where((value, row) => row >= nextPiece);
        }
        // Helper function to convert a set of IEnumerable<char> to strings.
        public static IEnumerable<string> ToStringSet(this IEnumerable<IEnumerable<char>> enumerable)
        {
            return enumerable.Select(v => new string(v.ToArray()));
        }
        // Helper function to convert a set of IEnumerable<char> to strings.
        public static string[] ToStringArray(this IEnumerable<IEnumerable<char>> enumerable)
        {
            return enumerable.ToStringSet().ToArray();
        }
    }
}
