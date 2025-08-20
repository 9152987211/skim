using UnityEngine;
using TMPro;

public class BounceNumber : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI bounceNumberText;
    [SerializeField] private float size = 0.05f;
    [SerializeField] private float floatSpeed = 5.0f;
    [SerializeField] private float fadeSpeed = 5.0f;

    private Color targetColour;

    private void Start()
    {
        targetColour = new Color(bounceNumberText.color.r, bounceNumberText.color.g, bounceNumberText.color.b, 0.0f);
    }

    public void SetTextValue(int value)
    {
        bounceNumberText.text = value.ToString();
    }

    private void Update()
    {
        //FLOAT UP
        transform.position += Vector3.up * floatSpeed * Time.deltaTime;

        //FADE OUT
        bounceNumberText.color = Color.Lerp(bounceNumberText.color, targetColour, fadeSpeed * Time.deltaTime);
        if (bounceNumberText.color.a <= 0.01f) Destroy(gameObject);

        //ALWAYS FACE PLAYER
        transform.LookAt(Player.instance.GetCameraTransform());
        transform.Rotate(0.0f, 180.0f, 0.0f);

        //SCALE SO IT ALWAYS APPEARS THE SAME SIZE ON THE SCREEN (REGARDLESS OF WORLD SPACE DISTANCE)
        transform.localScale = Vector3.Distance(Player.instance.GetCameraTransform().position, transform.position) * Vector3.one * size;
    }
}
