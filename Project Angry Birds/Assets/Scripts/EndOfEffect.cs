using System.Collections;
using UnityEngine;

public class EndOfEffect : MonoBehaviour {

    public float waitTime = .1f;

	// Use this for initialization
	void Start () {
        StartCoroutine(EndEffect());
	}

    IEnumerator EndEffect()
    {
        yield return new WaitForSeconds(waitTime);
        Destroy(gameObject);
    }
}
