using System;
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
    public AudioClip broomSound; // ���ڷ� ����
    public AudioClip getItem; // ������ ȹ��
    public AudioClip flashlightSound; // ������ ����/�״�
    public AudioClip keySuccessSound; // ���� ���� ����
    public AudioClip keyFailSound; // ���� ���� ����
    public AudioClip lockerFallSound; // ��Ŀ �Ѿ����� �Ҹ�
    public AudioClip enemyAppearance; // ���� ����
    public AudioClip chaseSound; // �߰� ����
    public AudioClip jumpScareSound; // �������ɾ� ����
    public AudioClip wallMoveSound; // �� �̵� ����

    public Dictionary<string, Action> soundMethods; // ���� �޼ҵ�

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

        soundMethods = new Dictionary<string, Action>
        {
            { "PlayDoorOpen", PlayDoorOpen },
            { "PlayDoorClose", PlayDoorClose },
            { "PlayDoorLocked", PlayDoorLocked },
            { "ClickButton", ClickButton },
            { "ButtonHover", ButtonHover },
            { "BroomSound", BroomSound },
            { "GetItemSound", GetItemSound },
            { "FlashlightSound", FlashlightSound },
            { "KeySuccessSound", KeySuccessSound },
            { "KeyFailSound", KeyFailSound },
            { "LockerFallSound", LockerFallSound },
            { "EnemyAppearanceSound", EnemyAppearanceSound },
            { "ChaseSound", ChaseSound },
            { "JumpScareSound", JumpScareSound },
            { "WallMoveSound", WallMoveSound }
        };
    }

    public void PlayDoorOpen() => sfxSource.PlayOneShot(doorOpen);
    public void PlayDoorClose() => sfxSource.PlayOneShot(doorClose);
    public void PlayDoorLocked() => sfxSource.PlayOneShot(doorLocked);
    public void ClickButton() => sfxSource.PlayOneShot(beepsound);
    public void ButtonHover() => sfxSource.PlayOneShot(mouseHover);
    public void BroomSound() => SfxSoundLoop(broomSound);
    public void GetItemSound() => sfxSource.PlayOneShot(getItem);
    public void FlashlightSound() => sfxSource.PlayOneShot(flashlightSound);
    public void KeySuccessSound() => sfxSource.PlayOneShot(keySuccessSound);
    public void KeyFailSound() => sfxSource.PlayOneShot(keyFailSound);
    public void LockerFallSound() => sfxSource.PlayOneShot(lockerFallSound);
    public void EnemyAppearanceSound() => sfxSource.PlayOneShot(enemyAppearance);
    public void ChaseSound() => SfxSoundLoop(chaseSound);
    public void JumpScareSound() => sfxSource.PlayOneShot(jumpScareSound);
    public void WallMoveSound() => sfxSource.PlayOneShot(wallMoveSound);

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
