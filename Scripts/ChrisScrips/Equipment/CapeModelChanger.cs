using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FYP
{
    public class CapeModelChanger : MonoBehaviour
    {
        public List<GameObject> capeModels = new List<GameObject>();

        private void Awake() {
            GetAllCapeModels();
        }

        private void GetAllCapeModels()
        {
            int childrenGameObjects = transform.childCount;

            for (int i = 0; i < childrenGameObjects; i++)
            {
                capeModels.Add(transform.GetChild(i).gameObject);
            }
        }

        public void UnEquipAllCapeModels()
        {
            foreach (GameObject capeModel in capeModels)
            {
                capeModel.SetActive(false);
            }
        }

        public void EquipCapeModelByName(string capeName)
        {
            for (int i = 0; i < capeModels.Count; i++)
            {
                if (capeModels[i].name == capeName)
                {
                    capeModels[i].SetActive(true);
                }
            }
        }
    }
}
