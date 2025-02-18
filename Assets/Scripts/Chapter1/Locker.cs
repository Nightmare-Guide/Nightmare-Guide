using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class Locker : MonoBehaviour
{
    public Transform setTr;//플레이어 목표 위치
    bool openDoor = false;
    [SerializeField]GameObject door;
    [SerializeField]Door door_Obj;
    public float speed=0.1f;

    private void Start()
    {
        door_Obj = door.GetComponent<Door>();
    }

 
    public void PlayerHide()//플레이어 캐비닛 숨기
    {
      
        Transform pr = Chapter1_Mgr.instance.player.gameObject.transform;
        pr.position = Vector3.MoveTowards(pr.position, setTr.position, speed);
        pr.rotation = Quaternion.Euler(0, 90, 0);
        door_Obj.Select_Door();
        Debug.Log("락커 숨기");
    }
    
   
}
