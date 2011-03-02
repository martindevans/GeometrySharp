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
            float y = pos.Y.Bucketise(BUCKET_SIZE);
            float z = pos.Z.Bucketise(BUCKET_SIZE);

            var xD = vertices.GetOrAdd(x, a => new ConcurrentDictionary<float, ConcurrentDictionary<float, Vertex>>());
            var yD = xD.GetOrAdd(y, a => new ConcurrentDictionary<float, Vertex>());
            return yD.GetOrAdd(z, a => new Vertex(new Vector3(x, y, z)));
        }
        #endregion

        Dictionary<Vertex, List<HalfEdge>> edges = new Dictionary<Vertex, List<HalfEdge>>();

        public HalfEdge GetEdge(Vertex a, Vertex b)
        {
            return GetEdge(a, b, null, null);
        }

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

        public Face GetFace(IEnumerable<Vertex> vertices)
        {
            return GetFace(vertices.ToArray());
        }

        public Face GetFace(params Vertex[] vertices)
        {
            CheckGetFaceParameters(vertices);

            Face existingFace = FindExistingFaces(vertices);
            if (existingFace != null)
            {
                CheckPotentiallyConflictingFace(vertices, existingFace);
                return existingFace;
            }

            return CreateNewFace(vertices);
        }

        private Face CreateNewFace(Vertex[] vertices)
        {
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

            f.Edge = edges[0];

            foreach (var vertex in vertices)
            {
                var set = faces.GetOrAdd(vertex, a => new ConcurrentDictionary<Face, bool>());
                set.AddOrUpdate(f, true, (a, b) => { throw new InvalidOperationException(); });
            }

            return f;
        }

        private static void CheckPotentiallyConflictingFace(Vertex[] vertices, Face existingFace)
        {
            var firstIndex = Array.IndexOf(vertices, existingFace.Vertices.First());
            int iteration = 0;
            foreach (var v in existingFace.Vertices)
            {
                if (v != vertices[(iteration + firstIndex) % vertices.Length])
                    throw new InvalidOperationException("A conflicting face already exists");

                iteration++;
            }
        }

        private Face FindExistingFaces(IEnumerable<Vertex> vertices)
        {
            HashSet<Face> existingFaces = new HashSet<Face>();
            bool firstCheck = true;
            int iterations = 0;
            foreach (var v in vertices)
            {
                var keys = faces.GetOrAdd(v, k => new ConcurrentDictionary<Face, bool>()).Keys;

                if (iterations == 1)
                    existingFaces.IntersectWith(keys);
                else
                {
                    firstCheck = false;
                    existingFaces.UnionWith(keys);
                }

                iterations++;
            }

            if (existingFaces.Count > 1)
                throw new MeshMalformedException("Multiple faces connecting this set of vertices together");

            return existingFaces.FirstOrDefault();
        }

        private static void CheckGetFaceParameters(IEnumerable<Vertex> vertices)
        {
            HashSet<Vertex> s = new HashSet<Vertex>();
            foreach (var v in vertices)
                if (!s.Add(v))
                    throw new ArgumentException("Input set contains duplicate vertices");

            if (s.Count < 3)
                throw new ArgumentException("Face must have 3 or more vertices");
        }
    }
}
