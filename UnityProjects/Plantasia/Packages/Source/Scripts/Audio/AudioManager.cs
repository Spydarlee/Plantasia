using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    // -------------------------------------------------------------------------------

    [System.Serializable]
    public class BGMusicData
    {
        public string       Name = "";
        public AudioClip    AudioClip = null;
        public bool         ContinueFromLastPlayback = true;

        public float LastPlaybackTime { get; set; }
    }

    // -------------------------------------------------------------------------------

    [System.Serializable]
    public class AudioClipPair
    {
        public string       Name = "";
        public AudioClip    AudioClip = null;
    }

    // -------------------------------------------------------------------------------

    public static AudioManager  Instance = null;
    public static string        InvalidSfxName = "NO_SFX";

    // -------------------------------------------------------------------------------

    [Header("Background Music")]
    public AudioSource          BGMusicAudioSourceA = null;
    public AudioSource          BGMusicAudioSourceB = null;
    public float                BGMusicCrossFadeDuration = 1.0f;
    public List<BGMusicData>    BGMusicDataList = new List<BGMusicData>();

    [Header("Sound Effects")]
    public List<AudioSource>    SoundEffectAudioSources = new List<AudioSource>();
    public List<AudioClipPair>  AudioClips = new List<AudioClipPair>();

    // -------------------------------------------------------------------------------

    private bool                            mCurrentBGMusicSourceIsA = true;
    private bool                            mIsCrossFadingBGMusic = false;
    private BGMusicData                     mCurrentBGMusicData = null;
    private int                             mCurrentSfxAudioSourceIndex = 0;

    private Dictionary<string, BGMusicData> mBGMusicDataDict = new Dictionary<string, BGMusicData>();
    private Dictionary<string, AudioClip>   mAudioClipsDict = new Dictionary<string, AudioClip>();
    private Dictionary<AudioSource, int>    mAudioSourcesBeingFadedIn = new Dictionary<AudioSource, int>();
    private Dictionary<AudioSource, int>    mAudioSourcesBeingFadedOut = new Dictionary<AudioSource, int>();

    // -------------------------------------------------------------------------------

    private void Awake()
    {
        Instance = this;

        // Build dictionaries for easier lookup at runtime!
        foreach (var bgMusicData in BGMusicDataList)
        {
            mBGMusicDataDict.Add(bgMusicData.Name, bgMusicData);
        }

        foreach (var audioClipPair in AudioClips)
        {
            mAudioClipsDict.Add(audioClipPair.Name, audioClipPair.AudioClip);
        }
    }

    // -------------------------------------------------------------------------------

    public void PlayBGMusic(string name)
    {
        if (mCurrentBGMusicData != null && name == mCurrentBGMusicData.Name)
            return;

        if (mIsCrossFadingBGMusic)
        {
            Debug.LogError("Sorry, can't change background music while in the middle of a cross-fade!", gameObject);
        }            

        BGMusicData nextBGMusicData;
        if (mBGMusicDataDict.TryGetValue(name, out nextBGMusicData))
        {
            mIsCrossFadingBGMusic = true;

            var currentBGMusicAudioSource = (mCurrentBGMusicSourceIsA) ? BGMusicAudioSourceA : BGMusicAudioSourceB;
            var nextBGMusicAudioSource = (mCurrentBGMusicSourceIsA) ? BGMusicAudioSourceB : BGMusicAudioSourceA;

            // Start playing the new clip but fade the volume in from 0
            nextBGMusicAudioSource.clip = nextBGMusicData.AudioClip;
            nextBGMusicAudioSource.volume = 0.0f;
            nextBGMusicAudioSource.Play();
            nextBGMusicAudioSource.time = (nextBGMusicData.ContinueFromLastPlayback) ? nextBGMusicData.LastPlaybackTime : 0.0f;

            LeanTween.value(0.0f, 1.0f, BGMusicCrossFadeDuration).setOnUpdate((float value) =>
            {
                currentBGMusicAudioSource.volume = (1.0f - value);
                nextBGMusicAudioSource.volume = (value);

            }).setOnComplete(()=>
            {
                if(mCurrentBGMusicData != null )
                {
                    mCurrentBGMusicData.LastPlaybackTime = currentBGMusicAudioSource.time;
                }      

                mCurrentBGMusicData = nextBGMusicData;
                currentBGMusicAudioSource.Stop();
                mCurrentBGMusicSourceIsA = !mCurrentBGMusicSourceIsA;
                mIsCrossFadingBGMusic = false;
            });
        }
        else
        {
            Debug.LogError("Couldn't find backgroud music with the name: " + name);
        }
    }

    // -------------------------------------------------------------------------------

    public void StopBGMusic(float duration = 1.0f)
    {
        var currentBGMusicAudioSource = (mCurrentBGMusicSourceIsA) ? BGMusicAudioSourceA : BGMusicAudioSourceB;
        if (currentBGMusicAudioSource != null)
        {
            LeanTween.value(1.0f, 0.0f, duration).setOnUpdate((float value) =>
            {
                if (currentBGMusicAudioSource != null)
                {
                    currentBGMusicAudioSource.volume = (value);
                }
                
            }).setOnComplete(() =>
            {
                if (currentBGMusicAudioSource != null)
                {
                    currentBGMusicAudioSource.Stop();
                }                
            });
        }
    }

    // -------------------------------------------------------------------------------

    public void PlaySfx(AudioSource audioSource, string name, float time = 0.0f)
    {
        AudioClip audioClip;
        if (mAudioClipsDict.TryGetValue(name, out audioClip))
        {
            AudioSource audioSourceToUse = audioSource;
            if (audioSourceToUse == null)
            {
                audioSourceToUse = SoundEffectAudioSources[mCurrentSfxAudioSourceIndex];

                mCurrentSfxAudioSourceIndex++;
                if (mCurrentSfxAudioSourceIndex >= SoundEffectAudioSources.Count)
                {
                    mCurrentSfxAudioSourceIndex = 0;
                }
            }

            audioSourceToUse.PlayOneShot(audioClip);
            audioSourceToUse.time = time;
        }
        else
        {
            Debug.LogError("Couldn't find '" + name + "' SFX to play", gameObject);
        }
    }

    // -------------------------------------------------------------------------------

    public void StopAllSfx()
    {
        foreach (var audioSource in SoundEffectAudioSources)
        {
            audioSource.Stop();
        }

        mCurrentSfxAudioSourceIndex = 0;
    }

    // -------------------------------------------------------------------------------

    public void PlayRandomSfx(AudioSource audioSource, float time = 0.0f, params string[] names)
    {
        if (names != null && names.Length > 0)
        {
            var randomIndex = Random.Range(0, names.Length);
            PlaySfx(audioSource, names[randomIndex], time);
        }
        else
        {
            Debug.LogError("Called PlayRandomSfx with an empty or null list!", gameObject);
        }
    }

    // -------------------------------------------------------------------------------

    public AudioClip GetAudioClip(string name)
    {
        AudioClip audioClipToReturn = null;
        mAudioClipsDict.TryGetValue(name, out audioClipToReturn);
        return audioClipToReturn;
    }

    // -------------------------------------------------------------------------------

    public void FadeAudioSourceIn(AudioSource audioSource, float duration, System.Action onCompleteCallback = null)
    {
        if (mAudioSourcesBeingFadedIn.ContainsKey(audioSource))
        {
            // We're already fading in, so we don't want to do anything
            return;
        }

        if (mAudioSourcesBeingFadedOut.ContainsKey(audioSource))
        {
            // We're currently being faded out but have just been asked to fade in
            // So, cancel the fade out and start fading in from whatever volume
            // level we got to while fading out, scaling the duration accordingly
            LeanTween.cancel(mAudioSourcesBeingFadedOut[audioSource]);
            mAudioSourcesBeingFadedOut.Remove(audioSource);
            duration = (audioSource.volume == 0f) ? duration : (duration * audioSource.volume);
        }
        else
        {
            audioSource.volume = 0.0f;
        }

        if(!audioSource.isPlaying)
        {
            audioSource.Play();
        }

        var ltDescr = LeanTween.value(audioSource.volume, 1.0f, duration).setOnUpdate((float value) =>
        {
            audioSource.volume = value;

        }).setOnComplete(() =>
        {
            mAudioSourcesBeingFadedIn.Remove(audioSource);

            if (onCompleteCallback != null)
            {
                onCompleteCallback.Invoke();
            }
        });

        mAudioSourcesBeingFadedIn.Add(audioSource, ltDescr.id);
    }

    // -------------------------------------------------------------------------------

    public void FadeAudioSourceOut(AudioSource audioSource, float duration, System.Action onCompleteCallback = null)
    {
        if (mAudioSourcesBeingFadedOut.ContainsKey(audioSource))
        {
            // We're already fading our, so we don't want to do anything
            return;
        }

        if (mAudioSourcesBeingFadedIn.ContainsKey(audioSource))
        {
            // We're currently being faded in but have just been asked to fade in
            // So, cancel the fade out and start fading in from whatever volume
            // level we got to while fading out, scaling the duration accordingly
            LeanTween.cancel(mAudioSourcesBeingFadedIn[audioSource]);
            mAudioSourcesBeingFadedIn.Remove(audioSource);
            duration = (audioSource.volume == 0f) ? duration : (duration * audioSource.volume);
        }
        else
        {
            audioSource.volume = 1.0f;
        }

        var ltDescr = LeanTween.value(audioSource.volume, 0.0f, duration).setOnUpdate((float value) =>
        {
            if (audioSource != null)
            {
                audioSource.volume = value;
            }

        }).setOnComplete(() =>
        {
            if (audioSource != null)
            {
                mAudioSourcesBeingFadedOut.Remove(audioSource);
                audioSource.Stop();

                if (onCompleteCallback != null)
                {
                    onCompleteCallback.Invoke();
                }
            }
        });

        mAudioSourcesBeingFadedOut.Add(audioSource, ltDescr.id);
    }

    // -------------------------------------------------------------------------------

    public void SetMuted(bool muted)
    {
        if (muted)
        {
            AudioListener.volume = 0.0f;
        }
        else
        {
            AudioListener.volume = 1.0f;
        }
    }

    // -------------------------------------------------------------------------------

    public void ToggledMuted()
    {
        PlaySfx(null, "Toggle");

        if (AudioListener.volume > 0.0f)
        {
            AudioListener.volume = 0.0f;
        }
        else
        {
            AudioListener.volume = 1.0f;
        }
    }

    // -------------------------------------------------------------------------------

    public static AudioSource GetOrCreateAudioSource(GameObject gameObject, string clipName = "", bool loop = false)
    {
        var audioSource = gameObject.GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = CreateAudioSource(gameObject, clipName, loop);
        }

        return audioSource;
    }

    // -------------------------------------------------------------------------------

    public static AudioSource CreateAudioSource(GameObject gameObject, string clipName = "", bool loop = false)
    {
        var audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.loop = loop;
        audioSource.clip = (clipName == "") ? null : Instance.GetAudioClip(clipName);
        return audioSource;
    }

    // -------------------------------------------------------------------------------
}
