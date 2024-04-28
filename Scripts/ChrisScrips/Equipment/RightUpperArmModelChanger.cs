using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FYP
{
    public class RightUpperArmModelChanger : MonoBehaviour
    {
        public List<GameObject> rightUpperArmModels = new List<GameObject>();

        private void Awake() {
            GetAllRightUpperArmModels();
        }

        private void GetAllRightUpperArmModels()
        {
            int childrenGameObjects = transform.childCount;

            for (int i = 0; i < childrenGameObjects; i++)
            {
                rightUpperArmModels.Add(transform.GetChild(i).gameObject);
            }
        }

        public void UnEquipAllRightUpperArmModels()
        {
            foreach (GameObject rightUpperArmModel in rightUpperArmModels)
            {
                rightUpperArmModel.SetActive(false);
            }
        }

        public void EquipRightUpperArmModelByName(string rightUpperArmName)
        {
            for (int i = 0; i < rightUpperArmModels.Count; i++)
            {
                if (rightUpperArmModels[i].name == rightUpperArmName)
                {
                    rightUpperArmModels[i].SetActive(true);
                }
            }
        }
    }
}
