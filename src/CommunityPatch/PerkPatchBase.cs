using System.Collections.Generic;
using JetBrains.Annotations;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;

namespace CommunityPatch {

  [PublicAPI]
  public abstract class PerkPatchBase<TPatch> : PatchBase<TPatch> where TPatch : IPatch {

    private readonly string _perkId;

    private PerkObject _perk;

    [PublicAPI]
    public string PerkName => LocalizedTextManager.GetTranslatedText(BannerlordConfig.Language, _perkId);

    protected PerkPatchBase(string perkId)
      => _perkId = perkId;

    [PublicAPI]
    public PerkObject Perk
      => (_perk ??= PerkObjectHelpers.Load(_perkId))
        ?? throw new KeyNotFoundException($"Can't find the {PerkName} ({_perkId}) perk.");

    public override void Reset()
      => _perk = null;

  }

}