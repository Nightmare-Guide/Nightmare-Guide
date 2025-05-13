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
        //if () idle에서 walk로 갈때 대화창 진행도 조건문 적기
        //{
        //    animator.SetTrigger("isWalk");
        //}
    }
    public void Idle()
    {
        //if () Idle로 바뀌는 조건
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
        Vector3 targetPosition = new Vector3(5, 0, 5); // 원하는 위치
        agent.SetDestination(targetPosition);
    }
}
