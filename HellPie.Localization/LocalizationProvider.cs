using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using HellPie.Localization.IO;

namespace HellPie.Localization {
    public class LocalizationProvider {
        private static readonly string InternalLanguagesPath = Path.Combine(Paths.LibraryPath(), "Languages");
        private static readonly string ExternalLanguagesPath = Path.Combine(Paths.DataPath(), "Languages");

        private readonly List<Language> _languages = new List<Language>();
        private Language _default;

        public IEnumerable<Language> Languages => _languages.OrderBy(i => i.Name);
        public Language Default => _default;

        public LocalizationProvider() {
            if(!Directory.Exists(ExternalLanguagesPath)) {
                Directory.CreateDirectory(ExternalLanguagesPath);
            }

            ReloadLanguages();
        }

        public Language GetLanguage(string code) {
            if(string.IsNullOrWhiteSpace(code)) {
                throw new ArgumentNullException(nameof(code), "Only valid IETF Language Tag can be requested.");
            }

            return _languages.FirstOrDefault(i => string.Equals(i.Code, code, StringComparison.InvariantCultureIgnoreCase));
        }

        public string GetString(Language language, string key) {
            if(language == null) {
                throw new ArgumentNullException(nameof(language), "A valid Language must always be provided.");
            }

            if(string.IsNullOrEmpty(key)) {
                throw new ArgumentNullException(nameof(key), "Key must not be empty and must exist in the provided Language.");
            }

            if(language.Entries.ContainsKey(key)) {
                string entry = language.Entries[key];
                // Note: Only default if null is returned in case the key is supposed to be whitespace.
                return entry ?? _default.Entries[key];
            }

            if(_default.Entries.ContainsKey(key)) {
                string entry = _default.Entries[key];
                return entry ?? throw new NullReferenceException("Requested key exists but value was empty or null in requested Language and default Language.");
            }

            throw new KeyNotFoundException("Unable to find requested key in either requested Language or default Language.");
        }

        private void ReloadLanguages() {
            _languages.Clear();

            string[] internalLanguages = Directory.GetFiles(InternalLanguagesPath, "*.xml");
            string[] externalLanguages = Directory.GetFiles(ExternalLanguagesPath, "*.xml");

            // Load external languages before internal languages to allow the user to provide their own.
            foreach(string language in internalLanguages) {
                try {
                    Language loadedLanguage = LoadFrom(File.OpenRead(language));
                    if(loadedLanguage == null) {
                        continue;
                    }

                    _languages.Add(LoadFrom(File.OpenRead(language)));
                } catch {
                    // Ignored
                }
            }

            foreach(string language in externalLanguages) {
                Language loadedLanguage = LoadFrom(File.OpenRead(language));
                if(loadedLanguage == null) {
                    throw new NullReferenceException($"Failed loading application Language in file \"{language}\".");
                }

                if(!_languages.Contains(loadedLanguage)) {
                    _languages.Add(loadedLanguage);
                }

                if(string.Equals(loadedLanguage.Code, CultureInfo.CurrentCulture.Name, StringComparison.InvariantCultureIgnoreCase)) {
                    _default = loadedLanguage;
                }
            }
        }

        private static Language LoadFrom(Stream stream) {
            XDocument document = XDocument.Load(stream);
            XElement root = document.Element("language");

            if(root == null) {
                return null;
            }

            string code = root.Attribute("code")?.Value;

            if(string.IsNullOrWhiteSpace(code)) {
                return null;
            }

            Language language = new Language(code);
            string name = root.Attribute("name")?.Value;
            if(!string.IsNullOrWhiteSpace(name)) {
                language.Name = name;
            }

            List<XElement> xmlEntries = root.Elements("string").ToList();
            if(xmlEntries.Count == 0) {
                return null;
            }

            Dictionary<string, string> entries = new Dictionary<string, string>();
            foreach(XElement xmlEntry in xmlEntries) {
                string key = xmlEntry.Attribute("key")?.Value;
                if(string.IsNullOrWhiteSpace(key) || entries.ContainsKey(key)) {
                    continue;
                }

                string value = xmlEntry.Value;
                entries.Add(key, value);
            }

            if(entries.Count == 0) {
                return null;
            }

            language.Entries = entries;
            return language;
        }
    }
}
