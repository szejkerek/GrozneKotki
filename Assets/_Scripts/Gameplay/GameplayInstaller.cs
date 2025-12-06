using UnityEngine;
using Zenject;

public class GameplayInstaller : MonoInstaller
{
    [SerializeField] private EnemySpawner spawner;

    public override void InstallBindings()
    {
        Container.Bind<EnemySpawner>()
            .FromInstance(spawner)
            .AsSingle();
    }
}