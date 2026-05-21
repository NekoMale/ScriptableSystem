using NamelessGames.ScriptableSystem.Events;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NamelessGames.ScriptableSystem.Collections
{
    public abstract class BaseArray : NGScriptableObject { }

    public abstract class BaseArray<TArg> : BaseArray
    {
        [SerializeField] protected TArg[] _items = new TArg[0];
        [SerializeField] TArg[] _startingItems;

        public VoidEvent OnCollectionChanged = null;

        public override void Initialize()
        {
            _items = new TArg[_startingItems.Length];
            for (int itemIndex = 0; itemIndex < _startingItems.Length; itemIndex++)
            {
                _items[itemIndex] = _startingItems[itemIndex];
            }
            OnCollectionChanged?.Invoke();
        }

        public void Set(TArg[] items)
        {
            _items = items;
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

        public TArg[] Values => _items;
        public List<TArg> ValuesList => _items.ToList();

        public int Length => _items.Length;

        public bool Contains(TArg item)
        {
            return _items.Contains(item);
        }
    }
}