using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LockerRoomDoor : Door
{
    private TextMeshPro bullyname;
    private string[] bullynameArrays = { "James", "John", "Robert", "David" };//가해자 목록(임시)

    private void Start()
    {
        bullyname = GetComponentInChildren<TextMeshPro>();
    }

    public void OpenLockerDoor()
    {
        string text = bullyname.text;

        if (System.Array.IndexOf(bullynameArrays, text) >= 0 && !doorState) //배열에 없으면 -1 반환 (정답문을 열었을때)
        {
            Chapter1_Mgr.instance.nextdoorPassword += 1;
            Debug.Log("현재 " + Chapter1_Mgr.instance.nextdoorPassword + "번 맞췄습니다.");
        }
        else if(System.Array.IndexOf(bullynameArrays, text) >= 0 && doorState) //정답문을 닫았을때
        {
            Chapter1_Mgr.instance.nextdoorPassword -= 1;
        }
        else if(System.Array.IndexOf(bullynameArrays, text) <= 0 && !doorState) //틀린문을 열었을 때
        {
            Chapter1_Mgr.instance.nextdoorPassword -= 1;
        }
        else if(System.Array.IndexOf(bullynameArrays, text) <= 0 && doorState) //틀린문을 닫았을때
        {
            Chapter1_Mgr.instance.nextdoorPassword += 1;
        }
        Debug.Log(text);
        Debug.Log(doorState);
    }

}
