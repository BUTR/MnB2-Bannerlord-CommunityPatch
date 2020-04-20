using System;

namespace Antijank {

  [Flags]
  public enum MessageBoxType : uint {

    Ok = 0,

    Cancel = 1 << 0,

    AbortRetryIgnore = 1 << 1,

    YesNo = 1 << 2,

    CancelTryAgainContinue = AbortRetryIgnore | YesNo,

    RetryCancel = Cancel | YesNo,

    YesNoCancel = AbortRetryIgnore | Cancel,

    IconError = 1 << 4,

    IconQuestion = 1 << 5,

    IconInformation = 1 << 6,

    IconWarning = IconError | IconQuestion,

    DefaultButtonSecond = 1 << 8,

    DefaultButtonThird = 1 << 9,

    DefaultButtonFourth = DefaultButtonSecond | DefaultButtonThird,

    SystemModal = 1 << 12,

    TaskModal = 1 << 13,

    Help = 1 << 14,

    SetForeground = 1 << 16,

    DefaultDesktopOnly = 1 << 17,

    TopMost = 1 << 18,

    RightJustifiedText = 1 << 19,

    RightToLeft = 1 << 20,

    ServiceNotification = 1 << 21,

  }

}