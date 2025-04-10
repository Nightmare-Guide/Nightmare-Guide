using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static SchoolUIManager;

public class MainUIManager : UIUtility
{
    public static MainUIManager instance { get; private set; }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject); // 중복 생성 방지
        }
    }
}
