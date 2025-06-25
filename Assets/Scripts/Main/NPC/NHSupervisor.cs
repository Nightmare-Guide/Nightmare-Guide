using UnityEngine;
using System.Collections;
// �������� AnimHelper ��ũ��Ʈ�� ����ϱ� ���� �߰��մϴ�.
// AnimHelper ��ũ��Ʈ�� NPC ���ӽ����̽� �ȿ� �ִٸ� using NPC; �� �߰��ؾ� �մϴ�.
// UnityEditor.Experimental.GraphView.GraphView�� �Ϲ������� ��Ÿ�� �ڵ忡 �ʿ����� �����Ƿ� �����մϴ�.
// static NPC; �� static UnityEditor.Experimental.GraphView.GraphView; �� �����մϴ�.
// AnimHelper ��ũ��Ʈ�� ������ ���ӽ����̽� ���� ������Ʈ ��Ʈ�� �ִٸ� using ���� �ʿ� ���� ���� �ֽ��ϴ�.
// ���⼭�� ������ ���ӽ����̽� ���� ���� ȣ���Ѵٰ� �����մϴ�.

public class NHSupervisor : NPC
{
    // �⺻ ���� ����
    [SerializeField]
    [Tooltip("ĳ���Ͱ� ���������� �̵��� ��ǥ �������Դϴ�.")]
    private Transform[] moveTr; // �̵��� ��ǥ Transform �迭

    // �̵� ���� �� ���� ����
    private bool isMoving = false; // ���� �̵� ������ ����
    private Coroutine currentMoveCoroutine; // ���� ���� ���� �̵� �ڷ�ƾ ����

    // ĳ���� �̵� �� ȸ�� �ӵ�
    public float speed = 5f; // �̵� �ӵ�
    public float rotationSpeed = 100f; // ȸ�� �ӵ�

    

    
    void Start()
    {
        // �׽�Ʈ�� ���� ���� ���� �� �ٷ� NPC �帧 ����
        StartNPCFlow();
    }
    // ---
    // 1. NPC�� �����ʰ� �������� ���� ȸ���ϴ� �ڷ�ƾ
    // ---

    /// <summary>
    /// NPC�� ������ ������ŭ �������� ȸ����Ű�� �ڷ�ƾ.
    /// </summary>
    /// <param name="angle">ȸ���� ���� (��: 90f)</param>
    public IEnumerator RotateLeft(float angle)
    {
        float currentRotatedAngle = 0f;
        Quaternion startRotation = transform.rotation;
        Quaternion targetRotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y - angle, transform.eulerAngles.z);

