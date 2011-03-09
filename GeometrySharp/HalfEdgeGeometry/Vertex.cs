using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GeometrySharp.HalfEdgeGeometry
{
    public class Vertex
    {
        public Vector3 Position;
        public string Name = "";

        public IEnumerable<HalfEdge> IncomingEdges
        {
            get
            {
                return Mesh.VertexIncoming(this);
            }
        }

        public IEnumerable<HalfEdge> OutgoingEdges
        {
            get
            {
                return IncomingEdges.Select(a => a.Twin);
            }
        }

        public IEnumerable<Face> Neighbours
        {
            get
            {
                return Mesh.BorderingFaces(this);
            }
        }

        public readonly Mesh Mesh;

        protected internal Vertex(Vector3 position, string name, Mesh mesh)
        {
            Mesh = mesh;
            Position = position;
            Name = name;
        }

        public override string ToString()
        {
            return Name + " " + Position.ToString();
        }
    }
}
