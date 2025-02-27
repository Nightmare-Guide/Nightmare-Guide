using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Chapter1_Mgr : MonoBehaviour
{
    public static Chapter1_Mgr instance;
    public TextMeshPro [] lockerNames;

    [Header("�÷��̾�")]
    public GameObject player;

    [Header("UI")]
    public GameObject aim_Obj;

    [Header("Aim")]
    public bool isPlaying=true;
    private void Start()
    {
        if (instance == null) { instance = this; }
        for(int i = 0; i < lockerNames.Length; i++)
        {
            lockerNames[i].text = "";
        } // �ʱ�ȭ

    }
    private void Update()
    {
        if (isPlaying)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;  // Ŀ���� �Ⱥ��̰� �ϱ�
        }
    }

    private void RandomLockerShuffle()
    {
        string[] names = { "James","John", "Robert", "David", "Mark", "Paul", "Steven", "Kevin" };
        int[]List = new int[lockerNames.Length];
        for (int i = 0; i < names.Length -1; i++)
        {
            int randomNumber;
            randomNumber = Random.Range(0, lockerNames.Length - 1);
            lockerNames[randomNumber].text = names[i];
        }
    }
}
