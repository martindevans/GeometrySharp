using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometrySharp.HalfEdgeGeometry;
using Microsoft.Xna.Framework;

namespace GeometrySharp.ConstructiveSolidGeometry.Operations
{
    public class Difference
        : CsgNode
    {
        CsgNode left;
        public CsgNode Left
        {
            get { return left; }
            set
            {
                left = value;
                IsDirty = true;
            }
        }

        CsgNode right;
        public CsgNode Right
        {
            get { return right; }
            set
            {
                right = value;
                IsDirty = true;
            }
        }

        public Difference(CsgNode left, CsgNode right)
        {
            Left = left;
            Right = right;
        }

        public override bool Contains(Vector3 point)
        {
            return left.Contains(point) && !right.Contains(point);
        }

        public override Mesh MakeMesh()
        {
            var leftMesh = left.MakeMesh();
            var rightMesh = right.MakeMesh();

            var insideRight = ClassifyPoints(right, leftMesh);
            var insideLeft = ClassifyPoints(left, rightMesh);

            foreach (var vertex in insideRight)
            {
                foreach (var edge in vertex.OutgoingEdges)
                {
                    Vector3 start = vertex.Position;
                    Vector3 end = edge.End.Position;
                    //var intersection = right.IntersectionPoint(start, end);
                }
            }

            return leftMesh;
        }
    }
}
