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
        public readonly FaceDiminishment Parent;

        public FaceDevelopment(FaceDiminishment parent)
        {
            Parent = parent;
        }

        public FaceDiminishment Apply(ProceduralFace face)
        {
            FaceDiminishment diminish = new FaceDiminishment(Parent, this, face.Mesh, face);
            
            Apply(face, face.Mesh, diminish);

            return diminish;
        }

        protected abstract void Apply(ProceduralFace face, Mesh m, FaceDiminishment inverse);
    }
}
