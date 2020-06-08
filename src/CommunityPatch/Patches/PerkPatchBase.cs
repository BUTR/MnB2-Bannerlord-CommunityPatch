using System;
using System.Runtime.CompilerServices;
using CommunityPatchAnalyzer;
using JetBrains.Annotations;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;

namespace CommunityPatch {

  [PublicAPI]
  public abstract class PerkPatchBase<TPatch> : PatchBase<TPatch> where TPatch : IPatch {

    [PublicAPI]
    public string PerkId { get; }

    private readonly Func<PerkObject, bool> _finder;

    private PerkObject _perk;

    [PublicAPI]
    public string PerkName => LocalizedTextManager.GetTranslatedText(BannerlordConfig.Language, PerkId);

    protected PerkPatchBase(string perkId) {
      PerkId = perkId;
      _finder = perk => perk.Name.GetID() == perkId;
    }

    protected PerkPatchBase(string perkId, Func<PerkObject, bool> disambiguation) {
      PerkId = perkId;
      _finder = perk => perk.Name.GetID() == perkId && disambiguation(perk);
    }

    [PublicAPI]
    [CanBeNull]
    public PerkObject Perk {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get {
        if (_perk != null)
          return _perk;

        _perk = PerkObjectHelpers.Load(_finder);

        if (_perk == null) {
          //throw new KeyNotFoundException($"Can't find the {PerkName} ({_perkId}) perk.");
          CommunityPatchSubModule.Error($"Can't find the {PerkName} ({PerkId}) perk.");
        }

        return _perk;
      }
    }

    [RequireBaseMethodCall]
    public override bool? IsApplicable(Game game) {
      try {
        if (Perk == null)
          return false;
      }
      catch {
        return false;
      }

      return base.IsApplicable(game);
    }

    public override void Reset()
      => _perk = null;

  }

}