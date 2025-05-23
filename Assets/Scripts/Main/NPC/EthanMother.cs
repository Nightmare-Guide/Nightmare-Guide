using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class EthanMother : NPC
{
    public Supervisor supervisor;
    public bool playerInCollider = false;
    public bool supervisorInCollider = false;


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInCollider = true;
        }
        if (other.CompareTag("Supervisor"))
        {
            supervisorInCollider = true;
            supervisor.agent.isStopped = true;
            supervisor.WalktoIdle();
        }
        if (playerInCollider && supervisorInCollider)
        {
            playerTransform = other.transform;
            
            FirstMeet();
            //대화 시작
        }
    }
    public void FirstMeet()
    {
        story = "0_3_1";
        CSVRoad_Story.instance.OnSelectChapter(story, this);
        col.enabled = false;
        LookAtPlayer();
    }

    public void WaitJames()
    {
        story = "0_3_2";
        CSVRoad_Story.instance.OnSelectChapter(story, this);
        PlayerController.instance.Open_PlayerController();
        //James기다리기 

    }
    

}
