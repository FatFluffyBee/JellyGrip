using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class TentacleManager : MonoBehaviour, IMoveGiver, IGameplayHandler
{
    [SerializeField] private Transform launchPos;
    [SerializeField] private List<GameObject> tentaclePrefabs;
    [SerializeField] private GameObject dirSelectionGizmos;
    [SerializeField] private SpriteRenderer headSpriteSelection;

    [SerializeField] private List<Sprite> tentacleHeads;

    private Tentacle currentTentacle;
    private int tentacleIndex = 0;
    private List<MoveInput> moveInputs = new List<MoveInput>();
    private bool primaryFireStarted = false;
    private bool blockRetractationUntilRelease = false;
 
    void Start()
    {
        headSpriteSelection.sprite = tentacleHeads[0];
    }

    public void PrimaryFirePressed()
    {
        if(!primaryFireStarted && currentTentacle == null)
        {
            currentTentacle = Instantiate(tentaclePrefabs[tentacleIndex], launchPos.position, Quaternion.identity).GetComponent<Tentacle>();
            currentTentacle.InitializeTentacle(this, launchPos);
        }
        primaryFireStarted = true;
    }

    public void PrimaryFire()
    {
        if(currentTentacle != null)
        {
            currentTentacle.TryExpand();
        }  
    }

    public void PrimaryFireRelease()
    {
        primaryFireStarted = false;
    }

    public void SecondaryFirePressed()
    {
        
    }

    public void SecondaryFire()
    {
        if(blockRetractationUntilRelease)
        {
            return;
        }

        if(currentTentacle != null)
        {
            currentTentacle.TryRetract();
        }
        else
        {
            blockRetractationUntilRelease = true;
        }
    }

    public void SecondaryFireRelease()
    {
        blockRetractationUntilRelease = false;
    }

    public void Aiming(Vector2 direction)
    {
        
    }

    public void OnWeaponChange(bool up)
    {
        if(currentTentacle == null)
        {
            if(up)
            {
                ChangeTentacleIndex(tentacleIndex + 1);
            } 
            else
            {
                ChangeTentacleIndex(tentacleIndex - 1);
            } 
        } 
        else
        {
            RetractAllTentacles();
        }
    }

    public void ChangeTentacleIndex(int index)
    {
        if(index < 0)
        {
           index = tentacleHeads.Count - 1;
        }
        else if(index >= tentacleHeads.Count)
        {
            index = 0;
        }

        tentacleIndex = index;
        headSpriteSelection.sprite = tentacleHeads[tentacleIndex];
    }

    /*void Update()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        
        if(currentTentacle == null)
        {
            dirSelectionGizmos.SetActive(true);
        }
        else
        {
            dirSelectionGizmos.SetActive(false);
        }

        //! how to pass the direction without a ref to input or manager? I can update visual and target ig
        Vector3 dir = mousePos - transform.position;
        dir.ToV2Dir();
        dirSelectionGizmos.transform.up = -dir;
    }*/

    public List<MoveInput> GetDesiredMovement()
    {
        moveInputs.Clear();
        if(currentTentacle != null)
        {
            moveInputs.AddRange(currentTentacle.GetDesiredMovement());
        }
        
        return moveInputs;
    }

    public void RetractAllTentacles()
    {
        if(currentTentacle != null)        
        {
            currentTentacle.ForceRetract();
        }
    }

    public void DisconnectTentacle(Tentacle tentacle)
    {
        Debug.Log("Disconnecting Tentacle");
        if(currentTentacle == tentacle)
        {
            currentTentacle = null;
        }
        tentacle.OnForceRetract -= DisconnectTentacle;
    }
}
