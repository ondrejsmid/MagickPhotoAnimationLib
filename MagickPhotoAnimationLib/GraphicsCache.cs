using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagickPhotoAnimationLib
{
    internal class GraphicsCache
    {
        private Dictionary<string, MagickImageAndVector> _cache = new Dictionary<string, MagickImageAndVector>();

        public void Set(string name, MagickImageAndVector magickImageAndVector)
        {
            _cache[name] = magickImageAndVector;
        }

        public MagickImageAndVector Get(string name)
        {
            if (!_cache.ContainsKey(name))
            {
                throw new InvalidOperationException($"A key {name} isn't cached in {nameof(GraphicsCache)}");
            }
            return _cache[name];
        }
    }
}
