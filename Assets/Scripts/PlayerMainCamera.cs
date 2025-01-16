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
    [SerializeField] PostProcessingProfile day_Scene;
    [SerializeField] PostProcessingProfile night_Scene;


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
    private void Update()
    {

        if (Input.GetKeyDown("o")) // ������ ����
        {
            Change_Day();
        }
        if (Input.GetKeyDown("u")) // ������ ����
        {
            Change_Night();
        }
    }
    void Change_Day() // �㿡�� ������ ����
    {
        mainCamera.renderingPath = RenderingPath.UsePlayerSettings;
        mainCamera.allowHDR = false;
        mainCamera.allowMSAA = true;
        postProcessingBehaviour.profile = day_Scene;
    }
    void Change_Night() // ������ ������ ����
    {
        mainCamera.renderingPath = RenderingPath.DeferredShading;
        mainCamera.allowHDR = true;
        mainCamera.allowMSAA = false;
        postProcessingBehaviour.profile = night_Scene;
    }

    public void DeathCamera()
    {
        death_Camera_Target = Enemy.enemy_single.deathCamTarget; // �÷��̾ enemy Ÿ������ ī�޶� ��ȯ
        Debug.Log(death_Camera_Target);
    }

    public void RotateTarget() // ���ʹ̿��� �۵���Ű���� �޼ҵ�ȭ ��Ų��
    {
        StartCoroutine(RotateTargetCamera());
    }

    private IEnumerator RotateTargetCamera() // ī�޶� ������ �ڵ�
    {
        DeathCamera();
        Quaternion initialRotation = transform.rotation;
        Quaternion targetRotation = Quaternion.LookRotation(death_Camera_Target.position - transform.position);
        float elapsedTime = 0f;

        while (elapsedTime < rotationDuration)
        {
            float t = elapsedTime / rotationDuration;
            transform.rotation = Quaternion.Slerp(initialRotation, targetRotation, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.rotation = targetRotation;
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
