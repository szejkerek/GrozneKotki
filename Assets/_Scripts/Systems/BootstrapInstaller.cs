using UnityEngine;
using Zenject;

public class BootstrapInstaller : MonoInstaller
{
    [SerializeField] private AudioManager audioManager;
    [SerializeField] private SceneController sceneController;
    [SerializeField] private GhostRunManager ghostRunManager;

    public override void InstallBindings()
    {
        Container.Bind<AudioManager>()
            .FromInstance(audioManager)
            .AsSingle();

        Container.Bind<SceneController>()
            .FromInstance(sceneController)
            .AsSingle();

        Container.Bind<GhostRunManager>()
            .FromInstance(ghostRunManager)
            .AsSingle();
    }
}