using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Collections.Concurrent;

namespace GeometrySharp.HalfEdgeGeometry
{
    public class Mesh
    {
        #region fields
        public const float BUCKET_SIZE = 0.01f;

        public IEnumerable<HalfEdge> HalfEdges
        {
            get
            {
                throw new NotImplementedException();
            }
        }
        #endregion

        public Mesh()
            : this(a => new Vertex(a))
        {

        }

        public Mesh(Func<Vector3, Vertex> vertexFactory)
        {

        }

        #region vertices
        ConcurrentDictionary<float, ConcurrentDictionary<float, ConcurrentDictionary<float, Vertex>>> vertices = new ConcurrentDictionary<float, ConcurrentDictionary<float, ConcurrentDictionary<float, Vertex>>>();
        public IEnumerable<Vertex> Vertices
        {
            get
            {
                foreach (var xD in vertices.Values)
                    foreach (var yD in xD.Values)
                        foreach (var v in yD.Values)
                            yield return v;
            }
        }

        public Vertex GetVertex(Vector3 pos)
        {
            float x = pos.X.Bucketise(BUCKET_SIZE);
            float y = pos.X.Bucketise(BUCKET_SIZE);
            float z = pos.X.Bucketise(BUCKET_SIZE);

            var xD = vertices.GetOrAdd(x, a => new ConcurrentDictionary<float, ConcurrentDictionary<float, Vertex>>());
            var yD = xD.GetOrAdd(y, a => new ConcurrentDictionary<float, Vertex>());
            return yD.GetOrAdd(z, a => new Vertex(new Vector3(x, y, z)));
        }
        #endregion
        Dictionary<Vertex, List<HalfEdge>> edges = new Dictionary<Vertex, List<HalfEdge>>();
        public HalfEdge GetEdge(Vertex a, Vertex b, Face f, HalfEdge abNext)
        {
            List<HalfEdge> appropriateEdges;
            if (edges.ContainsKey(b))
            {
                appropriateEdges = edges[b];
            }
            else
            {
                appropriateEdges = new List<HalfEdge>();
                edges[b] = appropriateEdges;
            }

            var query = appropriateEdges.Where(x => x.Twin.End == a);
            switch (query.Count())
            {
                case 0:
                    {
                        HalfEdge edge = new HalfEdge(this, null, true);
                        HalfEdge twin = edge.Twin;
                        edge.End = b;
                        twin.End = a;
                        edge.Face = f;
                        edge.Next = abNext;
                        return edge;
                    }
                case 1:
                    {
                        var edge = query.First();

                        if (abNext != null && edge.Next != abNext && edge.Next != null)
                            throw new ArgumentException("Edge already attached to edge " + edge.Next);

                        if (f != null && edge.Face != f && edge.Face != null)
                            throw new ArgumentException("Edge already attached to face " + edge.Face);

                        edge.Face = f ?? edge.Face;
                        edge.Next = abNext ?? edge.Next;
                        return edge;
                    }
                default:
                    throw new MeshMalformedException("More than one edge already exists between " + a + " and " + b);
            }

        }

        public HalfEdge GetEdge(Vertex a, Vertex b, Face abf, HalfEdge abNext, Face baf, HalfEdge baNext)
        {
            throw new NotImplementedException();
        }

        ConcurrentDictionary<Vertex, ConcurrentDictionary<Face, bool>> faces = new ConcurrentDictionary<Vertex, ConcurrentDictionary<Face, bool>>();
        public IEnumerable<Face> Faces
        {
            get
            {
                foreach (var set in faces.Values)
                    foreach (var face in set.Keys)
                        yield return face;
            }
        }

        public Face GetFace(params Vertex[] vertices)
        {
            return GetFace(vertices as IEnumerable<Vertex>);
        }

        public Face GetFace(IEnumerable<Vertex> vertices)
        {
            //throw new NotImplementedException("Check if this face already exists");
            //throw new NotImplementedException("Check if this face would conflict with an already existing face");

            List<HalfEdge> edges = new List<HalfEdge>();
            Face f = new Face();

            Vertex first = null;
            Vertex previous = null;
            foreach (var v in vertices)
            {
                if (first == null)
                    first = v;
                else
                    edges.Add(GetEdge(previous, v, f, null));

                previous = v;
            }

            edges.Add(GetEdge(previous, first, f, null));

            for (int i = 0; i < edges.Count; i++)
            {
                var next = edges[(i + 1) % edges.Count];

                if (edges[i].Next != null && edges[i].Next != next)
                    throw new InvalidOperationException("Cannot link edges, conflicting link already exists");

                edges[i].Next = edges[(i + 1) % edges.Count];
            }

            foreach (var vertex in vertices)
            {
                var set = faces.GetOrAdd(vertex, a => new ConcurrentDictionary<Face, bool>());
                set.AddOrUpdate(f, true, (a, b) => { throw new InvalidOperationException(); });
            }

            return f;
        }
    }
}
