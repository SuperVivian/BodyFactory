﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreeCamera : MonoBehaviour {

    //观察目标  
    public Transform Target;
    public Transform LookAt;
    public Vector3 cameraPos;
    public float distance;
    //鼠标旋转
    float mX, mY;
    private float SpeedX = 240;
    private float SpeedY = 120;
    public  float MinLimitY =0;
    public  float MaxLimitY = 70;

    //鼠标缩放
    public  float MaxDistance = 10;
    public  float MinDistance =2.5F;
    public  float ZoomSpeed = 4F;
    Vector3 newPosition = Vector3.zero;

    //遮挡住主角的物体
    public  List<Transform> lastHitObjs;
    public  List<Transform> hitObjs;
    public  LayerMask layermasks = -1;

    void Start () {
        mX = transform.eulerAngles.x;
        mY = transform.eulerAngles.y;
        lastHitObjs = new List<Transform>();
        hitObjs = new List<Transform>();
    }	

	void Update () {

        if (Target == null || LookAt == null) return;
        if (Target.GetComponent<Hair_PlayerMove>().animPaused) return;//动画静止

        AvoidViewBlock();
        ControlCamera();
    }

    void ControlCamera()
    {
        //锁定相机中心
        transform.LookAt(LookAt.transform.position);

        //鼠标右键旋转
        if (Input.GetMouseButton(1) || Input.GetMouseButton(0))
        {
            mX += Input.GetAxis("Mouse X") * SpeedX * 0.02F;
            mY -= Input.GetAxis("Mouse Y") * SpeedY * 0.02F;
            mY = ClampAngle(mY, MinLimitY, MaxLimitY);
            //将玩家转到和相机对应的位置上
            if (Input.GetMouseButton(1))
            {
                Target.eulerAngles = new Vector3(0, mX, 0);
            }
        }

        //鼠标滚轮缩放  
        distance += Input.GetAxis("Mouse ScrollWheel") * ZoomSpeed;
        distance = Mathf.Clamp(distance, MinDistance, MaxDistance);

        //计算相机位置和角度  
        Quaternion mRotation = Quaternion.Euler(mY, mX, 0);
        Vector3 mPosition = mRotation * new Vector3(cameraPos.x, cameraPos.y, -distance) + Target.position;

        //设置相机的角度和位置      
        transform.rotation = mRotation;
        transform.position = mPosition;
    }

    private float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360) angle += 360;
        if (angle > 360) angle -= 360;
        return Mathf.Clamp(angle, min, max);
    }    

    void AvoidViewBlock()
    {
        Vector3 ori = Target.position+Vector3.up*0.2f;
        Vector3 ori2 = Target.position - Vector3.up * 0.2f;
        Vector3 dir = (transform.position-Target.position).normalized;

        Debug.DrawRay(ori,dir);
        Debug.DrawRay(ori2, dir);
        RaycastHit[] raycastHits=Physics.RaycastAll(ori,dir,Mathf.Infinity,layermasks);
        RaycastHit[] raycastHits2 = Physics.RaycastAll(ori2, dir, Mathf.Infinity,layermasks);

        hitObjs.Clear();

        for(int i = 0; i < raycastHits.Length; i++)
        {
            if (raycastHits[i].transform.Equals(Target)) continue;
            Transform trans = raycastHits[i].transform;
            if (trans.GetComponent < Renderer >()== null) continue;
            hitObjs.Add(trans);//把除了主角本身以外碰撞到的物体加入到List中              
            InActivateGo(trans);
        }
        
        for (int i = 0; i < raycastHits2.Length; i++)
        {
            if (raycastHits2[i].transform.Equals(Target)) continue;
            Transform trans = raycastHits2[i].transform;
            if (trans.GetComponent<Renderer>() == null) continue;
            if (!hitObjs.Contains(trans))
            {
                hitObjs.Add(trans);//把除了主角本身以外碰撞到的物体加入到List中              
                InActivateGo(trans);
            }
        }

        for (int j = 0; j < hitObjs.Count; j++)
        {
            if (lastHitObjs.Contains(hitObjs[j]))
            {
                lastHitObjs.RemoveAt(lastHitObjs.IndexOf(hitObjs[j]));
            }                                
        }

        for (int i = 0; i < lastHitObjs.Count; i++)
        {
             ActivateGo(lastHitObjs[i]);
        }
        for (int i = 0; i < hitObjs.Count; i++)
        {
            lastHitObjs.Add(hitObjs[i]);//这次的物体移到上次被碰到的List当中去
        }

    }

    void InActivateGo(Transform trans)
    {
        trans.GetComponent<Renderer>().enabled=false;//好像是因为禁用了之后就打不到了
    }
    void ActivateGo(Transform trans)
    {
        trans.GetComponent<Renderer>().enabled = true;
    }

    void SetMaterialAlpha(GameObject go,float alpha)
    {
        Renderer renderer = go.GetComponent<Renderer>();
        int matNum= renderer.sharedMaterials.Length;
        for (int i=0;i<matNum;i++)
        {
            Color _color = renderer.materials[i].color;
            _color.a = alpha;
            renderer.materials[i].SetColor("_Color", _color);
        }
    }
}
