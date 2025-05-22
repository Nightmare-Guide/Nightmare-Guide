using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Michael : NPC
{
    [Header ("# ETC")]
    public Animator broomAnim;

    private void Awake()
    {
        col = GetComponent<Collider>();
    }

    private void Start()
    {
        story = "0_2_0";
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
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CSVRoad_Story.instance.OnSelectChapter(story, this);
            AnimHelper.TryPlay(myAnim, "Idle_Broom", 0.3f);
            AnimHelper.TryPlay(broomAnim, "Idle", 0.3f);
            col.enabled = false;
        }
    }
}
