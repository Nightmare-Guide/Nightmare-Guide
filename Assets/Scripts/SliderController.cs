using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SliderController : MonoBehaviour
{
    public static SliderController instance;

    public AudioMixer mixer;
    public Slider bgmSlider;
    public Slider sfxSlider;

    [Header("볼륨 범위 설정 (dB)")]
    public float bgmMaxVolume = -20f;  // BGM 최대 볼륨
    public float bgmMinVolume = -80f;  // BGM 최소 볼륨
    public float sfxMaxVolume = -10f;  // SFX 최대 볼륨
    public float sfxMinVolume = -80f;  // SFX 최소 볼륨

    private void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
            // 초기값을 중간 정도로 설정
        float initialBGM = Mathf.Lerp(bgmMinVolume, bgmMaxVolume, 0.5f); // 70% 위치
        float initialSFX = Mathf.Lerp(sfxMinVolume, sfxMaxVolume, 0.5f); // 80% 위치

        mixer.SetFloat("BGMVolume", initialBGM);
        mixer.SetFloat("SFXVolume", initialSFX);

        bgmSlider.value = DbToSlider(initialBGM, bgmMinVolume, bgmMaxVolume);
        sfxSlider.value = DbToSlider(initialSFX, sfxMinVolume, sfxMaxVolume);

        bgmSlider.onValueChanged.AddListener(SetBGMVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);
    }

    public void SetBGMVolume(float value)
    {
        float dbValue = SliderToDb(value, bgmMinVolume, bgmMaxVolume);
        mixer.SetFloat("BGMVolume", dbValue);
    }

    public void SetSFXVolume(float value)
    {
        float dbValue = SliderToDb(value, sfxMinVolume, sfxMaxVolume);
        mixer.SetFloat("SFXVolume", dbValue);
    }

    private float SliderToDb(float value, float minDb, float maxDb)
    {
        return Mathf.Lerp(minDb, maxDb, value);
    }

    private float DbToSlider(float db, float minDb, float maxDb)
    {
        return Mathf.InverseLerp(minDb, maxDb, db);
    }
}
