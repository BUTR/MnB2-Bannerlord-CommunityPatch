using System.Text.RegularExpressions;
using JetBrains.Annotations;
using TaleWorlds.Localization;

namespace CommunityPatch {

  public static class LocalizationHelpers {

    [CanBeNull]
    [ContractAnnotation("id:notnull => notnull;id:notnull,blankIsDefault:false => notnull;;id:null,blankIsDefault:false => null;blankIsDefault:true => notnull")]
    public static TextObject Localized(this string id, bool blankIsDefault = false)
      => id == null
        ? blankIsDefault ? new TextObject("") : null
        : id.Localized(blankIsDefault ? "" : id.ToSentenceCase());

    [CanBeNull]
    [ContractAnnotation("id:notnull => notnull;id:null,fallback:null => null;fallback:notnull => notnull")]
    public static TextObject Localized(this string id, TextObject fallback) {
      if (id == null)
        return fallback;

      var localized = id.Localized();
      return string.IsNullOrEmpty(localized?.ToString())
        ? fallback
        : localized;
    }

    [CanBeNull]
    [ContractAnnotation("id:notnull => notnull;id:null,fallback:null => null;fallback:notnull => notnull")]
    public static TextObject Localized(this string id, string fallback)
      => id == null
        ? fallback == null ? null : new TextObject(fallback)
        : new TextObject($"{{={id}}}{LocalizedTextManager.GetTranslatedText("English", id) ?? fallback}");

    [CanBeNull]
    [ContractAnnotation("str:null => null;str:notnull => notnull")]
    public static string ToSentenceCase([CanBeNull] this string str)
      => str == null ? null : Regex.Replace(str, "[a-z][A-Z]", m => $"{m.Value[0]} {char.ToLower(m.Value[1])}");

    [CanBeNull]
    [ContractAnnotation("str:null => null;str:notnull => notnull")]
    public static string ToTitleCase([CanBeNull] this string str)
      => str == null ? null : Regex.Replace(str, "[a-z][A-Z]", m => $"{m.Value[0]} {char.ToLower(m.Value[1])}");

  }

}