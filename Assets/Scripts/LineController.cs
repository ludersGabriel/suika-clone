using UnityEngine;

public class LineController : MonoBehaviour {
    [SerializeField] private Transform lineStart;
    [SerializeField] private Transform lineEnd;

    private float topPos;
    private float bottomPos;
    private float x;

    private LineRenderer line;

    void Awake() {
        line = GetComponent<LineRenderer>();
    }

    void Update() {
        setLinePos();
    }

    void OnValidate() {
        line = GetComponent<LineRenderer>();

        setLinePos();
    }

    void setLinePos() {
        x = lineStart.position.x;
        topPos = lineStart.position.y;
        bottomPos = lineEnd.position.y;

        line.SetPosition(0, new Vector3(x, topPos));
        line.SetPosition(1, new Vector3(x, bottomPos));
    }
}
