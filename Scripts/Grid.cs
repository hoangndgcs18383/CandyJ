using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine.Events;

public enum CandyType
{
    Default,
    Red,
    Blue,
    Yellow,
    Black,
    White,
    Key,
    Special,
    EMPTY
}

//skill
    //1: Random 30% candies
    //2: Random 3 => 4 white
    //3: Random row or column destroy => 1 white


//advance after win => key == special => to stuck

[System.Serializable]
public struct CandySprite
{
    public CandyType type;
    public Sprite sprite;
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

    public Level level;
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
    
    public bool FillStep()
    {
        bool isMove = false;

        foreach (var candy in listCandy)
        {
            Candy check = candies[candy.x, candy.y];

            if (check.CandyType.Color == CandyType.EMPTY)
            {
                check.Fill(fillTime);
                isMove = true;
            }
        }

        return isMove;
    }

    private float timer = 0f;
    private bool isPlayerable;

    private void FixedUpdate()
    {
        if (listCandy != null)
            if (counter >= total) return;
        fillTime += Time.fixedDeltaTime * _speedFill;
        if (!(fillTime >= _timeFill)) return;
        
        listCandy?[counter].gameObject.SetActive(true);
        listCandy[counter].OnMoveComplete += OnCandyMoveComplete;
        listCandy[counter].ID = counter;
        fillTime = 0f;
        counter++;
        if (counter == total)
        {
            Debug.Log("ok");
            StartCoroutine(Fill());
        }
    }

    IEnumerator Fill()
    {
        bool needsRefill = true;
        while (needsRefill)
        {
            yield return new WaitForSeconds(0.5f);
            /*while (FillStep())
            {
                yield return new WaitForSeconds(fillTime);
            }*/
            needsRefill = ClearAllValid();
        }
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
            var (_x, _y) = MatchUtils.GetFromID(i, X);
            Slot item = Instantiate(cell, cellTF);
            item.name = $"Cell x:{_x}, y:{_y} => id:{MatchUtils.SetID(_x, _y,X)}";
            item.transform.position = new Vector3(_x * offset, _y *  -offset + offsetY, 0f);
            cellList.Add(item);
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
    
    public void SpawnCandy(int x, int y)
    {
        Candy item = Instantiate(candy, candyTF);
        item.ID = MatchUtils.SetID(x, y, Width);
        item.Index = index;
        GameManager.Instance.listPath.Add(new Vector2(x * offset, y * -offset + offsetY));
        item.transform.position = new Vector3(0, -10);
        CheckRule(item);
        item.gameObject.SetActive(false);
        listCandy.Add(item);
        index++;
        item.Init(this);
    }
    
    public void RespawnCandy(int x, int y)
    {
        Candy item = candies[x, y];
        
        item.Init(this);
    }

    private void OnCandyMoveComplete(Candy candy, int x, int y)
    {
        candies[x, y] = candy;
        candy.name = $"Candy x:{x}, y:{y} id:{candy.ID}";
        //candy.OnMoveComplete -= OnCandyMoveComplete;
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
    

    
    public void OnEnterCandy(Candy candy)
    {
        enteredCandy = candy;
    }

    public void OnPressedCandy(Candy candy)
    {
        pressedCandy = candy;
        var opa = pressedCandy.lightRender.color;
        opa.a = 1f;
        pressedCandy.lightRender.color = opa;
    }

    public void OnReleaseCandy()
    {
        if (MatchUtils.IsAdjacent(pressedCandy, enteredCandy, offset))
        {
            SwapBlock(pressedCandy, enteredCandy);
            var opa1 = pressedCandy.lightRender.color;
            opa1.a = 0f;
            pressedCandy.lightRender.color = opa1;
            var opa2 = enteredCandy.lightRender.color;
            opa2.a = 0f;
            pressedCandy.lightRender.color = opa2;
        }
        //Debug.Log($"{IsAdjacent(enteredCandy, pressedCandy)}");
    }
    
    public void SwapBlock(Candy candy_1,Candy candy_2)
    {
        candies[candy_1.X, candy_1.Y] = candy_2;
        candies[candy_2.X, candy_2.Y] = candy_1;

        if (GetMatch(candy_1, candy_2.X, candy_2.Y) != null || GetMatch(candy_2, candy_1.X, candy_1.Y) != null)
        {
            int candy1X = candy_1.X;
            int candy1Y = candy_1.Y;

            int candy2X = candy_2.X;
            int candy2Y = candy_2.Y;
            
            ClearAllValid();
            Debug.Log("GetMatch");
            
            pressedCandy.Dissolving();
            enteredCandy.Dissolving();
            
            //pressedCandy = null;
            //enteredCandy = null;
            //StartCoroutine(Fill());
        }
        else
        {
            Debug.Log("Can not matc");
        }
    }
    
    [Button]
    public bool ClearAllValid()
    {
        bool needsRefill = false;
        
        var currentPos = new Vector2(-2f, 8f);
        var dirPos = new Vector2(2f, 0);
        var currentIndex = 0;
        
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                if (currentPos.x < 8)
                {
                    currentPos += dirPos;
                }
                else
                {
                    currentPos.x = 0;
                    currentPos.y -= 2f;
                }

                List<Candy> match = GetMatch(candies[(int)(currentPos.x), (int)currentPos.y], (int)(currentPos.x),
                    (int)currentPos.y);

                if (match != null)
                {
                    CandyType specialBlockType = CandyType.Special;
                    Candy randonBlock = match[Random.Range(0, match.Count)];
                    int specialBlockX = randonBlock.X;
                    int specialBlockY = randonBlock.Y;
                    
                    for(int i = 0; i < match.Count; i++)
                    {
                        if(ClearBlock(match[i].X, match[i].Y))
                        {
                            needsRefill = true;

                            if(match[i] == pressedCandy || match[i] == enteredCandy)
                            {
                                specialBlockX = match[i].X;
                                specialBlockY = match[i].Y;
                            }
                        }
                    }
                }
                //Random
                
                
                currentIndex++;
            }
        }

        return needsRefill;
    }
    
