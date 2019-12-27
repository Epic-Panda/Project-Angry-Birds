using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreController : MonoBehaviour
{
    public float secondsTillDestuction = 1.5f;
    public float sizeMultiplier = 4f;
    public float upTranslationSpeed = 4f;
    
    // Update is called once per frame
    void Update()
    {
        secondsTillDestuction -= Time.deltaTime;

        if (secondsTillDestuction <= 0)
        {
            Destroy(gameObject);
            return;
        }

        transform.localScale += Vector3.one * sizeMultiplier * Time.deltaTime;
        transform.position += Vector3.up * upTranslationSpeed * Time.deltaTime;
    }

}
