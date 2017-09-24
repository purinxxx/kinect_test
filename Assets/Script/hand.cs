using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hand : MonoBehaviour {

    public GameObject handpoint1;
    public GameObject handpoint2;
    public GameObject handpoint3;
    public GameObject handpoint4;
    public static Vector2[] handvertex = new[] { new Vector2(0f, 0f), new Vector2(0f, 0f), new Vector2(0f, 0f), new Vector2(0f, 0f)};
    

    // Use this for initialization
    void Start () {

    }
	
	// Update is called once per frame
	void Update ()
    {
        handvertex[0].x = handpoint1.transform.position.x;
        handvertex[1].x = handpoint2.transform.position.x;
        handvertex[2].x = handpoint3.transform.position.x;
        handvertex[3].x = handpoint4.transform.position.x;
        handvertex[0].y = handpoint1.transform.position.y;
        handvertex[1].y = handpoint2.transform.position.y;
        handvertex[2].y = handpoint3.transform.position.y;
        handvertex[3].y = handpoint4.transform.position.y;

    }
}
