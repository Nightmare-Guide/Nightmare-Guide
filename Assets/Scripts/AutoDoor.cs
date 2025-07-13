using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.Playables;

public class AutoDoor : MonoBehaviour
{
    public Door door;
    public bool firstStart = false; // ù���� �Ǵܿ�
    public BoxCollider col;
    public MainUIManager mainUIManager;
    public Supervisor supervisor;
    private void Start()
    {
        door.enabled = false;
    }
    private void Update()
    {
        DoorTrigger();
        if (supervisor.firstmeet)
        {
            door.enabled = true;
        }
    }

    public void DoorTrigger()
    {
        if (door.doorState && !firstStart)
        {
            firstStart = true; // �ѹ��̶� ����Ǹ� ����
            mainUIManager.FirstMeetEthanMother();
            col.enabled = false;
        }
    }
}
