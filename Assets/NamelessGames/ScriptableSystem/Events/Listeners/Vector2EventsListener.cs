using UnityEngine;

namespace NamelessGames.ScriptableSystem.Events
{
    [System.Serializable]
    public class Vector2EventData : BaseEventData<Vector2Event, Vector2> { }

    public class Vector2EventsListener : BaseEventsListener<Vector2EventData, Vector2Event, Vector2> { }
}