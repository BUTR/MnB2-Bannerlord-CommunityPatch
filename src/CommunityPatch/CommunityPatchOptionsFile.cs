using System.Collections.Generic;

namespace CommunityPatch {

  public sealed class CommunityPatchOptionsFile : OptionsFile {

    public override string Name => "CommunityPatch";

    public CommunityPatchOptionsFile(string fileName) : base(fileName) {
    }

    // essentially this is the option file's schema
    public static readonly Dictionary<string, object> Metadata
      = new Dictionary<string, object> {
        {"SomeNs:SomeFloatOption.Min", 0f},
        {"SomeNs:SomeFloatOption.Max", 1f},
      };

    public override object GetOptionMetadata(string ns, string key, string metadataType)
      => Metadata.TryGetValue(ns == null ? $"{key}.{metadataType}" : $"{ns}:{key}.{metadataType}", out var result) ? result : null;

    public override IEnumerable<Option> GetKnownOptions() {
      yield return new Option<bool>(this, null, "DisableIntroVideo");
      yield return new Option<float>(this, "SomeNamespace", "SomeFloatOption");
      yield return new StringOption(this, "SomeNamespace", "SomeStringOption");
    }

  }

}