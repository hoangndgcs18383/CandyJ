using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Level : MonoBehaviour
{
    public enum LevelType
    {
        TIMER,
        OBSTACLE,
        MOVES
    }

    public Grid grid;

    public Text scoreTxt;

    protected LevelType type;
    protected int currentScore;

    public LevelType Type
    {
        get { return type; }
    }

    public virtual void GameWin()
    {
        Debug.Log("You win");
        grid.GameOver();
    }

    public virtual void GameLose()
    {
        Debug.Log("You win");
        grid.GameOver();
    }
    public virtual void OnMove()
    {
        
    }
    public virtual void OnBlockClear(Candy block)
    {
        currentScore += block.score;
        scoreTxt.text = ("Score: "+ currentScore);
    }
}
