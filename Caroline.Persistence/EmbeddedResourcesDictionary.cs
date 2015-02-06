using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Caroline.Persistence
{
    class EmbeddedResourcesDictionary : IReadOnlyDictionary<string, string>
    {
        readonly Dictionary<string, string> _resources = new Dictionary<string, string>();

        public EmbeddedResourcesDictionary(Type namespaceScope)
            : this(namespaceScope.Assembly, namespaceScope.Namespace)
        {
        }

        public EmbeddedResourcesDictionary(Assembly containingAssembly, string namespaceScope)
        {
            if (namespaceScope == null)
                throw new ArgumentException("namespaceScope may not be a generic parameter.", "namespaceScope");
            namespaceScope += '.';
            var namespaceTrim = namespaceScope.Length;
            // +1 to remove trailing '.' after namespace but before type.Name, eg with namespace MyApp.Resources
            var resources = containingAssembly.GetManifestResourceNames();
            for (int i = 0; i < resources.Length; i++)
            {
                var name = resources[i];
                if (!name.StartsWith(namespaceScope))
                    continue; // ignore resources that are out of scope/
                using (var stream = containingAssembly.GetManifestResourceStream(name))
                {
                    if (stream == null) // should never be null
                        throw new ArgumentException();
                    using (var reader = new StreamReader(stream))
                    {
                        _resources.Add(name.Substring(namespaceTrim), reader.ReadToEnd());
                    }
                }
            }
        }

        public EmbeddedResourcesDictionary(Assembly containingAssembly)
        {
            var resources = containingAssembly.GetManifestResourceNames();
            for (int i = 0; i < resources.Length; i++)
            {
                var name = resources[i];
                using (var stream = containingAssembly.GetManifestResourceStream(name))
                {
                    if (stream == null) // should never be null
                        throw new ArgumentException();
                    using (var reader = new StreamReader(stream))
                    {
                        _resources.Add(name, reader.ReadToEnd());
                    }
                }
            }
        }


        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return _resources.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int Count { get { return _resources.Count; } }
        public bool ContainsKey(string key)
        {
            return _resources.ContainsKey(key);
        }

        public bool TryGetValue(string key, out string value)
        {
            return _resources.TryGetValue(key, out value);
        }

        public string this[string key]
        {
            get { return _resources[key]; }
        }

        public IEnumerable<string> Keys { get { return _resources.Keys; } }
        public IEnumerable<string> Values { get { return _resources.Values; } }
    }
}
