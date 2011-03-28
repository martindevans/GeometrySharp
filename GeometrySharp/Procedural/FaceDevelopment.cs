using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometrySharp.HalfEdgeGeometry;
using Microsoft.Xna.Framework;

namespace GeometrySharp.Procedural
{
    public abstract class FaceDevelopment
    {
        public abstract FaceDiminishment Apply(ProceduralFace face, FaceDiminishment parent = null);
    }
}
