using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class ClearLine : MonoBehaviour
{
    
    private void OnTriggerEnter(Collider other)//플레이어가 구역 이탈시 비밀번호 입력값 초기화
    {
        if (other.CompareTag("Player"))
        {
            Maze_Mgr.instance.Btn_Clear();
        }
      
    }


}
