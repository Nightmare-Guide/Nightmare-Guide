using System.Collections.Generic;
using System;
using UnityEngine;

using static SchoolUIManager;
using static CommonUIManager;

[CreateAssetMenu(fileName = "ProgressData", menuName = "Game/ProgressData")]
public class ProgressData : ScriptableObject
{
    [Header("���൵ ��: é��_����_��� ����")]
    public string storyProgress = "0_0_0";

    [Header("������ ���� ����")]
    public bool getSmartPhone = false;

    [Header("�÷��̾� ��ġ")]
    public Vector3 playerPosition = Vector3.zero;

    [Header("�÷��̾� ��ġ")]
    public int sanchi = 0;
    [Header("MainUIManager")]
    public List<String> mainInventoryDatas;
    [Header("SchoolUIManager")]
    public List<SavePhoneData> phoneDatas;
    public List<String> inventoryDatas;
    [Header("CommonUIManager")]
    public SaveStevenPhoneData stevenPhoneDatas;
    float bgVolume;
    float effectVolume;
    float characterVolume;
    bool isFullScreen;
    string language;
    // �ʱ�ȭ ���
    public void ResetProgress()
    {
        storyProgress = "0_0_0";
        getSmartPhone = false;
        playerPosition = Vector3.zero;
    }
}
