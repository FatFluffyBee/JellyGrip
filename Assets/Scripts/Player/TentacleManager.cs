using System.Collections.Generic;
using UnityEngine;

public class TentacleManager : MonoBehaviour, IMoveGiver, IGameplayHandler
{
    [SerializeField] private Transform launchPos;
    [SerializeField] private List<GameObject> tentaclePrefabs;
    [SerializeField] private GameObject dirSelectionGO;
    [SerializeField] private SpriteRenderer headSpriteSelection;
    [SerializeField] private List<Sprite> tentacleHeads;

    private Tentacle currentTentacle;
    private int tentacleIndex = 0;
    
    private bool primaryFireStarted = false;
    private bool blockRetractationUntilRelease = false;
    private List<MoveInput> moveInputs = new List<MoveInput>();

    private Camera mainCam;
    private AimData lastAimInput;
    private Vector2 aimDirFromHead;
    
    private void Awake()
    {
        mainCam = Camera.main;
    }

    void Start()
    {
        headSpriteSelection.sprite = tentacleHeads[0];
    }

    void Update()
    {
        if(currentTentacle != null)
        {
            SetTentacleTargetDir(lastAimInput);
        }
        SetAimVisual();
    }

    public void PrimaryFirePressed()
    {
        if(!primaryFireStarted && currentTentacle == null)
        {
            currentTentacle = Instantiate(tentaclePrefabs[tentacleIndex], launchPos.position, Quaternion.identity).GetComponent<Tentacle>();
            currentTentacle.InitializeTentacle(launchPos, aimDirFromHead);
            currentTentacle.OnForceRetract += DisconnectTentacle;
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

    public void Aiming(AimData aimData)
    {
        lastAimInput = aimData;
        CalculateDirFromHead(lastAimInput);
        SetAimingVisualDir(aimDirFromHead);
    }

    public void SetAimingVisualDir(Vector2 aimDir)
    {
        dirSelectionGO.transform.up = -aimDir;
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

    private void CalculateDirFromHead(AimData aimData)
    {
        if(aimData.aimMode == AimMode.Direction)
        {
            if(Vector2.SqrMagnitude(aimData.value) > 0.001f)
            {
                aimDirFromHead = aimData.value.normalized;
            }   
        }
        else 
        {
            Vector2 mouseWorldPos = mainCam.ScreenToWorldPoint(aimData.value);
            Vector2 aimDir = mouseWorldPos - (Vector2)transform.position;
            if(Vector2.SqrMagnitude(aimDir) > 0.001f)
            {
                aimDirFromHead = aimDir.normalized;
            }  
        }
    }

    private void SetTentacleTargetDir(AimData aimData)
    {
        if(aimData.aimMode == AimMode.Direction)
        {   
            currentTentacle.SetTargetDir(aimData.value);
        }
        else 
        {
            Vector2 mouseWorldPos = mainCam.ScreenToWorldPoint(aimData.value);
            currentTentacle.SetTargetDirFromPos(mouseWorldPos);
        }
    }

    private void SetAimVisual()
    {
        if(currentTentacle != null)
        {
            dirSelectionGO.SetActive(false);
        }
        else 
        {
            dirSelectionGO.SetActive(true);
        }
    }
}

