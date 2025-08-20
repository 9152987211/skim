using UnityEngine;
using System.Collections.Generic;
using FishNet.Object;
using FishNet.Connection;
using FishNet.Managing;

public class Player : NetworkBehaviour
{
    public static Player instance;

    [SerializeField] private GameObject playerModel;
    [SerializeField] private Transform orientation;
    [SerializeField] private Transform stoneHolder;
    [SerializeField] private TMPro.TextMeshProUGUI nameBadge;

    [Header("CAMERAS")]
    [SerializeField] private Camera cameraRef;
    [SerializeField] private Camera UICameraRef;
    [SerializeField] private Camera worldUICameraRef;


    [Header("SCRIPTS")]
    [SerializeField] private PlayerLook playerLook;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private PickingUpStones pickingUpStones;
    [SerializeField] private StoneHandling stoneHandling;

    private bool isEnabled;

    private Animator animator;
    private float animationTransitionSmoothness = 10.0f;
    private Vector2 currentMovementVector = Vector2.zero;

    private float defaultFOV;
    private float FOVChangeSpeed = 10.0f;
    private float zoomAmount = 50.0f;
    private float zoomOffset = 0.0f;
    private float primeOffset = 0.0f;

    private int best = -1;

    private bool clientConnected = false;


    //SERVER VARIABLES
    private float facingDirection = 0.0f;
    private Vector2 movementInput = Vector2.zero;
    private float holdingStoneAnimationLayerWeight = 0.0f;
    private float holdingStoneAnimationSpeed = 10.0f;
    private float threwStoneAt = -999.0f;
    private float throwStoneTime = 0.3f;
    private bool isHoldingStone = false;
    private bool isPrimingStone = false;
    private string displayName = "";



    public override void OnStartClient()
    {
        base.OnStartClient();
        clientConnected = true;
        if (base.IsOwner)
        {
            instance = this;
            SetEnabled(true);
            defaultFOV = cameraRef.fieldOfView;
            UI.instance.SetUICamera(UICameraRef);

            playerLook.enabled = true;
            playerMovement.enabled = true;
            pickingUpStones.enabled = true;
            stoneHandling.enabled = true;

            cameraRef.gameObject.SetActive(true);

            playerModel.SetActive(false);
            UpdateDisplayNameS(PlayerPrefs.GetString("DisplayName", "?"));

            UI.instance.CloseMainMenu();
        }
        else
        {
            GetComponent<Rigidbody>().isKinematic = true;
            playerModel.SetActive(true);
            animator = playerModel.GetComponent<Animator>();

            //THE 4 SCRIPTS START DISABLED BY DEFAULT SO DON'T NEED TO DISABLE THEM HERE
            //THE CAMERA IS ALSO DISABLED BY DEFAULT
        }
    }

    private void Update()
    {
        if (!clientConnected) return;

        if (!IsOwner)
        {
            NonOwnerHandling();
            return;
        }

        if (Input.GetKeyDown(KeyCode.Mouse1)) ZoomIn();
        if (Input.GetKeyUp(KeyCode.Mouse1)) ZoomOut();

        cameraRef.fieldOfView = Mathf.Lerp(cameraRef.fieldOfView, defaultFOV + primeOffset + zoomOffset, FOVChangeSpeed * Time.deltaTime);
        worldUICameraRef.fieldOfView = cameraRef.fieldOfView;

        UpdatePlayerModelS(orientation.localEulerAngles.y, movementInput, isHoldingStone, isPrimingStone);
    }

    public void SetEnabled(bool b)
    {
        isEnabled = b;
        Cursor.lockState = b ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !b;
    }

    public bool IsEnabled()
    {
        return isEnabled;
    }

    public void PickedUpStone()
    {
        isHoldingStone = true;
    }

    public bool IsHoldingStone()
    {
        return isHoldingStone;
    }

    public void LetGoOfStone()
    {
        isHoldingStone = false;
    }

