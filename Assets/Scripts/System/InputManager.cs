using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    [SerializeField] private IInputHandler inputHandler;
    [SerializeField] private InputAction primaryFireAction;
    [SerializeField] private InputAction secondaryFireAction;
    [SerializeField] private InputAction aimingAction;

    void OnEnable()
    {
        primaryFireAction.Enable();
        secondaryFireAction.Enable();
        aimingAction.Enable();

        primaryFireAction.performed += OnPrimartyFire;
        primaryFireAction.canceled += OnPrimaryFireEnd;

        secondaryFireAction.performed += OnSecondaryFire;
        secondaryFireAction.canceled += OnSecondaryFireEnd;

        aimingAction.performed += OnAiming;
    }

    void OnDisable()
    {
        primaryFireAction.performed -= OnPrimartyFire;
        primaryFireAction.canceled -= OnPrimaryFireEnd;

        secondaryFireAction.performed -= OnSecondaryFire;
        secondaryFireAction.canceled -= OnSecondaryFireEnd; 

        aimingAction.performed -= OnAiming;

        primaryFireAction.Disable();
        secondaryFireAction.Disable();
    }

    void OnPrimartyFire(InputAction.CallbackContext context)
    {
        inputHandler.PrimaryFire();
    }

    void OnPrimaryFireEnd(InputAction.CallbackContext context)
    {
        inputHandler.PrimaryFireEnd();
    }

    void OnSecondaryFire(InputAction.CallbackContext context)
    {
        inputHandler.SecondaryFire();
    }

    void OnSecondaryFireEnd(InputAction.CallbackContext context)
    {
        inputHandler.SecondaryFireEnd();
    }

    void OnAiming(InputAction.CallbackContext context)
    {
        inputHandler.Aiming(context.ReadValue<Vector2>());
    }
}