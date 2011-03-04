using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GeometrySharp.HalfEdgeGeometry
{
    public class HalfEdge
    {
        private Vertex end;
        public Vertex End
        {
            get
            {
                return end;
            }
            set
            {
                Mesh.UpdateIndex(this, value);
                end = value;
            }
        }
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
            var a = Twin.End;
            var m = midpoint;
            var b = End;

            Twin.End = m;
            var bm = Twin;
            var mb = this;

            var am = Mesh.GetEdge(a, m, Face, Face == null ? null : mb, mb.Twin.Face, mb.Twin.Next);
            var ma = am.Twin;

            if (Face != null)
            {
                bm.Next = ma;
                Face.Edges.Where(e => e.Next == this).First().Next = am;
            }

            if (Twin.Face != null)
                Twin.Face.Edges.Where(e => e.Next == Twin).First().Next = bm;

            return am;
        }

        public override string ToString()
        {
            return Twin.End.ToString() + " -> " + End.ToString();
        }
    }
}
