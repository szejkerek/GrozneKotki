using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GhostRunData
{
    public string sceneName;
    public float duration;
    public List<GhostFrameSample> frames = new();
    public List<GhostSkillUseSample> skillUses = new();
}

[Serializable]
public struct GhostFrameSample
{
    public float time;
    public Vector3 position;
    public Quaternion rotation;
}

[Serializable]
public struct GhostSkillUseSample
{
    public float time;
    public int index;
}