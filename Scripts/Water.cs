using UnityEngine;

public class Water : MonoBehaviour
{
    public static Water instance;

    [SerializeField] private GameObject waterRippleEffect;
    [SerializeField] private GameObject waterSplashEffect;

    [SerializeField] private GameObject waterRippleSound;
    [SerializeField] private GameObject waterSplashSound;

    private void Awake()
    {
        instance = this;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Stone>() != null)
        {
            other.GetComponent<Stone>().HitWater();
        }
    }

    public void Ripple(Vector3 stonePosition)
    {
        GameObject ripple = Instantiate(waterRippleEffect);
        ripple.transform.position = new Vector3(stonePosition.x, -2.15f, stonePosition.z);

        GameObject rippleSound = Instantiate(waterRippleSound);
        rippleSound.transform.position = new Vector3(stonePosition.x, -2.15f, stonePosition.z);
        Destroy(rippleSound, 3.0f);
    }

    public void Splash(Vector3 stonePosition)
    {
        GameObject splash = Instantiate(waterSplashEffect);
        splash.transform.position = new Vector3(stonePosition.x, -2.15f, stonePosition.z);

        GameObject splashSound = Instantiate(waterSplashSound);
        splashSound.transform.position = new Vector3(stonePosition.x, -2.15f, stonePosition.z);
        Destroy(splashSound, 3.0f);
    }
}
