using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class movefish : MonoBehaviour
{

    private float[] d = new float[4];
    private int targetnum = -1;
    private Vector3 target;
    private Vector3 targetvector;
    public float avairabledistance = 0.5f;
    public float firstspeed = 0.004f, speed = 0.004f, maxspeed = 0.006f, minspeed = 0.0005f, rotatespeed = 0.01f, rotatespeed2=0.01f;
    public float x1, x2, y1, y2;    //行動範囲
    public float rx1, rx2, ry1, ry2;    //↑より強固な行動範囲
    public float myx, myy, tx, ty,dx, dy;
    //public float x1 = 153f, x2 = 154.64f, y1 = -58.8f, y2 = -59.8f;
    //private bool orikaesi = true;
    private bool normalflag = false;
    private Vector3 normalv;
    public bool yuuyo = true, syoutotu = false;

    // Use this for initialization
    void Start()
    {
        float size = Random.Range(0.75f, 1.25f);
        transform.localScale = new Vector3(size, size, size);    //ランダムに成長具合を決めてサイズを変更する
        randomTarget();
    }

    // Update is called once per frame
    void Update()
    {
        if(speed>firstspeed) rotatespeed2 = rotatespeed + (speed - firstspeed)*10; //回転速度は現在の速度の影響を少し受ける
        myx = transform.position.x*100;
        myy = transform.position.y*100;
        tx = target.x*100;
        ty = target.y*100;
        dx = tx - myx;
        dy = ty - myy;
        Vector3 pos = transform.position;
        pos.z = 45.546f;    //z座標はhandpointと同じ初期位置で固定する
        transform.position = pos;

        for (int i = 0; i < hand.handvertex.Length; i++)
        {
            if (DepthSourceView.handexit[i] == 1)   //手が存在している場合のみ距離を測る
            {
                d[i] = Mathf.Sqrt(Mathf.Pow(hand.handvertex[i].x - this.transform.position.x, 2) + Mathf.Pow(hand.handvertex[i].y - this.transform.position.y, 2));
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
            if (normalflag && yuuyo) //衝突した魚と反対方向に泳ぐ
            {
                //Debug.Log(target);
                speed += Random.Range(-0.00005f, 0.00005f);
                if (speed > maxspeed) speed -= 0.0001f;
                if (speed < minspeed) speed += 0.0001f;
                target.z = transform.position.z;
                targetvector = normalv - transform.position;
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(normalv), rotatespeed2);   
                transform.position += transform.forward * speed;
            }
            else //ランダムなターゲット方向に泳ぐ
            {
                float td = Mathf.Sqrt(Mathf.Pow(target.x - this.transform.position.x, 2) + Mathf.Pow(target.y - this.transform.position.y, 2));
                if (td < 0.1f)  //ターゲットまで移動しきったら違うターゲットをランダム指定する
                {
                    yuuyo = true;
                    randomTarget();
                }
                //Debug.Log(target);
                speed += Random.Range(-0.00005f, 0.00005f);
                if (speed > maxspeed) speed -= 0.0001f;
                if (speed < minspeed) speed += 0.0001f;
                target.z = transform.position.z;
                targetvector = target - transform.position;
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(targetvector), rotatespeed2);
                transform.position += transform.forward * speed;
            }

        }


        if (targetnum != -1)    //手の方向に泳ぐ
        {
            speed = 0.006f;
            target.z = transform.position.z;
            targetvector = target - transform.position;
            //targetの方に少しずつ向きが変わる
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(targetvector), rotatespeed2);
            //targetに向かって進む
            transform.position += transform.forward * speed;
            targetnum = -1;
        }

        if (transform.position.x < rx1 || transform.position.x > rx2 || transform.position.y < ry2 || transform.position.y > ry1)   //画面外
        {
            if (yuuyo)
            {
                yuuyo = false;
                randomTarget();
            }
        }

        if (transform.position.x < 151f || transform.position.x > 156f || transform.position.y < -61f || transform.position.y > -58f)   //バグで画面外に行き過ぎたら戻す
        {
            float r = Random.Range(-0.2f, 0.2f); ;
            Vector3 pos2 = transform.position;
            if (r > 0)
            {
                pos2.x = 154.9f;
                transform.rotation = Quaternion.Euler(0.0f, -90.0f, 0.0f);
            }
            else
            {
                pos2.x = 152.7f;
                transform.rotation = Quaternion.Euler(0.0f, 90.0f, 0.0f);
            }
            pos2.y = -59.2f + r;
            pos2.z = 45.546f;
            transform.position = pos2;
        }
    }


    public void OnCollisionStay(Collision other)
    {
        if (syoutotu == false)
        {
            syoutotu = true;
            StartCoroutine("Wait");
            foreach (ContactPoint point in other.contacts)
            {
                //衝突した点の法線ベクトルを使い、衝突した魚と反対方向に泳ぐ
                normalv = point.normal;
                normalv.z = 0;  //あくまで平面的な場所に魚は泳いでるのでXとY方向のみ
                normalv.x += Random.Range(-0.3f, 0.3f); //真反対に向くのではなくランダムさを持たせる
                normalv.y += Random.Range(-0.3f, 0.3f);
                normalflag = true;
                speed -= 0.00001f;
            }
        }
    }


    private void randomTarget()
    {
        normalflag = false;
        target.x = Random.Range(x1, x2);
        target.y = (Random.Range(y1, y2) + transform.position.y) / 2;
    }

    private IEnumerator Wait()
    {
        yield return new WaitForSeconds(1.0f);
        syoutotu = false;
    }

}