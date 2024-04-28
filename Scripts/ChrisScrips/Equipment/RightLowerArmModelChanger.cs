using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FYP
{
    public class RightLowerArmModelChanger : MonoBehaviour
    {
        public List<GameObject> rightLowerArmModels = new List<GameObject>();

        private void Awake() {
            GetAllRightLowerArmModels();
        }

        private void GetAllRightLowerArmModels()
        {
            int childrenGameObjects = transform.childCount;

            for (int i = 0; i < childrenGameObjects; i++)
            {
                rightLowerArmModels.Add(transform.GetChild(i).gameObject);
            }
        }

        public void UnEquipAllRightLowerArmModels()
        {
            foreach (GameObject rightLowerArmModel in rightLowerArmModels)
            {
                rightLowerArmModel.SetActive(false);
            }
        }

        public void EquipRightLowerArmModelByName(string rightLowerArmName)
        {
            for (int i = 0; i < rightLowerArmModels.Count; i++)
            {
                if (rightLowerArmModels[i].name == rightLowerArmName)
                {
                    rightLowerArmModels[i].SetActive(true);
                }
            }
        }
    }
}
