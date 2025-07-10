using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TheKiwiCoder;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.AI;
using UnityStandardAssets.Characters.FirstPerson;
using static UnityEngine.GraphicsBuffer;

public class Enemy : MonoBehaviour
{
    public EnemyState currentState = EnemyState.Normal;
    public BehaviourTree blackboard;
    // 싱글턴 패턴을 사용하여 하나의 Enemy 인스턴스만 존재하도록 설정
    public static Enemy enemy_single { get; private set; }

    // 플레이어가 잡혔는지 여부를 나타내는 변수 (기본값: false)
    private bool caught_player = false;

    // 플레이어가 죽었을 때 카메라의 타겟 위치
    public Transform deathCamTarget;

    // 추적할 플레이어의 Transform
    public Transform targetPlayer;

    // 적 캐릭터의 애니메이터
    public Animator animator;

    private void Awake()
    {
        // 싱글턴 인스턴스 설정
        if (enemy_single == null)
        {
            enemy_single = this;
        }
    
        // Animator 컴포넌트 가져오기
        animator = GetComponent<Animator>();
    }

    public enum EnemyState
    {
        Normal,
        Frozen
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !caught_player)
        {
            Jumpscare();
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && !caught_player)
        {
            Jumpscare();
        }
    }

    public void InitEnemy(Transform respawnTransform)
    {
        this.gameObject.SetActive(false);
        this.transform.position = respawnTransform.position;
        this.transform.rotation = respawnTransform.rotation;

        caught_player = false;
        currentState = EnemyState.Normal;

        //// Rigidbody 멈춤
        //Rigidbody rb = GetComponent<Rigidbody>();

        //if (rb != null)
        //{
        //    rb.isKinematic = false;
        //    rb.constraints = RigidbodyConstraints.None;
        //}

        //// NavMeshAgent 정지
        //NavMeshAgent agent = GetComponent<NavMeshAgent>();
        //if (agent != null)
        //{
        //    agent.isStopped = false;
        //    agent.updatePosition = true;
        //    agent.updateRotation = true;
        //}

    }

    void Jumpscare()
    {
        caught_player = true;

        // 플레이어 움직임 멈춤
        PlayerController.instance.Close_PlayerController();
        Camera_Rt.instance.Close_Camera();

        // 몬스터 움직임 멈춤
        // FreezeEnemy();

        // 플레이어 카메라가 몬스터를 향해 회전
        PlayerMainCamera.camera_single.RotateToTarget(this.transform, 0.2f);

        this.gameObject.SetActive(false);
    }

    public void FreezeEnemy()
    {
        currentState = EnemyState.Frozen;

        // Rigidbody 멈춤
        Rigidbody rb = GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.isKinematic = true;
            rb.constraints = RigidbodyConstraints.FreezeAll;
        }

        // NavMeshAgent 정지
        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            agent.isStopped = true;
            agent.updatePosition = false;
            agent.updateRotation = false;
        }

    }

    public void LookAtPlayer(Transform playerTransform, float rotationDuration = 1f)
    {
        StartCoroutine(RotateToFacePlayer(playerTransform, rotationDuration));
    }

    private IEnumerator RotateToFacePlayer(Transform target, float duration)
    {
        Quaternion initialRotation = transform.rotation;
        Vector3 directionToTarget = (target.position - transform.position).normalized;
        directionToTarget.y = 0; // Y축 회전만 하도록

        Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
        float elapsed = 0f;

        while (elapsed < duration)
        {
            transform.rotation = Quaternion.Slerp(initialRotation, targetRotation, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.rotation = targetRotation; // 최종 고정
    }


    // 기본 Object 클래스의 메서드를 재정의 (필요하지 않다면 삭제 가능)
    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public override bool Equals(object other)
    {
        return base.Equals(other);
    }

    public override string ToString()
    {
        return base.ToString();
    }
}
