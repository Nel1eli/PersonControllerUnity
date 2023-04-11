using System.Net.Http.Headers;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
public class MovingPlayer : MonoBehaviour
{
    public AudioClip[] steps;
    public AudioSource m_Source;

    private CharacterController controller;
    private Animator animator;

    [Header("Звуки шагов по полу")]
    [SerializeField] private AudioClip[] stepsTerrain;

    [Header("GameObjectComponents")]
    public Camera cameraPlayer;
    public GameObject player;

    [Header("Move Components")]
    [SerializeField] private float moveSpeed = 3;
    [SerializeField] private float runSpeedKoef = 1.5f;
    [SerializeField] private float smoothSpeed = 4; // Плавный переход из движения в бег
    private float speedCurrent;
    [SerializeField] private float rotateSpeed = 20;
    private float rotateHor, currentVelocityHor, rotateHorCurrent;
    private float rotateVer, currentVelocityVer, rotateVerCurrent;

    [Header("Stamina Player`s")]
    [SerializeField] private float minStamina = 0;
    [SerializeField] private float maxStamina = 100;
    [Range(60, 80)]
    [SerializeField] private float fovStart;
    [Range(0, 10)]
    private float stamina;


    [Header("Gravity feature")]
    [Range(50,70)]
    [SerializeField] private float gravityForce = 20;
    private float gravityCurrent;


    [Header("Components")]
    [Range(0, 1)]
    [SerializeField] private float smoothTime = 0.2f; // Плавность вращения
    private float fovInit;
    private bool isLock; // Проверка паузы
    private bool isBlock; // Отдышка

    private float moveVer;
    private float moveHor;
    private bool sprint;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        fovStart = fovInit = 70;

        stamina = maxStamina;
    }
    void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, -transform.up, out hit, 2))
        {
            if (hit.collider.tag == "SurfaceTerrain")
            {
                steps = stepsTerrain;
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isLock = !isLock;
            if (isLock)
            {
                Time.timeScale = 0;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Time.timeScale = 1;
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
        if (controller.isGrounded)
        {
            moveVer = Input.GetAxis("Vertical");
            moveHor = Input.GetAxis("Horizontal");
            sprint = Input.GetKey(KeyCode.LeftShift);
        }
        if (!controller.isGrounded) Movement();
        else if ((moveVer != 0 || moveHor != 0) && !sprint && controller.isGrounded)
        {
            animator.SetInteger("Walk", 1);
            Movement();
        }
        else if ((moveVer != 0 || moveHor != 0) && sprint && controller.isGrounded)
        {
            animator.SetInteger("Walk", 2);
            Movement();
        }

        else if (controller.isGrounded) animator.SetInteger("Walk", 0);

        Rotate();

    }
    private void Movement()
    {
        Vector3 moveDirection = new Vector3(moveHor, -GravityMoment(), moveVer);
        moveDirection = transform.TransformDirection(moveDirection);
        controller.Move(moveDirection * Time.deltaTime * speedCurrent);
        if (sprint && !isLock && !isBlock)
        {
            stamina -= 20 * Time.deltaTime;
            speedCurrent = Mathf.Lerp(speedCurrent, runSpeedKoef * moveSpeed, Time.deltaTime * smoothSpeed);
            fovInit = Mathf.Lerp(fovInit, fovStart - fovStart * 0.15f, Time.deltaTime * smoothSpeed);
            if (stamina <= minStamina+5) isBlock = true;
        }

        else if (!isLock && stamina <= 100)
        {
            if (stamina >= minStamina + 30) isBlock = false;
            speedCurrent = moveSpeed;
            stamina += 7.5f * Time.deltaTime;
            fovInit = Mathf.Lerp(fovInit, fovStart, Time.deltaTime * smoothSpeed * 0.5f);
        }
        cameraPlayer.fieldOfView = fovInit;
        
        if (!m_Source.isPlaying && controller.isGrounded) PlayFootStepAudio();
    }
    private void Rotate()
    {
        if (!isLock && !InteractPlayer.statusRead)
        {
            rotateHor += Input.GetAxis("Mouse X") * rotateSpeed;
            rotateVer += Input.GetAxis("Mouse Y") * rotateSpeed;
            rotateVer = Mathf.Clamp(rotateVer, -60, 60);
            rotateHorCurrent = Mathf.SmoothDamp(rotateHor, rotateHorCurrent, ref currentVelocityHor, smoothTime);
            rotateVerCurrent = Mathf.SmoothDamp(rotateVer, rotateVerCurrent, ref currentVelocityVer, smoothTime);
            cameraPlayer.transform.rotation = Quaternion.Euler(-rotateVerCurrent, rotateHorCurrent, 0);
            player.transform.rotation = Quaternion.Euler(0, rotateHor, 0);
        }
    }
    private float GravityMoment()
    {
        if (!controller.isGrounded) gravityCurrent = gravityForce * 10 * Time.deltaTime;
        else gravityCurrent = 0;
        return gravityCurrent;
    }

    public void PlayFootStepAudio()
    {
        if (!controller.isGrounded)
        {
            return;
        }
        // pick & play a random footstep sound from the array,
        // excluding sound at index 0
        int n = Random.Range(0, steps.Length-1);
        m_Source.clip = steps[n];
        m_Source.PlayOneShot(m_Source.clip);
        // move picked sound to index 0 so it's not picked next time
        steps[n] = steps[0];
        steps[0] = m_Source.clip;
    }

}
