using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometrySharp.Procedural;
using GeometrySharp.HalfEdgeGeometry;

namespace MeshRenderer.Developments
{
    public class WindowWallSection
        :FaceDevelopment
    {
        public WindowWallSection(Mesh mesh, FaceDiminishment parent)
            :base(mesh, parent)
        {
        }

        protected override void Apply(IEnumerable<ProceduralFace> face, Mesh m)
        {
            throw new NotImplementedException();
        }
    }
}
