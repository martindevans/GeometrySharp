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
        public readonly int IndexOffset;

        public GableRoof(int indexOffset, FaceDiminishment parentDiminish)
            :base(parentDiminish)
        {
            IndexOffset = indexOffset;
        }

        protected override void Apply(ProceduralFace face, Mesh m, FaceDiminishment inverse)
        {
            Vertex[] v = face.Vertices.ToArray();

            face.Delete();

            var v0 = v[IndexOffset % v.Length];
            var v1 = v[(1 + IndexOffset) % v.Length];
            var v2 = v[(2 + IndexOffset) % v.Length];
            var v3 = v[(3 + IndexOffset) % v.Length];

            Vector3 up = -new Plane(v[0].Position, v[1].Position, v[2].Position).Normal;

            Vertex a = m.GetVertex(v0.Position * 0.5f + v1.Position * 0.5f + up * 2);
            Vertex b = m.GetVertex(v2.Position * 0.5f + v3.Position * 0.5f + up * 2);

            var faces = inverse.Add(
                m.GetFace(v1, v2, b, a),
                m.GetFace(v3, v0, a, b),
                m.GetFace(v0, v1, a),
                m.GetFace(v2, v3, b)
            );

            foreach (var f in faces.Take(2))
                (f as ProceduralFace).Development = new GableRoof(IndexOffset + 3, inverse);
        }
    }
}
