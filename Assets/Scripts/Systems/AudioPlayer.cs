using System.Runtime.CompilerServices;
using UnityEngine;

public class AudioPlayer : MonoBehaviour
{
    public AudioSource MyAudioSource;
    public AudioClip[] AudioClips;
    public bool IsMusic;
    public bool AutoPlay;
    public bool AutoPlayLoop;
    public bool IgnoreTimeScale;
    public int AutoPlayIndex;
    private void Awake()
    {
        MyAudioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        if (IgnoreTimeScale)
        {
            MyAudioSource.ignoreListenerPause = true;
        }
        if(AutoPlay)
        {
            if (AutoPlayLoop)
            {
                PlayAudioLoop(AutoPlayIndex);
            }
            else
            {
                PlayAudio(AutoPlayIndex);
            }
        }
    }

    private void Update()
    {
        if (IsMusic)
        {
            MyAudioSource.volume = GameSystem.Instance.MusicVolume;
        }
        else
        {
            MyAudioSource.volume = GameSystem.Instance.SoundVolume;
        }
    }

    public void PlayAudio(int audioIndex = 0)
    {
        MyAudioSource.clip = AudioClips[audioIndex];
        MyAudioSource.loop = false;
        MyAudioSource.Play();
    }

    public void PlayAudioLoop(int audioIndex = 0)
    {
        MyAudioSource.clip = AudioClips[audioIndex];
        MyAudioSource.loop = true;
        MyAudioSource.Play();
    }

}
