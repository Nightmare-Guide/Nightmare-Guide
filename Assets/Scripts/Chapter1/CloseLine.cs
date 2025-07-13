using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseLine : MonoBehaviour
{
    public GameObject door;
    bool open = true;
    private void OnTriggerEnter(Collider other)//플레이어가 구역 이탈시 비밀번호 입력값 초기화
    {
        if (other.CompareTag("Player") &&open)
        {

            Door obj = door.GetComponent<Door>();
            // obj.Select_Door();
            open = false;

        }

    }
}
