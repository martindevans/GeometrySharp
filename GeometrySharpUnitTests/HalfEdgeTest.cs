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
    public class HalfEdgeTest
    {
        [TestMethod]
        public void SplitHalfEdgeOnASingleFace()
        {
            Mesh m = new Mesh();

            var a = m.GetVertex(new Vector3(1, 0, 0), "a");
            var b = m.GetVertex(new Vector3(2, 0, 0), "b");
            var c = m.GetVertex(new Vector3(3, 0, 0), "c");

            var f = m.GetFace(a, b, c);

            var mid = m.GetVertex(new Vector3(4, 0, 0), "mid");
            var ab = m.GetEdge(a, b);
            ab.Split(mid);

            Assert.AreEqual(4, f.Edges.Count());
            Assert.AreEqual(4, f.Vertices.Count());
            Assert.AreEqual(0, f.Neighbours.Count());

            var ba = m.GetEdge(b, a);

            Assert.IsNull(ba.Face);
            Assert.IsNull(ba.Next);
        }

        [TestMethod]
        public void SplitHalfEdgeBetweenTwoFaces()
        {
            Mesh m = new Mesh();

            var a = m.GetVertex(new Vector3(1, 0, 0), "a");
            var b = m.GetVertex(new Vector3(2, 0, 0), "b");
            var c = m.GetVertex(new Vector3(3, 0, 0), "c");
            var d = m.GetVertex(new Vector3(4, 0, 0), "d");

            Face abc = m.GetFace(a, b, c);
            Face bcd = m.GetFace(c, b, d);

            var bc = m.GetEdge(b, c);
            bc.Split(m.GetVertex(new Vector3(5, 0, 0), "m"));

            foreach (var face in m.Faces)
            {
                foreach (var edge in face.Edges)
                    Assert.AreEqual(edge.End, edge.Next.Twin.End);
            }

            Assert.AreEqual(4, abc.Edges.Count());
            Assert.AreEqual(1, abc.Neighbours.Count());
            Assert.AreEqual(4, abc.Vertices.Count());

            Assert.AreEqual(4, bcd.Edges.Count());
            Assert.AreEqual(1, bcd.Neighbours.Count());
            Assert.AreEqual(4, bcd.Vertices.Count());
        }

        [TestMethod]
        public void SplitFloatingEdge()
        {
            Mesh m = new Mesh();

            var a = m.GetVertex(Vector3.Zero, "a");
            var b = m.GetVertex(Vector3.One, "b");

            var ab = m.GetEdge(a, b);

            var mid = m.GetVertex(Vector3.Up, "mid");

            var splitResult = ab.Split(mid);

            Assert.AreEqual(ab.End, b);
            Assert.AreEqual(ab.Twin.End, mid);
            Assert.AreEqual(splitResult.End, mid);
            Assert.AreEqual(splitResult.Twin.End, a);

            Assert.IsNull(ab.Face);
            Assert.IsNull(ab.Next);
            Assert.IsNull(ab.Twin.Face);
            Assert.IsNull(ab.Twin.Next);

            Assert.AreEqual(1, a.OutgoingEdges.Count());
            Assert.AreEqual(1, a.IncomingEdges.Count());
            Assert.AreEqual(1, b.OutgoingEdges.Count());
            Assert.AreEqual(1, b.IncomingEdges.Count());

            Assert.AreEqual(2, mid.OutgoingEdges.Count());
            Assert.AreEqual(2, mid.IncomingEdges.Count());
        }

        [TestMethod]
        public void SplitEdgeCheckUpdatedIndices()
        {
            Mesh m = new Mesh();

            var a = m.GetVertex(new Vector3(1, 0, 0), "a");
            var b = m.GetVertex(new Vector3(2, 0, 0), "b");
            var c = m.GetVertex(new Vector3(3, 0, 0), "c");
            var d = m.GetVertex(new Vector3(4, 0, 0), "d");

            Face abc = m.GetFace(a, b, c);
            Face bcd = m.GetFace(c, b, d);

            var bc = m.GetEdge(b, c);
            var mid = m.GetVertex(new Vector3(5, 0, 0), "m");
            bc.Split(mid);

            Assert.AreEqual(2, mid.Neighbours.Count());
            Assert.IsTrue(mid.Neighbours.Contains(abc));
            Assert.IsTrue(mid.Neighbours.Contains(bcd));
        }

        [TestMethod]
        public void SplitEdgeWithDuplicatePoint()
        {
            Mesh m = new Mesh();

            var a = m.GetVertex(new Vector3(0, 0, 1), "a");
            var b = m.GetVertex(new Vector3(0, 0, 2), "b");

            var ab = m.GetEdge(a, b);

            var mid = m.GetVertex(new Vector3(0, 0, 2), "mid");

            ab.Split(mid);

            Assert.IsNull(ab.Face);
            Assert.IsNull(ab.Next);
            Assert.IsNull(ab.Twin.Face);
            Assert.IsNull(ab.Twin.Next);

            Assert.AreEqual(1, a.OutgoingEdges.Count());
            Assert.AreEqual(1, a.IncomingEdges.Count());
            Assert.AreEqual(1, b.OutgoingEdges.Count());
            Assert.AreEqual(1, b.IncomingEdges.Count());

            Assert.AreEqual(1, mid.OutgoingEdges.Count());
            Assert.AreEqual(1, mid.IncomingEdges.Count());

            Assert.AreEqual(2, m.HalfEdges.Count());
            Assert.AreEqual(2, m.Vertices.Count());
            Assert.AreEqual(0, m.Faces.Count());
        }

        [TestMethod]
        public void MergeEdgesAroundALoneFace()
        {
            Mesh m = new Mesh();

            var a = m.GetVertex(new Vector3(0, 0, 1), "a");
            var b = m.GetVertex(new Vector3(0, 0, 2), "b");
            var c = m.GetVertex(new Vector3(0, 0, 3), "c");
            var d = m.GetVertex(new Vector3(0, 0, 4), "d");

            var abcd = m.GetFace(a, b, c, d);

            var ab = m.GetEdge(a, b);

            Assert.AreEqual(8, m.HalfEdges.Count());

            ab.Merge();

            Assert.AreEqual(c, ab.End);
            Assert.AreEqual(ab.Next, m.GetEdge(c, d, false));
            Assert.IsNull(m.GetEdge(a, b, false));
            Assert.AreEqual(ab, m.GetEdge(d, a, false).Next);

            Assert.AreEqual(6, m.HalfEdges.Count());
            Assert.AreEqual(3, m.HalfEdges.Where(e => e.Face == null).Count());
            Assert.AreEqual(0, m.HalfEdges.Where(e => e.Face != null && e.Next == null).Count());
            Assert.AreEqual(0, m.HalfEdges.Where(e => e.Face == null && e.Next != null).Count());
        }

        [TestMethod]
        public void MergeEdgesOnACube()
        {
            Mesh m = PrimitiveShapes.Cube();

            var e = m.HalfEdges.First();
            var a = e.Twin.End;
            var b = e.End;
            var x = m.GetVertex(new Vector3(0, 0, 100));

            Assert.AreEqual(9, m.Vertices.Count());
            Assert.AreEqual(24, m.HalfEdges.Count());
            Assert.AreEqual(6, m.Faces.Count());

            var e2 = e.Split(x);

            foreach (var face in m.Faces)
            {
                foreach (var edge in face.Edges)
                    Assert.AreEqual(edge.End, edge.Next.Twin.End);
            }

            Assert.AreEqual(e.End, b);
            Assert.AreEqual(e.Twin.End, x);
            Assert.AreEqual(e2.End, x);
            Assert.AreEqual(e2.Twin.End, a);

            Assert.AreEqual(9, m.Vertices.Count());
            Assert.AreEqual(26, m.HalfEdges.Count());
            Assert.AreEqual(6, m.Faces.Count());

            e2.Merge();

            foreach (var face in m.Faces)
            {
                foreach (var edge in face.Edges)
                    Assert.AreEqual(edge.End, edge.Next.Twin.End);
            }

            Assert.AreEqual(9, m.Vertices.Count());
            Assert.AreEqual(24, m.HalfEdges.Count());
            Assert.AreEqual(6, m.Faces.Count());

            foreach (var face in m.Faces)
            {
                Assert.AreEqual(4, face.Edges.Count());
                Assert.AreEqual(4, face.Vertices.Count());
                Assert.AreEqual(4, face.Neighbours.Count());
                Assert.IsFalse(face.Vertices.Contains(x));
            }

            m.CleanVertices();

            Assert.AreEqual(8, m.Vertices.Count());
            Assert.AreEqual(24, m.HalfEdges.Count());
            Assert.AreEqual(6, m.Faces.Count());
        }
    }
}
