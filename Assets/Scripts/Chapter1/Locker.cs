using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityStandardAssets.Characters.FirstPerson;

public class Locker : MonoBehaviour
{
    public Transform setTr;//�÷��̾� ��ǥ ��ġ
    public Vector3 startPr;//�÷��̾� ��Ŀ �� ������ ��ġ
    private bool isMovingToLocker = false; //��Ŀ�� ����
    private bool outMovingToLocker = false; //��Ŀ�� ������
    [SerializeField] GameObject door; //���� ������Ʈ
    [SerializeField] Door door_Obj; //���� ������Ʈ ��ũ��Ʈ ������Ʈ
    public float speed = 0.1f; // �÷��̾ ��Ŀ�� ���� �̵� �ӵ�
    [Header("�÷��̾� ȸ��")]
    public bool lockPr = false;//��Ŀ�� ����� �÷��̾� ȸ��
    Quaternion targetRotation; // ��ǥ ȸ�� ��
    Quaternion startRotation; // ���� ȸ����
    Transform pr; // �÷��̾� ��ġ��
    //ī�޶� ȸ�� ����
    public float rotateRange = 180f;
    private void Start()
    {
       
        door_Obj = door.GetComponent<Door>();

        speed = 0.1f;
    }

    public void PlayerHide()
    {
        door_Obj.Select_Door();
        pr = Chapter1_Mgr.instance.player.transform;
        startPr = pr.position;
        Debug.Log("ó�� ��ǥ ��ǥ: " + startPr);
        if (lockPr)
        {
            startRotation = pr.rotation;
            targetRotation = Quaternion.Euler(0, startRotation.eulerAngles.y + rotateRange, 0);
            Debug.Log("���� ȸ��: " + startRotation.eulerAngles);
            Debug.Log("��ǥ ȸ��: " + targetRotation.eulerAngles);
        }
        isMovingToLocker = true;
    }

    public void OpenLocker()//��Ŀ�� ���� ���� ��
    {
        door_Obj.Select_Door();// ��Ŀ ����
       
        Debug.Log("���� ��ǥ: " + pr.position);
        Debug.Log("��ǥ ��ǥ: " + startPr);
        outMovingToLocker = true;
        
    }

    private void FixedUpdate()
    {
        if (isMovingToLocker) //��Ŀ�ȱ��� �÷��̾� �̵� �� ī�޶� ȸ��
        {
            
            pr.position = Vector3.MoveTowards(pr.position, setTr.position, speed);
           
            if (Vector3.Distance(pr.position, setTr.position) < 0.01f && Quaternion.Angle(pr.rotation, targetRotation) < 1f)
            {            
                isMovingToLocker = false;
                pr.rotation = targetRotation;
                lockPr = false;
                Debug.Log("���� ȸ��: " + pr.rotation.eulerAngles);

                Invoke("InLocker", 0.5f);//�������� �ڷ�ƾ ������ ����
            }


            if (lockPr)
            {
                pr.rotation = Quaternion.Slerp(pr.rotation, targetRotation, Time.deltaTime * 5f);
                Debug.Log("���� ȸ��: " + pr.rotation.eulerAngles);
            }
          
            
        }

        if (outMovingToLocker)
        {
            pr.position = Vector3.MoveTowards(pr.position, startPr, speed);
            if (Vector3.Distance(pr.position, startPr) < 0.01f)
            {
                outMovingToLocker = false;
                Debug.Log("��Ŀ Ż��");
                Invoke("OutLocker", 1f); // �� �ݱ� ����
            }
        }

    }


    public void OutLocker()
    {
        door_Obj.Select_Door(); // �÷��̾� Ż���� ���ݱ�
        Camera_Rt.instance.Open_Camera();
        PlayerController.instance.Open_PlayerController();
    }
    public void InLocker() //�÷��̾� ������ ���� �ݱ�
    {

     
        Debug.Log("���� ȸ��2" + pr.rotation);
        door_Obj.Select_Door(); // �÷��̾� ������ ���ݱ�
        Debug.Log("���");
        lockPr = false;
        Camera_Rt.instance.Open_Camera();

    }

}
