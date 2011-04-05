using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometrySharp.HalfEdgeGeometry;
using Microsoft.Xna.Framework;

namespace GeometrySharp.Procedural
{
    public class ChangeSet
        :Mesh.IChangeListener
    {
        public readonly Mesh Mesh;
        public bool Logging
        {
            get;
            private set;
        }
        public bool Logged
        {
            get;
            private set;
        }

        private Stack<Change> changes = new Stack<Change>();
        public int Changes
        {
            get
            {
                return changes.Count;
            }
        }

        public ChangeSet(Mesh m)
        {
            Mesh = m;
        }

        public void Begin()
        {
            if (Logged)
                throw new InvalidOperationException("Cannot activate logging twice");
            Logging = true;
            Logged = true;

            Mesh.AddListener(this);
        }

        public void End()
        {
            if (!Logging)
                throw new InvalidOperationException("Cannot stop logging before starting");
            Logging = false;

            Mesh.RemoveListener(this);
        }

        public void Undo()
        {
            while (changes.Count > 0)
                changes.Pop().Undo(Mesh, changes);
        }

        public void Added(Face f)
        {
            changes.Push(new AddFace(f));
        }

        public void Deleted(Face f)
        {
            changes.Push(new DeleteFace(f));
        }

        public void Added(Vertex v)
        {
            //changes.Push(new AddVertex(v));
        }

        public void Deleted(Vertex v)
        {
            
        }

        public void Added(HalfEdge e)
        {
            changes.Push(new AddHalfEdge(e));
        }

        public void Deleted(HalfEdge e)
        {
            changes.Push(new DeleteHalfEdge(e));
        }

        public void SplitMidpointBegin(HalfEdge e, Vertex mid)
        {
            changes.Push(new BeginSplitMidpointHalfEdge(e, mid));
        }

        public void SplitMidpointEnd(HalfEdge e, Vertex mid)
        {
            changes.Push(new EndSplitMidpointHalfEdge(e, mid));
        }

        public void SplitMidpointBegin(Face f, Vertex mid)
        {
            throw new NotImplementedException();
        }

        public void SplitMidpointEnd(Face f, Vertex mid)
        {
            throw new NotImplementedException();
        }

        private abstract class Change
        {
            public abstract void Undo(Mesh mesh, Stack<Change> changes);
        }

        private class AddVertex
            :Change
        {
            public readonly Vector3 Position;

            public AddVertex(Vertex v)
            {
                Position = v.Position;
            }

            public override void Undo(Mesh mesh, Stack<Change> changes)
            {
                //Vertex.Delete();
            }
        }

        private class AddHalfEdge
            :Change
        {
            public readonly Vector3 Start;
            public readonly Vector3 End;

            public AddHalfEdge(HalfEdge h)
            {
                Start = h.Twin.End.Position;
                End = h.End.Position;
            }

            public override void Undo(Mesh mesh, Stack<Change> changes)
            {
                mesh.GetEdge(mesh.GetVertex(Start), mesh.GetVertex(End)).Delete();
            }
        }

        private class AddFace
            : Change
        {
            public readonly Vertex[] Border;

            public AddFace(Face f)
            {
                Border = f.Vertices.ToArray();
            }

            public override void Undo(Mesh mesh, Stack<Change> changes)
            {
                mesh.GetFace(Border).Delete();
            }
        }

        private class DeleteFace
            :Change
        {
            public readonly Vector3[] Border;

            public DeleteFace(Face f)
            {
                Border = f.Vertices.Select(a => a.Position).ToArray();
            }

            public override void Undo(Mesh mesh, Stack<Change> changes)
            {
                var f = mesh.GetFace(Border.Select(a => mesh.GetVertex(a)));
            }
        }

        private class DeleteHalfEdge
            : Change
        {
            public readonly bool Skip;

            public readonly String StartName;
            public readonly Vector3 Start;

            public readonly String EndName;
            public readonly Vector3 End;

            public DeleteHalfEdge(HalfEdge e)
            {
                Skip = false;

                Vertex sV = e.Twin.End;
                if (sV != null)
                {
                    Start = sV.Position;
                    StartName = sV.Name;
                }
                else
                {
                    Start = default(Vector3);
                    StartName = null;
                    Skip = true;
                }

                Vertex eV = e.End;
                if (eV != null)
                {
                    End = eV.Position;
                    EndName = eV.Name;
                }
                else
                {
                    End = default(Vector3);
                    EndName = null;
                    Skip = true;
                }
            }

            public override void Undo(Mesh mesh, Stack<Change> changes)
            {
                if (!Skip)
                    mesh.GetEdge(mesh.GetVertex(Start), mesh.GetVertex(End));
            }
        }

        private class BeginSplitMidpointHalfEdge
            : Change
        {
            public BeginSplitMidpointHalfEdge(HalfEdge e, Vertex m)
            {
                //throw new NotImplementedException();
            }

            public override void Undo(Mesh mesh, Stack<Change> changes)
            {
                //throw new NotImplementedException();
            }
        }

        private class EndSplitMidpointHalfEdge
            : Change
        {
            public EndSplitMidpointHalfEdge(HalfEdge e, Vertex m)
            {
                //throw new NotImplementedException();
            }

            public override void Undo(Mesh mesh, Stack<Change> changes)
            {
                AddHalfEdge h = (AddHalfEdge)changes.Pop();
                BeginSplitMidpointHalfEdge b = (BeginSplitMidpointHalfEdge)changes.Pop();

                mesh.GetEdge(mesh.GetVertex(h.Start), mesh.GetVertex(h.End), false).Merge();

                mesh.CleanEdges();
                mesh.CleanVertices();
            }
        }
    }
}
