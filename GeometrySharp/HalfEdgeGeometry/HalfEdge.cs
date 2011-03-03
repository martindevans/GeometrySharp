using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GeometrySharp.HalfEdgeGeometry
{
    public class HalfEdge
    {
        public Vertex End;
        public readonly HalfEdge Twin;
        public HalfEdge Next;
        public Face Face;

        public readonly Mesh Mesh;
        internal readonly bool Primary;

        protected internal HalfEdge(Mesh m, HalfEdge twin, bool primary)
        {
            Mesh = m;
            Twin = primary ? new HalfEdge(m, this, false) : twin;
            Primary = primary;
        }

        public HalfEdge Split(Vector3 midpoint)
        {
            var a = Twin.End;
            var b = Mesh.GetVertex(midpoint);
            var c = End;

            Twin.End = b;

            var ab = Mesh.GetEdge(a, b, Face, this, Twin.Face, Twin.Next);

            Twin.Next = ab;

            return ab.Primary ? ab : ab.Twin;
        }

        public override string ToString()
        {
            return Twin.End.ToString() + " -> " + End.ToString();
        }
    }
}
