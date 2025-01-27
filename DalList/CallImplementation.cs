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
            throw new InvalidOperationException("Call already exists");
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
            throw new InvalidOperationException("Call does not exist");
        }
    }
    public void Delete(int id)
    {
        if (Read(id) != null)
        {
            DataSource.Calls.Remove(Read(id));
        }
        else
        {
            throw new InvalidOperationException("Call does not exist");
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
            throw new InvalidOperationException("Call does not exist");
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
