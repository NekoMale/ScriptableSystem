using UnityEngine;

[System.Serializable]
public class GameObjectEventData : BaseEventData<GameObjectEvent, GameObject> { }

public class GameObjectEventsListener : BaseEventsListener<GameObjectEventData, GameObjectEvent, GameObject> { }