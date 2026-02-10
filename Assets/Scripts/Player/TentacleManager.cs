using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class TentacleManager : MonoBehaviour, IMoveGiver
{
    [SerializeField] private Transform launchPos;
    [SerializeField] private List<GameObject> tentaclePrefabs;
    [SerializeField] private GameObject dirSelectionGizmos;
    [SerializeField] private SpriteRenderer headSpriteSelection;

    [SerializeField] private List<Sprite> tentacleHeads;

    private Tentacle currentTentacle;
    private int tentacleIndex = 0;
    private List<MoveInput> moveInputs = new List<MoveInput>();
 
    void Start()
    {
        headSpriteSelection.sprite = tentacleHeads[0];
    }

    void Update()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;

        if(Input.GetMouseButton(0))
        {
            if(currentTentacle == null)
            {
                currentTentacle = Instantiate(tentaclePrefabs[tentacleIndex], launchPos.position, Quaternion.identity).GetComponent<Tentacle>();
                currentTentacle.root = launchPos;
                currentTentacle.InitializeTentacle(this);
                currentTentacle.TryExpand();
            }
            else 
            {
                currentTentacle.TryExpand();
            }
        }

        if(Input.GetMouseButton(1) && currentTentacle != null)
        {
            currentTentacle.TryRetract();
        }

        float scroll = Mouse.current.scroll.y.ReadValue();
        
        if(scroll != 0)
        {
           if(currentTentacle == null)
            {
                if(scroll > 0)
                {
                    tentacleIndex++;
                    if(tentacleIndex >=tentaclePrefabs.Count)
                    {
                        tentacleIndex = 0;
                    }
                    headSpriteSelection.sprite = tentacleHeads[tentacleIndex];
                } 
                else if(scroll < 0f)
                {
                    tentacleIndex--;
                    if(tentacleIndex < 0)
                    {
                        tentacleIndex = tentaclePrefabs.Count - 1;
                    }
                    headSpriteSelection.sprite = tentacleHeads[tentacleIndex];
                } 
            } 
            else
            {
                RetractAllTentacles();
            }
        }
        

        if(currentTentacle == null)
        {
            dirSelectionGizmos.SetActive(true);
        }
        else
        {
            dirSelectionGizmos.SetActive(false);
        }

        Vector3 dir = mousePos - transform.position;
        dir.ToV2Dir();
        dirSelectionGizmos.transform.up = -dir;
    }

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
