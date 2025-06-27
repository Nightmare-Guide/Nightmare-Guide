using UnityEngine;
using System.Collections;

public class NHSupervisor : MonoBehaviour
{
    [SerializeField]
    [Tooltip("NPC�� �̵��� �� �ϳ��� ��ǥ �����Դϴ�.")]
    private Transform moveTr;

    public float speed = 5f;
    public float rotationSpeed = 100f;

    private bool isMoving = false;
    private Coroutine currentMoveCoroutine;

    private Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
        Invoke("StartNPCFlow", 10f); // ���� ����
    }

    private IEnumerator MoveToTargetWaypoint(Transform targetWaypoint)
    {
        if (targetWaypoint == null)
        {
            Debug.LogWarning("NHSupervisor: �̵��� ��ǥ ������ null�Դϴ�.");
            yield break;
        }

        while (Vector3.Distance(transform.position, targetWaypoint.position) > 0.1f)
        {
            Vector3 direction = (targetWaypoint.position - transform.position).normalized;
            direction.y = 0;

            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime / 100f);
            }

            transform.position = Vector3.MoveTowards(transform.position, targetWaypoint.position, speed * Time.deltaTime);
            yield return null;
        }

        Debug.Log("NHSupervisor: ����Ʈ 1 ����");

        // ���� �� ������ ����
        StartCoroutine(PlayPostArrivalSequence());
    }

    private IEnumerator PlayPostArrivalSequence()
    {
        Debug.Log("NHSupervisor: talk1 ����");
        anim.SetTrigger("isTalk");
        yield return new WaitForSeconds(2.0f); // talk1 �ִϸ��̼� ����

        Debug.Log("NHSupervisor: Turn_Left ����");
        anim.SetTrigger("LeftTurn");
        yield return new WaitForSeconds(1.0f); // Turn_Left �ִϸ��̼� ����

        Debug.Log("NHSupervisor: Turn_Right ����");
        anim.SetTrigger("RightTurn");
        yield return new WaitForSeconds(1f); // Turn_Right �ִϸ��̼� ����

        Debug.Log("NHSupervisor: Action1 ����");
        anim.SetTrigger("Action");
        yield return new WaitForSeconds(4f); // Action1 �ִϸ��̼� ����

        Debug.Log("NHSupervisor: walk ����");
        anim.SetBool("isWalk", true);
        // �̵� ����� �ʿ� ��, ���� �������� MoveToTargetWaypoint ȣ�� ����
    }

    public IEnumerator NPCFlowSequence()
    {
        if (moveTr == null)
        {
            Debug.LogError("NHSupervisor: moveTr�� �����Ǿ� ���� �ʽ��ϴ�.");
            yield break;
        }

        isMoving = true;

        Debug.Log("NHSupervisor: walk �ִϸ��̼� ����");
        anim.SetBool("isWalk", true);
        yield return StartCoroutine(MoveToTargetWaypoint(moveTr));

        Debug.Log("NHSupervisor: �̵� �Ϸ� - walk ����");
        anim.SetBool("isWalk", false);

        isMoving = false;
    }

    public void StartNPCFlow()
    {
        if (isMoving)
        {
            Debug.LogWarning("NHSupervisor: �̹� ���� ���Դϴ�.");
            return;
        }

        if (currentMoveCoroutine != null)
        {
            StopCoroutine(currentMoveCoroutine);
        }

        currentMoveCoroutine = StartCoroutine(NPCFlowSequence());
    }

    public void StopMoveSequence()
    {
        if (currentMoveCoroutine != null)
        {
            StopCoroutine(currentMoveCoroutine);
            currentMoveCoroutine = null;
        }

        isMoving = false;
        anim.SetTrigger("isIdle");
        Debug.Log("NHSupervisor: ������ ���� ������");
    }
}
