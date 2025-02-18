using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationAudioBehavior : StateMachineBehaviour
{
    public string categoryName;
    public int audioIndex;

    public bool StopSFX = false;
    public bool Loop = true;
    public bool OverrideAudio = true;

    private AudioHandler audioHandler;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (audioHandler == null)
        {
            audioHandler = FindObjectOfType<AudioHandler>();
        }

        if (StopSFX)
        {
            audioHandler.StopPlayingSFX(animator.gameObject.transform);
        }
        else if (audioHandler != null && !string.IsNullOrEmpty(categoryName))
        {
            audioHandler.PlaySFXIfNotPlaying(categoryName, audioIndex, animator.gameObject.transform, Loop, OverrideAudio);
        }
    }
}
