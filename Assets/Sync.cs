using UnityEngine;
using UnityEngine.UI;

public class Sync : MonoBehaviour {
    public Slider Target;

    public bool SyncFillAmount;
    public bool SyncAlpha;

    private Image _image;
    // Start is called before the first frame update
    void Start() {
        _image = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update() {
        if (SyncFillAmount) {
            _image.fillAmount = Target.value;
        }

        if (SyncAlpha) {
            Color c = _image.color;
            c.a = Target.value;
            _image.color = c;
        }
    }
}
