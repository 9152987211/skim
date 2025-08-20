using UnityEngine;
using System.Collections.Generic;

public class PickingUpStones : MonoBehaviour
{
    public static PickingUpStones instance;

    [SerializeField] private List<GameObject> stones;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private LayerMask silouhetteMask;
    [SerializeField] private float pickupRange = 3.0f;
    [SerializeField] private GameObject stoneSilouhette;
    [SerializeField] private int amount = 5;
    [SerializeField] private float stoneSpacing = 1.0f;
    [SerializeField] private float positionRandomness = 0.1f;
    [SerializeField] private float stoneCoverage = 0.6f;

    private List<List<GameObject>> silouhettes;
    private Vector3 playerTarget;

    private void Start()
    {
        instance = this;

        if (amount % 2 == 0) Debug.LogError("Amount of stone silouhettes must be an odd number.");

        silouhettes = new List<List<GameObject>>();
        for (int i = 0; i < amount; i++)
        {
            silouhettes.Add(new List<GameObject>());
            for (int j = 0; j < amount; j++)
            {
                silouhettes[i].Add(Instantiate(stoneSilouhette));
            }
        }
    }

    private void Update()
    {
        CheckGroundRaycast();
        CheckSilouhetteRaycast();
    }

    private void LateUpdate()
    {
        //I PUT THIS IN LATE UPDATE TO PREVENT THE STONE INSTANTLY BEING PRIMED AFTER PICKING IT UP (DUE TO THEM BOTH USING THE SAME BIND)
        if (Input.GetKeyDown(KeyCode.Mouse0) && !Player.instance.IsHoldingStone()) PickupStone();
    }

    private void CheckGroundRaycast()
    {
        RaycastHit hit;
        if (Physics.Raycast(Player.instance.GetCameraTransform().position, Player.instance.GetCameraTransform().forward, out hit, pickupRange, groundMask) && !Player.instance.IsHoldingStone())
        {
            playerTarget = hit.point;
            float startX = Mathf.Round(hit.point.x / stoneSpacing) * stoneSpacing;
            float startZ = Mathf.Round(hit.point.z / stoneSpacing) * stoneSpacing;
            for (int i = 0; i < amount; i++)
            {
                for (int j = 0; j < amount; j++)
                {
                    float x = startX + (float)(i - (amount - 1) / 2) * stoneSpacing;
                    float z = startZ + (float)(j - (amount - 1) / 2) * stoneSpacing;

                    float noise1 = Hash01(x * 0.45f, z * 0.78f);
                    noise1 = (noise1 * 2.0f) - 1.0f;
                    float noise2 = Hash01(x * 0.2f, z * 0.5f);
                    noise2 = (noise2 * 2.0f) - 1.0f;
                    silouhettes[i][j].transform.position = new Vector3(x + noise1 * positionRandomness, -0.19f, z + noise2 * positionRandomness);

                    float noise3 = Hash01(x * 0.6f, z * 0.15f);
                    noise3 = ((noise3 * 2.0f) - 1.0f) * 40.0f;
                    silouhettes[i][j].transform.eulerAngles = new Vector3(silouhettes[i][j].transform.eulerAngles.x, silouhettes[i][j].transform.eulerAngles.z, noise3);

                    silouhettes[i][j].GetComponent<StoneSilouhette>().SetStoneType(StoneType(x, z));

                    if (Hash01(x, z) < stoneCoverage)
                    {
                        silouhettes[i][j].GetComponent<StoneSilouhette>().Show();
                    }
                    else
                    {
                        silouhettes[i][j].GetComponent<StoneSilouhette>().Hide();
                    }
                }
            }
        }
        else
        {
            for (int i = 0; i < amount; i++)
            {
                for (int j = 0; j < amount; j++)
                {
                    silouhettes[i][j].GetComponent<StoneSilouhette>().Hide();
                }
            }
        }
    }

    private void CheckSilouhetteRaycast()
    {
        StoneSilouhette hitSilouhette = null;
        RaycastHit hit;
        if (Physics.Raycast(Player.instance.GetCameraTransform().position, Player.instance.GetCameraTransform().forward, out hit, pickupRange, silouhetteMask) && !Player.instance.IsHoldingStone())
        {
            hitSilouhette = hit.collider.gameObject.GetComponent<StoneSilouhette>();
            hitSilouhette.Highlight();
        }

        for (int i = 0; i < amount; i++)
        {
            for (int j = 0; j < amount; j++)
            {
                if (hitSilouhette != silouhettes[i][j].GetComponent<StoneSilouhette>()) silouhettes[i][j].GetComponent<StoneSilouhette>().StopHighlight();
            }
        }
    }

    private void PickupStone()
    {
        RaycastHit hit;
        if (Physics.Raycast(Player.instance.GetCameraTransform().position, Player.instance.GetCameraTransform().forward, out hit, pickupRange, silouhetteMask))
        {
            Player.instance.SpawnStoneS(stones[hit.collider.gameObject.GetComponent<StoneSilouhette>().GetStoneType()], Player.instance);
            Player.instance.PickedUpStone();
        }
    }


    public Vector3 GetPlayerTarget()
    {
        return playerTarget;
    }

    public float GetMaxRadius()
    {
        return amount * 0.5f * stoneSpacing;
    }


    private float  Hash(float x, float y)
    {
        int h = Mathf.RoundToInt(x * 73856093) ^ Mathf.RoundToInt(y * 19349663);
        return Mathf.Abs(h);
    }

    private float Hash01(float x, float y)
    {
        return Hash(x, y) % 10000 / 10000f;
    }

    private int StoneType(float x, float y)
    {
        float scale = 0.2f;
        float hashValue = Hash01(x * scale, y * scale);

        if (hashValue > 0.5f) return 0;
        else return 1;
    }
}
