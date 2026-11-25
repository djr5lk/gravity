using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Gravity
{
    public abstract class CelestialBody : INotifyPropertyChanged
    {
        #region Fields/Properties
        protected bool modelInitialized;

        // Movement properties
        protected Point3D location;
        protected Vector3D velocity;
        protected Vector3D rotationAxis;
        protected double rotationAngle;
        protected double rotationSpeed;

        // 3D properties
        protected Transform3DGroup transformGroup;
        protected TranslateTransform3D locationTransform;
        protected RotateTransform3D rotationTransform;
        protected AxisAngleRotation3D rotation3D;

        // Generic properties
        protected string name;
        protected double mass;
        protected int size;

        public virtual string Name
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
        public virtual double Mass
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
        public virtual int Size
        {
            get
            {
                return size;
            }
            set
            {
                size = value;

                if (modelInitialized)
                {
                    RefreshGeometry();  //  If size is changed need to create new model
                }

                OnPropertyChanged("Size");
                OnPropertyChanged("SelectorCameraPosition");
            }
        }
        public virtual string SizeName { get { return "Size"; } }
        public virtual Model3DGroup Model { get; protected set; }
        public virtual Model3DGroup ModelRotationOnly { get; protected set; }

        public abstract Point3D SelectorCameraPosition { get; }
        public abstract Vector3D SelectorCameraUpDirection { get; }
        public abstract Vector3D SelectorCameraLookDirection { get; }
        public abstract double SelectorCameraFieldOfView { get; }

        public abstract Point3D InitialFollowCameraPosition { get; }
        public abstract Vector3D InitialFollowCameraUpDirection { get; }
        public abstract Vector3D InitialFollowCameraLookDirection { get; }

        public abstract Point3D InitialRideCameraPosition { get; }
        public abstract Vector3D InitialRideCameraUpDirection { get; }
        public abstract Vector3D InitialRideCameraLookDirection { get; }
        
        public virtual Point3D Location
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
        // Need to create separate Properties for location components to allow binding to each component
        public virtual double X
        {
            get
            {
                return location.X;
            }
            set
            {
                location.X = value;
                locationTransform.OffsetX = value;

                OnPropertyChanged("X");
            }
        }
        public virtual double Y
        {
            get
            {
                return location.Y;
            }
            set
            {
                location.Y = value;
                locationTransform.OffsetY = value;

                OnPropertyChanged("Y");
            }
        }
        public virtual double Z
        {
            get
            {
                return location.Z;
            }
            set
            {
                location.Z = value;
                locationTransform.OffsetZ = value;

                OnPropertyChanged("Z");
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
        public bool VelocityInitialized { get; set; }
        // Need to create separate Properties for velocity components so that binding will work properly
        public virtual double XVel
        {
            get
            {
                return velocity.X;
            }
            set
            {
                velocity.X = value;
                OnPropertyChanged("XVel");
            }
        }
        public virtual double YVel
        {
            get
            {
                return velocity.Y;
            }
            set
            {
                velocity.Y = value;
                OnPropertyChanged("YVel");
            }
        }
        public virtual double ZVel
        {
            get
            {
                return velocity.Z;
            }
            set
            {
                velocity.Z = value;
                OnPropertyChanged("ZVel");
            }
        }

        public virtual Vector3D RotationAxis
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
        public virtual double RotationAngle
        {
            get
            {
                return rotationAngle;
            }
            set
            {
                if ((value <= -360.0) || (value >= 360.0))
                {
                    throw new ArgumentOutOfRangeException("RotationAngle", "Rotation Angle must be >= -360 and <= 360");
                }
                rotationAngle = value;
                rotation3D.Angle = value;

                OnPropertyChanged("RotationAngle");
            }
        }
        public virtual double RotationSpeed
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
            : this(celestialBodySize: 10, celestialBodyMass: 1000, celestialBodyMaterial: new DiffuseMaterial(new SolidColorBrush(Colors.Blue)), celestialBodyRotationAxis: new Vector3D(0,1,0), celestialBodyRotationAngle: 0.0, celestialBodyRotationSpeed: 0.0)
        {
        }

        public CelestialBody(int celestialBodySize) 
            : this(celestialBodySize: celestialBodySize, celestialBodyMass: 1000, celestialBodyMaterial: new DiffuseMaterial(new SolidColorBrush(Colors.Blue)), celestialBodyRotationAxis: new Vector3D(0,1,0), celestialBodyRotationAngle: 0.0, celestialBodyRotationSpeed: 0.0)
        {
        }

        public CelestialBody(int celestialBodySize, Vector3D celestialBodyRotationAxis, double celestialBodyRotationSpeed) 
            : this(celestialBodySize: celestialBodySize, celestialBodyMass: 1000, celestialBodyMaterial: new DiffuseMaterial(new SolidColorBrush(Colors.Blue)), celestialBodyRotationAxis: celestialBodyRotationAxis, celestialBodyRotationAngle: 0.0, celestialBodyRotationSpeed: celestialBodyRotationSpeed)
        {
        }

        public CelestialBody(int celestialBodySize, Material celestialBodyMaterial)
            : this(celestialBodySize: celestialBodySize, celestialBodyMass: 1000, celestialBodyMaterial: celestialBodyMaterial, celestialBodyRotationAxis: new Vector3D(0, 1, 0), celestialBodyRotationAngle: 0.0, celestialBodyRotationSpeed: 0.0)
        {
        }

        public CelestialBody(int celestialBodySize, Material celestialBodyMaterial, Vector3D celestialBodyRotationAxis, double celestialBodyRotationSpeed)
            : this(celestialBodySize: celestialBodySize, celestialBodyMass: 1000, celestialBodyMaterial: celestialBodyMaterial, celestialBodyRotationAxis: celestialBodyRotationAxis, celestialBodyRotationAngle: 0.0, celestialBodyRotationSpeed: celestialBodyRotationSpeed)
        {
        }

        public CelestialBody(int celestialBodySize, double celestialBodyMass, Material celestialBodyMaterial, Vector3D celestialBodyRotationAxis, double celestialBodyRotationAngle, double celestialBodyRotationSpeed)
        {
            modelInitialized = false;
            VelocityInitialized = false;
            location = new Point3D(0, 0, 0);
            velocity = new Vector3D(0, 0, 0);

            size = celestialBodySize;
            mass = celestialBodyMass;
            rotationAxis = celestialBodyRotationAxis;
            rotationAngle = celestialBodyRotationAngle;
            rotationSpeed = celestialBodyRotationSpeed;

            Initialize3DComponents(celestialBodyMaterial);
        }

        #endregion

        protected abstract void Initialize3DComponents(Material material);
        protected abstract void RefreshGeometry();
        public abstract void SetMaterial(Material material);
        public abstract void Highlight();
        public abstract void Unhighlight();
        public abstract Collection<CelestialBody> Explode(TimeSpan deltaTime, double timeMultiplier);

        public override string ToString()
        {
            return this.Name;
        }

        public virtual void Move(TimeSpan deltaTime, double timeMultiplier)
        {
            Location += (Velocity * (deltaTime.TotalSeconds * timeMultiplier));
        }
        public virtual void Move(Vector3D moveVector)
        {
            Location += moveVector;
        }
        public virtual void MoveTo(Point3D newLocation)
        {
            Location = newLocation;
        }

        public virtual void Rotate(TimeSpan deltaTime, double timeMultiplier)
        {
            RotationAngle = (RotationAngle + (RotationSpeed * (deltaTime.TotalSeconds * timeMultiplier))) % 360.0;
        }
        public virtual void Rotate(double rotateDegrees)
        {
            RotationAngle = (RotationAngle + rotateDegrees) % 360.0;
        }
        public virtual void RotateTo(double newRotationAngle)
        {
            RotationAngle = newRotationAngle;
        }

        public virtual void ApplyGravity(CelestialBody cbOther, TimeSpan deltaTime, double timeMultiplier)
        {
            Vector3D accelVector;
            Vector3D r;
            double accel;

            r = cbOther.Location - this.Location;
            accel = ((r.Length == 0) ? cbOther.Mass : cbOther.Mass / r.LengthSquared);

            r.Normalize();
            accelVector = accel * r;

            Accelerate(accelVector, deltaTime, timeMultiplier);
        }

        public virtual void Accelerate(Vector3D accelVector, TimeSpan deltaTime, double timeMultiplier)
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

    public abstract class ControllableCelestialBody : CelestialBody
    {
        #region Fields/Properties

        // Movement properties
        protected Vector3D yawRotationAxis;
        protected Vector3D pitchRotationAxis;
        protected Vector3D rollRotationAxis;
        protected double yawRotationAngle;
        protected double pitchRotationAngle;
        protected double rollRotationAngle;
        //protected Transform3DGroup rotationTransformGroup;

        // 3D properties
        protected new QuaternionRotation3D rotation3D;
        protected Quaternion yawRotation = Quaternion.Identity;
        protected Quaternion pitchRotation = Quaternion.Identity;
        protected Quaternion rollRotation = Quaternion.Identity;
        //protected RotateTransform3D yawRotationTransform;
        //protected RotateTransform3D pitchRotationTransform;
        //protected RotateTransform3D rollRotationTransform;

        // Generic properties
        protected double maxThrust;
        protected double thrustPercent;

        public abstract Vector3D InitialThrustVector { get; }
        public virtual double MaxThrust
        {
            get
            {
                return maxThrust;
            }
            set
            {
                maxThrust = value;

                OnPropertyChanged("MaxThrust");
            }
        }
        public virtual double ThrustPercent
        {
            get
            {
                return thrustPercent;
            }
            set
            {
                if ((value < 0) || (value > 1))
                {
                    throw new ArgumentOutOfRangeException("ThrustPercent", "Thrust Percent must be >= 0 and <= 1");
                }

                thrustPercent = value;

                OnPropertyChanged("ThrustPercent");
            }
        }
        public virtual Vector3D YawRotationAxis
        {
            get
            {
                return yawRotationAxis;
            }
            set
            {
                yawRotationAxis = value;
                yawRotation = new Quaternion(yawRotationAxis, yawRotationAngle);

                OnPropertyChanged("YawRotationAxis");
            }
        }
        public virtual Vector3D PitchRotationAxis
        {
            get
            {
                return pitchRotationAxis;
            }
            set
            {
                pitchRotationAxis = value;
                pitchRotation = new Quaternion(pitchRotationAxis, pitchRotationAngle);
                
                OnPropertyChanged("PitchRotationAxis");
            }
        }
        public virtual Vector3D RollRotationAxis
        {
            get
            {
                return rollRotationAxis;
            }
            set
            {
                rollRotationAxis = value;
                rollRotation = new Quaternion(rollRotationAxis, rollRotationAngle);

                OnPropertyChanged("RollRotationAxis");
            }
        }
        public virtual double YawRotationAngle
        {
            get
            {
                return yawRotationAngle;
            }
            set
            {
                if ((value <= -360.0) || (value >= 360.0))
                {
                    throw new ArgumentOutOfRangeException("YawRotationAngle", "Yaw Rotation Angle must be >= -360 and <= 360");
                }
                yawRotationAngle = value;
                yawRotation = new Quaternion(yawRotationAxis, yawRotationAngle);

                OnPropertyChanged("YawRotationAngle");
            }
        }
        public virtual double PitchRotationAngle
        {
            get
            {
                return pitchRotationAngle;
            }
            set
            {
                if ((value <= -360.0) || (value >= 360.0))
                {
                    throw new ArgumentOutOfRangeException("PitchRotationAngle", "Pitch Rotation Angle must be >= -360 and <= 360");
                }
                pitchRotationAngle = value;
                pitchRotation = new Quaternion(pitchRotationAxis, pitchRotationAngle);

                OnPropertyChanged("PitchRotationAngle");
            }
        }
        public virtual double RollRotationAngle
        {
            get
            {
                return rollRotationAngle;
            }
            set
            {
                if ((value <= -360.0) || (value >= 360.0))
                {
                    throw new ArgumentOutOfRangeException("RollRotationAngle", "Roll Rotation Angle must be >= -360 and <= 360");
                }
                rollRotationAngle = value;
                rollRotation = new Quaternion(rollRotationAxis, rollRotationAngle);

                OnPropertyChanged("RollRotationAngle");
            }
        }
        #endregion

        #region Constructors

        public ControllableCelestialBody()
            : this(celestialBodySize: 10, celestialBodyMass: 1000, celestialBodyMaterial: new DiffuseMaterial(new SolidColorBrush(Colors.Blue)), celestialBodyRotationAxis: new Vector3D(0, 1, 0), celestialBodyRotationAngle: 0.0, celestialBodyRotationSpeed: 0.0)
        {
        }

        public ControllableCelestialBody(int celestialBodySize)
            : this(celestialBodySize: celestialBodySize, celestialBodyMass: 1000, celestialBodyMaterial: new DiffuseMaterial(new SolidColorBrush(Colors.Blue)), celestialBodyRotationAxis: new Vector3D(0, 1, 0), celestialBodyRotationAngle: 0.0, celestialBodyRotationSpeed: 0.0)
        {
        }

        public ControllableCelestialBody(int celestialBodySize, Vector3D celestialBodyRotationAxis, double celestialBodyRotationSpeed)
            : this(celestialBodySize: celestialBodySize, celestialBodyMass: 1000, celestialBodyMaterial: new DiffuseMaterial(new SolidColorBrush(Colors.Blue)), celestialBodyRotationAxis: celestialBodyRotationAxis, celestialBodyRotationAngle: 0.0, celestialBodyRotationSpeed: celestialBodyRotationSpeed)
        {
        }

        public ControllableCelestialBody(int celestialBodySize, Material celestialBodyMaterial)
            : this(celestialBodySize: celestialBodySize, celestialBodyMass: 1000, celestialBodyMaterial: celestialBodyMaterial, celestialBodyRotationAxis: new Vector3D(0, 1, 0), celestialBodyRotationAngle: 0.0, celestialBodyRotationSpeed: 0.0)
        {
        }

        public ControllableCelestialBody(int celestialBodySize, Material celestialBodyMaterial, Vector3D celestialBodyRotationAxis, double celestialBodyRotationSpeed)
            : this(celestialBodySize: celestialBodySize, celestialBodyMass: 1000, celestialBodyMaterial: celestialBodyMaterial, celestialBodyRotationAxis: celestialBodyRotationAxis, celestialBodyRotationAngle: 0.0, celestialBodyRotationSpeed: celestialBodyRotationSpeed)
        {
        }

        public ControllableCelestialBody(int celestialBodySize, double celestialBodyMass, Material celestialBodyMaterial, Vector3D celestialBodyRotationAxis, double celestialBodyRotationAngle, double celestialBodyRotationSpeed)
        {
            modelInitialized = false;
            VelocityInitialized = false;
            location = new Point3D(0, 0, 0);
            velocity = new Vector3D(0, 0, 0);

            maxThrust = 10000;
            thrustPercent = 0;

            size = celestialBodySize;
            mass = celestialBodyMass;

            rotationAxis = celestialBodyRotationAxis;
            yawRotationAxis = new Vector3D(0, 1, 0);
            pitchRotationAxis = new Vector3D(1, 0, 0);
            rollRotationAxis = new Vector3D(0, 0, -1);

            rotationAngle = celestialBodyRotationAngle;
            yawRotationAngle = 0;
            pitchRotationAngle = 0;
            rollRotationAngle = 0;

            rotationSpeed = celestialBodyRotationSpeed;

            Initialize3DComponents(celestialBodyMaterial);
        }

        #endregion

        public abstract void Yaw(double degrees);
        public abstract void Pitch(double degrees);
        public abstract void Roll(double degrees);

        public override void Move(TimeSpan deltaTime, double timeMultiplier)
        {
            Vector3D accelVector = InitialThrustVector;

            accelVector.Normalize();
            accelVector *= ((MaxThrust * ThrustPercent) / Mass);

            rotation3D.Quaternion = yawRotation * pitchRotation * rollRotation;
            accelVector = rotationTransform.Transform(accelVector);
            //accelVector = pitchRotationTransform.Transform(accelVector);
            //accelVector = rollRotationTransform.Transform(accelVector);

            Accelerate(accelVector, deltaTime, timeMultiplier);

            Location += (Velocity * (deltaTime.TotalSeconds * timeMultiplier));
        }

        public override void Rotate(TimeSpan deltaTime, double timeMultiplier)
        {
            // do nothing
        }
    }

    public class SphericalCelestialBody : CelestialBody
    {
        #region Fields/Properties

        public override int Size
        {
            get
            {
                return base.Size;
            }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException("Radius", "Radius must be positive");
                }

                base.Size = value;
            }
        }
        public override string SizeName { get { return "Radius"; } }

        public override Point3D SelectorCameraPosition { get { return new Point3D(0, 0, (double)Size * 3.0); } }
        public override Vector3D SelectorCameraUpDirection { get { return new Vector3D(0, 1, 0); } }
        public override Vector3D SelectorCameraLookDirection { get { return new Vector3D(0, 0, -1); } }
        public override double SelectorCameraFieldOfView { get { return 60; } }

        public override Point3D InitialFollowCameraPosition { get { return Location + new Vector3D(0, 0, (double)Size * 20.0); } }
        public override Vector3D InitialFollowCameraUpDirection { get { return new Vector3D(0, 1, 0); } }
        public override Vector3D InitialFollowCameraLookDirection { get { return new Vector3D(0, 0, -1); } }

        public override Point3D InitialRideCameraPosition { get { return Location + new Vector3D(0, 0, (double)Size); } }
        public override Vector3D InitialRideCameraUpDirection { get { return new Vector3D(0, 1, 0); } }
        public override Vector3D InitialRideCameraLookDirection { get { return new Vector3D(0, 0, 1); } }

        #endregion

        #region Constructors

        public SphericalCelestialBody()
            : base()
        {
        }

        public SphericalCelestialBody(int size)
            : base(size)
        {
        }

        public SphericalCelestialBody(int size, Vector3D rotationAxis, double rotationSpeed)
            : base(size, rotationAxis, rotationSpeed)
        {
        }

        public SphericalCelestialBody(int size, Material material)
            : base(size, material)
        {
        }

        public SphericalCelestialBody(int size, Material material, Vector3D rotationAxis, double rotationSpeed)
            : base(size, material, rotationAxis, rotationSpeed)
        {
        }

        public SphericalCelestialBody(int size, double mass, Material material, Vector3D rotationAxis, double rotationAngle, double rotationSpeed)
            : base(size, mass, material, rotationAxis, rotationAngle, rotationSpeed)
        {
        }

        #endregion

        protected override void Initialize3DComponents(Material material)
        {
            // Create sphere geometry for planet based on radius
            Sphere3D geometry = new Sphere3D(Size);

            // Assign geometry to a mesh object that can be added into the 3D model
            MeshGeometry3D mesh = new MeshGeometry3D
            {
                Positions = geometry.Points,
                TriangleIndices = geometry.TriangleIndices,
                TextureCoordinates = geometry.TextureCoordinates
            };

            // Create transforms to show models in proper location and with proper rotation
            locationTransform = new TranslateTransform3D((Vector3D)location);
            rotation3D = new AxisAngleRotation3D(rotationAxis, rotationAngle);
            rotationTransform = new RotateTransform3D(rotation3D, location);
            transformGroup = new Transform3DGroup();
            transformGroup.Children.Add(rotationTransform);
            transformGroup.Children.Add(locationTransform);

            // Create models using assigned mesh and material
            Model = new Model3DGroup();
            ModelRotationOnly = new Model3DGroup();

            Model.Children.Add(new GeometryModel3D(mesh, material));
            ModelRotationOnly.Children.Add(new GeometryModel3D(mesh, material));

            Model.Transform = transformGroup;
            ModelRotationOnly.Transform = rotationTransform;

            // Set flag indicating that the 3D model has been initialized
            modelInitialized = true;
        }

        protected override void RefreshGeometry()
        {
            // Create sphere geometry for planet based on radius
            Sphere3D geometry = new Sphere3D(Size);

            // Assign geometry to a mesh object that can be added into the 3D model
            MeshGeometry3D mesh = new MeshGeometry3D
            {
                Positions = geometry.Points,
                TriangleIndices = geometry.TriangleIndices,
                TextureCoordinates = geometry.TextureCoordinates
            };

            ((GeometryModel3D)Model.Children[0]).Geometry = mesh;
            ((GeometryModel3D)ModelRotationOnly.Children[0]).Geometry = mesh;
        }

        public override void SetMaterial(Material material)
        {
            ((GeometryModel3D)Model.Children[0]).Material = material;
            ((GeometryModel3D)ModelRotationOnly.Children[0]).Material = material;
        }
        public override void Highlight()
        {
            transformGroup.Children.RemoveAt(1);
            transformGroup.Children.Add(new ScaleTransform3D(2, 2, 2));
            transformGroup.Children.Add(locationTransform);
        }
        public override void Unhighlight()
        {
            transformGroup.Children.RemoveAt(1);
        }
        public override Collection<CelestialBody> Explode(TimeSpan deltaTime, double timeMultiplier)
        {
            Random rand = new Random();

            Collection<CelestialBody> pieces = new Collection<CelestialBody>();

            double percentLeft = 1;
            double piecePercent;
            double pieceVolume;
            int pieceNumber = 1;
            double pieceMass;
            int pieceRadius;
            string pieceName;
            CelestialBody newPiece;
            Vector3D accelVector;
            Vector3D rotateVector;

            double explosionForce = Mass * 1000;

            while (percentLeft > 0)
            {
                piecePercent = rand.NextDouble() * 0.20;
                if (piecePercent > percentLeft)
                    piecePercent = percentLeft;
                percentLeft -= piecePercent;

                pieceName = Name + '_' + pieceNumber;
                pieceMass = (Mass * piecePercent);
                pieceVolume = ((4.0 / 3.0) * Math.PI * Math.Pow(Size, 3.0)) * piecePercent;
                pieceRadius = (int)Math.Pow(((3.0 * pieceVolume) / (4.0 * Math.PI)), (1.0 / 3.0));

                do
                {
                    accelVector = new Vector3D(rand.NextDouble() * rand.Next(-1, 2), rand.NextDouble() * rand.Next(-1, 2), rand.NextDouble() * rand.Next(-1, 2));
                } while (accelVector.Length == 0);

                do
                {
                    rotateVector = new Vector3D(rand.NextDouble() * rand.Next(-1, 2), rand.NextDouble() * rand.Next(-1, 2), rand.NextDouble() * rand.Next(-1, 2));
                } while (rotateVector.Length == 0);

                accelVector.Normalize();
                rotateVector.Normalize();

                newPiece = new AsteroidCelestialBody(pieceRadius, ((GeometryModel3D)Model.Children[0]).Material, rotateVector, rand.NextDouble() * (100-25) + 25)
                {
                    Name = pieceName,
                    Mass = pieceMass
                };

                newPiece.MoveTo(Location + (accelVector * 100));
                newPiece.Velocity = Velocity;
                newPiece.Accelerate(accelVector * (explosionForce / pieceMass), deltaTime, timeMultiplier);

                pieces.Add(newPiece);
                pieceNumber++;
            }
            return pieces;
        }
    }
    public class AsteroidCelestialBody : SphericalCelestialBody
    {
        #region Fields/Properties

        #endregion

        #region Constructors

        public AsteroidCelestialBody()
            : base()
        {
        }

        public AsteroidCelestialBody(int size)
            : base(size)
        {
        }

        public AsteroidCelestialBody(int size, Vector3D rotationAxis, double rotationSpeed)
            : base(size, rotationAxis, rotationSpeed)
        {
        }

        public AsteroidCelestialBody(int size, Material material)
            : base(size, material)
        {
        }

        public AsteroidCelestialBody(int size, Material material, Vector3D rotationAxis, double rotationSpeed)
            : base(size, material, rotationAxis, rotationSpeed)
        {
        }

        public AsteroidCelestialBody(int size, double mass, Material material, Vector3D rotationAxis, double rotationAngle, double rotationSpeed)
            : base(size, mass, material, rotationAxis, rotationAngle, rotationSpeed)
        {
        }

        #endregion

        protected override void Initialize3DComponents(Material material)
        {
            // Create sphere geometry for planet based on radius
            Sphere3D geometry = new Sphere3D(Size, 5);

            // Assign geometry to a mesh object that can be added into the 3D model
            MeshGeometry3D mesh = new MeshGeometry3D
            {
                Positions = geometry.Points,
                TriangleIndices = geometry.TriangleIndices,
                TextureCoordinates = geometry.TextureCoordinates
            };

            //Randomly modify points
            Random rand = new Random();
            ScaleTransform3D s = new ScaleTransform3D();
            for (int i = 25; i < (mesh.Positions.Count - 25); i++)
            {
                if (rand.Next(0, 4) == 0)  // only modify roughly 1/4 of the points (chosen randomly)
                {
                    s.ScaleX = rand.NextDouble() * (1.15 - 1.05) + 1.05;
                    s.ScaleY = rand.NextDouble() * (1.15 - 1.05) + 1.05;
                    s.ScaleZ = rand.NextDouble() * (1.15 - 1.05) + 1.05;
                    mesh.Positions[i] = s.Transform(mesh.Positions[i]);
                }
                if ((i + 1) % 25 == 0)
                {
                    mesh.Positions[i] = mesh.Positions[i - 24];
                }
            }
            for (int i = 25; i < (mesh.Positions.Count - 25); i++)
            {
                if (rand.Next(0, 4) == 0)  // only modify roughly 1/4 of the points (chosen randomly)
                {
                    s.ScaleX = rand.NextDouble() * (0.95 - 0.85) + 0.85;
                    s.ScaleY = rand.NextDouble() * (0.95 - 0.85) + 0.85;
                    s.ScaleZ = rand.NextDouble() * (0.95 - 0.85) + 0.85;
                    mesh.Positions[i] = s.Transform(mesh.Positions[i]);
                }
                if ((i + 1) % 25 == 0)
                {
                    mesh.Positions[i] = mesh.Positions[i - 24];
                }
            }
            for (int i = 25; i < (mesh.Positions.Count - 25); i++)
            {
                if (rand.Next(0, 4) == 0)  // only modify roughly 1/4 of the points (chosen randomly)
                {
                    s.ScaleX = rand.NextDouble() * (1.08 - 0.92) + 0.92;
                    s.ScaleY = rand.NextDouble() * (1.08 - 0.92) + 0.92;
                    s.ScaleZ = rand.NextDouble() * (1.08 - 0.92) + 0.92;
                    mesh.Positions[i] = s.Transform(mesh.Positions[i]);
                }
                if ((i + 1) % 25 == 0)
                {
                    mesh.Positions[i] = mesh.Positions[i - 24];
                }
            }
            s.ScaleX = rand.NextDouble() * (2.2 - 1.42) + 1.42;
            s.ScaleY = 1;
            s.ScaleZ = 1;
            for (int i = 0; i < mesh.Positions.Count; i++)
            {
                mesh.Positions[i] = s.Transform(mesh.Positions[i]);
            }

            // Create transforms to show models in proper location and with proper rotation
            locationTransform = new TranslateTransform3D((Vector3D)location);
            rotation3D = new AxisAngleRotation3D(rotationAxis, rotationAngle);
            rotationTransform = new RotateTransform3D(rotation3D, location);
            transformGroup = new Transform3DGroup();
            transformGroup.Children.Add(rotationTransform);
            transformGroup.Children.Add(locationTransform);

            // Create models using assigned mesh and material
            Model = new Model3DGroup();
            ModelRotationOnly = new Model3DGroup();

            Model.Children.Add(new GeometryModel3D(mesh, material));
            ModelRotationOnly.Children.Add(new GeometryModel3D(mesh, material));

            Model.Transform = transformGroup;
            ModelRotationOnly.Transform = rotationTransform;

            // Set flag indicating that the 3D model has been initialized
            modelInitialized = true;
        }

        protected override void RefreshGeometry()
        {
            // Create sphere geometry for planet based on radius
            Sphere3D geometry = new Sphere3D(Size);

            // Assign geometry to a mesh object that can be added into the 3D model
            MeshGeometry3D mesh = new MeshGeometry3D
            {
                Positions = geometry.Points,
                TriangleIndices = geometry.TriangleIndices,
                TextureCoordinates = geometry.TextureCoordinates
            };

            ((GeometryModel3D)Model.Children[0]).Geometry = mesh;
            ((GeometryModel3D)ModelRotationOnly.Children[0]).Geometry = mesh;
        }
    }

    public class SphericalRingedCelestialBody : SphericalCelestialBody
    {
        #region Fields/Properties

        #endregion

        #region Constructors

        public SphericalRingedCelestialBody()
            : base()
        {
        }

        public SphericalRingedCelestialBody(int size)
            : base(size)
        {
        }

        public SphericalRingedCelestialBody(int size, Vector3D rotationAxis, double rotationSpeed)
            : base(size, rotationAxis, rotationSpeed)
        {
        }

        public SphericalRingedCelestialBody(int size, Material material)
            : base(size, material)
        {
        }

        public SphericalRingedCelestialBody(int size, Material material, Vector3D rotationAxis, double rotationSpeed)
            : base(size, material, rotationAxis, rotationSpeed)
        {
        }

        public SphericalRingedCelestialBody(int size, double mass, Material material, Vector3D rotationAxis, double rotationAngle, double rotationSpeed)
            : base(size, mass, material, rotationAxis, rotationAngle, rotationSpeed)
        {
        }

        #endregion

        protected override void Initialize3DComponents(Material material)
        {
            // Create sphere geometry for planet based on radius
            Sphere3D geometry = new Sphere3D(Size);

            DiscGeometry3D ringGeometry = new DiscGeometry3D(Size * 3, Size * 2);

            // Assign geometry to a mesh object that can be added into the 3D model
            MeshGeometry3D mesh = new MeshGeometry3D
            {
                Positions = geometry.Points,
                TriangleIndices = geometry.TriangleIndices,
                TextureCoordinates = geometry.TextureCoordinates
            };

            MeshGeometry3D ringMesh = new MeshGeometry3D
            {
                Positions = ringGeometry.Points,
                TriangleIndices = ringGeometry.TriangleIndices,
                TextureCoordinates = ringGeometry.TextureCoordinates
            };

            // Create transforms to show models in proper location and with proper rotation
            locationTransform = new TranslateTransform3D((Vector3D)location);
            rotation3D = new AxisAngleRotation3D(rotationAxis, rotationAngle);
            rotationTransform = new RotateTransform3D(rotation3D, location);
            transformGroup = new Transform3DGroup();
            transformGroup.Children.Add(rotationTransform);
            transformGroup.Children.Add(locationTransform);

            // Create models using assigned mesh and material
            Model = new Model3DGroup();
            ModelRotationOnly = new Model3DGroup();

            Model.Children.Add(new GeometryModel3D(mesh, material));
            Model.Children.Add(new GeometryModel3D(ringMesh, material) { BackMaterial = material });
            ModelRotationOnly.Children.Add(new GeometryModel3D(mesh, material));
            ModelRotationOnly.Children.Add(new GeometryModel3D(ringMesh, material) { BackMaterial = material });

            Model.Children[0].Transform = transformGroup;
            Model.Children[1].Transform = locationTransform;
            ModelRotationOnly.Transform = rotationTransform;

            // Set flag indicating that the 3D model has been initialized
            modelInitialized = true;
        }

        protected override void RefreshGeometry()
        {
            // Create sphere geometry for planet based on radius
            Sphere3D geometry = new Sphere3D(Size);

            DiscGeometry3D ringGeometry = new DiscGeometry3D(Size * 3, Size * 2);

            // Assign geometry to a mesh object that can be added into the 3D model
            MeshGeometry3D mesh = new MeshGeometry3D
            {
                Positions = geometry.Points,
                TriangleIndices = geometry.TriangleIndices,
                TextureCoordinates = geometry.TextureCoordinates
            };

            MeshGeometry3D ringMesh = new MeshGeometry3D
            {
                Positions = ringGeometry.Points,
                TriangleIndices = ringGeometry.TriangleIndices,
                TextureCoordinates = ringGeometry.TextureCoordinates
            };

            ((GeometryModel3D)Model.Children[0]).Geometry = mesh;
            ((GeometryModel3D)Model.Children[1]).Geometry = ringMesh;
            ((GeometryModel3D)ModelRotationOnly.Children[0]).Geometry = mesh;
            ((GeometryModel3D)ModelRotationOnly.Children[1]).Geometry = ringMesh;
        }
    }


    public class SpaceshipCelestialBody : ControllableCelestialBody
    {
        #region Fields/Properties

        public override Point3D SelectorCameraPosition { get { return new Point3D(0, 0, (double)Size * 3.0); } }
        public override Vector3D SelectorCameraUpDirection { get { return new Vector3D(0, 1, 0); } }
        public override Vector3D SelectorCameraLookDirection { get { return new Vector3D(0, 0, -1); } }
        public override double SelectorCameraFieldOfView { get { return 60; } }

        public override Point3D InitialFollowCameraPosition { get { return Location + new Vector3D(0, 0, (double)Size * 20.0); } }
        public override Vector3D InitialFollowCameraUpDirection { get { return new Vector3D(0, 1, 0); } }
        public override Vector3D InitialFollowCameraLookDirection { get { return new Vector3D(0, 0, -1); } }

        public override Point3D InitialRideCameraPosition { get { return Location + new Vector3D(0, 0, (double)Size); } }
        public override Vector3D InitialRideCameraUpDirection { get { return new Vector3D(0, 1, 0); } }
        public override Vector3D InitialRideCameraLookDirection { get { return new Vector3D(0, 0, 1); } }

        public override Vector3D InitialThrustVector { get { return new Vector3D(0, 0, -1); } }

        #endregion

        #region Constructors

        public SpaceshipCelestialBody()
            : base()
        {
        }

        public SpaceshipCelestialBody(int size)
            : base(size)
        {
        }

        public SpaceshipCelestialBody(int size, Vector3D rotationAxis, double rotationSpeed)
            : base(size, rotationAxis, rotationSpeed)
        {
        }

        public SpaceshipCelestialBody(int size, Material material)
            : base(size, material)
        {
        }

        public SpaceshipCelestialBody(int size, Material material, Vector3D rotationAxis, double rotationSpeed)
            : base(size, material, rotationAxis, rotationSpeed)
        {
        }

        public SpaceshipCelestialBody(int size, double mass, Material material, Vector3D rotationAxis, double rotationAngle, double rotationSpeed)
            : base(size, mass, material, rotationAxis, rotationAngle, rotationSpeed)
        {
        }

        #endregion

        protected override void Initialize3DComponents(Material material)
        {
            // Assign geometry to a mesh object that can be added into the 3D model
            MeshGeometry3D mesh = new MeshGeometry3D();

            mesh.Positions.Add(new Point3D(-Size, Size/3, Size/2));         //0
            mesh.Positions.Add(new Point3D(Size, Size/3, Size/2));          //1
            mesh.Positions.Add(new Point3D(Size, -Size/3, Size/2));         //2
            mesh.Positions.Add(new Point3D(-Size, -Size/3, Size/2));        //3
            mesh.Positions.Add(new Point3D(-Size, Size / 3, 0));            //4
            mesh.Positions.Add(new Point3D(Size, Size / 3, 0));             //5
            mesh.Positions.Add(new Point3D(Size, -Size / 3, 0));            //6
            mesh.Positions.Add(new Point3D(-Size, -Size / 3, 0));           //7
            mesh.Positions.Add(new Point3D(-(Size * 0.8), Size / 3, 0));    //8
            mesh.Positions.Add(new Point3D((Size * 0.8), Size / 3, 0));     //9
            mesh.Positions.Add(new Point3D((Size * 0.8), -Size / 3, 0));    //10
            mesh.Positions.Add(new Point3D(-(Size * 0.8), -Size / 3, 0));   //11
            mesh.Positions.Add(new Point3D(-Size, 0, -Size));               //12
            mesh.Positions.Add(new Point3D(-(Size * 0.6), 0, -Size));       //13
            mesh.Positions.Add(new Point3D((Size * 0.6), 0, -Size));        //14
            mesh.Positions.Add(new Point3D(Size, 0, -Size));                //15

            mesh.TriangleIndices.Add(0); mesh.TriangleIndices.Add(3); mesh.TriangleIndices.Add(1);
            mesh.TriangleIndices.Add(3); mesh.TriangleIndices.Add(2); mesh.TriangleIndices.Add(1);
            mesh.TriangleIndices.Add(0); mesh.TriangleIndices.Add(4); mesh.TriangleIndices.Add(3);
            mesh.TriangleIndices.Add(4); mesh.TriangleIndices.Add(7); mesh.TriangleIndices.Add(3);
            mesh.TriangleIndices.Add(5); mesh.TriangleIndices.Add(1); mesh.TriangleIndices.Add(6);
            mesh.TriangleIndices.Add(1); mesh.TriangleIndices.Add(2); mesh.TriangleIndices.Add(6);
            mesh.TriangleIndices.Add(0); mesh.TriangleIndices.Add(5); mesh.TriangleIndices.Add(4);
            mesh.TriangleIndices.Add(5); mesh.TriangleIndices.Add(0); mesh.TriangleIndices.Add(1);
            mesh.TriangleIndices.Add(3); mesh.TriangleIndices.Add(7); mesh.TriangleIndices.Add(6);
            mesh.TriangleIndices.Add(3); mesh.TriangleIndices.Add(6); mesh.TriangleIndices.Add(2);

            mesh.TriangleIndices.Add(4); mesh.TriangleIndices.Add(8); mesh.TriangleIndices.Add(12);
            mesh.TriangleIndices.Add(7); mesh.TriangleIndices.Add(4); mesh.TriangleIndices.Add(12);
            mesh.TriangleIndices.Add(11); mesh.TriangleIndices.Add(7); mesh.TriangleIndices.Add(12);
            mesh.TriangleIndices.Add(8); mesh.TriangleIndices.Add(11); mesh.TriangleIndices.Add(12);

            mesh.TriangleIndices.Add(9); mesh.TriangleIndices.Add(5); mesh.TriangleIndices.Add(15);
            mesh.TriangleIndices.Add(5); mesh.TriangleIndices.Add(6); mesh.TriangleIndices.Add(15);
            mesh.TriangleIndices.Add(6); mesh.TriangleIndices.Add(10); mesh.TriangleIndices.Add(15);
            mesh.TriangleIndices.Add(10); mesh.TriangleIndices.Add(9); mesh.TriangleIndices.Add(15);

            mesh.TriangleIndices.Add(8); mesh.TriangleIndices.Add(9); mesh.TriangleIndices.Add(13);
            mesh.TriangleIndices.Add(14); mesh.TriangleIndices.Add(13); mesh.TriangleIndices.Add(9);
            mesh.TriangleIndices.Add(11); mesh.TriangleIndices.Add(8); mesh.TriangleIndices.Add(13);
            mesh.TriangleIndices.Add(10); mesh.TriangleIndices.Add(11); mesh.TriangleIndices.Add(14);
            mesh.TriangleIndices.Add(11); mesh.TriangleIndices.Add(13); mesh.TriangleIndices.Add(14);
            mesh.TriangleIndices.Add(9); mesh.TriangleIndices.Add(10); mesh.TriangleIndices.Add(14);

            //mesh.Positions.Add(new Point3D(-Size, Size, Size));
            //mesh.Positions.Add(new Point3D(Size, Size, Size));
            //mesh.Positions.Add(new Point3D(Size, -Size, Size));
            //mesh.Positions.Add(new Point3D(-Size, -Size, Size));
            //mesh.Positions.Add(new Point3D(0, 0, -Size));

            //mesh.TriangleIndices.Add(0); mesh.TriangleIndices.Add(3); mesh.TriangleIndices.Add(1);
            //mesh.TriangleIndices.Add(3); mesh.TriangleIndices.Add(2); mesh.TriangleIndices.Add(1);
            //mesh.TriangleIndices.Add(2); mesh.TriangleIndices.Add(4); mesh.TriangleIndices.Add(1);
            //mesh.TriangleIndices.Add(1); mesh.TriangleIndices.Add(4); mesh.TriangleIndices.Add(0);
            //mesh.TriangleIndices.Add(0); mesh.TriangleIndices.Add(4); mesh.TriangleIndices.Add(3);
            //mesh.TriangleIndices.Add(3); mesh.TriangleIndices.Add(4); mesh.TriangleIndices.Add(2);

            // Create transforms to show models in proper location and with proper rotation
            locationTransform = new TranslateTransform3D((Vector3D)location);
            rotation3D = new QuaternionRotation3D(yawRotation * pitchRotation * rollRotation);
            rotationTransform = new RotateTransform3D(rotation3D, location);
            //yawRotation3D = new AxisAngleRotation3D(yawRotationAxis, yawRotationAngle);
            //yawRotationTransform = new RotateTransform3D(yawRotation3D, location);
            //pitchRotation3D = new AxisAngleRotation3D(pitchRotationAxis, pitchRotationAngle);
            //pitchRotationTransform = new RotateTransform3D(pitchRotation3D, location);
            //rollRotation3D = new AxisAngleRotation3D(rollRotationAxis, rollRotationAngle);
            //rollRotationTransform = new RotateTransform3D(rollRotation3D, location);
            transformGroup = new Transform3DGroup();
            transformGroup.Children.Add(rotationTransform);
            //transformGroup.Children.Add(yawRotationTransform);
            //transformGroup.Children.Add(pitchRotationTransform);
            //transformGroup.Children.Add(rollRotationTransform);
            transformGroup.Children.Add(locationTransform);

            //rotationTransformGroup = new Transform3DGroup();
            //rotationTransformGroup.Children.Add(rotationTransform);
            //rotationTransformGroup.Children.Add(yawRotationTransform);
            //rotationTransformGroup.Children.Add(pitchRotationTransform);
            //rotationTransformGroup.Children.Add(rollRotationTransform);

            // Create models using assigned mesh and material
            Model = new Model3DGroup();
            ModelRotationOnly = new Model3DGroup();

            Model.Children.Add(new GeometryModel3D(mesh, material));
            ModelRotationOnly.Children.Add(new GeometryModel3D(mesh, material));

            Model.Transform = transformGroup;
            ModelRotationOnly.Transform = rotationTransform;

            // Set flag indicating that the 3D model has been initialized
            modelInitialized = true;
        }

        protected override void RefreshGeometry()
        {
            // Assign geometry to a mesh object that can be added into the 3D model
            MeshGeometry3D mesh = new MeshGeometry3D();

            mesh.Positions.Add(new Point3D(-Size, Size / 3, Size / 2));         //0
            mesh.Positions.Add(new Point3D(Size, Size / 3, Size / 2));          //1
            mesh.Positions.Add(new Point3D(Size, -Size / 3, Size / 2));         //2
            mesh.Positions.Add(new Point3D(-Size, -Size / 3, Size / 2));        //3
            mesh.Positions.Add(new Point3D(-Size, Size / 3, 0));            //4
            mesh.Positions.Add(new Point3D(Size, Size / 3, 0));             //5
            mesh.Positions.Add(new Point3D(Size, -Size / 3, 0));            //6
            mesh.Positions.Add(new Point3D(-Size, -Size / 3, 0));           //7
            mesh.Positions.Add(new Point3D(-(Size * 0.8), Size / 3, 0));    //8
            mesh.Positions.Add(new Point3D((Size * 0.8), Size / 3, 0));     //9
            mesh.Positions.Add(new Point3D((Size * 0.8), -Size / 3, 0));    //10
            mesh.Positions.Add(new Point3D(-(Size * 0.8), -Size / 3, 0));   //11
            mesh.Positions.Add(new Point3D(-Size, 0, -Size));               //12
            mesh.Positions.Add(new Point3D(-(Size * 0.6), 0, -Size));       //13
            mesh.Positions.Add(new Point3D((Size * 0.6), 0, -Size));        //14
            mesh.Positions.Add(new Point3D(Size, 0, -Size));                //15

            mesh.TriangleIndices.Add(0); mesh.TriangleIndices.Add(3); mesh.TriangleIndices.Add(1);
            mesh.TriangleIndices.Add(3); mesh.TriangleIndices.Add(2); mesh.TriangleIndices.Add(1);
            mesh.TriangleIndices.Add(0); mesh.TriangleIndices.Add(4); mesh.TriangleIndices.Add(3);
            mesh.TriangleIndices.Add(4); mesh.TriangleIndices.Add(7); mesh.TriangleIndices.Add(3);
            mesh.TriangleIndices.Add(5); mesh.TriangleIndices.Add(1); mesh.TriangleIndices.Add(6);
            mesh.TriangleIndices.Add(1); mesh.TriangleIndices.Add(2); mesh.TriangleIndices.Add(6);
            mesh.TriangleIndices.Add(0); mesh.TriangleIndices.Add(5); mesh.TriangleIndices.Add(4);
            mesh.TriangleIndices.Add(5); mesh.TriangleIndices.Add(0); mesh.TriangleIndices.Add(1);
            mesh.TriangleIndices.Add(3); mesh.TriangleIndices.Add(7); mesh.TriangleIndices.Add(6);
            mesh.TriangleIndices.Add(3); mesh.TriangleIndices.Add(6); mesh.TriangleIndices.Add(2);

            mesh.TriangleIndices.Add(4); mesh.TriangleIndices.Add(8); mesh.TriangleIndices.Add(12);
            mesh.TriangleIndices.Add(7); mesh.TriangleIndices.Add(4); mesh.TriangleIndices.Add(12);
            mesh.TriangleIndices.Add(11); mesh.TriangleIndices.Add(7); mesh.TriangleIndices.Add(12);
            mesh.TriangleIndices.Add(8); mesh.TriangleIndices.Add(11); mesh.TriangleIndices.Add(12);

            mesh.TriangleIndices.Add(9); mesh.TriangleIndices.Add(5); mesh.TriangleIndices.Add(15);
            mesh.TriangleIndices.Add(5); mesh.TriangleIndices.Add(6); mesh.TriangleIndices.Add(15);
            mesh.TriangleIndices.Add(6); mesh.TriangleIndices.Add(10); mesh.TriangleIndices.Add(15);
            mesh.TriangleIndices.Add(10); mesh.TriangleIndices.Add(9); mesh.TriangleIndices.Add(15);

            mesh.TriangleIndices.Add(8); mesh.TriangleIndices.Add(9); mesh.TriangleIndices.Add(13);
            mesh.TriangleIndices.Add(14); mesh.TriangleIndices.Add(13); mesh.TriangleIndices.Add(9);
            mesh.TriangleIndices.Add(11); mesh.TriangleIndices.Add(8); mesh.TriangleIndices.Add(13);
            mesh.TriangleIndices.Add(10); mesh.TriangleIndices.Add(11); mesh.TriangleIndices.Add(14);
            mesh.TriangleIndices.Add(11); mesh.TriangleIndices.Add(13); mesh.TriangleIndices.Add(14);
            mesh.TriangleIndices.Add(9); mesh.TriangleIndices.Add(10); mesh.TriangleIndices.Add(14);

            ((GeometryModel3D)Model.Children[0]).Geometry = mesh;
            ((GeometryModel3D)ModelRotationOnly.Children[0]).Geometry = mesh;
        }

        public override void SetMaterial(Material material)
        {
            ((GeometryModel3D)Model.Children[0]).Material = material;
            ((GeometryModel3D)ModelRotationOnly.Children[0]).Material = material;
        }
        public override void Highlight()
        {
            transformGroup.Children.RemoveAt(1);
            transformGroup.Children.Add(new ScaleTransform3D(2, 2, 2));
            transformGroup.Children.Add(locationTransform);
        }
        public override void Unhighlight()
        {
            transformGroup.Children.RemoveAt(1);
        }
        public override Collection<CelestialBody> Explode(TimeSpan deltaTime, double timeMultiplier)
        {
            Random rand = new Random();

            Collection<CelestialBody> pieces = new Collection<CelestialBody>();

            double percentLeft = 1;
            double piecePercent;
            double pieceVolume;
            int pieceNumber = 1;
            double pieceMass;
            int pieceRadius;
            string pieceName;
            CelestialBody newPiece;
            Vector3D accelVector;
            Vector3D rotateVector;

            double explosionForce = Mass * 1000;

            while (percentLeft > 0)
            {
                piecePercent = rand.NextDouble() * 0.20;
                if (piecePercent > percentLeft)
                    piecePercent = percentLeft;
                percentLeft -= piecePercent;

                pieceName = Name + '_' + pieceNumber;
                pieceMass = (Mass * piecePercent);
                pieceVolume = ((4.0 / 3.0) * Math.PI * Math.Pow(Size, 3.0)) * piecePercent;
                pieceRadius = (int)Math.Pow(((3.0 * pieceVolume) / (4.0 * Math.PI)), (1.0 / 3.0));

                do
                {
                    accelVector = new Vector3D(rand.NextDouble() * rand.Next(-1, 2), rand.NextDouble() * rand.Next(-1, 2), rand.NextDouble() * rand.Next(-1, 2));
                } while (accelVector.Length == 0);

                do
                {
                    rotateVector = new Vector3D(rand.NextDouble() * rand.Next(-1, 2), rand.NextDouble() * rand.Next(-1, 2), rand.NextDouble() * rand.Next(-1, 2));
                } while (rotateVector.Length == 0);

                accelVector.Normalize();
                rotateVector.Normalize();

                newPiece = new AsteroidCelestialBody(pieceRadius, ((GeometryModel3D)Model.Children[0]).Material, rotateVector, rand.NextDouble() * (100 - 25) + 25)
                {
                    Name = pieceName,
                    Mass = pieceMass
                };

                newPiece.MoveTo(Location + (accelVector * 100));
                newPiece.Velocity = Velocity;
                newPiece.Accelerate(accelVector * (explosionForce / pieceMass), deltaTime, timeMultiplier);

                pieces.Add(newPiece);
                pieceNumber++;
            }
            return pieces;
        }
        public override void Yaw(double degrees)
        {
            YawRotationAngle = (YawRotationAngle + degrees) % 360.0;
        }
        public override void Pitch(double degrees)
        {
            PitchRotationAngle = (PitchRotationAngle + degrees) % 360.0;
        }
        public override void Roll(double degrees)
        {
            RollRotationAngle = (RollRotationAngle + degrees) % 360.0;
        }
    }
}