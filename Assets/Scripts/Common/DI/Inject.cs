namespace Common.DI
{
    /**
     * Класс для удобного подключения зависимостей в другие классы.
     *     ВНИМАНИЕ: Подключение происходит ленивым способом во время доступа к свойству .Instance
     *
     * Напимер в классе обьявляем приватное поле:
     *
     *     private Inject<YourUniverse> yourUniverse;
     *
     *  Можем обращаться к зависимости так: yourUniverse.Instance.Method() ....
     */
    public struct Inject<T>
    {
        /**
         * Существует ли зависисимость и ее обьект не равен null
         */
        public bool Exists => Dependency.Exists<T>();
        
        /**
         * Экземпляр (обьект) зависимости.
         */
        public T Instance => Dependency.Get<T>();
    }
}