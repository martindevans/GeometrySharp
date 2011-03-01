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
    public class PrimitiveShapes
    {
        [TestMethod]
        public void ConstructTriangle()
        {
            Mesh m = new Mesh();
            //oh hai
            var a = m.GetVertex(Vector3.Zero);
            var b = m.GetVertex(Vector3.Zero);
            var c = m.GetVertex(Vector3.Zero);

            var f = m.GetFace(a, b, c);
        }
    }
}
