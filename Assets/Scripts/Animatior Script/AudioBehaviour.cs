using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioBehaviour : StateMachineBehaviour
{
    public AudioClip[] enterClips;
    public AudioClip[] exitClips;
    public float enterClipDelay = 0f; 
    public float exitClipDelay = 0f;  

    private AudioSource audioSource;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (audioSource == null)
            audioSource = animator.GetComponent<AudioSource>();

        if (audioSource != null && enterClips.Length > 0)
        {
            AudioClip clip = GetRandomClip(enterClips);
            if (clip != null)
            {
                CoroutineHelper coroutineHelper = GetOrCreateCoroutineHelper(animator.gameObject);
                coroutineHelper.StartCoroutine(PlayAudioWithDelay(audioSource, clip, enterClipDelay));
            }
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (audioSource == null)
            audioSource = animator.GetComponent<AudioSource>();

        if (audioSource != null && exitClips.Length > 0)
        {
            AudioClip clip = GetRandomClip(exitClips);
            if (clip != null)
            {
                CoroutineHelper coroutineHelper = GetOrCreateCoroutineHelper(animator.gameObject);
                coroutineHelper.StartCoroutine(PlayAudioWithDelay(audioSource, clip, exitClipDelay));
            }
        }
    }

    private AudioClip GetRandomClip(AudioClip[] clips)
    {
        if (clips.Length == 0) return null;
        return clips[Random.Range(0, clips.Length)];
    }

    private IEnumerator PlayAudioWithDelay(AudioSource source, AudioClip clip, float delay)
    {
        if (delay > 0)
            yield return new WaitForSeconds(delay);

        source.PlayOneShot(clip);
    }

    private CoroutineHelper GetOrCreateCoroutineHelper(GameObject target)
    {
        CoroutineHelper helper = target.GetComponent<CoroutineHelper>();
        if (helper == null)
            helper = target.AddComponent<CoroutineHelper>();
        return helper;
    }
}
public class CoroutineHelper : MonoBehaviour { }
