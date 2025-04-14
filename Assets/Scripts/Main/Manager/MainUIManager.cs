using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static SchoolUIManager;

public class MainUIManager : UIUtility
{
    public List<VerticalLayoutGroup> textBoxLayouts;

    private void OnEnable()
    {
        CommonUIManager.instance.mainUIManager = this;
    }

    private void OnDisable()
    {
        CommonUIManager.instance.mainUIManager = null;
    }
}
