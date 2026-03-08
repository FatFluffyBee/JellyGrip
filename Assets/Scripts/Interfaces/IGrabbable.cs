using UnityEngine;

public interface IGrabbable 
{
    public void OnGrabStart(Transform grabber);
    public void OnGrabEnd(Vector3 grabberVelocity);
}
