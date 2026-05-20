using UnityEngine;

namespace NamelessGames.ScriptableSystem.Events
{
    [CreateAssetMenu(fileName = "Int-String Event", menuName = "Nameless Games/Scriptable System/Events/Int-String Event")]
    public class IntStringEvent : BaseEvent<(int num, string str)>
    {
    }
}