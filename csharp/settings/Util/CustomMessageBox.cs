using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

public static class CustomMessageBox
{
    /// <summary>
    /// Displays a basic message box with a unique title. If in non-GUI mode, the message box will automatically close after 5 seconds.
    /// </summary>
    /// <param name="message">The message to be displayed in the message box.</param>
    /// <param name="title">The title of the message box.</param>
    /// <param name="isGuiMode">Indicates whether the application is running in GUI mode or not.</param>
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

    /// <summary>
    /// Displays a configurable message box with timeout and returns the user's response.
    /// </summary>
    /// <param name="message">The message to be displayed in the message box.</param>
    /// <param name="title">The title of the message box.</param>
    /// <param name="buttons">The type of buttons to be displayed in the message box.</param>
    /// <param name="icon">The icon to be displayed in the message box.</param>
    /// <param name="defaultResult">The default result to return if the message box times out.</param>
    /// <param name="isGuiMode">Indicates whether the application is running in GUI mode or not.</param>
    /// <param name="timeoutMilliseconds">The timeout duration in milliseconds for non-GUI mode.</param>
    /// <returns>The result of the user's interaction with the message box.</returns>
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

    /// <summary>
    /// Displays a message box that automatically closes after a specified timeout in non-GUI mode.
    /// </summary>
    /// <param name="message">The message to be displayed in the message box.</param>
    /// <param name="title">The title of the message box (should be unique to avoid conflicts).</param>
    /// <param name="buttons">The type of buttons to be displayed in the message box.</param>
    /// <param name="icon">The icon to be displayed in the message box.</param>
    /// <param name="timeoutMilliseconds">The timeout duration in milliseconds after which the message box will automatically close.</param>
    /// <param name="defaultResult">The default result to return if the message box is closed automatically.</param>
    /// <returns>The result of the user's interaction or the default result if the message box times out.</returns>
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

        // Wait for the timeout, then attempt to close the message box if it's still open
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

    /// <summary>
    /// Finds a window by its class name and window title.
    /// </summary>
    /// <param name="lpClassName">The class name of the window. Use null to search by title only.</param>
    /// <param name="lpWindowName">The title of the window to search for.</param>
    /// <returns>The handle to the window if found; otherwise, IntPtr.Zero.</returns>
    [DllImport("user32.dll", SetLastError = true)]
    private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

    /// <summary>
    /// Sends a message to the specified window.
    /// </summary>
    /// <param name="hWnd">The handle to the window.</param>
    /// <param name="Msg">The message to send.</param>
    /// <param name="wParam">Additional message-specific information.</param>
    /// <param name="lParam">Additional message-specific information.</param>
    /// <returns>The result of the message processing.</returns>
    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    private static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

    private const uint WM_CLOSE = 0x0010;
}
