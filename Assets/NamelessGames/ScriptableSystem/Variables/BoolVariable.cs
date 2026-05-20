using NamelessGames.ScriptableSystem.Events;
using UnityEngine;

namespace NamelessGames.ScriptableSystem.Variables
{
    [CreateAssetMenu(fileName = "Bool Variable", menuName = "Nameless Games/Scriptable System/Variables/Bool Variable")]
    public class BoolVariable : BaseVariable<bool, BoolEvent>
    {
        public static bool operator !(BoolVariable variable)
        {
            return !variable.Value;
        }

    }
}