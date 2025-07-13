using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.Playables;

public class AutoDoor : MonoBehaviour
{
    public Door door;
    public bool firstStart = false; // 첫실행 판단용
    public PlayableDirector director;
    public BoxCollider col;
    public MainUIManager mainUIManager;

    private void Update()
    {
        DoorTrigger();
    }

    public void DoorTrigger()
    {
        if (door.doorState && !firstStart)
        {
            firstStart = true; // 한번이라도 실행되면 변경
            mainUIManager.FirstMeetEthanMother();
            col.enabled = false;
        }
    }
}
