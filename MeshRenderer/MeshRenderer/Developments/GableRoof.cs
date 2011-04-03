using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometrySharp.Procedural;
using GeometrySharp.HalfEdgeGeometry;
using Microsoft.Xna.Framework;

namespace MeshRenderer.Developments
{
    public class GableRoof
        :FaceDevelopment
    {
        public GableRoof(Mesh m, FaceDiminishment parentDiminish)
            :base(m, parentDiminish)
        {
        }

        protected override void Apply(IEnumerable<ProceduralFace> faces, Mesh m, FaceDiminishment inverse)
        {
            HashSet<ProceduralFace> added = new HashSet<ProceduralFace>();

            foreach (var f in faces)
                added.UnionWith(Split(f));

            inverse.Add(added);
        }

        private IEnumerable<ProceduralFace> Split(Face f)
        {
            Vertex[] v = f.Vertices.ToArray();

            f.Delete();

            Vector3 up = -new Plane(v[0].Position, v[1].Position, v[2].Position).Normal;

            Vertex a = Mesh.GetVertex(v[0].Position * 0.5f + v[1].Position * 0.5f + up * 2);
            Vertex b = Mesh.GetVertex(v[2].Position * 0.5f + v[3].Position * 0.5f + up * 2);

            yield return (ProceduralFace)Mesh.GetFace(v[1], v[2], b, a);
            yield return (ProceduralFace)Mesh.GetFace(v[3], v[0], a, b);
            yield return (ProceduralFace)Mesh.GetFace(v[0], v[1], a);
            yield return (ProceduralFace)Mesh.GetFace(v[2], v[3], b);
        }
    }
}
