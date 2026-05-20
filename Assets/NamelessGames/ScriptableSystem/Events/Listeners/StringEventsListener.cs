namespace NamelessGames.ScriptableSystem.Events
{
    [System.Serializable]
    public class StringEventData : BaseEventData<StringEvent, string> { }

    public class StringEventsListener : BaseEventsListener<StringEventData, StringEvent, string> { }
}