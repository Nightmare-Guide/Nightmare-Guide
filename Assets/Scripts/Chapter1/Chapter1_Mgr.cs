using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Chapter1_Mgr : MonoBehaviour
{
    public static Chapter1_Mgr instance;
    public TextMeshPro [] lockerNames;
    public GameObject lockerRoomMainDoor1; // ��Ŀ�� ����
    public GameObject lockerRoomMainDoor2; 

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
        RandomLockerShuffle();

    }
    private void Update()
    {
        if (isPlaying)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;  // Ŀ���� �Ⱥ��̰� �ϱ�
        }
    }

    private void RandomLockerShuffle() //Locker �̸� ���� �ο�
    {
        string[] names = { "James", "John", "Robert", "David", "Mark", "Paul", "Steven", "Kevin" };
        List<int> usedIndices = new List<int>();

        int count = Mathf.Min(lockerNames.Length, names.Length);

        for (int i = 0; i < count; i++)
        {
            int randomIndex;
            do
            {
                randomIndex = Random.Range(0, lockerNames.Length);
            }
            while (usedIndices.Contains(randomIndex));

            usedIndices.Add(randomIndex);
            lockerNames[randomIndex].text = names[i];
            Debug.Log("Locker " + randomIndex + " assigned to " + names[i]);
        }
    }
}
