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

            m.GetEdge(a, b).Split(m.GetVertex(new Vector3(4, 0, 0), "mid"));

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

            ab.Split(mid);

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
    }
}
