using UnityEngine;

namespace NamelessGames.ScriptableSystem.Events
{
    public abstract class BaseEvent : NGScriptableObject { }

    public abstract class BaseEvent<TArg> : BaseEvent
    {
        event System.Action<TArg> OnEvent;
        bool _firstTime = true;
        TArg _lastMessage;

#if UNITY_EDITOR
        [SerializeField] TArg _fakeArg;
        public void SendFakeArg() => Invoke(_fakeArg);
#endif

        // TODO: Controllare se serve che metta tutto a default o meno
        public override void Initialize()
        {
            OnEvent = null;
            _firstTime = true;
            _lastMessage = default;
        }

        public void AddListener(System.Action<TArg> action, bool getLastEvent = false)
        {
            OnEvent += action;

            if (_firstTime || !getLastEvent) return;

            action.Invoke(_lastMessage);
        }

        public void CallActionWithLastEvent(System.Action<TArg> action)
        {
            if (_firstTime) return;
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
}