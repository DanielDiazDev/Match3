

using ScriptableObjects.Level;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI.Level
{
    public class LevelButtonUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text _levelNameText;
        [SerializeField] private Button _button;
        private LevelSO _levelSO;

        public void Setup(LevelSO levelSO, bool isUnlocked, Action<LevelSO> onSelected)
        {
            _levelSO = levelSO;
            _levelNameText.text = _levelSO.levelID.ToString();
            _button.interactable = isUnlocked;
            _button.onClick.AddListener(() => onSelected?.Invoke(_levelSO));
        }

        private void OnClick()
        {
            SceneManager.LoadScene("Game");
        }
    }
}
