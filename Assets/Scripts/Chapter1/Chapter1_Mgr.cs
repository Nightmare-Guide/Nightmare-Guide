using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chapter1_Mgr : MonoBehaviour
{
    public static Chapter1_Mgr instance;

    [Header("플레이어")]
    public GameObject player;

    [Header("UI")]
    public GameObject aim_Obj;

    [Header("Aim")]
    public bool isPlaying=true;
    private void Start()
    {
        if (instance == null) { instance = this; }


    }
    private void Update()
    {
        if (isPlaying)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;  // 커서를 안보이게 하기
        }
    }

}
