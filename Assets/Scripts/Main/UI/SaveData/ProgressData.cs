using System.Collections.Generic;
using System;
using UnityEngine;

using static SchoolUIManager;
using static CommonUIManager;
using static TimeLineManager;


[CreateAssetMenu(fileName = "ProgressData", menuName = "Game/ProgressData")]
public class ProgressData : ScriptableObject
{

    [Header("�� �������� �Ǻ�")]
    public bool newGame = true;
    [Header("���� ��")]
    public string scene= "DayHouse";
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
    public float characterVolume=50.0f;
    public bool isFullScreen = true;
    public string language = "en";
    [Header("TimeLine")]
    public List<TimelineEntry> timelineWatchedList;

    // �ʱ�ȭ ���
    public void ResetProgress()
    {
        storyProgress = "0_0_0";
        playerPosition = Vector3.zero;
    }
}
