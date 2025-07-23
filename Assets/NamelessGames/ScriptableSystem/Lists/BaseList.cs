using System.Collections.Generic;
using UnityEngine;

public abstract class BaseList<TArg> : ScriptableObject
{
    [SerializeField] protected List<TArg> _items = new List<TArg>();

    public VoidEvent OnCollectionChanged = null;

    public TArg this[int index]
    {
        get => _items[index];
        set
        {
            _items[index] = value;
            OnCollectionChanged.Invoke(VoidArg.Empty);
        }
    }

    public List<TArg> Values => _items;
    public TArg[] ValuesArray => _items.ToArray();

    public int Count => _items.Count;

    public void Add(TArg item)
    {
        _items.Add(item);
        OnCollectionChanged.Invoke(VoidArg.Empty);
    }

    public void Insert(TArg item, int index) => AddAt(item, index);

    public void AddAt(TArg item, int index)
    {
        _items.Insert(index, item);
        OnCollectionChanged.Invoke(VoidArg.Empty);
    }

    public void AddRange(IEnumerable<TArg> items)
    {
        _items.AddRange(items);
        OnCollectionChanged.Invoke(VoidArg.Empty);
    }

    public void Remove(TArg item) {
        _items.Remove(item);
        OnCollectionChanged.Invoke(VoidArg.Empty);
    }

    public void RemoveAt(int index) {
        _items.RemoveAt(index);
        OnCollectionChanged.Invoke(VoidArg.Empty);
    }

    public void Clear() {
        _items.Clear();
        OnCollectionChanged.Invoke(VoidArg.Empty);
    }

    public bool Contains(TArg item) => _items.Contains(item);
}