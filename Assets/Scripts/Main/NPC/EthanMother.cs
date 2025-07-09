using System.Collections;
using System.Collections.Generic;
using UnityEditor.Overlays;
using UnityEngine;
using UnityEngine.Playables;
using UnityStandardAssets.Characters.FirstPerson;

public class EthanMother : NPC
{
    public Supervisor supervisor;
    public PlayableDirector director;

    public AutoDoor door;

    private void OnTriggerEnter(Collider other)
    {
        if (door.inplayer)
        {
            //supervisor.WalktoIdle();
            FirstMeet();
        }
    }
    public void FirstMeet()
    {
        director.Play();
        //LookAtPlayer();
    }

    //public void WorktoPosition()
    //{
    //    story = "0_3_4";
    //    CSVRoad_Story.instance.OnSelectChapter(story, this);
    //    supervisor.agent.SetDestination(workposition.position);
    //}
    

}
