using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Text;
using Microsoft.Build.Definition;
using Microsoft.Build.Evaluation;

namespace PackageRelease {

  class Program {

    static void Main(string[] args) {
      var asmDir = Path.GetDirectoryName(new Uri(typeof(Program).Assembly.CodeBase).LocalPath);
      Environment.CurrentDirectory = new Uri(Path.Combine(asmDir, "..", "..", "..", "..", "..")).LocalPath;
      using (var proc = new Process()) {
        proc.StartInfo = new ProcessStartInfo {
          FileName ="git",
          Arguments = "archive --prefix=CommunityPatch/ -o CommunityPatch.zip -9 HEAD",
          UseShellExecute = true
        };
        proc.Start();
        proc.WaitForExit();
      }

      using var fs = File.Open("CommunityPatch.zip", FileMode.Open);
      using var za = new ZipArchive(fs, ZipArchiveMode.Update, true, Encoding.UTF8);
      var dir = Path.Combine(Environment.CurrentDirectory, "bin", "Win64_Shipping_Client");
      foreach (var filePath in Directory.EnumerateFiles(dir)) {
        var fi = new FileInfo(filePath);
        if (fi.Name[0] == '.') continue;
        using var file = fi.OpenRead();
        var ze = za.CreateEntry("CommunityPatch/bin/Win64_Shipping_Client/" + fi.Name, CompressionLevel.Optimal);
        using var zs = ze.Open();
        file.CopyTo(zs);
      }
    }

  }

}
