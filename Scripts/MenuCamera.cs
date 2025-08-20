using UnityEngine;

public class MenuCamera : MonoBehaviour
{
    public static MenuCamera instance;

    private void Awake()
    {
        instance = this;
    }
}
