using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarCheckPoint : MonoBehaviour
{
    public static CarCheckPoint instance;

    [Header("�浹 ����")]
    protected bool pointA = true;
    protected bool pointB = true;
    //üũ ����Ʈ
    public bool a_one = true;//a���� �켱������ 1��
    public bool a_two = true;
    public bool b_one = true;//b���� �켱������ 1��
    public bool b_two = true;

    void Awake() { if (instance == null) { instance = this; } }



}
