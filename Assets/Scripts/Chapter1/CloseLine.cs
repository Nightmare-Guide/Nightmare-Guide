using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseLine : MonoBehaviour
{
    public GameObject door;
    bool open = true;
    private void OnTriggerEnter(Collider other)//�÷��̾ ���� ��Ż�� ��й�ȣ �Է°� �ʱ�ȭ
    {
        if (other.CompareTag("Player") &&open)
        {

            Door obj = door.GetComponent<Door>();
            // obj.Select_Door();
            open = false;

        }

    }
}
