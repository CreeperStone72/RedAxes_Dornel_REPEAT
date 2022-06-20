using UnityEngine;
using UnityEngine.Playables;

namespace Norsevar
{
    [System.Serializable]
    public class FoxPlayableAsset : PlayableAsset
    {
        // Factory method that generates a playable based on this asset
        public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
        {
            return Playable.Create(graph);
        }
    }
}
