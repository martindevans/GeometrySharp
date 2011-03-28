using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometrySharp.HalfEdgeGeometry;

namespace GeometrySharp.Procedural
{
    public class FaceDiminishment
    {
        Vertex[] border;
        Mesh mesh;

        HashSet<Face> faces = new HashSet<Face>();
        HashSet<FaceDiminishment> children = new HashSet<FaceDiminishment>();

        public FaceDiminishment Parent
        {
            get;
            private set;
        }
        public readonly FaceDevelopment Counter;

        public IEnumerable<FaceDiminishment> Leaves
        {
            get
            {
                if (children.Count == 0)
                    yield return this;
                else
                {
                    foreach (var leaf in children.SelectMany(a => a.Leaves))
                        yield return leaf;
                }
            }
        }

        public FaceDiminishment(Face face, FaceDiminishment parent, FaceDevelopment development)
        {
            mesh = face.Mesh;
            border = face.Vertices.ToArray();

            Counter = development;

            if (parent != null)
                parent.Add(this, face);
        }

        public void Add(Face f)
        {
            faces.Add(f);
        }

        public Face[] Add(params Face[] f)
        {
            faces.UnionWith(f);

            return f;
        }

        public void Apply()
        {
            Delete();

            Face f = mesh.GetFace(border);
            (f as ProceduralFace).Development = Counter;
            if (Parent != null)
                Parent.Add(f);

            mesh.CleanEdges();
            mesh.CleanVertices();
        }

        private void Delete()
        {
            foreach (var c in children)
                c.Delete();

            foreach (var face in faces)
                face.Delete();
            faces.Clear();

            if (Parent != null)
                Parent.children.Remove(this);
        }

        private void Add(FaceDiminishment child, Face face)
        {
            faces.Remove(face);

            if (child.Parent != null)
                throw new ArgumentException("Child is already in an operation tree");
            child.Parent = this;

            children.Add(child);
        }
    }
}
