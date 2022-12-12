using UnityEngine;

public static class MatchUtils
{
    public  static int SetID(int x, int y, int width)
    {
        return y * width + x;
    }

    public static (int, int) GetFromID(int id, int width)
    {
        return (id % width, (int)Mathf.Floor(id / width));
    }

    public static int CheckLeft(int originX, int xOffset, int offset)
    {
        return originX - xOffset * offset;
    }
    
    public static bool IsAdjacent(Candy candy_1, Candy candy_2, int offset)
    {
        return (candy_1.X == candy_2.X && (int)Mathf.Abs(candy_1.Y - candy_2.Y) == offset) ||
               (candy_1.Y == candy_2.Y && (int)Mathf.Abs(candy_1.X - candy_2.X) == offset);
    }
}
