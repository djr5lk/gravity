using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace Gravity
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            if (!Directory.Exists(Gravity.Properties.Settings.Default.TexturesPath))
            {
                var dialog = new System.Windows.Forms.FolderBrowserDialog();
                dialog.Description = "Texture Folder not found.  Select folder containing textures.";
                dialog.ShowNewFolderButton = false;
                System.Windows.Forms.DialogResult result = dialog.ShowDialog();

                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    Gravity.Properties.Settings.Default.TexturesPath = dialog.SelectedPath;
                    Gravity.Properties.Settings.Default.Save();
                    dialog.Dispose();
                }
                else
                {
                    dialog.Dispose();
                    Application.Current.Shutdown();
                }
            }

            EventManager.RegisterClassHandler(typeof(TextBox),
                TextBox.GotFocusEvent,
                new RoutedEventHandler(TextBox_GotFocus));

            base.OnStartup(e);
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            (sender as TextBox).SelectAll();
        }
    }
}
