using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KinematicToDynamicAreaOfEffect : MonoBehaviour {

    public List<Rigidbody2D> kinematicRbs;
    
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "bird")
        {
            foreach (Rigidbody2D r in kinematicRbs)
                r.bodyType = RigidbodyType2D.Dynamic;
            Destroy(gameObject);
        }
    }
}
