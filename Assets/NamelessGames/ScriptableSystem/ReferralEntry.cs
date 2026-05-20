using System.Collections.Generic;

namespace NamelessGames.ScriptableSystem
{
    public enum ReferralKind { Scene, Prefab, Asset }

    [System.Serializable]
    public class ReferralEntry
    {
        [ReadOnly] public string ContainerGUID;
        [ReadOnly] public string ContainerName;
        [ReadOnly] public string GameObjectFileID; // empty on SO
        [ReadOnly] public string _gameObjectName; // empty on SO

        [ReadOnly] public List<string> ComponentNames;
        [ReadOnly] public ReferralKind Kind;
        [ReadOnly] public string Label;

        public ReferralEntry(string containerGUID, string containerName, string gameObjectFileID, string gameObjectName, List<string> componentNames, ReferralKind kind)
        {
            ContainerGUID = containerGUID;
            ContainerName = containerName;
            GameObjectFileID = gameObjectFileID;
            _gameObjectName = gameObjectName;
            ComponentNames = componentNames;
            Kind = kind;
        }

        public ReferralEntry(string containerGUID, string containerName, ReferralKind kind) : this(containerGUID, containerName, string.Empty, string.Empty, new List<string>(), kind) { }

        public void RebuildLabel()
        {
            Label = ContainerName;

            if (Kind == ReferralKind.Asset) return;

            string components = ComponentNames.Count > 0 ? $" [{string.Join(", ", ComponentNames)}]" : string.Empty;

            Label += $" > {_gameObjectName}{components}";
        }
    } 
}