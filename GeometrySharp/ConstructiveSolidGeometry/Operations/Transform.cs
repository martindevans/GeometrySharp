using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using GeometrySharp.HalfEdgeGeometry;

namespace GeometrySharp.ConstructiveSolidGeometry.Operations
{
    public class Transform
        :CsgNode
    {
        private CsgNode child;
        public CsgNode Child
        {
            get
            {
                return child;
            }
            set
            {
                if (child != null)
                    child.Parent = null;
                child = value;
                if (child != null)
                    child.Parent = this;
                IsDirty = true;
            }
        }

        private Matrix inverseTransformation;
        private Matrix transformation;
        public Matrix Transformation
        {
            get
            {
                return transformation;
            }
            set
            {
                transformation = value;
                inverseTransformation = Matrix.Invert(transformation);
                IsDirty = true;
            }
        }

        public Transform(CsgNode child, Matrix transformation)
        {
            Child = child;

            Transformation = transformation;
        }

        public override bool Contains(Vector3 point)
        {
            return child.Contains(Vector3.Transform(point, inverseTransformation));
        }

        public override Mesh MakeMesh()
        {
            Mesh m = child.MakeMesh();

            foreach (var v in m.Vertices)
                v.Position = Vector3.Transform(v.Position, transformation);

            return m;
        }
    }
}
