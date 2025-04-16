using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static SchoolUIManager;

public class MainUIManager : UIUtility
{
    public List<VerticalLayoutGroup> textBoxLayouts;

    public GameObject cellPhoneObjs;
    public CharacterPhoneInfo phoneInfos; // �޴��� ������ ��� ����

    private void OnEnable()
    {
        CommonUIManager.instance.mainUIManager = this;
        optionUI = CommonUIManager.instance.optionUI;
    }

    private void Start()
    {
        CommonUIManager.instance.mainUIManager = this;
        optionUI = CommonUIManager.instance.optionUI;

        phoneInfos = new CharacterPhoneInfo { name = "Steven", hasPhone = false, isUnlocked = false, cellPhoneObj = cellPhoneObjs, cellPhoneUI = uiObjects[2] };
    }

    private void Update()
    {
        // ESC Ű
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (AreAllObjectsDisabled(uiObjects))
            {
                // �Ͻ����� UI Ȱ��ȭ
                PauseGame(uiObjects[0]);
            }
            else
            {
                // ��� UI �ݱ�
                foreach (GameObject uiObj in uiObjects)
                {
                    if (uiObj.activeInHierarchy)
                    {
                        InGameCloseUI(uiObj);
                    }
                }
            }
        }

        // Tab Ű -> �κ��丮
        //if (Input.GetKeyDown(KeyCode.Tab))
        //{
        //    if (AreAllObjectsDisabled(uiObjects))
        //    {
        //        InGameOpenUI(uiObjects[0]); // blur ��� Ȱ��ȭ
        //        InGameOpenUI(uiObjects[3]);
        //    }
        //    else if (uiObjects[3].activeInHierarchy)
        //    {
        //        uiObjects[0].SetActive(false);
        //        uiObjects[3].SetActive(false);
        //    }
        //}

        // I Ű -> �޴���
        if (Input.GetKeyDown(KeyCode.I) && phoneInfos.hasPhone)
        {
            OpenCellPhoneItem(phoneInfos);
        }
    }

    private void OnDisable()
    {
        CommonUIManager.instance.mainUIManager = null;
    }

    // �޴��� ���� �Լ� -> I Ű
    public void OpenCellPhoneItem(CharacterPhoneInfo cellPhone)
    {
        InGameOpenUI(uiObjects[0]); // blur ��� Ȱ��ȭ
        InGameOpenUI(uiObjects[2]);

        CellPhone cpLogic = cellPhone.cellPhoneUI.GetComponent<CellPhone>();


        // ��������� �� ���°� �ƴ϶�� �ʱ�ȭ
        if (!cellPhone.isUnlocked)
        {
            cpLogic.SetFirst();
        }
        else
        {
            Debug.Log($"name : {cellPhone.name} , isUnlocked : {cellPhone.isUnlocked}");

            // Lock Screen ��Ȱ��ȭ, App Screen UI Ȱ��ȭ
            cpLogic.LockPhoneUI.SetActive(false);
            cpLogic.appScreenUI.SetActive(true);

            foreach (Image img in cpLogic.appScreenImgs)
            {
                SetUIOpacity(img, true, 0f, 0f);
            }
            foreach (TextMeshProUGUI text in cpLogic.appScreenTexts)
            {
                SetUIOpacity(text, true, 0f, 0f);
            }
        }


    }

    // �޴��� ���� Class
    public class CharacterPhoneInfo
    {
        public string name;
        public bool hasPhone;
        public bool isUnlocked;
        public GameObject cellPhoneObj;
        public GameObject cellPhoneUI;
    }


}
