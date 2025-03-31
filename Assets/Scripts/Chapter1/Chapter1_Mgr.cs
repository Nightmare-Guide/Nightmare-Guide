using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class Chapter1_Mgr : MonoBehaviour
{
    public static Chapter1_Mgr instance;
    [Header("lockerRoom관련")]
    public TextMeshPro [] lockerNames;
    public GameObject lockerRoomMainDoor1; // 락커룸 도어
    public GameObject lockerRoomMainDoor2;
    public int nextdoorPassword = 0;

    [Header("Door")]
    public Door doorScript1;
    public Door doorScript2;

    [Header("플레이어")]
    public GameObject player;

    [Header("UI")]
    public GameObject aim_Obj;

    [Header("Aim")]
    public bool isPlaying=true;


    private void Start()
    {
        if (instance == null) { instance = this; }
        // 초기화
        for (int i = 0; i < lockerNames.Length; i++)
        {
            lockerNames[i].text = "";
        }
        doorScript1 = lockerRoomMainDoor1.GetComponent<Door>();
        doorScript2 = lockerRoomMainDoor2.GetComponent<Door>();
        doorScript1.enabled = false;
        doorScript2.enabled = false;
        //
        RandomLockerShuffle();

     

    }
    private void Update()
    {
        if(nextdoorPassword == 4)
        {
            LockerRoomNextDoor();
            nextdoorPassword = 0;
            Debug.Log("다음지역이 열렸습니다.");
        }
     
    }

    private void RandomLockerShuffle() //Locker 이름 랜덤 부여
    {
        string[] names = { "James", "John", "Robert", "David", "Mark", "Paul", "Steven", "Kevin" }; //가해자는 James, John, Robert, David (임시)
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

        }

    }
    private void LockerRoomNextDoor()
    {
        doorScript1.enabled = true;
        doorScript2.enabled = true;
    }

    public void MoveWall(Animator gameobject)
    {
        gameobject.SetTrigger("StartTrigger");
    }

    public void CloseDoor(Animator gameobject)
    {
        gameobject.SetTrigger("CloseDoor");
    }


}
