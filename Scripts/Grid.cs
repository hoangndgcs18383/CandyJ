using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using Unity.VisualScripting;

public enum CandyType
{
    Default,
    Red,
    Blue,
    Yellow,
    Black,
    White,
    Key,
    Special
}

[System.Serializable]
public struct CandySprite
{
    public CandyType type;
    public Sprite sprite;
}

[System.Serializable]
public struct Candies
{
    public int X;
    public int Y;
}

public class Grid : MonoBehaviour
{
    [SerializeField] private float _timeFill = 0.5f;
    [SerializeField] private float _speedFill = 2f;
    [SerializeField] private int Height = 5;
    [SerializeField] private int Width = 5;
    [SerializeField] private int offset;
    [SerializeField] private int offsetY = 8;
    [SerializeField] private Slot cell;
    [SerializeField] private Candy candy;
    [SerializeField] private Transform cellTF;
    [SerializeField] private Transform candyTF;
    [SerializeField] private Candy enteredCandy;
    [SerializeField] private Candy pressedCandy;

    [HideInInspector] private int X;
    [HideInInspector] private int Y;
    [HideInInspector] private List<Candy> listCandy;
    [HideInInspector] private List<Slot> cellList;


    private Candy[,] candies;
    private bool gameOver;
    private int total;
    private float fillTime;
    private int counter;
    private int index = 0;
    
    private void Awake()
    {
        X = Width;
        Y = Height;
        candies = new Candy[25, 25];

        GenerationGrid();
        StartCoroutine(GenerationCandy());    
    }

    private float timer = 0f;
    private void FixedUpdate()
    {
        if (listCandy != null) 
            
            if (counter >= total) return;
        fillTime += Time.fixedDeltaTime * _speedFill; 
        if (!(fillTime >= _timeFill)) return;
        
        listCandy?[counter].gameObject.SetActive(true);
        //listCandy[counter].Index = counter;
        fillTime = 0f;
        counter++;
    }
    
    public Vector2 Fill(Vector2 starPos, Vector2 endPos, float t)
    {
        Vector2 result = Vector2.Lerp(starPos, endPos, t);
        return result;
    }

    public void MovePath()
    {
        
    }

    //generation grid
    void GenerationGrid()
    {
        //store total cell
        total = X * Y;
        cellList = new List<Slot>(total);
        //loop though total

        for (int i = 0; i < total; i++)
        {
            var (_x, _y) = GetFromID(i, X);
            Slot item = Instantiate(cell, cellTF);
            item.name = $"Cell x:{_x}, y:{_y} => id:{SetID(_x, _y,X)}";
            item.transform.position = new Vector3(_x * offset, _y *  -offset + offsetY, 0f);
        }
    }
    
    public IEnumerator GenerationCandy()
    {
        var x = 0;
        var y = Y;
        var endFill = 0;
        
        bool revertY = false;
        bool revertX = true;
        listCandy = new List<Candy>(total);
        //listPath = new List<Vector2>();
        GameManager.Instance.listPath.Add(new Vector2(x * offset, y * -offset + offsetY));
        
        for (int i = 0; i < Width + Height; i++)
        {
            //up
            if(i % 2 == 0 && !revertY){
                for (int j = Y; j > 0; j--)
                {
                    y--;
                    SpawnCandy(x, y);
                }
                Y--;
                revertX = !revertX;
            }
            //right
            else if(i % 2 != 0 && !revertX)
            {
                for (int j = 0; j < Y; ++j)
                {
                    x++;
                    SpawnCandy(x, y);
                }
                X--;
                revertY = !revertY;
            }
            //button
            else if(i % 2 == 0 && revertY){
                for (int j = Y; j > 0; j--)
                {
                    y++;
                    SpawnCandy(x, y);
                }
                Y--;
                revertX = !revertX;
            }
            //left
            else if(i % 2 != 0 && revertX)
            {
                for (int j = 0; j < Y; ++j)
                {
                    x--;
                    SpawnCandy(x, y);
                }
                X--;
                endFill++;
                Y = y - endFill;
                revertY = !revertY;
            }
        }

        yield return new WaitForSeconds(1f);
    }

