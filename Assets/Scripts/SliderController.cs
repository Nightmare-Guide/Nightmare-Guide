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

    [Header("���� ���� ���� (dB)")]
    public float bgmMaxVolume = -20f;  // BGM �ִ� ����
    public float bgmMinVolume = -80f;  // BGM �ּ� ����
    public float sfxMaxVolume = -10f;  // SFX �ִ� ����
    public float sfxMinVolume = -80f;  // SFX �ּ� ����

    private void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
            // �ʱⰪ�� �߰� ������ ����
        float initialBGM = Mathf.Lerp(bgmMinVolume, bgmMaxVolume, 0.5f); // 70% ��ġ
        float initialSFX = Mathf.Lerp(sfxMinVolume, sfxMaxVolume, 0.5f); // 80% ��ġ

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
