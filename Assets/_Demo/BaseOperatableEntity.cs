using System.Collections;
using System.Collections.Generic;
using Cosmos.Unity;
using UnityEngine;
using Cosmos.OperatorSystem;

[RequireComponent(typeof(OperatorMgr))]
[RequireComponent(typeof(LineRenderer))]
[RequireComponent(typeof(GuideMgr))]
public abstract class BaseOperatableEntity : MonoBehaviour, IGuidable, IOperable
{
    void Awake()
    {
        operatorMgr.obj = this;
        guideMgr.obj = this;
    }

    public virtual GuideMgr guideMgr => GetComponent<GuideMgr>();
    public virtual LineRenderer lineRenderer => GetComponent<LineRenderer>();
    public virtual OperatorMgr operatorMgr => GetComponent<OperatorMgr>();
    public virtual bool operateLocked { get; set; }
    public abstract GameObject ArrowPrefab { get; }
    public abstract Transform GuideTransform { get; }

    public virtual void OnDragEnd(Vector2 screenposition)
    {
        guideMgr.ClearLines();
    }
    public virtual void OnDraging(Vector2 screenposition)
    {
        var position = Camera.main.ScreenToWorldPoint(screenposition);
        guideMgr.DrawSimpleLine(GetComponent<RectTransform>().position, position);
    }
    public virtual void OnHold() { }
    public virtual void OnHoldEnd() { }
    public virtual void OnLongTapOnce() { }
    public virtual void OnPointerEnter(GameObject gameObject) { }
    public virtual void OnPointerExit(GameObject gameObject) { }
    public virtual void OnTap() { }
}
