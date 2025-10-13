using System;
using System.Collections.Generic;
using UnityEngine;

public class FruitsManager : MonoBehaviour {
    [SerializeField] private List<FruitData> possibleFruits = new();
    [SerializeField] private List<Fruit> prefabs = new();

    public static FruitData currentFruit { get; private set; }
    public static FruitData nextFruit { get; private set; }

    public static List<Fruit> fruitPrefabs { get; private set; }

    public static event Action<FruitData, FruitData> OnFruitCycle;

    void Awake() {
        if (possibleFruits == null || possibleFruits.Count == 0) {
            Debug.LogError("FruitsManager: no fruits configured");
            return;
        }

        currentFruit = Pick();
        nextFruit = Pick();
        fruitPrefabs = prefabs;
        DropperController.OnDrop += Cycle;

    }

    private void Cycle() {
        currentFruit = nextFruit;
        nextFruit = Pick();
        OnFruitCycle?.Invoke(currentFruit, nextFruit);
    }

    private FruitData Pick() => possibleFruits[UnityEngine.Random.Range(0, possibleFruits.Count)];
}
