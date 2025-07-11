using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class RightPortal : MonoBehaviour
{
    public GameObject exitPortal;
    public Camera rightPortalCamera;
    public RenderTexture viewTexture;   // 포탈 뷰를 저장할 텍스처
    public PortalManager portalManager;

    private MeshRenderer screen;
    public bool turnPlayer; // true면 회전, false면 회전X

    [Header("성공포탈")]
    public bool succeesPortal;
    [Header("마지막포탈확인")]
    public bool lastportal;

    void Awake()
    {
        screen = GetComponentInChildren<MeshRenderer>();
        rightPortalCamera.targetTexture = viewTexture;
        screen.material.SetTexture("_MainTex", viewTexture);
    }
    public void UpdateCamera(Camera playerCamera)
    {
        // 1️ 플레이어의 위치를 포탈 기준으로 변환
        Vector3 playerOffsetFromPortal = playerCamera.transform.position - transform.position;

        // 2️ 포탈 회전 보정
        Quaternion portalRotationalDifference = Quaternion.Inverse(transform.rotation) * exitPortal.transform.rotation;

        // 3️ 플레이어 카메라의 회전을 가져와서 변환
        Quaternion playerRotation = playerCamera.transform.rotation;

        // 포털 회전 차이에 따른 결과 회전 계산
        Quaternion resultRotation = portalRotationalDifference * playerRotation;
        Vector3 resultEulerAngles = resultRotation.eulerAngles;

        // x와 z축 회전값을 고정 (출구 포털의 회전값 또는 원하는 고정값 사용)
        float fixedXRotation = 10f; // 고정할 x축 회전값
        float fixedZRotation = exitPortal.transform.rotation.eulerAngles.z; // 고정할 z축 회전값

        // y축 회전값은 계산된 값 유지 (플레이어 카메라의 y축 회전을 따라감)
        float dynamicYRotation = resultEulerAngles.y;

        // 수정된 오일러 각도로 새 회전 생성 (x, z는 고정, y는 동적)
        Quaternion finalRotation = Quaternion.Euler(fixedXRotation, dynamicYRotation, fixedZRotation);

        // 수정된 회전으로 포털 카메라 회전 설정
        rightPortalCamera.transform.rotation = finalRotation;

        // 4️ 카메라 시야각 및 클리핑 조정
        rightPortalCamera.fieldOfView = playerCamera.fieldOfView;

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            TeleportPlayer(other.gameObject);
        }

    }

    private void TeleportPlayer(GameObject player)
    {
        // CharacterController가 있을 때 텔레포트 처리
        CharacterController characterController = player.GetComponent<CharacterController>();
        if (characterController != null)
        {
            // 1. 포탈 통과 전 CharacterController 비활성화
            PlayerController.instance.Close_PlayerController();

            // 2. 플레이어 위치를 출구 포탈의 위치로 직접 설정
            player.transform.position = exitPortal.transform.position;

            // 3. 카메라 회전 잠금 해제
            if (Camera_Rt.instance != null)
            {
                Camera_Rt.instance.Close_Camera();
            }

            // 4. 설정한 값에 따라 플레이어 회전 처리
            if (turnPlayer)
            {
                player.transform.Rotate(0, 180, 0);
            }

            // 5. CharacterController 활성화 (딜레이 후 활성화)
            StartCoroutine(EnableCharacterControllerAfterDelay());

            // 6. Rigidbody가 있을 경우 속도 초기화
            Rigidbody rb = player.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = Vector3.zero;
            }
            if (succeesPortal && !lastportal) // 성공 포탈인 경우
            {
                // 최대 단계를 넘지 않도록 제한
                if (portalManager.portalStage < 4)
                {
                    portalManager.portalStage++;
                    Debug.Log("진행: 포탈 단계 증가 -> " + portalManager.portalStage);
                }
            }
            else if (lastportal)
            {
                
            }
            else // 실패 포탈인 경우
            {
                // 최소 0 단계 이하로 내려가지 않도록 제한
                if (portalManager.portalStage > 0)
                {
                    portalManager.portalStage--;
                    Debug.Log("후퇴: 포탈 단계 감소 -> " + portalManager.portalStage);
                }
            }
            //if (succeesPortal)  // 성공 포탈
            //{
            //    if (portalcameramove.Count == 0)
            //    {
            //        portalcameramove.Push(0);
            //    }
            //    else
            //    {
            //        portalcameramove.Push(portalcameramove.Peek() + 1);  // 이전 값 +1
            //    }
            //}
            //else if (!succeesPortal)
            //{
            //    if (portalcameramove.Count > 0 && portalcameramove.Peek() != 0)
            //    {
            //        portalcameramove.Pop();
            //    }

            //}
        }

    }


    // 딜레이 후 CharacterController 활성화
    private IEnumerator EnableCharacterControllerAfterDelay()
    {
        yield return new WaitForSeconds(0.1f);  // 딜레이 추가
        if (PlayerController.instance != null)
        {
            PlayerController.instance.Open_PlayerController(); // 활성화
            if (Camera_Rt.instance != null)
            {
                Camera_Rt.instance.Open_Camera();
            }
        }
    }
}
