using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GeometrySharp.ConstructiveSolidGeometry.Primitives;
using GeometrySharp.HalfEdgeGeometry;
using Microsoft.Xna.Framework;

namespace GeometrySharpUnitTests
{
    /// <summary>
    /// Summary description for CsgPrimitivesTest
    /// </summary>
    [TestClass]
    public class CsgPrimitivesTest
    {
        [TestMethod]
        public void SpherePrimitive()
        {
            int subdivs = 3;

            Sphere sphere = new Sphere(subdivs);
            Mesh pSphere = sphere.MakeMesh();
            Mesh mSphere = PrimitiveShapes.Sphere(subdivs);

            Assert.AreEqual(mSphere.Vertices.Count(), pSphere.Vertices.Count());
            Assert.AreEqual(mSphere.HalfEdges.Count(), pSphere.HalfEdges.Count());
            Assert.AreEqual(mSphere.Faces.Count(), pSphere.Faces.Count());

            Assert.IsTrue(sphere.Contains(Vector3.Zero));
            Assert.IsTrue(sphere.Contains(new Vector3(0.99f, 0, 0)));
            Assert.IsFalse(sphere.Contains(Vector3.One));
        }

        [TestMethod]
        public void CubePrimitive()
        {
            Cube cube = new Cube();
            Mesh pCube = cube.MakeMesh();
            Mesh mCube = PrimitiveShapes.Cube();

            Assert.AreEqual(mCube.Vertices.Count(), pCube.Vertices.Count());
            Assert.AreEqual(mCube.HalfEdges.Count(), pCube.HalfEdges.Count());
            Assert.AreEqual(mCube.Faces.Count(), pCube.Faces.Count());

            Assert.IsTrue(cube.Contains(Vector3.Zero));
            Assert.IsTrue(cube.Contains(new Vector3(0.499f)));
            Assert.IsFalse(cube.Contains(Vector3.One));
        }
    }
}
