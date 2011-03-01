using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using GeometrySharp.HalfEdgeGeometry;

namespace GeometrySharp.ConstructiveSolidGeometry.Primitives
{
    public class Sphere
        :IPrimitive
    {
        public bool Contains(Vector3 point)
        {
            return point.LengthSquared() < 1;
        }

        public Mesh MakeMesh()
        {
            throw new NotImplementedException();
        }
    }
}
