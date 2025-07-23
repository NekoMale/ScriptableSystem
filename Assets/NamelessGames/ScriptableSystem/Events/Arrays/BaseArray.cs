using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class BaseArray<TArg> : ScriptableObject
{
    [SerializeField] protected TArg[] _items = new TArg[0];

    public VoidEvent OnCollectionChanged = null;

    public void Set(TArg[] items)
    {
        _items = items;
        OnCollectionChanged.Invoke(VoidArg.Empty);
    }

    public TArg this[int index]
    {
        get => _items[index];
        set
        {
            _items[index] = value;
            OnCollectionChanged.Invoke(VoidArg.Empty);
        }
    }

    public TArg[] Values => _items;
    public List<TArg> ValuesList => _items.ToList();

    public int Length => _items.Length;

    public bool Contains(TArg item)
    {
        return _items.Contains(item);
    }
}