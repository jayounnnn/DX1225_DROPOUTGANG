using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioHandler : MonoBehaviour
{
    [System.Serializable]
    public class AudioClipCategory
    {
        public string categoryName;
        public List<AudioClip> audioClips;
    }

    public static AudioHandler Instance;

    [Header("---------- Audio Source ----------")]
    [SerializeField] AudioSource BGMSource;
    [SerializeField] AudioSource SFXSource;

    [Header("---------- Audio Clips ----------")]
    public List<AudioClipCategory> audioClipCategories;

    private Dictionary<string, List<AudioClip>> audioClipsDict;
    private Dictionary<Transform, AudioSource> activeAudioSources = new Dictionary<Transform, AudioSource>();

    private void Awake()
    {
        audioClipsDict = new Dictionary<string, List<AudioClip>>();
        foreach (var category in audioClipCategories)
        {
            audioClipsDict[category.categoryName] = category.audioClips;
        }

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlaySFX(string clipCategory, int clipIndex, Transform callerTransform, bool shouldLoop = false)
    {
        if (audioClipsDict.TryGetValue(clipCategory, out List<AudioClip> clips))
        {
            if (clipIndex >= 0 && clipIndex < clips.Count)
            {
                if (activeAudioSources.ContainsKey(callerTransform))
                {
                    Destroy(activeAudioSources[callerTransform].gameObject);
                    activeAudioSources.Remove(callerTransform);
                }

                GameObject tempAudio = new GameObject("TempAudio");
                tempAudio.transform.position = callerTransform.position;

                AudioSource tempAudioSource = tempAudio.AddComponent<AudioSource>();

                tempAudioSource.outputAudioMixerGroup = SFXSource.outputAudioMixerGroup;
                tempAudioSource.spatialBlend = 1.0f; // Enables 3D sound
                tempAudioSource.rolloffMode = AudioRolloffMode.Logarithmic;
                tempAudioSource.minDistance = 1.0f;
                tempAudioSource.maxDistance = 50.0f;
                tempAudioSource.dopplerLevel = 1.0f;

                tempAudioSource.clip = clips[clipIndex];
                tempAudioSource.loop = shouldLoop;
                tempAudioSource.Play();

                tempAudio.AddComponent<AudioFollower>().target = callerTransform;

                activeAudioSources[callerTransform] = tempAudioSource;

                if (!shouldLoop)
                {
                    Destroy(tempAudio, clips[clipIndex].length);
                }
            }
            else
            {
                Debug.LogWarning($"Clip index {clipIndex} out of range for category {clipCategory}.");
            }
        }
        else
        {
            Debug.LogWarning($"Audio clip category {clipCategory} not found.");
        }
    }

    public void PlaySFXIfNotPlaying(string clipCategory, int clipIndex, Transform callerTransform, bool shouldLoop = false, bool overrideAudio = true)
    {
        if (activeAudioSources.TryGetValue(callerTransform, out AudioSource activeSource))
        {
            if (activeSource == null)
            {
                activeAudioSources.Remove(callerTransform);
            }
            else if (activeSource.clip == audioClipsDict[clipCategory][clipIndex] && activeSource.isPlaying)
            {
                return;
            }
            else if (overrideAudio)
            {
                Destroy(activeSource.gameObject);
                activeAudioSources.Remove(callerTransform);
            }
            else
            {
                PlayTempSFX(clipCategory, clipIndex, callerTransform, shouldLoop);
                return;
            }
        }

        PlaySFX(clipCategory, clipIndex, callerTransform, shouldLoop);
    }

    private void PlayTempSFX(string clipCategory, int clipIndex, Transform callerTransform, bool shouldLoop)
    {
        if (audioClipsDict.TryGetValue(clipCategory, out List<AudioClip> clips))
        {
            if (clipIndex >= 0 && clipIndex < clips.Count)
            {
                GameObject tempAudio = new GameObject("1TempAudio");
                tempAudio.transform.position = callerTransform.position;

                AudioSource tempAudioSource = tempAudio.AddComponent<AudioSource>();

                tempAudioSource.outputAudioMixerGroup = SFXSource.outputAudioMixerGroup;
                tempAudioSource.spatialBlend = 1.0f; // Enables 3D sound
                tempAudioSource.rolloffMode = AudioRolloffMode.Logarithmic;
                tempAudioSource.minDistance = 1.0f;
                tempAudioSource.maxDistance = 50.0f;
                tempAudioSource.dopplerLevel = 1.0f;

                tempAudioSource.clip = clips[clipIndex];
                tempAudioSource.loop = shouldLoop;
                tempAudioSource.Play();

                tempAudio.AddComponent<AudioFollower>().target = callerTransform;

                if (!shouldLoop)
                {
                    Destroy(tempAudio, clips[clipIndex].length);
                }
            }
            else
            {
                Debug.LogWarning($"Clip index {clipIndex} out of range for category {clipCategory}.");
            }
        }
        else
        {
            Debug.LogWarning($"Audio clip category {clipCategory} not found.");
        }
    }

    public void PlayBGM(string clipCategory, int clipIndex)
    {
        if (audioClipsDict.TryGetValue(clipCategory, out List<AudioClip> clips))
        {
            if (clipIndex >= 0 && clipIndex < clips.Count)
            {
                BGMSource.PlayOneShot(clips[clipIndex]);
            }
            else
            {
                Debug.LogWarning($"Clip index {clipIndex} out of range for category {clipCategory}.");
            }
        }
        else
        {
            Debug.LogWarning($"Audio clip category {clipCategory} not found.");
        }
    }

    public void PlaySFXAtPosition(string clipCategory, int clipIndex, Vector3 position)
    {
        if (audioClipsDict.TryGetValue(clipCategory, out List<AudioClip> clips))
        {
            if (clipIndex >= 0 && clipIndex < clips.Count)
            {
                GameObject tempAudio = new GameObject("TempAudio");
                tempAudio.transform.position = position;

                AudioSource tempAudioSource = tempAudio.AddComponent<AudioSource>();

                tempAudioSource.outputAudioMixerGroup = SFXSource.outputAudioMixerGroup;
                tempAudioSource.spatialBlend = 1.0f; // Enables 3D sound
                tempAudioSource.rolloffMode = AudioRolloffMode.Logarithmic;
                tempAudioSource.minDistance = 1.0f;
                tempAudioSource.maxDistance = 50.0f;
                tempAudioSource.dopplerLevel = 1.0f;

                tempAudioSource.clip = clips[clipIndex];
                tempAudioSource.Play();

                Destroy(tempAudio, clips[clipIndex].length);
            }
            else
            {
                Debug.LogWarning($"Clip index {clipIndex} out of range for category {clipCategory}.");
            }
        }
        else
        {
            Debug.LogWarning($"Audio clip category {clipCategory} not found.");
        }
    }

    public void StopPlayingSFX(Transform callerTransform)
    {
        if (activeAudioSources.TryGetValue(callerTransform, out AudioSource activeSource))
        {
            activeSource.Stop();
            Destroy(activeSource.gameObject);
            activeAudioSources.Remove(callerTransform);
        }
    }
}
