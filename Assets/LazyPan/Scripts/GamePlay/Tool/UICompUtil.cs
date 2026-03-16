using TMPro;
using UnityEngine;

namespace LazyPan {
    /// <summary>
    /// 与 UI 结构和 Comp 查找相关的通用工具方法。
    /// </summary>
    public static class UICompUtil {
        /// <summary>
        /// 销毁指定父物体下的所有子物体。
        /// 仅影响层级结构和实例，不做额外逻辑。
        /// </summary>
        /// <param name="parent">要清空子物体的父 Transform。</param>
        public static void ClearChildren(Transform parent) {
            if (parent == null) {
                return;
            }

            foreach (Transform tmp in parent) {
                GameObject.Destroy(tmp.gameObject);
            }
        }

        /// <summary>
        /// 在指定 Comp 下按约定名称优先读取 TMP_InputField 的文本，
        /// 若不存在输入框则退回 TextMeshProUGUI 文本。
        /// </summary>
        /// <param name="parent">要查找子节点的父 Comp。</param>
        /// <param name="sign">用于 Cond 查找的标识名。</param>
        /// <returns>返回读取到的文本，若未找到则返回空字符串。</returns>
        public static string GetInputOrText(Comp parent, string sign) {
            TMP_InputField input = Cond.Instance.Get<TMP_InputField>(parent, sign);
            if (input != null) {
                return input.text;
            }

            TextMeshProUGUI text = Cond.Instance.Get<TextMeshProUGUI>(parent, sign);
            if (text != null) {
                return text.text;
            }

            return string.Empty;
        }
    }
}

