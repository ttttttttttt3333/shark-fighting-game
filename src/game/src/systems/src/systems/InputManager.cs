using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    public enum ControlScheme { PC, Console, Mobile }

    private ControlScheme currentScheme;
    private PlayerInput playerInput;
    private Vector2 movementInput;
    private bool isAttack1Pressed;
    private bool isAttack2Pressed;
    private bool isAttack3Pressed;
    private bool isAbilityPressed;
    private bool isSpecialPressed;
    private bool isPausePressed;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        playerInput = GetComponent<PlayerInput>();
        DetectControlScheme();
    }

    private void DetectControlScheme()
    {
        #if UNITY_STANDALONE
            currentScheme = ControlScheme.PC;
        #elif UNITY_SWITCH || UNITY_PS4 || UNITY_PS5 || UNITY_XBOXONE
            currentScheme = ControlScheme.Console;
        #elif UNITY_IOS || UNITY_ANDROID
            currentScheme = ControlScheme.Mobile;
        #else
            currentScheme = ControlScheme.PC;
        #endif

        Debug.Log($"Control Scheme: {currentScheme}");
    }

    private void Update()
    {
        HandleInput();
    }

    private void HandleInput()
    {
        switch (currentScheme)
        {
            case ControlScheme.PC:
                HandlePCInput();
                break;
            case ControlScheme.Console:
                HandleConsoleInput();
                break;
            case ControlScheme.Mobile:
                HandleMobileInput();
                break;
        }
    }

    private void HandlePCInput()
    {
        movementInput = Vector2.zero;
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) movementInput.y = 1;
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) movementInput.y = -1;
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) movementInput.x = -1;
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) movementInput.x = 1;

        isAttack1Pressed = Input.GetKeyDown(KeyCode.Alpha1);
        isAttack2Pressed = Input.GetKeyDown(KeyCode.Alpha2);
        isAttack3Pressed = Input.GetKeyDown(KeyCode.Alpha3);
        isAbilityPressed = Input.GetKeyDown(KeyCode.E);
        isSpecialPressed = Input.GetKeyDown(KeyCode.Space);
        isPausePressed = Input.GetKeyDown(KeyCode.Escape);

        OnInputProcessed();
    }

    private void HandleConsoleInput()
    {
        movementInput = Input.GetAxis("Horizontal") * Vector2.right + Input.GetAxis("Vertical") * Vector2.up;

        isAttack1Pressed = Input.GetButtonDown("Fire1");
        isAttack2Pressed = Input.GetButtonDown("Fire2");
        isAttack3Pressed = Input.GetButtonDown("Fire3");
        isAbilityPressed = Input.GetButtonDown("Jump");
        isSpecialPressed = Input.GetButtonDown("Interact");
        isPausePressed = Input.GetButtonDown("Cancel");

        OnInputProcessed();
    }

    private void HandleMobileInput()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            Vector2 touchPos = touch.position;

            if (touchPos.x < Screen.width * 0.3f)
            {
                movementInput = (touchPos - new Vector2(Screen.width * 0.15f, Screen.height * 0.5f)).normalized;
            }
            else if (touchPos.x > Screen.width * 0.7f)
            {
                if (touchPos.y > Screen.height * 0.66f)
                    isAttack1Pressed = true;
                else if (touchPos.y > Screen.height * 0.33f)
                    isAttack2Pressed = true;
                else
                    isAttack3Pressed = true;
            }
        }

        OnInputProcessed();
    }

    private void OnInputProcessed()
    {
        OnMovementInput?.Invoke(movementInput);
        
        if (isAttack1Pressed) OnAttack?.Invoke(0);
        if (isAttack2Pressed) OnAttack?.Invoke(1);
        if (isAttack3Pressed) OnAttack?.Invoke(2);
        if (isAbilityPressed) OnAbilityInput?.Invoke();
        if (isSpecialPressed) OnSpecialInput?.Invoke();
        if (isPausePressed) OnPauseInput?.Invoke();
    }

    public ControlScheme GetCurrentControlScheme() => currentScheme;

    public event System.Action<Vector2> OnMovementInput;
    public event System.Action<int> OnAttack;
    public event System.Action OnAbilityInput;
    public event System.Action OnSpecialInput;
    public event System.Action OnPauseInput;
}
