using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class OpenHint : MonoBehaviour
{
    [Header("힌트 오브젝트")]
    [SerializeField] GameObject hint;

    [Header("Event")]
    [SerializeField] GameObject basic;//기본상태
    [SerializeField] GameObject event_Obj;//대신 켜지는거

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
