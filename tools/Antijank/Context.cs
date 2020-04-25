using HarmonyLib;

namespace Antijank {

  static internal class Context {

    public static readonly Harmony Harmony = new Harmony(nameof(Antijank));

  }

}