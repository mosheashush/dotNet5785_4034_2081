
namespace DalApi;
using DO;

 public interface ICall
{
    void Create(Call item);
    void Update(Call item);
    void Delete(int id);
    Call Read(int id);
    List<Call> ReadAll();
    void DeleteAll();
}
