using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using GeometrySharp.HalfEdgeGeometry;

namespace GeometrySharp.ConstructiveSolidGeometry.Primitives
{
    public class Cube
        :CsgNode
    {
        public override bool Contains(Vector3 point)
        {
            return point.X < 0.5f && point.X > -0.5f
                && point.Y < 0.5f && point.Y > -0.5f
                && point.Z < 0.5f && point.Z > -0.5f;
        }

        public override Mesh MakeMesh()
        {
            IsDirty = false;
            return PrimitiveShapes.Cube();
        }
    }
}
