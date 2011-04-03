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
        public readonly Mesh Mesh;

        private HashSet<ProceduralFace> faces = new HashSet<ProceduralFace>();

        public bool Developed
        {
            get;
            internal set;
        }

        public FaceDevelopment(Mesh m, FaceDiminishment parent)
        {
            Parent = parent;
            Mesh = m;
        }

        public void BindFace(ProceduralFace f)
        {
            if (faces.Add(f))
                f.Development = this;
        }

        public void ClearBindings()
        {
            Developed = false;
            foreach (var f in faces)
                f.Development = null;
            faces.Clear();
        }

        public FaceDiminishment Apply()
        {
            if (!Developed)
            {
                Developed = true;

                FaceDiminishment diminish = new FaceDiminishment(Parent, this, Mesh, faces);

                Apply(faces, Mesh, diminish);

                return diminish;
            }

            return null;
        }

        protected abstract void Apply(IEnumerable<ProceduralFace> face, Mesh m, FaceDiminishment inverse);
    }
}
