using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.Universal;


public class Candy : MonoBehaviour
{
    public int score;
    private int id;
    public int index;
    private CandyType type;
    private Grid grid;
    private Colorable _candyType;
    private Moveable _move;
    private Dissolve _dissolve;
    private Clearable _clearable;
    public Light2D lightRender;
    public int x;
    public int y;
    public float lerpDuration = 5f;
    public float animationTime = 5f;

    public UnityAction<Candy, int, int> OnMoveComplete;
    float timeElapsed;
    float lerpedValue;
    public int counter;

    #region Properties

    public CandyType Type => type;
    
    public Grid Grid
    {
        get { return grid; }
    }
    public Colorable CandyType => _candyType;
    public Clearable Clearable => _clearable;
    

    public int ID
    {
        get => id;
        set => id = value;
    }

    public int X
    {
        get => x;
        set => x = value;
    }

    public int Y
    {
        get => y;
        set => y = value;
    }
    
    public int Index
    {
        get => index;
        set => index = value;
    }

    #endregion

    private void Awake()
    {
        _candyType = GetComponent<Colorable>();
        _clearable = GetComponent<Clearable>();
        _dissolve = GetComponentInChildren<Dissolve>();
    }
    
    public void Init(Grid _grid)
    {
        grid = _grid;
    }

    void FixedUpdate()
    {
        if (timeElapsed < lerpDuration)
        {
            Fill(timeElapsed / lerpDuration * animationTime);
            timeElapsed += Time.fixedDeltaTime;
        }
    }

    public void Dissolving()
    {
        _dissolve.Dissolving(true);
    }

    private void OnMouseEnter()
    {
        grid.OnEnterCandy(this);
    }
    
    private void OnMouseDown()
    {
        grid.OnPressedCandy(this);
    }

    private void OnMouseUp()
    {
        grid.OnReleaseCandy();
    }

    private List<Vector2> ListPath()
    {
        return GameManager.Instance.listPath;
    }
    
    public void Fill(float time)
    {
        var position = transform.position;
        transform.position = Vector2.Lerp(ListPath()[counter], ListPath()[counter + 1], time);
        x = (int)position.x;
        y = (int)position.y;
        

        if (!IsNextPath()) return;
        if (counter >= ListPath().Count - 2 - index)
        {
            OnMoveComplete?.Invoke(this, x, y);
            StartCoroutine(IDelay());
            return;
        }
        
        counter++;
        timeElapsed = 0;
    }

    IEnumerator IDelay()
    {
        yield return new WaitForSeconds(1f);
    }
    
    public bool IsNextPath()
    {
        Vector2 path = ListPath()[counter + 1];
        Vector2 localPos = transform.position;
        
        return Math.Round(localPos.x) == Math.Round(path.x) && Math.Round(localPos.y) == Math.Round(path.y);
    }
}
