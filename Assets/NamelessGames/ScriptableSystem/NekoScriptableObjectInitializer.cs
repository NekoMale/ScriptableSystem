using UnityEngine;

namespace NamelessGames.ScriptableSystem
{
    [DisallowMultipleComponent, DefaultExecutionOrder(-90)]
    public class NekoScriptableObjectInitializer : MonoBehaviour
    {
        [SerializeField] NGScriptableObject[] _nekoScriptableObjectToInitialize;

        private void OnEnable()
        {
            for (int nsoIndex = 0; nsoIndex < _nekoScriptableObjectToInitialize.Length; nsoIndex++)
            {
                _nekoScriptableObjectToInitialize[nsoIndex].Initialize();
            }
        }
    }
}