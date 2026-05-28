using NamelessGames;
using NamelessGames.ScriptableSystem.Events;
using NamelessGames.ScriptableSystem.Variables;
using UnityEngine;
using UnityEngine.Events;

public class Prova : MonoBehaviour
{
    [ReadOnly, SerializeField] IntEvent _bla;
    [SerializeField] IntVariable _variable;

    public UnityEvent<int> CIAO;
}