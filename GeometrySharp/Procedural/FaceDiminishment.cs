using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometrySharp.HalfEdgeGeometry;

namespace GeometrySharp.Procedural
{
    public class FaceDiminishment
        :Mesh.IChangeListener
    {
        ChangeSet changes;

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

        public FaceDiminishment(FaceDiminishment parent, FaceDevelopment development, ChangeSet changes)
        {
            Counter = development;
            this.changes = changes;

            if (parent != null)
                parent.Add(this);
        }

        public bool DeleteLeaves()
        {
            if (children.Count > 0)
            {
                for (int i = children.Count - 1; i >= 0; i--)
                    children[i].DeleteLeaves();

                return true;
            }
            else
            {
                DeleteTree();

                return false;
            }
        }

        private void DeleteTree()
        {
            for (int i = children.Count - 1; i >= 0; i--)
                children[i].DeleteTree();

            changes.Mesh.AddListener(this);
            changes.Undo();
            changes.Mesh.RemoveListener(this);

            if (Parent != null)
                Parent.children.Remove(this);
        }

        private void Add(FaceDiminishment child)
        {
            if (child.Parent != null)
                throw new ArgumentException("Child is already in an operation tree");
            child.Parent = this;

            children.Add(child);
        }

        #region change listener
        void Mesh.IChangeListener.Added(Face f)
        {
            (f as ProceduralFace).Development = Counter;
        }

        void Mesh.IChangeListener.Deleted(Face f)
        {
        }

        void Mesh.IChangeListener.Added(Vertex v)
        {
        }

        void Mesh.IChangeListener.Deleted(Vertex v)
        {
        }

        void Mesh.IChangeListener.Added(HalfEdge e)
        {
        }

        void Mesh.IChangeListener.Deleted(HalfEdge e)
        {
        }

        void Mesh.IChangeListener.SplitMidpointBegin(HalfEdge e, Vertex mid)
        {
        }

        void Mesh.IChangeListener.SplitMidpointEnd(HalfEdge e, Vertex mid)
        {
        }

        void Mesh.IChangeListener.SplitMidpointBegin(Face f, Vertex mid)
        {
        }

        void Mesh.IChangeListener.SplitMidpointEnd(Face f, Vertex mid)
        {
        }
        #endregion
    }
}
