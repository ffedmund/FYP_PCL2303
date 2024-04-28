using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text.RegularExpressions;

namespace FYP
{
    public class CharacterCustomizationUI : MonoBehaviour
    {
        [SerializeField] private PlayerCharacterCustomizationManager _playerCharacterCustomizationManager;
        [SerializeField] private PlayerCharacterCustomizationManager.BodyPartType bodyPartType;

        [SerializeField] private GameObject _bodyPartPanel;
        [SerializeField] private GameObject _bodyPartRowPrefab;

        [SerializeField] private TextMeshProUGUI[] _bodyPartTexts;
        [SerializeField] private Button[] _bodyPartPreviousButtons;
        [SerializeField] private Button[] _bodyPartNextButtons;
        [SerializeField] private TextMeshProUGUI[] _bodyPartModelIndexTexts;

        [SerializeField] private Button _saveButton;
        [SerializeField] private Button _loadButton;
        [SerializeField] private Button _randomButton;
        [SerializeField] private Button _resetButton;

        [SerializeField] private Button _backButton;
        [SerializeField] private Button _confirmButton;

        private void Start()
        {
            int numberOfBodyParts = Enum.GetNames(typeof(PlayerCharacterCustomizationManager.BodyPartType)).Length;

            _bodyPartTexts = new TextMeshProUGUI[numberOfBodyParts];
            _bodyPartPreviousButtons = new Button[numberOfBodyParts];
            _bodyPartNextButtons = new Button[numberOfBodyParts];
            _bodyPartModelIndexTexts = new TextMeshProUGUI[numberOfBodyParts];

            for (int i = 0; i < numberOfBodyParts; i++)
            {
                int index = i;

                GameObject bodyPartRow = Instantiate(_bodyPartRowPrefab, _bodyPartPanel.transform);
                bodyPartRow.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = Regex.Replace(((PlayerCharacterCustomizationManager.BodyPartType)i).ToString(), "([A-Z])", " $1").Trim();
                bodyPartRow.transform.Find("Previous").GetComponent<Button>().onClick.AddListener(() => OnPreviousButtonClicked(index));
                bodyPartRow.transform.Find("Next").GetComponent<Button>().onClick.AddListener(() => OnNextButtonClicked(index));
                bodyPartRow.transform.Find("Index").GetComponent<TextMeshProUGUI>().text = (_playerCharacterCustomizationManager.GetCurrentBodyPartModelIndex((PlayerCharacterCustomizationManager.BodyPartType)i) + 1) + " / " + _playerCharacterCustomizationManager.GetBodyPartModelsCount((PlayerCharacterCustomizationManager.BodyPartType)i);
            }

            for (int i = 0; i < numberOfBodyParts; i++)
            {
                _bodyPartTexts[i] = _bodyPartPanel.transform.GetChild(i).Find("Text").GetComponent<TextMeshProUGUI>();
                _bodyPartPreviousButtons[i] = _bodyPartPanel.transform.GetChild(i).Find("Previous").GetComponent<Button>();
                _bodyPartNextButtons[i] = _bodyPartPanel.transform.GetChild(i).Find("Next").GetComponent<Button>();
                _bodyPartModelIndexTexts[i] = _bodyPartPanel.transform.GetChild(i).Find("Index").GetComponent<TextMeshProUGUI>();
            }

            _saveButton.onClick.AddListener(OnSaveButtonClicked);
            _loadButton.onClick.AddListener(OnLoadButtonClicked);
            _randomButton.onClick.AddListener(OnRandomButtonClicked);
            _resetButton.onClick.AddListener(OnResetButtonClicked);

            _backButton.onClick.AddListener(OnBackButtonClicked);
            _confirmButton.onClick.AddListener(OnConfirmButtonClicked);

            UpdateAllIndexText();
        }

        private void UpdateAllIndexText()
        {
            for (int i = 0; i < _bodyPartModelIndexTexts.Length; i++)
            {
                _bodyPartModelIndexTexts[i].text = (_playerCharacterCustomizationManager.GetCurrentBodyPartModelIndex((PlayerCharacterCustomizationManager.BodyPartType)i) + 1) + " / " + _playerCharacterCustomizationManager.GetBodyPartModelsCount((PlayerCharacterCustomizationManager.BodyPartType)i);
            }
        }

        private void UpdateIndexText(int index)
        {
            _bodyPartModelIndexTexts[index].text = (_playerCharacterCustomizationManager.GetCurrentBodyPartModelIndex((PlayerCharacterCustomizationManager.BodyPartType)index) + 1) + " / " + _playerCharacterCustomizationManager.GetBodyPartModelsCount((PlayerCharacterCustomizationManager.BodyPartType)index);
        }

        private void OnPreviousButtonClicked(int index)
        {
            bodyPartType = (PlayerCharacterCustomizationManager.BodyPartType)index;

            _playerCharacterCustomizationManager.PreviousBodyPartModel(bodyPartType);

            UpdateIndexText(index);
        }

        private void OnNextButtonClicked(int index)
        {
            bodyPartType = (PlayerCharacterCustomizationManager.BodyPartType)index;

            _playerCharacterCustomizationManager.NextBodyPartModel(bodyPartType);

            UpdateIndexText(index);
        }

        private void OnSaveButtonClicked()
        {
            _playerCharacterCustomizationManager.Save();
        }

        private void OnLoadButtonClicked()
        {
            _playerCharacterCustomizationManager.Load();
            UpdateAllIndexText();
        }

        private void OnRandomButtonClicked()
        {
            _playerCharacterCustomizationManager.Randomize();
            UpdateAllIndexText();
        }

        private void OnResetButtonClicked()
        {
            _playerCharacterCustomizationManager.Reset();
            UpdateAllIndexText();
        }

        private void OnBackButtonClicked()
        {
            _playerCharacterCustomizationManager.Back();
        }

        private void OnConfirmButtonClicked()
        {
            _playerCharacterCustomizationManager.Confirm();
        }
    }
}