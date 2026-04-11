using System;
using UnityEngine;

public static class PlayerEvents
{
    public static Func<Transform> OnRequestPlayerTransform;
    public static Func<PlayerStateContext> OnRequestPlayerContext;
    public static Func<bool> OnRequestIsPlayerLock;
}