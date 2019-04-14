using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SimpleInputNamespace
{
    public class CustomedButtonInputUI : ButtonInputUI
    {
        private void OnDisable()
        {
            // FIXME: Temporary fix 
            // Reset axis when UI is disabled
            button.value = false;
            button.StopTracking();
        }
    }
}