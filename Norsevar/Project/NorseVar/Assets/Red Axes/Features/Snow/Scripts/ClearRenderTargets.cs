using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Norsevar.Snow
{
    public class ClearRenderTargets : MonoBehaviour
    {

        #region Serialized Fields

        [FormerlySerializedAs("RenderTextures")] [SerializeField]
        private List<RenderTexture> renderTextures = new();

        #endregion

        #region Unity Methods

        private void Awake()
        {
            foreach (RenderTexture renderTexture in renderTextures)
                renderTexture.Release();
        }

        private void OnDestroy()
        {
            foreach (RenderTexture renderTexture in renderTextures)
                renderTexture.Release();
        }

        #endregion

        #region Public Methods

        public void Clear()
        {
            foreach (RenderTexture renderTexture in renderTextures)
                renderTexture.Release();
        }

        #endregion

    }
}
