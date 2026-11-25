using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;

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

        private Point viewMoveStartPoint;

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
            BodyView.Explode += BodyView_Explode;

            // TODO: Remove below section once testing done
            ImageBrush sunBrush = new ImageBrush();
            string sunPath = Gravity.Properties.Settings.Default.TexturesPath + "\\sun.jpg";
            sunBrush.ImageSource =
                new BitmapImage(
                    new Uri(sunPath, UriKind.RelativeOrAbsolute)
                );
            testPlanet = new SphericalCelestialBody(150, new DiffuseMaterial(sunBrush), new Vector3D(0, 1, 0), 10.0)
            {
                Mass = 1000000000,
                Name = "Sun"
            };
            myUniverse.Add(testPlanet);

            SpaceshipCelestialBody spaceship = new SpaceshipCelestialBody(10, new DiffuseMaterial(new LinearGradientBrush(Colors.Red, Colors.Gray, 45))) { Name = "SpaceShip", Location = new Point3D(4000, 0, 0), Mass = 50, ThrustPercent = 0.35, Size = 100 };

            myUniverse.Add(spaceship);
            myUniverse.ControlledObject = spaceship;
            // END remove section

            CompositionTarget.Rendering += myUniverse.Run;

            BodyList.ItemsSource = myUniverse.Children;
            rdoCamFree.IsChecked = true;
        }

        #endregion

        #region Control Event Handling

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch(e.Key)
            {
                case Key.Up:
                    myUniverse.ControlledObject.Pitch(5);
                    break;
                case Key.Down:
                    myUniverse.ControlledObject.Pitch(-5);
                    break;
                case Key.Left:
                    if (Keyboard.IsKeyDown(Key.LeftCtrl))
                        myUniverse.ControlledObject.Roll(-5);
                    else
                        myUniverse.ControlledObject.Yaw(-5);
                    break;
                case Key.Right:
                    if (Keyboard.IsKeyDown(Key.LeftCtrl))
                        myUniverse.ControlledObject.Roll(5);
                    else
                        myUniverse.ControlledObject.Yaw(5);
                    break;
                case Key.NumPad0:
                    myUniverse.ControlledObject.ThrustPercent = 0;
                    break;
                case Key.PageUp:
                    if ((myUniverse.ControlledObject.ThrustPercent + .05) >= 1)
                        myUniverse.ControlledObject.ThrustPercent = 1;
                    else
                        myUniverse.ControlledObject.ThrustPercent += 0.05;
                    break;
                case Key.PageDown:
                    if ((myUniverse.ControlledObject.ThrustPercent - .05) <= 0)
                        myUniverse.ControlledObject.ThrustPercent = 0;
                    else
                        myUniverse.ControlledObject.ThrustPercent -= 0.05;
                    break;
            }
        }

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

        private void CBSelector_MouseEnter(object sender, MouseEventArgs e)
        {
            CBSelector cb = (CBSelector)sender;

            ((CelestialBody)cb.Tag).Highlight();
        }

        private void CBSelector_MouseLeave(object sender, MouseEventArgs e)
        {
            CBSelector cb = (CBSelector)sender;

            ((CelestialBody)cb.Tag).Unhighlight();
        }

        private void btnAddBody_Click(object sender, RoutedEventArgs e)
        {
            ImageBrush questionBrush = new ImageBrush()
            {
                ImageSource =
                new BitmapImage(
                    new Uri(BaseUriHelper.GetBaseUri(this), @"/Resources/question.jpg")
                )
            };
            CelestialBody newCB = new SphericalCelestialBody(100, new DiffuseMaterial(questionBrush), new Vector3D(0, 1, 0), 10.0)
            {
                Name = "NoName"
            };
            newCB.RotateTo(90);

            ListView.Visibility = Visibility.Collapsed;

            BodyView.Visibility = Visibility.Visible;
            BodyView.DataContext = newCB;
            BodyView.Mode = "New";
        }

        private void rdoCamFree_Checked(object sender, RoutedEventArgs e)
        {
            cbCameraCelestialBody.Visibility = Visibility.Collapsed;

            myUniverse.CameraMode = "Free";
        }

        private void rdoCamFollow_Checked(object sender, RoutedEventArgs e)
        {
            cbCameraCelestialBody.Visibility = Visibility.Visible;

            cbCameraCelestialBody.ItemsSource = null;
            cbCameraCelestialBody.ItemsSource = myUniverse.Children;

            myUniverse.CameraMode = "Follow";
        }

        private void rdoCamRide_Checked(object sender, RoutedEventArgs e)
        {
            cbCameraCelestialBody.Visibility = Visibility.Visible;

            cbCameraCelestialBody.ItemsSource = null;
            cbCameraCelestialBody.ItemsSource = myUniverse.Children;

            myUniverse.CameraMode = "Ride";
        }

        private void cbCameraCelestialBody_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            myUniverse.CameraAttachedObject = (CelestialBody)((ComboBox)sender).SelectedItem;


            if (myUniverse.CameraAttachedObject != null)
            {
                switch (myUniverse.CameraMode)
                {
                    case "Free":
                        break;
                    case "Follow":
                        myUniverse.UniverseCamera.Position = myUniverse.CameraAttachedObject.InitialFollowCameraPosition;
                        myUniverse.UniverseCamera.UpDirection = myUniverse.CameraAttachedObject.InitialFollowCameraUpDirection;
                        myUniverse.UniverseCamera.LookDirection = myUniverse.CameraAttachedObject.InitialFollowCameraLookDirection;
                        break;
                    case "Ride":
                        myUniverse.UniverseCamera.Position = myUniverse.CameraAttachedObject.InitialRideCameraPosition;
                        myUniverse.UniverseCamera.UpDirection = myUniverse.CameraAttachedObject.InitialRideCameraUpDirection;
                        myUniverse.UniverseCamera.LookDirection = myUniverse.CameraAttachedObject.InitialRideCameraLookDirection;
                        break;
                }
            }
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

        void BodyView_Explode(object sender, EventArgs e)
        {
            myUniverse.Explode((CelestialBody)BodyView.Tag);

            ListView.Visibility = Visibility.Visible;

            BodyView.Visibility = Visibility.Collapsed;
            BodyView.DataContext = null;
        }

        private void btnSS_Click(object sender, RoutedEventArgs e)
        {
            CelestialBody sun, mercury, venus, earth, mars, jupiter, saturn, uranus, neptune, pluto;
            ImageBrush sunBrush = new ImageBrush(), mercuryBrush = new ImageBrush(), venusBrush = new ImageBrush(), earthBrush = new ImageBrush(), marsBrush = new ImageBrush(), jupiterBrush = new ImageBrush(),
                        saturnBrush = new ImageBrush(), uranusBrush = new ImageBrush(), neptuneBrush = new ImageBrush(), plutoBrush = new ImageBrush();

            string bPath = Gravity.Properties.Settings.Default.TexturesPath + "\\sun.jpg";
            sunBrush.ImageSource =
                new BitmapImage(
                    new Uri(bPath, UriKind.RelativeOrAbsolute)
                );
            sun = new SphericalCelestialBody(696, new DiffuseMaterial(sunBrush), new Vector3D(0, -1, 0), 25.0);
            sun.Mass = 2000000000;
            sun.Name = "Sun";
            myUniverse.Add(sun);

            bPath = Gravity.Properties.Settings.Default.TexturesPath + "\\mercury.jpg";
            mercuryBrush.ImageSource =
                new BitmapImage(
                    new Uri(bPath, UriKind.RelativeOrAbsolute)
                );
            mercury = new SphericalCelestialBody(24, new DiffuseMaterial(mercuryBrush), new Vector3D(0, -1, 0), 25.0);
            mercury.Mass = 100000;
            mercury.Name = "Mercury";
            mercury.CreateOrbit(sun, "Right", 1160);
            myUniverse.Add(mercury);

            bPath = Gravity.Properties.Settings.Default.TexturesPath + "\\venus.jpg";
            venusBrush.ImageSource =
                new BitmapImage(
                    new Uri(bPath, UriKind.RelativeOrAbsolute)
                );
            venus = new SphericalCelestialBody(61, new DiffuseMaterial(venusBrush), new Vector3D(0, -1, 0), 25.0);
            venus.Mass = 100000;
            venus.Name = "Venus";
            venus.CreateOrbit(sun, "Right", 2160);
            myUniverse.Add(venus);

            bPath = Gravity.Properties.Settings.Default.TexturesPath + "\\earth.jpg";
            earthBrush.ImageSource =
                new BitmapImage(
                    new Uri(bPath, UriKind.RelativeOrAbsolute)
                );
            earth = new SphericalCelestialBody(64, new DiffuseMaterial(earthBrush), new Vector3D(0, -1, 0), 25.0);
            earth.Mass = 100000;
            earth.Name = "Earth";
            earth.CreateOrbit(sun, "Right", 3000);
            myUniverse.Add(earth);

            bPath = Gravity.Properties.Settings.Default.TexturesPath + "\\mars.jpg";
            marsBrush.ImageSource =
                new BitmapImage(
                    new Uri(bPath, UriKind.RelativeOrAbsolute)
                );
            mars = new SphericalCelestialBody(34, new DiffuseMaterial(marsBrush), new Vector3D(0, -1, 0), 25.0);
            mars.Mass = 100000;
            mars.Name = "Mars";
            mars.CreateOrbit(sun, "Right", 4560);
            myUniverse.Add(mars);

            bPath = Gravity.Properties.Settings.Default.TexturesPath + "\\jupiter.jpg";
            jupiterBrush.ImageSource =
                new BitmapImage(
                    new Uri(bPath, UriKind.RelativeOrAbsolute)
                );
            jupiter = new SphericalCelestialBody(715, new DiffuseMaterial(jupiterBrush), new Vector3D(0, -1, 0), 25.0);
            jupiter.Mass = 100000;
            jupiter.Name = "Jupiter";
            jupiter.CreateOrbit(sun, "Right", 15560);
            myUniverse.Add(jupiter);

            bPath = Gravity.Properties.Settings.Default.TexturesPath + "\\saturn.jpg";
            saturnBrush.ImageSource =
                new BitmapImage(
                    new Uri(bPath, UriKind.RelativeOrAbsolute)
                );
            saturn = new SphericalRingedCelestialBody(603, new DiffuseMaterial(saturnBrush), new Vector3D(0, -1, 0), 25.0);
            saturn.Mass = 100000;
            saturn.Name = "Saturn";
            saturn.CreateOrbit(sun, "Right", 28540);
            myUniverse.Add(saturn);

            bPath = Gravity.Properties.Settings.Default.TexturesPath + "\\uranus.jpg";
            uranusBrush.ImageSource =
                new BitmapImage(
                    new Uri(bPath, UriKind.RelativeOrAbsolute)
                );
            uranus = new SphericalCelestialBody(256, new DiffuseMaterial(uranusBrush), new Vector3D(0, -1, 0), 25.0);
            uranus.Mass = 100000;
            uranus.Name = "Uranus";
            uranus.CreateOrbit(sun, "Right", 57400);
            myUniverse.Add(uranus);

            bPath = Gravity.Properties.Settings.Default.TexturesPath + "\\neptune.jpg";
            neptuneBrush.ImageSource =
                new BitmapImage(
                    new Uri(bPath, UriKind.RelativeOrAbsolute)
                );
            neptune = new SphericalCelestialBody(248, new DiffuseMaterial(neptuneBrush), new Vector3D(0, -1, 0), 25.0);
            neptune.Mass = 100000;
            neptune.Name = "Neptune";
            neptune.CreateOrbit(sun, "Right", 89940);
            myUniverse.Add(neptune);

            bPath = Gravity.Properties.Settings.Default.TexturesPath + "\\pluto.jpg";
            plutoBrush.ImageSource =
                new BitmapImage(
                    new Uri(bPath, UriKind.RelativeOrAbsolute)
                );
            pluto = new SphericalCelestialBody(12, new DiffuseMaterial(plutoBrush), new Vector3D(0, -1, 0), 25.0);
            pluto.Mass = 100000;
            pluto.Name = "Pluto";
            pluto.CreateOrbit(sun, "Right", 117400);
            myUniverse.Add(pluto);
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

            Vector3D newLookDirection = (new Point3D(0, 0, 0) - (myUniverse.UniverseCamera.Position));
            newLookDirection.Normalize();
            myUniverse.UniverseCamera.LookDirection = newLookDirection; //new Vector3D(-1, -1, -1);

            Vector3D newUpDirection = new Vector3D(-0.5, 1, -0.5);
            newUpDirection.Normalize();
            myUniverse.UniverseCamera.UpDirection = newUpDirection; //new Vector3D(-40.825, 81.65, -40.825);
        }

        private void universeViewport3D_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            myUniverse.UniverseCamera.Position += (myUniverse.UniverseCamera.LookDirection * e.Delta);
        }

        private void universeViewport3d_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.MiddleButton == MouseButtonState.Pressed)
            {
                UIElement el = (UIElement)sender;
                viewMoveStartPoint = e.GetPosition(el);

                el.CaptureMouse();
            }
        }

        private void universeViewport3d_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.MiddleButton == MouseButtonState.Released)
            {
                UIElement el = (UIElement)sender;
                el.ReleaseMouseCapture();
            }
        }

        private void universeViewport3d_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.MiddleButton == MouseButtonState.Pressed)
            {
                UIElement el = (UIElement)sender;

                Vector moveVector = e.GetPosition(el) - viewMoveStartPoint;

                RotateTransform3D rotateYawTransform;
                RotateTransform3D rotatePitchTransform;

                if (Keyboard.IsKeyDown(Key.LeftShift))
                {
                    myUniverse.UniverseCamera.Position -= (Vector3D.CrossProduct(myUniverse.UniverseCamera.UpDirection, myUniverse.UniverseCamera.LookDirection) * moveVector.X);
                    myUniverse.UniverseCamera.Position -= (myUniverse.UniverseCamera.UpDirection * moveVector.Y);
                }
                else
                {
                    if (myUniverse.CameraMode == "Free")
                    {
                        rotateYawTransform = new RotateTransform3D(new AxisAngleRotation3D(myUniverse.UniverseCamera.UpDirection, -moveVector.X % 360));
                        rotatePitchTransform = new RotateTransform3D(new AxisAngleRotation3D(Vector3D.CrossProduct(myUniverse.UniverseCamera.UpDirection, myUniverse.UniverseCamera.LookDirection), moveVector.Y % 360));
                    }
                    else
                    {
                        rotateYawTransform = new RotateTransform3D(new AxisAngleRotation3D(myUniverse.UniverseCamera.UpDirection, -moveVector.X % 360), myUniverse.CameraAttachedObject.Location);
                        rotatePitchTransform = new RotateTransform3D(new AxisAngleRotation3D(Vector3D.CrossProduct(myUniverse.UniverseCamera.UpDirection, myUniverse.UniverseCamera.LookDirection), moveVector.Y % 360), myUniverse.CameraAttachedObject.Location);
                    }

                    Transform3DGroup rotateTransformGroup = new Transform3DGroup();
                    rotateTransformGroup.Children.Add(rotatePitchTransform);
                    rotateTransformGroup.Children.Add(rotateYawTransform);

                    myUniverse.UniverseCamera.UpDirection = rotateTransformGroup.Transform(myUniverse.UniverseCamera.UpDirection);
                    myUniverse.UniverseCamera.LookDirection = rotateTransformGroup.Transform(myUniverse.UniverseCamera.LookDirection);

                    if (!Keyboard.IsKeyDown(Key.LeftCtrl))
                        myUniverse.UniverseCamera.Position = rotateTransformGroup.Transform(myUniverse.UniverseCamera.Position);
                }

                viewMoveStartPoint = e.GetPosition(el);
            }
        }

        #endregion
    }
}
