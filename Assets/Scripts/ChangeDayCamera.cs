using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;
using UnityStandardAssets.ImageEffects;

public class ChangeDayCamera : MonoBehaviour
{
    [SerializeField] Camera mainCamera;
    [SerializeField] PostProcessingBehaviour postProcessingBehaviour;
    [SerializeField] PostProcessingProfile day_Scene;
    [SerializeField] PostProcessingProfile night_Scene;
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
}
