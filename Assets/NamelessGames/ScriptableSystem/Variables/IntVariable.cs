using NamelessGames.ScriptableSystem.Events;
using UnityEngine;

namespace NamelessGames.ScriptableSystem.Variables
{
    [CreateAssetMenu(fileName = "Int Variable", menuName = "Nameless Games/Scriptable System/Variables/Int Variable")]
    public class IntVariable : BaseVariable<int, IntEvent>
    {
        public static IntVariable operator ++(IntVariable variable)
        {
            variable.Value++;
            return variable;
        }

        public static int operator +(IntVariable variable)
        {
            return variable.Value;
        }

        public static int operator +(IntVariable left, IntVariable right)
        {
            return left.Value + right.Value;
        }

        public static IntVariable operator --(IntVariable variable)
        {
            variable.Value--;
            return variable;
        }

        public static int operator -(IntVariable variable)
        {
            return variable.Value;
        }

        public static int operator -(IntVariable left, IntVariable right)
        {
            return left.Value - right.Value;
        }
    }
}