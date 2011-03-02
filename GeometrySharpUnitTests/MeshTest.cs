﻿using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GeometrySharp.HalfEdgeGeometry;
using Microsoft.Xna.Framework;

namespace GeometrySharpUnitTests
{
    [TestClass]
    public class MeshTest
    {
        #region vertices
        [TestMethod]
        public void GetTwoDifferentVertices()
        {
            Mesh m = new Mesh();

            var a = m.GetVertex(Vector3.Zero);
            Assert.AreEqual(Vector3.Zero, a.Position);

            var b = m.GetVertex(new Vector3(1, 2, 3));
            Assert.AreEqual(new Vector3(1, 2, 3), b.Position);

            Assert.AreNotEqual(a, b);
        }

        [TestMethod]
        public void GetSameVertexTwice()
        {
            Mesh m = new Mesh();

            var a = m.GetVertex(Vector3.Zero);
            var b = m.GetVertex(Vector3.Zero);
            var c = m.GetVertex(Vector3.One);
            var d = m.GetVertex(Vector3.One);

            Assert.AreEqual(a, b);
            Assert.AreEqual(c, d);
            Assert.AreNotEqual(a, c);
        }

        [TestMethod]
        public void GetSimilarVertices()
        {
            Mesh m = new Mesh();

            var a = m.GetVertex(new Vector3(0, 0, 0));
            var b = m.GetVertex(new Vector3(Mesh.BUCKET_SIZE / 2, 0, 0));

            Assert.AreEqual(a, b);
            Assert.AreEqual(a.Position, Vector3.Zero);
            Assert.AreEqual(b.Position, Vector3.Zero);
        }
        #endregion

        #region edges
        [TestMethod]
        public void GetTwoDifferentEdges()
        {
            Mesh m = new Mesh();

            var a = m.GetVertex(Vector3.Zero);
            var b = m.GetVertex(Vector3.One);
            var c = m.GetVertex(Vector3.UnitX);

            var e1 = m.GetEdge(a, b);
            var e2 = m.GetEdge(b, c);

            Assert.AreNotEqual(e1, e2);
        }

        [TestMethod]
        public void GetSameEdgeTwice()
        {
            Mesh m = new Mesh();

            var a = m.GetVertex(Vector3.Zero);
            var b = m.GetVertex(Vector3.One);

            var e1 = m.GetEdge(a, b);
            var e2 = m.GetEdge(a, b);

            Assert.AreEqual(e1, e2);
        }

        [TestMethod]
        public void GetEdgeAndTwinSeparately()
        {
            Mesh m = new Mesh();

            var a = m.GetVertex(Vector3.Zero);
            var b = m.GetVertex(Vector3.One);

            var e1 = m.GetEdge(a, b);
            var e2 = m.GetEdge(b, a);

            Assert.AreEqual(e1, e2.Twin);
            Assert.AreEqual(e1.Twin, e2);
        }

