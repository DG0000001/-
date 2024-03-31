using Spine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Character : MonoBehaviour
{
    public abstract void HandleAnimationStateEvent(TrackEntry trackEntry, Spine.Event e);
}
