using UnityEngine;

public class Slot : MonoBehaviour
{
    public int ID;
    public SpriteRenderer bg;

    public bool IsThisID(int id1, int id2)
    {
        if (id1 == id2)
        {
            bg.color = new Color(1f, 1f, 1f, 1f);
            return true;
        }
        bg.color = new Color(0f, 0f, 0f, 1f);
        return false;
    }
}
