using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class ForcedReset : MonoBehaviour
{
    public Image resetImage; // GUITexture 대신 UI.Image로 대체

    private void Update()
    {
        // "ResetObject" 버튼이 눌리면
        if (Input.GetButtonDown("ResetObject")) // CrossPlatformInputManager를 대체
        {
            // 현재 씬을 비동기로 다시 로드
            SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);
        }
    }
}