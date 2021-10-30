namespace Common.DI
{
    /**
     * Утилитный класс фасад для поиска зависимостей.
     */
    public static class Dependency
    {
        /**
         * Есть ли зависимость, т.е. объявлена + обьект зависимости не равен null
         */
        public static bool Exists<T>()
        {
            if (Scope.Scene.IsBinded<T>() && Scope.Scene.Get<T>() != null) return true;
            if (Scope.Game.IsBinded<T>() && Scope.Game.Get<T>() != null) return true;
            
            return (Scope.Global.IsBinded<T>() && Scope.Global.Get<T>() != null);
        }
        
        /**
         * Напрямую взять экземпляр (объект) зависимости.
         */
        public static T Get<T>()
        {
            if (Scope.Scene.IsBinded<T>()) return Scope.Scene.Get<T>();
            if (Scope.Game.IsBinded<T>()) return Scope.Game.Get<T>();
            
            return Scope.Global.Get<T>();
        }
    }
}