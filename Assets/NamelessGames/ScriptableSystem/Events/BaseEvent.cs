using UnityEngine;

public abstract class BaseEvent<TArg> : BaseEvent
{
#if UNITY_EDITOR
    [SerializeField] TArg _fakeArg;

    public sealed override void SendFakeArg()
    {
        Invoke(_fakeArg);
    }
#endif

    event System.Action<TArg> OnEvent;
    bool _firstTime = true;
    TArg _lastMessage;

    public void AddListener(System.Action<TArg> action, bool getLastEvent = false)
    {
        OnEvent += action;
        
        if (_firstTime || !getLastEvent) return;
        
        action.Invoke(_lastMessage);
    }
    public void Invoke(TArg arg)
    {
        OnEvent?.Invoke(arg);
        _lastMessage = arg;
        _firstTime = false;
    }

    public void RemoveListener(System.Action<TArg> action)
    {
        OnEvent -= action;
    }
}