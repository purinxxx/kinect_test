﻿using UnityEngine;
using System.Collections;
using Windows.Kinect;
using System;
using System.Collections.Generic;

public enum DepthViewMode
{
    SeparateSourceReaders,
    MultiSourceReader,
}

public class DepthSourceView : MonoBehaviour
{
    public int pre_i=-1;
    public float real_x = 0;
    public float real_y = 0;
    public List<float> detectedx = new List<float>();    //検出されたx座標を格納
    public List<float> detectedy = new List<float>();    //検出されたy座標を格納
    public float[] handx = new float[10];    //detectedから複数の手の位置を推測して格納　４つくらいまで
    public float[] handy = new float[10];    //detectedから複数の手の位置を推測して格納　４つくらいまで
    public List<float> hand1x = new List<float>();
    public List<float> hand1y = new List<float>();
    public List<float> hand2x = new List<float>();
    public List<float> hand2y = new List<float>();
    public List<float> hand3x = new List<float>();
    public List<float> hand3y = new List<float>();
    public List<float> hand4x = new List<float>();
    public List<float> hand4y = new List<float>();

    public DepthViewMode ViewMode = DepthViewMode.SeparateSourceReaders;
    
    public GameObject ColorSourceManager;
    public GameObject DepthSourceManager;
    public GameObject MultiSourceManager;
    public GameObject localPotisionAdapter;

    public GameObject MainScreen;
    public GameObject HandPoint1;
    public GameObject HandPoint2;
    public GameObject HandPoint3;
    public GameObject HandPoint4;

    private KinectSensor _Sensor;
    private CoordinateMapper _Mapper;
    private Mesh _Mesh;
    private Vector3[] _Vertices;
    private Vector2[] _UV;
    private int[] _Triangles;
    
    // Only works at 4 right now
    private const int _DownsampleSize = 4;
    private const double _DepthScale = 0.1f;
    private const int _Speed = 50;
    
    private MultiSourceManager _MultiManager;
    private ColorSourceManager _ColorManager;
    private DepthSourceManager _DepthManager;

    void Start()
    {
        detectedx.Clear();
        detectedy.Clear();
        hand1x.Clear();
        hand1y.Clear();
        hand2x.Clear();
        hand2y.Clear();
        hand3x.Clear();
        hand3y.Clear();
        hand4x.Clear();
        hand4y.Clear();
        _Sensor = KinectSensor.GetDefault();
        if (_Sensor != null)
        {
            _Mapper = _Sensor.CoordinateMapper;
            var frameDesc = _Sensor.DepthFrameSource.FrameDescription;

            // Downsample to lower resolution
            //CreateMesh(frameDesc.Width / _DownsampleSize, frameDesc.Height / _DownsampleSize);

            if (!_Sensor.IsOpen)
            {
                _Sensor.Open();
            }
        }
    }

