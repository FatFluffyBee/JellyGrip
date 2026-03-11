using System.Collections.Generic;
using UnityEngine;

public class MultiTentacleController : MonoBehaviour, IGameplayHandler
{
    [SerializeField] private IMoveReceiver moveReceiver;
    [SerializeField] private HealthSystem hs;

    [Header("Tentacles")]
    [SerializeField] private Transform launchPos;
    [SerializeField] private List<GameObject> tentaclePrefabs;
    [SerializeField] private int nMaxTentacles;

    [Header("Selection Visuals")]
    [SerializeField] private List<Sprite> tentacleHeadsSprite;
    [SerializeField] private SpriteRenderer headSpriteSelection;
    [SerializeField] private GameObject dirSelectionGO;
    
    [Header("Force Retract")]
    [SerializeField] public SelectionMode retractSelectionMode;
    [SerializeField] public bool retractTentacleIfLimitReached;

    [Header("Dangerous Surface")]
    [SerializeField] private bool damagePlayerOnDangerousSurfaceTentacleTouch= false;


    private bool primaryFireStarted = false;
    //private bool blockRetractationUntilRelease = false;

    private List<Tentacle> tentacles = new List<Tentacle>();
    private int tentacleIndex = 0;
    private List<MoveInput> moveInputs = new List<MoveInput>();

    private AimData lastAimInput;
    private Vector2 aimDirFromBody;
    private Tentacle selectedTentacle; 

    private Camera mainCam;
    public enum SelectionMode {Queue, Stack, Nearest};

    private void OnEnable()
    {
        hs.OnHit += RetractAllTentacles;
    }

    private void OnDisable()
    {
        hs.OnHit -= RetractAllTentacles;
    }

    private void Awake()
    {
        mainCam = Camera.main;
        moveReceiver = GetComponentInParent<IMoveReceiver>();
    }

    void Start()
    {
        headSpriteSelection.sprite = tentacleHeadsSprite[0];
    }

    void Update()
    {
        if(tentacles.Count > 0)
        {
            SetTentaclesTargetDir(lastAimInput);
            SetSelectedTentacle();
        }
        SetAimVisualFeedback();
    }

    public void PrimaryFirePressed()
    {
        if(primaryFireStarted)
            return;

        primaryFireStarted = true;

        if(tentacles.Count >= nMaxTentacles)
        {
            if(!retractTentacleIfLimitReached)
                return;

            MakeSpaceForNewTentacle();
        }

        SpawnTentacle();
    }

    private void MakeSpaceForNewTentacle()
    {
        Tentacle tentacleToRetract = GetTentacleSelection(SelectionMode.Queue, tentacles, aimDirFromBody);
        tentacleToRetract.ForceRetract();
    }

    private void SpawnTentacle()
    {
        Tentacle tentacle = Instantiate(tentaclePrefabs[tentacleIndex], launchPos.position, Quaternion.identity).GetComponent<Tentacle>();
        tentacle.InitializeTentacle(launchPos, aimDirFromBody, moveReceiver);
        tentacle.OnForceRetract += DisconnectTentacle;
        tentacle.OnTouchingDanger += DamagePlayer;
        tentacles.Add(tentacle);
    }

    public void PrimaryFire()
    {
        if(tentacles.Count > 0)
        {
            tentacles[^1].TryExpand();
        }  
    }

    public void PrimaryFireRelease()
    {
        primaryFireStarted = false;
    }

    public void SecondaryFirePressed()
    {
        if(tentacles.Count > 0)
        {
            Tentacle tentacleToRetract = GetTentacleSelection(retractSelectionMode, tentacles, aimDirFromBody);
            tentacleToRetract.ForceRetract();
        }
    }

    public void SecondaryFire()
    {

    }

    public void SecondaryFireRelease()
    {
       
    }

    public void Aiming(AimData aimData)
    {
        lastAimInput = aimData;
        CalculateAimDirFromBody(lastAimInput);
        SetAimingVisualDir(aimDirFromBody);
    }

    public void SetAimingVisualDir(Vector2 aimDir)
    {
        dirSelectionGO.transform.up = -aimDir;
    }

    public void SetSelectedTentacle()
    {
        Tentacle newSelection = GetTentacleSelection(SelectionMode.Nearest, tentacles, aimDirFromBody);
        
        if(newSelection == selectedTentacle || newSelection == null)
            return;

        if(selectedTentacle != null)
        {
            selectedTentacle.OnDeselected();
        }
        
        newSelection.OnSelected();
        selectedTentacle = newSelection;
    }

    public void OnWeaponChange(bool up)
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

    public void ChangeTentacleIndex(int index)
    {
        if(index < 0)
        {
           index = tentacleHeadsSprite.Count - 1;
        }
        else if(index >= tentacleHeadsSprite.Count)
        {
            index = 0;
        }

        tentacleIndex = index;
        headSpriteSelection.sprite = tentacleHeadsSprite[tentacleIndex];
    }

    public void RetractAllTentacles()
    {
        for(int i = tentacles.Count - 1; i >= 0; i--)
        {
            Tentacle e = tentacles[i];
            if(e != null)
                e.ForceRetract();
        }
    }

    public void DisconnectTentacle(Tentacle toRemoveTentacle)
    {
        if(tentacles.Contains(toRemoveTentacle))
        {
            tentacles.Remove(toRemoveTentacle);
        }
        toRemoveTentacle.OnForceRetract -= DisconnectTentacle;
    }

    //Aim direction 
    private void CalculateAimDirFromBody(AimData aimData)
    {
        if(aimData.aimMode == AimMode.Direction)
        {
            if(Vector2.SqrMagnitude(aimData.value) > 0.001f)
            {
                aimDirFromBody = aimData.value.normalized;
            }   
        }
        else 
        {
            Vector2 mouseWorldPos = mainCam.ScreenToWorldPoint(aimData.value);
            Vector2 aimDir = mouseWorldPos - (Vector2)transform.position;
            if(Vector2.SqrMagnitude(aimDir) > 0.001f)
            {
                aimDirFromBody = aimDir.normalized;
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

    private void SetAimVisualFeedback()
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

    //Tentacle Selection
    private Tentacle GetTentacleSelection(SelectionMode selectionMode, List<Tentacle> tentacles, Vector2 aimDir)
    {
        if(tentacles.Count == 0)
            return null;

        return selectionMode switch
        {
            SelectionMode.Queue => tentacles[0],
            SelectionMode.Stack => tentacles[tentacles.Count - 1],
            SelectionMode.Nearest => GetNearestTentacle(tentacles, aimDir),
            _ => null
        };
    }

    private Tentacle GetNearestTentacle(List<Tentacle> tentacles, Vector2 selectDir)
    {
        if(tentacles.Count == 0)
        {
            Debug.LogError("List is empty this shouldn't fire");
            return null;
        }

        if(tentacles.Count == 1)
        {
            return tentacles[0];
        }
            
        int index = 0;
        float dot = -1f;

        for(int i = 0; i < tentacles.Count; i++)
        {
            Vector2 dirBodyToHead = (tentacles[i].HeadPos - (Vector2)transform.position).normalized;
            float newDot = Vector2.Dot(selectDir, dirBodyToHead);

            if(newDot > dot)
            {
                dot = newDot;
                index = i;
            }
        }
        
        return tentacles[index];
    }

    private void DamagePlayer()
    {
        if(damagePlayerOnDangerousSurfaceTentacleTouch)
        {
            hs.TakeDamage(1);
        }
    }
}

