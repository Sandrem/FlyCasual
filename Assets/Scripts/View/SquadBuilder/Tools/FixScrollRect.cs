using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SquadBuilderNS
{
    public class FixScrollRect : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IScrollHandler
    {
        public ScrollRect MainScroll;

        public void OnBeginDrag(PointerEventData eventData)
        {
            MainScroll.OnBeginDrag(eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            MainScroll.OnDrag(eventData);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            MainScroll.OnEndDrag(eventData);
        }

        public void OnScroll(PointerEventData data)
        {
            MainScroll.OnScroll(data);
        }

    }
}
