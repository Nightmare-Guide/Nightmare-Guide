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
        death_Camera_Target = Enemy.enemy_single.deathCamTarget; // �÷��̾ enemy Ÿ������ ī�޶� ��ȯ
        Debug.Log(death_Camera_Target);
    }

    public void RotateTarget() // ���ʹ̿��� �۵���Ű���� �޼ҵ�ȭ
    {
        StartCoroutine(RotateTargetCamera());
    }

    private IEnumerator RotateTargetCamera()
    {
        DeathCamera();

        Vector3 lookTarget = death_Camera_Target.position + Vector3.up * 0.3f; // ������ �ü� ���� ī�޶� Ÿ���� �������� �ٲٴ°� ��õ
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

        // �������� ��Ȯ�ϰ� ����
        transform.rotation = targetRotation;
        // �Ǵ� transform.LookAt(lookTarget);

        rotationCoroutine = null;
    }

    public void CameraEffect() // ���ʹ̿��� �۵���Ű���� �޼ҵ�ȭ
    {
        StartCoroutine(ShakeCamera());
    }

    private IEnumerator ShakeCamera() // ī�޶� ���� �ڵ�
    {
        float elapsedTime = 0f; // ����ð�
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
        // ��ǥ ���� ��� (y�� ȸ����)
        Vector3 directionToTarget = (target.position - transform.position).normalized;
        float targetY = Quaternion.LookRotation(directionToTarget).eulerAngles.y;

        // ���� ��ǥ ȸ���� ���� (x: -26, y: ���� ����, z: 0��)
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

        transform.rotation = targetRotation; // ��Ȯ�ϰ� ���� ����

        jumpscareObj.SetActive(true);

        Animator monsterAnim = jumpscareObj.GetComponent<Animator>();
        AnimHelper.TryPlay(monsterAnim, "killPlayer", 0f);

        yield return new WaitForSeconds(0.55f);

        // ī�޶� ����Ʈ ���� (��: ȭ�� ������ ��)
        CameraEffect();

        yield return new WaitForSeconds(2f);

        // ��� �� �׼�
        if (CommonUIManager.instance.uiManager is SchoolUIManager schoolUIManager)
        {
            Debug.Log("��� �� �׼�");
            StartCoroutine(schoolUIManager.RevivalPlayer(ProgressManager.ActionType.EnteredBackRoom));
        }
    }
}
