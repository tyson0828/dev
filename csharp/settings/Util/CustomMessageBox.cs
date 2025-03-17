using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

public static class CustomMessageBox
{
    public static void Show(string message, string title, bool isGuiMode)
    {
        string timestamp = DateTime.Now.ToString("HHmmss");
        string uniqueTitle = $"{title} - {timestamp}";

        if (isGuiMode)
        {
            MessageBox.Show(message, uniqueTitle);
        }
        else
        {
            ShowWithTimeout(message, uniqueTitle, 5);
        }
    }

    public static MessageBoxResult ShowWithConfig(string message, string title, MessageBoxButton buttons,
                                                  MessageBoxImage icon, MessageBoxResult defaultResult, 
                                                  bool isGuiMode, int timeoutMilliseconds = 5000)
    {
        string timestamp = DateTime.Now.ToString("HHmmss");
        string uniqueTitle = $"{title} - {timestamp}";
        MessageBoxResult result = defaultResult;

        if (isGuiMode)
        {
            result = MessageBox.Show(message, uniqueTitle, buttons, icon, defaultResult);
        }
        else
        {
            var messageBoxThread = new Thread(() =>
            {
                result = MessageBox.Show(message, uniqueTitle, buttons, icon, defaultResult);
            });

            messageBoxThread.SetApartmentState(ApartmentState.STA);
            messageBoxThread.Start();

            if (!messageBoxThread.Join(timeoutMilliseconds))
            {
                IntPtr msgBoxHandle = FindWindow(null, uniqueTitle);
                if (msgBoxHandle != IntPtr.Zero)
                {
                    SendMessage(msgBoxHandle, WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
                }
            }
        }

        return result;
    }

    // P/Invoke for interacting with native Windows API
    [DllImport("user32.dll", SetLastError = true)]
    private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    private static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

    private const uint WM_CLOSE = 0x0010;
}
