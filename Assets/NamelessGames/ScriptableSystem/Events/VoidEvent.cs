using UnityEngine;

namespace NamelessGames.ScriptableSystem.Events
{
    [CreateAssetMenu(fileName = "Void Event", menuName = "Nameless Games/Scriptable System/Events/Void Event", order = 0)]
    public class VoidEvent : BaseEvent<VoidArg>
    {
        event System.Action OnVoidEvent;

        public void AddListener(System.Action listener)
        {
            OnVoidEvent += listener;
        }

        public void Invoke()
        {
            Invoke(VoidArg.Empty);
        }

        public new void Invoke(VoidArg arg)
        {
            OnVoidEvent?.Invoke();
            base.Invoke(arg);
        }

        public void RemoveListener(System.Action listener)
        {
            OnVoidEvent -= listener;
        }
    }
}