using UnityEngine;
using System.Collections;
using UnityStandardAssets.Characters.FirstPerson;

public class Portal : MonoBehaviour
{
    public GameObject exitPortal;
    public Camera portalCamera;         // 포탈 뷰를 렌더링할 카메라
    public RenderTexture viewTexture;   // 포탈 뷰를 저장할 텍스처
    public bool turnPlayer; // true면 회전, false면 회전X

    private MeshRenderer screen;

    void Awake()
    {
        screen = GetComponentInChildren<MeshRenderer>();
        portalCamera.targetTexture = viewTexture;
        screen.material.SetTexture("_MainTex", viewTexture);
    }

    public void UpdateCamera(Camera playerCamera)
    {
        // 1️ 플레이어의 위치를 포탈 기준으로 변환
        Vector3 playerOffsetFromPortal = playerCamera.transform.position - transform.position;

        // 2️ 포탈 카메라의 위치 설정 (출구 위치 + 변환된 플레이어 위치)
        portalCamera.transform.position = exitPortal.transform.position +
                                          exitPortal.transform.TransformVector(transform.InverseTransformVector(playerOffsetFromPortal));

        // 3️ 포탈 회전 보정 (180도 회전 추가)
        Quaternion portalRotationalDifference = Quaternion.Inverse(transform.rotation) * exitPortal.transform.rotation;
        Quaternion portalCameraRotation = portalRotationalDifference * playerCamera.transform.rotation;

        // Y축 회전만 사용하고, X, Z축 회전은 고정
        portalCamera.transform.rotation = Quaternion.Euler(0, portalCameraRotation.eulerAngles.y, 0) * playerCamera.transform.rotation;

        // 4️ 카메라 시야각 및 클리핑 조정
        portalCamera.fieldOfView = playerCamera.fieldOfView;
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
