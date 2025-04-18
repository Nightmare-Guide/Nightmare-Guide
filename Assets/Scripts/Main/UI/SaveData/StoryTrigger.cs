using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class StoryTrigger : MonoBehaviour
{
    [SerializeField] string story;
    [SerializeField] enum progress { 
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CSVRoad_Story.instance.OnSelectChapter(story);
        }
    }
  
    public void OnStory(string value)
    {
        CSVRoad_Story.instance.OnSelectChapter(value);
    }
}
