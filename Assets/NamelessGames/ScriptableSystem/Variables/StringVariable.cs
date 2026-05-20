using NamelessGames.ScriptableSystem.Events;
using UnityEngine;

namespace NamelessGames.ScriptableSystem.Variables
{
    [CreateAssetMenu(fileName = "String Variable", menuName = "Nameless Games/Scriptable System/Variables/String Variable")]
    public class StringVariable : BaseVariable<string, StringEvent> { }
}