using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using GeometrySharp.HalfEdgeGeometry;

namespace GeometrySharp.ConstructiveSolidGeometry.Operations
{
    public class Union
        :CsgNode
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

        public Union(CsgNode left, CsgNode right)
        {
            Left = left;
            Right = right;
        }

        public override bool Contains(Vector3 point)
        {
            return left.Contains(point) || right.Contains(point);
        }

        public override Mesh MakeMesh()
        {
            throw new NotImplementedException();
        }
    }
}
