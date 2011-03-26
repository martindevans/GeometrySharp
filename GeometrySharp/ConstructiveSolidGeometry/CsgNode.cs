using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using GeometrySharp.HalfEdgeGeometry;

namespace GeometrySharp.ConstructiveSolidGeometry
{
    public abstract class CsgNode
    {
        private CsgNode parent;
        public CsgNode Parent
        {
            get
            {
                return parent;
            }
            set
            {
                if (parent == null || value == null)
                    parent = value;
                else if (!parent.Equals(value))
                    throw new InvalidOperationException("Cannot set parent to " + value + " when " + parent + " is set as parent");
            }
        }

        private bool isDirty = true;
        public bool IsDirty
        {
            get
            {
                return isDirty;
            }
            set
            {
                isDirty = value;
                if (isDirty && Parent != null)
                    Parent.IsDirty = true;
            }
        }

        public abstract bool Contains(Vector3 point);

        public abstract Mesh MakeMesh();
    }
}
