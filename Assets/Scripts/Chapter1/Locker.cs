using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityStandardAssets.Characters.FirstPerson;

public class Locker : MonoBehaviour
{
    public Transform setTr;//�÷��̾� ��ǥ ��ġ
    private Transform startPr;//�÷��̾� ��Ŀ �� ������ ��ġ
    private bool isMovingToLocker = false; //���� ����
    [SerializeField] GameObject door;
    [SerializeField] Door door_Obj;
    public float speed = 0.1f; // �÷��̾ ��Ŀ�� ���� �̵� �ӵ�
    [Header("�÷��̾� ȸ��")]
    public bool lockPr = false;//��Ŀ�� ����� �÷��̾� ȸ��
    //Transform playerCameraTransform;//ī�޶�
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

            // Y�� ȸ�� ���� 180���� ����
            currentRotation.y += 180f;

            // Y�� ȸ���� 360���� �ʰ��ϸ� 360�� ���� 0-360 ������ ����
            if (currentRotation.y >= 360f)
            {
                currentRotation.y -= 360f;
            }

            // ���ο� ȸ���� Quaternion���� ����
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
                Invoke("OffDoor", 0.5f);//�������� �ڷ�ƾ ������ ����
                
            }


            if (lockPr)
            {
                pr.rotation = Quaternion.Slerp(pr.rotation, targetRotation, speed);
                Debug.Log("�÷��̾� ȸ��");
               
                pr.rotation = targetRotation; // ��ǥ ȸ������ ��Ȯ�� ����
                Debug.Log("���� ȸ��" + pr.rotation);
                
            }
        }
      
        

    }
    public void OffDoor() //�÷��̾� ������ ���� �ݱ�
    {

     
        Debug.Log("���� ȸ��2" + pr.rotation);
        door_Obj.Select_Door(); // �÷��̾� ������ ���ݱ�
        Debug.Log("���");
        lockPr = false;
        Invoke("OnCamera", 1f);
      
    }
    public void OnCamera()
    {   //ī�޶� ȸ���� �̰� Ű�� ���� �ʱ�ȭ��
        Camera_Rt.instance.Open_Camera();
    }
}
