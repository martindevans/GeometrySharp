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
        public const float BUCKET_SIZE = 0.01f;

        public IEnumerable<HalfEdge> HalfEdges
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public IEnumerable<Face> Faces
        {
            get
            {
                throw new NotImplementedException();
            }
        }

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

        public Mesh()
            :this(a => new Vertex(a))
        {

        }

        public Mesh(Func<Vector3, Vertex> vertexFactory)
        {

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

        public HalfEdge GetEdge(Vertex a, Vertex b, Face f, HalfEdge abNext)
        {
            throw new NotImplementedException();
        }

        public HalfEdge GetEdge(Vertex a, Vertex b, Face abf, HalfEdge abNext, Face baf, HalfEdge baNext)
        {
            throw new NotImplementedException();
        }

        public Face GetFace(params Vertex[] vertices)
        {
            return GetFace(vertices as IEnumerable<Vertex>);
        }

        public Face GetFace(IEnumerable<Vertex> vertices)
        {
            throw new NotImplementedException();
        }
    }
}
