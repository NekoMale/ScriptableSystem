using UnityEngine;

[DisallowMultipleComponent, DefaultExecutionOrder(-90)]
public class VariablesInitializer : MonoBehaviour
{
    [SerializeField] BaseVariable[] _variablesToInitialize;

    private void OnEnable()
    {
        for (int variableIndex = 0; variableIndex < _variablesToInitialize.Length; variableIndex++)
        {
            _variablesToInitialize[variableIndex].Initialize();
        }
    }
}