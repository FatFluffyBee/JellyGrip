using UnityEngine;

public interface IInputHandler 
{
    public void PrimaryFire();
    public void PrimaryFireEnd();

    public void SecondaryFire();
    public void SecondaryFireEnd();

    public void Aiming(Vector2 input);
}
