using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class Michael : NPC
{
    [Header ("# ETC")]
    public Animator broomAnim;
    AudioSource audioSource;

    private void Awake()
    {
        col = GetComponent<Collider>();
        npcTransform = GetComponent<Transform>();
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        col.enabled = false;
        AnimHelper.TryPlay(myAnim, "SweepBroom", 0);
        AnimHelper.TryPlay(broomAnim, "Sweep", 0);
        audioSource.Play();

        if (ProgressManager.Instance != null && !ProgressManager.Instance.IsActionCompleted(ProgressManager.ActionType.FirstMeetMichael))
        {
            StartCoroutine(EnableCollider(col, 2f));
        }
    }

    public void DoSweepBroom()
    {
        Debug.Log("DoSweepBroom");
        AnimHelper.TryPlay(myAnim, "SweepBroom", 0.6f);
        AnimHelper.TryPlay(broomAnim, "Sweep", 0.6f);
        audioSource.Play();

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
        audioSource.Stop();
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
