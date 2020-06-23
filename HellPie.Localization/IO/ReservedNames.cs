using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace HellPie.Localization.IO {
    public static class ReservedNames {
        private static readonly string[] Names = {
            "CON", "PRN", "AUX", "CLOCK$", "NUL", "COM0", "COM1", "COM2", "COM3", "COM4", "COM5", "COM6", "COM7", "COM8", "COM9", "LPT0", "LPT1", "LPT2", "LPT3", "LPT4", "LPT5", "LPT6", "LPT7", "LPT8", "LPT9"
        };

        private static readonly char[] Chars = Path.GetInvalidPathChars().Concat(new [] {
            Path.VolumeSeparatorChar,
            Path.DirectorySeparatorChar,
            Path.AltDirectorySeparatorChar,
            Path.PathSeparator,
        }).ToArray();

        private static readonly Regex[] NamesMatchers = Names.Select(n => new Regex($"^{n}\\.", RegexOptions.Compiled)).ToArray();
        private static readonly Regex CharsMatcher = new Regex($"[{Regex.Escape(new string(Chars))}]+", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public static string Sanitize(string name) {
            string sanitized = CharsMatcher.Replace(name, " ");
            return NamesMatchers.Aggregate(sanitized, (i, m) => m.Replace(i, "_reservedWord_"));
        }
    }
}
