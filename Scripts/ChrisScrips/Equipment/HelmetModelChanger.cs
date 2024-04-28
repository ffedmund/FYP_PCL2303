using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FYP
{
    public class HelmetModelChanger : MonoBehaviour
    {
        PlayerCharacterCustomizationManager playerCharacterCustomizationManager;
        public List<GameObject> helmetModels = new List<GameObject>();

        private void Awake() {
            playerCharacterCustomizationManager = GetComponentInParent<PlayerCharacterCustomizationManager>();
            GetAllHelmetModels();
        }

        private void GetAllHelmetModels()
        {
            int childrenGameObjects = transform.childCount;

            for (int i = 0; i < childrenGameObjects; i++)
            {
                helmetModels.Add(transform.GetChild(i).gameObject);
            }
        }

        public void UnEquipAllHelmetModels()
        {
            foreach (GameObject helmetModel in helmetModels)
            {
                helmetModel.SetActive(false);
            }

            if (playerCharacterCustomizationManager.GetCurrentBodyPartModelIndex(PlayerCharacterCustomizationManager.BodyPartType.Head) != 0)
            {
                playerCharacterCustomizationManager.EquipBodyPartModel(PlayerCharacterCustomizationManager.BodyPartType.Head);
            }

            if (playerCharacterCustomizationManager.GetCurrentBodyPartModelIndex(PlayerCharacterCustomizationManager.BodyPartType.Hair) != 0)
            {
                playerCharacterCustomizationManager.EquipBodyPartModel(PlayerCharacterCustomizationManager.BodyPartType.Hair);
            }
            
            if (playerCharacterCustomizationManager.GetCurrentBodyPartModelIndex(PlayerCharacterCustomizationManager.BodyPartType.FacialHair) != 0)
            {
                playerCharacterCustomizationManager.EquipBodyPartModel(PlayerCharacterCustomizationManager.BodyPartType.FacialHair);
            }

            if (playerCharacterCustomizationManager.GetCurrentBodyPartModelIndex(PlayerCharacterCustomizationManager.BodyPartType.Eyebrows) != 0)
            {
                playerCharacterCustomizationManager.EquipBodyPartModel(PlayerCharacterCustomizationManager.BodyPartType.Eyebrows);
            }

            if (playerCharacterCustomizationManager.GetCurrentBodyPartModelIndex(PlayerCharacterCustomizationManager.BodyPartType.Ear) != 0)
            {
                playerCharacterCustomizationManager.EquipBodyPartModel(PlayerCharacterCustomizationManager.BodyPartType.Ear);
            }
            
        }

        public void EquipHelmetModelByName(string helmetName)
        {
            for (int i = 0; i < helmetModels.Count; i++)
            {
                if (helmetModels[i].name == helmetName)
                {
                    if (playerCharacterCustomizationManager.GetCurrentBodyPartModelIndex(PlayerCharacterCustomizationManager.BodyPartType.Head) != 0)
                    {
                        playerCharacterCustomizationManager.UnequipBodyPartModel(PlayerCharacterCustomizationManager.BodyPartType.Head);
                    }

                    if (playerCharacterCustomizationManager.GetCurrentBodyPartModelIndex(PlayerCharacterCustomizationManager.BodyPartType.Hair) != 0)
                    {
                        playerCharacterCustomizationManager.UnequipBodyPartModel(PlayerCharacterCustomizationManager.BodyPartType.Hair);
                    }

                    if (playerCharacterCustomizationManager.GetCurrentBodyPartModelIndex(PlayerCharacterCustomizationManager.BodyPartType.FacialHair) != 0)
                    {
                        playerCharacterCustomizationManager.UnequipBodyPartModel(PlayerCharacterCustomizationManager.BodyPartType.FacialHair);
                    }

                    if (playerCharacterCustomizationManager.GetCurrentBodyPartModelIndex(PlayerCharacterCustomizationManager.BodyPartType.Eyebrows) != 0)
                    {
                        playerCharacterCustomizationManager.UnequipBodyPartModel(PlayerCharacterCustomizationManager.BodyPartType.Eyebrows);
                    }

                    if (playerCharacterCustomizationManager.GetCurrentBodyPartModelIndex(PlayerCharacterCustomizationManager.BodyPartType.Ear) != 0)
                    {
                        playerCharacterCustomizationManager.UnequipBodyPartModel(PlayerCharacterCustomizationManager.BodyPartType.Ear);
                    }          

                    helmetModels[i].SetActive(true);
                }
            }
        }
    }
}
