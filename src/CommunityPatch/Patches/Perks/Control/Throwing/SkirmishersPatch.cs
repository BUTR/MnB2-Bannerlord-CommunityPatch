// This perk was removed when the Throwing perk category was revamped.

#if !AFTER_E1_5_1

using Patches;

namespace CommunityPatch.Patches.Perks.Control.Throwing {

  public class SkirmishersPatch : PartySizeLimitSubPatch<SkirmishersPatch> {

    public SkirmishersPatch () : base("cmn6qNoX") { }

  }

}

#endif