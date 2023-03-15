using UnityEngine;
using UnityEngine.UI;


[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
public class MovingPlayer : MonoBehaviour
{
    private CharacterController controller;
    private Animator animator;

    [Header("GameObjectComponents")]
    public Camera cameraPlayer;
    public GameObject player;
    public Slider staminaBar;

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

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        Cursor.visible = false;
        fovStart = fovInit = 70;

        staminaBar.maxValue = maxStamina;
        staminaBar.minValue = minStamina;
        staminaBar.value = maxStamina;
        stamina = maxStamina;
    }
    void Update()
    {
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
        Movement();
        Rotate();
    }
    private void Movement()
    {
        float moveVer = Input.GetAxis("Vertical");
        float moveHor = Input.GetAxis("Horizontal");
        Vector3 moveDirection = new Vector3(moveHor, -GravityMoment(), moveVer);
        moveDirection = transform.TransformDirection(moveDirection);
        bool sprint = Input.GetKey(KeyCode.LeftShift);
        if (sprint && !isLock && !isBlock && (moveVer != 0 || moveHor !=0))
        {
            staminaBar.gameObject.SetActive(true);
            stamina -= 20 * Time.deltaTime;
            speedCurrent = Mathf.Lerp(speedCurrent, runSpeedKoef * moveSpeed, Time.deltaTime * smoothSpeed);
            fovInit = Mathf.Lerp(fovInit, fovStart * 1.5f, Time.deltaTime * smoothSpeed);
            if (stamina <= minStamina+5) isBlock = true;
        }

        else if (!isLock && stamina <= 100)
        {
            if (stamina >= minStamina + 30) isBlock = false;
            staminaBar.gameObject.SetActive(false); 
            speedCurrent = moveSpeed;
            stamina += 7.5f * Time.deltaTime;
            fovInit = Mathf.Lerp(fovInit, fovStart, Time.deltaTime * smoothSpeed * 0.5f);
        }
        staminaBar.value = stamina;
        cameraPlayer.fieldOfView = fovInit;
        controller.Move(moveDirection * Time.deltaTime * speedCurrent);
        if ((moveVer != 0 || moveHor != 0) && !sprint) animator.SetInteger("Walk", 1);
        else if ((moveVer != 0 || moveHor != 0) && sprint) animator.SetInteger("Walk", 2);
        else animator.SetInteger("Walk", 0);
        
    }
    private void Rotate()
    {
        if (!isLock)
        {
            rotateHor += Input.GetAxis("Mouse X") * rotateSpeed;
            rotateVer += Input.GetAxis("Mouse Y") * rotateSpeed;
            rotateVer = Mathf.Clamp(rotateVer, -50, 50);
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
}
