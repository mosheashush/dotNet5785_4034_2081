
using DO;
namespace DalApi
{
    public interface ICrud<T> where T : class
    {
        void Create(T item);
        void Update(T item);
        void Delete(int id);
        T? Read(int id);
        T? Read(Func<T, bool> filter); // stage 2
        IEnumerable<T>? ReadAll(Func<T , bool>? filter = null);
        void DeleteAll();
    }

}
