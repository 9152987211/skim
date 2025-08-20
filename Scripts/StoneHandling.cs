using UnityEngine;

public class StoneHandling : MonoBehaviour
{
    public static StoneHandling instance;

    [SerializeField] private Transform stoneHolder;
    [SerializeField] private AnimationCurve powerCurve;

    private bool stonePrimed = false;
    private Vector3 startingPosition;
    private Vector3 stonePrimedOffset = new Vector3(0.0f, 0.0f, -0.3f);
    private float primeSpeed = 5.0f;
    private float zoomAmount = 10.0f;

    private float maxThrowForce = 90.0f;

    private float powerAmount = 0.0f;
    private float cycleDuration = 1.5f;
    private float primedAt;

    private GameObject heldStone;

    private void Start()
    {
        instance = this;
        startingPosition = stoneHolder.localPosition;
    }

    private void Update()
    {
        if (!Player.instance.IsHoldingStone()) return;


        //PRIME THROW
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            stonePrimed = true;
            Player.instance.UpdateStonePrimingStatus(stonePrimed);
            Player.instance.UpdatePrimeZoom(-zoomAmount);
            GameUI.instance.ShowPowerBar();
            powerAmount = 0.0f;
            GameUI.instance.UpdatePowerBar(powerAmount);
            primedAt = Time.time;
        }

        //RELEASE
        if (Input.GetKeyUp(KeyCode.Mouse0) && stonePrimed) ThrowStone();


        stoneHolder.localPosition = Vector3.Lerp(stoneHolder.localPosition, stonePrimed ? startingPosition + stonePrimedOffset : startingPosition, primeSpeed * Time.deltaTime);


        //POWER BAR
        if (stonePrimed)
        {
            float timeInCycle = ((Time.time-primedAt) % cycleDuration) / cycleDuration;
            powerAmount = powerCurve.Evaluate(timeInCycle);
            GameUI.instance.UpdatePowerBar(powerAmount);
        }
    }


    private void ThrowStone()
    {
        float forceAmount = maxThrowForce * powerAmount;

        stonePrimed = false;
        Player.instance.UpdateStonePrimingStatus(stonePrimed);
        Player.instance.ThrowStoneS(); //SERVER CALL

        heldStone.GetComponent<Rigidbody>().isKinematic = false;
        heldStone.GetComponent<Stone>().Throw();
        heldStone.GetComponent<Rigidbody>().AddForce(Player.instance.GetCameraTransform().forward * forceAmount, ForceMode.Impulse);
        
        Player.instance.LetGoOfStone();
        stoneHolder.localPosition = startingPosition;
        Player.instance.UpdatePrimeZoom(0.0f);
        GameUI.instance.HidePowerBar();
    }

    public bool IsPrimed()
    {
        return stonePrimed;
    }

    public void AssignStone(GameObject obj)
    {
        heldStone = obj;
    }
}
