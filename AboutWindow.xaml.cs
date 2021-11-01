using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Navigation;

namespace OffsetEditor
{
    /// <summary>
    /// Interaction logic for AboutWindow.xaml
    /// </summary>
    public partial class AboutWindow : Window
    {
        public AboutWindow()
        {
            InitializeComponent();
        }

        private void CloseButtonClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            //Process.Start(e.Uri.AbsoluteUri));
            //e.Handled = true;
            string url = e.Uri.AbsoluteUri;
            // https://stackoverflow.com/questions/4580263/how-to-open-in-default-browser-in-c-sharp
            try
            {
                Process.Start(url);
            }
            catch
            {
                // hack because of this: https://github.com/dotnet/corefx/issues/10361
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    url = url.Replace("&", "^&");
                    Process.Start(new ProcessStartInfo { FileName = url, UseShellExecute = true });
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    Process.Start("xdg-open", url);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    Process.Start("open", url);
                }
                else
                {
                    throw;
                }
            }
        }
    }
}
