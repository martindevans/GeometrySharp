using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeometrySharp.HalfEdgeGeometry;
using System.Collections.Concurrent;

namespace GeometrySharp.Procedural
{
    public class ProceduralFace
        :Face
    {
        private FaceDevelopment development;
        public FaceDevelopment Development
        {
            get
            {
                return development;
            }
            set
            {
                if (development != null)
                    throw new InvalidOperationException("Cannot set the expansion for a face twice");
                development = value;
            }
        }

        private ConcurrentDictionary<String, ISet<String>> tags = new ConcurrentDictionary<string, ISet<string>>();

        public ProceduralFace(Mesh m)
            :base(m)
        {

        }

        #region tags
        public void Tag(String nameSpace, String t)
        {
            tags[nameSpace].Add(t);
        }

        public ISet<String> GetNameSpace(String nameSpace)
        {
            return tags.GetOrAdd(nameSpace, a => new HashSet<String>());
        }

        public bool HasTag(String nameSpace, String t)
        {
            ISet<String> set;
            if (tags.TryGetValue(nameSpace, out set))
                return set.Contains(t);
            return false;
        }

        public void Untag(String nameSpace, String t)
        {
            ISet<String> set;
            if (tags.TryGetValue(nameSpace, out set))
                set.Remove(t);
        }
        #endregion

        public FaceDiminishment Develop()
        {
            if (Development == null)
                throw new NullReferenceException("Development is null");

            var dim = Development.Apply(this);

            return dim;
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}
