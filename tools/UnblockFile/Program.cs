using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using Com;

namespace UnblockFile {

  class Program {

    static void Main(string[] args) {
      foreach (var fileName in args) {
        var pzi = new PersistentZoneIdentifier();
        pzi.Load(fileName, StorageMode.Read);
        pzi.Remove();
      }
    }

  }

}