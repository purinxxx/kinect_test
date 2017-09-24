using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class movefish : MonoBehaviour {

    private float[] d = new float[4];
    private int targetnum = -1;
    private Vector3 target;
    private Vector3 targetvector;
    private float speed = 0.004f;
    private float x1 = 153f, x2 = 154.65f, y1 = -58.8f, y2 = -59.85f, zfix = 0.003f;
    //private bool orikaesi = true;

    // Use this for initialization
    void Start ()
    {
        target.x = Random.Range(x1, x2);
        target.y = Random.Range(y1, y2);
    }
	
	// Update is called once per frame
	void Update () {
		for(int i = 0; i < hand.handvertex.Length; i++)
        {
            if (DepthSourceView.handexit[i] == 1)   //手が存在している場合のみ距離を測る
            {
                d[i] = Mathf.Sqrt(Mathf.Pow(hand.handvertex[i].x - this.transform.position.x,2) + Mathf.Pow(hand.handvertex[i].y - this.transform.position.y, 2));
            }
            else
            {
                d[i] = 10000;
            }

        }

        float dmin = Mathf.Min(d);  //最も近くの手が一定の距離以内の場合、その手に向かって泳ぐ

        if (dmin < 0.5f)
        {
            for (int i = 0; i < d.Length; i++)
            {
                if (d[i] == dmin)
                {
                    targetnum = i;
                    target.x = hand.handvertex[targetnum].x;
                    target.y = hand.handvertex[targetnum].y;
                }
            }
        }
        else
        {
            float td = Mathf.Sqrt(Mathf.Pow(target.x - this.transform.position.x, 2) + Mathf.Pow(target.y - this.transform.position.y, 2));
            if (td < 0.1f)
            {
                target.x = Random.Range(x1, x2);
                target.y = Random.Range(y1, y2);
            }
            //Debug.Log(target);
            speed += Random.Range(-0.0003f,0.0003f);
            if (speed > 0.006f) speed -= 0.001f;
            if (speed < 0.0005f) speed += 0.001f;
            target.z = transform.position.z;
            targetvector = target - transform.position;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(targetvector), 0.05f);
            transform.position += transform.forward * speed;
            Vector3 pos = transform.position;
            if (pos.z > 45.546) //z座標はhandpointと同じ初期位置で固定する
            {
                pos.z -= zfix;
            }
            else
            {
                pos.z += zfix;
            }
            transform.position = pos;
        }


        if (targetnum != -1)
        {
            speed = 0.005f;
            target.z = transform.position.z;
            targetvector = target - transform.position;
            //targetの方に少しずつ向きが変わる
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(targetvector), 0.05f);
            //targetに向かって進む
            transform.position += transform.forward * speed;
            Vector3 pos = transform.position;
            if (pos.z > 45.546) //z座標はhandpointと同じ初期位置で固定する
            {
                pos.z -= zfix;
            }
            else
            {
                pos.z += zfix;
            }
            transform.position = pos;
            targetnum = -1;
        }
	}

    /*private IEnumerator wait()
    {
        // 5秒待つ  
        yield return new WaitForSeconds(5.0f);
        orikaesi = true;
    }*/
    private void OnCollisionEnter(Collision collision)
    {
        target.x = Random.Range(x1, x2);
        target.y = Random.Range(y1, y2);
    }

}
