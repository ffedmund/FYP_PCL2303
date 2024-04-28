using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FYP
{
    public class HeadCoveringsNoHairModelChanger : MonoBehaviour
    {
        PlayerCharacterCustomizationManager playerCharacterCustomizationManager;
        public List<GameObject> headCoveringsNoHairModels = new List<GameObject>();

        private void Awake() {
            playerCharacterCustomizationManager = GetComponentInParent<PlayerCharacterCustomizationManager>();
            GetAllHeadCoveringsNoHairModels();
        }

        private void GetAllHeadCoveringsNoHairModels()
        {
            int childrenGameObjects = transform.childCount;

            for (int i = 0; i < childrenGameObjects; i++)
            {
                headCoveringsNoHairModels.Add(transform.GetChild(i).gameObject);
            }
        }

        public void UnEquipAllHeadCoveringsNoHairModels()
        {
            foreach (GameObject headCoveringsNoHairModel in headCoveringsNoHairModels)
            {
                headCoveringsNoHairModel.SetActive(false);
            }

            if (playerCharacterCustomizationManager.GetCurrentBodyPartModelIndex(PlayerCharacterCustomizationManager.BodyPartType.Hair) != 0)
            {
                playerCharacterCustomizationManager.EquipBodyPartModel(PlayerCharacterCustomizationManager.BodyPartType.Hair);
            }

            if (playerCharacterCustomizationManager.GetCurrentBodyPartModelIndex(PlayerCharacterCustomizationManager.BodyPartType.Ear) != 0)
            {
                playerCharacterCustomizationManager.EquipBodyPartModel(PlayerCharacterCustomizationManager.BodyPartType.Ear);
            }
        }

        public void EquipHeadCoveringsNoHairModelByName(string headCoveringsNoHairName)
        {
            for (int i = 0; i < headCoveringsNoHairModels.Count; i++)
            {
                if (headCoveringsNoHairModels[i].name == headCoveringsNoHairName)
                {
                    if (playerCharacterCustomizationManager.GetCurrentBodyPartModelIndex(PlayerCharacterCustomizationManager.BodyPartType.Hair) != 0)
                    {
                        playerCharacterCustomizationManager.UnequipBodyPartModel(PlayerCharacterCustomizationManager.BodyPartType.Hair);
                    }

                    if (playerCharacterCustomizationManager.GetCurrentBodyPartModelIndex(PlayerCharacterCustomizationManager.BodyPartType.Ear) != 0)
                    {
                        playerCharacterCustomizationManager.UnequipBodyPartModel(PlayerCharacterCustomizationManager.BodyPartType.Ear);
                    }

                    headCoveringsNoHairModels[i].SetActive(true);
                }
            }
        }
    }
}
