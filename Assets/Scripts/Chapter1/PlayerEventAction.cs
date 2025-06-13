using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class PlayerEventAction : MonoBehaviour
{
    [SerializeField] Transform targetTr; // �÷��̾ �̵��� ��ġ
    [SerializeField] float targetYRotation = 0f; // ��ǥ ȸ�� ���� (Y��)
    [SerializeField] float rotationDuration = 1.5f; // ȸ�� �ð� (��)
    [SerializeField] float moveSpeed = 2f;          // �̵� �ӵ�
    [SerializeField] float stopDistance = 0.1f;     // ���� ���� �Ÿ�


    [SerializeField] GameObject obj;
    private void Start()
    {
        Invoke("RotatePlayer", 7f);
       
    }

    public void RotatePlayer()
    {
        PlayerController.instance.Close_PlayerController();
        Camera_Rt.instance.Close_Camera(); // ī�޶� ȸ�� ���
        Cursor.lockState = CursorLockMode.Locked;

        // �ڷ�ƾ���� �ε巴�� ȸ��
        StartCoroutine(SmoothRotateY(targetYRotation, rotationDuration));
    }

    private IEnumerator SmoothRotateY(float targetY, float duration)
    {
        Transform playerTransform = PlayerController.instance.transform;
        Quaternion startRot = playerTransform.rotation;
        Quaternion endRot = Quaternion.Euler(0f, targetY, 0f);

        float time = 0f;
        while (time < duration)
        {
            float t = time / duration;
            playerTransform.rotation = Quaternion.Slerp(startRot, endRot, t);
            time += Time.deltaTime;
            yield return null;
        }

        playerTransform.rotation = endRot;

      
    }

    /*public void PlayerMove()
    {
        StartCoroutine(SmoothMoveToTarget());
    }

    private IEnumerator SmoothMoveToTarget()
    {
        Transform playerTr = PlayerController.instance.transform;

        while (Vector3.Distance(playerTr.position, targetTr.position) > stopDistance)
        {
            Vector3 direction = (targetTr.position - playerTr.position).normalized;
            playerTr.position += direction * moveSpeed * Time.deltaTime;
            yield return null;
        }

        playerTr.position = targetTr.position; // ��ġ ����
        StartCoroutine(SmoothRotateY(130f, rotationDuration));
        // �̵� �������� �ٽ� ���� �����ϰ�
        PlayerController.instance.Open_PlayerController();
        Camera_Rt.instance.Open_Camera();
    }*/

    public void EnableAllMeshColliders()
    {
        MeshCollider[] colliders = obj.GetComponentsInChildren<MeshCollider>();
        foreach (var col in colliders)
        {
            col.enabled = true;
        }
    }
}
