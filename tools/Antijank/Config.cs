using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using JetBrains.Annotations;

namespace Antijank {

  [PublicAPI]
  public class Config {

    private XDocument _xDoc;

    public Config(string filePath = null) {
      filePath ??= Path.Combine(PathHelpers.GetConfigsDir(), "user-load-order.xml");

      try {
        if (File.Exists(filePath))
          _xDoc = XDocument.Load(filePath);
        ModuleSequence = _xDoc.Root
          ?.Element("Sequence")
          ?.Elements("Module")
          .Select(el => el.Attribute("id")?.Value)
          .Where(id => id != null)
          .ToList();

        return;
      }
      catch {
        // well ok
      }

      _xDoc ??= new XDocument();
      ModuleSequence ??= new List<string>();
    }

    public IList<string> ModuleSequence { get; private set; }

  }

}