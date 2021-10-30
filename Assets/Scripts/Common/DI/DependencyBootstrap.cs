using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace Common.DI
{
    /**
     * Класс, экземпляр которого (обьект на сцене) должен существовать в единственном виде в каждой сцене.
     *
     *      Лучше всего сделать prefab с этим обьектом, с едиными настройками и добавить сразу на все сцены.
     *      - На игровых сценах не забыть поставить опцию gameScene = true.
     *      - На события навесить биндинг зависимостей, см. методы Scope.Bind<>() и т.п.
     *      - На событие Launch - общие зависимости для всего проекта.
     *      - На событие StartGame - зависимости, которые нужны только когда играется игра.
     *      - На событие StartScene - зависимости, которые нужны в каждой сцене с нуля (пересоздавать).
     */
    public class DependencyBootstrap : MonoBehaviour
    {
        [SerializeField] private bool gameScene;

        [Title("Events")] 
        [SerializeField] private UnityEvent onLaunch = new UnityEvent();
        [SerializeField] private UnityEvent onStartGame = new UnityEvent();
        [SerializeField] private UnityEvent onStartScene = new UnityEvent();
        
        private static bool launched;
        private static bool inGame;

        private void Awake()
        {
            if (!launched)
            {
                launched = true;
                WhenStartUp();
            }

            if (inGame)
            {
                inGame = false;
                WhenFinishGame();
            }
            else
            {
                if (gameScene)
                {
                    inGame = true;
                    WhenStartGame();
                }
            }
            
            WhenStartScene();
        }

        private void WhenStartUp()
        {
            onLaunch?.Invoke();
        }

        private void WhenStartGame()
        {
            Scope.StartGame();
            onStartGame?.Invoke();
        }

        private void WhenFinishGame()
        {
            Scope.FinishGame();
        }

        private void WhenStartScene()
        {
            Scope.StartScene();
            onStartScene?.Invoke();
        }
    }
}
