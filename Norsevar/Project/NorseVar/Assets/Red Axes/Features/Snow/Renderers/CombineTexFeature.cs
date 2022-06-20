using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Norsevar.Snow.Renderers
{
    public class CombineTexFeature : ScriptableRendererFeature
    {

        #region Private Fields

        // References to our pass and its settings.
        private CombineTexPass pass;

        #endregion

        #region Serialized Fields

        public PassSettings passSettings = new();

        #endregion

        #region Public Methods

        // Injects one or multiple render passes in the renderer.
        // Gets called when setting up the renderer, once per-camera.
        // Gets called every frame, once per-camera.
        // Will not be called if the renderer feature is disabled in the renderer inspector.
        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            // Here you can queue up multiple passes after each other.
            renderer.EnqueuePass(pass);
        }

        // Gets called every time serialization happens.
        // Gets called when you enable/disable the renderer feature.
        // Gets called when you change a property in the inspector of the renderer feature.
        public override void Create()
        {
            // Pass the settings as a parameter to the constructor of the pass.
            pass = new CombineTexPass(passSettings);
        }

        #endregion

        [Serializable]
        public class PassSettings
        {

            #region Serialized Fields

            // Where/when the render pass should be injected during the rendering process.
            public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
            public RenderTexture persistentRT;
            public RenderTexture tempRT;
            public RenderTexture camTargetRT;

            #endregion

        }
    }
}
