using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SliderController : MonoBehaviour
{
    public AudioMixer mixer;
    public Slider bgmSlider;
    public Slider sfxSlider;

    private void Start()
    {
        // 초기값 설정
        float bgmVolume;
        if (mixer.GetFloat("BGMVolume", out bgmVolume))
            bgmSlider.value = DbToSlider(bgmVolume);

        float sfxVolume;
        if (mixer.GetFloat("SFXVolume", out sfxVolume))
            sfxSlider.value = DbToSlider(sfxVolume);

        // 슬라이더 이벤트 연결
        bgmSlider.onValueChanged.AddListener(SetBGMVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);
    }

    public void SetBGMVolume(float value)
    {
        mixer.SetFloat("BGMVolume", SliderToDb(value));
    }

    public void SetSFXVolume(float value)
    {
        mixer.SetFloat("SFXVolume", SliderToDb(value));
    }

    private float SliderToDb(float value)
    {
        return Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20f;
    }

    private float DbToSlider(float db)
    {
        return Mathf.Pow(10f, db / 20f);
    }
}
