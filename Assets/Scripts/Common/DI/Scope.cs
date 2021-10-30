using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Common.DI
{
    /**
     * Контейнер для хранения зависимостей и их обьектов (singleton'ов).
     */
    public class Scope
    {
        /**
         * Контейнер зависимостей глобальный на все приложение - низкий приоритет поиска зависимости.
         */
        public static readonly Scope Global = new Scope();
        
        /**
         * Контейнер на игровые сцены - средний приоритет поиска зависимости.
         */
        public static readonly Scope Game = new Scope();
        
        /**
         * Контейнер на одну сцену - высший приоритет поиска зависимости.
         */
        public static readonly Scope Scene = new Scope();
        
        private readonly IDictionary<Type, Definition> definitions = new Dictionary<Type, Definition>();
        private readonly IDictionary<Type, object> singletons = new Dictionary<Type, object>();

        /**
         * Старт игры.
         */
        public static void StartGame()
        {
            Game.Clean();
        }

        /**
         * Финиш игры.
         */
        public static void FinishGame()
        {
            Game.Clean();
        }

        /**
         * Старт новой сцены.
         */
        public static void StartScene()
        {
            Scene.Clean();
        }

        /**
         * Очистка скоупа.
         */
        private void Clean()
        {
            foreach (var singleton in singletons)
            {
                if (singleton.Value is IDestroiable destroiable)
                {
                    destroiable.OnDestroyInstance();
                }
            }
            
            singletons.Clear();
            definitions.Clear();
        }
        
        /**
         * Обьявить реализацию зависимости, по-умолчанию singleton.
         */
        public Definition Bind<T>()
        {
            if (definitions.ContainsKey(typeof(T)))
            {
                throw new Exception($"Type {typeof(T).FullName} already binded");
            }
            
            return definitions[typeof(T)] = new Definition();
        }

        /**
         * Объявлена ли зависимость
         */
        public bool IsBinded<T>()
        {
            return definitions.ContainsKey(typeof(T));
        }
        
        /**
         * Вернуть экземпляр зависимости.
         */
        public T Get<T>()
        {
            var type = typeof(T);

            if (definitions.TryGetValue(type, out var def))
            {
                switch (def.Context)
                {
                    case Context.SINGLETON:
                        if (def.fromMethod != null)
                        {
                            return (T)singletons.GetOrInstantiate(type, type1 => def.fromMethod.Invoke());
                        }
                        
                        return (T)singletons.GetOrInstantiate(
                            type, 
                            type1 => def.implementType == null ? Activator.CreateInstance(typeof(T)) : (T)Activator.CreateInstance(def.implementType)
                        );
                    
                    case Context.PROTOTYPE:
                        if (def.fromMethod != null) return (T)def.fromMethod.Invoke();

                        if (def.implementType != null) return (T)Activator.CreateInstance(def.implementType);
                        
                        return (T)Activator.CreateInstance(typeof(T));
                }
            }

            throw new Exception($"Type {typeof(T).FullName} is not binded in scope order to Get<T>()");
        }

        /**
         * Класс для описания того, как создавать обьекты зависимости.
         */
        public class Definition
        {
            public Context Context => context;

            private Context context = Context.SINGLETON;
            internal Func<object> fromMethod;
            internal Type implementType;

            public Definition AsPrototype()
            {
                context = Context.PROTOTYPE;
                return this;
            }

            public Definition To<T>()
            {
                implementType = typeof(T);
                return this;
            }

            public Definition FromMethod(Func<object> func)
            {
                fromMethod = func;
                return this;
            }

            public Definition FromInstance(object obj)
            {
                context = Context.SINGLETON;
                return FromMethod(() => obj);
            }
        }
        
        /**
         * Контекст, в рамках которого живет обьект зависимости.
         */
        public enum Context
        {
            SINGLETON, // одиночка
            PROTOTYPE // постоянно новый обьект
        }
    }
}