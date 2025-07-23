[System.Serializable]
public class IntStringEventData : BaseEventData<IntStringEvent, (int, string)> { }

public class IntStringEventsListener : BaseEventsListener<IntStringEventData, IntStringEvent, (int, string)>
{
}