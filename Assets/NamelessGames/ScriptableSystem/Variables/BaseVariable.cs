using NamelessGames.ScriptableSystem.Events;
using System.Collections.Generic;
using UnityEngine;

namespace NamelessGames.ScriptableSystem.Variables
{
    public abstract class BaseVariable : NGScriptableObject { }

    public abstract class BaseVariable<TArg, TEvent/*, TTEvent*/> : BaseVariable where TEvent : BaseEvent<TArg> //where TTEvent : BaseEvent<(TArg, TArg)>
    {
        [SerializeField] protected TArg _value;
        [SerializeField] TArg _startingValue;

        public TEvent ValueChanged;
        //public TTEvent ValueChangedWithLastValue;

        public TArg Value
        {
            get => _value;
            set
            {
                if (EqualityComparer<TArg>.Default.Equals(_value, value)) return;

                //TArg lastValue = _value;
                _value = value;
                ValueChanged?.Invoke(_value);
                //ValueChangedWithLastValue?.Invoke(lastValue, _value);
            }
        }

        public sealed override void Initialize()
        {
            Value = _startingValue;
        }

        public static implicit operator TArg(BaseVariable<TArg, TEvent> variable) => variable._value;

        public override bool Equals(object other)
        {            
            if (other is BaseVariable<TArg, TEvent> otherVariable)
            {
                return otherVariable._value.Equals(_value);
            }
            if (other is TArg otherArg)
            {
                return EqualityComparer<TArg>.Default.Equals(otherArg, _value);
            }
            return false;
        }

        public override int GetHashCode()
        {
            if (Value == null) return 0;
            return _value.GetHashCode();
        }
    }
}