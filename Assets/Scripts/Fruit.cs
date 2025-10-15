// Fruit.cs
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(Rigidbody2D), typeof(Collider2D))]
public class Fruit : MonoBehaviour {
    [SerializeField] private FruitData fruitData;

    public event System.Action<Fruit> OnFirstCollision; // instance-scoped

    private bool isBusy;
    private bool firstCollision = true;
    private Rigidbody2D rb;
    private int sessionId;

    void Awake() {
        rb = GetComponent<Rigidbody2D>();
    }

    public static Fruit instantiateFruit(Fruit prefab, Vector3 position, Vector3 initialVelocity = default) {
        DropperController dropper = FindObjectsByType<DropperController>(FindObjectsInactive.Include, FindObjectsSortMode.None)[0];

        Fruit instance = Instantiate(prefab, position, Quaternion.identity);
        instance.sessionId = SessionManager.Next();
        instance.rb.linearVelocity = initialVelocity;
        instance.transform.parent = dropper.transform;

        return instance;
    }

    void OnCollisionEnter2D(Collision2D collision) {
        Player p = collision.collider.GetComponent<Player>();
        if (p) return;

        if (firstCollision) {
            firstCollision = false;
            OnFirstCollision?.Invoke(this);
            return;
        }

        Fruit other = collision.collider.GetComponent<Fruit>();
        if (!other) return;
        if (other.fruitData.Id != fruitData.Id) return;
        if (other.sessionId < sessionId) return;
        if (isBusy || other.isBusy) return;

        isBusy = true;
        other.isBusy = true;

        if (fruitData.Id + 1 < FruitsManager.fruitPrefabs.Count) {
            Fruit spawnPrefab = FruitsManager.fruitPrefabs[fruitData.Id + 1];
            Vector3 avgPos = (transform.position + other.transform.position) * 0.5f;
            Vector3 avgVel = (rb.linearVelocity + other.rb.linearVelocity) * 0.5f;
            instantiateFruit(spawnPrefab, avgPos, avgVel);

            GameManager.AddScore(fruitData.Points);
        } else GameManager.AddScore(fruitData.Points);

        AudioManager.I.PlaySFX("pop");
        Destroy(other.gameObject);
        Destroy(gameObject);
    }
}
