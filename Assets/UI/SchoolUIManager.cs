using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class SchoolUIManager : MonoBehaviour
{
    public GameObject blurBG;
    public GameObject[] uiObjects;

    // �޴��� ���ȭ��
    [Header ("#Lock")]
    public GameObject LockPhoneUI;
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI dateText;

    private void Awake()
    {
        FirstSetUP();
    }

    private void Update()
    {
        // �޴��� UI �ð� �ǽð� ����
        if (LockPhoneUI.activeInHierarchy)
        {
            GetDate();
        }

        // ui �ݱ�
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            foreach (GameObject uiObj in uiObjects)
            {
                if (uiObj.activeInHierarchy)
                {
                    CloseUI(uiObj);
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            //���콺 Ŀ�� �߾ӿ� ����
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;  // Ŀ���� �Ⱥ��̰� �ϱ�

            Debug.Log("Cursor Locked");
        }
        if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            //���콺 Ŀ�� �߾ӿ� ����
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;  // Ŀ���� �Ⱥ��̰� �ϱ�

            Debug.Log("Cursor Enable");
        }
    }

    void GetDate()
    {
        // ���� ��¥�� �ð� ��������
        DateTime currentTime = DateTime.Now;

        // �̱� �������� ��¥ ������ (MM/dd/yyyy)
        string formattedDate = currentTime.ToString("MM.dd.yyyy", CultureInfo.InvariantCulture);

        // �ð� ������ (������ 00��, 13�ÿ��� 24�ô� -12�� ǥ��, 1�ú��� 12�ô� �״�� ǥ��)
        int hour = currentTime.Hour;
        string formattedTime;

        if (hour == 0)
        {
            // ������ 00�÷� ǥ��
            formattedTime = "00:" + currentTime.ToString("mm");
        }
        else if (hour >= 13)
        {
            // 13�ÿ��� 24�ô� -12�� ��ȯ
            formattedTime = (hour - 12).ToString("00") + ":" + currentTime.ToString("mm");
        }
        else
        {
            // 1�ÿ��� 12�ô� �״�� ǥ��
            formattedTime = hour.ToString("00") + ":" + currentTime.ToString("mm");
        }

        // UI �ؽ�Ʈ�� ��¥�� �ð� ǥ��
        dateText.text = formattedDate;
        timeText.text = formattedTime;
    }

    void FirstSetUP()
    {
        blurBG.SetActive(false);
        LockPhoneUI.SetActive(false);

        //���콺 Ŀ�� �߾ӿ� ����
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;  // Ŀ���� �Ⱥ��̰� �ϱ�

        Debug.Log("UI First Setup");
    }

    public void CloseUI(GameObject ui)
    {
        ui.SetActive(false);

        //ī�޶� ȸ�� Ȱ��ȭ
        Camera_Rt.instance.Open_Camera();

        //�÷��̾� ��Ʈ�� On
        PlayerController.instance.Open_PlayerController();

        //���콺 Ŀ�� �߾ӿ� ����
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;  // Ŀ���� �Ⱥ��̰� �ϱ�

        Debug.Log("Close UI : " + ui.name);  
    }

    public void OpenPhoneUI()
    {
        blurBG.SetActive(true);
        LockPhoneUI.SetActive(true);

        //ī�޶� ȸ�� ����
        Camera_Rt.instance.Close_Camera();

        //���콺 Ŀ�� Ȱ��ȭ
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;  // Ŀ���� ���̰� �ϱ�

        Debug.Log("Open PhoneUI");
    }
}
