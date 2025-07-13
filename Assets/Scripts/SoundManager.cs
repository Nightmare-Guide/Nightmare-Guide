using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Localization.Plugins.XLIFF.V12;
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
    public AudioClip doorBangSound;
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
    public AudioClip enemyWalkSound;
    public AudioClip enemyRunSound;
    public AudioClip chaseSound; // �߰� ����
    public AudioClip jumpScareSound; // �������ɾ� ����
    public AudioClip wallMoveSound; // �� �̵� ����
    public AudioClip turnOffLightSound; // ���� ������ �Ҹ�
    public AudioClip elevatorOpenSound;
    public AudioClip elevatorCloseSound;
    public AudioClip elevatorMoveSound;
    public AudioClip footStepSound;


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
            { "EnemyWalkSound", EnemyWalkSound },
            { "EnemyRunSound", EnemyRunSound },
            { "ChaseSound", ChaseSound },
            { "JumpScareSound", JumpScareSound },
            { "WallMoveSound", WallMoveSound },
            { "TurnOffLightSound", TurnOffLightSound },
            { "ElevatorOpenSound", ElevatorOpenSound },
            { "ElevatorCloseSound", ElevatorCloseSound },
            { "ElevatorMoveSound", ElevatorMoveSound },
            { "FootStepSound", FootStepSound }
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
    public void EnemyWalkSound() => SfxSoundLoop(enemyWalkSound);
    public void EnemyRunSound() => SfxSoundLoop(enemyRunSound);
    public void ChaseSound() => SfxSoundLoop(chaseSound);
    public void JumpScareSound() => sfxSource.PlayOneShot(jumpScareSound);
    public void WallMoveSound() => sfxSource.PlayOneShot(wallMoveSound);
    public void TurnOffLightSound() => sfxSource.PlayOneShot(turnOffLightSound);
    public void ElevatorOpenSound() => sfxSource.PlayOneShot(elevatorOpenSound);
    public void ElevatorCloseSound() => sfxSource.PlayOneShot(elevatorCloseSound);
    public void ElevatorMoveSound() => sfxSource.PlayOneShot(elevatorMoveSound);
    public void FootStepSound() => sfxSource.PlayOneShot(footStepSound);

    public void SfxSoundLoop(AudioClip clip)
    {
        sfxSource.loop = true;
        sfxSource.clip = clip;
        sfxSource.Play();
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

    public void StopSfxSound()
    {
        sfxSource.Stop();
        sfxSource.clip = null;
    }
}
