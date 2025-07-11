using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;


    [Header("BGM")]
    public AudioSource bgmSource;
    public AudioClip hospitalSound;
    public AudioClip windSound;
    [Header("SFX")]
    public AudioSource sfxSource;
    public AudioClip beepsound;
    public AudioClip doorOpen;
    public AudioClip doorClose;
    public AudioClip doorLocked;
    public AudioClip mouseHover;
    public AudioClip carEnginsound1;
    public AudioClip carEnginsound2;
    public AudioClip broomSound;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayDoorOpen() => sfxSource.PlayOneShot(doorOpen);
    public void PlayDoorClose() => sfxSource.PlayOneShot(doorClose);
    public void PlayDoorLocked() => sfxSource.PlayOneShot(doorLocked);
    public void ClickButton() => sfxSource.PlayOneShot(beepsound);
    public void ButtonHover() => sfxSource.PlayOneShot(mouseHover);

    public void BroomSound() => SfxSoundLoop(broomSound);



    public void SfxSoundLoop(AudioClip clip)
    {
        sfxSource.loop = true;
        sfxSource.PlayOneShot(clip);
    }

    public void PlayBGM(AudioClip clip, bool loop = true)
    {
        if (clip == null) return;

        bgmSource.clip = clip;
        bgmSource.loop = loop;
        bgmSource.Play();
    }

    public void StopBGM()
    {
        bgmSource.Stop();
    }
}
