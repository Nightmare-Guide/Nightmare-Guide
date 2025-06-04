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

        // GameManager에서 플레이어 Transform을 받아오는 코드 (주석 처리됨)
        // targetPlayer = GameManager.instance.player_tr;
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
            //caught_player = true;

            //PlayerMainCamera.camera_single.RotateTarget(); // 플레이어 카메라 회전
            //this.LookAtPlayer(targetPlayer, 0f);              // 몬스터 회전

            //animator.SetTrigger("Attack");

            //other.GetComponent<PlayerController>().DisableInput();

            //FreezeEnemy();

            //StartCoroutine(JumpscareSequence());

            Jumpscare();
        }
    }

    public void InitEnemy(Transform respawnTransform)
    {
        this.gameObject.SetActive(true);
        this.transform.position = respawnTransform.position;
        this.transform.rotation = respawnTransform.rotation;

        caught_player = false;
        currentState = EnemyState.Normal;

        // Rigidbody 멈춤
        Rigidbody rb = GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.isKinematic = false;
            rb.constraints = RigidbodyConstraints.None;
        }

        // NavMeshAgent 정지
        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            agent.isStopped = false;
            agent.updatePosition = true;
            agent.updateRotation = true;
        }

    }

    void Jumpscare()
    {
        caught_player = true;

        // 플레이어 움직임 멈춤
        PlayerController.instance.Close_PlayerController();
        Camera_Rt.instance.Close_Camera();

        // 몬스터 움직임 멈춤
        FreezeEnemy();

        // 플레이어 카메라가 몬스터를 향해 회전
        PlayerMainCamera.camera_single.RotateToTarget(this.transform, 0.2f);

        this.gameObject.SetActive(false);
    }

    //private IEnumerator JumpscareSequence()
    //{
    //    // 플레이어 카메라를 회전시키는 연출 실행
    //    PlayerMainCamera.camera_single.RotateTarget();

    //    // 회전이 완료될 때까지 대기
    //    yield return new WaitForSeconds(PlayerMainCamera.camera_single.rotationDuration);

    //    // 적을 플레이어 앞에 순간이동
    //    TeleportEnemy();


    //    // 카메라 이펙트 실행 (예: 화면 깜빡임 등)
    //    PlayerMainCamera.camera_single.CameraEffect();
    //}

    //public void TeleportEnemy()
    //{
    //    float jumpscareDistance = 1f;

    //    // 플레이어 카메라 기준으로 바라보는 방향 (수평만 고려)
    //    Vector3 cameraForward = PlayerMainCamera.camera_single.transform.forward;
    //    cameraForward.y = 0;
    //    cameraForward.Normalize();

    //    // 목표 위치 설정
    //    Vector3 jumpscarePosition = targetPlayer.position + (cameraForward * jumpscareDistance);

    //    // 적 높이 보정
    //    float heightOffset = -1f;
    //    jumpscarePosition.y = targetPlayer.position.y + heightOffset;

    //    // 위치 이동
    //    transform.position = jumpscarePosition;

    //    // Enemy → Player 방향을 정밀하게 계산 (회전)
    //    Vector3 lookDirection = (targetPlayer.position - transform.position);
    //    lookDirection.y = 0; // 수평 방향만 사용
    //    lookDirection.Normalize();

    //    // 적이 플레이어 바라보게 회전
    //    transform.rotation = Quaternion.LookRotation(lookDirection);

    //    // 회전 보정 (X, Z축 고정)
    //    Vector3 euler = transform.rotation.eulerAngles;
    //    euler.x = 0f;
    //    euler.z = 0f;
    //    transform.rotation = Quaternion.Euler(euler);
    //}
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
