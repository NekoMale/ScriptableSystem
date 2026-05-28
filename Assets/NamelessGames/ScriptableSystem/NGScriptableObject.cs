using System.Collections.Generic;
using UnityEngine;

namespace NamelessGames.ScriptableSystem
{
    public abstract class NGScriptableObject : ScriptableObject
    {
        [SerializeField] string _ngName;
        [SerializeField, Multiline(5)] string _ngDescription;
        [SerializeField] public List<ReferralEntry> Referrals = new();

        public string NGName => _ngName;
        public string NGDescription => _ngDescription;

        public abstract void Initialize();
    }
}
