using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FYP
{
    public class HeadCoveringsBaseHairModelChanger : MonoBehaviour
    {
        public List<GameObject> headCoveringsBaseHairModels = new List<GameObject>();

        private void Awake() {
            GetAllHeadCoveringsBaseHairModels();
        }

        private void GetAllHeadCoveringsBaseHairModels()
        {
            int childrenGameObjects = transform.childCount;

            for (int i = 0; i < childrenGameObjects; i++)
            {
                headCoveringsBaseHairModels.Add(transform.GetChild(i).gameObject);
            }
        }

        public void UnEquipAllHeadCoveringsBaseHairModels()
        {
            foreach (GameObject headCoveringsBaseHairModel in headCoveringsBaseHairModels)
            {
                headCoveringsBaseHairModel.SetActive(false);
            }
        }

        public void EquipHeadCoveringsBaseHairModelByName(string headCoveringsBaseHairName)
        {
            for (int i = 0; i < headCoveringsBaseHairModels.Count; i++)
            {
                if (headCoveringsBaseHairModels[i].name == headCoveringsBaseHairName)
                {
                    headCoveringsBaseHairModels[i].SetActive(true);
                }
            }
        }
    }
}
