using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Norsevar.Snow.Renderers
{
    public class CombineTexPass : ScriptableRenderPass
    {

        #region Constants and Statics

        // The profiler tag that will show up in the frame debugger.
        private const string ProfilerTag = "Combine Tex Pass";

        #endregion

        #region Private Fields

        private readonly Material material;

        // We will store our pass settings in this variable.
        private readonly CombineTexFeature.PassSettings passSettings;

        private readonly int tex1ID = Shader.PropertyToID("_MainTex");
        private readonly int tex2ID = Shader.PropertyToID("_CombineTex");

        #endregion

        #region Constructors

        // The constructor of the pass. Here you can set any material properties that do not need to be updated on a per-frame basis.
        public CombineTexPass(CombineTexFeature.PassSettings passSettings)
        {
            this.passSettings = passSettings;

            // Set the render pass event.
            renderPassEvent = passSettings.renderPassEvent;

            // We create a material that will be used during our pass. You can do it like this using the 'CreateEngineMaterial' method, giving it
            // a shader path as an input or you can use a 'public Material material;' field in your pass settings and access it here through 'passSettings.material'.
            if (material == null) material = CoreUtils.CreateEngineMaterial("Shader Graphs/SnowPostProcessShader");
        }

        #endregion

        #region Public Methods

        // The actual execution of the pass. This is where custom rendering occurs.
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            // Grab a command buffer. We put the actual execution of the pass inside of a profiling scope.
            var cmd = CommandBufferPool.Get();
            using (new ProfilingScope(cmd, new ProfilingSampler(ProfilerTag)))
            {
                // Blit from the color buffer to a temporary buffer and back. This is needed for a two-pass shader.
                cmd.Blit(passSettings.camTargetRT, passSettings.tempRT, material);
                cmd.CopyTexture(passSettings.tempRT, passSettings.persistentRT);
            }

            // Execute the command buffer and release it.
            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();
            CommandBufferPool.Release(cmd);
        }

        // Called when the camera has finished rendering.
        // Here we release/cleanup any allocated resources that were created by this pass.
        // Gets called for all cameras i na camera stack.
        public override void OnCameraCleanup(CommandBuffer cmd)
        {
            if (cmd == null) throw new ArgumentNullException("cmd");
        }

        // Gets called by the renderer before executing the pass.
        // Can be used to configure render targets and their clearing state.
        // Can be user to create temporary render target textures.
        // If this method is not overriden, the render pass will render to the active camera render target.
        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            material.SetTexture(tex2ID, passSettings.persistentRT);
        }

        #endregion

    }
}
