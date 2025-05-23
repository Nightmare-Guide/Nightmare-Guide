using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class Michael : NPC
{
    [Header ("# ETC")]
    public Animator broomAnim;

    private void Awake()
    {
        col = GetComponent<Collider>();
        npcTransform = GetComponent<Transform>();
    }

    private void Start()
    {
        col.enabled = false;
        StartCoroutine(EnableCollider(col, 2f));
        AnimHelper.TryPlay(myAnim, "SweepBroom", 0);
        AnimHelper.TryPlay(broomAnim, "Sweep", 0);
    }

    public void DoSweepBroom()
    {
        Debug.Log("DoSweepBroom");
        AnimHelper.TryPlay(myAnim, "SweepBroom", 0.6f);
        AnimHelper.TryPlay(broomAnim, "Sweep", 0.6f);

        //카메라 회전 활성화
        Camera_Rt.instance.Open_Camera();

        //플레이어 컨트롤 On
        PlayerController.instance.Open_PlayerController();
    }

    void FirstMeet()
    {
        story = "0_2_0";
        CSVRoad_Story.instance.OnSelectChapter(story, this);
        AnimHelper.TryPlay(myAnim, "Idle_Broom", 0.3f);
        AnimHelper.TryPlay(broomAnim, "Idle", 0.3f);
        col.enabled = false;

        LookAtPlayer();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerTransform = other.transform;
            FirstMeet();
        }
    }
}
