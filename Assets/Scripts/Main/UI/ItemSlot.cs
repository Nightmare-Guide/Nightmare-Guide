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
    public TextMeshProUGUI decText; // ������ ���� Text
    public Image itemImg; // ������ Img �� ���� �ڽ� ������Ʈ
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

    // Ŭ�� �Լ�
    public void OnPointerClick(PointerEventData eventData)
    {
        if (itemData == null)
            return;

        if (itemData.uiObj == null)
            return;

        SchoolUIManager schoolUIManager = itemData.schoolUIManager;

        schoolUIManager.uiObjects[4].SetActive(false); // �κ��丮 Off

        // Ŭ������ �� �ൿ
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
