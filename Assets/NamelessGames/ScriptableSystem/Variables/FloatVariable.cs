using NamelessGames.ScriptableSystem.Events;
using UnityEngine;

namespace NamelessGames.ScriptableSystem.Variables
{
    [CreateAssetMenu(fileName = "Float Variable", menuName = "Nameless Games/Scriptable System/Variables/Float Variable")]
    public class FloatVariable : BaseVariable<float, FloatEvent> { }
}