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
            Face cbd = m.GetFace(c, b, d);

            var ca = m.GetEdge(c, a);
            var bd = m.GetEdge(b, d);

            m.GetEdge(b, c, abc, ca, cbd, bd);
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

        [TestMethod]
        public void GetEdgeThenGetTwin()
        {
            Mesh m = new Mesh();

            var a = m.GetVertex(Vector3.Zero);
            var b = m.GetVertex(Vector3.One);

            var e1 = m.GetEdge(a, b);
            var e2 = m.GetEdge(b, a);

            Assert.AreEqual(e1, e2.Twin);
            Assert.AreEqual(e1.Twin, e2);
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
        [ExpectedException(typeof(InvalidOperationException))]
        public void GetIllegalFaceDueToVerticesAlreadyInUse()
        {
            Mesh m = new Mesh();

            var a = m.GetVertex(Vector3.Zero);
            var b = m.GetVertex(Vector3.UnitX);
            var c = m.GetVertex(Vector3.UnitY);
            var d = m.GetVertex(Vector3.UnitZ);

            var f = m.GetFace(a, b, c, d);

            var f2 = m.GetFace(a, b, c);
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

        [TestMethod]
        public void DeleteFace()
        {
            Mesh m = new Mesh();

            var a = m.GetVertex(new Vector3(1, 0, 0));
            var b = m.GetVertex(new Vector3(2, 0, 0));
            var c = m.GetVertex(new Vector3(3, 0, 0));
            var d = m.GetVertex(new Vector3(4, 0, 0));

            Face abc = m.GetFace(a, b, c);
            Face bcd = m.GetFace(c, b, d);

            List<HalfEdge> edges = bcd.Edges.ToList();

            bcd.Delete();

            foreach (var edge in edges)
            {
                Assert.IsNull(edge.Face);
                Assert.IsNull(edge.Next);
            }
        }
        #endregion

        #region enumerators
        [TestMethod]
        public void FacesEnumeratorReturnsCorrectFacesAndCount()
        {
            Mesh m = new Mesh();

            var a = m.GetVertex(new Vector3(1, 0, 0));
            var b = m.GetVertex(new Vector3(2, 0, 0));
            var c = m.GetVertex(new Vector3(3, 0, 0));
            var d = m.GetVertex(new Vector3(4, 0, 0));

            Face abc = m.GetFace(a, b, c);
            Face bcd = m.GetFace(c, b, d);

            Assert.AreEqual(2, m.Faces.Count());

            Assert.IsTrue(m.Faces.Contains(abc));
            Assert.IsTrue(m.Faces.Contains(bcd));
        }

        [TestMethod]
        public void EdgesEnumeratorReturnsCorrectEdgesAndCount()
        {
            Mesh m = new Mesh();

            var a = m.GetVertex(new Vector3(1, 0, 0));
            var b = m.GetVertex(new Vector3(2, 0, 0));
            var c = m.GetVertex(new Vector3(3, 0, 0));

            Face abc = m.GetFace(a, b, c);

            Assert.AreEqual(6, m.HalfEdges.Count());

            Assert.IsTrue(m.HalfEdges.Contains(abc.Edges.Skip(0).First()));
            Assert.IsTrue(m.HalfEdges.Contains(abc.Edges.Skip(0).First().Twin));
            Assert.IsTrue(m.HalfEdges.Contains(abc.Edges.Skip(1).First()));
            Assert.IsTrue(m.HalfEdges.Contains(abc.Edges.Skip(1).First().Twin));
            Assert.IsTrue(m.HalfEdges.Contains(abc.Edges.Skip(2).First()));
            Assert.IsTrue(m.HalfEdges.Contains(abc.Edges.Skip(2).First().Twin));
        }

        [TestMethod]
        public void VerticesEnumeratorReturnsCorrectVerticesAndCount()
        {
            Mesh m = new Mesh();

            var a = m.GetVertex(new Vector3(1, 0, 0));
            var b = m.GetVertex(new Vector3(2, 0, 0));
            var c = m.GetVertex(new Vector3(3, 0, 0));

            Face abc = m.GetFace(a, b, c);

            Assert.AreEqual(3, m.Vertices.Count());

            Assert.IsTrue(m.Vertices.Contains(a));
            Assert.IsTrue(m.Vertices.Contains(b));
            Assert.IsTrue(m.Vertices.Contains(c));
        }
        #endregion

        [TestMethod]
        public void TriangulateCuboid()
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

            m.SubdivideAllFaces(Mesh.SubdivideOperation.Triangulate);

            Assert.AreEqual(8, m.Vertices.Count());
            Assert.AreEqual(36, m.HalfEdges.Count());
            Assert.AreEqual(12, m.Faces.Count());
        }

        #region listeners
        [TestMethod]
        public void Listener()
        {
            Mesh m = new Mesh();

            CounterListener l = new CounterListener();
            m.AddListener(l);

            var a = m.GetVertex(new Vector3(0, 0, 1));
            Assert.AreEqual(1, l.Vertices);
            Assert.AreEqual(0, l.Edges);
            Assert.AreEqual(0, l.Faces);

            m.Delete(a);
            Assert.AreEqual(0, l.Vertices);
            Assert.AreEqual(0, l.Edges);
            Assert.AreEqual(0, l.Faces);

            a = m.GetVertex(new Vector3(0, 0, 1));
            Assert.AreEqual(1, l.Vertices);
            Assert.AreEqual(0, l.Edges);
            Assert.AreEqual(0, l.Faces);

            var b = m.GetVertex(new Vector3(0, 0, 2));
            Assert.AreEqual(2, l.Vertices);
            Assert.AreEqual(0, l.Edges);
            Assert.AreEqual(0, l.Faces);

            var c = m.GetVertex(new Vector3(0, 0, 3));
            Assert.AreEqual(3, l.Vertices);
            Assert.AreEqual(0, l.Edges);
            Assert.AreEqual(0, l.Faces);

            var d = m.GetVertex(new Vector3(0, 0, 4));
            Assert.AreEqual(4, l.Vertices);
            Assert.AreEqual(0, l.Edges);
            Assert.AreEqual(0, l.Faces);

            var abcd = m.GetFace(a, b, c, d);
            Assert.AreEqual(4, l.Vertices);
            Assert.AreEqual(4, l.Edges);
            Assert.AreEqual(1, l.Faces);

            abcd.Delete();
            Assert.AreEqual(4, l.Vertices);
            Assert.AreEqual(4, l.Edges);
            Assert.AreEqual(0, l.Faces);

            var ab = m.GetEdge(a, b);
            ab.Delete();
            Assert.AreEqual(4, l.Vertices);
            Assert.AreEqual(3, l.Edges);
            Assert.AreEqual(0, l.Faces);
        }

        class CounterListener : Mesh.IChangeListener
        {
            public int Faces = 0;
            public int Vertices = 0;
            public int Edges = 0;

            public void Added(Face f)
            {
                Faces++;
            }

            public void Deleted(Face f)
            {
                Faces--;
            }

            public void Added(Vertex v)
            {
                Vertices++;
            }

            public void Deleted(Vertex v)
            {
                Vertices--;
            }

            public void Added(HalfEdge e)
            {
                Edges++;

                Assert.IsTrue(e.Primary);
            }

            public void Deleted(HalfEdge e)
            {
                Edges--;

                Assert.IsTrue(e.Primary);
            }


            public void SplitMidpointBegin(HalfEdge e, Vertex mid)
            {
                throw new NotImplementedException();
            }

            public void SplitMidpointEnd(HalfEdge e, Vertex mid)
            {
                throw new NotImplementedException();
            }

            public void SplitMidpointBegin(Face f, Vertex mid)
            {
                throw new NotImplementedException();
            }

            public void SplitMidpointEnd(Face f, Vertex mid)
            {
                throw new NotImplementedException();
            }
        }
        #endregion
    }
}
