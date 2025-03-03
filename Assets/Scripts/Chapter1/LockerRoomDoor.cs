using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LockerRoomDoor : Door
{
    private TextMeshPro bullyname;
    private string[] bullynameArrays = { "James", "John", "Robert", "David" };//������ ���(�ӽ�)

    private void Start()
    {
        bullyname = GetComponentInChildren<TextMeshPro>();
    }

    public void OpenLockerDoor()
    {
        string text = bullyname.text;

        if (System.Array.IndexOf(bullynameArrays, text) >= 0 && !doorState) //�迭�� ������ -1 ��ȯ (���乮�� ��������)
        {
            Chapter1_Mgr.instance.nextdoorPassword += 1;
            Debug.Log("���� " + Chapter1_Mgr.instance.nextdoorPassword + "�� ������ϴ�.");
        }
        else if(System.Array.IndexOf(bullynameArrays, text) >= 0 && doorState) //���乮�� �ݾ�����
        {
            Chapter1_Mgr.instance.nextdoorPassword -= 1;
        }
        else if(System.Array.IndexOf(bullynameArrays, text) <= 0 && !doorState) //Ʋ������ ������ ��
        {
            Chapter1_Mgr.instance.nextdoorPassword -= 1;
        }
        else if(System.Array.IndexOf(bullynameArrays, text) <= 0 && doorState) //Ʋ������ �ݾ�����
        {
            Chapter1_Mgr.instance.nextdoorPassword += 1;
        }
        Debug.Log(text);
        Debug.Log(doorState);
    }

}
