using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class QuadWarp : MonoBehaviour {

    public Material _mat;

    public Texture _tex;
    public Texture _test;
    public Vector2[] _vertixes = new[] { new Vector2(0f, 0f), new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(1f, 0f) };
    public GameObject hosei,floor,iwa;

    int vnum = 0;
    int unlock = 1;
    int test = -1;
    float alpha,rgb;

    Matrix4x4 CalcHomography(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3)
    {
        var sx = p0.x - p1.x + p2.x - p3.x;
        var sy = p0.y - p1.y + p2.y - p3.y;

        var dx1 = p1.x - p2.x;
        var dx2 = p3.x - p2.x;
        var dy1 = p1.y - p2.y;
        var dy2 = p3.y - p2.y;

        var z = (dy1 * dx2) - (dx1 * dy2);
        var g = ((sx * dy1) - (sy * dx1)) / z;
        var h = ((sy * dx2) - (sx * dy2)) / z;

        var system = new[]{
            p3.x * g - p0.x + p3.x,
            p1.x * h - p0.x + p1.x,
            p0.x,
            p3.y * g - p0.y + p3.y,
            p1.y * h - p0.y + p1.y,
            p0.y,
            g,
            h,
        };

        var mtx = Matrix4x4.identity;
        mtx.m00 = system[0]; mtx.m01 = system[1]; mtx.m02 = system[2];
        mtx.m10 = system[3]; mtx.m11 = system[4]; mtx.m12 = system[5];
        mtx.m20 = system[6]; mtx.m21 = system[7]; mtx.m22 = 1f;

		return mtx;
    }

    public void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        var homography = CalcHomography(_vertixes[0], _vertixes[1], _vertixes[2], _vertixes[3]).inverse;
        
        Graphics.SetRenderTarget(destination);
        GL.Clear(true, true, Color.clear);

        GL.PushMatrix();
        GL.LoadOrtho();

        //_mat.mainTexture = _tex;
        _mat.SetMatrix("_Homography", homography);
        _mat.SetPass(0);

        var rectPixel = new Rect(0f, 0f , Screen.width, Screen.height);
        GL.Viewport(rectPixel);

        GL.Begin(GL.QUADS);

        for (var i = 0; i < 4; ++i)
        {
            GL.Vertex(_vertixes[i]);
        }

        GL.End();


        GL.PopMatrix();
    }

    private void Start()
    {
        _vertixes[0].x = PlayerPrefs.GetFloat("0x", 0f);
        _vertixes[0].y = PlayerPrefs.GetFloat("0y", 0f);
        _vertixes[1].x = PlayerPrefs.GetFloat("1x", 0f);
        _vertixes[1].y = PlayerPrefs.GetFloat("1y", 1f);
        _vertixes[2].x = PlayerPrefs.GetFloat("2x", 1f);
        _vertixes[2].y = PlayerPrefs.GetFloat("2y", 1f);
        _vertixes[3].x = PlayerPrefs.GetFloat("3x", 1f);
        _vertixes[3].y = PlayerPrefs.GetFloat("3y", 0f);
        alpha = PlayerPrefs.GetFloat("alpha", 50);
        rgb = PlayerPrefs.GetFloat("rgb", 130);
    }

    void Update()
    {
        if (Input.GetButtonDown("Scuare"))
        {
            vnum = 0;
        }
        else if (Input.GetButtonDown("Cross"))
        {
            vnum = 3;
        }
        else if (Input.GetButtonDown("Circle"))
        {
            vnum = 2;

        }
        else if (Input.GetButtonDown("Triangle"))
        {
            vnum = 1;
        }

        if (Input.GetButtonDown("R2"))
        {
            unlock *= -1;
            PlayerPrefs.Save();
        }

        if (unlock==1)
        { 
            _vertixes[vnum].x += Input.GetAxis("Horizontal1") / 400;
            _vertixes[vnum].y += Input.GetAxis("Vertical1") / 400;
            _vertixes[vnum].x += Input.GetAxis("Mouse X1") / 1200;
            _vertixes[vnum].y += Input.GetAxis("Mouse Y1") / 1200;
            alpha += Input.GetAxis("juuji");
            rgb += Input.GetAxis("sayuu");

            hosei.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, alpha / 255.0f);
            floor.GetComponent<Renderer>().material.SetColor("_FogColor1", new Color(rgb / 255.0f, rgb / 255.0f, rgb / 255.0f, 1));
            iwa.GetComponent<SpriteRenderer>().color = new Color(rgb / 255.0f, rgb / 255.0f, rgb / 255.0f, 1);
            PlayerPrefs.SetFloat("0x", _vertixes[0].x);
            PlayerPrefs.SetFloat("0y", _vertixes[0].y);
            PlayerPrefs.SetFloat("1x", _vertixes[1].x);
            PlayerPrefs.SetFloat("1y", _vertixes[1].y);
            PlayerPrefs.SetFloat("2x", _vertixes[2].x);
            PlayerPrefs.SetFloat("2y", _vertixes[2].y);
            PlayerPrefs.SetFloat("3x", _vertixes[3].x);
            PlayerPrefs.SetFloat("3y", _vertixes[3].y);
            PlayerPrefs.SetFloat("alpha", alpha);
            PlayerPrefs.SetFloat("rgb", rgb);

            if (Input.GetButtonDown("L2"))
            {
                test *= -1;
            }

            if (test == 1)
            {
                _mat.mainTexture = _test;
            }

            if (test == -1)
            {
                _mat.mainTexture = _tex;
            }
        }
    }
}
