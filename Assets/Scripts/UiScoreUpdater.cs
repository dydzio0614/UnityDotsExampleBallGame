using TMPro;
using Unity.Entities;
using UnityEngine;

public class UiScoreUpdater : MonoBehaviour
{
    private const string ScoreLabel = "Score";
    
    private TextMeshProUGUI _scoreText;
    private EntityQuery _query;
    
    private void Awake()
    {
        _query = World.DefaultGameObjectInjectionWorld.EntityManager.CreateEntityQuery(ComponentType.ReadOnly<ScorePointsComponent>());
        
        _scoreText = GetComponent<TextMeshProUGUI>();
        _scoreText.text = $"{ScoreLabel}: 0";
    }

    private void Update()
    {
        ScorePointsComponent scorePointsData = _query.GetSingleton<ScorePointsComponent>();
        _scoreText.text = $"{ScoreLabel}: {scorePointsData.Value}";
    }
}
