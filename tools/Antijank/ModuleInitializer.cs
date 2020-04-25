using System;
using Antijank;

public static class ModuleInitializer {

  public static void Initialize() {
    try {
      CosturaUtility.Initialize();
    }
    catch (Exception ex) {
      Logging.Log(ex);
    }
  }

}