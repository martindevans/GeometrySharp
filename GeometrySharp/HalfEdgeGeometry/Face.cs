using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometrySharp.HalfEdgeGeometry
{
    public class Face
    {
        public HalfEdge Edge;

        public IEnumerable<HalfEdge> Edges
        {
            get
            {
                HalfEdge e = Edge;

                do
                {
                    yield return e;
                    e = e.Next;
                } while (e != null && e != Edge);
            }
        }

        public IEnumerable<Vertex> Vertices
        {
            get
            {
                foreach (var edge in Edges)
                    yield return edge.End;
            }
        }

        public IEnumerable<Face> Neighbours
        {
            get
            {
                foreach (var f in Edges.Select(e => e.Twin.Face).Where(f => f != null).Distinct())
                    yield return f;
            }
        }

        public readonly Mesh Mesh;

        protected internal Face(Mesh m)
        {
            Mesh = m;
        }

        public void Delete()
        {
            Mesh.Delete(this);
        }
    }
}
