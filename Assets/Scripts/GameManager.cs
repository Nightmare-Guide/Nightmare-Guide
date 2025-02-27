using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [Header("플레이어 데이터")]
    public Transform player_tr;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

}
