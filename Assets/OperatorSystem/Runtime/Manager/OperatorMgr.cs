using UnityEngine;
using UnityEngine.EventSystems;

namespace Cosmos.OperatorSystem
{
    public interface IOperable
    {
        GameObject gameObject { get; }
        OperatorMgr operatorMgr { get; }
        bool operateLocked { get; set; }
        void OnTap();
        void OnLongTapOnce();
        void OnHold();
        void OnDraging(Vector2 screenposition);
        void OnDragEnd(Vector2 screenposition);
        void OnHoldEnd();
        void OnPointerEnter(GameObject gameObject);
        void OnPointerExit(GameObject gameObject);
    }
    public class OperatorMgr: MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
    {
        public IOperable obj;

        private float _time;
        private bool _hint;
        private bool _drag;
        private bool _hold;
        private Vector2 _firstPointPosition;

        public void OnPointerDown(PointerEventData eventData)
        {
            _hint = true;
            _firstPointPosition = InputManager.GetInputPosion();
        }
        public void OnPointerUp(PointerEventData eventData)
        {
            _hint = false;
            if (_drag)
            {
                OnDragEnd(InputManager.GetInputPosion());
                return;
            }
            if (_time <= 0.2f)
            {
                OnShortTap();
            }
            else if (_time > 0.2f && !_hold)
            {
                OnLongTapOnce();
            }
            if (_hold) OnHoldEnd();
        }
        public void OnPointerEnter(PointerEventData eventData)
        {
            obj.OnPointerEnter(eventData.pointerEnter);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            obj.OnPointerExit(eventData.pointerEnter);
        }
        void FixedUpdate()
        {
            _time = _hint ? _time + Time.fixedDeltaTime : 0;
            if (_hint && (InputManager.GetInputPosion() - _firstPointPosition).magnitude > 2f)
                OnDraging(InputManager.GetInputPosion());
            if (_time > 0.9f && !_drag)
                OnHold();
        }

        private void OnShortTap()
        {
            obj.OnTap();
        }
        private void OnLongTapOnce()
        {
            obj.OnLongTapOnce();
        }
        private void OnHold()
        {
            _hold = true;
            obj.OnHold();
        }
        private void OnHoldEnd()
        {
            _hold = false;
            obj.OnHoldEnd();
        }
        private void OnDraging(Vector2 vector2)
        {
            _drag = true;
            obj.OnDraging(vector2);
        }
        private void OnDragEnd(Vector2 vector2)
        {
            _drag = false;
            obj.OnDragEnd(vector2);
        }
    }
}
