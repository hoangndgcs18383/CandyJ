using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clearable : MonoBehaviour
{

    private bool isBeingCleared = false;
    public bool IsBeingCleared
    {
        get { return isBeingCleared; }
    }

    protected Candy candy;

    private void Awake()
    {
        candy = GetComponent<Candy>();
    }

    public virtual void Clear()
    {
        //block.Grid.level.OnBlockClear(block);

        isBeingCleared = true;
        StartCoroutine(ClearCoroutine());
    }
    private IEnumerator ClearCoroutine()
    {
        candy.CandyType.Color = CandyType.EMPTY;
        yield return new WaitForSeconds(0.1f);
        candy.Dissolving();
    }
}
