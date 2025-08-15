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
    public bool isMovingToLocker = false; //��Ŀ�� ����
    public bool outMovingToLocker = false; //��Ŀ�� ������
    [SerializeField] GameObject door; //���� ������Ʈ
    [SerializeField] Door door_Obj; //���� ������Ʈ ��ũ��Ʈ ������Ʈ
    public float speed = 0.1f; // �÷��̾ ��Ŀ�� ���� �̵� �ӵ�
    [Header("�÷��̾� ȸ��")]
    public bool lockPr = false;//��Ŀ�� ����� �÷��̾� ȸ��
    Quaternion targetRotation; // ��ǥ ȸ�� ��
    Quaternion startRotation; // ���� ȸ����
    [SerializeField] Transform pr; // �÷��̾� ��ġ��
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
    public Coroutine moveCoroutine;



    public enum LockerStat
    {
        In,
        InMove,
        OutMove,
        None
    }

    public LockerStat stat = LockerStat.None;
    private void Start()
    {
        startPr = this.transform.position + this.transform.forward * 0.4f;//�÷��̾ ���Ë� ��Ŀ���� ���鿡�� �տ��� ������ �ϱ�
        door_Obj = door.GetComponent<Door>();
        boxcollider = GetComponent<BoxCollider>();
        speed = 0.01f;
    }
    public void PlayerHide()
    {
        //���� ��ȯ
        stat = LockerStat.InMove;
        PlayerController.instance.stat = PlayerController.PlayerState.Hiding;
        PlayerController.instance.Close_PlayerController();//�÷��̾� ��Ʈ�� OFF
        lockPr = true;
        ProgressManager.Instance.progressData.hideInLocker = true;

        Camera_Rt.instance.Close_Camera();//ī�޶� ȸ�� ���
        Select_Locker();//��Ŀ ����
        pr = Chapter1_Mgr.instance.player.transform;
        //startPr = pr.position; //�÷��̾� ���� ��ġ�� ���� �÷��̾ ���ö� �����ߴ� ��ġ�� �̵��ϱ� ���� ��ǥ��
        //Debug.Log("ó�� ��ǥ ��ǥ: " + startPr);
        if (lockPr)
        {
            startRotation = pr.rotation;
            // targetRotation = Quaternion.Euler(0, startRotation.eulerAngles.y + rotateRange, 0);
            targetRotation = setTr.rotation;
            StartCoroutine(PlayerController.instance.SmoothRotateTo(PlayerController.instance.GetPlayerCamera().transform, Vector3.zero, 0.25f)); // �÷��̾� ī�޶� ���� ����
            // Debug.Log("���� ȸ��: " + startRotation.eulerAngles);
            // Debug.Log("��ǥ ȸ��: " + targetRotation.eulerAngles);
        }

        if (CommonUIManager.instance.uiManager is SchoolUIManager schoolUIManager)
        {
            // ������ ��Ȱ��ȭ
            Camera_Rt.instance.postProecessingBehaviour.gameObject.GetComponent<RayCast_Aim>().flashlight.SetActive(false);
            if (ProgressManager.Instance != null && ProgressManager.Instance.progressData.fogName == "Nightmare")
            {
                RenderSettings.fogDensity = 0.55f; // Fog fogDensity �� ����
            }

            if (this.gameObject.name.Contains("Lounge Locker")
            && schoolUIManager.useLockerKey
            && !ProgressManager.Instance.IsActionCompleted(ProgressManager.ActionType.GetOutOfLocker))
            {
                schoolUIManager.StartLoungeTimeLine();
            }
        }

    }

    public void OpenLocker()//��Ŀ�� ���� ���� ��
    {
        Debug.Log("Played OpenLocker");
        stat = LockerStat.OutMove;
        PlayerController.instance.stat = PlayerController.PlayerState.Idle;
        Camera_Rt.instance.Close_Camera();
        ProgressManager.Instance.progressData.hideInLocker = false;

        if (!PlayerController.instance.stat.Equals(PlayerController.PlayerState.Idle))
        {
            return;
        }
        Select_Locker();// ��Ŀ ����

        //Debug.Log("���� ��ǥ: " + pr.position);
        //Debug.Log("��ǥ ��ǥ: " + startPr);
        outMovingToLocker = true;

        if (CommonUIManager.instance.uiManager is SchoolUIManager schoolUIManager)
        {
            if (this.gameObject.name.Contains("Lounge Locker")
                && ProgressManager.Instance.IsActionCompleted(ProgressManager.ActionType.UseLockerKey)
                && !ProgressManager.Instance.IsActionCompleted(ProgressManager.ActionType.GetOutOfLocker))
            {
                schoolUIManager.FinishLoungeTimeLine();
            }
        }
    }

    //private void FixedUpdate()
    //{
    //    if (stat.Equals(LockerStat.InMove)) //��Ŀ�ȱ��� �÷��̾� �̵� �� ī�޶� ȸ��
    //    {

    //        pr.position = Vector3.MoveTowards(pr.position, setTr.position, Time.fixedDeltaTime * 2f);//�̵� �ӵ� ���� �ʿ�

    //        if (lockPr)
    //        {
    //            pr.rotation = Quaternion.Slerp(pr.rotation, targetRotation, Time.deltaTime * 10f);//ī�޶� ȸ�� �ӵ�
    //        }

    //        if (Vector3.Distance(pr.position, setTr.position) < 0.01f && Quaternion.Angle(pr.rotation, targetRotation) < 1f)
    //        {

    //            pr.rotation = targetRotation;
    //            lockPr = false;
    //            //Debug.Log("���� ȸ��: " + pr.rotation.eulerAngles);

    //            if (currentCoroutine == null)
    //            {
    //                InLocker();//�������� �ڷ�ƾ ������ ����
    //            }

    //        }


    //    }

    //    if (stat.Equals(LockerStat.OutMove))
    //    {
    //        // Debug.Log($"Player Pos : {pr.position}, start Pos : {startPr}");
    //        pr.position = Vector3.MoveTowards(pr.position, startPr, Time.fixedDeltaTime * 2f);//������ �ӵ� ���� �ʿ�
    //        // Debug.Log($"Player Pos : {pr.position}");
    //        if (Vector3.Distance(pr.position, startPr) < 0.01f)
    //        {
    //            //Debug.Log("��Ŀ Ż��");
    //            // Debug.Log($"Final Player Pos : {pr.position}, start Pos : {startPr}");
    //            if (currentCoroutine == null)
    //            {
    //                OutLocker();//�������� �ڷ�ƾ ������ ����
    //            }

    //        }
    //    }

    //}

    public void StartMoveToTarget()
    {
        moveCoroutine = StartCoroutine(MoveAndRotateToTarget());
    }

    private IEnumerator MoveAndRotateToTarget()
    {
        PlayerController.instance.stat = PlayerController.PlayerState.Moving;

        float moveSpeed = 2f;
        float rotateSpeed = 10f;

        if (lockPr)
        {
            while (Vector3.Distance(pr.position, setTr.position) > 0.01f || Quaternion.Angle(pr.rotation, targetRotation) > 1f)
            {
                // ��ġ �̵�
                pr.position = Vector3.MoveTowards(pr.position, setTr.position, Time.deltaTime * moveSpeed);
                // ȸ�� (�ε巴��)
                pr.rotation = Quaternion.Slerp(pr.rotation, targetRotation, Time.deltaTime * rotateSpeed);

                yield return null; // ���� �����ӱ��� ���
            }

            // ���� ��ġ �� ȸ�� ��Ȯ�� ����
            pr.position = setTr.position;
            pr.rotation = targetRotation;
        }
        else
        {
            while (Vector3.Distance(pr.position, startPr) > 0.01f)
            {
                // ��ġ�� �̵�
                pr.position = Vector3.MoveTowards(pr.position, startPr, Time.deltaTime * moveSpeed);

                yield return null; // ���� �����ӱ��� ���
            }

            // ���� ��ġ ��Ȯ�� ����
            pr.position = startPr;
        }

        if (currentCoroutine == null)
        {
            if (lockPr) { InLocker(); }
            else { OutLocker(); }
        }

        lockPr = false;
        moveCoroutine = null;
    }


    public void OutLocker()
    {
        Debug.Log("Playered Out Locker");
        PlayerController.instance.stat = PlayerController.PlayerState.Idle;
        Select_Locker(); // �÷��̾� Ż�� �� ���ݱ�
        Camera_Rt.instance.Open_Camera();
        PlayerController.instance.Open_PlayerController();
        stat = LockerStat.None;
        outMovingToLocker = false;
    }
    public void InLocker() //�÷��̾� ������ ���� �ݱ�
    {
        Debug.Log("Playered In Locker");
        PlayerController.instance.stat = PlayerController.PlayerState.Hide;
        //Debug.Log("���� ȸ��2" + pr.rotation);
        Select_Locker(); // �÷��̾� ������ ���ݱ�
        // Debug.Log("���");
        lockPr = false;
        PlayerController.instance.Close_PlayerController();
        Camera_Rt.instance.Close_Camera();
        stat = LockerStat.In;
        isMovingToLocker = false;
    }


    public void Select_Locker()//���� ����
    {
        if (!isRotation)
        {
            isRotation = true;
            StartCoroutine(RotationDoor());
        }

    }

    public void OpenDoor()
    {
        if (!isRotation)
        {
            isRotation = true;
            StartCoroutine(RotationDoor(2f));
        }
    }

    private IEnumerator RotationDoor(float time = 0.5f)//���� ����
    {
        float startTime = 0f;
        float speedMultiplier = 2.5f; // �ӵ� ����
        startRotationLocker = transform.rotation;

        if (doorState)
            endRotation = Quaternion.Euler(0, startRotationLocker.eulerAngles.y + 110, 0);
        else
            endRotation = Quaternion.Euler(0, startRotationLocker.eulerAngles.y - 110, 0);

        istrigger_on();

        while (startTime < time)
        {
            float t = (startTime / time);
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
        boxcollider.enabled = false;
        // �θ��� MeshCollider�� ��Ȱ��ȭ
        transform.parent.GetComponent<MeshCollider>().enabled = false;
    }
    public void istrigger_off()
    {
        boxcollider.isTrigger = false;
        boxcollider.enabled = true;
        // �θ��� MeshCollider�� ��Ȱ��ȭ
        transform.parent.GetComponent<MeshCollider>().enabled = true;
    }


    public void Clear_Locker()
    {
        // �÷��̾� ���� ���� �ʱ�ȭ
        isMovingToLocker = false;
        outMovingToLocker = false;
        lockPr = false;

        // �̵��� ȸ�� �� �ʱ�ȭ
        //startPr = Vector3.zero;
        startRotation = Quaternion.identity;

        // ��Ŀ �� ���� �ʱ�ȭ
        doorState = true; // �⺻������ ���� ���·� ����
        isRotation = false;

        // �� ȸ�� �ʱ�ȭ
        transform.rotation = startRotationLocker;

        // �ڷ�ƾ �ʱ�ȭ
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
            currentCoroutine = null;
        }

        // �浹 ó�� �ʱ�ȭ
        boxcollider.isTrigger = false;
        if (transform.parent != null && transform.parent.GetComponent<MeshCollider>() != null)
        {
            transform.parent.GetComponent<MeshCollider>().enabled = true;
        }

        // ��Ŀ ���� �ʱ�ȭ
        stat = LockerStat.None;

        Debug.Log("��Ŀ ���°� �ʱ�ȭ�Ǿ����ϴ�.");
    }
}
