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
    public class VertexTest
    {
        [TestMethod]
        public void IncomingEdges()
        {
            Mesh m = new Mesh();

            var a = m.GetVertex(new Vector3(1, 0, 0), "a");
            var b = m.GetVertex(new Vector3(2, 0, 0), "b");
            var c = m.GetVertex(new Vector3(3, 0, 0), "c");
 
            m.GetFace(a, b, c);

            Assert.AreEqual(2, a.IncomingEdges.Count());
            Assert.AreEqual(1, a.IncomingEdges.Where(e => e.Twin.End == b).Count());
        }

        [TestMethod]
        public void OutgoingEdges()
        {
            Mesh m = new Mesh();

            var a = m.GetVertex(new Vector3(1, 0, 0), "a");
            var b = m.GetVertex(new Vector3(2, 0, 0), "b");
            var c = m.GetVertex(new Vector3(3, 0, 0), "c");

            m.GetFace(a, b, c);

            Assert.AreEqual(2, a.OutgoingEdges.Count());
            Assert.AreEqual(1, a.OutgoingEdges.Where(e => e.End == b).Count());
        }
    }
}
