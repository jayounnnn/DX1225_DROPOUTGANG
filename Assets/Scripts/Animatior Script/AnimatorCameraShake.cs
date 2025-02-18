using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorCameraShake : StateMachineBehaviour
{
    public float enterShakeIntensity = 1.0f;
    public float enterShakeDuration = 0.3f;  
    public float enterShakeDelay = 0f;       

    public float exitShakeIntensity = 1.0f;  
    public float exitShakeDuration = 0.3f;  
    public float exitShakeDelay = 0f;        

    private CinemachineFreeLook freeLookCamera;
    private CoroutineHelper coroutineHelper;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        coroutineHelper = GetOrCreateCoroutineHelper(animator.gameObject);
        freeLookCamera = FindCinemachineFreeLook();

        if (freeLookCamera != null)
        {
            coroutineHelper.StartCoroutine(ShakeCinemachine(freeLookCamera, enterShakeIntensity, enterShakeDuration, enterShakeDelay));
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (freeLookCamera != null)
        {
            coroutineHelper.StartCoroutine(ShakeCinemachine(freeLookCamera, exitShakeIntensity, exitShakeDuration, exitShakeDelay));
        }
    }

    private CinemachineFreeLook FindCinemachineFreeLook()
    {
        if (freeLookCamera != null) return freeLookCamera;
        return GameObject.FindObjectOfType<CinemachineFreeLook>();
    }

    private IEnumerator ShakeCinemachine(CinemachineFreeLook cam, float intensity, float duration, float delay)
    {
        var noiseTop = cam.GetRig(0)?.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        var noiseMiddle = cam.GetRig(1)?.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        var noiseBottom = cam.GetRig(2)?.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        if (noiseTop == null || noiseMiddle == null || noiseBottom == null) yield break;

        if (delay > 0)
            yield return new WaitForSeconds(delay);

        noiseTop.m_AmplitudeGain = intensity;
        noiseMiddle.m_AmplitudeGain = intensity;
        noiseBottom.m_AmplitudeGain = intensity;

        yield return new WaitForSeconds(duration);

        noiseTop.m_AmplitudeGain = 0f;
        noiseMiddle.m_AmplitudeGain = 0f;
        noiseBottom.m_AmplitudeGain = 0f;
    }

    private CoroutineHelper GetOrCreateCoroutineHelper(GameObject target)
    {
        CoroutineHelper helper = target.GetComponent<CoroutineHelper>();
        if (helper == null)
            helper = target.AddComponent<CoroutineHelper>();
        return helper;
    }
}
