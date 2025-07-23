using UnityEngine;

[CreateAssetMenu(fileName = "Vector2 Variable", menuName = "Nameless Games/Scriptable System/Variables/Vector2 Variable")]
public class Vector2Variable : BaseVariable<Vector2, Vector2Event>
{
    public float x
    {
        get => _value.x;
        set => _value.x = value;
    }
    public float y
    {
        get => _value.y;
        set => _value.y = value;
    }

    public static implicit operator Vector3(Vector2Variable variable) { return variable._value; }
}