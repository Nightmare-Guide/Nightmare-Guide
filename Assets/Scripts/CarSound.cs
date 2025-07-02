using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarSound : MonoBehaviour
{
    public AudioSource engineAudiosource;
    public AudioClip engineAudioclip;

    void Start()
    {
        engineAudiosource = GetComponent<AudioSource>();

        // Mixer의 SFX 그룹에 연결 (선택적)
        if (engineAudiosource.outputAudioMixerGroup == null && SoundManager.instance != null)
        {
            engineAudiosource.outputAudioMixerGroup = SoundManager.instance.sfxSource.outputAudioMixerGroup;
        }

        engineAudiosource.clip = engineAudioclip;
        engineAudiosource.loop = true;
        engineAudiosource.Play();
    }
}
