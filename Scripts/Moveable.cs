using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Moveable : MonoBehaviour
{
    public Candy block;
    private IEnumerator moveCoroutine;
    
    public void Move(int newX, int newY, float time)
    {
        if(moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
        }

        //moveCoroutine = MoveCoroutine(newX, newY, time);
        //StartCoroutine(moveCoroutine);
    }

    /*private IEnumerator MoveCoroutine(int newX, int newY, float time)
    {
        block.X = newX;
        block.Y = newY;
        
        Vector3 startPos = transform.position;
        Vector3 endPos = block.Grid.GetWorldPosition(newX, newY);

        for(float t = 0; t <= 1 * time; t += Time.deltaTime)
        {
            block.transform.position = Vector3.Lerp(startPos, endPos, t / time);
            yield return 0;
        }
        block.transform.position = endPos;
    }*/
}
