using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [Header("�÷��̾� ������")]
    public Transform player_tr;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

}
