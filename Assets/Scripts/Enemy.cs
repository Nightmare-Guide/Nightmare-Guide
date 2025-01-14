using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class Enemy : MonoBehaviour
{
    public static Enemy enemy_single {  get; private set; }
    private bool caught_player = false; // �÷��̾� ĳġ �� = false, �÷��̾� ĳġ �� = true
    public Transform deathTarget;

    private void Awake()
    {
        if (enemy_single == null)
        {
            enemy_single = this;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !caught_player)
        {
            caught_player = true;

            other.GetComponent<PlayerController>().DisableInput(); // ������Ʈ ������ , �Է� ���ۺҰ����ϰ� ����
            Debug.Log(PlayerMainCamera.camera_single);
            PlayerMainCamera.camera_single.RotateTarget();



        }
    }

}
