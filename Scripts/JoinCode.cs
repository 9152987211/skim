using UnityEngine;

public class JoinCode : MonoBehaviour
{
    public static JoinCode instance;

    private string joinCode = "";

    private void Awake()
    {
        instance = this;
    }

    public string GetCode()
    {
        return joinCode;
    }

    public void SetCode(string code)
    {
        joinCode = code;
    }
}
