using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;
using static UnityEngine.GraphicsBuffer;

public class Enemy : MonoBehaviour
{
    public static Enemy enemy_single {  get; private set; }
    private bool caught_player = false; // 플레이어 캐치 전 = false, 플레이어 캐치 후 = true
    public Transform deathCamTarget; // 게임매니저에서 플레이어 불러오도록 하기
    public Transform targetPlayer;
    public Animator animator;

    private void Awake()
    {
        if (enemy_single == null)
        {
            enemy_single = this;
        }
        animator = GetComponent<Animator>();
        //targetPlayer = GameManager.instance.player_tr;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !caught_player)
        {
            caught_player = true;
            other.GetComponent<PlayerController>().DisableInput();
            StartCoroutine(JumpscareSequence());
        }
    }
    private IEnumerator JumpscareSequence()
    {
        PlayerMainCamera.camera_single.RotateTarget();

        yield return new WaitForSeconds(PlayerMainCamera.camera_single.rotationDuration);

        TeleportEnemy();
        PlayerMainCamera.camera_single.CameraEffect();
        animator.SetTrigger("Attack");
    }

    public void TeleportEnemy()
    {
        float jumpscareDistance = 12f;

        Vector3 cameraForward = PlayerMainCamera.camera_single.transform.forward;
        cameraForward.y = 0;
        cameraForward.Normalize();

        Vector3 jumpscarePosition = targetPlayer.position + (cameraForward * jumpscareDistance);
        float heightOffset = -3f;
        jumpscarePosition.y = targetPlayer.position.y + heightOffset;

        transform.position = jumpscarePosition;

        transform.rotation = Quaternion.LookRotation(-cameraForward);
        Vector3 fixedEuler = transform.rotation.eulerAngles;
        fixedEuler.x = 30f;
        transform.rotation = Quaternion.Euler(fixedEuler);

    }

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
