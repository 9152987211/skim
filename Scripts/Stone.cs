using UnityEngine;
using FishNet.Object;

public class Stone : NetworkBehaviour
{
    [SerializeField] private GameObject bounceNumber;
    [SerializeField] private float velocityDampening = 0.85f;
    [SerializeField] private float flatness = 1.0f;

    private Player owner;

    private Rigidbody rb;
    private MeshCollider meshCollider;
    private int bounces = 0;

    private float bounceForce = 5.0f;
    private float bounceDampening = 0.75f;
    private float minimumBounceForce = 0.2f;

    private bool isBeingHeld = true;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        meshCollider = GetComponent<MeshCollider>();
    }

    private void LateUpdate()
    {
        if (Player.instance != owner || !isBeingHeld) return;

        transform.position = owner.GetStoneHolderPosition();
    }

    public void HitWater()
    {
        if (Player.instance != owner) return; //ONLY RUN THE FOLLOWING CODE ON THE CLIENT THAT OWNS THE STONE

        if (isBeingHeld) return; //PREVENTS WEIRD WATER INTERACTIONS BEFORE YOU HAVE THROWN THE STONE

        float horizontalSpeed = (new Vector3(rb.linearVelocity.x, 0.0f, rb.linearVelocity.z)).magnitude;
        float impactAngle = Vector3.Angle(rb.linearVelocity, Vector3.up) - 90.0f;

        if (!ShouldBounce(impactAngle, horizontalSpeed))
        {
            SplashS(transform.position, bounces);

            meshCollider.enabled = false;
            Destroy(gameObject, 3.0f);
            Water.instance.Splash(transform.position);
            SpawnBounceNumber(bounces);
            Player.instance.NewScore(bounces);
            return;
        }

        BounceS(transform.position);

        bounces++;
        Water.instance.Ripple(transform.position);

        //print("Angle: " + impactAngle.ToString() + ", Speed: " + horizontalSpeed.ToString());

        rb.linearVelocity = new Vector3(rb.linearVelocity.x * velocityDampening, 0.0f, rb.linearVelocity.z * velocityDampening);
        bounceForce *= bounceDampening;
        bounceForce = Mathf.Max(bounceForce, minimumBounceForce);
        rb.AddForce(Vector3.up * bounceForce, ForceMode.Impulse);
    }

    private bool ShouldBounce(float impactAngle, float horizontalSpeed)
    {
        float angleFactor = Mathf.InverseLerp(20.0f, 2.0f, Mathf.Abs(impactAngle)); //1 = SHALLOW ANGLE, 0 = STEEP ANGLE
        float speedFactor = Mathf.InverseLerp(2.0f, 25.0f, horizontalSpeed); //1 = FAST, 0 = SLOW

        if (Random.Range(0.0f, 1.0f) > angleFactor) return false;
        if (Random.Range(0.0f, 1.0f) > speedFactor) return false;
        if (Random.Range(0.0f, 1.0f) > flatness) return false;

        return true;
    }

    private void SpawnBounceNumber(int value)
    {
        GameObject bn = Instantiate(bounceNumber);
        bn.transform.position = new Vector3(transform.position.x, -1.0f, transform.position.z);
        bn.GetComponent<BounceNumber>().SetTextValue(value);
    }

    public void SetOwner(Player p)
    {
        owner = p;

        if (Player.instance != owner)
        {
            GetComponent<MeshCollider>().enabled = false;
        } 
    }

    public void Throw()
    {
        isBeingHeld = false;
        meshCollider.enabled = true;
    }

    public Player GetOwner()
    {
        return owner;
    }

    public bool GetIsBeingHeld()
    {
        return isBeingHeld;
    }


    [ServerRpc]
    public void BounceS(Vector3 pos)
    {
        BounceO(pos);
    }

    [ObserversRpc(ExcludeOwner = true)]
    public void BounceO(Vector3 pos)
    {
        Water.instance.Ripple(pos);
    }


    [ServerRpc]
    public void SplashS(Vector3 pos, int numberOfBounces)
    {
        SplashO(pos, numberOfBounces);
    }

    [ObserversRpc(ExcludeOwner = true)]
    public void SplashO(Vector3 pos, int numberOfBounces)
    {
        Water.instance.Splash(pos);
        SpawnBounceNumber(numberOfBounces);
    }
}