    void CreateMesh(int width, int height)
    {
        _Mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = _Mesh;

        _Vertices = new Vector3[width * height];
        _UV = new Vector2[width * height];
        _Triangles = new int[6 * ((width - 1) * (height - 1))];

        int triangleIndex = 0;
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int index = (y * width) + x;

                _Vertices[index] = new Vector3(x, -y, 0);
                _UV[index] = new Vector2(((float)x / (float)width), ((float)y / (float)height));

                // Skip the last row/col
                if (x != (width - 1) && y != (height - 1))
                {
                    int topLeft = index;
                    int topRight = topLeft + 1;
                    int bottomLeft = topLeft + width;
                    int bottomRight = bottomLeft + 1;

                    _Triangles[triangleIndex++] = topLeft;
                    _Triangles[triangleIndex++] = topRight;
                    _Triangles[triangleIndex++] = bottomLeft;
                    _Triangles[triangleIndex++] = bottomLeft;
                    _Triangles[triangleIndex++] = topRight;
                    _Triangles[triangleIndex++] = bottomRight;
                }
            }
        }

        _Mesh.vertices = _Vertices;
        _Mesh.uv = _UV;
        _Mesh.triangles = _Triangles;
        _Mesh.RecalculateNormals();
    }
    
    void OnGUI()
    {
        //GUI.BeginGroup(new Rect(0, 0, Screen.width, Screen.height));
        //GUI.TextField(new Rect(Screen.width - 250 , 10, 250, 20), "DepthMode: " + ViewMode.ToString());
        //GUI.EndGroup();
    }

    void Update()
    {
        if (_Sensor == null)
        {
            return;
        }

        /*if (Input.GetButtonDown("Fire1"))
        {

            if (ViewMode == DepthViewMode.MultiSourceReader)
            {
                ViewMode = DepthViewMode.SeparateSourceReaders;
            }
            else
            {
                ViewMode = DepthViewMode.MultiSourceReader;
            }
        }

        float yVal = Input.GetAxis("Horizontal");
        float xVal = -Input.GetAxis("Vertical");

        transform.Rotate(
            (xVal * Time.deltaTime * _Speed),
            (yVal * Time.deltaTime * _Speed),
            0,
            Space.Self);*/

        if (ViewMode == DepthViewMode.SeparateSourceReaders)
        {
            if (ColorSourceManager == null)
            {
                return;
            }

            _ColorManager = ColorSourceManager.GetComponent<ColorSourceManager>();
            if (_ColorManager == null)
            {
                return;
            }

            if (DepthSourceManager == null)
            {
                return;
            }

            _DepthManager = DepthSourceManager.GetComponent<DepthSourceManager>();
            if (_DepthManager == null)
            {
                return;
            }

            gameObject.GetComponent<Renderer>().material.mainTexture = _ColorManager.GetColorTexture();
            RefreshData(_DepthManager.GetData(),    //デプスマネージャーからushortのデプスデータ取ってきてる
                _ColorManager.ColorWidth,
                _ColorManager.ColorHeight);


        }
        else
        {
            if (MultiSourceManager == null)
            {
                return;
            }

            _MultiManager = MultiSourceManager.GetComponent<MultiSourceManager>();
            if (_MultiManager == null)
            {
                return;
            }

            gameObject.GetComponent<Renderer>().material.mainTexture = _MultiManager.GetColorTexture();

            RefreshData(_MultiManager.GetDepthData(),    //デプスマネージャーからushortのデプスデータ取ってきてる
                        _MultiManager.ColorWidth,
                        _MultiManager.ColorHeight);
        }

        Debug.Log(detectedx.Count);
        for (int i = 0; i < detectedx.Count; i++)  //複数手判定
        {
            if (hand1x.Count == 0)
            {
                hand1x.Add(detectedx[i]);
                hand1y.Add(detectedy[i]);
            }
            else
            {
                if (Mathf.Abs(detectedx[i] - hand1x[0]) + Mathf.Abs(detectedy[i] - hand1y[0]) < 300)
                {
                    hand1x.Add(detectedx[i]);
                    hand1y.Add(detectedy[i]);
                }
                else
                {
                    if (hand2x.Count == 0)
                    {
                        hand2x.Add(detectedx[i]);
                        hand2y.Add(detectedy[i]);
                    }
                    else
                    {
                        if (Mathf.Abs(detectedx[i] - hand2x[0]) + Mathf.Abs(detectedy[i] - hand2y[0]) < 300)
                        {
                            hand2x.Add(detectedx[i]);
                            hand2y.Add(detectedy[i]);
                        }
                        else
                        {
                            if (hand3x.Count == 0)
                            {
                                hand3x.Add(detectedx[i]);
                                hand3y.Add(detectedy[i]);
                            }
                            else
                            {
                                if (Mathf.Abs(detectedx[i] - hand3x[0]) + Mathf.Abs(detectedy[i] - hand3y[0]) < 300)
                                {
                                    hand3x.Add(detectedx[i]);
                                    hand3y.Add(detectedy[i]);
                                }
                                else
                                {
                                    if (hand4x.Count == 0)
                                    {
                                        hand4x.Add(detectedx[i]);
                                        hand4y.Add(detectedy[i]);
                                    }
                                    else
                                    {
                                        if (Mathf.Abs(detectedx[i] - hand4x[0]) + Mathf.Abs(detectedy[i] - hand4y[0]) < 30)
                                        {
                                            hand4x.Add(detectedx[i]);
                                            hand4y.Add(detectedy[i]);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        hand1ave();
        hand2ave();
        hand3ave();
        hand4ave();

        detectedx.Clear();
        detectedy.Clear();
        hand1x.Clear();
        hand1y.Clear();
        hand2x.Clear();
        hand2y.Clear();
        hand3x.Clear();
        hand3y.Clear();
        hand4x.Clear();
        hand4y.Clear();
    }
    
    private void RefreshData(ushort[] depthData, int colorWidth, int colorHeight)   //データ入れる場所　デプス,1920,1080
    {
        int cnt = 0;
        for (int i = 0; i < 512; i+=2)  //計算量短縮
        {
            int num = 108544 + i;
            if (800 < depthData[num] && depthData[num] < 1800 && pre_i != i)
            {
                pre_i = i;
                float a = 6.5f;  //6.5385
                float b = 850f;
                float c = (depthData[num] - b) / a;
                real_x = (1303 / 2) / (256 - c) * (i - c);
                real_x = Mathf.Floor(real_x);
                real_y = (depthData[num] - 850) * 1.14f;    //970 / 850;
                real_y = Mathf.Floor(real_y);
                Debug.Log("x = " + i.ToString() + "    y = " + depthData[num].ToString() + "    real_x = " + real_x.ToString() + "    real_y = " + real_y.ToString());
                detectedx.Add(real_x);
                detectedy.Add(real_y);
                cnt++;
                
            }
            else if (pre_i == i) {
                //Vector3 pos = HandPoint1.transform.localPosition;
                //pos.z = 1;
                //HandPoint1.transform.localPosition = pos;
            }

        }
        /*var frameDesc = _Sensor.DepthFrameSource.FrameDescription;
        
        ColorSpacePoint[] colorSpace = new ColorSpacePoint[depthData.Length];
        _Mapper.MapDepthFrameToColorSpace(depthData, colorSpace);

        for (int y = 0; y < frameDesc.Height; y += _DownsampleSize) //0-1080 step 4
        {
            for (int x = 0; x < frameDesc.Width; x += _DownsampleSize)  //0-1920 step 4
            {
                int indexX = x / _DownsampleSize;
                int indexY = y / _DownsampleSize;
                int smallIndex = (indexY * (frameDesc.Width / _DownsampleSize)) + indexX;
                
                double avg = GetAvg(depthData, x, y, frameDesc.Width, frameDesc.Height);
                
                avg = avg * _DepthScale;
                
                _Vertices[smallIndex].z = (float)avg;
                
                // Update UV mapping with CDRP
                var colorSpacePoint = colorSpace[(y * frameDesc.Width) + x];
                _UV[smallIndex] = new Vector2(colorSpacePoint.X / colorWidth, colorSpacePoint.Y / colorHeight);
            }
        }
        
        _Mesh.vertices = _Vertices;
        _Mesh.uv = _UV;
        _Mesh.triangles = _Triangles;
        _Mesh.RecalculateNormals();*/
    }
    
    private double GetAvg(ushort[] depthData, int x, int y, int width, int height)
    {
        double sum = 0.0;
        
        for (int y1 = y; y1 < y + 4; y1++)
        {
            for (int x1 = x; x1 < x + 4; x1++)
            {
                int fullIndex = (y1 * width) + x1;
                if (y1==212 && x1==256)   //x1(0-511) y1(0-423)
                {
                    //Debug.Log(fullIndex);
                    //Debug.Log(x1);
                }

                if (depthData[fullIndex] == 0)  //０は認識できてないので一番遠いことにする
                    sum += 4500;
                else
                    sum += depthData[fullIndex];
            }
        }

        return sum / 16;
    }

    void OnApplicationQuit()
    {
        if (_Mapper != null)
        {
            _Mapper = null;
        }
        
        if (_Sensor != null)
        {
            if (_Sensor.IsOpen)
            {
                _Sensor.Close();
            }

            _Sensor = null;
        }
    }

    void hand1ave()
    {
        if (hand1x.Count != 0)
        {
            float avex = 0;
            float avey = 0;
            for (int i = 0; i < hand1x.Count; i++)
            {
                avex += hand1x[i];
                avey += hand1y[i];
            }

            avex = avex / hand1x.Count;
            avey = avey / hand1y.Count;
            Debug.Log(avex.ToString() + "," + avey.ToString());

            Vector3 pos = HandPoint1.transform.localPosition;
            pos.x = avex / 10;
            pos.y = avey / 10;
            pos.z = -1;
            HandPoint1.transform.localPosition = pos;
        }
        else {
            Vector3 pos = HandPoint1.transform.localPosition;
            pos.z = 1;
            HandPoint1.transform.localPosition = pos;
        }

    }

    void hand2ave()
    {
        if (hand2x.Count != 0)
        {
            float avex = 0;
            float avey = 0;

            for (int i = 0; i < hand2x.Count; i++)
            {
                avex += hand2x[i];
                avey += hand2y[i];
            }

            avex = avex / hand2x.Count;
            avey = avey / hand2y.Count;
            Debug.Log(avex.ToString() + "," + avey.ToString());

            Vector3 pos = HandPoint2.transform.localPosition;
            pos.x = avex / 10;
            pos.y = avey / 10;
            pos.z = -1;
            HandPoint2.transform.localPosition = pos;
        }
        else
        {
            Vector3 pos = HandPoint2.transform.localPosition;
            pos.z = 1;
            HandPoint2.transform.localPosition = pos;
        }
    }

    void hand3ave()
    {
        if (hand3x.Count != 0)
        {
            float avex = 0;
            float avey = 0;

            for (int i = 0; i < hand3x.Count; i++)
            {
                avex += hand3x[i];
                avey += hand3y[i];
            }

            avex = avex / hand3x.Count;
            avey = avey / hand3y.Count;
            Debug.Log(avex.ToString() + "," + avey.ToString());

            Vector3 pos = HandPoint3.transform.localPosition;
            pos.x = avex / 10;
            pos.y = avey / 10;
            pos.z = -1;
            HandPoint3.transform.localPosition = pos;
        }
        else
        {
            Vector3 pos = HandPoint3.transform.localPosition;
            pos.z = 1;
            HandPoint3.transform.localPosition = pos;
        }
    }

    void hand4ave()
    {
        if (hand4x.Count != 0)
        {
            float avex = 0;
            float avey = 0;

            for (int i = 0; i < hand4x.Count; i++)
            {
                avex += hand4x[i];
                avey += hand4y[i];
            }

            avex = avex / hand4x.Count;
            avey = avey / hand4y.Count;
            Debug.Log(avex.ToString() + "," + avey.ToString());

            Vector3 pos = HandPoint4.transform.localPosition;
            pos.x = avex / 10;
            pos.y = avey / 10;
            pos.z = -1;
            HandPoint4.transform.localPosition = pos;
        }
        else
        {
            Vector3 pos = HandPoint4.transform.localPosition;
            pos.z = 1;
            HandPoint4.transform.localPosition = pos;
        }
    }
}