using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace FYP
{
    public class SelectSliderOnEnable : MonoBehaviour
    {
        public Slider statSlider;

        protected void OnEnable()
        {
            statSlider.Select();
            statSlider.OnSelect(null);
        }
    }
}