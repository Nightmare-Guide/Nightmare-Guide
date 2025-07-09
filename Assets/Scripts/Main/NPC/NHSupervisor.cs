using UnityEngine;
using System.Collections;

public class NHSupervisor : MonoBehaviour
{
    public static NHSupervisor instance;
    [SerializeField] Transform moveTr; // ��ǥ ��ġ�� ������ Transform
    [SerializeField] Transform endPoint; // ��ǥ ��ġ�� ������ Transform
    [SerializeField] float speed = 5f; // �̵� �ӵ�
    [SerializeField] float stopDistance = 0.1f; // ��ǥ ���� ��ó�� �����ߴ��� �Ǵ��� �Ÿ�

    private Animator anim;
    private bool isMoving = false; // �̵� ������ ����




    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }

        anim = GetComponent<Animator>();
        if (anim == null)
        {
            Debug.LogError("Animator ������Ʈ�� NHSupervisor�� ���� ������Ʈ�� �����ϴ�!");
        }
    }

    void Update()
    {
        // isMoving�� true�� ���� �̵� ���� ����
        if (isMoving && moveTr != null)
        {
            // ��ǥ ��ġ������ ���� ���� ���
            Vector3 direction = (moveTr.position - transform.position).normalized;

            // ���� ��ġ���� ��ǥ ��ġ������ �Ÿ� ���
            float distanceToTarget = Vector3.Distance(transform.position, moveTr.position);

            // ��ǥ ������ ���� �����ߴٸ� ����
            if (distanceToTarget < stopDistance)
            {
                transform.position = moveTr.position; // ��Ȯ�� ��ǥ ��ġ�� ����
                StopMovement();
            }
            else
            {
                // ���� ��ġ�� ��ǥ �������� speed��ŭ �̵�
                transform.position += direction * speed * Time.deltaTime;
            }
        }
    }

    // Ÿ�Ӷ��� ���� �� Signal Receiver�� ���� ȣ��� �Լ�
    public void StartMovement()
    {
        Debug.Log("Timeline Ended! Starting Movement...");

        // �ִϸ��̼��� "isWalk" �Ķ���͸� true�� ����
        if (anim != null)
        {
            anim.SetBool("isWalk", true);
        }
        else
        {
            Debug.LogWarning("Animator is null. Cannot set 'isWalk' parameter.");
        }

        // �̵� ���� �÷��� ����
        isMoving = true;
    }

    // �̵��� ���߰� �ִϸ��̼��� ���� �Լ�
    private void StopMovement()
    {
        Debug.Log("Movement Finished!");
        isMoving = false; // �̵� ����

        if (anim != null)
        {
            anim.SetBool("isWalk", false); // �ȱ� �ִϸ��̼� ����
            anim.SetBool("isIdle1", true);
        }
    }
}