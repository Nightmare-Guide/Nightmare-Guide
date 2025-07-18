using System;
using System.Collections;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;
using UnityStandardAssets.CrossPlatformInput;
using UnityStandardAssets.Utility;
using Random = UnityEngine.Random;

namespace UnityStandardAssets.Characters.FirstPerson
{
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(AudioSource))]

    public class PlayerController : MonoBehaviour
    {
        public static PlayerController instance;
        
        [SerializeField] private bool m_IsWalking;
        [SerializeField] private float m_WalkSpeed;
        [SerializeField] private float m_RunSpeed;
        [SerializeField][Range(-10f, 1f)] private float m_RunstepLenghten;
        [SerializeField] private float m_JumpSpeed;
        [SerializeField] private float m_StickToGroundForce;
        [SerializeField] private float m_GravityMultiplier;
        [SerializeField] private MouseLook m_MouseLook;
        [SerializeField] private bool m_UseFovKick;
        [SerializeField] private FOVKick m_FovKick = new FOVKick();
        [SerializeField] private bool m_UseHeadBob;
        [SerializeField] private CurveControlledBob m_HeadBob = new CurveControlledBob();
        [SerializeField] private LerpControlledBob m_JumpBob = new LerpControlledBob();
        [SerializeField] private float m_StepInterval;
        [SerializeField] private AudioClip[] m_FootstepSounds;    // an array of footstep sounds that will be randomly selected from.
        [SerializeField] private AudioClip m_JumpSound;           // the sound played when character leaves the ground.
        [SerializeField] private AudioClip m_LandSound;           // the sound played when character touches back on ground.

        [SerializeField] private Camera m_Camera;
        private bool m_Jump;
        private float m_YRotation;
        private Vector2 m_Input;
        private Vector3 m_MoveDir = Vector3.zero;
        private CharacterController m_CharacterController;
        private CollisionFlags m_CollisionFlags;
        private bool m_PreviouslyGrounded;
        private Vector3 m_OriginalCameraPosition;
        private float m_StepCycle;
        private float m_NextStep;
        private bool m_Jumping;
        private AudioSource m_AudioSource;
        public NavMeshAgent agent;
        public Transform playerwalkposition;
        private Transform playerTransform;
        public Transform destination;
        public Animator anim;

        public enum PlayerState
        {
            Idle,
            Hide,
            Hiding,
            Death,
            Moving // -> 이동 애니메이션,코루틴 전용
        }
        public PlayerState stat = PlayerState.Idle;

        // Use this for initialization
        private void Start()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject); // 중복된 인스턴스 제거
            }
            if (agent == null)
            {
                TryGetComponent(out NavMeshAgent agent);
                // agent = GetComponent<NavMeshAgent>();
            }

            playerTransform = GetComponent<Transform>();
            m_CharacterController = GetComponent<CharacterController>();
            m_Camera = Camera.main;
            m_OriginalCameraPosition = m_Camera.transform.localPosition; // 카메라 로컬 포지션입력
            m_FovKick.Setup(m_Camera);
            m_HeadBob.Setup(m_Camera, m_StepInterval);
            m_StepCycle = 0f;
            m_NextStep = m_StepCycle / 2f;
            m_Jumping = false;
            m_AudioSource = GetComponent<AudioSource>();
            m_MouseLook.Init(transform, m_Camera.transform);
            if (ProgressManager.Instance != null&&!ProgressManager.Instance.progressData.newGame) { SpawnSet(); }
            anim = GetComponent<Animator>();

        }


        // Update is called once per frame
        private void Update()
        {
            //RotateView(); // 기존 회전 함수가 있다면 여기에 포함
            HandleJump();
        }
        public void SpawnSet()
        {
            Close_PlayerController();
            if (ProgressManager.Instance != null)
            {
                transform.position = ProgressManager.Instance.progressData.playerPosition;
            }
            Open_PlayerController();
          
        }

        private void HandleJump()
        {
            if (!m_Jump)
            {
                m_Jump = CrossPlatformInputManager.GetButtonDown("Jump");
            }

            if (!m_PreviouslyGrounded && m_CharacterController.isGrounded)
            {
                StartCoroutine(m_JumpBob.DoBobCycle());
                PlayLandingSound();
                m_MoveDir.y = 0f;
                m_Jumping = false;
            }
            if (!m_CharacterController.isGrounded && !m_Jumping && m_PreviouslyGrounded)
            {
                m_MoveDir.y = 0f;
            }

            m_PreviouslyGrounded = m_CharacterController.isGrounded;
        }



        public void DisableInput() // 플레이어 움직임 제거
        {
            GetComponent<PlayerController>().enabled = false;
            Camera_Rt.instance.Close_Camera();
        }
        public void AbleInput()
        {
            Camera_Rt.instance.Open_Camera();
            Open_PlayerController();
        }

        private void PlayLandingSound()
        {
            m_AudioSource.clip = m_LandSound;
            m_AudioSource.Play();
            m_NextStep = m_StepCycle + .5f;
        }
        public void OpenAnim()
        {
            anim.enabled = true;
        }

        private void FixedUpdate()
        {
            float speed;
            GetInput(out speed);
            // always move along the camera forward as it is the direction that it being aimed at
            Vector3 desiredMove = transform.forward * m_Input.y + transform.right * m_Input.x;

            // get a normal for the surface that is being touched to move along it
            RaycastHit hitInfo;
            Physics.SphereCast(transform.position, m_CharacterController.radius, Vector3.down, out hitInfo,
                               m_CharacterController.height / 2f);
            desiredMove = Vector3.ProjectOnPlane(desiredMove, hitInfo.normal).normalized;


            m_MoveDir.x = desiredMove.x * speed;
            m_MoveDir.z = desiredMove.z * speed;


            if (m_CharacterController.isGrounded)
            {
                m_MoveDir.y = -m_StickToGroundForce;

                if (m_Jump)
                {
                    m_MoveDir.y = m_JumpSpeed;
                    PlayJumpSound();
                    m_Jump = false;
                    m_Jumping = true;
                }
            }
            else
            {
                m_MoveDir += Physics.gravity * m_GravityMultiplier * Time.fixedDeltaTime;
            }

            if (m_CharacterController.enabled)
            {
                m_CollisionFlags = m_CharacterController.Move(m_MoveDir * Time.fixedDeltaTime);
            }


            ProgressStepCycle(speed);
            UpdateCameraPosition(speed);
        }


        private void PlayJumpSound()
        {
            m_AudioSource.clip = m_JumpSound;
            m_AudioSource.Play();
        }


        private void ProgressStepCycle(float speed)
        {
            if (m_CharacterController.velocity.sqrMagnitude > 0 && (m_Input.x != 0 || m_Input.y != 0))
            {
                m_StepCycle += (m_CharacterController.velocity.magnitude + (speed * 0.03f * (m_IsWalking ? 1f : m_RunstepLenghten))) *
                             Time.fixedDeltaTime;
            }

            if (!(m_StepCycle > m_NextStep))
            {
                return;
            }

            m_NextStep = m_StepCycle + m_StepInterval;

            PlayFootStepAudio();
        }


        private void PlayFootStepAudio()
        {
            if (!m_CharacterController.enabled || !m_CharacterController.isGrounded)
            {
                return;
            }
            // pick & play a random footstep sound from the array,
            // excluding sound at index 0
            int n = Random.Range(1, m_FootstepSounds.Length);
            m_AudioSource.clip = m_FootstepSounds[n];
            m_AudioSource.PlayOneShot(m_AudioSource.clip);
            // move picked sound to index 0 so it's not picked next time
            m_FootstepSounds[n] = m_FootstepSounds[0];
            m_FootstepSounds[0] = m_AudioSource.clip;
        }


        private void UpdateCameraPosition(float speed)
        {
            Vector3 newCameraPosition;
            if (!m_UseHeadBob)
            {
                return;
            }
            if (m_CharacterController.velocity.magnitude > 0 && m_CharacterController.isGrounded)
            {
                // 이동 시 y 축 위아래 흔들림
                //m_Camera.transform.localPosition =
                //    m_HeadBob.DoHeadBob(m_CharacterController.velocity.magnitude +
                //                      (speed*(m_IsWalking ? 1f : m_RunstepLenghten)));
                newCameraPosition = m_Camera.transform.localPosition;
                //newCameraPosition.y = m_Camera.transform.localPosition.y - m_JumpBob.Offset();
            }
            else
            {
                newCameraPosition = m_Camera.transform.localPosition;
                newCameraPosition.y = m_OriginalCameraPosition.y - m_JumpBob.Offset();
            }
            m_Camera.transform.localPosition = newCameraPosition;
        }


        private void GetInput(out float speed)
        {
            // Read input
            float horizontal = CrossPlatformInputManager.GetAxisRaw("Horizontal");
            float vertical = CrossPlatformInputManager.GetAxisRaw("Vertical");

            bool waswalking = m_IsWalking;

#if !MOBILE_INPUT
            // On standalone builds, walk/run speed is modified by a key press.
            // keep track of whether or not the character is walking or running
            m_IsWalking = !Input.GetKey(KeyCode.LeftShift);
#endif
            bool backRun = Input.GetKey(KeyCode.S);
            bool rightRun = Input.GetKey(KeyCode.A);
            bool leftRun = Input.GetKey(KeyCode.D);
            bool goRun = Input.GetKey(KeyCode.W);
            if (!m_IsWalking && backRun || rightRun || leftRun)
            {
                if (!goRun)
                {
                    m_IsWalking = true;
                }
            }
            // set the desired speed to be walking or running
            speed = m_IsWalking ? m_WalkSpeed : m_RunSpeed;
            m_Input = new Vector2(horizontal, vertical);

            // normalize input if it exceeds 1 in combined length:
            if (m_Input.sqrMagnitude > 1)
            {
                m_Input.Normalize();
            }

            // handle speed change to give an fov kick
            // only if the player is going to a run, is running and the fovkick is to be used
            if (m_IsWalking != waswalking && m_UseFovKick && m_CharacterController.velocity.sqrMagnitude > 0)
            {
                StopAllCoroutines();
                StartCoroutine(!m_IsWalking ? m_FovKick.FOVKickUp() : m_FovKick.FOVKickDown());
            }
        }

        private void RotateView()
        {
            m_MouseLook.LookRotation(transform, m_Camera.transform);
        }


        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            Rigidbody body = hit.collider.attachedRigidbody;
            //dont move the rigidbody if the character is on top of it
            if (m_CollisionFlags == CollisionFlags.Below)
            {
                return;
            }

            if (body == null || body.isKinematic)
            {
                return;
            }
            body.AddForceAtPosition(m_CharacterController.velocity * 0.1f, hit.point, ForceMode.Impulse);
        }
        public void Open_PlayerController()
        {
            m_CharacterController.enabled = true;

            if (agent == null)
                return;

            agent.enabled = false;
        }
        public void Close_PlayerController()
        {
            m_CharacterController.enabled = false;
        }


        public Camera GetPlayerCamera()
        {
            return m_Camera;
        }
        public void GoNavposition(Transform nav)
        {
            Close_PlayerController();

            if (agent == null)
                return;

            agent.enabled = true;
            agent.SetDestination(nav.position);
        }

        public void StopAutoMove()
        {
            if (agent == null)
                return;

            agent.enabled = false;
            agent.ResetPath();
        }

        public void LookTarget(Transform target)
        {
            StartCoroutine(SmoothLookAt(playerTransform, target, 0.25f));

            // 플레이어 카메라 각도 변경
            StartCoroutine(SmoothRotateTo(GetPlayerCamera().transform, Vector3.zero, 0.25f));
        }

        public IEnumerator SmoothLookAt(Transform me, Transform target, float duration)
        {
            Vector3 direction = target.position - me.position;
            direction.y = 0f; // 수직 방향 제외 (고개 꺾이는 것 방지)

            Quaternion startRotation = me.rotation;
            Quaternion targetRotation = Quaternion.LookRotation(direction.normalized);
            Vector3 r = targetRotation.eulerAngles;

            string meName = me.gameObject.name;
            string targetName = target.gameObject.name;

            if (meName.Contains("Michael") || targetName.Contains("Michael"))
                r += Vector3.down * 5f;
            else if (meName.Contains("Alex") || targetName.Contains("Alex"))
                r += Vector3.up * 2.5f;

            targetRotation = Quaternion.Euler(r);

            float time = 0f;
            while (time < duration)
            {
                float t = Mathf.Clamp01(time / duration);
                me.rotation = Quaternion.Slerp(startRotation, targetRotation, t);
                time += Time.deltaTime;
                yield return null;
            }

            me.rotation = targetRotation; // 마지막 각도 보정
        }

        public IEnumerator SmoothRotateTo(Transform targetTransform, Vector3 targetEulerAngles, float duration)
        {
            Quaternion startRotation = targetTransform.localRotation;
            Quaternion targetRotation = Quaternion.Euler(targetEulerAngles);

            float elapsed = 0f;
            while (elapsed < duration)
            {
                float t = elapsed / duration;
                targetTransform.localRotation = Quaternion.Slerp(startRotation, targetRotation, t);
                elapsed += Time.deltaTime;
                yield return null;
            }

            targetTransform.localRotation = targetRotation; // 마지막 각도 보정
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Trigger"))
            {
                if (other.gameObject.name.Contains("Ethan Locker Trigger") && !ProgressManager.Instance.IsActionCompleted(ProgressManager.ActionType.FirstMeetEthan))
                {
                    SchoolUIManager schoolUIManager = CommonUIManager.instance.uiManager as SchoolUIManager;
                    AudioSource ehtanLockerAudio = schoolUIManager.activeObjs[7].GetComponent<AudioSource>();
                    ehtanLockerAudio.enabled = true;
                    ehtanLockerAudio.Play();
                }
                else if (other.gameObject.name == "EnemyFirstMeet Wall")
                {
                    SchoolUIManager schoolUIManager = CommonUIManager.instance.uiManager as SchoolUIManager;

                    Close_PlayerController();
                    schoolUIManager.FirstMeetEnemy();
                }
                else if (other.gameObject.name == "Lounge Trigger Wall")
                {
                    SchoolUIManager schoolUIManager = CommonUIManager.instance.uiManager as SchoolUIManager;

                    schoolUIManager.enterLounge = true;
                }
                else if (other.gameObject.name == "Portal Room Trigger Wall (Lounge Door)")
                {
                    SchoolUIManager schoolUIManager = CommonUIManager.instance.uiManager as SchoolUIManager;

                    schoolUIManager.EnterPortalRoom();
                }
                else if(other.gameObject.name.Contains("Backroom Trigger"))
                {
                    SchoolUIManager schoolUIManager = CommonUIManager.instance.uiManager as SchoolUIManager;

                    schoolUIManager.EnterBackroom();
                }
                else if (other.gameObject.name.Contains("Backroom Clear Trigger Wall"))
                {
                    SchoolUIManager schoolUIManager = CommonUIManager.instance.uiManager as SchoolUIManager;

                    schoolUIManager.OutOfBackroom();
                }
                else if (other.gameObject.name.Contains("Ethah House Trigger Wall"))
                {
                    SchoolUIManager schoolUIManager = CommonUIManager.instance.uiManager as SchoolUIManager;

                    schoolUIManager.EnterEthanHouse();
                }
                else if (other.gameObject.name.Contains("Start Last Chase Trigger"))
                {
                    SchoolUIManager schoolUIManager = CommonUIManager.instance.uiManager as SchoolUIManager;

                    schoolUIManager.StartFinalChase();
                }
                else if (other.gameObject.name.Contains("Last TimeLine Tirrger"))
                {
                    SchoolUIManager schoolUIManager = CommonUIManager.instance.uiManager as SchoolUIManager;

                    schoolUIManager.StartLastTimeLine();
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Trigger"))
            {
                if (other.gameObject.name == "Lounge Trigger Wall")
                {
                    SchoolUIManager schoolUIManager = CommonUIManager.instance.uiManager as SchoolUIManager;

                    schoolUIManager.enterLounge = false;
                }
                else if (other.gameObject.name.Contains("Ethan Locker Trigger") && !ProgressManager.Instance.IsActionCompleted(ProgressManager.ActionType.FirstMeetEthan))
                {
                    SchoolUIManager schoolUIManager = CommonUIManager.instance.uiManager as SchoolUIManager;

                    schoolUIManager.activeObjs[7].GetComponent<AudioSource>().Stop();
                }
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            Debug.Log("Hit : " + collision.gameObject.name);
        }

        public void AutoMove(Transform destination)
        {
            agent.enabled = true;
            Close_PlayerController();
            agent.SetDestination(destination.position);
        }
    }
}
