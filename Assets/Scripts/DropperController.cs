// DropperController.cs
using System;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class DropperController : MonoBehaviour {
    private SpriteRenderer sr;
    private bool isDropping;
    private Fruit current;

    public static event Action OnDrop;

    void Awake() {
        sr = GetComponent<SpriteRenderer>();
    }

    void Start() {
        sr.sprite = FruitsManager.currentFruit?.Sprite;
    }

    void OnEnable() {
        FruitsManager.OnFruitCycle += HandleFruitCycle;
    }

    void OnDisable() {
        FruitsManager.OnFruitCycle -= HandleFruitCycle;
        UnsubscribeCurrent();
    }

    void Update() {
        if (!PlayerInputControl.isDropPressed) return;
        if (isDropping) return;

        isDropping = true;
        sr.enabled = false;

        Fruit prefab = FruitsManager.fruitPrefabs[FruitsManager.currentFruit.Id];
        current = Fruit.instantiateFruit(prefab, transform.position);
        current.OnFirstCollision += HandleFruitFirstCollision;

        OnDrop?.Invoke();
    }

    private void HandleFruitFirstCollision(Fruit f) {
        if (current == f) {
            current.OnFirstCollision -= HandleFruitFirstCollision;
            current = null;
        }

        isDropping = false;
        sr.enabled = true;
    }

    private void HandleFruitCycle(FruitData currentFruit, FruitData nextFruit) {
        sr.sprite = currentFruit ? currentFruit.Sprite : null;
    }

    private void UnsubscribeCurrent() {
        if (current != null) current.OnFirstCollision -= HandleFruitFirstCollision;
        current = null;
    }
}
