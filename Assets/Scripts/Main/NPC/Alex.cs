using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Alex : NPC
{

    void FirstMeet()
    {
        story = "2_2_0";
        CSVRoad_Story.instance.OnSelectChapter(story, this);
        // AnimHelper.TryPlay(myAnim, "Idle_Broom", 0.3f);
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
