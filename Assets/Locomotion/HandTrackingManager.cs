using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Hands;
using UnityEngine.XR.Management;
using System;

public class HandTrackingManager : MonoBehaviour
{
    public List<HandTrackingDataSettings> dataSettings = new List<HandTrackingDataSettings>();

    private XRHandSubsystem handSubsystem;
    private Dictionary<XRHandJointID, GameObject> spawnedObjDic = new Dictionary<XRHandJointID, GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        GetHandSubsystem();
    }

    // Update is called once per frame
    void Update()
    {
        if (!CheckHandSubsystem())
        {
            return;
        }
        //处理物体与手部关节的同步跟随
        TrackHands();
    }
    private void OnDestroy()
    {
        if (!CheckHandSubsystem())
        {
            return;
        }
        handSubsystem.trackingAcquired -= OnHandTrackingAcquired;
        handSubsystem.trackingLost -= OnHandTrackingLost;
    }
    /// <summary>
    /// 检查有没有HandSubsystem
    /// </summary>
    /// <returns></returns>
    private bool CheckHandSubsystem()
    {
        if (handSubsystem == null)
        {
#if !UNITY_EDITOR
                Debug.LogError("Could not find Hand Subsystem");
#endif
            enabled = false;
            return false;
        }

        return true;
    }
    /// <summary>
    /// 得到HandSubsystem并且进行初始化
    /// </summary>
    private void GetHandSubsystem()
    {
        XRGeneralSettings xrGeneralSettings = XRGeneralSettings.Instance;
        if (xrGeneralSettings == null)
        {
            Debug.LogError("XR general settings not set");
            return;
        }

        XRManagerSettings manager = xrGeneralSettings.Manager;
        if (manager != null)
        {
            XRLoader loader = manager.activeLoader;
            if (loader != null)
            {
                handSubsystem = loader.GetLoadedSubsystem<XRHandSubsystem>();
                if (!CheckHandSubsystem())
                    return;

                handSubsystem.Start();
                handSubsystem.trackingAcquired += OnHandTrackingAcquired; //识别到手势的事件
                handSubsystem.trackingLost += OnHandTrackingLost; //丢失追踪的事件
                //handSubsystem.updatedHands += OnUpdateHands; //更新手部追踪数据的事件
            }
        }
    }



    private void OnUpdateHands(XRHandSubsystem handSubsystem, XRHandSubsystem.UpdateSuccessFlags updateSuccessFlags, XRHandSubsystem.UpdateType updateType)
    {
        //执行自定义逻辑
    }

    private void OnHandTrackingAcquired(XRHand hand)
    {
        foreach (HandTrackingDataSettings handTrackingData in dataSettings)
        {
            //追踪到左手，则显示同步跟随左手的所有物体；右手同理
            if (hand.handedness == handTrackingData.handedness)
            {
                if (spawnedObjDic.TryGetValue(handTrackingData.handJointID, out GameObject trackingObj))
                {
                    trackingObj.SetActive(true);
                    
                }
            }
        }
    }

    private void OnHandTrackingLost(XRHand hand)
    {
        foreach(HandTrackingDataSettings handTrackingData in dataSettings)
        {
            //左手丢失追踪，则隐藏同步跟随左手的所有物体；右手同理
            if (hand.handedness == handTrackingData.handedness)
            {
                if(spawnedObjDic.TryGetValue(handTrackingData.handJointID, out GameObject trackingObj))
                {
                    trackingObj.SetActive(false);
                    
                }
            }
        }
    }
    

    private void TrackHands()
    {
        foreach(HandTrackingDataSettings handTrackingData in dataSettings)
        {
            XRHand hand = default(XRHand);
            if (handTrackingData.handedness == Handedness.Left)
            {
                hand = handSubsystem.leftHand;
            }
            else if (handTrackingData.handedness == Handedness.Right)
            {
                hand = handSubsystem.rightHand;
            }
            //如果HandSubsystem正在运行，处理对应手的物体跟随功能
            if (handSubsystem.running)
            {
                //得到我们定义的手部关节
                XRHandJoint joint = hand.GetJoint(handTrackingData.handJointID);
                if (joint.id == XRHandJointID.Invalid)
                {
                    return;
                }
                GameObject spawnedObj = null;
                //我们在脚本中引用的prefabToSpawn是预制体，一开始还没有在场景当中被创建出来，我们需要先实例化
                if (!spawnedObjDic.ContainsKey(handTrackingData.handJointID))
                {
                    spawnedObj = GameObject.Instantiate(handTrackingData.prefabToSpawn);
                    //把手部关节与生成物体的对应关系存放进字典，下一次直接得到该手部关节上绑定的物体
                    spawnedObjDic.Add(handTrackingData.handJointID, spawnedObj);
                }
                else
                {
                    spawnedObj = spawnedObjDic[handTrackingData.handJointID];
                }
                //处理物体与手部关节的同步跟随
                AttachObjToJoint(spawnedObj, joint);
            }
        }
        
        
    }
    private void AttachObjToJoint(GameObject spawnedObj, XRHandJoint joint)
    {
        if(joint.TryGetPose(out Pose pose))
        {
            //Pose包含关节的位置与旋转数据
            spawnedObj.transform.SetPositionAndRotation(pose.position, pose.rotation);
            
        }
    }
    public XRHandJoint GetSpecificHandJoint(Handedness handedness, XRHandJointID jointID)
    {
        if (!CheckHandSubsystem())
        {
            return default(XRHandJoint);
        }
        else
        {
            XRHand hand = default(XRHand);
            if (handedness == Handedness.Left)
            {
                hand = handSubsystem.leftHand;
            }
            else if (handedness == Handedness.Right)
            {
                hand = handSubsystem.rightHand; 
            }
            if (handSubsystem.running)
            {
                return hand.GetJoint(jointID);
            }
            else
            {
                return default(XRHandJoint);
            }
        }
    }
}
[Serializable]
public struct HandTrackingDataSettings
{
    public Handedness handedness; //哪只手
    public XRHandJointID handJointID; //哪个关节
    public GameObject prefabToSpawn; //该关节处需要生成的物体
}