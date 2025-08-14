using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cosmos.unity
{
    public interface IGuidable
    {
        GameObject gameObject { get; }
        GuideMgr guideMgr { get; }
        LineRenderer lineRenderer { get; }
        GameObject ArrowPrefab { get; }
        Transform GuideTransform { get; }
    }
    public class GuideMgr : MonoBehaviour
    {
        public IGuidable obj;
        public void DrawSimpleLine(Vector2 pointA, Vector2 pointB)
        {
            obj.lineRenderer.startColor = Color.red;
            obj.lineRenderer.endColor = Color.red;
            obj.lineRenderer.startWidth = 0.02f;
            obj.lineRenderer.endWidth = 0.02f;
            obj.lineRenderer.positionCount = 2;
            obj.lineRenderer.SetPosition(0, pointA);
            obj.lineRenderer.SetPosition(1, pointB);
        }
        public void UIDrawArrow(Vector2 pointA, Vector2 pointB)
        {
            Vector3 screenPointA = Camera.main.WorldToScreenPoint(pointA);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(obj.GuideTransform.GetComponent<RectTransform>(), screenPointA, Camera.main,
                out Vector2 localPointA);
            Vector3 screenPointB = Camera.main.WorldToScreenPoint(pointB);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(obj.GuideTransform.GetComponent<RectTransform>(), screenPointB, Camera.main,
                out Vector2 localPointB);
            var drect = localPointB - localPointA;
            var arrow = obj.GuideTransform.Find("arrow")?.gameObject;
            if (arrow == null)
            {
                arrow = Instantiate(obj.ArrowPrefab);
                arrow.name = "arrow";
                arrow.transform.SetParent(obj.GuideTransform);
                arrow.transform.localPosition = screenPointA;
                arrow.transform.localScale = Vector3.one;
                arrow.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(drect.magnitude, 50);
                float angle = Mathf.Atan2(drect.y, drect.x) * Mathf.Rad2Deg;
                Quaternion targetRotation = Quaternion.Euler(0f, 0f, angle);
                arrow.transform.GetComponent<RectTransform>().rotation = targetRotation;
            }
            else
            {
                arrow.transform.localPosition = localPointA;
                arrow.transform.localScale = Vector3.one;
                arrow.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(drect.magnitude, 50);
                float angle = Mathf.Atan2(drect.y, drect.x) * Mathf.Rad2Deg;
                Quaternion targetRotation = Quaternion.Euler(0f, 0f, angle);
                arrow.transform.GetComponent<RectTransform>().rotation = targetRotation;
            }
            }
        public void ClearLines()
        {
            obj.lineRenderer.positionCount = 0;
        }
    }
}
