using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class Portalwarp : MonoBehaviour
{
    public GameObject exitPortal;
    public bool turnPlayer; // true면 회전, false면 회전X

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            TeleportPlayer(other.gameObject);
        }
    }
    private void TeleportPlayer(GameObject player)
    {
        CharacterController characterController = player.GetComponent<CharacterController>();
        if (characterController != null)
        {
            // 1. CharacterController 비활성화 (강제 이동 문제 방지)
            characterController.enabled = false;

            // 2. 플레이어의 부모 해제 (부모 영향 제거)
            player.transform.SetParent(null);

            // 3. 출구 포탈의 월드 좌표 가져오기
            Vector3 exitPosition = exitPortal.transform.position; // 혹은 exitPortal.transform.TransformPoint(Vector3.zero);

            // 4. 플레이어를 정확한 위치로 이동
            player.transform.position = exitPosition;

            // 5. 플레이어 회전 (필요 시)
            if (turnPlayer)
            {
                player.transform.Rotate(0, 90, 0);
            }

            // 6. CharacterController 다시 활성화 (0.1초 후)
            StartCoroutine(EnableCharacterControllerAfterDelay(characterController));
        }
    }

    // CharacterController 활성화 코루틴
    private IEnumerator EnableCharacterControllerAfterDelay(CharacterController characterController)
    {
        yield return new WaitForSeconds(0.1f);
        characterController.enabled = true;
    }

}
