using System.Collections.Generic;
using UnityEngine;

public class MultiTentacleController : MonoBehaviour, IMoveGiver, IGameplayHandler
{
    [SerializeField] private Transform launchPos;
    [SerializeField] private List<GameObject> tentaclePrefabs;
    [SerializeField] private GameObject dirSelectionGO;
    [SerializeField] private SpriteRenderer headSpriteSelection;

    [SerializeField] private List<Sprite> tentacleHeads;
    [SerializeField] private int nMaxTentacles;

    private List<Tentacle> tentacles = new List<Tentacle>();
    private int tentacleIndex = 0;
    private List<MoveInput> moveInputs = new List<MoveInput>();
    private bool primaryFireStarted = false;
    private bool blockRetractationUntilRelease = false;

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
        Debug.Log("Tentacle Count :" + tentacles.Count);
        if(tentacles.Count > 0)
        {
            SetTentaclesTargetDir(lastAimInput);
        }
        SetAimVisual();
    }

    public void PrimaryFirePressed()
    {
        if(!primaryFireStarted)
        {
            if(tentacles.Count >= nMaxTentacles)
            {
                tentacles[0].ForceRetract();
            }

            Tentacle tentacle = Instantiate(tentaclePrefabs[tentacleIndex], launchPos.position, Quaternion.identity).GetComponent<Tentacle>();
            tentacle.InitializeTentacle(launchPos, aimDirFromHead);
            tentacle.OnForceRetract += DisconnectTentacle;
            tentacles.Add(tentacle);
        }
        primaryFireStarted = true;
    }

    public void PrimaryFire()
    {
       /* if(currentTentacle != null)
        {
            currentTentacle.TryExpand();
        }  */
    }

    public void PrimaryFireRelease()
    {
        primaryFireStarted = false;
    }

    public void SecondaryFirePressed()
    {
        if(tentacles.Count > 0)
        {
            tentacles[0].ForceRetract();
        }
    }

    public void SecondaryFire()
    {
       /* if(blockRetractationUntilRelease)
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
        }*/
    }

    public void SecondaryFireRelease()
    {
       // blockRetractationUntilRelease = false;
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
        /*if(currentTentacle == null)
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
        }*/
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
        if(tentacles.Count > 0)
        {
            foreach(Tentacle e in tentacles)
            {
                moveInputs.AddRange(e.GetDesiredMovement());
            }
        }
   
        return moveInputs;
    }

    public void RetractAllTentacles()
    {
        if(tentacles.Count > 0)
        {
            foreach(Tentacle e in tentacles)
            {
                e.ForceRetract();
            }
        }
    }

    public void DisconnectTentacle(Tentacle toRemoveTentacle)
    {
        Debug.Log("Disconnecting Tentacle");
        if(tentacles.Contains(toRemoveTentacle));
        {
            tentacles.Remove(toRemoveTentacle);
        }
        toRemoveTentacle.OnForceRetract -= DisconnectTentacle;
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

    private void SetTentaclesTargetDir(AimData aimData)
    {
        if(aimData.aimMode == AimMode.Direction)
        {   
            foreach(Tentacle currentTentacle in tentacles)
            {
                currentTentacle.SetTargetDir(aimData.value);
            }
        }
        else 
        {
            foreach(Tentacle currentTentacle in tentacles)
            {
                Vector2 mouseWorldPos = mainCam.ScreenToWorldPoint(aimData.value);
                currentTentacle.SetTargetDirFromPos(mouseWorldPos);
            }
        }
    }

    private void SetAimVisual()
    {
        if(tentacles.Count >= nMaxTentacles)
        {
            dirSelectionGO.SetActive(false);
        }
        else 
        {
            dirSelectionGO.SetActive(true);
        }
    }
}

