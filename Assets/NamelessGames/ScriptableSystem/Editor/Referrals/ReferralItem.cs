using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace NamelessGames.ScriptableSystem.ScriptableSystemEditor
{
    [UxmlElement]
    public partial class ReferralItem : VisualElement
    {
        ReferralEntry _referral = null;

        public ReferralItem()
        {
            VisualTreeAsset template = Resources.Load<VisualTreeAsset>("scriptable-system-viewer-referral");
            template.CloneTree(this);

            this.Q<Button>("referral-ping-button").clicked += PingReferral;
        }

        public void BindReferral(ReferralEntry referral) => _referral = referral;

        private void PingReferral()
        {
            if(_referral == null)
            {
                Debug.LogWarning("Something wrong happened to referrals. Cant ping this one.");
                return;
            }
            _referral.Ping();
        }
    }
}
