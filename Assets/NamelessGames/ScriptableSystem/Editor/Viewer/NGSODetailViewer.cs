using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace NamelessGames.ScriptableSystem.ScriptableSystemEditor
{
    public abstract class NGSODetailViewer
    {
        protected SerializedObject _viewedNGObject;
        protected VisualTreeAsset _template;

        protected VisualElement _root;

        public NGSODetailViewer(VisualTreeAsset template, VisualElement root)
        {
            _template = template;
            _root = root;

            _template.CloneTree(_root);
        }

        public void Bind(SerializedObject serializedNGObject)
        {
            _root.Q<Label>("selected-item-type-label").text = serializedNGObject.targetObject.GetType().Name;
            _root.Bind(serializedNGObject);

            _viewedNGObject = serializedNGObject;

            _root.Q<Button>("selected-item-ping-button").clicked += PingSelectedAsset;
            _root.Q<ListView>("selected-item-referral-list-view").bindItem += RegisterPingButton;

            OnBind();
        }

        protected void PingSelectedAsset()
        {
            EditorGUIUtility.PingObject(_viewedNGObject.targetObject as NGScriptableObject);
        }

        protected void RegisterPingButton(VisualElement element, int index)
        {
            ReferralItem referralItem = element.Q<ReferralItem>();
            NGScriptableObject targetNGSO = _viewedNGObject.targetObject as NGScriptableObject;
            referralItem.BindReferral(targetNGSO.Referrals[index]);
        }

        protected abstract void OnBind();
    }
}
