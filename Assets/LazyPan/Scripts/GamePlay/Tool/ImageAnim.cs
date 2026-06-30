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

    private Coroutine currentCoroutine;
    private string currentPlaying;

    /// <summary>
    /// 播放动画
    /// </summary>
    /// <param name="anim"></param>
    public void OnPlay(string anim) {
        OnResetAllDefault();
        if (currentCoroutine != null) {
            StopCoroutine(currentCoroutine);
            currentCoroutine = null;
            currentPlaying = null;
        }
        foreach (var tmp in FrameDatas) {
            if (tmp.AnimName == anim) {
                currentCoroutine = StartCoroutine(PlayImage(tmp));
                currentPlaying = anim;
                break;
            }
        }
    }

    IEnumerator PlayImage(FrameData frameDatas) {
        DisableFrame(frameDatas);
        int last = frameDatas.AnimationFrames.Count - 1;
        for (int i = 0; i < frameDatas.AnimationFrames.Count; i++) {
            Image img = frameDatas.AnimationFrames[i];
            Color c = img.color;
            c.a = 1;
            img.color = c;

            if (i == last) {
                yield break;
            }
            yield return new WaitForSeconds(frameDatas.Interval);
            Color cc = img.color;
            cc.a = 0;
            img.color = cc;
        }
    }
    
    /// <summary>
    /// 重置指定默认动画帧
    /// </summary>
    /// <param name="anim"></param>
    public void OnResetAllDefault() {
        foreach (var tmp in FrameDatas) {
            DisableFrame(tmp);
            EnableDefaultFrame(tmp);
        }
    }

    /// <summary>
    /// 重置指定默认动画帧
    /// </summary>
    /// <param name="anim"></param>
    public void OnResetDefault(string anim) {
        foreach (var tmp in FrameDatas) {
            if (tmp.AnimName == anim) {
                DisableFrame(tmp);
                EnableDefaultFrame(tmp);
            }
        }
    }

    /// <summary>
    /// 取消指定所有动画帧
    /// </summary>
    /// <param name="frameDatas"></param>
    private void DisableFrame(FrameData frameDatas) {
        foreach (var tmpImage in frameDatas.AnimationFrames) {
            Color c = tmpImage.color;
            c.a = 0;
            tmpImage.color = c;
        }
    }
    
    /// <summary>
    /// 激活指定默认动画帧
    /// </summary>
    /// <param name="frameDatas"></param>
    private void EnableDefaultFrame(FrameData frameDatas) {
        DisableFrame(frameDatas);
        if (frameDatas.DefaultFrame != null) {
            Color c = frameDatas.DefaultFrame.color;
            c.a = 1;
            frameDatas.DefaultFrame.color = c;
        }
    }
}
