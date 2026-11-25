using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;

namespace Gravity
{
    /// <summary>
    /// Interaction logic for CBView.xaml
    /// </summary>
    public partial class CBView : UserControl
    {
        #region Properties

        private string mode = "Edit";
        private Universe universe;
        private struct namedBrush
        {
            private Brush brush;
            private string name;

            public Brush Brush
            {
                get
                {
                    return brush;
                }
                set
                {
                    brush = value;
                }
            }

            public string Name
            {
                get
                {
                    return name;
                }
                set
                {
                    name = value;
                }
            }

            public namedBrush(Brush b, string n)
            {
                brush = b;
                name = n;
            }
        }
        private ObservableCollection<namedBrush> brushCollection = new ObservableCollection<namedBrush>();

        public string Mode
        {
            get
            {
                return mode;
            }
            set
            {
                if ((value != "Edit") && (value != "New"))
                {
                    throw new ArgumentOutOfRangeException("Mode", "Mode for CBView must be \"Edit\" or \"New\".");
                }
                mode = value;

                switch (Mode)
                {
                    case "New":
                        btnConfirmAdd.Visibility = Visibility.Visible;
                        btnCancel.Visibility = Visibility.Visible;
                        break;
                    case "Edit":
                        btnConfirmAdd.Visibility = Visibility.Collapsed;
                        btnCancel.Visibility = Visibility.Collapsed;
                        break;
                }
            }
        }

        public Universe Universe
        {
            get
            {
                return universe;
            }
            set
            {
                universe = value;
                cbOther.ItemsSource = universe.Children;
            }
        }

        #endregion

        #region Events

        public event EventHandler AddConfirmed;
        public event EventHandler AddCanceled;
        public event EventHandler Explode;

        protected void OnAddConfirmed()
        {
            EventHandler handler = AddConfirmed;
            if (handler != null)
            {
                handler(this, new EventArgs());
            }
        }

        protected void OnAddCanceled()
        {
            EventHandler handler = AddCanceled;
            if (handler != null)
            {
                handler(this, new EventArgs());
            }
        }

        protected void OnExplode()
        {
            EventHandler handler = Explode;
            if (handler != null)
            {
                handler(this, new EventArgs());
            }
        }

        #endregion

        #region Constructors

        public CBView()
        {
            InitializeComponent();

            if (Universe != null)
            {
                cbOther.ItemsSource = Universe.Children;
            }

            loadBrushCollection();
            BrushList.ItemsSource = brushCollection;
        }

        #endregion

        #region Control Event Handling

        private void btnConfirmAdd_Click(object sender, RoutedEventArgs e)
        {
            OnAddConfirmed();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            OnAddCanceled();
        }

        private void btnExplode_Click(object sender, RoutedEventArgs e)
        {
            OnExplode();
        }

        private void btnUpLeft_Click(object sender, RoutedEventArgs e)
        {
            if (verifyRotateInfo())
            {
                ((CelestialBody)this.Tag).CreateOrbit((CelestialBody)cbOther.SelectedItem, "UpLeft", Double.Parse(txtOrbitRadius.Text));
            }
        }

        private void btnUp_Click(object sender, RoutedEventArgs e)
        {
            if (verifyRotateInfo())
            {
                ((CelestialBody)this.Tag).CreateOrbit((CelestialBody)cbOther.SelectedItem, "Up", Double.Parse(txtOrbitRadius.Text));
            }
        }

        private void btnUpRight_Click(object sender, RoutedEventArgs e)
        {
            if (verifyRotateInfo())
            {
                ((CelestialBody)this.Tag).CreateOrbit((CelestialBody)cbOther.SelectedItem, "UpRight", Double.Parse(txtOrbitRadius.Text));
            }
        }

        private void btnLeft_Click(object sender, RoutedEventArgs e)
        {
            if (verifyRotateInfo())
            {
                ((CelestialBody)this.Tag).CreateOrbit((CelestialBody)cbOther.SelectedItem, "Left", Double.Parse(txtOrbitRadius.Text));
            }
        }

