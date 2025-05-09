using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Supervisor : MonoBehaviour
{
    public Animator animator;
    public string storyprogress;
    private NavMeshAgent agent;
    void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        Walk();
        Idle();
        Talk();
    }

    public void Walk()
    {
        //if () idle���� walk�� ���� ��ȭâ ���൵ ���ǹ� ����
        //{
        //    animator.SetTrigger("isWalk");
        //}
    }
    public void Idle()
    {
        //if () Idle�� �ٲ�� ����
        //{
        //    animator.SetTrigger("isIdle");
        //}
    }
    public void Talk()
    {
        //if ()
        //{
        //    animator.SetTrigger("talk");
        //}
    }
    public void NPCMove()
    {
        Vector3 targetPosition = new Vector3(5, 0, 5); // ���ϴ� ��ġ
        agent.SetDestination(targetPosition);
    }
}