    public Transform GetCameraTransform()
    {
        return cameraRef.transform;
    }

    public void UpdatePrimeZoom(float offset)
    {
        primeOffset = offset;
    }

    private void ZoomIn()
    {
        zoomOffset = -zoomAmount;
    }

    private void ZoomOut()
    {
        zoomOffset = 0.0f;
    }

    public void NewScore(int value)
    {
        GameUI.instance.SetLastValue(value);

        if (value > best)
        {
            best = value;
            GameUI.instance.SetBestValue(best);
        }
    }

    public void UpdateMovementVector(float x, float y)
    {
        movementInput = new Vector2(x, y);
    }

    public void UpdateStonePrimingStatus(bool newStatus)
    {
        isPrimingStone = newStatus;
    }

    public Vector3 GetStoneHolderPosition()
    {
        return stoneHolder.position;
    }

    public GameObject[] GetListOfAllStones()
    {
        GameObject[] stones = GameObject.FindGameObjectsWithTag("Stone");
        return stones;
    }

    public int GetOwnerID()
    {
        return base.OwnerId;
    }

    private void UpdateNameBadge(string name)
    {
        nameBadge.text = name;
    }


    private void NonOwnerHandling()
    {
        playerModel.transform.localRotation = Quaternion.Euler(0.0f, facingDirection, 0.0f);

        currentMovementVector = Vector2.Lerp(currentMovementVector, movementInput, animationTransitionSmoothness * Time.deltaTime);
        animator.SetFloat("x", currentMovementVector.x);
        animator.SetFloat("y", currentMovementVector.y);

        if (Time.time >= threwStoneAt + throwStoneTime)
        {
            holdingStoneAnimationLayerWeight = Mathf.Lerp(holdingStoneAnimationLayerWeight, isHoldingStone ? 1.0f : 0.0f, holdingStoneAnimationSpeed * Time.deltaTime);
            animator.SetLayerWeight(2, holdingStoneAnimationLayerWeight);
        }
        animator.SetBool("Primed", isPrimingStone);
    }



    [ServerRpc]
    public void UpdatePlayerModelS(float direction, Vector2 movement, bool holdingStone, bool primingStone)
    {
        UpdatePlayerModelO(direction, movement, holdingStone, primingStone);
    }

    [ObserversRpc(ExcludeOwner = true)]
    public void UpdatePlayerModelO(float direction, Vector2 movement, bool holdingStone, bool primingStone)
    {
        facingDirection = direction;
        movementInput = movement;
        isHoldingStone = holdingStone;
        isPrimingStone = primingStone;
    }

    [ServerRpc]
    public void ThrowStoneS()
    {
        ThrowStoneO();
    }
    
    [ObserversRpc(ExcludeOwner = true)]
    public void ThrowStoneO()
    {
        animator.SetTrigger("Throw");
        threwStoneAt = Time.time;
    }


    [ServerRpc]
    public void SpawnStoneS(GameObject obj, Player owner)
    {
        GameObject stone = Instantiate(obj);
        ServerManager.Spawn(stone, base.Owner);
        SpawnStoneO(stone, owner);
    }

    [ObserversRpc]
    public void SpawnStoneO(GameObject spawnedObj, Player owner)
    {
        spawnedObj.GetComponent<Stone>().SetOwner(owner);
        
        if (owner == this)
        {
            StoneHandling.instance.AssignStone(spawnedObj);
        }
    }


    [ServerRpc]
    public void UpdateDisplayNameS(string name)
    {
        displayName = name;
        UpdateDisplayNameO(name);
    }

    [ObserversRpc]
    public void UpdateDisplayNameO(string name)
    {
        UpdateNameBadge(name);
        Player.instance.UpdateDisplayName2S();
    }

    [ServerRpc]
    public void UpdateDisplayName2S()
    {
        UpdateDisplayName2O(displayName);
    }

    [ObserversRpc]
    public void UpdateDisplayName2O(string name)
    {
        UpdateNameBadge(name);
    }
}
