using System.Collections.Generic;
using System;
using UnityEngine;

using static SchoolUIManager;
using static CommonUIManager;
using static TimeLineManager;


[CreateAssetMenu(fileName = "ProgressData", menuName = "Game/ProgressData")]
public class ProgressData : ScriptableObject
{

    [Header("새 게임인지 판별")]
    public bool newGame = true;
    [Header("진행 씬")]
    public string scene;
    [Header("진행도 예: 챕터_서브_노드 형식")]
    public string storyProgress = "0_0_0";
    public List<ActionStatus> actionStatuses;

    [Header("플레이어 위치")]
    public Vector3 playerPosition = new Vector3(-550, -67, 278);
    public Vector3 playerEulerAngles;
    public List<PlayerTr> playerTr;

    [Header("Player")]
    public int sanchi = 0;
    public string postProcessingName;
    public string fogName;
    public bool hideInLocker;
    [Header("MainUIManager")]
    public List<String> mainInventoryDatas;
    [Header("SchoolUIManager")]
    public List<String> schoolInventoryDatas;
    [Header("CommonUIManager")]
    public List<SavePhoneData> phoneDatas;
    public float bgVolume = 0.5f;
    public float effectVolume = 0.5f;
    public float characterVolume= 0.5f;
    public bool isFullScreen = true;
    public string language = "en";
    [Header("CSVRoad_Story")]
    public string quest;
    [Header("TimeLine")]
    public List<TimelineEntry> timelineWatchedList;

    // 초기화 기능
    public void ResetProgress()
    {
        storyProgress = "0_0_0";
        playerPosition = Vector3.zero;
    }
}
