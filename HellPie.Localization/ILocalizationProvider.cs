using System.Collections.Generic;

namespace HellPie.Localization {
    public interface ILocalizationProvider {
        IEnumerable<Language> Languages { get; }
        Language Default { get; }

        Language GetLanguage(string code);
        string GetString(Language language, string key);
        string GetString(Language language, string key, params object[] args);
    }
}
