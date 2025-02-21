using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityStandardAssets.Characters.FirstPerson;

public class Locker : MonoBehaviour
{
    public Transform setTr;//플레이어 목표 위치
    private Transform startPr;//플레이어 락커 연 시점의 위치
    private bool isMovingToLocker = false; //도어 열림
    [SerializeField] GameObject door;
    [SerializeField] Door door_Obj;
    public float speed = 0.1f; // 플레이어가 락커로 들어가는 이동 속도
    [Header("플레이어 회전")]
    public bool lockPr = false;//락커에 입장시 플레이어 회전
    //Transform playerCameraTransform;//카메라
    Quaternion targetRotation;
    Quaternion startRotation;
    Transform pr;
    private void Start()
    {
        door_Obj = door.GetComponent<Door>();
    }

    public void PlayerHide()
    {
        door_Obj.Select_Door();
        if (lockPr)
        {
           // playerCameraTransform = Chapter1_Mgr.instance.player.transform;
           // startRotation = playerCameraTransform.rotation;
            Vector3 currentRotation = startRotation.eulerAngles;

            // Y축 회전 값에 180도를 더함
            currentRotation.y += 180f;

            // Y축 회전이 360도를 초과하면 360을 빼서 0-360 범위로 맞춤
            if (currentRotation.y >= 360f)
            {
                currentRotation.y -= 360f;
            }

            // 새로운 회전을 Quaternion으로 생성
            targetRotation = Quaternion.Euler(currentRotation);
            targetRotation = setTr.transform.rotation;
        }
        isMovingToLocker = true;

    }

    private void FixedUpdate()
    {
        if (isMovingToLocker) 
        {
            startPr = Chapter1_Mgr.instance.player.transform; 
            pr = Chapter1_Mgr.instance.player.transform;
            pr.position = Vector3.MoveTowards(pr.position, setTr.position, speed);
           
            if (Vector3.Distance(pr.position, setTr.position) < 0.01f)
            {            
                isMovingToLocker = false;
                Invoke("OffDoor", 0.5f);//문열리는 코루틴 종료후 실행
                
            }


            if (lockPr)
            {
                pr.rotation = Quaternion.Slerp(pr.rotation, targetRotation, speed);
                Debug.Log("플레이어 회전");
               
                pr.rotation = targetRotation; // 목표 회전으로 정확히 맞춤
                Debug.Log("최종 회전" + pr.rotation);
                
            }
        }
      
        

    }
    public void OffDoor() //플레이어 진입후 도어 닫김
    {

     
        Debug.Log("최종 회전2" + pr.rotation);
        door_Obj.Select_Door(); // 플레이어 입장후 문닫기
        Debug.Log("잠김");
        lockPr = false;
        Invoke("OnCamera", 1f);
      
    }
    public void OnCamera()
    {   //카메라 회전값 이거 키면 강제 초기화됨
        Camera_Rt.instance.Open_Camera();
    }
}
