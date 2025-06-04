using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class Chapter1_Mgr : MonoBehaviour
{
    public static Chapter1_Mgr instance;
    [Header("lockerRoom����")]
    public TextMeshPro[] lockerNames;
    public GameObject lockerRoomMainDoor1; // ��Ŀ�� ����
    public GameObject lockerRoomMainDoor2;
    public int nextdoorPassword = 0;
    [Header("StrangeRoom")]
    public GameObject[] strangeRoom1;
    public GameObject[] strangeRoom2;
    [Header("Door")]
    public Door doorScript1;
    public Door doorScript2;

    [Header("�÷��̾�")]
    public GameObject player;

    [Header("Enemy")]
    public GameObject Chase_Enemy;

    [Header("UI")]
    public GameObject aim_Obj;

    [Header("Aim")]
    public bool isPlaying = true;

    [Header("Teleport_Point")]
    public GameObject Point_A;
    public GameObject Point_B;
    public GameObject Point_C;
    public int Point = 1;



    private void Start()
    {
        if (instance == null) { instance = this; }
        // �ʱ�ȭ
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
        if (nextdoorPassword == 4)
        {
            LockerRoomNextDoor();
            nextdoorPassword = 0;
            Debug.Log("���������� ���Ƚ��ϴ�.");
        }

    }

    private void RandomLockerShuffle() //Locker �̸� ���� �ο�
    {
        string[] names = { "James", "John", "Robert", "David", "Mark", "Paul", "Steven", "Kevin" }; //�����ڴ� James, John, Robert, David (�ӽ�)
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

    public void ActiveTriggerAnimator(Animator gameobject)
    {
        gameobject.SetTrigger("StartTrigger");
    }

    public void CloseDoor(Animator gameobject)
    {
        gameobject.SetTrigger("CloseDoor");
    }

    public void MoveStrangeClass(GameObject[] strange)
    {
        foreach (GameObject obj in strange)
        {
            if (obj.GetComponent<Rigidbody>() == null)
            {
                Rigidbody rb = obj.AddComponent<Rigidbody>();
                rb.useGravity = true;
                rb.AddTorque(Random.onUnitSphere * Random.Range(5f, 15f), ForceMode.Impulse); //ȸ������ �������� ����
                rb.mass = 1f; // �ʿ信 ���� ����
            }
        }
    }

    public void Teleport_Enemy(GameObject obj)
    {
        if (Point == 1)
        {
            obj.transform.position = Point_A.transform.position;
            obj.transform.rotation = Point_A.transform.rotation;
            Point += 1;
        }
        if (Point == 2)
        {
            obj.transform.position = Point_B.transform.position;
            obj.transform.rotation = Point_B.transform.rotation;
            Point += 1;
        }
        if (Point == 3)
        {
            obj.transform.position = Point_C.transform.position;
            obj.transform.rotation = Point_C.transform.rotation;
            Point += 1;
        }
    }
}
