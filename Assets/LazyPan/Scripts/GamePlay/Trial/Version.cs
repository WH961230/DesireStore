using System;
using UnityEngine;

[Serializable]
public enum VersionType {
    试用版,
    正式版,
}

public class Version : MonoBehaviour {
    [SerializeField] public VersionType versionType = VersionType.试用版;
    
    /// <summary>
    /// 检查是否为正式版（已购买）
    /// </summary>
    public bool IsFullVersion {
        get { return versionType == VersionType.正式版; }
    }
    
    public static Version Instance;
    private void Awake() {
        if (Instance == null) {
            Instance = this;
        }
    }
}
