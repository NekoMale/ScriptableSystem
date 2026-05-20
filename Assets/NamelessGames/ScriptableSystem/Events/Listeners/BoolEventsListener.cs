namespace NamelessGames.ScriptableSystem.Events
{
    [System.Serializable]
    public class BoolEventData : BaseEventData<BoolEvent, bool> { }

    public class BoolEventsListener : BaseEventsListener<BoolEventData, BoolEvent, bool> { }
}