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
    // �̱��� ������ ����Ͽ� �ϳ��� Enemy �ν��Ͻ��� �����ϵ��� ����
    public static Enemy enemy_single { get; private set; }

    // �÷��̾ �������� ���θ� ��Ÿ���� ���� (�⺻��: false)
    private bool caught_player = false;

    // �÷��̾ �׾��� �� ī�޶��� Ÿ�� ��ġ
    public Transform deathCamTarget;

    // ������ �÷��̾��� Transform
    public Transform targetPlayer;

    // �� ĳ������ �ִϸ�����
    public Animator animator;

    private void Awake()
    {
        // �̱��� �ν��Ͻ� ����
        if (enemy_single == null)
        {
            enemy_single = this;
        }
    

        // Animator ������Ʈ ��������
        animator = GetComponent<Animator>();

        // GameManager���� �÷��̾� Transform�� �޾ƿ��� �ڵ� (�ּ� ó����)
        // targetPlayer = GameManager.instance.player_tr;
    }

    private void OnTriggerEnter(Collider other)
    {
        // �浹�� ��ü�� "Player" �±׸� ������ �ְ�, ���� �÷��̾ ���� �ʾҴٸ� ����
        if (other.CompareTag("Player") && !caught_player)
        {
            caught_player = true; // �÷��̾ ����

            // �÷��̾��� �Է��� ��Ȱ��ȭ
            other.GetComponent<PlayerController>().DisableInput();

            // ���� ���ɾ�(���� ����) ������ ����
            StartCoroutine(JumpscareSequence());
        }
    }

    private IEnumerator JumpscareSequence()
    {
        // �÷��̾� ī�޶� ȸ����Ű�� ���� ����
        PlayerMainCamera.camera_single.RotateTarget();

        // ȸ���� �Ϸ�� ������ ���
        yield return new WaitForSeconds(PlayerMainCamera.camera_single.rotationDuration);

        // ���� �÷��̾� �տ� �����̵�
        TeleportEnemy();

        // ī�޶� ����Ʈ ���� (��: ȭ�� ������ ��)
        PlayerMainCamera.camera_single.CameraEffect();

        // ���� ���� �ִϸ��̼� ����
        animator.SetTrigger("Attack");
    }

    public void TeleportEnemy()
    {
        // ���� ���ɾ� �߻� �� �÷��̾� ���� ���� �Ÿ��� �����̵�
        float jumpscareDistance = 2f;

        // �÷��̾� ī�޶� �ٶ󺸴� ���� ��� (���� ���⸸ ���)
        Vector3 cameraForward = PlayerMainCamera.camera_single.transform.forward;
        cameraForward.y = 0;
        cameraForward.Normalize();

        // ���� �����̵� ��ġ ���
        Vector3 jumpscarePosition = targetPlayer.position + (cameraForward * jumpscareDistance);

        // ���� ���̸� �����Ͽ� �ڿ������� ���� ����
        float heightOffset = -3f;
        jumpscarePosition.y = targetPlayer.position.y + heightOffset;

        // ���� �����̵� ��ġ�� �̵�
        transform.position = jumpscarePosition;

        // ���� �÷��̾ �ٶ󺸵��� ȸ�� ����
        transform.rotation = Quaternion.LookRotation(-cameraForward);

        // ���� ȸ�� ������ �����Ͽ� Ư���� �ð��� ���� ����
        Vector3 fixedEuler = transform.rotation.eulerAngles;
        fixedEuler.x = 30f;
        transform.rotation = Quaternion.Euler(fixedEuler);
    }

    // �⺻ Object Ŭ������ �޼��带 ������ (�ʿ����� �ʴٸ� ���� ����)
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
