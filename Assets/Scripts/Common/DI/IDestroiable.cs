namespace Common.DI
{
    /**
     * Через интерфейс можно отловить, когда зависимость будет уже не нужна.
     *
     *      TODO не работает для context = PROTOTYPE, только для Singleton зависимостей.
     *
     *  Для MonoBehaviour можно юзать вместо этого стандартный OnDestroy метод.
     */
    public interface IDestroiable
    {
        public void OnDestroyInstance();
    }
}