using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

public static class CustomMessageBox
{
    // Basic Show method (no configuration)
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
            ShowWithTimeout(message, uniqueTitle, MessageBoxButtons.OK, MessageBoxIcon.Information, 5000);
        }
    }

    // Configurable Show method with timeout and DialogResult return
    public static DialogResult ShowWithConfig(string message, string title, MessageBoxButtons buttons,
                                              MessageBoxIcon icon, DialogResult defaultResult,
                                              bool isGuiMode, int timeoutMilliseconds = 5000)
    {
        string timestamp = DateTime.Now.ToString("HHmmss");
        string uniqueTitle = $"{title} - {timestamp}";
        DialogResult result = defaultResult;

        if (isGuiMode)
        {
            result = MessageBox.Show(message, uniqueTitle, buttons, icon, defaultResult);
        }
        else
        {
            result = ShowWithTimeout(message, uniqueTitle, buttons, icon, timeoutMilliseconds, defaultResult);
        }

        return result;
    }

    // Show message box with timeout logic for non-GUI mode
    private static DialogResult ShowWithTimeout(string message, string title, MessageBoxButtons buttons,
                                                MessageBoxIcon icon, int timeoutMilliseconds,
                                                DialogResult defaultResult = DialogResult.None)
    {
        DialogResult result = defaultResult;

        var messageBoxThread = new Thread(() =>
        {
            result = MessageBox.Show(message, title, buttons, icon, defaultResult);
        });

        messageBoxThread.SetApartmentState(ApartmentState.STA);
        messageBoxThread.Start();

        // Wait for timeout, if the thread is still alive, attempt to close the message box
        if (!messageBoxThread.Join(timeoutMilliseconds))
        {
            IntPtr hWnd = FindWindow(null, title);
            if (hWnd != IntPtr.Zero)
            {
                SendMessage(hWnd, WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
            }
        }

        return result;
    }

    // P/Invoke declarations for interacting with Windows API
    [DllImport("user32.dll", SetLastError = true)]
    private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    private static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

    private const uint WM_CLOSE = 0x0010;
}
