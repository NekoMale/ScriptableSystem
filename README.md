# Scriptable Object Architecture System (ALPHA VERSION - DONT USE IN NON-TEST PROJECT)

## [EN] English Version
 
### Short Description
An advanced Unity ScriptableObject system that enables decoupling and easy testing through independen variables, events, lists and arrays.
**Dictionary on roadmap**

### Key Features
- **System Decoupling**: GameObjects no more need direct dependencies with other GameObjects just for few variables data to know or for Events subscription
- **Event System**: ScriptableObject-based events can be easily raised also through simple Unity Buttons click without scripting code
- **Auto OnValueChange Call**: Variables automatically invoke event when value changes
- **Generic Types**: Fully generic type system, supporting any custom data type
- **Reference Tracking**: Automatically tracks who reference (this system) ScriptableObjects (\*)
- **ScriptableObjectViewer Tool**: Integrated tool to identify references on (this system) ScriptableObjects (\*)
- **ScriptableObjectCreator Tool**: Integrated tool to automatically create Variable and Event with your custom data type

\*Actually automatic reference tracking works just with events. Further details below

### Advantages
- **Modularity**: Completely independent systems
- **Reusability**: ScriptableObjects are reusable across scenes without requiring any mid saving systems
- **Easier debugging**: You don't need full scene to test just a system. Events are `invokable` also through inspector without a real invoker
- **Performace**: No `FindObject***` and `GetComponent` needed

### Installation
1. Download repositiory
2. Import folder `NamelessGames` into you Unity project's `Assets` folder
3. The system will be automatically available

### Main Architecture

System based on four (five in future) main generic classes:

#### BaseVariable<TArg, TEvent> (Advanced work status)
Contains a value of type TArg and automatically invokes a TEvent (if set) when value changes, passing new TArg value.

```csharp
// Usage example
[SerializeField] private IntVariable playerHealth;

void GetDamage(int damage)
{
    playerHealth.Value -= damage; // Automatically invokes ValueChanged event
}
```

Default BaseVariable methods, properties and variables:
| Visibility | Type | Name | Description |
| --- | --- | --- | --- |
| private | variable | \_startingValue | Variable starting value |
| protected | variable | \_value | Default container for variable value |
| public | variable | ValueChanged | If set, event invoked when value changes its value |
| public | property | Value | Property to set and get variable value. On set, automatically invoke ValueChanged event |
| public | *sealed* method | Initialize() | Method called to set starting value on variable |
| public | implicit operator | TArg | Automatically convert BaseVartiable<TArg, TEvent> into its value |
| public | method | Equals(object other) | Default C# method overriden to check if variable value is equals to other variable value. Returns true also if BaseVariable<TArg, TEvent> tested with a TArg variable type |
| public | method | GetHashCode() | 0 if \_value is null, else \_value hashcode |

##### Existing variables:
- BoolVariable, FloatVariable, GameObjectVariable, IntVariable, StringVariable, ULongVariable, Vector2Variable

#### BaseEvent<TArg> (Advanced work status)
Let invokes events passing TArg parameters. Can also store last parameter sent in order to send it to new listeners if required.

```csharp
// Usage example
public GameObjectEvent OnEnemySpawned;

private void HandlePlayerDamaged(Vector3 position)
{
    var newEnemy = Instantiate(_enemyPrefab, position, Quaternion.identity);
	// set any enemy property to set
	OnEnemySpawned.Invoke(newEnemy);
}
```

Default BaseEvent methods, properties and variables:
| Visibility | Type | Name | Description |
| --- | --- | --- | --- |
| private | event | OnEvent| Encapsulated event where BaseEvent register listeners |
| private | variable | \_firstTime | Store if event has been called at least one time |
| private | variable | \_lastMessage | Store last event parameter |
| public | method | AddListener(System.Action<TArg> action, bool getLastEvent = false) | Add listener to OnEvent event. If *getLastEvent* parameter true and event has been called before this registration, raise event just for this listener |
| public | method | Invoke(TArg arg) | Invoke OnEvent to every listener, saves arg as \_lastMessage and set \_firstTime as false |
| public | method | RemoveListener(System.Action<TArg> action) | Remove listener from OnEvent event |

##### Existing events:
- BoolEvent, FloatEvent, GameObjectEvent, IntEvent, StringEvent, ULongEvent, Vector2Event
- IntStringEvent (\*)
- VoidEvent (\*\*)

