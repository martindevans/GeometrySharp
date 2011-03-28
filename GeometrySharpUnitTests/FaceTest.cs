using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GeometrySharp.HalfEdgeGeometry;
using Microsoft.Xna.Framework;

namespace GeometrySharpUnitTests
{
    [TestClass]
    public class FaceTest
    {
        [TestMethod]
        public void EdgesEnumerator()
        {
            Mesh m = new Mesh();

            var a = m.GetVertex(new Vector3(1, 0, 0));
            var b = m.GetVertex(new Vector3(2, 0, 0));
            var c = m.GetVertex(new Vector3(3, 0, 0));

            var f = m.GetFace(a, b, c);

            var edges = f.Edges.ToArray();
            var abc = m.GetEdge(a, b) == edges[0] && m.GetEdge(b, c) == edges[1] && m.GetEdge(c, a) == edges[2];
            var bca = m.GetEdge(a, b) == edges[1] && m.GetEdge(b, c) == edges[2] && m.GetEdge(c, a) == edges[0];
            var cab = m.GetEdge(a, b) == edges[2] && m.GetEdge(b, c) == edges[0] && m.GetEdge(c, a) == edges[1];

            Assert.IsTrue(abc ^ bca ^ cab);
        }

        [TestMethod]
        public void VerticesEnumerator()
        {
            Mesh m = new Mesh();

            var a = m.GetVertex(new Vector3(1, 0, 0));
            var b = m.GetVertex(new Vector3(2, 0, 0));
            var c = m.GetVertex(new Vector3(3, 0, 0));

            var f = m.GetFace(a, b, c);

            var vertices = f.Vertices.ToArray();
            var abc = vertices[0] == a && vertices[1] == b && vertices[2] == c;
            var bca = vertices[1] == a && vertices[2] == b && vertices[0] == c;
            var cab = vertices[2] == a && vertices[0] == b && vertices[1] == c;

            Assert.IsTrue(abc ^ bca ^ cab);
        }

        [TestMethod]
        public void FacesEnumerator()
        {
            Mesh m = new Mesh();

            var a = m.GetVertex(new Vector3(1, 0, 0));
            var abp = m.GetVertex(new Vector3(0, 1, 0));
            var b = m.GetVertex(new Vector3(2, 0, 0));
            var bcp = m.GetVertex(new Vector3(0, 2, 0));
            var c = m.GetVertex(new Vector3(3, 0, 0));
            var cap = m.GetVertex(new Vector3(0, 3, 0));

            var f = m.GetFace(a, b, c);
            var ab = m.GetFace(a, abp, b);
            var bc = m.GetFace(b, bcp, c);
            var ca = m.GetFace(c, cap, a);

            var faces = f.Neighbours.ToArray();
            var abc = faces[0] == ab && faces[1] == bc && faces[2] == ca;
            var bca = faces[1] == ab && faces[2] == bc && faces[0] == ca;
            var cab = faces[2] == ab && faces[0] == bc && faces[1] == ca;

            Assert.IsTrue(abc ^ bca ^ cab);
        }

        [TestMethod]
        public void DeleteLoneFace()
        {
            Mesh m = new Mesh();

            var a = m.GetVertex(new Vector3(1, 0, 0));
            var b = m.GetVertex(new Vector3(2, 0, 0));
            var c = m.GetVertex(new Vector3(3, 0, 0));

            var f = m.GetFace(a, b, c);

            f.Delete();

            foreach (var edge in m.HalfEdges)
            {
                Assert.IsNull(edge.Face);
                Assert.IsNull(edge.Next);
                Assert.IsNull(edge.Twin.Face);
                Assert.IsNull(edge.Twin.Next);
            }

            Assert.AreEqual(0, m.Faces.Count());
        }

        [TestMethod]
        public void DeleteAndReplaceSurroundedFace()
        {
            Mesh m = new Mesh();

            var a = m.GetVertex(new Vector3(1, 0, 0));
            var abp = m.GetVertex(new Vector3(0, 1, 0));
            var b = m.GetVertex(new Vector3(2, 0, 0));
            var bcp = m.GetVertex(new Vector3(0, 2, 0));
            var c = m.GetVertex(new Vector3(3, 0, 0));
            var cap = m.GetVertex(new Vector3(0, 3, 0));

            var f = m.GetFace(a, b, c);
            var ab = m.GetFace(a, abp, b);
            var bc = m.GetFace(b, bcp, c);
            var ca = m.GetFace(c, cap, a);

            var fEdges = f.Edges.ToList();

            f.Delete();

            Assert.AreEqual(3, m.Faces.Count());

            foreach (var e in fEdges)
            {
                Assert.IsNull(e.Face);
                Assert.IsNull(e.Next);
            }

            foreach (var e in fEdges)
            {
                Assert.IsNotNull(e.Twin.Face);
                Assert.IsNotNull(e.Twin.Next);
            }

            f = m.GetFace(b, c, m.GetVertex(Vector3.Zero));
        }

        [TestMethod]
        public void InsertMidpoint()
        {
            Mesh m = new Mesh();

            var a = m.GetVertex(new Vector3(1, 0, 0));
            var b = m.GetVertex(new Vector3(2, 0, 0));
            var c = m.GetVertex(new Vector3(3, 0, 0));

            var f = m.GetFace(a, b, c);

            f.InsertMidpoint(m.GetVertex(new Vector3(4, 0, 0)));

            Assert.AreEqual(3, m.Faces.Count());
        }

        [TestMethod]
        public void VertexIndicesPreserved()
        {
            Mesh m = PrimitiveShapes.Cube();

            Face f = m.Faces.First();
            Vertex[] v1 = f.Vertices.ToArray();

            f.Delete();
            Face f2 = m.GetFace(v1);
            Vertex[] v2 = f2.Vertices.ToArray();

            Assert.AreEqual(v1.Length, v2.Length);

            for (int i = 0; i < v1.Length; i++)
                Assert.AreEqual(v1[i], v2[i]);
        }
    }
}
