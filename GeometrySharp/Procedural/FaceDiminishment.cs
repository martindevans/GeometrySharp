using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometrySharp.HalfEdgeGeometry;

namespace GeometrySharp.Procedural
{
    public class FaceDiminishment
    {
        List<Vertex[]> borders = new List<Vertex[]>();
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

        public FaceDiminishment(FaceDiminishment parent, FaceDevelopment development, Mesh m, params Face[] faces)
            :this(parent, development, m, faces as IEnumerable<Face>)
        {
        }

        public FaceDiminishment(FaceDiminishment parent, FaceDevelopment development, Mesh m, IEnumerable<Face> faces)
        {
            mesh = m;
            borders.AddRange(faces.Select(a => a.Vertices.ToArray()));

            Counter = development;

            if (parent != null)
                parent.Add(this, faces);
        }

        public IEnumerable<ProceduralFace> Add(params Face[] f)
        {
            return Add(f as IEnumerable<Face>);
        }

        public IEnumerable<ProceduralFace> Add(IEnumerable<Face> f)
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

                Counter.ClearBindings();
                for (int i = 0; i < borders.Count; i++)
                {
                    Face f = mesh.GetFace(borders[i]);
                    (f as ProceduralFace).Development = Counter;
                    if (Parent != null)
                        Parent.Add(f);
                }

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

        private void Add(FaceDiminishment child, IEnumerable<Face> faces)
        {
            this.faces.ExceptWith(faces);

            if (child.Parent != null)
                throw new ArgumentException("Child is already in an operation tree");
            child.Parent = this;

            children.Add(child);
        }
    }
}
