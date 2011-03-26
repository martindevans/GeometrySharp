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

        private static readonly Func<Vector3, string, Mesh, Vertex> defaultFactory = (a, b, c) => new Vertex(a, b, c);

        public static Mesh Icosahedron(Func<Vector3, string, Mesh, Vertex> factory = null)
        {
            Mesh m = new Mesh(factory ?? defaultFactory);

            var vertices = icosahedronVertices.Select((a, i) => m.GetVertex(a, i.ToString())).ToArray();

            for (int i = 0; i < icosahedronIndices.Length; i += 3)
            {
                var a = vertices[icosahedronIndices[i + 2]];
                var b = vertices[icosahedronIndices[i + 1]];
                var c = vertices[icosahedronIndices[i]];

                Face f = m.GetFace(a, b, c);
            }

            return m;
        }
        #endregion

        public static Mesh Sphere(int subdivisions, Func<Vector3, string, Mesh, Vertex> factory = null, Mesh.SubdivideOperation subdivisionOperation = Mesh.SubdivideOperation.InternalFace)
        {
            Mesh m = Icosahedron(factory ?? defaultFactory);

            for (int i = 0; i < subdivisions; i++)
                m.SubdivideAllFaces(subdivisionOperation);

            foreach (var vertex in m.Vertices)
                vertex.Position.Normalize();

            return m;
        }

        /// <summary>
        /// Construct a cuboid graph on the given vertices, winding around { top1, top2, top3, top4 } and then all neighbours accordingly
        /// </summary>
        /// <param name="top1"></param>
        /// <param name="top2"></param>
        /// <param name="top3"></param>
        /// <param name="top4"></param>
        /// <param name="bottom1"></param>
        /// <param name="bottom2"></param>
        /// <param name="bottom3"></param>
        /// <param name="bottom4"></param>
        /// <param name="factory"></param>
        /// <returns></returns>
        public static Mesh Cuboid(Vector3 top1, Vector3 top2, Vector3 top3, Vector3 top4, Vector3 bottom1, Vector3 bottom2, Vector3 bottom3, Vector3 bottom4, Func<Vector3, string, Mesh, Vertex> factory = null)
        {
            Mesh m = new Mesh(factory ?? defaultFactory);

            Vertex t1 = m.GetVertex(top1);
            Vertex t2 = m.GetVertex(top2);
            Vertex t3 = m.GetVertex(top3);
            Vertex t4 = m.GetVertex(top4);

            Vertex b5 = m.GetVertex(bottom1);
            Vertex b6 = m.GetVertex(bottom2);
            Vertex b7 = m.GetVertex(bottom3);
            Vertex b8 = m.GetVertex(bottom4);

            Face t1234  = m.GetFace(t4, t3, t2, t1);
            Face t21b56 = m.GetFace(b6, b5, t1, t2);
            Face t32b67 = m.GetFace(b7, b6, t2, t3);
            Face t43b78 = m.GetFace(b8, b7, t3, t4);
            Face t14b85 = m.GetFace(b5, b8, t4, t1);
            Face b6587  = m.GetFace(b7, b8, b5, b6);

            return m;
        }

        /// <summary>
        /// Creates a unit cuboid
        /// </summary>
        /// <returns></returns>
        public static Mesh Cube(Func<Vector3, string, Mesh, Vertex> factory = null)
        {
            return Cuboid(
                new Vector3(-0.5f, -0.5f, 0.5f),
                new Vector3(0.5f, -0.5f, 0.5f),
                new Vector3(0.5f, 0.5f, 0.5f),
                new Vector3(-0.5f, 0.5f, 0.5f),

                new Vector3(-0.5f, -0.5f, -0.5f),
                new Vector3(0.5f, -0.5f, -0.5f),
                new Vector3(0.5f, 0.5f, -0.5f),
                new Vector3(-0.5f, 0.5f, -0.5f),
                factory
            );
        }

        public static Mesh Cylinder(int segments, int slices, float radius, float height, Func<Vector3, string, Mesh, Vertex> factory = null)
        {
            if (slices < 2)
                throw new ArgumentException("Must be more than 2 slices");

            height /= 2f;

            Mesh m = new Mesh(factory ?? defaultFactory);

            Vertex[][] sliceVerts = new Vertex[slices][];
            for (int i = 0; i < slices; i++)
                sliceVerts[i] = new Vertex[segments];

            float angle = 0;
            float step = MathHelper.TwoPi / (float)segments;
            for (int segment = 0; segment < segments; segment++)
            {
                Vector2 xy = new Vector2((float)Math.Sin(angle), (float)Math.Cos(angle));

                for (int slice = 0; slice < slices; slice++)
                {
                    float l = slice / (float)slices;
                    float z = MathHelper.Lerp(-height, height, l);

                    sliceVerts[slice][segment] = m.GetVertex(new Vector3(xy, z), "slice:" + slice + " seg:" + segment);
                }

                angle += step;
            }

            m.GetFace(sliceVerts[0].Reverse());
            m.GetFace(sliceVerts[slices - 1]);

            for (int segment = 0; segment < segments; segment++)
            {
                for (int slice = 0; slice < slices - 1; slice++)
                {
                    m.GetFace(
                        sliceVerts[slice][segment],
                        sliceVerts[slice][(segment + 1) % segments],
                        sliceVerts[slice + 1][(segment + 1) % segments],
                        sliceVerts[slice + 1][segment]
                    );
                }
            }

            return m;
        }
    }
}
