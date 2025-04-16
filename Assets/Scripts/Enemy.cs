using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TheKiwiCoder;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;
using static UnityEngine.GraphicsBuffer;

public class Enemy : MonoBehaviour
{
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

    private void OnTriggerEnter(Collider other)
    {
        // 충돌한 객체가 "Player" 태그를 가지고 있고, 아직 플레이어를 잡지 않았다면 실행
        if (other.CompareTag("Player") && !caught_player)
        {
            caught_player = true; // 플레이어가 잡힘

            // 플레이어의 입력을 비활성화
            other.GetComponent<PlayerController>().DisableInput();

            // 점프 스케어(공포 연출) 시퀀스 시작
            StartCoroutine(JumpscareSequence());
        }
    }

    private IEnumerator JumpscareSequence()
    {
        // 플레이어 카메라를 회전시키는 연출 실행
        PlayerMainCamera.camera_single.RotateTarget();

        // 회전이 완료될 때까지 대기
        yield return new WaitForSeconds(PlayerMainCamera.camera_single.rotationDuration);

        // 적을 플레이어 앞에 순간이동
        TeleportEnemy();

        // 카메라 이펙트 실행 (예: 화면 깜빡임 등)
        PlayerMainCamera.camera_single.CameraEffect();

        // 적의 공격 애니메이션 실행
        animator.SetTrigger("Attack");
    }

    public void TeleportEnemy()
    {
        // 점프 스케어 발생 시 플레이어 앞의 일정 거리로 순간이동
        float jumpscareDistance = 2f;

        // 플레이어 카메라가 바라보는 방향 계산 (수평 방향만 고려)
        Vector3 cameraForward = PlayerMainCamera.camera_single.transform.forward;
        cameraForward.y = 0;
        cameraForward.Normalize();

        // 적의 순간이동 위치 계산
        Vector3 jumpscarePosition = targetPlayer.position + (cameraForward * jumpscareDistance);

        // 적의 높이를 조정하여 자연스러운 연출 구현
        float heightOffset = -3f;
        jumpscarePosition.y = targetPlayer.position.y + heightOffset;

        // 적을 순간이동 위치로 이동
        transform.position = jumpscarePosition;

        // 적이 플레이어를 바라보도록 회전 설정
        transform.rotation = Quaternion.LookRotation(-cameraForward);

        // 적의 회전 각도를 수정하여 특정한 시각적 연출 구현
        Vector3 fixedEuler = transform.rotation.eulerAngles;
        fixedEuler.x = 30f;
        transform.rotation = Quaternion.Euler(fixedEuler);
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
