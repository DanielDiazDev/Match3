

using Level;
using ScriptableObjects.Level;
using System;
using Systems;
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
        [SerializeField] private Image[] _stars;
        [SerializeField] private Sprite _fullStar;
        private LevelSO _levelSO;

        public void Setup(LevelSO levelSO, bool isUnlocked, Action<LevelSO> onSelected)
        {
            _levelSO = levelSO;
            _levelNameText.text = _levelSO.levelID.ToString();
            _button.interactable = isUnlocked;
            _button.onClick.AddListener(() => onSelected?.Invoke(_levelSO));
            var starsEarned = ServiceLocator.Instance.Get<ILevelProgress>().GetStars(_levelSO.levelID.ToString());

            for(var i = 0; i < _stars.Length; i++)
            {
                if(i < starsEarned)
                {
                    _stars[i].sprite = _fullStar;
                }
            }
        }

        private void OnClick()
        {
            SceneManager.LoadScene("Game");
        }
    }
}
