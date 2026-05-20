namespace NamelessGames.ScriptableSystem.Events
{
    [System.Serializable]
    public class IntStringEventData : BaseEventData<IntStringEvent, (int num, string str)> { }

    public class IntStringEventsListener : BaseEventsListener<IntStringEventData, IntStringEvent, (int num, string str)>
    {
    }
}