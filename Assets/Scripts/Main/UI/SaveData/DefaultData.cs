using System.Collections.Generic;
using System;
using UnityEngine;
using static SchoolUIManager;
using static CommonUIManager;

[CreateAssetMenu(fileName = "DefaultData", menuName = "Game/DefaultData")]
public class DefaultData : ScriptableObject
{
    [Header("���� ��")]
    public string scene = "0_1";
    [Header("���൵ ��: é��_����_��� ����")]
    public string storyProgress = "0_0_0";

    [Header("�÷��̾� ��ġ")]
    public Vector3 playerPosition = new Vector3(-550, -67, 278);

    [Header("�÷��̾� ��ġ")]
    public int sanchi = 0;
    [Header("MainUIManager")]
    public List<String> mainInventoryDatas;
    [Header("SchoolUIManager")]
    public List<SavePhoneData> phoneDatas;
    public List<String> inventoryDatas;
    [Header("CommonUIManager")]
    public SaveStevenPhoneData stevenPhoneDatas;
    public float bgVolume = 50.0f;
    public float effectVolume = 50.0f;
    public float characterVolume = 50.0f;
    public bool isFullScreen = true;
    public string language = "en";
    // �ʱ�ȭ ���
    public void ResetProgress()
    {
        storyProgress = "0_0_0";
        playerPosition = Vector3.zero;
    }
}
