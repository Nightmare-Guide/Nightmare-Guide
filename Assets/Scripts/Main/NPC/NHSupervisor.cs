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
        Invoke("StartNPCFlow", 5f); // ���� ����
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
      

        Debug.Log("NHSupervisor: -90�� ȸ�� ����");
        yield return StartCoroutine(RotateByRelativeAngle(-90f, 1f));
        Debug.Log("NHSupervisor: talk1 ����");
        anim.SetTrigger("isTalk");
        yield return new WaitForSeconds(2.0f); // talk1 �ִϸ��̼� ����
        CSVRoad_Story.instance.OnSelectChapter("2_0_0");
        yield return new WaitForSeconds(1.0f);
        CSVRoad_Story.instance.OnSelectChapter("2_0_1");

        Debug.Log("NHSupervisor: +160�� ȸ�� ����");
        yield return StartCoroutine(RotateByRelativeAngle(160f, 1f));
        Debug.Log("NHSupervisor: Action1 ����");
        anim.SetTrigger("Action");
        yield return new WaitForSeconds(4f); // Action1 �ִϸ��̼� ����

        Debug.Log("NHSupervisor: walk ����");
        anim.SetBool("isWalk", true);
    }

    private IEnumerator RotateByRelativeAngle(float angle, float duration)
    {
        Quaternion startRot = transform.rotation;
        Quaternion endRot = startRot * Quaternion.Euler(0, angle, 0); // ��� ȸ��

        float elapsed = 0f;
        while (elapsed < duration)
        {
            transform.rotation = Quaternion.Slerp(startRot, endRot, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.rotation = endRot; // ��Ȯ�� ���� �� ����
    }

    private IEnumerator RotateByAngle(float angle, float duration)
    {
        Quaternion startRot = transform.rotation;
        Quaternion endRot = startRot * Quaternion.Euler(0, angle, 0);
        float elapsed = 0f;

        while (elapsed < duration)
        {
            transform.rotation = Quaternion.Slerp(startRot, endRot, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.rotation = endRot;
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
