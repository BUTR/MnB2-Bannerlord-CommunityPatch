using Patches;

namespace CommunityPatch.Patches.Perks.Control.Bow {

  public class MerryMenPatch : PartySizeLimitSubPatch<MerryMenPatch> {
    
    public MerryMenPatch() : base("ssljPTUr", perk => perk.StringId == "BowMerryMen") { }

  }

}