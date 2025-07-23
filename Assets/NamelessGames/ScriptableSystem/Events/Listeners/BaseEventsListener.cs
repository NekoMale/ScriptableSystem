using System.Linq;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public abstract class BaseEventData<TEvent, TArg> where TEvent : BaseEvent<TArg>
{
    public TEvent Event;
    public bool SubscribeOnEnable;
    public bool UnsubscribeOnDisable;
    public bool AskLastEventOnSubscribe;
    public UnityEvent<TArg> Response;
}

public abstract class BaseEventsListener : MonoBehaviour { }

[ExecuteAlways]
public abstract class BaseEventsListener<TEventData, TEvent, TArg> : BaseEventsListener
    where TEventData : BaseEventData<TEvent, TArg>
    where TEvent : BaseEvent<TArg>
{
    [SerializeField] TEventData[] _baseEventsDatas;

    bool _firstEnable = true;

    void OnEnable()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying) return;
#endif

        foreach (TEventData baseEventData in _baseEventsDatas)
        {
            if (!baseEventData.SubscribeOnEnable) continue;
            if (!baseEventData.UnsubscribeOnDisable && !_firstEnable) continue;
            baseEventData.Event.AddListener(baseEventData.Response.Invoke, baseEventData.AskLastEventOnSubscribe);
        }
        _firstEnable = false;
    }

    void OnDisable()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying) return;
#endif

        foreach (TEventData baseEventData in _baseEventsDatas)
        {
            if (!baseEventData.UnsubscribeOnDisable) continue;
            baseEventData.Event.RemoveListener(baseEventData.Response.Invoke);
        }
    }

    public void EnableAll()
    {
        for (int eventIndex = 0; eventIndex < _baseEventsDatas.Length; eventIndex++)
        {
            TEventData baseEventData = _baseEventsDatas[eventIndex];
            baseEventData.Event.AddListener(baseEventData.Response.Invoke);
        }
    }

    public void Enable(TEvent eventToEnable)
    {
        TEventData eventData = _baseEventsDatas.FirstOrDefault(x => x.Event == eventToEnable);
        if (eventData == null) return;
        eventToEnable.AddListener(eventData.Response.Invoke);
    }

    public void DisableAll()
    {
        for (int eventIndex = 0; eventIndex < _baseEventsDatas.Length; eventIndex++)
        {
            TEventData baseEventData = _baseEventsDatas[eventIndex];
            baseEventData.Event.RemoveListener(baseEventData.Response.Invoke);
        }
    }

    public void Disable(TEvent eventToDisable)
    {
        TEventData eventData = _baseEventsDatas.FirstOrDefault(x => x.Event == eventToDisable);
        if (eventData == null) return;
        eventToDisable.RemoveListener(eventData.Response.Invoke);
    }
}