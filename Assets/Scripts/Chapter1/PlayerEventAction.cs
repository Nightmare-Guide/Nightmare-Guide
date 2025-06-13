using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class PlayerEventAction : MonoBehaviour
{
    [SerializeField] Transform targetTr; // 플레이어가 이동할 위치
    [SerializeField] float targetYRotation = 0f; // 목표 회전 각도 (Y축)
    [SerializeField] float rotationDuration = 1.5f; // 회전 시간 (초)
    [SerializeField] float moveSpeed = 2f;          // 이동 속도
    [SerializeField] float stopDistance = 0.1f;     // 도착 판정 거리


    [SerializeField] GameObject obj;
    private void Start()
    {
        Invoke("RotatePlayer", 7f);
       
    }

    public void RotatePlayer()
    {
        PlayerController.instance.Close_PlayerController();
        Camera_Rt.instance.Close_Camera(); // 카메라 회전 잠금
        Cursor.lockState = CursorLockMode.Locked;

        // 코루틴으로 부드럽게 회전
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

        playerTr.position = targetTr.position; // 위치 정렬
        StartCoroutine(SmoothRotateY(130f, rotationDuration));
        // 이동 끝났으니 다시 조작 가능하게
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
