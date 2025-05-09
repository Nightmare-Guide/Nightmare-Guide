using System.Collections.Generic;
using System;
using UnityEngine;
using static SchoolUIManager;
using static CommonUIManager;

[CreateAssetMenu(fileName = "DefaultData", menuName = "Game/DefaultData")]
public class DefaultData : ScriptableObject
{
    [Header("진행 씬")]
    public string scene = "0_1";
    [Header("진행도 예: 챕터_서브_노드 형식")]
    public string storyProgress = "0_0_0";

    [Header("플레이어 위치")]
    public Vector3 playerPosition = new Vector3(-550, -67, 278);

    [Header("플레이어 산치")]
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
    // 초기화 기능
    public void ResetProgress()
    {
        storyProgress = "0_0_0";
        playerPosition = Vector3.zero;
    }
}
