using DalApi;
using DO;
namespace Dal;

internal class CallImplementation : ICall
{
    public void Create(Call item)
    {
        Call call = DataSource.Calls.FirstOrDefault(c => c.Id == item.Id);  
        if (Read(call.Id) == null)
        {
            int index = DataSource.Calls.FindIndex(c => c.Id == item.Id);
            DataSource.Calls[index] = item;
        }
        

            
    }
    public void Update(Call item)
    {
        throw new NotImplementedException();


    }
    public void Delete(int id)
    {
        throw new NotImplementedException();
    }
    public Call Read(int id)
    {
        throw new NotImplementedException();
    }
    public List<Call> ReadAll()
    {
        throw new NotImplementedException();
    }
    public void DeleteAll()
    {
        throw new NotImplementedException();
    }
}
