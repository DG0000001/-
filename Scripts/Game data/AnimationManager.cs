using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using System.Data;
using UnityEngine.SocialPlatforms;
using Spine;
using System;
using JetBrains.Annotations;

public class AnimationManager : MonoBehaviour
{
    public string roteFolderPath;

    private SkeletonAnimation skeletonAnimation;
    public SkeletonDataAsset[] skeletonDataAsset;
    private List<AnimationReferenceAsset> animationReferenceAssetsFace = new List<AnimationReferenceAsset>();
    private List<AnimationReferenceAsset> animationReferenceAssetsBack = new List<AnimationReferenceAsset>();
    private List<AnimationReferenceAsset> animationReferenceAssets = new List<AnimationReferenceAsset>();

    public delegate void callb(TrackEntry trackEntry, Spine.Event e);
    private void Start()
    {
        skeletonAnimation = GetComponent<SkeletonAnimation>();
        AddAnimationReferenceAssets(roteFolderPath);
    }

    private void AddAnimationReferenceAssets(string path)//添加动画文件到储存list
    {
        AnimationReferenceAsset[] assets = Resources.LoadAll<AnimationReferenceAsset>(path + "/face/ReferenceAssets");
        animationReferenceAssetsFace.AddRange(assets);
        assets = Resources.LoadAll<AnimationReferenceAsset>(path + "/back/ReferenceAssets");
        animationReferenceAssetsBack.AddRange(assets);
        assets = Resources.LoadAll<AnimationReferenceAsset>(path + "/ReferenceAssets");
        animationReferenceAssets.AddRange(assets);
        if (animationReferenceAssetsFace.Count>0)
        animationReferenceAssets = animationReferenceAssetsFace;
    }
    public void SetSkeletonDataAsset(string whichSide)//设置骨骼文件
    {
        if (whichSide.Equals("Face"))
        {
            if(skeletonAnimation.SkeletonDataAsset!= skeletonDataAsset[0])
            {
                //Debug.Log("SetSkeletonDataAsset-Face");
                skeletonAnimation.ClearState();
                skeletonAnimation.skeletonDataAsset = skeletonDataAsset[0];
                skeletonAnimation.Initialize(true);
                animationReferenceAssets = animationReferenceAssetsFace;
                SetAnimation("Idle", true, 1f);

            }

        }
        else if (whichSide.Equals("Back"))
        {
            if (skeletonAnimation.SkeletonDataAsset != skeletonDataAsset[1])
            {

                //Debug.Log("SetSkeletonDataAsset-Back");
                skeletonAnimation.ClearState();
                skeletonAnimation.skeletonDataAsset = skeletonDataAsset[1];
                skeletonAnimation.Initialize(true);
                animationReferenceAssets = animationReferenceAssetsBack;
                SetAnimation("Idle", true, 1f);
            }
        }
        
    }

    public void SetAnimation(string animationName, bool loop, float timeScale)//设置骨骼对应动画文件
    {
        foreach (var item in animationReferenceAssets)
        {
            //Debug.Log("animationReferenceAssets.name:" + item.name);
        }
        AnimationReferenceAsset animationAsset = animationReferenceAssets.Find(x => (x.name == animationName || x.name == (animationName + "_Loop")));
        if (animationAsset != null)
        {
            Spine.TrackEntry trackEntry = skeletonAnimation.state.SetAnimation(0, animationAsset, loop);
            trackEntry.TimeScale = timeScale;
        }
        else
        {
            Debug.LogError("Animation not found: " + animationName);
        }
    }

    public void SetAnimation(string animationName, bool loop, float timeScale, Character character)//设置骨骼对应动画文件
    {
        foreach (var item in animationReferenceAssets)
        {
           // Debug.Log("animationReferenceAssets.name:" + item.name);
        }
        AnimationReferenceAsset animationAsset = animationReferenceAssets.Find(x => (x.name == animationName || x.name == (animationName + "_Loop"))); 
        if (animationAsset != null)
        {
            Spine.TrackEntry trackEntry = skeletonAnimation.state.SetAnimation(0, animationAsset, loop);
            trackEntry.TimeScale = timeScale;
            trackEntry.Event += character.HandleAnimationStateEvent;
        }
        else
        {
            Debug.LogError("Animation not found: " + animationName);
        }
    }
    public void AddAnimation(string animationName, bool loop, float timeScale, float delay)//设置过度动画
    {
        AnimationReferenceAsset animationAsset = animationReferenceAssets.Find(x => x.name == animationName);
        if (animationAsset != null)
        {
            Spine.TrackEntry trackEntry = skeletonAnimation.state.AddAnimation(0, animationAsset, loop,delay);
            trackEntry.TimeScale = timeScale;
        }
        else
        {
            Debug.LogError("Animation not found: " + animationName);
        }
    }
    //返回当前动画名字
    public string ReturnAnimationName()
    {
        return skeletonAnimation.AnimationName;
    }
}