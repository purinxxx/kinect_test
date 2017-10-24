using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bubbleparticleplay : MonoBehaviour {

    private ParticleSystem particle;

    // Use this for initialization
    void Start () {
        particle = this.GetComponent<ParticleSystem>();
    }
	
	// Update is called once per frame
	void Update () {
        if (transform.position.z > 45)
        {
            particle.Play();
        }
        else
        {
            particle.Stop();
        }
	}
}
