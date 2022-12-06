using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dissolve : MonoBehaviour
{
    	Material material;
    
    	bool isDissolving = false;
    	float fade = 1f;
        float speedDissolving = 1f;
    
    	void Start()
    	{
    		// Get a reference to the material
    		material = GetComponent<SpriteRenderer>().material;
    	}

        public void Dissolving(bool isEnable)
        {
	        isDissolving = isEnable;
        }
        
    	void Update()
        {
	        if(!isDissolving) return;
	        
	        if (isDissolving)
    		{
    			fade -= Time.deltaTime * speedDissolving;
    
    			if (fade <= 0f)
    			{
    				fade = 0f;
    				isDissolving = false;
    			}
    
    			// Set the property
    			material.SetFloat("_Fade", fade);
    		}
        }
}
