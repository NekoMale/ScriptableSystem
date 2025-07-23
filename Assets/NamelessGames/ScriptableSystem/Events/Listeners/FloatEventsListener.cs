[System.Serializable]
public class FloatEventData : BaseEventData<FloatEvent, float> { }

public class FloatEventsListener : BaseEventsListener<FloatEventData, FloatEvent, float> { }