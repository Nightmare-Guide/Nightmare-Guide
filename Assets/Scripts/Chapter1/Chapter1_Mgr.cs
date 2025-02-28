using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Chapter1_Mgr : MonoBehaviour
{
    public static Chapter1_Mgr instance;
    public TextMeshPro [] lockerNames;
    public GameObject lockerRoomMainDoor1; // 락커룸 도어
    public GameObject lockerRoomMainDoor2; 

    [Header("플레이어")]
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
        } // 초기화
        RandomLockerShuffle();

    }
    private void Update()
    {
        if (isPlaying)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;  // 커서를 안보이게 하기
        }
    }

    private void RandomLockerShuffle() //Locker 이름 랜덤 부여
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
