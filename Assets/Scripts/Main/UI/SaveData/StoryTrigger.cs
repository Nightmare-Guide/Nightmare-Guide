using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class StoryTrigger : MonoBehaviour
{
    [SerializeField] string story;
    [SerializeField] enum progress { 
    }

    public void PrintDialogue(string chapter)
    {
        CSVRoad_Story.instance.OnSelectChapter(chapter);
    }
    
    public void StopTimeLine()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CSVRoad_Story.instance.OnSelectChapter(story);
        }
    }
}
