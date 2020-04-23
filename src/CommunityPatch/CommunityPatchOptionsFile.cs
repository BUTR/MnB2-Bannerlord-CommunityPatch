using System.Collections.Generic;
using CommunityPatch.Options;
using JetBrains.Annotations;

namespace CommunityPatch {

  [PublicAPI]
  public sealed class CommunityPatchOptionsFile : OptionsFile {

    public override string Name => "CommunityPatch";

    public CommunityPatchOptionsFile() : base(nameof(CommunityPatch) + ".txt") {
    }

    // essentially this is the option file's schema
    public static readonly Dictionary<string, object> Metadata
      = new Dictionary<string, object> {
        {"SomeNs:SomeFloatOption.Min", 0f},
        {"SomeNs:SomeFloatOption.Max", 1f},
      };

    public override object GetOptionMetadata(string ns, string key, string metadataType)
      => Metadata.TryGetValue(ns == null ? $"{key}.{metadataType}" : $"{ns}:{key}.{metadataType}", out var result) ? result : null;

    public Option<bool> DisableIntroVideo { get; set; }

    public Option<bool> RecordFirstChanceExceptions { get; set; }

    public Option<bool> DontGroupThirdPartyMenuOptions { get; set; }

    public Option<bool> QuartermasterIsClanWide { get; set; }

  }

}