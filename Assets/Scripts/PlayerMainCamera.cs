using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.PostProcessing;
using UnityStandardAssets.ImageEffects;
using static UnityEngine.GraphicsBuffer;

public class PlayerMainCamera : MonoBehaviour
{
    [SerializeField] Camera mainCamera;
    [SerializeField] PostProcessingBehaviour postProcessingBehaviour;
    public GameObject jumpscareObj;


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
        float float_duration = 0.2f;
        float maxtime = 2f;
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

    public void InitCameraRotation()
    {
        transform.localRotation = Quaternion.Euler(0, 0, 0);
    }

    public void RotateToTarget(Transform target, float time)
    {
        StartCoroutine(SmoothRotateToTarget(target, time));
    }

    public Quaternion GetTargetRotation(Transform target)
    {
        // 목표 방향 계산 (y축 회전용)
        Vector3 directionToTarget = (target.position - transform.position).normalized;
        float targetY = Quaternion.LookRotation(directionToTarget).eulerAngles.y;

        // 최종 목표 회전값 설정 (x: -26, y: 몬스터 방향, z: 0도)
        Quaternion targetRotation = Quaternion.Euler(0f, targetY, 0f);

        return targetRotation;
    }

    private IEnumerator SmoothRotateToTarget(Transform target, float time)
    {
        Quaternion startRotation = transform.rotation;
        Quaternion targetRotation = GetTargetRotation(target);

        float elapsed = 0f;
        while (elapsed < time)
        {
            transform.rotation = Quaternion.Slerp(startRotation, targetRotation, elapsed / time);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.rotation = targetRotation; // 정확하게 정렬 보정

        jumpscareObj.SetActive(true);

        Animator monsterAnim = jumpscareObj.GetComponent<Animator>();
        AnimHelper.TryPlay(monsterAnim, "killPlayer", 0f);

        yield return new WaitForSeconds(0.55f);

        // 카메라 이펙트 실행 (예: 화면 깜빡임 등)
        CameraEffect();

        yield return new WaitForSeconds(2f);

        // 사망 후 액션
        if (CommonUIManager.instance.uiManager is SchoolUIManager schoolUIManager)
        {
            Debug.Log("사망 후 액션");
            StartCoroutine(schoolUIManager.RevivalPlayer(ProgressManager.ActionType.EnteredBackRoom));
        }
    }
}
