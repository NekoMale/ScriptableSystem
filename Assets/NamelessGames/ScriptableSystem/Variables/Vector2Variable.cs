using NamelessGames.ScriptableSystem.Events;
using UnityEngine;

namespace NamelessGames.ScriptableSystem.Variables
{
    [CreateAssetMenu(fileName = "Vector2 Variable", menuName = "Nameless Games/Scriptable System/Variables/Vector2 Variable")]
    public class Vector2Variable : BaseVariable<Vector2, Vector2Event>
    {
        public float x
        {
            get => _value.x;
            set
            {
                Vector2 vector = _value;
                vector.x = value;
                Value = vector;
            }
        }
        public float y
        {
            get => _value.y;
            set
            {
                Vector2 vector = _value;
                vector.y = value;
                Value = vector;
            }
        }

        public static implicit operator Vector3(Vector2Variable variable) { return variable._value; }
    }
}