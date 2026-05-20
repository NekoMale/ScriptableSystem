using UnityEngine;

namespace NamelessGames.ScriptableSystem.Events
{
    [System.Serializable]
    public class GameObjectEventData : BaseEventData<GameObjectEvent, GameObject> { }

    public class GameObjectEventsListener : BaseEventsListener<GameObjectEventData, GameObjectEvent, GameObject> { }
}