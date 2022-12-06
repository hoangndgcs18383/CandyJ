using UnityEngine;


public class Colorable : MonoBehaviour
{
    public SpriteRenderer sprRenderer;
    private CandyType type;
    
    public CandyType Color
    {
        get { return type; }
        set { SetType(value); }
    }

    public void SetType(CandyType newColor)
    {
        type = newColor;

        if (GameManager.Instance.candyDict.ContainsKey(newColor))
        {
            sprRenderer.sprite = GameManager.Instance.candyDict[newColor];
            Debug.Log(type);
        }
    }
    
}
