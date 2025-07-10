using UnityEngine;
using System.Collections;
using UnityStandardAssets.Characters.FirstPerson;

public class NHSupervisor : MonoBehaviour
{
    public static NHSupervisor instance;

    [SerializeField] Transform moveTr;
    [SerializeField] Transform endPoint;
    [SerializeField] Transform endPoint2;
    [SerializeField] float speed = 5f;
    [SerializeField] float stopDistance = 0.1f;

    [SerializeField] GameObject roomDoor;
    [SerializeField] GameObject roomDoor2;

    private Animator anim;
    private bool isMoving = false;
    private int movementStage = 0;
    private Transform currentTarget;

    private Transform playerTr;
    private bool isDraggingPlayer = false;
    [SerializeField] float dragDistance = 1.8f;
    [SerializeField] float playerFollowSpeed = 3f;

    void Start()
    {
        if (instance == null) instance = this;

        anim = GetComponent<Animator>();
        if (anim == null) Debug.LogError("Animator ����");

        playerTr = PlayerController.instance.transform;
    }

    void Update()
    {
        if (isMoving && currentTarget != null)
        {
            float distanceToTarget = Vector3.Distance(transform.position, currentTarget.position);

            if (distanceToTarget < stopDistance)
            {
                transform.position = currentTarget.position;
                HandleMovementStage();
            }
            else
            {
                Vector3 direction = (currentTarget.position - transform.position).normalized;
                transform.position += direction * speed * Time.deltaTime;
            }
        }

        if (isDraggingPlayer)
        {
            // Supervisor�� ���� �Ÿ� �����ϸ� �÷��̾ ���� �̵�
            Vector3 targetPos = transform.position - transform.forward * dragDistance;
            playerTr.position = Vector3.MoveTowards(playerTr.position, targetPos, playerFollowSpeed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (movementStage == 1 && other.CompareTag("Player"))
        {
            Debug.Log("�÷��̾ ������ �� �� ���� ���� �ܰ�� �̵�");
            roomDoor.GetComponent<Door>().Select_Door();

            PlayerController.instance.Close_PlayerController(); // ���� ����
            isDraggingPlayer = true;

            MoveTo(endPoint);
        }
    }

    public void StartMovement()
    {
        anim?.SetBool("isWalk", true);
        movementStage = 0;
        MoveTo(moveTr);
    }

    private void MoveTo(Transform target)
    {
        currentTarget = target;
        isMoving = true;
        anim?.SetBool("isWalk", true);
        anim?.SetTrigger("isIdle");
    }

    private void StopAndIdle()
    {
        isMoving = false;
        anim?.SetBool("isWalk", false);
        anim?.SetTrigger("isIdle");
    }

    private void HandleMovementStage()
    {
        isMoving = false;

        switch (movementStage)
        {
            case 0:
                Debug.Log("1�ܰ� ����. Ʈ���� ���");
                movementStage = 1;
                anim.SetTrigger("isIdle");
                break;

            case 1:
                Debug.Log("2�ܰ� ����. �� ����");
                roomDoor2.GetComponent<Door>().Select_Door();
                MoveTo(endPoint2);
                movementStage = 2;
                break;

            case 2:
                Debug.Log("���� ����");
                StopAndIdle();
                isDraggingPlayer = false;
                PlayerController.instance.Open_PlayerController(); // ���� ����
                break;
        }
    }
}
