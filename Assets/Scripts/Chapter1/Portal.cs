using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class Portal : MonoBehaviour
{
    public GameObject exitPortal; // 출구 포탈 오브젝트

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerController.instance.Close_PlayerController();
            
            // exitPortal의 위치와 회전값을 사용하여 플레이어 이동
            other.gameObject.transform.position = exitPortal.transform.position;
            other.gameObject.transform.rotation = exitPortal.transform.rotation;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerController.instance.Open_PlayerController();
        }
    }
}