        private void btnRight_Click(object sender, RoutedEventArgs e)
        {
            if (verifyRotateInfo())
            {
                ((CelestialBody)this.Tag).CreateOrbit((CelestialBody)cbOther.SelectedItem, "Right", Double.Parse(txtOrbitRadius.Text));
            }
        }

        private void btnDownLeft_Click(object sender, RoutedEventArgs e)
        {
            if (verifyRotateInfo())
            {
                ((CelestialBody)this.Tag).CreateOrbit((CelestialBody)cbOther.SelectedItem, "DownLeft", Double.Parse(txtOrbitRadius.Text));
            }
        }

        private void btnDown_Click(object sender, RoutedEventArgs e)
        {
            if (verifyRotateInfo())
            {
                ((CelestialBody)this.Tag).CreateOrbit((CelestialBody)cbOther.SelectedItem, "Down", Double.Parse(txtOrbitRadius.Text));
            }
        }

        private void btnDownRight_Click(object sender, RoutedEventArgs e)
        {
            if (verifyRotateInfo())
            {
                ((CelestialBody)this.Tag).CreateOrbit((CelestialBody)cbOther.SelectedItem, "DownRight", Double.Parse(txtOrbitRadius.Text));
            }
        }

        private void CelestialBodyViewport3D_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            CelestialBodyViewport3D.Visibility = Visibility.Collapsed;
            BrushScroll.Visibility = Visibility.Visible;
            e.Handled = true;
        }

        private void BrushSelector_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            BrushScroll.Visibility = Visibility.Collapsed;
            CelestialBodyViewport3D.Visibility = Visibility.Visible;
            e.Handled = true;

