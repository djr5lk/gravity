using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Gravity
{
    // Sphere3D implementation modified from http://www.codeproject.com/Articles/14414/Generating-a-sphere-mesh-in-XAML by r.stropek
    public abstract class RoundMesh3D
    {
        protected int n = 10;
        protected int r = 20;
        protected Point3DCollection points;
        protected Int32Collection triangleIndices;
        protected PointCollection textureCoordinates;

        public virtual int Radius
        {
            get { return r; }
            set { r = value; CalculateGeometry(); }
        }

        public virtual int Separators
        {
            get { return n; }
            set { n = value; CalculateGeometry(); }
        }

        public Point3DCollection Points
        {
            get { return points; }
        }

        public Int32Collection TriangleIndices
        {
            get { return triangleIndices; }
        }

        public PointCollection TextureCoordinates
        {
            get { return textureCoordinates; }
        }

        protected abstract void CalculateGeometry();
    }

    public class DiscGeometry3D : RoundMesh3D
    {
        protected int r2 = 0;
        public virtual int InnerRadius
        {
            get { return r2; }
            set { r2 = value; CalculateGeometry(); }
        }

        protected override void CalculateGeometry()
        {
            int numberOfSeparators = 4 * n + 4;
            double segmentRad = Math.PI / 2 / (n + 1);

            points = new Point3DCollection(numberOfSeparators + 1);
            triangleIndices = new Int32Collection((numberOfSeparators + 1) * 3);
            textureCoordinates = new PointCollection(numberOfSeparators + 1);

            for (int s = 0; s <= numberOfSeparators; s++)               // Create outer points
            {
                points.Add(new Point3D(r * Math.Cos(segmentRad * s), 0, -1 * r * Math.Sin(segmentRad * s)));
                textureCoordinates.Add(new Point(((double)s / (double)numberOfSeparators), 1));
            }

            for (int s = 0; s <= numberOfSeparators; s++)               // Create inner points
            {
                points.Add(new Point3D(r2 * Math.Cos(segmentRad * s), 0, -1 * r2 * Math.Sin(segmentRad * s)));
                textureCoordinates.Add(new Point(((double)s / (double)numberOfSeparators), 0));
            }

            for (int i = 0; i <= numberOfSeparators; i++)           // Create two triangles for each rectangular segment
            {
                triangleIndices.Add(i);
                triangleIndices.Add((numberOfSeparators + 1) + (i + 1));
                triangleIndices.Add((numberOfSeparators + 1) + i);

                triangleIndices.Add((numberOfSeparators + 1) + (i + 1));
                triangleIndices.Add(i);
                triangleIndices.Add(i + 1);
            }
        }

        public DiscGeometry3D()
        {
            CalculateGeometry();
        }

        public DiscGeometry3D(int radius)
        {
            r = radius;
            CalculateGeometry();
        }

        public DiscGeometry3D(int radius, int innerRadius)
        {
            r = radius;
            r2 = innerRadius;
            CalculateGeometry();
        }

        public DiscGeometry3D(int radius, int innerRadius, int numSeparators)
        {
            r = radius;
            r2 = innerRadius;
            n = numSeparators;
            CalculateGeometry();
        }
    }

    public class Sphere3D : RoundMesh3D
    {
        protected override void CalculateGeometry()
        {
            int e;
            double segmentRad = Math.PI / 2 / (n + 1);
            int numberOfSeparators = 4 * n + 4;

            points = new Point3DCollection();
            triangleIndices = new Int32Collection();
            textureCoordinates = new PointCollection();

            for (int s = 0; s <= numberOfSeparators; s++)               // Create multiple points at lower pole to help texture wrap better
            {
                points.Add(new Point3D(0, -1 * r, 0));
                textureCoordinates.Add(new Point(((double)s / (double)numberOfSeparators), 1));
            }

            for (e = -n; e <= n; e++)                                   // Build all non-pole points of sphere from bottom up
            {
                double r_e = r * Math.Cos(segmentRad * e);
                double y_e = r * Math.Sin(segmentRad * e);

                for (int s = 0; s <= numberOfSeparators; s++)
                {
                    double z_s = r_e * Math.Sin(segmentRad * s) * (-1);
                    double x_s = r_e * Math.Cos(segmentRad * s);
                    points.Add(new Point3D(x_s, y_e, z_s));
                    textureCoordinates.Add(new Point(((double)s / (double)numberOfSeparators), ((-(double)e / (2 * (double)n + 2)) + 0.5)));
                }
            }

            for (int s = 0; s <= numberOfSeparators; s++)               // Create multiple points at upper pole to help texture wrap better
            {
                points.Add(new Point3D(0, r, 0));
                textureCoordinates.Add(new Point(((double)s / (double)numberOfSeparators), 0));
            }

            for (int i = 0; i <= numberOfSeparators; i++)                // Create triangles for bottom pole
            {
                triangleIndices.Add((numberOfSeparators + 1) + i);
                triangleIndices.Add(i);
                triangleIndices.Add((numberOfSeparators + 1) + (i + 1));
            }

            for (e = 0; e < (2 * n); e++)                               // Create two triangles for each rectangular segment within the non-pole portion of sphere
            {
                for (int i = 0; i <= numberOfSeparators; i++)
                {
                    triangleIndices.Add(((e + 1) * (numberOfSeparators + 1)) + i);
                    triangleIndices.Add(((e + 2) * (numberOfSeparators + 1)) + (i + 1));
                    triangleIndices.Add(((e + 2) * (numberOfSeparators + 1)) + i);

                    triangleIndices.Add(((e + 2) * (numberOfSeparators + 1)) + (i + 1));
                    triangleIndices.Add(((e + 1) * (numberOfSeparators + 1)) + i);
                    triangleIndices.Add(((e + 1) * (numberOfSeparators + 1)) + (i + 1));
                }
            }

            for (int i = 0; i <= numberOfSeparators; i++)            // Create triangles for top pole
            {
                triangleIndices.Add((numberOfSeparators + 1) + ((2 * n) * (numberOfSeparators + 1)) + i);
                triangleIndices.Add((numberOfSeparators + 1) + ((2 * n) * (numberOfSeparators + 1)) + (i + 1));
                triangleIndices.Add((numberOfSeparators + 1) + ((2 * n) + 1) * (numberOfSeparators + 1) + i);
            }
        }

        public Sphere3D()
        {
            CalculateGeometry();
        }

        public Sphere3D(int radius)
        {
            r = radius;
            CalculateGeometry();
        }

        public Sphere3D(int radius, int numSeparators)
        {
            r = radius;
            n = numSeparators;
            CalculateGeometry();
        }
    }
}