        [TestMethod]
        public void GetEdgeAndCheckFaceAndNext()
        {
            Mesh m = new Mesh();

            var a = m.GetVertex(new Vector3(1, 0, 0));
            var b = m.GetVertex(new Vector3(2, 0, 0));
            var c = m.GetVertex(new Vector3(3, 0, 0));
            var d = m.GetVertex(new Vector3(4, 0, 0));

            Face abc = m.GetFace(a, b, c);

            m.GetEdge(a, b, abc, m.GetEdge(b, c));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GetEdgeAndCheckFaceAndIncorrectNext()
        {
            Mesh m = new Mesh();

            var a = m.GetVertex(new Vector3(1, 0, 0));
            var b = m.GetVertex(new Vector3(2, 0, 0));
            var c = m.GetVertex(new Vector3(3, 0, 0));
            var d = m.GetVertex(new Vector3(4, 0, 0));

            Face abc = m.GetFace(a, b, c);

            m.GetEdge(a, b, abc, m.GetEdge(c, a));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GetEdgeAndCheckIncorrectFaceAndNext()
        {
            Mesh m = new Mesh();

            var a = m.GetVertex(new Vector3(1, 0, 0));
            var b = m.GetVertex(new Vector3(2, 0, 0));
            var c = m.GetVertex(new Vector3(3, 0, 0));
            var d = m.GetVertex(new Vector3(4, 0, 0));

            Face abc = m.GetFace(a, b, c);
            Face bcd = m.GetFace(b, c, d);

            m.GetEdge(a, b, bcd, m.GetEdge(b, c));
        }

        [TestMethod]
        public void GetEdgeAndCheckEverythingIsValid()
        {
            Mesh m = new Mesh();

            var a = m.GetVertex(new Vector3(1, 0, 0));
            var b = m.GetVertex(new Vector3(2, 0, 0));
            var c = m.GetVertex(new Vector3(3, 0, 0));
            var d = m.GetVertex(new Vector3(4, 0, 0));

            Face abc = m.GetFace(a, b, c);
            Face bcd = m.GetFace(c, b, d);

            var ca = m.GetEdge(c, a);
            var ba = m.GetEdge(b, a);

            m.GetEdge(b, c, abc, ca, bcd, ba);
        }

        [TestMethod]
        public void GetEdgeBetweenTwoFaces()
        {
            Mesh m = new Mesh();

            var a = m.GetVertex(new Vector3(1, 0, 0));
            var b = m.GetVertex(new Vector3(2, 0, 0));
            var c = m.GetVertex(new Vector3(3, 0, 0));
            var d = m.GetVertex(new Vector3(4, 0, 0));

            Face abc = m.GetFace(a, b, c);
            Face bcd = m.GetFace(c, b, d);

            HalfEdge ca = m.GetEdge(c, a);
            HalfEdge bd = m.GetEdge(b, d);

            m.GetEdge(b, c, abc, ca, bcd, bd);
        }
        #endregion

        #region faces
        [TestMethod]
        public void GetFace()
        {
            Mesh m = new Mesh();
            
            var a = m.GetVertex(Vector3.UnitY);
            var b = m.GetVertex(Vector3.UnitX);
            var c = m.GetVertex(-Vector3.UnitX);

            var f = m.GetFace(a, b, c);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GetIllegalFaceWithDuplicateVertices()
        {
            Mesh m = new Mesh();

            var a = m.GetVertex(Vector3.Zero);

            m.GetFace(a, a, a);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GetIllegalFaceWithConflictingEdgeAlreadyInPlaceDueToWinding()
        {
            Mesh m = new Mesh();

            var a = m.GetVertex(Vector3.Zero);
            var b = m.GetVertex(Vector3.UnitX);
            var c = m.GetVertex(Vector3.UnitY);
            var d = m.GetVertex(Vector3.UnitZ);

            var f = m.GetFace(a, b, c);
            var f2 = m.GetFace(a, b, d);
        }

        [TestMethod]
        public void GetSameFaceTwice()
        {
            Mesh m = new Mesh();

            var a = m.GetVertex(new Vector3(1, 0, 0));
            var b = m.GetVertex(new Vector3(0, 2, 0));
            var c = m.GetVertex(new Vector3(0, 0, 3));

            Face f1 = m.GetFace(a, b, c);
            Face f2 = m.GetFace(a, b, c);

            Assert.AreEqual(f1, f2);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GetIllegalFaceWithOnlyTwoVertices()
        {
            Mesh m = new Mesh();

            var a = m.GetVertex(Vector3.Zero);
            var b = m.GetVertex(Vector3.One);

            Face f = m.GetFace(a, b);
        }

        [TestMethod]
        public void CreateTwoBorderingFaces()
        {
            Mesh m = new Mesh();

            var a = m.GetVertex(new Vector3(1, 0, 0));
            var b = m.GetVertex(new Vector3(2, 0, 0));
            var c = m.GetVertex(new Vector3(3, 0, 0));
            var d = m.GetVertex(new Vector3(4, 0, 0));

            Face abc = m.GetFace(a, b, c);
            Face bcd = m.GetFace(c, b, d);
        }
        #endregion
    }
}