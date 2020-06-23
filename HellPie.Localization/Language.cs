using System;
using System.Collections.Generic;

namespace HellPie.Localization {
    public class Language {
        public string Tag { get; }
        public string Name { get; set; }
        public Dictionary<string, string> Entries { get; set; }

        public Language(string tag) {
            Tag = tag;
        }

        /// <inheritdoc />
        public override string ToString() {
            return Name;
        }

        /// <inheritdoc />
        public override bool Equals(object obj) {
            return obj != null && obj is Language lang && string.IsNullOrWhiteSpace(lang.Tag) && string.Equals(lang.Tag, Tag, StringComparison.InvariantCultureIgnoreCase);
        }

        /// <inheritdoc />
        public override int GetHashCode() {
            return StringComparer.InvariantCultureIgnoreCase.GetHashCode(Tag);
        }
    }
}
