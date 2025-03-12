using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class Portalwarp : MonoBehaviour
{
    public GameObject exitPortal;
    public bool turnPlayer; // true�� ȸ��, false�� ȸ��X

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            TeleportPlayer(other.gameObject);
        }
    }
    private void TeleportPlayer(GameObject player)
    {
        CharacterController characterController = player.GetComponent<CharacterController>();
        if (characterController != null)
        {
            // 1. CharacterController ��Ȱ��ȭ (���� �̵� ���� ����)
            characterController.enabled = false;

            // 2. �÷��̾��� �θ� ���� (�θ� ���� ����)
            player.transform.SetParent(null);

            // 3. �ⱸ ��Ż�� ���� ��ǥ ��������
            Vector3 exitPosition = exitPortal.transform.position; // Ȥ�� exitPortal.transform.TransformPoint(Vector3.zero);

            // 4. �÷��̾ ��Ȯ�� ��ġ�� �̵�
            player.transform.position = exitPosition;

            // 5. �÷��̾� ȸ�� (�ʿ� ��)
            if (turnPlayer)
            {
                player.transform.Rotate(0, 90, 0);
            }

            // 6. CharacterController �ٽ� Ȱ��ȭ (0.1�� ��)
            StartCoroutine(EnableCharacterControllerAfterDelay(characterController));
        }
    }

    // CharacterController Ȱ��ȭ �ڷ�ƾ
    private IEnumerator EnableCharacterControllerAfterDelay(CharacterController characterController)
    {
        yield return new WaitForSeconds(0.1f);
        characterController.enabled = true;
    }

}
