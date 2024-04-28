using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FYP
{
    public class HeadCoveringsNoFacialHairModelChanger : MonoBehaviour
    {
        PlayerCharacterCustomizationManager playerCharacterCustomizationManager;
        public List<GameObject> headCoveringsNoFacialHairModels = new List<GameObject>();

        private void Awake() {
            playerCharacterCustomizationManager = GetComponentInParent<PlayerCharacterCustomizationManager>();
            GetAllHeadCoveringsNoFacialHairModels();
        }

        private void GetAllHeadCoveringsNoFacialHairModels()
        {
            int childrenGameObjects = transform.childCount;

            for (int i = 0; i < childrenGameObjects; i++)
            {
                headCoveringsNoFacialHairModels.Add(transform.GetChild(i).gameObject);
            }
        }

        public void UnEquipAllHeadCoveringsNoFacialHairModels()
        {
            foreach (GameObject headCoveringsNoFacialHairModel in headCoveringsNoFacialHairModels)
            {
                headCoveringsNoFacialHairModel.SetActive(false);
            }

            if (playerCharacterCustomizationManager.GetCurrentBodyPartModelIndex(PlayerCharacterCustomizationManager.BodyPartType.FacialHair) != 0)
            {
                playerCharacterCustomizationManager.EquipBodyPartModel(PlayerCharacterCustomizationManager.BodyPartType.FacialHair);
            }
        }

        public void EquipHeadCoveringsNoFacialHairModelByName(string headCoveringsNoFacialHairName)
        {
            for (int i = 0; i < headCoveringsNoFacialHairModels.Count; i++)
            {
                if (headCoveringsNoFacialHairModels[i].name == headCoveringsNoFacialHairName)
                {
                    if (playerCharacterCustomizationManager.GetCurrentBodyPartModelIndex(PlayerCharacterCustomizationManager.BodyPartType.FacialHair) != 0)
                    {
                        playerCharacterCustomizationManager.UnequipBodyPartModel(PlayerCharacterCustomizationManager.BodyPartType.FacialHair);
                    }

                    headCoveringsNoFacialHairModels[i].SetActive(true);
                }
            }
        }
    }
}