\* Events with two arguments use tuples (e.g. IntStringEvent = BaseEvent<(int, string)> in order to be used also with UnityEvents
\*\* VoidEvent uses a fake argument. However, VoidEvent has a Invoke method without parameter.

#### BaseList<TArg> (Early work status)
Shared list with automatic event on list changes

Default BaseList methods, properties and variables:
| Visibility | Type | Name | Description |
| --- | --- | --- | --- |
| protected | variable | \_items | Encapsulated list containing values |
| public | variable | OnCollectionChanged | Event invoked when list changes |
| public | property | this\[int index\] { get; set; } | Returns value stored at list index position. On set, invokes OnCollectionChanged event |
| public | property | Values { get; } | Return the list |
| public | property | ValuesArray { get; } | Returns an array with list values |
| public | property | Count { get; } | Returns how many elements list contains |
| public | method | Add(TArg item) | Add item inside list. OnCollectionChanged invoked |
| public | method | Insert(TArg item, int index) | Add item inside list at index position. OnCollectionChanged invoked |
| public | method | AddAt(TArg item, int index) | Add item inside list at index position. OnCollectionChanged invoked |
| public | method | AddRange(IEnumerable<TArg> items) | Add all items value inside list. OnCollectionChanged invoked |
| public | method | Remove(TArg item) | Remove item from list. OnCollectionChanged invoked, no matter if item was not into the list |
| public | method | RemoveAt(TArg item, int index) | Remove item at index position from list. OnCollectionChanged invoked |
| public | method | Clear() | Remove every value from list. OnCollectionChanged invoked, no matter if list was empty |
| public | method | Contains(TArg item) | Returns true if list contains item else false |

##### Existing lists:
*None*

#### BaseArray<TArg> (Early work status)
Shared array with automatic event on array changes

Default BaseList methods, properties and variables:
| Visibility | Type | Name | Description |
| --- | --- | --- | --- |
| protected | variable | \_items | Encapsulated array containing values |
| public | variable | OnCollectionChanged | Event invoked when array changes |
| public | method | Set(TArg\[\] items) | Set \_items equals to items. OnCollectionChanged invoked |
| public | property | this\[int index\] { get; set; } | Returns value stored at array index position. On set, invokes OnCollectionChanged event |
| public | property | Values { get; } | Return the array |
| public | property | ValuesList { get; } | Returns a list with array values |
| public | property | Length { get; } | Returns how many elements array contains |
| public | method | Contains(TArg item) | Returns true if array contains item else false |

##### Existing Arrays:
*None*

###C ompanion Architecture

#### BaseEventsListener<TEvent, TArg> (Advanced work status)
MonoBehaviour class who let GameObjects listen BaseEvents in order to not create references inside components.

BaseEventsListener (BEL) contains an array with events to listen.
An array with events to listen element contains (you can set them by inspector):
- Event to listen
- If BEL has to subscribe on OnEnable
- If BEL has to unsubscribe on OnDisable
- If BEL has to ask last event on subscribe
- BEL response when event invoked

#### VariablesInitializer \[DefaultExecutionOrder(-90)\] (Advanced work status)
MonoBehaviour class who initializes every BaseVariables assigned by inspector.
Initialization happens when GameObject with VariablesInitialized component is enabled => OnEnable

### Tools

#### Scriptable Events Viewer (Early work status)
**Position:** Tools/Nameless Games/Scriptable System/Scriptable Events Viewer

Let you track every BaseEvent ScriptableObject references (listeners and invokers).
Viewer let you ping any Event referral, opening scene / prefab stages when required.

**Warning** actually doesn't ask to save any unsaved changes on open scene!!

Actually, precise tracking of every reference is not guaranteed, expecially when removing BaseEventsListener array elements without deleting BaseEvent listened before.

**Variables, Lists, Arrays tracking on roadmap**

#### Scriptable Type Creator (Early work status)
**Position:** Tools/Nameless Games/Scriptable System/Scriptable Type Creator

Let you easy creates Variables and Events without coding them (still you can do it, it's just so boring and slow).

**Lists and Arrays tracking on roadmap**

### Manually Extending System
**CustomTypeEvent.cs**
```csharp
using UnityEngine;

[CreateAssetMenu(fileName = "Custom Type Event", menuName = "My Game/Scriptable System/Events/Custom Type Event")]
public class CustomTypeEvent : BaseEvent<CustomType> { }
```

**CustomTypeEventsListener.cs**
```csharp
[System.Serializable]
public class CustomTypeEventData : BaseEventData<CustomTypeEvent, CustomType> { }

public class CustomTypeEventsListener : BaseEventsListener<CustomTypeEventData, CustomTypeEvent, CustomType> { }
```

**CustomTypeVariable.cs**
```csharp
using UnityEngine;

[CreateAssetMenu(fileName = "Custom Type Variable", menuName = "My Game/Scriptable System/Variables/Custom Type Variable")]
public class CustomTypeVariable : BaseVariable<CustomType, CustomTypeEvent>
{
    // Custom additional methods if required
}
```

**CustomTypeList.cs**
```csharp
using UnityEngine;

[CreateAssetMenu(fileName = "Custom Type List", menuName = "My Game/Scriptable System/Lists/Custom Type List")]
public class CustomTypeList : BaseList<CustomType> 
{
    // Custom additional methods if required 
}
```

**CustomTypeArray.cs**
```csharp
using UnityEngine;

[CreateAssetMenu(fileName = "Custom Type Array", menuName = "My Game/Scriptable System/Arrays/Custom Type Array")]
public class CustomTypeArray : BaseArray<CustomType> 
{
    // Custom additional methods if required
}
```

### Requirements
- Unity 6000.0.X 

### Contributing
Contributions are always welcom. Feel free to open issues or pull requests.

### License
Distributed under MIT License
