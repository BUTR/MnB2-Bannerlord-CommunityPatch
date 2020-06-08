using System;
using TaleWorlds.Library;

namespace CommunityPatch {

  [AttributeUsage(AttributeTargets.Class)]
  public class PatchObsoleteAttribute : Attribute {

    public ApplicationVersion Version { get; }

    public PatchObsoleteAttribute(ApplicationVersionType type, int major, int minor, int revision = 0, int changeSet = 0)
#if AFTER_E1_4_2
      => Version = new ApplicationVersion(type, major, minor, revision, changeSet, ApplicationVersionGameType.Singleplayer);
#else
      => Version = new ApplicationVersion(type, major, minor, revision, changeSet);
#endif

  }

}