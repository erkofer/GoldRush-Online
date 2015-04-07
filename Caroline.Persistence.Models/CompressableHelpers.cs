using JetBrains.Annotations;
using System.Collections.Generic;
using System.Diagnostics;

namespace Caroline.Persistence.Models
{
    public static class CompressableHelpers
    {
        public static void CompressList<T>([NotNull] List<T> currentList, [NotNull] List<T> oldList, List<T> destinationList)
            where T : class, ICompressable<T>, IIdentifiableObject
        {
            Debug.Assert(destinationList != null);
            Debug.Assert(VerifyListAscendingIds(currentList));
            Debug.Assert(VerifyListAscendingIds(oldList));

            var searchedOldIndex = 0;
            for (int i = 0; i < currentList.Count; i++)
            {
                var item = currentList[i];
                var itemId = item.Id;
                Debug.Assert(itemId > 0);
                T oldItem = null;
                var oldItemId = -1;
                for (; searchedOldIndex < oldList.Count; searchedOldIndex++)
                {
                    oldItem = oldList[searchedOldIndex];
                    oldItemId = oldItem.Id;
                    Debug.Assert(oldItemId >= 0);
                    if (itemId > oldItemId) continue;

                    // if pair found (itemId == oldItemId) compress them
                    // if itemId < oldItemId then a pair wont be found in the rest of 
                    // the list as oldItemId only increases as i increases
                    searchedOldIndex++;
                    break;
                }

                // if the loop ended prematurely, then itemId != oldItemid
                // check if oldItem is null, in the rare chance that oldList.Count==0
                if (itemId != oldItemId || oldItem == null)
                {
                    destinationList.Add(item);
                }
                else
                {
                    // oldItem is never null since IIdentifiableEntity.Id
                    // never returns a negative number and wont get above the above if
                    var newItem = item.Compress(oldItem);
                    if (newItem != null)
                    {
                        destinationList.Add(newItem);
                    }
                }
            }
            Debug.Assert(VerifyListAscendingIds(destinationList));
        }

        /*
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
        */

        public static bool VerifyListAscendingIds<T>(List<T> idList)
            where T : IIdentifiableObject
        {
            var lastId = 0;
            for (var j = 0; j < idList.Count; j++)
            {
                var id = idList[j].Id;
                if (id <= lastId)
                    return false;
                lastId = id;
            }
            return true;
        }
    }
}