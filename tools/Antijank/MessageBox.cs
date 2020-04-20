using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using JetBrains.Annotations;
using TaleWorlds.Library;
using TaleWorlds.TwoDimension.Standalone;
using TaleWorlds.TwoDimension.Standalone.Native.Windows;

namespace Antijank {

  [PublicAPI]
  public static class MessageBox {

    [DllImport("user32", SetLastError = true, CharSet = CharSet.Auto, EntryPoint = "MessageBox")]
    private static extern MessageBoxResult Native(IntPtr hWnd, string text, string caption, MessageBoxType type);

    private static readonly ResourceDepot OwnerFormResourceDepot;

    private static Stack<Action> Help = new Stack<Action>();

    public static MessageBoxResult Show(string text, string caption = null, MessageBoxType type = default, Action help = null) {
      try {
        Help.Push(help);
        if (help != null)
          type |= MessageBoxType.Help;
        type |= MessageBoxType.SetForeground | MessageBoxType.SystemModal;
        try {
        }
        catch {
          //ok
        }

        var hWndConsole = Kernel32.GetConsoleWindow();
        if (hWndConsole == default) {
          AppDomainManager.AllocConsole();
          User32.ReleaseCapture();
          hWndConsole = Kernel32.GetConsoleWindow();
          User32.SetActiveWindow(hWndConsole);
          User32.SetForegroundWindow(hWndConsole);
          User32.SetForegroundWindow(hWndConsole);
        }

        var result = Native(hWndConsole, text, caption, type);

        if (result == MessageBoxResult.Error)
          Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());

        if (!Options.EnableDiagnosticConsole) {
          AppDomainManager.FreeConsole();
        }

        return result;
      }
      finally {
        Help.Pop();
      }
    }

    public static MessageBoxResult Info(string text, string caption = null, MessageBoxType type = default, Action help = null)
      => Show(text, caption, type | MessageBoxType.IconInformation, help);

    public static MessageBoxResult Warning(string text, string caption = null, MessageBoxType type = default, Action help = null)
      => Show(text, caption, type | MessageBoxType.IconWarning, help);

    public static MessageBoxResult Error(string text, string caption = null, MessageBoxType type = default, Action help = null)
      => Show(text, caption, type | MessageBoxType.IconError, help);

  }

}