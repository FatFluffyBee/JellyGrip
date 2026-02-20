using UnityEngine;
using UnityEngine.InputSystem;

//This architecture is a bit weird, it's cause I wanted to be able to swap handlers on the fly and I would need to unsuscribe
public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }
    
    [SerializeField] private TentacleManager sourceGameplayHandler; //original intent was that I could change manager on the fly without changinc code for testing
    private IGameplayHandler gameplayHandler; 

    [SerializeField] private InputActionReference primaryFireAction;
    [SerializeField] private InputActionReference secondaryFireAction;
    [SerializeField] private InputActionReference aimingAction;
    [SerializeField] private InputActionReference changeWeaponAction;

    private bool primaryHeld = false;
    private bool secondaryHeld = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        gameplayHandler = sourceGameplayHandler;
    }

    void Update()
    {
        if(primaryHeld)
        {
            gameplayHandler.PrimaryFire();
        }

        if(secondaryHeld)
        {
            gameplayHandler.SecondaryFire();
        }
    }

    void OnEnable()
    {
        primaryFireAction.action.Enable();
        secondaryFireAction.action.Enable();
        aimingAction.action.Enable();

        primaryFireAction.action.performed += OnPrimaryFirePressed;
        primaryFireAction.action.canceled += OnPrimaryFireRelease;

        secondaryFireAction.action.performed += OnSecondaryFirePressed;
        secondaryFireAction.action.canceled += OnSecondaryFireEnd;

        aimingAction.action.performed += OnAiming;

        changeWeaponAction.action.performed += OnWeaponChange;
    }

    void OnDisable()
    {
        primaryFireAction.action.performed -= OnPrimaryFirePressed;
        primaryFireAction.action.canceled -= OnPrimaryFireRelease;

        secondaryFireAction.action.performed -= OnSecondaryFirePressed;
        secondaryFireAction.action.canceled -= OnSecondaryFireEnd; 

        aimingAction.action.performed -= OnAiming;

        changeWeaponAction.action.performed -= OnWeaponChange;

        primaryFireAction.action.Disable();
        secondaryFireAction.action.Disable();
        aimingAction.action.Disable();
    }

    public void SwitchToUIInput()
    {
        
    }

    public void SwitchToGameplayInput()
    {
        
    }


    void OnPrimaryFirePressed(InputAction.CallbackContext context)
    {
        primaryHeld = true;
        gameplayHandler.PrimaryFirePressed();
    }

    void OnPrimaryFireRelease(InputAction.CallbackContext context)
    {
        primaryHeld = false;
        gameplayHandler.PrimaryFireRelease();
    }

    void OnSecondaryFirePressed(InputAction.CallbackContext context)
    {
        secondaryHeld = true;
        gameplayHandler.SecondaryFirePressed();
    }

    void OnSecondaryFireEnd(InputAction.CallbackContext context)
    {
        secondaryHeld = false;
        gameplayHandler.SecondaryFireRelease();
    }

    void OnWeaponChange(InputAction.CallbackContext context)
    {
        bool value = context.ReadValueAsButton();
        gameplayHandler.OnWeaponChange(value);
    }

    void OnAiming(InputAction.CallbackContext context)
    {
        Debug.Log(context.control.device is Gamepad);
        AimData aimData = new AimData
        {
            value = context.ReadValue<Vector2>(),
            aimMode = (context.control.device is Gamepad)? AimMode.Direction : AimMode.Position
        };
        gameplayHandler.Aiming(aimData);
    }
}

public enum AimMode {Direction, Position}

public struct AimData
{
    public Vector2 value;
    public AimMode aimMode;
}

