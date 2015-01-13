using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using JetBrains.Annotations;

namespace Caroline.App.Models
{
    public interface ICompressable<T>
    {
        [CanBeNull]
        T Compress([NotNull]T oldObject);
    }

    internal static class CompressableHelpers
    {
        public static IEnumerator<Pair<T>> SortByPairFast<T>(List<T> newList, List<T> oldList)
            where T : class, IIdentifiableObject
        {
            VerifyListAscendingIds(newList, oldList);
            return new PairEnumerable<T>(oldList, newList);
        }

        [CanBeNull]
        public static void CompressList<T>([NotNull] List<T> currentList, [NotNull] List<T> oldList, List<T> destinationList)
            where T : class, ICompressable<T>, IIdentifiableObject
        {
            VerifyListAscendingIds(currentList, oldList);

            var searchedOldIndex = 0;
            for (int i = 0; i < currentList.Count; i++)
            {
                var item = currentList[i];
                var itemId = item.Id;
                Debug.Assert(itemId >= 0);
                T oldItem = null;
                var oldItemId= -1;
                for (; searchedOldIndex < oldList.Count; searchedOldIndex++)
                {
                    oldItem = oldList[searchedOldIndex];
                    oldItemId = oldItem.Id;
                    Debug.Assert(oldItemId >= 0);
                    if (itemId == oldItemId)
                        break;
                }
                
                // if the loop ended prematurely, then itemId != oldItemid
                if (itemId != oldItemId)
                {
                    if(destinationList==null)
                        destinationList=new List<T>();
                    destinationList.Add(item);
                }
                else
                {
                    // oldItem is never null since IIdentifiableEntity.Id
                    // never returns a negative number and wont get above the above if
                    var newItem = item.Compress(oldItem);
                    if (newItem != null)
                    {
                        if(destinationList == null)
                            destinationList = new List<T>();
                        destinationList.Add(newItem);
                    }
                }
            }
        }

        public struct PairEnumerable<T> : IEnumerator<Pair<T>>, IEnumerable<Pair<T>>
            where T : class, IIdentifiableObject
        {
            int _searchedOldIndex;
            int _newIndex;
            readonly List<T> _oldList;
            readonly List<T> _newList;
            Pair<T> _Current;

            public PairEnumerable([NotNull] List<T> newList, [NotNull] List<T> oldList)
            {
                if (newList == null) throw new ArgumentNullException("newList");
                if (oldList == null) throw new ArgumentNullException("oldList");
                _newList = newList;
                _oldList = oldList;

                _searchedOldIndex = 0;
                _newIndex = 0;
                _Current = default(Pair<T>);
            }

            public void Dispose()
            {
            }

            public bool MoveNext()
            {
                if (_newIndex >= _newList.Count)
                    return false;

                var item = _newList[_newIndex];
                var itemId = item.Id;
                for (; _searchedOldIndex < _oldList.Count; _searchedOldIndex++)
                {
                    var oldItem = _oldList[_searchedOldIndex];
                    if (itemId != oldItem.Id) continue;

                    _newIndex++;
                    _searchedOldIndex++;
                    _Current = new Pair<T>(item, oldItem);
                    return true;
                }
                // didnt find any old objects that had matching item.Id
                _newIndex++;
                _Current = new Pair<T>(item);
                return true;
            }

            public void Reset()
            {
                _searchedOldIndex = 0;
                _newIndex = 0;
            }

            public Pair<T> Current
            {
                get { return _Current; }
            }

            object IEnumerator.Current
            {
                get { return Current; }
            }

            public PairEnumerable<T> GetEnumerator()
            {
                return this;
            }

            IEnumerator<Pair<T>> IEnumerable<Pair<T>>.GetEnumerator()
            {
                return this;
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        [Conditional("DEBUG")]
        static void VerifyListAscendingIds<T>(List<T> newList, List<T> oldList)
            where T : IIdentifiableObject
        {
            for (int i = 0; i < 2; i++)
            {
                var list = i == 0 ? oldList : newList;
                var lastId = 0;
                for (var j = 0; j < list.Count; j++)
                {
                    var id = list[j].Id;
                    if (id <= lastId)
                        Debug.Assert(false);
                    lastId = id;
                }
            }
        }
    }

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
