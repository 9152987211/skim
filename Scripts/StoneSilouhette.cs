using UnityEngine;
using UnityEngine.UI;

public class StoneSilouhette : MonoBehaviour
{
    [SerializeField] private Color colour;
    [SerializeField] private Image image;
    [SerializeField] private Sprite AOutline;
    [SerializeField] private Sprite AHighlighted;
    [SerializeField] private Sprite BOutline;
    [SerializeField] private Sprite BHighlighted;

    private bool isHidden = true;
    private float targetShowAlpha = 1.0f;
    private int stoneType;

    private void Start()
    {
        image.color = new Color(colour.r, colour.g, colour.b, 0.0f);
    }

    private void LateUpdate()
    {
        if (!isHidden) targetShowAlpha = 1.0f - Mathf.Clamp01(Vector3.Distance(PickingUpStones.instance.GetPlayerTarget(), transform.position) / PickingUpStones.instance.GetMaxRadius());
        image.color = new Color(colour.r, colour.g, colour.b, isHidden ? 0.0f : targetShowAlpha);
    }

    public void Show()
    {
        isHidden = false;
    }

    public void Hide()
    {
        isHidden = true;
    }

    public void Highlight()
    {
        image.sprite = GetHighlightSprite();
    }

    public void StopHighlight()
    {
        image.sprite = GetOutlineSprite();
    }

    public void SetStoneType(int t)
    {
        stoneType = t;
    }

    private Sprite GetHighlightSprite()
    {
        switch (stoneType)
        {
            case 0:
                return AHighlighted;
            case 1:
                return BHighlighted;
        }

        Debug.LogWarning("Unable to get valid highlighted sprite.");
        return AHighlighted;
    }

    private Sprite GetOutlineSprite()
    {
        switch (stoneType)
        {
            case 0:
                return AOutline;
            case 1:
                return BOutline;
        }

        Debug.LogWarning("Unable to get valid outline sprite.");
        return AOutline;
    }

    public int GetStoneType()
    {
        return stoneType;
    }
}
