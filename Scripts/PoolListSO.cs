using UnityEngine;
using System;

[Serializable]
public struct PoolSetting
{
    public PoolableMono poolingObjet;
    public int poolingCount;
}

[CreateAssetMenu(fileName = "PoolSettingSO", menuName = "SO/Pool Setting SO")]
public class PoolListSO : ScriptableObject
{
    public PoolSetting[] poolingSettings;
}
