using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    public static PlayerLook instance;

    [SerializeField] private Transform orientation;

    private float sensitivity = 0.7f;
    private float xRotation;
    private float yRotation;

    private void Start()
    {
        instance = this;
    }

    private void Update()
    {
        //GET MOUSE INPUT
        float mouseX = Player.instance.IsEnabled() ? Input.GetAxisRaw("Mouse X") * sensitivity : 0.0f;
        float mouseY = Player.instance.IsEnabled() ? Input.GetAxisRaw("Mouse Y") * sensitivity : 0.0f;

        yRotation += mouseX;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90.0f, 90.0f);


        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0.0f);
        orientation.rotation = Quaternion.Euler(0.0f, yRotation, 0.0f);
    }
}
