using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometrySharp.HalfEdgeGeometry;

namespace GeometrySharp.Procedural
{
    public class ProceduralFace
        :Face
    {
        private FaceDevelopment development;
        public FaceDevelopment Development
        {
            get
            {
                return development;
            }
            set
            {
                if (development != null)
                    throw new InvalidOperationException("Cannot set the expansion for a face twice");
                development = value;
            }
        }

        public ProceduralFace(Mesh m)
            :base(m)
        {

        }

        public FaceDiminishment Develop(FaceDiminishment parent = null)
        {
            if (Development == null)
                throw new NullReferenceException("Development is null");

            var dim = Development.Apply(this, parent);

            return dim;
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}
