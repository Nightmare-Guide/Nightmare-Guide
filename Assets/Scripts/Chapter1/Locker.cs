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

    [Header("��Ŀ ���� �ݱ�")]
    public bool doorState; //true�� �������� false�� ��������
    public BoxCollider boxcollider;

    [SerializeField] private Quaternion startRotationLocker;
    [SerializeField] private Quaternion endRotation;
    private float endTime = 0.5f; //ȸ�� �ð�
    public bool isRotation = false; // false�� ȸ�� ���ϰ�����.
    public Coroutine currentCoroutine;
    public enum LockerStat
    {
        Open,
        Close
    }
    public LockerStat stat = LockerStat.Close;
    private void Start()
    {
       
        door_Obj = door.GetComponent<Door>();
        boxcollider = GetComponent<BoxCollider>();
        speed = 0.01f;
    }

    public void PlayerHide()
    {
        if (!PlayerController.instance.stat.Equals(PlayerController.PlayerState.Hide))
        {
            return;
        }
        Select_Locker();//��Ŀ ����
        pr = Chapter1_Mgr.instance.player.transform;
        startPr = pr.position;
        //Debug.Log("ó�� ��ǥ ��ǥ: " + startPr);
        if (lockPr)
        {
            startRotation = pr.rotation;
            targetRotation = Quaternion.Euler(0, startRotation.eulerAngles.y + rotateRange, 0);
           // Debug.Log("���� ȸ��: " + startRotation.eulerAngles);
           // Debug.Log("��ǥ ȸ��: " + targetRotation.eulerAngles);
        }
        isMovingToLocker = true;
    }

    public void OpenLocker()//��Ŀ�� ���� ���� ��
    {
        if (!PlayerController.instance.stat.Equals(PlayerController.PlayerState.Idle))
        {
            return;
        }
        Select_Locker();// ��Ŀ ����
        
        //Debug.Log("���� ��ǥ: " + pr.position);
        //Debug.Log("��ǥ ��ǥ: " + startPr);
        outMovingToLocker = true;
        
    }

    private void FixedUpdate()
    {
        if (isMovingToLocker) //��Ŀ�ȱ��� �÷��̾� �̵� �� ī�޶� ȸ��
        {
            
            pr.position = Vector3.MoveTowards(pr.position, setTr.position, Time.fixedDeltaTime*1.2f);//�̵� �ӵ� ���� �ʿ�
           
            if (Vector3.Distance(pr.position, setTr.position) < 0.01f && Quaternion.Angle(pr.rotation, targetRotation) < 1f)
            {            
                isMovingToLocker = false;
                pr.rotation = targetRotation;
                lockPr = false;
                //Debug.Log("���� ȸ��: " + pr.rotation.eulerAngles);

                if (currentCoroutine ==null)
                {
                    InLocker();//�������� �ڷ�ƾ ������ ����
                }
              
            }


            if (lockPr)
            {
                pr.rotation = Quaternion.Slerp(pr.rotation, targetRotation, Time.deltaTime * 5f);//ī�޶� ȸ�� �ӵ�
                //Debug.Log("���� ȸ��: " + pr.rotation.eulerAngles);
            }
          
            
        }

        if (outMovingToLocker)
        {
            pr.position = Vector3.MoveTowards(pr.position, startPr, Time.fixedDeltaTime*1.2f);//������ �ӵ� ���� �ʿ�
            if (Vector3.Distance(pr.position, startPr) < 0.01f)
            {
                outMovingToLocker = false;
                //Debug.Log("��Ŀ Ż��");
                if (currentCoroutine == null)
                {
                    OutLocker();//�������� �ڷ�ƾ ������ ����
                }
               
            }
        }

    }


    public void OutLocker()
    {
        Select_Locker(); // �÷��̾� Ż���� ���ݱ�
        Camera_Rt.instance.Open_Camera();
        PlayerController.instance.Open_PlayerController();
    }
    public void InLocker() //�÷��̾� ������ ���� �ݱ�
    {

     
        //Debug.Log("���� ȸ��2" + pr.rotation);
        Select_Locker(); // �÷��̾� ������ ���ݱ�
       // Debug.Log("���");
        lockPr = false;
        Camera_Rt.instance.Open_Camera();

    }


    public void Select_Locker()//���� ����
    {
        if (!isRotation)
        {
            isRotation = true;
            StartCoroutine(RotationDoor());
        }

    }

    private IEnumerator RotationDoor()//���� ����
    {
        float startTime = 0f;
        float speedMultiplier = 2.5f; // �ӵ� ����
        startRotationLocker = transform.rotation;

        if (doorState)
            endRotation = Quaternion.Euler(0, startRotationLocker.eulerAngles.y + 110, 0);
        else
            endRotation = Quaternion.Euler(0, startRotationLocker.eulerAngles.y - 110, 0);

        istrigger_on();

        while (startTime < endTime)
        {
            float t = (startTime / endTime);
            transform.rotation = Quaternion.Slerp(startRotationLocker, endRotation, t);
            startTime += Time.deltaTime * speedMultiplier;
            yield return null;
        }

        transform.rotation = endRotation;
        doorState = !doorState;
        currentCoroutine = null;
        isRotation = false;
        istrigger_off();
    }

    public void istrigger_on()
    {
        boxcollider.isTrigger = true;
        // �θ��� MeshCollider�� ��Ȱ��ȭ
        transform.parent.GetComponent<MeshCollider>().enabled = false;
    }
    public void istrigger_off()
    {
        boxcollider.isTrigger = false;
        // �θ��� MeshCollider�� ��Ȱ��ȭ
        transform.parent.GetComponent<MeshCollider>().enabled = true;
    }

}
