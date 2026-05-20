namespace NamelessGames.ScriptableSystem.Events
{
    [System.Serializable]
    public class UlongEventData : BaseEventData<UlongEvent, ulong> { }

    public class UlongEventsListener : BaseEventsListener<UlongEventData, UlongEvent, ulong> { }
}