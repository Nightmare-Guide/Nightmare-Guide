using System.Collections;
using System.Collections.Generic;
using UnityEditor.Overlays;
using UnityEngine;
using UnityEngine.Playables;
using UnityStandardAssets.Characters.FirstPerson;

public class EthanMother : NPC
{
    public MainUIManager mainUI;
    public Supervisor supervisor;
    public PlayableDirector director;

    public Door door;

    private void OnTriggerEnter(Collider other)
    {
        if (door.inplayerTimeLine)
        {
            //supervisor.WalktoIdle();
            FirstMeet();
            door.inplayerTimeLine = false;
            door.doorID = "";
        }
    }
    public void FirstMeet()
    {
        mainUI.FirstEthanMotherMeet();
        //LookAtPlayer();
    }

    //public void WorktoPosition()
    //{
    //    story = "0_3_4";
    //    CSVRoad_Story.instance.OnSelectChapter(story, this);
    //    supervisor.agent.SetDestination(workposition.position);
    //}
    

}
