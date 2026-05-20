using NamelessGames.ScriptableSystem.Events;
using UnityEngine;

namespace NamelessGames.ScriptableSystem.Variables
{
    [CreateAssetMenu(fileName = "GameObject Variable", menuName = "Nameless Games/Scriptable System/Variables/GameObject Variable")]
    public class GameObjectVariable : BaseVariable<GameObject, GameObjectEvent> { }
}