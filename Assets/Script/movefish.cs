using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class movefish : MonoBehaviour {

    float[] d = new float[4];
    int targetnum = -1;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		for(int i = 0; i < hand.handvertex.Length; i++)
        {
            if (DepthSourceView.handexit[i] == 1)   //手が存在している場合のみ距離を測る
            {
                d[i] = Mathf.Sqrt(Mathf.Pow(hand.handvertex[i].x - this.transform.position.x,2) + Mathf.Pow(hand.handvertex[i].y - this.transform.position.y, 2));
            } else
            {
                d[i] = 10000;
            }

        }

        float dmin = Mathf.Min(d);
        if (dmin < 0.3f)
        {
            for (int i = 0; i < d.Length; i++)
            {
                if (d[i] == dmin)
                {
                    targetnum = i;
                }
            }
        }

        if (targetnum != -1)
        {
            Vector3 target;
            target.x = hand.handvertex[targetnum].x;
            target.y = hand.handvertex[targetnum].y;
            target.z = this.transform.position.z;
            //targetの方に少しずつ向きが変わる
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(target - this.transform.position), 0.01f);
            //targetに向かって進む
            transform.position += transform.forward * 0.005f;
            targetnum = -1;
        }
	}
}
