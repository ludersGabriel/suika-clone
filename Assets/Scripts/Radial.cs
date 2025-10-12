using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems; // for ICanvasElement

[ExecuteAlways]
public class RadialLayout : LayoutGroup, ICanvasElement {
    public float fDistance = 100f;
    [Range(0f, 360f)] public float MinAngle, MaxAngle, StartAngle;

    protected override void OnEnable() { base.OnEnable(); SetDirty(); }
    protected override void OnTransformChildrenChanged() { base.OnTransformChildrenChanged(); SetDirty(); }
    protected override void OnRectTransformDimensionsChange() { base.OnRectTransformDimensionsChange(); SetDirty(); }

    public override void SetLayoutHorizontal() { CalculateRadial(); }
    public override void SetLayoutVertical() { CalculateRadial(); }
    public override void CalculateLayoutInputHorizontal() { }
    public override void CalculateLayoutInputVertical() { }

#if UNITY_EDITOR
    protected override void OnValidate() {
        base.OnValidate();
        if (!this) return;
        SetDirty(); // schedule, do not mutate children here
    }
#endif

    void CalculateRadial() {
        m_Tracker.Clear();

        int n = rectTransform.childCount;
        if (n == 0) return;

        float step, start;
        if (n == 1) { step = 0f; start = (StartAngle != 0f) ? StartAngle : MinAngle + (MaxAngle - MinAngle) * 0.5f; } else { step = (MaxAngle - MinAngle) / (n - 1); start = MinAngle + StartAngle; }

        for (int i = 0; i < n; i++) {
            var child = rectTransform.GetChild(i) as RectTransform;
            if (!child) continue;

            // Only drive anchored position. Do not drive Anchors or Pivot.
            m_Tracker.Add(this, child, DrivenTransformProperties.AnchoredPosition);

            float angle = (start + step * i) * Mathf.Deg2Rad;
            Vector2 pos = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * fDistance;

            child.anchoredPosition = pos;

            // keep z at 0 for UI
            var lp = child.localPosition;
            if (lp.z != 0f) { lp.z = 0f; child.localPosition = lp; }
        }
    }

    // ICanvasElement (lets Canvas rebuild us safely)
    public void Rebuild(CanvasUpdate executing) { if (executing == CanvasUpdate.PostLayout) CalculateRadial(); }
    public void LayoutComplete() { }
    public void GraphicUpdateComplete() { }
    public new bool IsDestroyed() => this == null;

#if UNITY_EDITOR
    [ContextMenu("Center children anchors & pivot (one-time)")]
    void CenterChildrenAnchorsAndPivot() {
        foreach (RectTransform child in rectTransform) {
            child.anchorMin = child.anchorMax = child.pivot = new Vector2(0.5f, 0.5f);
        }
    }
#endif
}
