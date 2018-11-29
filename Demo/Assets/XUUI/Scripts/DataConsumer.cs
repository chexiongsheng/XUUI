namespace XUUI
{
    public interface DataConsumer
    {

    }

    public interface DataConsumer<T> : DataConsumer
    {
        T Value { set; }
    }
}
