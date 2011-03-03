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
    public class PrimitiveShapesTest
    {
        #region cuboid
        [TestMethod]
        public void Cuboid()
        {
            Mesh m = PrimitiveShapes.Cuboid(
                new Vector3(1, 0, 0),
                new Vector3(2, 0, 0),
                new Vector3(3, 0, 0),
                new Vector3(4, 0, 0),
                new Vector3(5, 0, 0),
                new Vector3(6, 0, 0),
                new Vector3(7, 0, 0),
                new Vector3(8, 0, 0));

            Assert.AreEqual(8, m.Vertices.Count());
            Assert.AreEqual(24, m.HalfEdges.Count());
            Assert.AreEqual(6, m.Faces.Count());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void DegenerateCuboid()
        {
            Mesh m = PrimitiveShapes.Cuboid(
                new Vector3(0, 0, 0),
                new Vector3(0, 0, 0),
                new Vector3(0, 0, 0),
                new Vector3(0, 0, 0),
                new Vector3(0, 0, 0),
                new Vector3(0, 0, 0),
                new Vector3(0, 0, 0),
                new Vector3(0, 0, 0));

            Assert.AreEqual(8, m.Vertices.Count());
            Assert.AreEqual(24, m.HalfEdges.Count());
            Assert.AreEqual(6, m.Faces.Count());
        }
        #endregion

        [TestMethod]
        public void Icosahedron()
        {
            Mesh m = PrimitiveShapes.Icosahedron();

            Assert.AreEqual(12, m.Vertices.Count());
            Assert.AreEqual(20, m.Faces.Count());
        }
    }
}
