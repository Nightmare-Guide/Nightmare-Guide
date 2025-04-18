using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using static SchoolUIManager;
using Unity.VisualScripting;

public class ItemSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public TextMeshProUGUI decText; // 아이템 설명 Text
    public Image itemImg; // 아이템 Img 를 넣을 자식 오브젝트
    public Item itemData;

    private void OnEnable()
    {
        if (itemData == null)
            return;

        itemImg.sprite = itemData.itemImg;
        itemImg.gameObject.SetActive(true);
    }

    private void OnDisable()
    {
        if (itemData == null)
            return;

        decText.text = "";
        itemImg.gameObject.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (itemData == null)
            return;

        decText.text = itemData.name;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (itemData == null)
            return;

        // decText.text = "";
    }

    // 클릭 함수
    public void OnPointerClick(PointerEventData eventData)
    {
        if (itemData == null)
            return;

        if (itemData.uiObj == null)
            return;

        SchoolUIManager schoolUIManager = itemData.schoolUIManager;

        schoolUIManager.uiObjects[4].SetActive(false); // 인벤토리 Off

        // 클릭했을 때 행동
        if (itemData.name.Contains("CellPhone"))
        {
            int index = itemData.name.Contains("Ethan") ? 0 : 1;

            schoolUIManager.OpenCellPhoneItem( itemData.schoolUIManager.phoneInfos[index], itemData.uiObj);
        }
        else
        {
            schoolUIManager.InGameOpenUI(itemData.schoolUIManager.uiObjects[0]);
            schoolUIManager.InGameOpenUI(itemData.uiObj);
        }
    }
}
