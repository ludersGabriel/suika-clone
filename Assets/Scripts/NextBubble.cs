using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class NextBubble : MonoBehaviour {
    private SpriteRenderer sr;

    void Awake() {
        sr = GetComponent<SpriteRenderer>();
    }

    void Start() {
        sr.sprite = FruitsManager.nextFruit?.Sprite;
    }

    void OnEnable() {
        FruitsManager.OnFruitCycle += HandleFruitCycle;
    }

    void OnDisable() {
        FruitsManager.OnFruitCycle -= HandleFruitCycle;
    }

    private void HandleFruitCycle(FruitData current, FruitData next) {
        sr.sprite = next != null ? next.Sprite : null;
    }
}
