using NamelessGames.ScriptableSystem.Events;
using System.Collections.Generic;
using UnityEngine;

namespace NamelessGames.ScriptableSystem.Collections
{
    public abstract class BaseList : NGScriptableObject { }

    public abstract class BaseList<TArg> : BaseList
    {
        [SerializeField] protected List<TArg> _items = new List<TArg>();
        [SerializeField] TArg[] _startingItems;

        public VoidEvent OnCollectionChanged = null;

        public override void Initialize()
        {
            _items = new List<TArg>();
            for (int itemIndex = 0; itemIndex < _startingItems.Length; itemIndex++)
            {
                _items.Add(_startingItems[itemIndex]);
            }
            OnCollectionChanged?.Invoke();
        }

        public TArg this[int index]
        {
            get => _items[index];
            set
            {
                _items[index] = value;
                OnCollectionChanged?.Invoke();
            }
        }

        public List<TArg> Values => _items;
        public TArg[] ValuesArray => _items.ToArray();

        public int Count => _items.Count;

        public void Add(TArg item)
        {
            _items.Add(item);
            OnCollectionChanged?.Invoke();
        }

        public void Insert(TArg item, int index) => AddAt(item, index);

        public void AddAt(TArg item, int index)
        {
            _items.Insert(index, item);
            OnCollectionChanged?.Invoke();
        }

        public void AddRange(IEnumerable<TArg> items)
        {
            _items.AddRange(items);
            OnCollectionChanged?.Invoke();
        }

        public void Remove(TArg item)
        {
            _items.Remove(item);
            OnCollectionChanged?.Invoke();
        }

        public void RemoveAt(int index)
        {
            _items.RemoveAt(index);
            OnCollectionChanged?.Invoke();
        }

        public void Clear()
        {
            _items.Clear();
            OnCollectionChanged?.Invoke();
        }

        public bool Contains(TArg item) => _items.Contains(item);
    }
}