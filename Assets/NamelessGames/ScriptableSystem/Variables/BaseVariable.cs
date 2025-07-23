using System;
using UnityEngine;

/// <summary>
/// Don't inherit from this. Inherit from BaseVariable<TArg, TEvent> instead
/// </summary>
public abstract class BaseVariable : ScriptableObject
{
    public abstract void Initialize();
}

public abstract class BaseVariable<TArg, TEvent> : BaseVariable where TEvent : BaseEvent<TArg>
{
    [SerializeField] protected TArg _value;
    [SerializeField] TArg _startingValue;

    public TEvent ValueChanged;
    //public event Action<T, T> ValueChangedWithLastValue;

    public TArg Value
    {
        get => _value;
        set
        {
            TArg lastValue = _value;
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
            return otherArg.Equals(_value);
        }
        return false;
    }

    public override int GetHashCode()
    {
        if (Value == null) return 0;
        return _value.GetHashCode();
    }
}