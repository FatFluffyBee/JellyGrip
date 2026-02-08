using UnityEngine;

public interface IMoveReceiver
{
   public void AddMovementSource(IMoveGiver giver);
   public void RemoveMovementSource(IMoveGiver giver);
}
