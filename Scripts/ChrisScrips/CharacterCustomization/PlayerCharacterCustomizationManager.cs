using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

namespace FYP
{
    public class PlayerCharacterCustomizationManager : NetworkBehaviour
    {
        PlayerManager playerManager;

        private const string SAVE_KEY = "PlayerCharacterCustomization";

        [SerializeField] private BodyPartData[] BodyPartDataArray;


        public enum BodyPartType
        {
            Hair,
            Ear,
            Head,
            Eyebrows,
            FacialHair,
            Back,
            Accessories,
            // Torso,
            // RightArm,
            // LeftArm,
            // Hips,
            // RightLegs,
            // LeftLegs,
        }

        [Serializable]
        public class BodyPartData
        {
            public BodyPartType bodyPartType;
            public int currentModelIndex;
            public GameObject[] models;
        }

        private void Awake()
        {
            for (int i = 0; i < BodyPartDataArray.Length; i++)
            {
                BodyPartDataArray[i].currentModelIndex = 0;
            }

            // if (PlayerPrefs.HasKey(SAVE_KEY))
            // {
            //     Debug.Log("Load in Awake");
            //     Load();
            // }

            playerManager = GetComponentInParent<PlayerManager>();
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            for (int i = 0; i < BodyPartDataArray.Length; i++)
            {
                BodyPartDataArray[i].currentModelIndex = 0;
            }

            if (playerManager.IsOwner)
            {
                if (PlayerPrefs.HasKey(SAVE_KEY))
                {
                    Debug.Log("Owner Load");
                    Load();
                }
            }
            else
            {
                Debug.Log("Not Owner");
            }
        }

        public int GetCurrentBodyPartModelIndex(BodyPartType bodyPartType)
        {
            return BodyPartDataArray[(int)bodyPartType].currentModelIndex;
        }

        public int GetBodyPartModelsCount(BodyPartType bodyPartType)
        {
            return BodyPartDataArray[(int)bodyPartType].models.Length;
        }

        public void PreviousBodyPartModel(BodyPartType bodyPartType)
        {
            int currentModelIndex = GetCurrentBodyPartModelIndex(bodyPartType);

            GameObject currentModel = BodyPartDataArray[(int)bodyPartType].models[currentModelIndex];
            if (currentModel != null)
            {
                currentModel.SetActive(false);
            }

            GameObject previousModel = BodyPartDataArray[(int)bodyPartType].models[(currentModelIndex - 1 + BodyPartDataArray[(int)bodyPartType].models.Length) % BodyPartDataArray[(int)bodyPartType].models.Length];
            if (previousModel != null)
            {
                previousModel.SetActive(true);
            }

            BodyPartDataArray[(int)bodyPartType].currentModelIndex = (currentModelIndex - 1 + BodyPartDataArray[(int)bodyPartType].models.Length) % BodyPartDataArray[(int)bodyPartType].models.Length;
        }

        public void NextBodyPartModel(BodyPartType bodyPartType)
        {
            int currentModelIndex = GetCurrentBodyPartModelIndex(bodyPartType);

            GameObject currentModel = BodyPartDataArray[(int)bodyPartType].models[currentModelIndex];
            if (currentModel != null)
            {
                currentModel.SetActive(false);
            }

            GameObject nextModel = BodyPartDataArray[(int)bodyPartType].models[(currentModelIndex + 1) % BodyPartDataArray[(int)bodyPartType].models.Length];
            if (nextModel != null)
            {
                nextModel.SetActive(true);
            }

            BodyPartDataArray[(int)bodyPartType].currentModelIndex = (currentModelIndex + 1) % BodyPartDataArray[(int)bodyPartType].models.Length;
        }

        public void UnequipBodyPartModel(BodyPartType bodyPartType)
        {
            int currentModelIndex = GetCurrentBodyPartModelIndex(bodyPartType);

            GameObject currentModel = BodyPartDataArray[(int)bodyPartType].models[currentModelIndex];
            if (currentModel != null)
            {
                currentModel.SetActive(false);
            }
        }

        public void EquipBodyPartModel(BodyPartType bodyPartType)
        {
            int currentModelIndex = GetCurrentBodyPartModelIndex(bodyPartType);

            GameObject currentModel = BodyPartDataArray[(int)bodyPartType].models[currentModelIndex];
            if (currentModel != null)
            {
                currentModel.SetActive(true);
            }
        }

