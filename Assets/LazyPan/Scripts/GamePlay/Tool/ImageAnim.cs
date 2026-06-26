using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class FrameData {
    public string AnimName;
    public Image DefaultFrame;
    public float Interval;
    public List<Image> AnimationFrames;
}

public class ImageAnim : MonoBehaviour {
    public List<FrameData> FrameDatas;

    public void OnPlay(string anim) {
        foreach (var tmp in FrameDatas) {
            if (tmp.AnimName == anim) {
                OnReset(tmp.AnimName);
                StartCoroutine("PlayImage", tmp);
                break;
            }
        }
    }

    IEnumerator PlayImage(FrameData frameDatas) {
        foreach (var tmpImage in frameDatas.AnimationFrames) {
            Color c = tmpImage.color;
            c.a = 1;
            tmpImage.color = c;
            yield return new WaitForSeconds(frameDatas.Interval);
            DisableAllFrame(frameDatas);
        }
        EnableDefaultFrame(frameDatas);
    }

    /// <summary>
    /// 重置
    /// </summary>
    /// <param name="anim"></param>
    public void OnReset(string anim) {
        foreach (var tmp in FrameDatas) {
            if (tmp.AnimName == anim) {
                DisableAllFrame(tmp);
                EnableDefaultFrame(tmp);
            }
        }
    }

    private void DisableAllFrame(FrameData frameDatas) {
        foreach (var tmpImage in frameDatas.AnimationFrames) {
            Color c = tmpImage.color;
            c.a = 0;
            tmpImage.color = c;
        }
    }
    
    private void EnableDefaultFrame(FrameData frameDatas) {
        Color c = frameDatas.DefaultFrame.color;
        c.a = 1;
        frameDatas.DefaultFrame.color = c;
    }
}
