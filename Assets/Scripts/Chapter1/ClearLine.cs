using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class ClearLine : MonoBehaviour
{
    
    private void OnTriggerEnter(Collider other)//�÷��̾ ���� ��Ż�� ��й�ȣ �Է°� �ʱ�ȭ
    {
        if (other.CompareTag("Player"))
        {
            Maze_Mgr.instance.Btn_Clear();
        }
      
    }


}
