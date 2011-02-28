using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometrySharp.HalfEdgeGeometry
{
    public class HalfEdge
    {
        public Vertex End;
        public HalfEdge Twin;
        public HalfEdge Next;
        public Face Face;
    }
}
