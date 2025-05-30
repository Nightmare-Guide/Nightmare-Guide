using System.Collections;
using System.Collections.Generic;
using UnityEditor.Overlays;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class EthanMother : NPC
{
    public Supervisor supervisor;
    public Transform workposition;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerTransform = other.transform;
            supervisor.agent.isStopped = true;
            supervisor.WalktoIdle();
            FirstMeet();
        }
    }
    public void FirstMeet()
    {
        story = "0_3_3";
        CSVRoad_Story.instance.OnSelectChapter(story, this);
        col.enabled = false;
        LookAtPlayer();
    }

    public void WorktoPosition()
    {
        story = "0_3_4";
        CSVRoad_Story.instance.OnSelectChapter(story, this);
        supervisor.agent.SetDestination(workposition.position);
    }
    

}
