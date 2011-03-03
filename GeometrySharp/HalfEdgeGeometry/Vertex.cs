﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GeometrySharp.HalfEdgeGeometry
{
    public class Vertex
    {
        public Vector3 Position;

        protected internal Vertex(Vector3 position)
        {
            Position = position;
        }

        public override string ToString()
        {
            return Position.ToString();
        }
    }
}
