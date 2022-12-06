using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    
    [HideInInspector] public List<Vector2> listPath;
    public Dictionary<CandyType, Sprite> candyDict =  new Dictionary<CandyType, Sprite>();
    public CandySprite[] listCandyPrefab;
    public int currentScore = 0;
    
    private void Awake()
    {
        if (Instance == null) Instance = this;
        listPath = new List<Vector2>();

        PlayerPrefs.SetInt("_score", currentScore);
        
        candyDict = new Dictionary<CandyType, Sprite>();

        for (int i = 0; i < listCandyPrefab.Length; i++)
        {
            if (!candyDict.ContainsKey(listCandyPrefab[i].type))
            {
                candyDict.Add(listCandyPrefab[i].type, listCandyPrefab[i].sprite);
            }
        }
    }

    public void Reset()
    {
        listPath = new List<Vector2>();
    }
    
}
