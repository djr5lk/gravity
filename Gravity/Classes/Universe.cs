using System;
using System.Collections.ObjectModel;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Gravity
{
    public class Universe
    {
        #region Properties

        //public string[,] logData = new string[14,100];
        //public int logCounter;
        //public StreamWriter logFile = new StreamWriter("C:\\temp\\Gravity.log",false);

        public TimeSpan DeltaTime = new TimeSpan(0, 0, 0);
        public DateTime LastTime = DateTime.Now;
        public bool isRunning = true;
        public double TimeMultiplier = 1.0;

        public ModelVisual3D UniverseModelVisual3D { get; set; }
        public Model3DGroup UniverseModel3DGroup { get; set; }
        public PerspectiveCamera UniverseCamera { get; set; }

        public string CameraMode = "Free";
        public CelestialBody CameraAttachedObject { get; set; }
        public ControllableCelestialBody ControlledObject { get; set; }

        public ObservableCollection<CelestialBody> Children { get; set; }

        #endregion Properties

        #region Constructors

        public Universe()
        {
            /*
             * Create collection to hold all objects in the universe
             */
            Children = new ObservableCollection<CelestialBody>();

            /*
             * Create objects for properties that hold overall 3D model definition for universe
             */
            UniverseModelVisual3D = new ModelVisual3D();
            UniverseModel3DGroup = new Model3DGroup();

            /*
             * Create perspective camera to allow the universe to be seen 
             */
            UniverseCamera = new PerspectiveCamera(new Point3D(0, 0, 8000), new Vector3D(0, 0, -1), new Vector3D(0, 1, 0), 90);

            /*
             * Let there be light!  Without some form of light no universe objects will be visible - currently using a directional light, but
             * may decide to change this to a point light in the future (new PointLight(Colors.Yellow, new Point3D(0, 0, 0)))
             */
            //UniverseModel3DGroup.Children.Add(new DirectionalLight(Colors.White, new Vector3D(-0.5, 0, -1)));
            UniverseModel3DGroup.Children.Add(new DirectionalLight(Colors.White, new Vector3D(0, 0, -1)));
            UniverseModel3DGroup.Children.Add(new DirectionalLight(Colors.White, new Vector3D(0, 0, 1)));
            UniverseModel3DGroup.Children.Add(new DirectionalLight(Colors.White, new Vector3D(1, 0, 0)));
            UniverseModel3DGroup.Children.Add(new DirectionalLight(Colors.White, new Vector3D(-1, 0, 0)));
            UniverseModel3DGroup.Children.Add(new DirectionalLight(Colors.White, new Vector3D(0, -1, 0)));
            UniverseModel3DGroup.Children.Add(new DirectionalLight(Colors.White, new Vector3D(0, 1, 0)));

            UniverseModelVisual3D.Content = UniverseModel3DGroup;
        }

        #endregion

        public void Add(CelestialBody cb)
        {
            /*
             * Adds a CelestialBody to the Universe by first adding the model to the Model3DGroup, 
             * and then adding the object itself to the internal list of objects in the Universe
             */
            UniverseModel3DGroup.Children.Add(cb.Model);
            this.Children.Add(cb);
        }

        public void Remove(CelestialBody cb)
        {
            /*
             * Removes a CelestialBody from the Universe by first removing the model from the Model3DGroup, 
             * and then removing the object itself from the internal list of objects in the Universe
             */
            UniverseModel3DGroup.Children.Remove(cb.Model);
            this.Children.Remove(cb);
        }

        public void Explode(CelestialBody cb)
        {
            Collection<CelestialBody> pieces;

            pieces = cb.Explode(DeltaTime, TimeMultiplier);

            foreach(CelestialBody piece in pieces)
            {
                Add(piece);
            }
            
            Remove(cb);
        }

        public void Run(Object sender, EventArgs e)
        {
            /*********************************************************************************************************
             * This method is called everytime the window renders, and does the main work of running the Universe
            *********************************************************************************************************/
            if (isRunning)
            {
                /*
                 * Determine the change in time from the last time movement calculations were made
                 */
                DateTime now = DateTime.Now;
                DeltaTime = now - LastTime;
                LastTime = DateTime.Now;
                bool accelerated;

                /*
                 * Cycle through each CelestialBody in the Universe, calculating and applying gravitational acceleration from every OTHER CelestialBody
                 */
                foreach (CelestialBody cbTarget in this.Children)
                {
                    accelerated = false;          // For each CelestialBody, need to know whether it was accelerated by any other CelestialBodies

                    foreach (CelestialBody cbSource in this.Children)
                    {
                        if (cbSource != cbTarget)
                        {
                            cbTarget.ApplyGravity(cbSource, DeltaTime, TimeMultiplier);
                            accelerated = true;
                        }
                    }

                    if (!cbTarget.VelocityInitialized && accelerated)   // Only mark velocity as initialized if the CelestialBody was accelerated by at least one other
                    {
                        cbTarget.VelocityInitialized = true;
                    }
                }

                /*
                 * Now that acceleration has been applied, instruct each CelestialBody to move by the newly modified velocity, as well as to rotate if
                 * set up with a RotationAngle / RotationAxis
                 */
                foreach (CelestialBody cbMove in this.Children)
                {
                    cbMove.Move(DeltaTime, TimeMultiplier);
                    cbMove.Rotate(DeltaTime, TimeMultiplier);
                }

                /*
                 * After all movements are applied, move the camera if it is attached to an object
                 */
                if (CameraMode == "Follow" && CameraAttachedObject != null)
                {
                    UniverseCamera.Position += (CameraAttachedObject.Velocity * (DeltaTime.TotalSeconds * TimeMultiplier));
                }
                if (CameraMode == "Ride" && CameraAttachedObject != null)
                {
                    UniverseCamera.Position += (CameraAttachedObject.Velocity * (DeltaTime.TotalSeconds * TimeMultiplier));

                    RotateTransform3D rotateTransform = new RotateTransform3D(new AxisAngleRotation3D(CameraAttachedObject.RotationAxis, (CameraAttachedObject.RotationSpeed * (DeltaTime.TotalSeconds * TimeMultiplier))), CameraAttachedObject.Location);

                    UniverseCamera.Position = rotateTransform.Transform(UniverseCamera.Position);
                    UniverseCamera.UpDirection = rotateTransform.Transform(UniverseCamera.UpDirection);
                    UniverseCamera.LookDirection = rotateTransform.Transform(UniverseCamera.LookDirection);
                }

                //logData[0, 0] = "DeltaTime";
                //logData[0, 1] = DeltaTime.ToString();
                //logData[1, 0] = "TimeMultiplier";
                //logData[1, 1] = TimeMultiplier.ToString();
                //logData[2, 0] = "name";
                //logData[3, 0] = "radius";
                //logData[4, 0] = "mass";
                //logData[5, 0] = "locationX";
                //logData[6, 0] = "locationY";
                //logData[7, 0] = "locationZ";
                //logData[8, 0] = "velX";
                //logData[9, 0] = "velY";
                //logData[10, 0] = "velZ";
                //logData[11, 0] = "rotationAngle";
                //logData[12, 0] = "rotationSpeed";
                //logData[13, 0] = "velocityInitialized";
                //logCounter = 0;
                //foreach (CelestialBody cbData in this.Children)
                //{
                //    logCounter++;
                //    logData[2, logCounter] = cbData.Name;
                //    logData[3, logCounter] = cbData.Radius.ToString();
                //    logData[4, logCounter] = cbData.Mass.ToString();
                //    logData[5, logCounter] = cbData.X.ToString();
                //    logData[6, logCounter] = cbData.Y.ToString();
                //    logData[7, logCounter] = cbData.Z.ToString();
                //    logData[8, logCounter] = cbData.XVel.ToString();
                //    logData[9, logCounter] = cbData.YVel.ToString();
                //    logData[10, logCounter] = cbData.ZVel.ToString();
                //    logData[11, logCounter] = cbData.RotationAngle.ToString();
                //    logData[12, logCounter] = cbData.RotationSpeed.ToString();
                //    logData[13, logCounter] = cbData.VelocityInitialized.ToString();
                //}
                //logFile.WriteLine("--------------------------------");
                //for (int x=0;x<=13;x++)
                //{
                //    for (int y=0;y<=this.Children.Count + 1;y++)
                //    {
                //        logFile.Write($"{logData[x, y]}\t");
                //    }
                //    logFile.Write("\r\n");
                //}
            }
        }

        public void Start()
        {
            //* Because we are "restarting time", we want all CelestialBodies to reinitialize their velocity
            foreach (CelestialBody cb in this.Children)
            {
                cb.VelocityInitialized = false;
            }

            LastTime = DateTime.Now;
            isRunning = true;
        }

        public void Pause()
        {
            isRunning = false;
        }
    }
}
