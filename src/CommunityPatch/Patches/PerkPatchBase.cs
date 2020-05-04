using System.Collections.Generic;
using System.Runtime.CompilerServices;
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
    public PerkObject Perk {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get {
        if (_perk != null)
          return _perk;

        _perk = PerkObjectHelpers.Load(_perkId);
        if (_perk == null)
          throw new KeyNotFoundException($"Can't find the {PerkName} ({_perkId}) perk.");

        return _perk;
      }
    }

    public override void Reset()
      => _perk = null;

  }

}