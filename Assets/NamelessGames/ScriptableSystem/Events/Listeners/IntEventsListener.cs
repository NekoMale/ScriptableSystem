[System.Serializable]
public class IntEventData : BaseEventData<IntEvent, int>
{
}

public class IntEventsListener : BaseEventsListener<IntEventData, IntEvent, int>
{
}