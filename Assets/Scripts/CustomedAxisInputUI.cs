using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SimpleInputNamespace
{
    public class CustomedAxisInputUI : AxisInputUI
    {
        private void OnDisable()
        {
            // FIXME: Temporary fix 
            // Reset axis when UI is disabled
            axis.value = 0f;
            axis.StopTracking();
        }
    }
}


