using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class OpenHint : MonoBehaviour
{
    [Header("��Ʈ ������Ʈ")]
    [SerializeField] GameObject hint;

    [Header("Event")]
    [SerializeField] GameObject basic;//�⺻����
    [SerializeField] GameObject event_Obj;//��� �����°�

    [SerializeField] GameObject backGround;

    public void HintEvent()
    {
        Time.timeScale = 0;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        backGround.SetActive(true);
        hint.SetActive(true);
    }
    public void CloseHint()
    {
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        backGround.SetActive(false);
        hint.SetActive(false);
    }

    void ClearObj()
    {
        hint.SetActive(false);
        basic.SetActive(true);
        event_Obj.SetActive(false);
    }
}
