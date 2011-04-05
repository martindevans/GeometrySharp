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

        public HalfEdge Next
        {
            get;
            set;
        }

        public Face Face;

        public readonly Mesh Mesh;
        public readonly bool Primary;

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
            Mesh.InformSplitMidpointBegin(this, midpoint);

            if (midpoint.Equals(End) || midpoint.Equals(Twin.End))
                return this;

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
            {
                Twin.Face.Edges.Where(e => e.Next == Twin).First().Next = bm;
            }

            Mesh.InformSplitMidpointEnd(this, midpoint);

            return am;
        }

        /// <summary>
        /// Merges this halfedge with the Next
        /// </summary>
        public void Merge()
        {
            if (Next == null)
                throw new InvalidOperationException("Next must not be null");
            if (Face == null)
                throw new InvalidOperationException("Face must not be null");
            if (Face != Next.Face)
                throw new InvalidOperationException("Cannot merge two edges bordering different faces");
            if (Twin.Face != null && Twin.Face != Next.Twin.Face)
                throw new InvalidOperationException("Cannot merge two edges with twins bordering different faces");

            HalfEdge oldNext = Next;

            Next = oldNext.Next;
            End = oldNext.End;

            if (Next.Twin.Next == oldNext.Twin)
            {
                Next.Twin.Next = Twin;
                Next.Twin.End = End;
            }

            if (Twin.Face != null)
                Twin.Face.Edges.Where(a => a.Next == oldNext.Twin).First().Next = Twin;

            oldNext.Face = null;
            oldNext.Twin.Face = null;
            oldNext.Next = null;
            oldNext.Twin.Next = null;

            if (Face.Edge == oldNext)
                Face.Edge = this;
            if (Twin.Face != null && Twin.Face.Edge == oldNext.Twin)
                Twin.Face.Edge = Twin;

            Mesh.Delete(oldNext);
        }

        public override string ToString()
        {
            return Twin.End.ToString() + " -> " + End.ToString();
        }

        public void Delete()
        {
            Mesh.Delete(this);
        }
    }
}
