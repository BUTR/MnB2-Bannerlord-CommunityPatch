using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using JetBrains.Annotations;
using TaleWorlds.TwoDimension.Standalone.Native.Windows;

namespace Antijank {

  
  public static class MessageBox {

    [DllImport("user32", SetLastError = true, CharSet = CharSet.Auto, EntryPoint = "MessageBox")]
    private static extern MessageBoxResult Native(IntPtr hWnd, string text, string caption, MessageBoxType type);

    private static Stack<Action> Help = new Stack<Action>();

    private static readonly WindowClass _windowClass;

    private static readonly string WindowClassName = "AntijankMessageBoxHolder";

    static MessageBox() {
      _windowClass = new WindowClass {
        style = 0U,
        lpfnWndProc = new WndProc((hWnd, msg, wParam, lParam) => {
          if (msg == 0x53) // WM_HELP
            Help.Peek()?.Invoke();

          return User32.DefWindowProc(hWnd, msg, wParam, lParam);
        }),
        cbClsExtra = 0,
        cbWndExtra = 0,
        hInstance = Kernel32.GetModuleHandle(null),
        lpszMenuName = null,
        lpszClassName = WindowClassName
      };
      User32.RegisterClass(ref _windowClass);
    }

    private static IntPtr CreateMessageBoxOwnerWindow(IntPtr hWndParent)
      => User32.CreateWindowEx(
        default,
        WindowClassName,
        "Antijank",
        WindowStyle.WS_POPUP | WindowStyle.WS_DISABLED | WindowStyle.WS_VISIBLE,
        0, 0,
        0, 0,
        hWndParent, IntPtr.Zero, Kernel32.GetModuleHandle(null),
        IntPtr.Zero
      );

    public static MessageBoxResult Show(string text, string caption = null, MessageBoxType type = default, Action help = null) {
      try {
        Help.Push(help);
        if (help != null)
          type |= MessageBoxType.Help;
        type |= MessageBoxType.SetForeground | MessageBoxType.SystemModal;

        var result = MessageBoxResult.Error;

        var hWndConsole = Kernel32.GetConsoleWindow();
        if (hWndConsole == default) {
          AppDomainManager.AllocConsole();
          hWndConsole = Kernel32.GetConsoleWindow();
          /*
            User32.ReleaseCapture();
            User32.SetActiveWindow(hWndConsole);
            User32.SetForegroundWindow(hWndConsole);
            */
        }

        try {
          // piggyback on the CSRSS window's window visibility stack
          var hWnd = CreateMessageBoxOwnerWindow(hWndConsole);

          try {
            result = Native(hWnd, text, caption, type);
          }
          finally {
            User32.DestroyWindow(hWnd);
          }
        }
        catch {
          // something happened, check FCE
        }

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