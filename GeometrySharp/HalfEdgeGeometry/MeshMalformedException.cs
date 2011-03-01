using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeometrySharp.HalfEdgeGeometry
{
    class MeshMalformedException : InvalidOperationException
    {

        public MeshMalformedException(String message)
            :base(message)
        {

        }
    }
}
