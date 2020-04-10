using System.Text.RegularExpressions;
using TaleWorlds.Localization;

namespace CommunityPatch {

  public static class LocalizationHelpers {

    public static TextObject Localized(this string id, bool blankIsDefault = false)
      => id.Localized(blankIsDefault ? "" : id.ToSentenceCase());

    public static TextObject Localized(this string id, TextObject fallback) {
      var localized = id.Localized();
      return localized.ToString() == "" ? fallback : localized;
    }

    public static TextObject Localized(this string id, string fallback)
      => new TextObject($"{{={id}}}{LocalizedTextManager.GetTranslatedText("English", id) ?? fallback}");

    public static string ToSentenceCase(this string str)
      => Regex.Replace(str, "[a-z][A-Z]", m => $"{m.Value[0]} {char.ToLower(m.Value[1])}");

    public static string ToTitleCase(this string str)
      => Regex.Replace(str, "[a-z][A-Z]", m => $"{m.Value[0]} {char.ToLower(m.Value[1])}");

  }

}