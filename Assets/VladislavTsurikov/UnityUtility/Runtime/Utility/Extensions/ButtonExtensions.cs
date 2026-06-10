using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace VladislavTsurikov.UnityUtility.Runtime.Utility.Extensions
{
    public static class ButtonExtensions
    {
        public static void SimulateClick(this Button button)
        {
            if (button == null || !button.interactable)
            {
                return;
            }

            PointerEventData pointerEventData = new(null) { button = PointerEventData.InputButton.Left };
            button.OnPointerClick(pointerEventData);
        }

        public static void SimulatePointerDown(this Button button)
        {
            if (button == null || !button.interactable)
            {
                return;
            }

            PointerEventData pointerEventData = new(null) { button = PointerEventData.InputButton.Left };
            button.OnPointerDown(pointerEventData);
        }
    }
}