    public Candy SpawnCandy(int x, int y)
    {
        Candy item = Instantiate(candy, candyTF);
        item.ID = SetID(x, y, Width);
        item.name = $"Candy x:{x * offset}, y:{y * -offset + offsetY} id:{item.ID}";
        item.Index = index;
        GameManager.Instance.listPath.Add(new Vector2(x * offset, y * -offset + offsetY));
        var result = new Vector2(x * offset, y * -offset);
        item.transform.position = new Vector3(0, -10);
        CheckRule(item);
        //item.CandyType.Color = (CandyType)Random.Range(0, 8);
        item.gameObject.SetActive(false);
        listCandy.Add(item);
        index++;
        candies[x * offset, y * -offset + offsetY] = item;
        item.Init(this);
        return candies[x * offset, y * -offset + offsetY];
            
    }

    public void CheckRule(Candy candy)
    {
        candy.CandyType.Color = candy.index switch
        {
            0 => CandyType.Special,
            24 => CandyType.Key,
            _ => (CandyType)Random.Range(0, 5)
        };
    }
    
    int SetID(int x, int y, int width)
    {
        return y * width + x;
    }

    
    public (int, int) GetFromID(int id, int width)
    {
        return (id % width, (int)Mathf.Floor(id / width));
    }
    
    public void OnEnterCandy(Candy candy)
    {
        enteredCandy = candy;
    }

    public void OnPressedCandy(Candy candy)
    {
        pressedCandy = candy;
    }

    public void OnReleaseCandy()
    {
        if (IsAdjacent(pressedCandy, enteredCandy))
        {
            SwapBlock(pressedCandy, enteredCandy);
            //pressedCandy.Dissolving();
            //enteredCandy.Dissolving();
        }
        //Debug.Log($"{IsAdjacent(enteredCandy, pressedCandy)}");
    }
    
    public void SwapBlock(Candy candy_1,Candy candy_2)
    {
        
    }

    public bool IsAdjacent(Candy candy_1, Candy candy_2)
    {
        return (candy_1.X == candy_2.X && (int)Mathf.Abs(candy_1.Y - candy_2.Y) == offset) ||
               (candy_1.Y == candy_2.Y && (int)Mathf.Abs(candy_1.X - candy_2.X) == offset);
    }

    [System.Serializable]
    public struct Score
    {
        public CandyType scoreType;
        public List<int> scoreValue;
    }

    public Score[] scores;

    public Dictionary<CandyType, List<int>> ScoreDir;
    
    
    [Button]
    public void ClearAllValid()
    {
        List<Candy> listMatch = new List<Candy>();
        
        for (int i = 0; i < Height; i++)
        {
            for (int j = 0; j < Width; j++)
            {
                Debug.Log($"{i * offset} || {j * -offset + offsetY}");
                
                
                //GetMatch(listCandy[i + j], listCandy[i].x, listCandy[j].y);
            }
        }
    }

    public List<Candy> GetMatch(Candy candy, int x, int y)
    {
        List<Candy> xCandy = new List<Candy>();
        List<Candy> yCandy = new List<Candy>();
        List<Candy> matchCandy = new List<Candy>();
        
        xCandy.Add(candy);
        Debug.Log($"{candy} || {x} || {y}");

        if (x < 0 || x >= Height * offset)
        {
            return null;
        }
        
        if (xCandy.Count >= 3)
        {
            for (int i = 0; i < xCandy.Count; i++)
            {
                matchCandy.Add(xCandy[i]);
            }
        }
        
        if (matchCandy.Count >= 3)
        {
            return matchCandy;
        }
        
        xCandy.Clear();
        yCandy.Clear();

        return null;
    }


    private int CalculateScore()
    {
        var sumScore = 0;
        
        return sumScore;
    }

    public void GameOver()
    {
        gameOver = true;
    }
    
    private bool IsHitWall(Vector2 nextCell)
    {
        return nextCell.x > 0 || nextCell.y < Width * offset - 1;
    }
    
}
