public interface IPooleable<T>
{
    public void GetObjectFromPool();
    public void ReturnToPool(T x);
    public void RefreshPosition(T x);
    public void ObjON(T obj);
    public void ObjOFF(T obj);
}
