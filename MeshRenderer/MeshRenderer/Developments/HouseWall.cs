using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometrySharp.Procedural;
using GeometrySharp.HalfEdgeGeometry;

namespace MeshRenderer.Developments
{
    public class HouseWall
        :FaceDevelopment
    {
        public HouseWall(Mesh mesh, FaceDiminishment parent)
            :base(mesh, parent)
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
            HashSet<HalfEdge> edges = new HashSet<HalfEdge>();
            foreach (var edge in f.Edges)
            {
                var dir = (edge.End.Position - edge.Twin.End.Position);
                if (Math.Abs(dir.X) > 0.1f || Math.Abs(dir.Z) > 0.1f)
                    edges.Add(edge);
            }

            if (edges.Count != 2)
                throw new InvalidOperationException("Not 2 edges");

            int splits = 4;
            foreach (var edge in edges)
            {
                var dir = edge.End.Position - edge.Twin.End.Position;
                var step = dir / (float)splits;
                var start = edge.Twin.End.Position;

                for (int i = 1; i < splits; i++)
                    edge.Split(Mesh.GetVertex(start + step * i));
            }

            Vertex[] vertices = f.Vertices.ToArray();

            f.Delete();

            for (int i = 0; i < vertices.Length / 2 - 1; i++)
                yield return (ProceduralFace)Mesh.GetFace(vertices[i], vertices[i + 1], vertices[vertices.Length - 2 - i], vertices[vertices.Length - 1 - i]);
        }
    }
}
