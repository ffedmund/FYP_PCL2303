using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FYP
{
    public class LeftLowerArmModelChanger : MonoBehaviour
    {
        public List<GameObject> leftLowerArmModels = new List<GameObject>();

        private void Awake() {
            GetAllLeftLowerArmModels();
        }

        private void GetAllLeftLowerArmModels()
        {
            int childrenGameObjects = transform.childCount;

            for (int i = 0; i < childrenGameObjects; i++)
            {
                leftLowerArmModels.Add(transform.GetChild(i).gameObject);
            }
        }

        public void UnEquipAllLeftLowerArmModels()
        {
            foreach (GameObject leftLowerArmModel in leftLowerArmModels)
            {
                leftLowerArmModel.SetActive(false);
            }
        }

        public void EquipLeftLowerArmModelByName(string leftLowerArmName)
        {
            for (int i = 0; i < leftLowerArmModels.Count; i++)
            {
                if (leftLowerArmModels[i].name == leftLowerArmName)
                {
                    leftLowerArmModels[i].SetActive(true);
                }
            }
        }
    }
}
