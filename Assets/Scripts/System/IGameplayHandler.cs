using UnityEngine;

public interface IGameplayHandler 
{
    public void PrimaryFirePressed();
    public void PrimaryFire();
    public void PrimaryFireRelease();

    public void SecondaryFirePressed();
    public void SecondaryFire();
    public void SecondaryFireRelease();

    public void Aiming(Vector2 input);

    public void OnWeaponChange(bool up);
}
