using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TitleButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] Image bgImg;

    private void OnDisable()
    {
        bgImg.gameObject.SetActive(false);
        text.color = Color.white;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        bgImg.gameObject.SetActive(true);
        text.color = Color.black;
        SoundManager.instance.ButtonHover();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        bgImg.gameObject.SetActive(false);
        text.color = Color.white;
    }
}
