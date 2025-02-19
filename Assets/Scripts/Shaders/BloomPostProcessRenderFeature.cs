using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class BloomPostProcessRenderFeature : ScriptableRendererFeature
{
    [SerializeField]
    private Shader m_bloomShader;

    [SerializeField]
    private Shader m_compositeShader;

    private Material m_bloomMaterial;
    private Material m_compositeMaterial;


    private BloomPostProcessPass m_bloomPass;

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(m_bloomPass);
    }

    public override void Create()
    {
        m_bloomMaterial = CoreUtils.CreateEngineMaterial(m_bloomShader);
        m_compositeMaterial = CoreUtils.CreateEngineMaterial(m_compositeShader);

        m_bloomPass = new BloomPostProcessPass(m_bloomMaterial, m_compositeMaterial);
    }

    protected override void Dispose(bool disposing)
    {
        CoreUtils.Destroy(m_bloomMaterial);
        CoreUtils.Destroy(m_compositeMaterial);
    }

    public override void SetupRenderPasses(ScriptableRenderer renderer, in RenderingData renderingData)
    {
        //Debug.Log($"Camera Type: {renderingData.cameraData.cameraType}");

        if (renderingData.cameraData.cameraType == CameraType.Game)
        {
            Debug.Log("Game Camera detected! Applying Bloom Effect.");
            m_bloomPass.ConfigureInput(ScriptableRenderPassInput.Depth);
            m_bloomPass.ConfigureInput(ScriptableRenderPassInput.Color);
            m_bloomPass.SetTarget(renderer.cameraColorTargetHandle, renderer.cameraDepthTargetHandle);
        }
        else
        {
            Debug.Log("Bloom Effect skipped: Not a Game Camera.");
        }
    }

}