            ((CelestialBody)this.Tag).SetMaterial(new DiffuseMaterial(((Rectangle)sender).Fill));
        }

        #endregion

        private bool verifyRotateInfo()
        {
            if (cbOther.SelectedItem == null)
            {
                MessageBox.Show("Object to orbit must be selected first", "Missing Object", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return false;
            }

            if (cbOther.SelectedItem == this.Tag)
            {
                MessageBox.Show("Object cannot orbit itself", "Invalid Object", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return false;
            }

            if (!Double.TryParse(txtOrbitRadius.Text, out double outRadius))
            {
                MessageBox.Show("Invalid orbit radius", "Invalid Radius", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return false;
            }

            return true;
        }

        private void UserControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (this.Visibility == Visibility.Visible)
            {
                cbOther.SelectedItem = null;
                txtOrbitRadius.Text = null;

                cbOther.ItemsSource = null;                             // Need to set to null and back when made visible to get any updates to names
                cbOther.ItemsSource = Universe.Children;

                BrushScroll.Visibility = Visibility.Collapsed;
                CelestialBodyViewport3D.Visibility = Visibility.Visible;
            }
        }

        private void loadBrushCollection()
        {
            brushCollection.Add(new namedBrush(new SolidColorBrush(Colors.Blue), "Blue"));
            brushCollection.Add(new namedBrush(new SolidColorBrush(Colors.Coral), "Coral"));
            brushCollection.Add(new namedBrush(new SolidColorBrush(Colors.Crimson), "Crimson"));
            brushCollection.Add(new namedBrush(new SolidColorBrush(Colors.FloralWhite), "FloralWhite"));
            brushCollection.Add(new namedBrush(new SolidColorBrush(Colors.ForestGreen), "ForestGreen"));
            brushCollection.Add(new namedBrush(new SolidColorBrush(Colors.Fuchsia), "fuchsia"));
            brushCollection.Add(new namedBrush(new SolidColorBrush(Colors.Goldenrod), "Goldenrod"));
            brushCollection.Add(new namedBrush(new SolidColorBrush(Colors.Green), "Green"));
            brushCollection.Add(new namedBrush(new SolidColorBrush(Colors.HotPink), "HotPink"));
            brushCollection.Add(new namedBrush(new SolidColorBrush(Colors.Lavender), "Lavender"));
            brushCollection.Add(new namedBrush(new SolidColorBrush(Colors.Orange), "Orange"));
            brushCollection.Add(new namedBrush(new SolidColorBrush(Colors.Pink), "Pink"));
            brushCollection.Add(new namedBrush(new SolidColorBrush(Colors.Purple), "Purple"));
            brushCollection.Add(new namedBrush(new SolidColorBrush(Colors.Red), "Red"));
            brushCollection.Add(new namedBrush(new SolidColorBrush(Colors.RoyalBlue), "RoyalBlue"));
            brushCollection.Add(new namedBrush(new SolidColorBrush(Colors.SeaGreen), "SeaGreen"));
            brushCollection.Add(new namedBrush(new SolidColorBrush(Colors.SkyBlue), "SkyBlue"));
            brushCollection.Add(new namedBrush(new SolidColorBrush(Colors.Violet), "Violet"));
            brushCollection.Add(new namedBrush(new SolidColorBrush(Colors.Yellow), "Yellow"));
            brushCollection.Add(new namedBrush(new LinearGradientBrush(Colors.Red, Colors.Gray, 0.0), "Red/Grey Gradient"));

            LinearGradientBrush tempLinearGradientBrush = new LinearGradientBrush();
            tempLinearGradientBrush.StartPoint = new Point(0, 0);
            tempLinearGradientBrush.EndPoint = new Point(1, 1);
            tempLinearGradientBrush.GradientStops.Add(new GradientStop(Colors.Yellow, 0.0));
            tempLinearGradientBrush.GradientStops.Add(new GradientStop(Colors.Red, 0.25));
            tempLinearGradientBrush.GradientStops.Add(new GradientStop(Colors.Blue, 0.75));
            tempLinearGradientBrush.GradientStops.Add(new GradientStop(Colors.LimeGreen, 1.0));
            brushCollection.Add(new namedBrush(tempLinearGradientBrush, "Yellow/Red/Blue/LimeGreen Gradient"));

            LinearGradientBrush tempLinearGradientBrush2 = new LinearGradientBrush();
            tempLinearGradientBrush2.StartPoint = new Point(0.5, 0);
            tempLinearGradientBrush2.EndPoint = new Point(0.5, 1);
            tempLinearGradientBrush2.GradientStops.Add(new GradientStop(Colors.Red, 0.0));
            tempLinearGradientBrush2.GradientStops.Add(new GradientStop(Colors.Orange, 0.167));
            tempLinearGradientBrush2.GradientStops.Add(new GradientStop(Colors.Yellow, 0.333));
            tempLinearGradientBrush2.GradientStops.Add(new GradientStop(Colors.Green, 0.5));
            tempLinearGradientBrush2.GradientStops.Add(new GradientStop(Colors.Blue, 0.666));
            tempLinearGradientBrush2.GradientStops.Add(new GradientStop(Colors.Indigo, 0.833));
            tempLinearGradientBrush2.GradientStops.Add(new GradientStop(Colors.Violet, 1.0));
            brushCollection.Add(new namedBrush(tempLinearGradientBrush2, "Rainbow"));

            DirectoryInfo directoryPath = new DirectoryInfo(Gravity.Properties.Settings.Default.TexturesPath);
            FileInfo[] textures = directoryPath.GetFiles();
            ImageBrush addBrush;

            foreach (FileInfo texture in textures)
            {
                if (texture.Extension.Equals(".jpg", StringComparison.CurrentCultureIgnoreCase) || texture.Extension.Equals(".png", StringComparison.CurrentCultureIgnoreCase) || texture.Extension.Equals(".bmp", StringComparison.CurrentCultureIgnoreCase))
                {
                    addBrush = new ImageBrush();
                    addBrush.ImageSource = new BitmapImage(new Uri(texture.FullName, UriKind.RelativeOrAbsolute));
                    brushCollection.Add(new namedBrush(addBrush, System.IO.Path.GetFileNameWithoutExtension(texture.Name)));
                }
            }
        }

    }
}
