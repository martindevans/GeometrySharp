using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using GeometrySharp.HalfEdgeGeometry;

namespace GeometrySharp.ConstructiveSolidGeometry.Primitives
{
    public class Sphere
        :CsgNode
    {
        private int subdivisions;
        public int Subdivisions
        {
            get
            {
                return subdivisions;
            }
            set
            {
                if (value != subdivisions)
                {
                    subdivisions = value;
                    IsDirty = true;
                }
            }
        }

        public Sphere(int subdivisions)
        {
            Subdivisions = subdivisions;
        }

        public override bool Contains(Vector3 point)
        {
            return point.LengthSquared() < 1;
        }

        public override Mesh MakeMesh()
        {
            IsDirty = false;
            return PrimitiveShapes.Sphere(Subdivisions, (a, b, c) => new CsgVertex(a, b, c));
        }
    }
}
