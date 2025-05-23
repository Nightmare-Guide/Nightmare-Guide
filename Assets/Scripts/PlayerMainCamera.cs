using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;
using UnityStandardAssets.ImageEffects;
using static UnityEngine.GraphicsBuffer;

public class PlayerMainCamera : MonoBehaviour
{
    [SerializeField] Camera mainCamera;
    [SerializeField] PostProcessingBehaviour postProcessingBehaviour;


    public static PlayerMainCamera camera_single {  get; private set; }
    private Transform death_Camera_Target;
    public float rotationDuration = 1f;
    private Coroutine rotationCoroutine;

    private void Awake()
    {
        if (camera_single == null)
        {
            camera_single = this;
        }
    }
    public void DeathCamera()
    {
        death_Camera_Target = Enemy.enemy_single.deathCamTarget; // 플레이어가 enemy 타겟으로 카메라 전환
        Debug.Log(death_Camera_Target);
    }

    public void RotateTarget() // 에너미에서 작동시키려고 메소드화
    {
        StartCoroutine(RotateTargetCamera());
    }

    private IEnumerator RotateTargetCamera()
    {
        DeathCamera();

        Vector3 lookTarget = death_Camera_Target.position + Vector3.up * 0.3f; // 보정된 시선 데스 카메라 타겟의 포지션을 바꾸는것 추천
        Quaternion initialRotation = transform.rotation;
        Quaternion targetRotation = Quaternion.LookRotation(lookTarget - transform.position);
        float elapsedTime = 0f;

        while (elapsedTime < rotationDuration)
        {
            float t = elapsedTime / rotationDuration;
            transform.rotation = Quaternion.Slerp(initialRotation, targetRotation, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 마지막에 정확하게 고정
        transform.rotation = targetRotation;
        // 또는 transform.LookAt(lookTarget);

        rotationCoroutine = null;
    }


    public void CameraEffect() // 에너미에서 작동시키려고 메소드화
    {
        StartCoroutine(ShakeCamera());
    }

    private IEnumerator ShakeCamera() // 카메라 흔드는 코드
    {
        float elapsedTime = 0f; // 경과시간
        float zRotation_duration = 0.5f;
        float float_duration = 0.15f;
        float maxtime = 5f;
        float originalFOV = mainCamera.fieldOfView;

        while (elapsedTime < maxtime)
        {
            elapsedTime += Time.deltaTime;
            float zRotation = Mathf.Sin(elapsedTime * Mathf.PI * 2 / zRotation_duration) * 3; // -3~3
            float shakefov = Mathf.Sin(elapsedTime * Mathf.PI * 2 / float_duration) * 3;
            Vector3 currentRotation = mainCamera.transform.localEulerAngles;
            mainCamera.fieldOfView = originalFOV + shakefov;
            currentRotation.z = zRotation;
            mainCamera.transform.localEulerAngles = currentRotation;

            yield return null;
        }
    }


}
