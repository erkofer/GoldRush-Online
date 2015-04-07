using System;
using JetBrains.Annotations;

namespace Caroline.Persistence.Models
{
    internal struct Pair<T>
        where T : class
    {
        public Pair([NotNull] T newObj, [CanBeNull] T oldObj)
        {
            if (newObj == null) throw new ArgumentNullException("newObj");
            New = newObj;
            Old = oldObj;
        }

        public Pair([NotNull] T newObj)
        {
            if (newObj == null) throw new ArgumentNullException("newObj");
            New = newObj;
            Old = default(T);
        }

        public T New;
        [CanBeNull]
        public T Old;
    }
}