using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometrySharp.HalfEdgeGeometry;
using Microsoft.Xna.Framework;

namespace GeometrySharp
{
    public class CsgVertex
        :Vertex
    {
        public ContainmentType Classification;

        public CsgVertex(Vector3 a, string b, Mesh c)
            :base(a, b, c)
        {

        }
    }
}
