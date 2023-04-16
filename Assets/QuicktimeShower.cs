using UnityEngine;
using UnityEngine.UI;


public class QuicktimeShower : MonoBehaviour
{
    [SerializeField] Image img;
    public QuicktimeShower Factory(Transform parent) {
        var instance = Instantiate(this, parent);
        return instance;
    }
    public void Show(Sprite sprite) {
        gameObject.SetActive(true);
        img.sprite = sprite;
    }
    public void Hide() {
        gameObject.SetActive(false);
    }
}
