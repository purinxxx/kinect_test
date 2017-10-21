using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class movefish : MonoBehaviour {

    private float[] d = new float[4];
    private int targetnum = -1;
    private Vector3 target;
    private Vector3 targetvector;
    public float avairabledistance = 0.5f;
    public float speed = 0.004f, maxspeed = 0.006f, minspeed = 0.0005f, rotatespeed = 0.01f;
    public float x1 = 153f, x2 = 154.64f, y1 = -58.8f, y2 = -59.8f;
    //private bool orikaesi = true;
    private bool normalflag = false;
    private Vector3 normalv;

    // Use this for initialization
    void Start ()
    {
        target.x = Random.Range(x1, x2);
        target.y = Random.Range(y1, y2);
    }
	
	// Update is called once per frame
	void Update () {
        Vector3 pos = transform.position;
        pos.z = 45.546f;    //z座標はhandpointと同じ初期位置で固定する
        transform.position = pos;

        for (int i = 0; i < hand.handvertex.Length; i++)
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

        float dmin = Mathf.Min(d);

        if (dmin < avairabledistance && dmin > 0.1f)    //最も近くの手が一定の距離以内の場合、その手に向かって泳ぐ
        {
            for (int i = 0; i < d.Length; i++)
            {
                if (d[i] == dmin)
                {
                    targetnum = i;
                    target.x = hand.handvertex[targetnum].x;
                    target.y = hand.handvertex[targetnum].y;
                    normalflag = false;
                }
            }
        }
        else
        {
            if (normalflag) //衝突した魚と反対方向に泳ぐ
            {
                //Debug.Log(target);
                speed += Random.Range(-0.00005f, 0.00005f);
                if (speed > maxspeed) speed -= 0.0001f;
                if (speed < minspeed) speed += 0.0001f;
                target.z = transform.position.z;
                targetvector = normalv - transform.position;
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(normalv), rotatespeed);
                transform.position += transform.forward * speed;
            }
            else //ランダムなターゲット方向に泳ぐ
            {
                float td = Mathf.Sqrt(Mathf.Pow(target.x - this.transform.position.x, 2) + Mathf.Pow(target.y - this.transform.position.y, 2));
                if (td < 0.1f)  //ターゲットまで移動しきったら違うターゲットをランダム指定する
                {
                    randomTarget();
                }
                //Debug.Log(target);
                speed += Random.Range(-0.00005f, 0.00005f);
                if (speed > maxspeed) speed -= 0.0001f;
                if (speed < minspeed) speed += 0.0001f;
                target.z = transform.position.z;
                targetvector = target - transform.position;
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(targetvector), rotatespeed);
                transform.position += transform.forward * speed;
            }
            
        }


        if (targetnum != -1)    //手の方向に泳ぐ
        {
            speed = 0.006f;
            target.z = transform.position.z;
            targetvector = target - transform.position;
            //targetの方に少しずつ向きが変わる
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(targetvector), rotatespeed);
            //targetに向かって進む
            transform.position += transform.forward * speed;
            targetnum = -1;
        }

        if (transform.position.x < x1 || transform.position.x > x2 || transform.position.y < y2 || transform.position.y > y1)
        {
            randomTarget();
        }
	}
    

    public void OnCollisionEnter(Collision other)
    {
        foreach (ContactPoint point in other.contacts)
        {
            //衝突した点の法線ベクトルを使い、衝突した魚と反対方向に泳ぐ
            normalv = point.normal;
            normalv.z = 0;
            Debug.Log(normalv);
            normalflag = true;
        }
    }


    private void randomTarget()
    {
        normalflag = false;
        target.x = Random.Range(x1, x2);
        target.y = Random.Range(y1, y2);
    }

}
