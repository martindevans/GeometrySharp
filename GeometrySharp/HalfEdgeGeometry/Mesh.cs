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
        public const float BUCKET_SIZE = 0;
        #endregion

        #region constructors
        public Mesh(Func<Vector3, string, Mesh, Vertex> vertexFactory = null)
        {
            this.vertexFactory = vertexFactory ?? ((a, b, c) => new Vertex(a, b, c));
        }
        #endregion

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

        private readonly Func<Vector3, string, Mesh, Vertex> vertexFactory;

        public Vertex GetVertex(Vector3 pos, String name = "")
        {
            float x = pos.X.Bucketise(BUCKET_SIZE);
            float y = pos.Y.Bucketise(BUCKET_SIZE);
            float z = pos.Z.Bucketise(BUCKET_SIZE);

            var xD = vertices.GetOrAdd(x, a => new ConcurrentDictionary<float, ConcurrentDictionary<float, Vertex>>());
            var yD = xD.GetOrAdd(y, a => new ConcurrentDictionary<float, Vertex>());
            return yD.GetOrAdd(z, a => vertexFactory(new Vector3(x, y, z), name, this));
        }

        internal IEnumerable<HalfEdge> VertexIncoming(Vertex vertex)
        {
            return edges.GetOrAdd(vertex, a => new HashSet<HalfEdge>());
        }

        internal IEnumerable<Face> BorderingFaces(Vertex vertex)
        {
            return faces[vertex].Keys;
        }
        #endregion

        #region edges
        ConcurrentDictionary<Vertex, HashSet<HalfEdge>> edges = new ConcurrentDictionary<Vertex, HashSet<HalfEdge>>();
        public IEnumerable<HalfEdge> HalfEdges
        {
            get
            {
                foreach (var l in edges.Values)
                    foreach (var edge in l)
                        yield return edge;
            }
        }

        public HalfEdge GetEdge(Vertex a, Vertex b)
        {
            return GetEdge(a, b, null, null);
        }

        public HalfEdge GetEdge(Vertex a, Vertex b, Face f, HalfEdge abNext)
        {
            return GetEdge(a, b, f, abNext, null, null);
        }

        public HalfEdge GetEdge(Vertex a, Vertex b, Face abf, HalfEdge abNext, Face baf, HalfEdge baNext)
        {
            HashSet<HalfEdge> edgesEndingAtB = edges.GetOrAdd(b, k => new HashSet<HalfEdge>());

            var query = edgesEndingAtB.Where(x => x.Twin.End == a);
            switch (query.Count())
            {
                case 0:
                    {
                        HalfEdge edge = new HalfEdge(this);
                        HalfEdge twin = edge.Twin;

                        edge.End = b;
                        twin.End = a;

                        edge.Face = abf;
                        twin.Face = baf;

                        edge.Next = abNext;
                        twin.Next = baNext;

                        return edge;
                    }
                case 1:
                    {
                        var edge = query.First();

                        if (abNext != null && edge.Next != abNext && edge.Next != null)
                            throw new ArgumentException("Edge already attached to edge " + edge.Next);

                        if (baNext != null && edge.Twin.Next != baNext && edge.Twin.Next != null)
                            throw new ArgumentException("Twin edge already attached to edge " + edge.Twin.Next);

                        if (abf != null && edge.Face != abf && edge.Face != null)
                            throw new ArgumentException("Edge already attached to face " + edge.Face);

                        if (baf != null && edge.Twin.Face != baf && edge.Twin.Face != null)
                            throw new ArgumentException("Twin edge already attached to face " + edge.Twin.Face);

                        edge.Face = abf ?? edge.Face;
                        edge.Next = abNext ?? edge.Next;

                        edge.Twin.Face = baf ?? edge.Twin.Face;
                        edge.Twin.Next = baNext ?? edge.Twin.Next;
                        return edge;
                    }
                default:
                    throw new MeshMalformedException("More than one edge already exists between " + a + " and " + b);
            }
        }

        internal void UpdateIndex(HalfEdge halfEdge, Vertex newEndValue)
        {
            if (newEndValue == null)
                throw new ArgumentException("Cannot set end to null");

            if (halfEdge.End != null)
                edges.GetOrAdd(halfEdge.End, a => new HashSet<HalfEdge>()).Remove(halfEdge);

            edges.GetOrAdd(newEndValue, a => new HashSet<HalfEdge>()).Add(halfEdge);
        }
        #endregion

        #region faces
        ConcurrentDictionary<Vertex, ConcurrentDictionary<Face, bool>> faces = new ConcurrentDictionary<Vertex, ConcurrentDictionary<Face, bool>>();
        public IEnumerable<Face> Faces
        {
            get
            {
                return faces.SelectMany(a => a.Value.Keys).GroupBy(a => a).Select(a => a.First());
            }
        }

        public Face GetFace(IEnumerable<Vertex> vertices)
        {
            return GetFace(vertices.ToArray());
        }

        public Face GetFace(params Vertex[] vertices)
        {
            CheckGetFaceParameters(vertices);

            Face existingFace = FindExistingFace(vertices);
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
            Face f = new Face(this);

            BuildEdgeList(vertices, edges, f);

            ConnectEdges(edges);

            f.Edge = edges[0];

            foreach (var vertex in vertices)
            {
                var set = faces.GetOrAdd(vertex, a => new ConcurrentDictionary<Face, bool>());
                set.AddOrUpdate(f, true, (a, b) => { throw new InvalidOperationException(); });
            }

            return f;
        }

        private static void ConnectEdges(List<HalfEdge> edges)
        {
            for (int i = 0; i < edges.Count; i++)
            {
                var next = edges[(i + 1) % edges.Count];

                if (edges[i].Next != null && edges[i].Next != next)
                    throw new InvalidOperationException("Cannot link edges, conflicting link already exists");
            }

            for (int i = 0; i < edges.Count; i++)
            {
                var next = edges[(i + 1) % edges.Count];

                edges[i].Next = edges[(i + 1) % edges.Count];
            }
        }

        private void BuildEdgeList(Vertex[] vertices, List<HalfEdge> edges, Face f)
        {
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

        private Face FindExistingFace(IEnumerable<Vertex> vertices)
        {
            HashSet<Face> existingFaces = new HashSet<Face>();
            int iterations = 0;
            foreach (var v in vertices)
            {
                var keys = faces.GetOrAdd(v, k => new ConcurrentDictionary<Face, bool>()).Keys;

                if (iterations == 0)
                    existingFaces.UnionWith(keys);
                else
                    existingFaces.IntersectWith(keys);

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

        public void Delete(Face f)
        {
            foreach (var vertex in f.Vertices)
            {
                bool b;
                if (!faces[vertex].TryRemove(f, out b))
                    throw new MeshMalformedException("Face was not indexed with an associated vertex");
            }

            var edges = f.Edges.ToArray();
            for (int i = 0; i < edges.Length; i++)
            {
                edges[i].Face = null;
                edges[i].Next = null;
            }
        }

        internal void UpdateIndex(Vertex vertex, Face face)
        {
            faces.GetOrAdd(vertex, a => new ConcurrentDictionary<Face, bool>()).AddOrUpdate(face, true, (a, b) => true);
        }
        #endregion

        #region global mutations
        private void SubdivideAllFacesWithMidpoint()
        {
            var faces = Faces.ToArray();

            foreach (var face in faces)
            {
                Vector3 mid = Vector3.Zero;
                float count = 0;
                foreach (var v in face.Vertices)
                {
                    mid += v.Position;
                    count++;
                }

                face.InsertMidpoint(GetVertex(mid / count));
            }
        }

        private void SubdivideAllFacesWithInternalFace()
        {
            var edges = new HashSet<HalfEdge>(HalfEdges.Where(a => a.Primary));
            var newVertices = new HashSet<Vertex>();

            foreach (var edge in edges)
                newVertices.Add(edge.Split(GetVertex(edge.End.Position * 0.5f + edge.Twin.End.Position * 0.5f)).End);

            var faces = Faces.ToArray();

            foreach (var face in faces)
            {
                var verts = face.Vertices.ToArray();
                face.Delete();

                GetFace(verts.Where(a => newVertices.Contains(a)));

                for (int i = 0; i < verts.Length; i++)
                {
                    if (newVertices.Contains(verts[i]))
                    {
                        GetFace(
                            verts[i],
                            verts[(i + 1) % verts.Length],
                            verts[(i + 2) % verts.Length]
                        );
                    }
                }
            }
        }

        private void TriangulateAllFaces()
        {
            throw new NotImplementedException();
        }

        public void SubdivideAllFaces(SubdivideOperation op)
        {
            switch (op)
            {
                case SubdivideOperation.Midpoint:
                    SubdivideAllFacesWithMidpoint();
                    break;
                case SubdivideOperation.InternalFace:
                    SubdivideAllFacesWithInternalFace();
                    break;
                case SubdivideOperation.Triangulate:
                    TriangulateAllFaces();
                    break;
                default:
                    break;
            }
        }

        public enum SubdivideOperation
        {
            Midpoint,
            InternalFace,
            Triangulate
        }
        #endregion        
    }
}
