using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Media.Media3D;

namespace Gravity
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Properties

        public Universe myUniverse;
        public CelestialBody testPlanet; // TODO: Remove property once testing done

        #endregion

        #region Constructors

        public MainWindow()
        {
            InitializeComponent();

            myUniverse = new Universe();
            universeViewport3D.Children.Add(myUniverse.UniverseModelVisual3D);
            universeViewport3D.Camera = myUniverse.UniverseCamera;

            BodyView.Universe = myUniverse;         // Need to give the BodyView usercontrol a reference to the universe so it can have list of potential objects to orbit

            BodyView.AddConfirmed += BodyView_AddConfirmed;
            BodyView.AddCanceled += BodyView_AddCanceled;

            /*
            // TODO: Remove below section once testing done
            ImageBrush sunBrush = new ImageBrush();
            string sunPath = Gravity.Properties.Settings.Default.TexturesPath + "\\sun.jpg";
            sunBrush.ImageSource =
                new BitmapImage(
                    new Uri(sunPath, UriKind.RelativeOrAbsolute)
                );
            testPlanet = new CelestialBody(150, new DiffuseMaterial(sunBrush), new Vector3D(0, 1, 0), 10.0);
            testPlanet.Mass = 1000000000;
            testPlanet.Name = "Sun";
            myUniverse.Add(testPlanet);
            // END remove section

             */

            CelestialBody sun, mercury, venus, earth, mars, jupiter, saturn, uranus, neptune, pluto;
            ImageBrush sunBrush = new ImageBrush(),mercuryBrush = new ImageBrush(),venusBrush = new ImageBrush(),earthBrush = new ImageBrush(),marsBrush = new ImageBrush(),jupiterBrush = new ImageBrush(),
                        saturnBrush = new ImageBrush(),uranusBrush = new ImageBrush(),neptuneBrush = new ImageBrush(),plutoBrush = new ImageBrush();
            
            string bPath = Gravity.Properties.Settings.Default.TexturesPath + "\\sun.jpg";
            sunBrush.ImageSource =
                new BitmapImage(
                    new Uri(bPath, UriKind.RelativeOrAbsolute)
                );
            sun = new CelestialBody(696, new DiffuseMaterial(sunBrush), new Vector3D(0, -1, 0), 25.0);
            sun.Mass = 2000000000;
            sun.Name = "Sun";
            myUniverse.Add(sun);

            bPath = Gravity.Properties.Settings.Default.TexturesPath + "\\mercury.jpg";
            mercuryBrush.ImageSource =
                new BitmapImage(
                    new Uri(bPath, UriKind.RelativeOrAbsolute)
                );
            mercury = new CelestialBody(24, new DiffuseMaterial(mercuryBrush), new Vector3D(0, -1, 0), 25.0);
            mercury.Mass = 10000;
            mercury.Name = "Mercury";
            mercury.CreateOrbit(sun, "Right", 1160);
            myUniverse.Add(mercury);

            bPath = Gravity.Properties.Settings.Default.TexturesPath + "\\venus.jpg";
            venusBrush.ImageSource =
                new BitmapImage(
                    new Uri(bPath, UriKind.RelativeOrAbsolute)
                );
            venus = new CelestialBody(61, new DiffuseMaterial(venusBrush), new Vector3D(0, -1, 0), 25.0);
            venus.Mass = 10000;
            venus.Name = "Venus";
            venus.CreateOrbit(sun, "Right", 2160);
            myUniverse.Add(venus);

            bPath = Gravity.Properties.Settings.Default.TexturesPath + "\\earth.jpg";
            earthBrush.ImageSource =
                new BitmapImage(
                    new Uri(bPath, UriKind.RelativeOrAbsolute)
                );
            earth = new CelestialBody(64, new DiffuseMaterial(earthBrush), new Vector3D(0, -1, 0), 25.0);
            earth.Mass = 10000;
            earth.Name = "Earth";
            earth.CreateOrbit(sun, "Right", 3000);
            myUniverse.Add(earth);

            bPath = Gravity.Properties.Settings.Default.TexturesPath + "\\mars.jpg";
            marsBrush.ImageSource =
                new BitmapImage(
                    new Uri(bPath, UriKind.RelativeOrAbsolute)
                );
            mars = new CelestialBody(34, new DiffuseMaterial(marsBrush), new Vector3D(0, -1, 0), 25.0);
            mars.Mass = 10000;
            mars.Name = "Mars";
            mars.CreateOrbit(sun, "Right", 4560);
            myUniverse.Add(mars);

            bPath = Gravity.Properties.Settings.Default.TexturesPath + "\\jupiter.jpg";
            jupiterBrush.ImageSource =
                new BitmapImage(
                    new Uri(bPath, UriKind.RelativeOrAbsolute)
                );
            jupiter = new CelestialBody(715, new DiffuseMaterial(jupiterBrush), new Vector3D(0, -1, 0), 25.0);
            jupiter.Mass = 10000;
            jupiter.Name = "Jupiter";
            jupiter.CreateOrbit(sun, "Right", 15560);
            myUniverse.Add(jupiter);

            bPath = Gravity.Properties.Settings.Default.TexturesPath + "\\saturn.jpg";
            saturnBrush.ImageSource =
                new BitmapImage(
                    new Uri(bPath, UriKind.RelativeOrAbsolute)
                );
            saturn = new CelestialBody(603, new DiffuseMaterial(saturnBrush), new Vector3D(0, -1, 0), 25.0);
            saturn.Mass = 10000;
            saturn.Name = "Saturn";
            saturn.CreateOrbit(sun, "Right", 28540);
            myUniverse.Add(saturn);

            bPath = Gravity.Properties.Settings.Default.TexturesPath + "\\uranus.jpg";
            uranusBrush.ImageSource =
                new BitmapImage(
                    new Uri(bPath, UriKind.RelativeOrAbsolute)
                );
            uranus = new CelestialBody(256, new DiffuseMaterial(uranusBrush), new Vector3D(0, -1, 0), 25.0);
            uranus.Mass = 10000;
            uranus.Name = "Uranus";
            uranus.CreateOrbit(sun, "Right", 57400);
            myUniverse.Add(uranus);

            bPath = Gravity.Properties.Settings.Default.TexturesPath + "\\neptune.jpg";
            neptuneBrush.ImageSource =
                new BitmapImage(
                    new Uri(bPath, UriKind.RelativeOrAbsolute)
                );
            neptune = new CelestialBody(248, new DiffuseMaterial(neptuneBrush), new Vector3D(0, -1, 0), 25.0);
            neptune.Mass = 10000;
            neptune.Name = "Neptune";
            neptune.CreateOrbit(sun, "Right", 89940);
            myUniverse.Add(neptune);

            bPath = Gravity.Properties.Settings.Default.TexturesPath + "\\pluto.jpg";
            plutoBrush.ImageSource =
                new BitmapImage(
                    new Uri(bPath, UriKind.RelativeOrAbsolute)
                );
            pluto = new CelestialBody(12, new DiffuseMaterial(plutoBrush), new Vector3D(0, -1, 0), 25.0);
            pluto.Mass = 10000;
            pluto.Name = "Pluto";
            pluto.CreateOrbit(sun, "Right", 117400);
            myUniverse.Add(pluto);

            CompositionTarget.Rendering += myUniverse.Run;

            BodyList.ItemsSource = myUniverse.Children;
        }
        
        #endregion

        #region Control Event Handling

        private void sldSpeed_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (myUniverse != null)
            {
                myUniverse.TimeMultiplier = sldSpeed.Value;
            }
        }

        private void btnPlay_Click(object sender, RoutedEventArgs e)
        {
            if (myUniverse != null)
            {
                if (myUniverse.isRunning)
                {
                    myUniverse.Pause();
                }
                else
                {
                    myUniverse.Start();
                }
            }
        }

        private void CBSelector_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            CBSelector cb = (CBSelector)sender;

            ListView.Visibility = Visibility.Collapsed;

            BodyView.Visibility = Visibility.Visible;
            BodyView.DataContext = cb.Tag;
            BodyView.Mode = "Edit";
        }

        private void CBView_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (BodyView.Mode == "Edit")
            {
                ListView.Visibility = Visibility.Visible;
                BodyView.Visibility = Visibility.Collapsed;
                BodyView.DataContext = null;
            }
        }

        private void CBView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (BodyView.Mode == "Edit")
            {
                ListView.Visibility = Visibility.Visible;
                BodyView.Visibility = Visibility.Collapsed;
                BodyView.DataContext = null;
            }
        }

        private void RemoveBody_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Grid cb = (Grid)sender;

            myUniverse.Remove((CelestialBody)cb.Tag);
        }

        private void btnAddBody_Click(object sender, RoutedEventArgs e)
        {
            ImageBrush questionBrush = new ImageBrush();
            questionBrush.ImageSource = 
                new BitmapImage(
                    new Uri(BaseUriHelper.GetBaseUri(this), @"/Resources/question.jpg")
                );

            CelestialBody newCB = new CelestialBody(100, new DiffuseMaterial(questionBrush), new Vector3D(0, -1, 0), 10.0);
            newCB.Name = "NoName";
            newCB.RotateTo(90);

            ListView.Visibility = Visibility.Collapsed;

            BodyView.Visibility = Visibility.Visible;
            BodyView.DataContext = newCB;
            BodyView.Mode = "New";
        }

        void BodyView_AddConfirmed(object sender, EventArgs e)
        {
            myUniverse.Add((CelestialBody)BodyView.Tag);

            ListView.Visibility = Visibility.Visible;

            BodyView.Visibility = Visibility.Collapsed;
            BodyView.DataContext = null;
        }

        void BodyView_AddCanceled(object sender, EventArgs e)
        {
            ListView.Visibility = Visibility.Visible;

            BodyView.Visibility = Visibility.Collapsed;
            BodyView.DataContext = null;
        }

        private void btnFrontView_Click(object sender, RoutedEventArgs e)
        {
            myUniverse.UniverseCamera.Position = new Point3D(0, 0, 8000);
            myUniverse.UniverseCamera.LookDirection = new Vector3D(0, 0, -1);
            myUniverse.UniverseCamera.UpDirection = new Vector3D(0, 1, 0);
        }

        private void btnTopView_Click(object sender, RoutedEventArgs e)
        {
            myUniverse.UniverseCamera.Position = new Point3D(0, 8000, 0);
            myUniverse.UniverseCamera.LookDirection = new Vector3D(0, -1, 0);
            myUniverse.UniverseCamera.UpDirection = new Vector3D(0, 0, -1);
        }
        
        private void btnDiagView_Click(object sender, RoutedEventArgs e)
        {
            myUniverse.UniverseCamera.Position = new Point3D(8000, 8000, 8000);
            myUniverse.UniverseCamera.LookDirection = new Vector3D(-1, -1, -1);
            myUniverse.UniverseCamera.UpDirection = new Vector3D(0, 1, 0);
        }

        private void btnZoomIn_Click(object sender, RoutedEventArgs e)
        {
            ZoomIn();
        }

        private void btnZoomOut_Click(object sender, RoutedEventArgs e)
        {
            ZoomOut();
        }

        private void universeViewport3D_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
            {
                ZoomIn();
            }
            if (e.Delta < 0)
            {
                ZoomOut();
            }
        }

        #endregion

        private void ZoomIn()
        {
            if ((myUniverse.UniverseCamera.Position - new Point3D(0, 0, 0)).Length > 500)
            {
                myUniverse.UniverseCamera.Position += (myUniverse.UniverseCamera.LookDirection * 500);
            }
        }

        private void ZoomOut()
        {
            myUniverse.UniverseCamera.Position -= (myUniverse.UniverseCamera.LookDirection * 500);
        }
    }
}