    public bool ClearBlock(int x, int y)
    {
        if(!candies[x, y].Clearable.IsBeingCleared)
        {
            candies[x, y].Clearable.Clear();
            
            RespawnCandy(x, y);

            //ClearObstacles(x, y);
            return true;
        }
        return false;
    }

    public void ClearObstacles(int x, int y)
    {
        //check the abjacent blocks x direction 
        for (int abjacentX = x -1; abjacentX <= x + 1; abjacentX++)
        {
            if(abjacentX != x && abjacentX >= 0 && abjacentX < Width)
                if(candies[abjacentX, y].CandyType.Color == CandyType.Black)
                {
                    candies[abjacentX, y].Clearable.Clear();
                    SpawnCandy(abjacentX, y);
                }
        }
        //check the abjacent blocks y direction 
        for(int abjacentY = y - 1; abjacentY <= y + 1; abjacentY++)
        {
            if (abjacentY != y && abjacentY >= 0 && abjacentY < Height)
                if (candies[x, abjacentY].CandyType.Color == CandyType.Black)
                {
                    candies[x, abjacentY].Clearable.Clear();
                    SpawnCandy(x, abjacentY);
                }
        }
    }
    
    public List<Candy> GetMatch(Candy candy, int originX, int originY)
    {
        CandyType color = candy.CandyType.Color;
        List<Candy> xPos = new List<Candy>();
        List<Candy> yPos = new List<Candy>();
        List<Candy> matchCandy = new List<Candy>();
        xPos.Add(candy);
        
        for (int dir = 0; dir <= 1; dir++)
        {
            for (int xOffset = 1; xOffset < Width; xOffset++)
            {
                int x;

                if (dir == 0)
                {
                    x = MatchUtils.CheckLeft(originX, xOffset, offset);
                }
                else
                {
                    x = originX + xOffset * offset;
                }
                
                if (x < 0 || x >= Width * offset)
                {
                    break;
                }

                if (candies[x, originY].CandyType.Color == color)
                {
                    if (!xPos.Contains(candies[x, originX]))
                    {
                        xPos.Add(candies[x, originY]);
                    }
                }
                else break;
            }
        }

        if (xPos.Count >= 3)
        {
            for (int i = 0; i < xPos.Count; i++)
            {
                if (!matchCandy.Contains(xPos[i]))
                {
                    matchCandy.Add(xPos[i]);
                }
            }
        }
        
        if (xPos.Count >= 3)
        {
            foreach (var t1 in xPos)
            {
                for (int dir = 0; dir <= 1; dir++)
                {
                    for (int yOffset = 1; yOffset < Height; yOffset++)
                    {
                        int y;

                        if (dir == 0)
                        {
                            y = originY - yOffset * offset;
                        }
                        else
                        {
                            y = originY + yOffset * offset;
                        }

                        if (y < 0 || y >= Height * offset)
                        {
                            break;
                        }
                        if (candies[t1.X, y].CandyType.Color == color)
                        { 
                            yPos.Add(candies[t1.X, y]);
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                if (yPos.Count < 2)
                {
                    yPos.Clear();
                }
                else
                {
                    foreach (var t in yPos)
                    {
                        matchCandy.Add(t);
                    }

                    break;
                }
            }
        }
        if (matchCandy.Count >= 3)
        {
            return matchCandy;
        }
        
        xPos.Clear();
        yPos.Clear();
        
        yPos.Add(candy);
        
        for (int dir = 0; dir <= 1; dir++)
        {
            for (int yOffset = 1; yOffset < Height; yOffset++)
            {
                int y;

                if (dir == 0)
                {
                    y = originY - yOffset * offset;
                }
                else
                {
                    y = originY + yOffset * offset;
                }
                if (y < 0 || y >= Height * offset)
                {
                    break;
                }
                if (candies[originX, y].CandyType.Color == color)
                {
                    yPos.Add(candies[originX, y]);
                }
                else
                {
                    break;
                }
            }
        }
        if (yPos.Count >= 3)
        {
            for (int i = 0; i < yPos.Count; i++)
            {
                matchCandy.Add(yPos[i]);
            }
        }
        
        if (yPos.Count >= 3)
        {
            for (int i = 0; i < yPos.Count; i++)
            {
                for (int dir = 0; dir <= 1; dir++)
                {
                    for (int xOffset = 1; xOffset < Width; xOffset++)
                    {
                        int x;

                        if (dir == 0)
                        {
                            x = originX - xOffset * offset;
                        }
                        else
                        {
                            x = originX + xOffset * offset;
                        }

                        if (x < 0 || x >= Width * offset)
                        {
                            break;
                        }
                        if (candies[x, yPos[i].Y].CandyType.Color == color)
                        {
                            xPos.Add(candies[x, yPos[i].Y]);
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                if (xPos.Count < 2)
                {
                    xPos.Clear();
                }
                else
                {
                    for (int j = 0; j < xPos.Count; j++)
                    {
                        matchCandy.Add(xPos[j]);
                    }
                    break;
                }
            }
        }
        if (matchCandy.Count >= 3)
        {
            return matchCandy;
        }
        
        return null;
    }
    
    public void GameOver()
    {
        gameOver = true;
    }

}
