using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;
using static UnityEditor.Progress;

public class CommonUIManager : MonoBehaviour
{
    public static CommonUIManager instance { get; private set; }

    [SerializeField] GameObject commonUICanvas;
    public GameObject optionUI;
    public GameObject fullScreenCheckImg;
    public GameObject windowedCheckImg;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            DontDestroyOnLoad(commonUICanvas);
        }
        else
        {
            Destroy(gameObject); // �ߺ� ���� ����
            Destroy(commonUICanvas);
        }

        FirstSet();
    }

    void FirstSet()
    {
        commonUICanvas.SetActive(true);
        optionUI.SetActive(false);

        FullScreenBtn();
    }

    public void FullScreenBtn()
    {
        // ��ü ȭ�� �ڵ� �ʿ�
        fullScreenCheckImg.SetActive(true);
        windowedCheckImg.SetActive(false);
    }

    public void WindowedBtn()
    {
        // â��� ȭ�� �ڵ� �ʿ�
        fullScreenCheckImg.SetActive(false);
        windowedCheckImg.SetActive(true);
    }
}
