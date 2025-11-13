using ScriptableObjects.Level;
using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public LevelSO CurrentLevelSO { get; private set; }
    public event Action<int> OnMoveLeftChanged;
    
    private int _movesLeft;
    public int MovesLeft
    {
        get => _movesLeft;
        private set
        {
            _movesLeft = value;
            OnMoveLeftChanged?.Invoke(_movesLeft);
        }
    }

    public void SetCurrentLevel(LevelSO levelSO)
    {
        CurrentLevelSO = levelSO;
        MovesLeft = levelSO.moveLimit; 
    }
    public bool LimitFinish()
    {
        return MovesLeft <= 0;
    }
    public void UseMove()
    {
        if (MovesLeft > 0)
            MovesLeft--;
    }
    
}
