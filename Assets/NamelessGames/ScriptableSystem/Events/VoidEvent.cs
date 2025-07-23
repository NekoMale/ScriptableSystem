using UnityEngine;

[CreateAssetMenu(fileName = "Void Event", menuName = "Nameless Games/Scriptable System/Events/Void Event", order = 0)]
public class VoidEvent : BaseEvent<VoidArg>
{
    public void Invoke() => Invoke(VoidArg.Empty);
}
