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
        public void SplitHalfEdge()
        {
            Mesh m = new Mesh();

            var a = m.GetVertex(new Vector3(1, 0, 0), "a");
            var b = m.GetVertex(new Vector3(2, 0, 0), "b");
            var c = m.GetVertex(new Vector3(3, 0, 0), "c");

            var f = m.GetFace(a, b, c);

            m.GetEdge(a, b).Split(m.GetVertex(new Vector3(4, 0, 0), "mid"));

            Assert.AreEqual(4, f.Edges.Count());
        }
    }
}
