using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextScene : MonoBehaviour
{
    public static NextScene instance{get; private set;}
    public string scene_Name;
    public string storyprogress;
    public MainUIManager mainUIManager;

    private void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    public void Next_Scene()
    {
        Debug.Log(scene_Name);
        CommonUIManager.instance.MoveScene(scene_Name);
        //if (ProgressManager.Instance.progressData.storyProgress == storyprogress)
        //{
        //    CommonUIManager.instance.MoveScene(scene_Name);
        //}
    }
    public void GoNightMareTimeLine()
    {
        mainUIManager.GoNightMare();
    }
    public void NextNightmare()
    {
        CommonUIManager.instance.MoveScene("School_Scene");
    }
}
