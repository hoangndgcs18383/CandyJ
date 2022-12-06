using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

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
    
    [Button]
    public void Test()
    {
        foreach (var item in candies)
        {
            Debug.Log(item);
        }
    }
    
    int SetID(int x, int y, int width)
    {
        return y * width + x;
    }

    
    public (int, int) GetFromID(int id, int width)
    {
        return (id % width, (int)Mathf.Floor(id / width));
    }
    
    public List<Vector2> GetSpiral(int x, int y)
    {
        List<Vector2> spiralPos = new List<Vector2>();
        
        spiralPos.Add(new Vector2(x, y * - offset));
        Debug.Log($"{x} : {y - offset}");
        
        return spiralPos;
    }
    
    public Vector2 GetWorldPosition(Candy candy)
    {
        return new Vector2(candy.x, candy.y);
    }

    public void OnExitCandy(Candy candy)
    {
        //enteredCandy.transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
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
        if (gameOver)
        {
            return;
        }
        
        candies[candy_1.X, candy_1.Y] = candy_2;
        candies[candy_2.X, candy_2.Y] = candy_1;
        
        Debug.Log(candies[candy_1.X, candy_1.Y]);
        Debug.Log(candies[candy_2.X, candy_2.Y]);

        if (GetMatch(candy_1, candy_2.X, candy_2.Y) != null || GetMatch(candy_2, candy_1.X, candy_1.Y) != null)
        {
            int block1X = candy_1.X;
            int block1Y = candy_1.Y;

            int block2X = candy_2.X;
            int block2Y = candy_2.Y;

            //candy_1.MoveableBlock.Move(candy_2.X, candy_2.Y, fillTime);
            //candy_2.MoveableBlock.Move(block1X, block1Y, fillTime);
            //ClearAllValidMatches();
            Debug.Log("Matching succesful !!");

            /*if(candy_1.BlockType == BlockType.ROW_CLEAR || candy_1.BlockType == BlockType.COLUMN_CLEAR)
            {
                ClearBlock(block1X, block1Y);
            }
            if (block_2.BlockType == BlockType.ROW_CLEAR || block_2.BlockType == BlockType.COLUMN_CLEAR)
            {
                ClearBlock(block2X, block2Y);
            }*/

            pressedCandy = null;
            enteredCandy = null;

            //StartCoroutine(Fill());
            //level.OnMove();
        }
        else
        {
            Debug.Log("Can't not match !!");
            //blocks[block_1.X, block_1.Y] = block_1;
            //blocks[block_2.X, block_2.Y] = block_1;
        }  
    }
    
    public List<Candy> GetMatch(Candy candy, int newX, int newY)
    {
        CandyType color = candy.CandyType.Color;
        List<Candy> xBlocks = new List<Candy>();
        List<Candy> yBlocks = new List<Candy>();
        List<Candy> matchBlocks = new List<Candy>();

        //check x
        xBlocks.Add(candy);
        Debug.Log($"{newX} || {newY}");
        
        
        for (int dir = 0; dir <= offset; dir++)
        {
            for (int xOffset = offset; xOffset < Width; xOffset++)
            {
                int x;

                if (dir == 0)
                {
                    //Left
                    x = newX - xOffset;
                }
                else
                {
                    //Right
                    x = newX + xOffset;
                }
                //check block match is out of ranged
                if (x < 0 || x >= Width)
                {
                    break;
                }
                
                
                //check block is the same color become matching.
                if (candies[x, newY].CandyType.Color == color)
                {
                    xBlocks.Add(candies[x, newY]);
                }
                else // or return
                {
                    break;
                }
            }
        }
        if (xBlocks.Count >= 3)
        {
            for (int i = 0; i < xBlocks.Count; i++)
            {
                matchBlocks.Add(xBlocks[i]);
            }
        }

        //Traverse verticalll and horizontally if we found a match
        if (xBlocks.Count >= 3)
        {
            for (int i = 0; i < xBlocks.Count; i++)
            {
                for (int dir = 0; dir <= offset; dir++)
                {
                    for (int yOffset = offset; yOffset < Height; yOffset++)
                    {
                        int y;

                        if (dir == 0)
                        {
                            //check UP
                            y = newY - yOffset;
                        }
                        else
                        {
                            //check DOWM
                            y = newY + yOffset;
                        }

                        if (y < 0 || y >= Height)
                        {
                            break;
                        }
                        //check this adjacent block is colored and this block is matching. 
                        if (candies[xBlocks[i].X, y].CandyType.Color == color)
                        { 
                            yBlocks.Add(candies[xBlocks[i].X, y]);
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                if (yBlocks.Count < 2)
                {
                    yBlocks.Clear();
                }
                else
                {
                    for (int j = 0; j < yBlocks.Count; j++)
                    {
                        matchBlocks.Add(yBlocks[j]);
                    }
                    break;
                }
            }
        }
        if (matchBlocks.Count >= 3)
        {
            return matchBlocks;
        }
        //Didn't find anything going x;
        xBlocks.Clear();
        yBlocks.Clear();
        //check vertical
        yBlocks.Add(candy);

        for (int dir = 0; dir <= offset; dir++)
        {
            for (int yOffset = offset; yOffset < Height; yOffset++)
            {
                int y;

                if (dir == 0)
                {
                    //Left
                    y = newY - yOffset;
                }
                else
                {
                    //Right
                    y = newY + yOffset;
                }
                //check block match is out of ranged
                if (y < 0 || y >= Height)
                {
                    break;
                }
                //check block is the same color become matching.
                if (candies[newX, y].CandyType.Color == color)
                {
                    yBlocks.Add(candies[newX, y]);
                }
                else // or return
                {
                    break;
                }
            }
        }
        if (yBlocks.Count >= 3)
        {
            for (int i = 0; i < yBlocks.Count; i++)
            {
                matchBlocks.Add(yBlocks[i]);
            }
        }

        //Traverse verticalll and horizontally if we found a match
        if (yBlocks.Count >= 3)
        {
            for (int i = 0; i < yBlocks.Count; i++)
            {
                for (int dir = 0; dir <= offset; dir++)
                {
                    for (int xOffset = offset; xOffset < Width; xOffset++)
                    {
                        int x;

                        if (dir == 0)
                        {
                            //check Left
                            x = newX - xOffset;
                        }
                        else
                        {
                            //check Right
                            x = newX + xOffset;
                        }

                        if (x < 0 || x >= Width)
                        {
                            break;
                        }
                        //check this adjacent block is colored and this block is matching. 
                        if (candies[x, yBlocks[i].Y].CandyType.Color == color)
                        {
                            xBlocks.Add(candies[x, yBlocks[i].Y]);
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                if (xBlocks.Count < 2)
                {
                    xBlocks.Clear();
                }
                else
                {
                    for (int j = 0; j < xBlocks.Count; j++)
                    {
                        matchBlocks.Add(xBlocks[j]);
                    }
                    break;
                }
            }
        }
        if (matchBlocks.Count >= 3)
        {
            return matchBlocks;
        }
            
        return null;
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

    public void ClearAllValid(Candy candy)
    {
        CandyType c = candy.CandyType.Color;
        
        CalculateScore();
        //Calculate score
        //score > 0
        // => scoring, destroy
        //
        var curPos = new Vector2(0, 0);
        var dir = new Vector2(1, 0);
        var i = 0;
        var collecting = new List<Candy>();

        int addCandy(List<int> candies, int idCandy)
        {
            return idCandy;
        }
        
        while (i < total)
        {
            //move
            curPos += dir;

            foreach (var item in listCandy)
            {
                if (c == item.CandyType.Color)
                {
                    //gan => vao list
                    collecting.Add(candy);
                }
            }

            /*if (idCandy == currentCandyid || collecting.Count == 0)
            {
                currentCandyid = addCandy(collecting, idCandy);
            }*/

            /*if (IsHitWall(curPos))
            {
                curPos = new Vector2(0, curPos.y + 1);
            }*/
            i++;
        }
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

    /*public Vector2 DirUp = new Vector2(0, -1);
    public Vector2 DirDown = new Vector2(0, 1);
    public Vector2 DirLeft = new Vector2(-1, 0);
    public Vector2 DirRight = new Vector2(1, 0);*/
    public List<Vector2> BoardCell = new List<Vector2>();

    [Button]
    public void Testt()
    {
        var i = 0;
        var currentDir = BoardCell[i];
        var currentPos = new Vector2(0, Height - 1);
        var nextCell = currentPos + currentDir;
        var cellCollectedCell = 0;
        
        while (cellCollectedCell < total)
        {
            //collectCell(currentPos);
            
            Debug.Log(IsHitWall(nextCell));
            
            if (IsHitWall(nextCell)) //|| isCollectedCell(nextCell))
            {
                i = (i + 1) % BoardCell.Count;
                currentDir = BoardCell[i];
                nextCell = currentPos + currentDir;
            }

            cellCollectedCell++;
            currentPos = nextCell;
            
            Debug.Log($"{nextCell}");
        }
    }

    private bool IsHitWall(Vector2 nextCell)
    {
        return nextCell.y > 0 || nextCell.y < Width * offset - 1;
    }
    
}