        while (currentRotatedAngle < angle)
        {
            float step = rotationSpeed * Time.deltaTime;
            transform.Rotate(0, -step, 0, Space.Self); // Y�� �������� �������� ȸ��
            currentRotatedAngle += step;
            yield return null;
        }
        // ��Ȯ�� ��ǥ ������ �����ϵ��� ������ ����
        transform.rotation = targetRotation;
    }

    /// <summary>
    /// NPC�� ������ ������ŭ ���������� ȸ����Ű�� �ڷ�ƾ.
    /// </summary>
    /// <param name="angle">ȸ���� ���� (��: 90f)</param>
    public IEnumerator RotateRight(float angle)
    {
        float currentRotatedAngle = 0f;
        Quaternion startRotation = transform.rotation;
        Quaternion targetRotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y + angle, transform.eulerAngles.z);

        while (currentRotatedAngle < angle)
        {
            float step = rotationSpeed * Time.deltaTime;
            transform.Rotate(0, step, 0, Space.Self); // Y�� �������� ���������� ȸ��
            currentRotatedAngle += step;
            yield return null;
        }
        // ��Ȯ�� ��ǥ ������ �����ϵ��� ������ ����
        transform.rotation = targetRotation;
    }

    // ---
    // Ư�� ��ǥ �������� �̵��ϴ� ���� �ڷ�ƾ
    // ---
    private IEnumerator MoveToTargetWaypoint(Transform targetWaypoint)
    {
        if (targetWaypoint == null)
        {
            Debug.LogWarning("NHSupervisor: �̵��� ��ǥ ������ null�Դϴ�.");
            yield break;
        }

        // ��ǥ �������� �̵�
        while (Vector3.Distance(transform.position, targetWaypoint.position) > 0.1f) // ����� ������� ������
        {
            // ��ǥ ���� ���
            Vector3 direction = (targetWaypoint.position - transform.position).normalized;
            // Y�� ȸ���� ���
            direction.y = 0;
            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime / 100); // ȸ�� �ӵ� ����
            }

            // �̵�
            transform.position = Vector3.MoveTowards(transform.position, targetWaypoint.position, speed * Time.deltaTime);

            yield return null; // ���� �����ӱ��� ���
        }
        Debug.Log($"NHSupervisor: {targetWaypoint.name} ������ �����߽��ϴ�.");
    }

    // ---
    // ��û�Ͻ� NPC �ൿ �帧�� �����ϴ� ���� �ڷ�ƾ
    // ---
    public IEnumerator NPCFlowSequence()
    {
        if (moveTr == null || moveTr.Length < 4) // �ּ� 4���� ����Ʈ�� �ʿ�
        {
            Debug.LogError("NHSupervisor: NPC �帧�� �����Ϸ��� �ּ� 4���� moveTr ��ǥ ������ �ʿ��մϴ�.");
            yield break;
        }

        isMoving = true; // �帧 ����


        // 1. 10���� -> ���������� ȸ�� -> ��ũ�ִϸ��̼� ����-> ����Ʈ 1���� �̵�
        Debug.Log("NHSupervisor: 10�� ��� ��...");
        yield return new WaitForSeconds(10f);

        Debug.Log("NHSupervisor: ���������� 90�� ȸ�� ����...");
        yield return StartCoroutine(RotateRight(90f)); // ���������� 90�� ȸ��
        AnimHelper.TryPlay(myAnim, "walk", 0.2f);
        Debug.Log("NHSupervisor: ����Ʈ 1�� �̵� ����...");
        yield return StartCoroutine(MoveToTargetWaypoint(moveTr[0])); // moveTr[0]�� ����Ʈ 1

        // 2. ����Ʈ1���� ȸ���ִϸ��̼�->��ũ�ִϸ��̼�-> ����Ʈ2���� �̵�
        Debug.Log("NHSupervisor: ����Ʈ 1���� ȸ�� �� ����Ʈ 2�� �̵� ����...");
        AnimHelper.TryPlay(myAnim, "idle1", 0.2f);
        yield return new WaitForSeconds(0.5f); // �ִϸ��̼� ��ȯ ���
        yield return StartCoroutine(RotateRight(90f)); // ����: ���������� 90�� ȸ�� (�ʿ��� �������� ����)
        AnimHelper.TryPlay(myAnim, "walk", 0.2f);
        yield return StartCoroutine(MoveToTargetWaypoint(moveTr[1])); // moveTr[1]�� ����Ʈ 2

        // 3. ����Ʈ 2���� ȸ���ִϸ��̼�->��ũ�ִϸ��̼�-> ����Ʈ3���� �̵�
        Debug.Log("NHSupervisor: ����Ʈ 2���� ȸ�� �� ����Ʈ 3���� �̵� ����...");
        AnimHelper.TryPlay(myAnim, "idle1", 0.2f);
        yield return new WaitForSeconds(0.5f); // �ִϸ��̼� ��ȯ ���
        yield return StartCoroutine(RotateLeft(90f)); // ����: �������� 90�� ȸ�� (�ʿ��� �������� ����)
        AnimHelper.TryPlay(myAnim, "walk", 0.2f);
        yield return StartCoroutine(MoveToTargetWaypoint(moveTr[2])); // moveTr[2]�� ����Ʈ 3

        // 4. ����Ʈ 3���� ȸ���ִϸ��̼�->��ũ�ִϸ��̼�-> ����Ʈ4���� �̵�
        Debug.Log("NHSupervisor: ����Ʈ 3���� ȸ�� �� ����Ʈ 4�� �̵� ����...");
        AnimHelper.TryPlay(myAnim, "idle1", 0.2f);
        yield return new WaitForSeconds(0.5f); // �ִϸ��̼� ��ȯ ���
        yield return StartCoroutine(RotateRight(180f)); // ����: 180�� ȸ�� (�ʿ��� �������� ����)
        AnimHelper.TryPlay(myAnim, "walk", 0.2f);
        yield return StartCoroutine(MoveToTargetWaypoint(moveTr[3])); // moveTr[3]�� ����Ʈ 4

        // 5. ����Ʈ 4���� ȸ�� �ִϸ��̼� -> ��ũ�ִϸ��̼�
        Debug.Log("NHSupervisor: ����Ʈ 4���� ȸ�� �� ��ũ �ִϸ��̼� ����...");
        AnimHelper.TryPlay(myAnim, "idle1", 0.2f);
        yield return new WaitForSeconds(0.5f); // �ִϸ��̼� ��ȯ ���
        yield return StartCoroutine(RotateLeft(90f)); // ����: �������� 90�� ȸ�� (�ʿ��� �������� ����)
        AnimHelper.TryPlay(myAnim, "talk1", 0.2f);

        isMoving = false; // �帧 ����
        Debug.Log("NHSupervisor: NPC �ൿ �帧 �Ϸ�.");
    }

    /// <summary>
    /// NPC �ൿ �帧�� �����ϴ� ���� �޼���.
    /// </summary>
    public void StartNPCFlow()
    {
        if (isMoving)
        {
            Debug.LogWarning("NHSupervisor: �̹� NPC �帧�� ���� ���Դϴ�.");
            return;
        }

        if (currentMoveCoroutine != null)
        {
            StopCoroutine(currentMoveCoroutine);
        }
        currentMoveCoroutine = StartCoroutine(NPCFlowSequence());
    }

    /// <summary>
    /// �̵� �������� ������ ���ߴ� ���� �޼���.
    /// </summary>
    public void StopMoveSequence()
    {
        if (currentMoveCoroutine != null)
        {
            StopCoroutine(currentMoveCoroutine);
            currentMoveCoroutine = null;
        }
        isMoving = false;
        AnimHelper.TryPlay(myAnim, "idle1", 0.2f);
        Debug.Log("NHSupervisor: �̵� �������� ������ �����߽��ϴ�.");
    }
}