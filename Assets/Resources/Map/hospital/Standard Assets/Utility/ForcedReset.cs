using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class ForcedReset : MonoBehaviour
{
    public Image resetImage; // GUITexture ��� UI.Image�� ��ü

    private void Update()
    {
        // "ResetObject" ��ư�� ������
        if (Input.GetButtonDown("ResetObject")) // CrossPlatformInputManager�� ��ü
        {
            // ���� ���� �񵿱�� �ٽ� �ε�
            SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);
        }
    }
}