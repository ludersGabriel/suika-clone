// KillZone.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class KillZone : MonoBehaviour {
    [SerializeField] private float dwellSeconds = 3f;

    private readonly HashSet<Collider2D> inside = new();
    private readonly Dictionary<Collider2D, Coroutine> timers = new();

    void OnTriggerEnter2D(Collider2D col) {
        if (inside.Add(col) && !timers.ContainsKey(col)) {
            timers[col] = StartCoroutine(DwellAndCheck(col));
        }
    }

    void OnTriggerExit2D(Collider2D col) {
        inside.Remove(col);
        if (timers.TryGetValue(col, out var co)) {
            StopCoroutine(co);
            timers.Remove(col);
        }
    }

    IEnumerator DwellAndCheck(Collider2D col) {
        yield return new WaitForSecondsRealtime(dwellSeconds);
        timers.Remove(col);
        if (inside.Contains(col)) {
            GameManager.TriggerGameOver();
        }
    }

    void OnDisable() {
        foreach (var co in timers.Values) StopCoroutine(co);
        timers.Clear();
        inside.Clear();
    }
}
