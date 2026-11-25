using System;
using System.ComponentModel;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Gravity
{
    public class CelestialBody : INotifyPropertyChanged
    {
        #region Properties
        // Properties to enable 3D representation
        private Sphere3D geometry;
        private Material material;
        private MeshGeometry3D mesh;
        private Transform3DGroup transformGroup;
        private TranslateTransform3D locationTransform;
        private RotateTransform3D rotationTransform;
        private AxisAngleRotation3D rotation3D;
        private bool modelInitialized = false;

        // Movement properties
        private Point3D location = new Point3D(0, 0, 0);
        private Vector3D velocity = new Vector3D(0, 0, 0);
        private Vector3D rotationAxis = new Vector3D(0, 1, 0);
        private double rotationAngle = 0.0;
        private double rotationSpeed = 0.0;

        // Generic properties
        private string name;
        private int radius = 10;
        private double mass = 1000;

        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;

                OnPropertyChanged("Name");
            }
        }

        public int Radius
        {
            get
            {
                return radius;
            }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException("Radius", "Radius must be positive");
                }

                radius = value;

                if (modelInitialized)
                {
                    RefreshGeometry();  //  If radius is changed need to create new model
                }

                OnPropertyChanged("Radius");
                OnPropertyChanged("ViewCameraPosition");
            }
        }

        public Point3D ViewCameraPosition
        {
            get
            {
                return new Point3D(0, 0, (double)Radius * 3.0);       // Creates unique camera position for CBSelector/CBView objects so that whole body can be seen
            }
        }

        public double Mass
        {
            get
            {
                return mass;
            }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException("Mass", "Mass must be positive");
                }

                mass = value;

                OnPropertyChanged("Mass");
            }
        }

        public GeometryModel3D Model { get; private set; }

        public GeometryModel3D ModelRotateOnly
        {
            get
            {
                GeometryModel3D tempModel = new GeometryModel3D(mesh, material);
                tempModel.Transform = rotationTransform;

                return tempModel;
            }
        }

        public Material Material
        {
            get
            {
                return material;
            }
            set
            {
                material = value;
                Model.Material = material;

                OnPropertyChanged("Material");
                OnPropertyChanged("Model");
                OnPropertyChanged("ModelRotateOnly");
            }
        }

        public Point3D Location
        {
            get
            {
                return location;
            }
            set
            {
                location = value;
                locationTransform.OffsetX = value.X;
                locationTransform.OffsetY = value.Y;
                locationTransform.OffsetZ = value.Z;

                OnPropertyChanged("Location");
                OnPropertyChanged("X");
                OnPropertyChanged("Y");
                OnPropertyChanged("Z");
            }
        }

        // Need to create separate Properties for location components so that binding will work properly

        public double X
        {
            get
            {
                return Location.X;
            }
            set
            {
                Location = new Point3D(value, Location.Y, Location.Z);
            }
        }

        public double Y
        {
            get
            {
                return Location.Y;
            }
            set
            {
                Location = new Point3D(Location.X, value, Location.Z);
            }
        }

        public double Z
        {
            get
            {
                return Location.Z;
            }
            set
            {
                Location = new Point3D(Location.X, Location.Y, value);
            }
        }

        public Vector3D Velocity
        {
            get
            {
                return velocity;
            }
            set
            {
                velocity = value;

                OnPropertyChanged("Velocity");
                OnPropertyChanged("XVel");
                OnPropertyChanged("YVel");
                OnPropertyChanged("ZVel");
            }
        }

        // Need to create separate Properties for velocity components so that binding will work properly

        public double XVel
        {
            get
            {
                return Velocity.X;
            }
            set
            {
                Velocity = new Vector3D(value, Velocity.Y, Velocity.Z);
            }
        }

        public double YVel
        {
            get
            {
                return Velocity.Y;
            }
            set
            {
                Velocity = new Vector3D(Velocity.X, value, Velocity.Z);
            }
        }

        public double ZVel
        {
            get
            {
                return Velocity.Z;
            }
            set
            {
                Velocity = new Vector3D(Velocity.X, Velocity.Y, value);
            }
        }

        public bool VelocityInitialized { get; set; } = false;

        public Vector3D RotationAxis
        {
            get
            {
                return rotationAxis;
            }
            set
            {
                rotationAxis = value;
                rotation3D.Axis = value;

                OnPropertyChanged("RotationAxis");
            }
        }

        public double RotationAngle
        {
            get
            {
                return rotationAngle;
            }
            set
            {
                if ((value <= -360.0) || (value >= 360.0))
                {
                    throw new ArgumentOutOfRangeException("RotationAngle", "Rotation Angle must be > -360 and < 360");
                }
                rotationAngle = value;
                rotation3D.Angle = value;

                OnPropertyChanged("RotationAngle");
            }
        }

        public double RotationSpeed
        {
            get
            {
                return rotationSpeed;
            }
            set
            {
                rotationSpeed = value;

                OnPropertyChanged("RotationSpeed");
            }
        }

        #endregion

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        #endregion

        #region Constructors

        public CelestialBody()
        {
            // Create default material
            material = new DiffuseMaterial(new SolidColorBrush(Colors.Blue));

            // Initialize 3D components
            Initialize3DComponents();
        }

        public CelestialBody(int celestialBodyRadius)
        {
            // Create default material
            material = new DiffuseMaterial(new SolidColorBrush(Colors.Blue));

            Radius = celestialBodyRadius;

            // Initialize 3D components
            Initialize3DComponents();
        }

        public CelestialBody(int celestialBodyRadius, Vector3D celestialBodyRotationAxis, double celestialBodyRotationSpeed)
        {
            // Create default material
            material = new DiffuseMaterial(new SolidColorBrush(Colors.Blue));

            Radius = celestialBodyRadius;
            rotationAxis = celestialBodyRotationAxis;
            rotationSpeed = celestialBodyRotationSpeed;

            // Initialize 3D components
            Initialize3DComponents();
        }

        public CelestialBody(int celestialBodyRadius, Material celestialBodyMaterial)
        {
            Radius = celestialBodyRadius;
            material = celestialBodyMaterial;

            // Initialize 3D components
            Initialize3DComponents();
        }

        public CelestialBody(int celestialBodyRadius, Material celestialBodyMaterial, Vector3D celestialBodyRotationAxis, double celestialBodyRotationSpeed)
        {
            Radius = celestialBodyRadius;
            material = celestialBodyMaterial;
            rotationAxis = celestialBodyRotationAxis;
            rotationSpeed = celestialBodyRotationSpeed;

            // Initialize 3D components
            Initialize3DComponents();
        }

        #endregion

        public override string ToString()
        {
            return this.Name;
        }

        private void Initialize3DComponents()
        {
            // Create sphere geometry for planet based on radius
            geometry = new Sphere3D(Radius);

            // Assign geometry to a mesh object that can be added into the 3D model
            mesh = new MeshGeometry3D
            {
                Positions = geometry.Points,
                TriangleIndices = geometry.TriangleIndices,
                TextureCoordinates = geometry.TextureCoordinates
            };

            // Create model using assigned mesh and material
            Model = new GeometryModel3D(mesh, material)
            {
                BackMaterial = material
            };

            // Create transforms to show model in proper location and with proper rotation
            locationTransform = new TranslateTransform3D((Vector3D)location);
            rotation3D = new AxisAngleRotation3D(rotationAxis, rotationAngle);
            rotationTransform = new RotateTransform3D(rotation3D, location);
            transformGroup = new Transform3DGroup();
            transformGroup.Children.Add(rotationTransform);
            transformGroup.Children.Add(locationTransform);
            Model.Transform = transformGroup;

            // Set flag indicating that the 3D model has been initialized
            modelInitialized = true;
        }

        private void RefreshGeometry()
        {
            // Create sphere geometry for planet based on radius
            geometry = new Sphere3D(Radius);

            // Assign geometry to a mesh object that can be added into the 3D model
            mesh.Positions = geometry.Points;
            mesh.TriangleIndices = geometry.TriangleIndices;
            mesh.TextureCoordinates = geometry.TextureCoordinates;

            Model.Geometry = mesh;
        }

        public void Move(TimeSpan deltaTime, double timeMultiplier)
        {
            Location += (Velocity * (deltaTime.TotalSeconds * timeMultiplier));
        }

        public void Move(Vector3D moveVector)
        {
            Location += moveVector;
        }

        public void MoveTo(Point3D newLocation)
        {
            Location = newLocation;
        }

        public void Rotate(TimeSpan deltaTime, double timeMultiplier)
        {
            RotationAngle = (RotationAngle + (RotationSpeed * (deltaTime.TotalSeconds * timeMultiplier))) % 360.0;
        }

        public void Rotate(double rotateDegrees)
        {
            RotationAngle = (RotationAngle + rotateDegrees) % 360.0;
        }

        public void RotateTo(double newRotationAngle)
        {
            RotationAngle = newRotationAngle;
        }

        public void GravAccel(CelestialBody cbOther, TimeSpan deltaTime, double timeMultiplier)
        {
            Vector3D accelVector;
            Vector3D r;
            double accel;

            r = cbOther.Location - this.Location;
            accel = cbOther.Mass / r.LengthSquared;

            r.Normalize();
            accelVector = accel * r;

            Accelerate(accelVector, deltaTime, timeMultiplier);
        }

        public void Accelerate(Vector3D accelVector, TimeSpan deltaTime, double timeMultiplier)
        {
            if (VelocityInitialized)
            {
                Velocity += (accelVector * (deltaTime.TotalSeconds * timeMultiplier));
            }
            else
            {
                Velocity += (accelVector * ((deltaTime.TotalSeconds * timeMultiplier) / 2.0));
            }
        }

        public void CreateOrbit(CelestialBody cbToOrbit, string orbitDirection, double orbitRadius)
        {
            Vector3D locationDirVector;
            Vector3D velocityDirVector;
            double orbitVelocity;

            orbitVelocity = Math.Sqrt((cbToOrbit.Mass + this.Mass) / orbitRadius);

            switch (orbitDirection)
            {
                case "UpLeft":
                    locationDirVector = new Vector3D(-1, 1, 0);
                    break;
                case "Up":
                    locationDirVector = new Vector3D(0, 1, 0);
                    break;
                case "UpRight":
                    locationDirVector = new Vector3D(1, 1, 0);
                    break;
                case "Left":
                    locationDirVector = new Vector3D(-1, 0, 0);
                    break;
                case "Right":
                    locationDirVector = new Vector3D(1, 0, 0);
                    break;
                case "DownLeft":
                    locationDirVector = new Vector3D(-1, -1, 0);
                    break;
                case "Down":
                    locationDirVector = new Vector3D(0, -1, 0);
                    break;
                case "DownRight":
                    locationDirVector = new Vector3D(1, -1, 0);
                    break;
                default:
                    locationDirVector = new Vector3D(0, 0, 0);
                    break;
            }

            velocityDirVector = new Vector3D(0, 0, -1);

            locationDirVector.Normalize();
            velocityDirVector.Normalize();

            this.MoveTo((cbToOrbit.Location + (locationDirVector * orbitRadius)));
            this.Velocity = cbToOrbit.Velocity + (orbitVelocity * velocityDirVector);
        }
    }
}
