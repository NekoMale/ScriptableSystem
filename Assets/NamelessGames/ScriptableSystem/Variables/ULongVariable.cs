using NamelessGames.ScriptableSystem.Events;
using UnityEngine;

namespace NamelessGames.ScriptableSystem.Variables
{
    [CreateAssetMenu(fileName = "ULong Variable", menuName = "Nameless Games/Scriptable System/Variables/ULong Variable")]
    public class ULongVariable : BaseVariable<ulong, UlongEvent> { }
}