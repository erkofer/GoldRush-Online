using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Caroline.Persistence.Redis
{
    public class ReadOnlyTypeSafeDictionary<TValue> : IReadOnlyDictionary<string, TValue>
    {
        readonly IReadOnlyDictionary<string, TValue> _scripts;

        public ReadOnlyTypeSafeDictionary(IReadOnlyDictionary<string, TValue> scripts, char? keyDelimiter)
        {
            _scripts = scripts;

            // assign scripts to the properties of inheritors.
            var properties = GetType().GetProperties();

            var propertyLookup = new Dictionary<string, PropertyInfo>();
            foreach (var prop in properties)
            {
                if (prop.PropertyType != typeof(TValue))
                    continue;
                propertyLookup.Add(prop.Name, prop);
            }

            if (keyDelimiter == null)
            {
                foreach (var script in scripts)
                {
                    PropertyInfo prop;
                    if(propertyLookup.TryGetValue(script.Key, out prop))
                        prop.SetValue(this, script.Value);
                }
            }
            else
            {
                foreach (var script in scripts)
                {
                    var split = script.Key.Split(keyDelimiter.Value);
                    for (int i = 0; i < split.Length; i++)
                    {
                        PropertyInfo prop;
                        var name = split[i];
                        if (propertyLookup.TryGetValue(name, out prop))
                            prop.SetValue(this, script.Value);
                    }
                }
            }
        }

        public bool ContainsKey(string key)
        {
            return _scripts.ContainsKey(key);
        }

        public bool TryGetValue(string key, out TValue value)
        {
            return _scripts.TryGetValue(key, out value);
        }

        public TValue this[string key]
        {
            get { return _scripts[key]; }
        }

        public IEnumerable<string> Keys { get { return _scripts.Keys; } }
        public IEnumerable<TValue> Values { get { return _scripts.Values; } }

        public IEnumerator<KeyValuePair<string, TValue>> GetEnumerator()
        {
            return _scripts.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int Count { get { return _scripts.Count; } }
    }
}
