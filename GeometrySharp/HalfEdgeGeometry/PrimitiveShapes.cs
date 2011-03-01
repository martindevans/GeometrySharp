using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GeometrySharp.HalfEdgeGeometry
{
    public static class PrimitiveShapes
    {
        #region icosahedron
        static readonly Vector3[] icosahedronVertices = new Vector3[] {
                new Vector3(-0.52573067f, 0, 0.850651082f),
                new Vector3(0.52573067f, 0, 0.850651082f),
                new Vector3(-0.52573067f, 0, -0.850651082f),
                new Vector3(0.52573067f, 0, -0.850651082f),
                new Vector3(0, 0.850651082f, 0.52573067f),
                new Vector3(0, 0.850651082f, -0.52573067f),
                new Vector3(0, -0.850651082f,0.52573067f),
                new Vector3(0, -0.850651082f, -0.52573067f),
                new Vector3(0.850651082f, 0.52573067f,0),
                new Vector3(-0.850651082f, 0.52573067f,0),
                new Vector3(0.850651082f, -0.52573067f,0),
                new Vector3(-0.850651082f, -0.52573067f,0)};
        static readonly int[] icosahedronIndices = new int[] {
                0, 6, 1,
                0, 11, 6,
                1, 4, 0,
                1, 8, 4,
                1, 10, 8,
                2, 5, 3,
                2, 9, 5,
                2, 11, 9,
                3, 7, 2,
                3, 10, 7,
                4, 8, 5,
                4, 9, 0,
                5, 8, 3,
                5, 9, 4,
                6, 10, 1,
                6, 11, 7,
                7, 10, 6,
                7, 11, 2,
                8, 10, 3,
                9, 11, 0};

        private static readonly Func<Vector3, Vertex> defaultFactory = a => new Vertex(a);

        public static Mesh Icosahedron(Func<Vector3, Vertex> factory = null)
        {
            Mesh m = new Mesh(factory ?? defaultFactory);

            var vertices = icosahedronVertices.Select(a => m.GetVertex(a)).ToArray();

            for (int i = 0; i < icosahedronIndices.Length; i += 3)
            {
                var a = vertices[icosahedronIndices[i]];
                var b = vertices[icosahedronIndices[i + 1]];
                var c = vertices[icosahedronIndices[i + 2]];

                Face f = m.GetFace(a, b, c);
            }

            return m;
        }
        #endregion

        public static Mesh Sphere(int subdivisions, Func<Vector3, Vertex> factory = null)
        {
            Mesh m = Icosahedron(factory);

            for (int i = 0; i < subdivisions; i++)
                m = SubdivideInvertedFace(m);

            return m;
        }

        /// <summary>
        /// Splits every face around a point inserted in the center of the face
        /// </summary>
        /// <param name="m">The mesh to subdivide</param>
        /// <returns>the mesh</returns>
        public static Mesh SubdivideMidpoint(Mesh m)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Splits all the edges of each face, and connects each of the new vertices together
        /// </summary>
        /// <param name="m">The m.</param>
        /// <returns></returns>
        public static Mesh SubdivideInvertedFace(Mesh m)
        {
            var edges = new HashSet<HalfEdge>(m.HalfEdges.Where(a => a.Primary));
            foreach (var edge in edges)
                edge.Split(edge.End.Position * 0.5f + edge.Twin.End.Position * 0.5f);

            var faces = m.Faces.ToArray();
            List<HalfEdge> faceEdgesNewVertices = new List<HalfEdge>();
            Queue<Vertex> outerTriangles = new Queue<Vertex>();
            foreach (var face in faces)
            {
                faceEdgesNewVertices.Clear();
                outerTriangles.Clear();

                //when edges are split, the original edge is left pointing at the new vertex inserted in the middle
                //build a list of all the original edges in this face, which means the end vertices of these edges are the new vertices
                foreach (var edge in face.Edges)
                {
                    if (edges.Contains(edge)) //this edge ends in a new vertex
                        faceEdgesNewVertices.Add(edge);
                    else //this edge ends in an old vertex, find the next and previous vertex and queue them up to become a triangle
                    {
                        outerTriangles.Enqueue(edge.End);
                        outerTriangles.Enqueue(edge.Next.End);
                        outerTriangles.Enqueue(edge.Twin.End);
                    }
                }

                if (outerTriangles.Count % 3 != 0)
                    throw new InvalidOperationException("Queue length not divisible by three, not possible to construct triangles from this!");

                face.Delete();

                var central = m.GetFace(faceEdgesNewVertices.Select(e => e.End));

                while (outerTriangles.Count > 0)
                {
                    var a = outerTriangles.Dequeue();
                    var b = outerTriangles.Dequeue();
                    var c = outerTriangles.Dequeue();

                    m.GetFace(a, b, c);
                }
            }

            throw new NotImplementedException();
        }
    }
}
