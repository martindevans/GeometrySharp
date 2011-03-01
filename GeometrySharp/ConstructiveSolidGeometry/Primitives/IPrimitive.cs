using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using GeometrySharp.HalfEdgeGeometry;

namespace GeometrySharp.ConstructiveSolidGeometry.Primitives
{
    public interface IPrimitive
    {
        bool Contains(Vector3 point);

        Mesh MakeMesh();
    }
}
