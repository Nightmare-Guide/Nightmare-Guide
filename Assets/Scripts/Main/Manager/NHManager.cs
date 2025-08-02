using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Diagnostics;
using UnityStandardAssets.Characters.FirstPerson;
using static ProgressManager;

public class NHManager : MonoBehaviour
{
    [SerializeField] MainUIManager mainUi;

    [SerializeField] GameObject player;
    [SerializeField] GameObject cam;


    private void Start()
    {
        if (ProgressManager.Instance != null)
        {
            if (ProgressManager.Instance.progressData.storyProgress != null)
            {
                string stroyPr = ProgressManager.Instance.progressData.storyProgress;

                if (stroyPr.Equals("1_1_5"))//챕터1 저녁병원 씬 대화
                {
                    if (ProgressManager.Instance != null &&
                        !ProgressManager.Instance.IsActionCompleted(ActionType.BackToHospital))
                    {
                        mainUi.GoNightHospital();
                        Cursor.lockState = CursorLockMode.Locked;
                        Debug.Log("타임라인 진행도 스토리1_1_5");
                    }
                    else
                    {
                        OpenPlayer();
                        Debug.Log("병원 타임라인 진행 완료");
                    }
                }
                
                else
                {
                    Debug.Log("타임라인 불러오기 실패");
                }
            }
        }
    }

    public void OpenPlayer()
    {
        cam.SetActive(false);
        player.transform.GetChild(0).gameObject.SetActive(true);
        PlayerController.instance.Open_PlayerController();
        Camera_Rt.instance.Open_Camera();
        Cursor.lockState = CursorLockMode.Locked;
       
    }







}
