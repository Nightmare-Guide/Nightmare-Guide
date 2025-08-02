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

                if (stroyPr.Equals("1_1_5"))//é��1 ���Ẵ�� �� ��ȭ
                {
                    if (ProgressManager.Instance != null &&
                        !ProgressManager.Instance.IsActionCompleted(ActionType.BackToHospital))
                    {
                        mainUi.GoNightHospital();
                        Cursor.lockState = CursorLockMode.Locked;
                        Debug.Log("Ÿ�Ӷ��� ���൵ ���丮1_1_5");
                    }
                    else
                    {
                        OpenPlayer();
                        Debug.Log("���� Ÿ�Ӷ��� ���� �Ϸ�");
                    }
                }
                
                else
                {
                    Debug.Log("Ÿ�Ӷ��� �ҷ����� ����");
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
