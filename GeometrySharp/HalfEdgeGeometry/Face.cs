using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometrySharp.HalfEdgeGeometry
{
    public class Face
    {
        public readonly Mesh Mesh;
        public HalfEdge Edge;

        #region enumerators
        public IEnumerable<HalfEdge> Edges
        {
            get
            {
                HalfEdge e = Edge;

                do
                {
                    yield return e;
                    e = e.Next;
                } while (e != Edge);
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

        public IEnumerable<Vertex> TriangulateFromSinglePoint
        {
            get
            {
                var verts = Vertices.ToArray();

                for (int i = 2; i < verts.Length; i++)
                {
                    yield return verts[0];
                    yield return verts[i - 1];
                    yield return verts[i];
                }
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
        #endregion

        protected internal Face(Mesh m)
        {
            Mesh = m;
        }

        /// <summary>
        /// Deletes this instance.
        /// </summary>
        public void Delete()
        {
            Mesh.Delete(this);
        }

        /// <summary>
        /// Inserts a midpoint into this face
        /// Deletes this face
        /// </summary>
        /// <param name="midpoint">The midpoint.</param>
        public void InsertMidpoint(Vertex midpoint)
        {
            Mesh.InformSplitMidpointBegin(this, midpoint);

            var edges = Edges.ToArray();

            Delete();

            for (int i = 0; i < edges.Length; i++)
            {
                var v1 = edges[i].End;
                var v2 = edges[(i + 1) % edges.Length].End;
                Mesh.GetFace(v1, v2, midpoint);
            }

            Mesh.InformSplitMidpointEnd(this, midpoint);
        }
    }
}
