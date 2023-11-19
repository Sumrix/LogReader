﻿using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;

namespace LogReader.Core.Helpers;

public class AppendOnlyObservableCollection<T> : IList, IReadOnlyList<T>, INotifyCollectionChanged,
    INotifyPropertyChanged
{
    private readonly SynchronizationContext _context = SynchronizationContext.Current ?? new SynchronizationContext();
    private readonly List<T> _items;
    private readonly object _syncRoot = new();

    public AppendOnlyObservableCollection()
    {
        _items = new();
    }

    public AppendOnlyObservableCollection(int capacity)
    {
        _items = new(capacity);
    }

    public AppendOnlyObservableCollection(IEnumerable<T> collection)
    {
        _items = new(collection);
        Count = _items.Count;
    }

    public event NotifyCollectionChangedEventHandler? CollectionChanged;
    public event PropertyChangedEventHandler? PropertyChanged;

    public IEnumerator<T> GetEnumerator() => new SnapEnumerator(_items);

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    void ICollection.CopyTo(Array array, int index) => throw new NotSupportedException();

    public int Count { get; private set; }

    bool ICollection.IsSynchronized => throw new NotSupportedException();

    object ICollection.SyncRoot => throw new NotSupportedException();

    int IList.Add(object? value) => throw new NotSupportedException();

    void IList.Clear() => throw new NotSupportedException();

    bool IList.Contains(object? value) => throw new NotSupportedException();

    int IList.IndexOf(object? value) => ((IList)_items).IndexOf(value);

    void IList.Insert(int index, object? value) => throw new NotSupportedException();

    void IList.Remove(object? value) => throw new NotSupportedException();

    void IList.RemoveAt(int index) => throw new NotSupportedException();

    bool IList.IsFixedSize => throw new NotSupportedException();

    bool IList.IsReadOnly => throw new NotSupportedException();

    object? IList.this[int index]
    {
        get => this[index];
        set => throw new NotSupportedException();
    }

    public T this[int index] => (uint)index >= Count
        ? throw new ArgumentOutOfRangeException(nameof(index))
        : _items[index];

    public void AddRange(IEnumerable<T> collection, bool useReset = false)
    {
        if (collection == null)
        {
            throw new ArgumentNullException(nameof(collection));
        }

        lock (_syncRoot)
        {
            if (useReset)
            {
                var oldCount = _items.Count;
                _items.AddRange(collection);

                if (oldCount == _items.Count)
                {
                    return;
                }

                Count = _items.Count;

                _context.Post(_ => { OnCollectionReset(); }, null);
            }
            else
            {
                var startIndex = Count;
                var list = collection as IList<T> ?? new List<T>(collection);

                if (list.Count == 0)
                {
                    return;
                }

                _items.AddRange(list);
                Count = _items.Count;

                _context.Post(_ =>
                {
                    OnCountPropertyChanged();
                    OnIndexerPropertyChanged();
                    OnCollectionChanged(new(NotifyCollectionChangedAction.Add, (IList)list, startIndex));
                }, null);
            }
        }
    }

    private void OnIndexerPropertyChanged() => OnPropertyChanged(new("Item[]"));

    private void OnCountPropertyChanged() => OnPropertyChanged(new(nameof(Count)));

    private void OnCollectionReset()
    {
        OnCountPropertyChanged();
        OnIndexerPropertyChanged();
        OnCollectionChanged(new(NotifyCollectionChangedAction.Reset));
    }

    private void OnPropertyChanged(PropertyChangedEventArgs e) => PropertyChanged?.Invoke(this, e);

    private void OnCollectionChanged(NotifyCollectionChangedEventArgs e) => CollectionChanged?.Invoke(this, e);

    private class SnapEnumerator : IEnumerator<T>
    {
        private readonly int _count;
        private readonly List<T> _list;
        private T? _current;
        private int _index;

        public SnapEnumerator(List<T> list)
        {
            _list = list;
            _count = list.Count;
            _index = 0;
            _current = default;
        }

        public void Dispose()
        {
        }

        public bool MoveNext()
        {
            if ((uint)_index < (uint)_count)
            {
                _current = _list[_index];
                _index++;
                return true;
            }

            _index = _count + 1;
            _current = default;
            return false;
        }

        public T Current => _current!;

        object? IEnumerator.Current => _current;

        void IEnumerator.Reset()
        {
            _index = 0;
            _current = default;
        }
    }
}