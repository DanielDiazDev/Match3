using ScriptableObjects.Level.Objetives;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Level
{
    public class ObjectiveSystem
    {
        private readonly ObjectiveContext _context = new();
        private readonly List<ObjectiveBaseSO> _objectives;
        public event Action<int> OnStarsEarned;

        public ObjectiveSystem(List<ObjectiveBaseSO> objectives)
        {
            _objectives = objectives;
            foreach (var obj in _objectives)
                obj.Initialize(_context);
        }

        public void AddScore(int score) => _context.CurrentScore += score;
        public void GemDestroyed(string gemName)
        {
            if (!_context.GemsDestroyed.ContainsKey(gemName))
                _context.GemsDestroyed[gemName] = 0;
            _context.GemsDestroyed[gemName]++;
        }
        public void ObstacleDestroyed() => _context.ObstaclesDestroyed++;

        // cuenta cuántos objetivos se han completado
        public int GetCompletedObjectivesCount() =>
            _objectives.Count(o => o.IsCompleted(_context));
        public bool AllObjectivesIsCompleted() => _objectives.All(o => o.IsCompleted(_context));

        // total de objetivos del nivel
        public int GetTotalObjectivesCount() => _objectives.Count;

        // cuántas estrellas obtuvo (1 por objetivo cumplido)
        public int GetStarCount() => GetCompletedObjectivesCount();

        //  nivel completado si se cumplió al menos 1 objetivo
        public bool IsLevelComplete() => GetCompletedObjectivesCount() > 0;
        public void SetLevelComplete() => OnStarsEarned?.Invoke(GetStarCount());

        public ObjectiveContext Context => _context;
    }
}