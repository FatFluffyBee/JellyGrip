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

    public event Action OnCancel;

    private Tentacle currentTentacle;
    private int tentacleIndex = 0;

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
                    Debug.Log("Next Tentacle");
                    tentacleIndex++;
                    if(tentacleIndex >=tentaclePrefabs.Count)
                    {
                        tentacleIndex = 0;
                    }
                    headSpriteSelection.sprite = tentacleHeads[tentacleIndex];
                } 
                else if(scroll < 0f)
                {
                    Debug.Log("Previous Tentacle");
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

    public MoveInput GetDesiredMovement()
    {
        if(currentTentacle != null)
        {
            return new MoveInput(currentTentacle.GetDesiredMovement(), MoveType.Velocity);
        }
        
        return new (Vector3.zero, MoveType.Velocity);
    }

    public void RetractAllTentacles()
    {
        OnCancel?.Invoke();
    }
}
