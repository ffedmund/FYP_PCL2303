using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FYP
{
    public class LeftUpperArmModelChanger : MonoBehaviour
    {
        public List<GameObject> leftUpperArmModels = new List<GameObject>();

        private void Awake() {
            GetAllLeftUpperArmModels();
        }

        private void GetAllLeftUpperArmModels()
        {
            int childrenGameObjects = transform.childCount;

            for (int i = 0; i < childrenGameObjects; i++)
            {
                leftUpperArmModels.Add(transform.GetChild(i).gameObject);
            }
        }

        public void UnEquipAllLeftUpperArmModels()
        {
            foreach (GameObject leftUpperArmModel in leftUpperArmModels)
            {
                leftUpperArmModel.SetActive(false);
            }
        }

        public void EquipLeftUpperArmModelByName(string leftUpperArmName)
        {
            for (int i = 0; i < leftUpperArmModels.Count; i++)
            {
                if (leftUpperArmModels[i].name == leftUpperArmName)
                {
                    leftUpperArmModels[i].SetActive(true);
                }
            }
        }
    }
}
