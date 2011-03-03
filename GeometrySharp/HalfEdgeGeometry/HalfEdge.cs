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

        protected internal HalfEdge(Mesh m)
        {
            Mesh = m;
            Twin = new HalfEdge(m, this);
            Primary = true;
        }

        private HalfEdge(Mesh m, HalfEdge twin)
        {
            Mesh = m;
            Twin = twin;
            Primary = false;
        }

        public HalfEdge Split(Vertex midpoint)
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return Twin.End.ToString() + " -> " + End.ToString();
        }
    }
}
