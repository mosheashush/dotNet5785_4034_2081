using DalApi;
using DO;
namespace Dal;

public class CallImplementation : ICall
{
    public void Create(Call item)
    {
        if (Read(item.Id) == null)
        {
            item = item.WithId(Config.NextCallId);
            DataSource.Calls.Add(item);
        }
        else
        {
            throw new Exception($"Call already ID={item.Id} exists");
        }
    }
    public void Update(Call item)
    {
        if(Read(item.Id) != null)
        {
            DataSource.Calls.Remove(Read(item.Id));
            DataSource.Calls.Add(item);
        }
        else
        {
            throw new Exception($"Call does ID={item.Id} not exist");
        }
    }
    public void Delete(int id)
    {
        Call? call = DataSource.Calls.Find(c => c.Id == id);
        if (call != null)
        {
            DataSource.Calls.Remove(call);
        }
        else
        {
            throw new Exception($"Call does ID={id} not exist");
        }
    }
    public Call Read(int id)
    {
        Call? call = DataSource.Calls.Find(c => c.Id == id);
        if (call != null)
        {
            return call;
        }
        else
        {
            return null;
        }
    }
    public List<Call> ReadAll()
    {
        List<Call> calls = DataSource.Calls;
        return calls;
    }
    public void DeleteAll()
    {
        DataSource.Calls.Clear();
    }
}
