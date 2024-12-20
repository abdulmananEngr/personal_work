using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
//string exePath = "C:\\Program Files\\JetBrains\\PyCharm 2022.1\\bin\\pycharm64.exe";
namespace ExternalApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Process _externalAppProcess;
        public MainWindow()
        {
            InitializeComponent();
        }
        // P/Invoke declarations
        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        private const int GWL_STYLE = -16;
        private const int WS_CHILD = 0x40000000;

        private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        private IntPtr FindWindowForProcess(int processId)
        {
            IntPtr windowHandle = IntPtr.Zero;

            EnumWindows((hWnd, lParam) =>
            {
                GetWindowThreadProcessId(hWnd, out uint windowProcessId);
                if (windowProcessId == processId)
                {
                    windowHandle = hWnd;
                    return false; // Stop enumeration
                }
                return true; // Continue enumeration
            }, IntPtr.Zero);

            return windowHandle;
        }

        private IntPtr WaitForMainWindowHandle(Process process)
        {
            const int timeout = 10000; // 10 seconds
            const int interval = 100; // 100 ms
            int waited = 0;

            while (waited < timeout)
            {
                if (process.HasExited)
                    throw new InvalidOperationException("The external application has exited.");

                process.Refresh(); // Refresh process information
                IntPtr handle = process.MainWindowHandle;

                Console.WriteLine($"Checked process MainWindowHandle: {handle} (Elapsed: {waited}ms)");

                if (handle != IntPtr.Zero)
                    return handle;

                Thread.Sleep(interval);
                waited += interval;
            }

            Console.WriteLine("Timeout exceeded while waiting for the MainWindowHandle.");

            // If MainWindowHandle is not found, fallback to FindWindowForProcess
            return FindWindowForProcess(process.Id);
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_externalAppProcess != null && !_externalAppProcess.HasExited)
                {
                    MessageBox.Show("The external application is already running.");
                    return;
                }

                // Path to the external application (replace with calc.exe for testing if needed)
                string exePath = "C:\\Program Files\\JetBrains\\PyCharm 2022.1\\bin\\pycharm64.exe";

                // Start the external application
                _externalAppProcess = Process.Start(exePath);

                // Wait for the main window handle
                IntPtr mainWindowHandle = WaitForMainWindowHandle(_externalAppProcess);
                if (mainWindowHandle == IntPtr.Zero)
                {
                    MessageBox.Show("Failed to retrieve the main window handle of the external application.");
                    return;
                }

                // Get the handle of the AppContainer
                HwndSource hwndSource = (HwndSource)PresentationSource.FromVisual(AppContainer);
                if (hwndSource == null)
                {
                    MessageBox.Show("Failed to retrieve the AppContainer handle.");
                    return;
                }
                IntPtr containerHandle = hwndSource.Handle;

                // Embed the external application into the AppContainer
                SetParent(mainWindowHandle, containerHandle);

                // Set the window style to WS_CHILD
                int style = GetWindowLong(mainWindowHandle, GWL_STYLE);
                SetWindowLong(mainWindowHandle, GWL_STYLE, style | WS_CHILD);

                // Resize and position the embedded window to fit the AppContainer
                MoveWindow(mainWindowHandle, 0, 0,
                    (int)AppContainer.ActualWidth, (int)AppContainer.ActualHeight, true);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error embedding external application: {ex.Message}");
            }
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_externalAppProcess != null && !_externalAppProcess.HasExited)
                {
                    _externalAppProcess.Kill();
                    _externalAppProcess = null;
                }
                else
                {
                    MessageBox.Show("No external application is currently running.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error stopping external application: {ex.Message}");
            }
        }
    }
}