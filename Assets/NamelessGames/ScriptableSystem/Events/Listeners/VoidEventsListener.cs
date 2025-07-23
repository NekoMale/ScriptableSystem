[System.Serializable]
public struct VoidArg
{
    public static readonly VoidArg Empty = new VoidArg();
}

[System.Serializable]
public class VoidEventData : BaseEventData<VoidEvent, VoidArg>
{
}

public class VoidEventsListener : BaseEventsListener<VoidEventData, VoidEvent, VoidArg>
{
}