        public void Randomize()
        {
            foreach (BodyPartData bodyPartData in BodyPartDataArray)
            {
                int randomModelIndex = UnityEngine.Random.Range(0, bodyPartData.models.Length);

                GameObject currentModel = bodyPartData.models[bodyPartData.currentModelIndex];
                if (currentModel != null)
                {
                    currentModel.SetActive(false);
                }

                GameObject randomModel = bodyPartData.models[randomModelIndex];
                if (randomModel != null)
                {
                    randomModel.SetActive(true);
                }

                bodyPartData.currentModelIndex = randomModelIndex;
            }
        }


        public void Reset()
        {
            foreach (BodyPartData bodyPartData in BodyPartDataArray)
            {
                GameObject currentModel = bodyPartData.models[bodyPartData.currentModelIndex];
                if (currentModel != null)
                {
                    currentModel.SetActive(false);
                }

                GameObject firstModel = bodyPartData.models[0];
                if (firstModel != null)
                {
                    firstModel.SetActive(true);
                }

                bodyPartData.currentModelIndex = 0;
            }
        }

        public void Back()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
        }

        public void Confirm()
        {
            Save();
            UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
        }

        [Serializable]
        public class BodyPartTypeIndex
        {
            public BodyPartType bodyPartType;
            public int index;
        }

        [Serializable]
        public class SaveObject
        {
            public List<BodyPartTypeIndex> bodyPartTypeIndexList;
        }

        public void Save()
        {
            List<BodyPartTypeIndex> bodyPartTypeIndexList = new List<BodyPartTypeIndex>();

            foreach (BodyPartType bodyPartType in Enum.GetValues(typeof(BodyPartType)))
            {
                BodyPartTypeIndex bodyPartTypeIndex = new BodyPartTypeIndex();
                bodyPartTypeIndex.bodyPartType = bodyPartType;
                bodyPartTypeIndex.index = GetCurrentBodyPartModelIndex(bodyPartType);
                bodyPartTypeIndexList.Add(bodyPartTypeIndex);
            }

            SaveObject saveObject = new SaveObject();
            saveObject.bodyPartTypeIndexList = bodyPartTypeIndexList;

            string saveString = JsonUtility.ToJson(saveObject);
            PlayerPrefs.SetString(SAVE_KEY, saveString);
        }

        public void Load()
        {
            string saveString = PlayerPrefs.GetString(SAVE_KEY);

            SendCharacterCustomizationDataToServerRpc(saveString);

            if (saveString != "")
            {
                SaveObject saveObject = JsonUtility.FromJson<SaveObject>(saveString);

                foreach (BodyPartTypeIndex bodyPartTypeIndex in saveObject.bodyPartTypeIndexList)
                {
                    BodyPartType bodyPartType = bodyPartTypeIndex.bodyPartType;
                    int index = bodyPartTypeIndex.index;

                    GameObject currentModel = BodyPartDataArray[(int)bodyPartType].models[GetCurrentBodyPartModelIndex(bodyPartType)];
                    if (currentModel != null)
                    {
                        currentModel.SetActive(false);
                    }

                    GameObject nextModel = BodyPartDataArray[(int)bodyPartType].models[index];
                    if (nextModel != null)
                    {
                        nextModel.SetActive(true);
                    }

                    BodyPartDataArray[(int)bodyPartType].currentModelIndex = index;
                }
            }
        }

        [Rpc(SendTo.Server)]
        public void SendCharacterCustomizationDataToServerRpc(string saveString)
        {
            ReceiveCharacterCustomizationDataFromClientRpc(saveString);
        }

        [ClientRpc]
        public void ReceiveCharacterCustomizationDataFromClientRpc(string saveString)
        {
            if (saveString != "")
            {
                SaveObject saveObject = JsonUtility.FromJson<SaveObject>(saveString);

                foreach (BodyPartTypeIndex bodyPartTypeIndex in saveObject.bodyPartTypeIndexList)
                {
                    BodyPartType bodyPartType = bodyPartTypeIndex.bodyPartType;
                    int index = bodyPartTypeIndex.index;

                    GameObject currentModel = BodyPartDataArray[(int)bodyPartType].models[GetCurrentBodyPartModelIndex(bodyPartType)];
                    if (currentModel != null)
                    {
                        currentModel.SetActive(false);
                    }

                    GameObject nextModel = BodyPartDataArray[(int)bodyPartType].models[index];
                    if (nextModel != null)
                    {
                        nextModel.SetActive(true);
                    }

                    BodyPartDataArray[(int)bodyPartType].currentModelIndex = index;
                }
            }

            playerManager.playerEquipmentManager.EquipAllEquipmentModelsOnStart();
        }

    }
}