﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "GlobalSettings", menuName = "Settings/GlobalSettings")]
public class GSettings : ScriptableObject
{
    [Header("关卡设置-------------------------------")]

    [Rename("与光源互动前的光源光照范围")]
    public float lightSourceRangeBeforeLightUp;
    [Rename("与光源互动后的光源光照范围")]
    public float lightSourceRangeAfterLightUp;
    [Rename("与光源互动前的玩家自身周边光照范围")]
    public float playerRangeBeforeLightUp;
    [Rename("与光源互动前的玩家自身周边光照范围")]
    public float playerRangeAfterLightUp;



    [Header("玩家属性-------------------------------")]

    [Rename("玩家移速")]
    public float playerSpeed;
    [Rename("玩家跳跃高度")]
    public float playerJumpHeight;
    [Rename("玩家怼墙CD")]
    public float dashCD;
    [Rename("怼墙前摇时长")]
    public float jumpAnimationTimer;
    [Rename("怼墙前摇速度")]
    public float jumpAnimationSpeed;
    [Rename("怼墙前摇转圈圈次数")]
    public float numberOfRotation;
    [Rename("怼墙时冲击速度")]
    public float dashSpeed;
    [Rename("怼墙时冲击距离")]
    public float dashDistance;
    [Rename("怼到墙时横向后坐力")]
    public float horizontalKnockBackForce;
    [Rename("怼到墙时纵向后坐力")]
    public float verticalKnockBackForce;



    [Header("光源属性-------------------------------")]

    [Rename("与玩家间最大距离")]
    public float maxDistanceBetweenPlayer;
    [Rename("光源移速")]
    public float lightSourceMoveSpeed;



    [Header("密码锁-------------------------------")]

    [Rename("密码")]
    public int password;



    [Header("相机-------------------------------")]

    [Rename("第三人称视角灵敏度")]
    public float thirdPersonCameraSensitive;
    [Rename("第三人称摄像机视野")]
    public float cameraFOV;
    [Rename("第二人称视角旋转速度")]
    public float CameraRotateSpeed;
    [Rename("第二人称视角跟随纵向轴偏移")]
    public float YOffset;
    [Rename("第二人称视角跟随横向轴偏移")]
    public float HorizontalOffset;
    [Rename("第二人称摄像机大小")]
    public float cameraSize;



    [Header("HELP谜题-------------------------------")]

    [Rename("触发荧光字母所需时长")]
    public float help_Timer;
    [Rename("触发荧光字母所需精度")]
    public float help_Offset;
}


public class RenameAttribute : PropertyAttribute
{
    public string NewName { get; private set; }
    public RenameAttribute(string name)
    {
        NewName = name;
    }
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(RenameAttribute))]
public class RenameEditor : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.PropertyField(position, property, new GUIContent((attribute as RenameAttribute).NewName));
    }
}
#endif
