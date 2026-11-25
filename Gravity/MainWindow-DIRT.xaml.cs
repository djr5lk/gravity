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

            CelestialBody john, alex, david, jacob, matt, nate, paul, ramya, carl, paulb;
            ImageBrush johnBrush = new ImageBrush(), alexBrush = new ImageBrush(), davidBrush = new ImageBrush(), jacobBrush = new ImageBrush(), mattBrush = new ImageBrush(), nateBrush = new ImageBrush(),
                        paulBrush = new ImageBrush(), ramyaBrush = new ImageBrush(), carlBrush = new ImageBrush(), paulbBrush = new ImageBrush();
            
            string bPath = Gravity.Properties.Settings.Default.TexturesPath + "\\JohnG.jpg";
            johnBrush.ImageSource =
                new BitmapImage(
                    new Uri(bPath, UriKind.RelativeOrAbsolute)
                );
            john = new CelestialBody(300, new DiffuseMaterial(johnBrush), new Vector3D(0, -1, 0), 50.0);
            john.Mass = 2000000000;
            john.Name = "John";
            myUniverse.Add(john);

            bPath = Gravity.Properties.Settings.Default.TexturesPath + "\\AlexB.jpg";
            alexBrush.ImageSource =
                new BitmapImage(
                    new Uri(bPath, UriKind.RelativeOrAbsolute)
                );
            alex = new CelestialBody(100, new DiffuseMaterial(alexBrush), new Vector3D(0, -1, 0), 50.0);
            alex.Mass = 10000;
            alex.Name = "Alex";
            alex.CreateOrbit(john, "Right", 1000);
            myUniverse.Add(alex);

            bPath = Gravity.Properties.Settings.Default.TexturesPath + "\\DavidR.jpg";
            davidBrush.ImageSource =
                new BitmapImage(
                    new Uri(bPath, UriKind.RelativeOrAbsolute)
                );
            david = new CelestialBody(100, new DiffuseMaterial(davidBrush), new Vector3D(0, -1, 0), 50.0);
            david.Mass = 10000;
            david.Name = "David";
            david.CreateOrbit(john, "UpRight", 1500);
            myUniverse.Add(david);

            bPath = Gravity.Properties.Settings.Default.TexturesPath + "\\JacobF.jpg";
            jacobBrush.ImageSource =
                new BitmapImage(
                    new Uri(bPath, UriKind.RelativeOrAbsolute)
                );
            jacob = new CelestialBody(100, new DiffuseMaterial(jacobBrush), new Vector3D(0, -1, 0), 50.0);
            jacob.Mass = 10000;
            jacob.Name = "Jacob";
            jacob.CreateOrbit(john, "Up", 2000);
            myUniverse.Add(jacob);

            bPath = Gravity.Properties.Settings.Default.TexturesPath + "\\MattK.jpg";
            mattBrush.ImageSource =
                new BitmapImage(
                    new Uri(bPath, UriKind.RelativeOrAbsolute)
                );
            matt = new CelestialBody(100, new DiffuseMaterial(mattBrush), new Vector3D(0, -1, 0), 50.0);
            matt.Mass = 10000;
            matt.Name = "Matt";
            matt.CreateOrbit(john, "UpLeft", 2500);
            myUniverse.Add(matt);

            bPath = Gravity.Properties.Settings.Default.TexturesPath + "\\NateP.jpg";
            nateBrush.ImageSource =
                new BitmapImage(
                    new Uri(bPath, UriKind.RelativeOrAbsolute)
                );
            nate = new CelestialBody(100, new DiffuseMaterial(nateBrush), new Vector3D(0, -1, 0), 50.0);
            nate.Mass = 10000;
            nate.Name = "Nate";
            nate.CreateOrbit(john, "Left", 3000);
            myUniverse.Add(nate);

            bPath = Gravity.Properties.Settings.Default.TexturesPath + "\\PaulT.jpg";
            paulBrush.ImageSource =
                new BitmapImage(
                    new Uri(bPath, UriKind.RelativeOrAbsolute)
                );
            paul = new CelestialBody(100, new DiffuseMaterial(paulBrush), new Vector3D(0, -1, 0), 50.0);
            paul.Mass = 10000;
            paul.Name = "PaulT";
            paul.CreateOrbit(john, "DownLeft", 3500);
            myUniverse.Add(paul);

            bPath = Gravity.Properties.Settings.Default.TexturesPath + "\\RamyaS.jpg";
            ramyaBrush.ImageSource =
                new BitmapImage(
                    new Uri(bPath, UriKind.RelativeOrAbsolute)
                );
            ramya = new CelestialBody(100, new DiffuseMaterial(ramyaBrush), new Vector3D(0, -1, 0), 50.0);
            ramya.Mass = 10000;
            ramya.Name = "Ramya";
            ramya.CreateOrbit(john, "Down", 4000);
            myUniverse.Add(ramya);

            bPath = Gravity.Properties.Settings.Default.TexturesPath + "\\CarlK.jpg";
            carlBrush.ImageSource =
                new BitmapImage(
                    new Uri(bPath, UriKind.RelativeOrAbsolute)
                );
            carl = new CelestialBody(100, new DiffuseMaterial(carlBrush), new Vector3D(0, -1, 0), 50.0);
            carl.Mass = 10000;
            carl.Name = "Carl";
            carl.CreateOrbit(john, "DownRight", 4500);
            myUniverse.Add(carl);

            bPath = Gravity.Properties.Settings.Default.TexturesPath + "\\PaulB.jpg";
            paulbBrush.ImageSource =
                new BitmapImage(
                    new Uri(bPath, UriKind.RelativeOrAbsolute)
                );
            paulb = new CelestialBody(100, new DiffuseMaterial(paulbBrush), new Vector3D(0, -1, 0), 50.0);
            paulb.Mass = 10000;
            paulb.Name = "PaulB";
            paulb.CreateOrbit(john, "Right", 5000);
            myUniverse.Add(paulb);

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
