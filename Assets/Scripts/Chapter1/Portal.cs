using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class Portal : MonoBehaviour
{
    public GameObject exitPortal; // �ⱸ ��Ż ������Ʈ

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerController.instance.Close_PlayerController();
            
            // exitPortal�� ��ġ�� ȸ������ ����Ͽ� �÷��̾� �̵�
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