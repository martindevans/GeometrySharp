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
        List<FaceDiminishment> children = new List<FaceDiminishment>();

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
                if (children.Count > 0)
                {
                    foreach (var leaf in children.SelectMany(a => a.Leaves))
                        yield return leaf;
                }
                else
                    yield return this;
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

        public IEnumerable<ProceduralFace> Add(params Face[] f)
        {
            faces.UnionWith(f);

            return f.Where(a => a != null).Cast<ProceduralFace>();
        }

        public bool Apply()
        {
            if (children.Count > 0)
            {
                for (int i = children.Count - 1; i >= 0; i--)
                    children[i].Apply();

                return true;
            }
            else
            {
                Delete();

                Face f = mesh.GetFace(border);
                (f as ProceduralFace).Development = Counter;
                if (Parent != null)
                    Parent.Add(f);

                mesh.CleanEdges();
                mesh.CleanVertices();

                return false;
            }
        }

        private void Delete()
        {
            for (int i = children.Count - 1; i >= 0; i--)
                children[i].Delete();

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
