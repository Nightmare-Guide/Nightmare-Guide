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
    public AudioClip broomSound; // 빗자루 사운드
    public AudioClip getItem; // 아이템 획득
    public AudioClip flashlightSound; // 손전등 껐다/켰다
    public AudioClip keySuccessSound; // 열쇠 성공 사운드
    public AudioClip keyFailSound; // 열쇠 실패 사운드
    public AudioClip lockerFallSound; // 락커 넘어지는 소리
    public AudioClip enemyAppearance; // 몬스터 등장
    public AudioClip chaseSound; // 추격 사운드
    public AudioClip jumpScareSound; // 점프스케어 사운드
    public AudioClip wallMoveSound; // 벽 이동 사운드


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
