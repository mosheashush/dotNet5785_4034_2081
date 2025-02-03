using DalApi;
using DO;
namespace Dal;

internal class CallImplementation : ICall
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
            throw new DalAlreadyExistsException($"Call already ID={item.Id} exists");
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
            throw new DalDoesNotExistException($"Call does ID={item.Id} not exist");
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
            throw new DalDoesNotExistException($"Call does ID={id} not exist");
        }
    }

    public void DeleteAll()
    {
        DataSource.Calls.Clear();
    }
    public Call? Read(int id)
    {
        //return DataSource.Assignment.Find(v => v.id == id); // stage 1

        return DataSource.Calls.FirstOrDefault(item => item.Id == id); //stage 2
    }
    public Call? Read(Func<Call, bool> filter)
    {
        return DataSource.Calls.FirstOrDefault(filter);
    }

    //public List<Assignment> ReadAll() // stage 1
    //{
    //    return DataSource.Assignment;
    //}

    public IEnumerable<Call> ReadAll(Func<Call, bool>? filter = null) // stage 2
        => filter == null
        ? DataSource.Calls.Select(item => item)
        : DataSource.Calls.Where(filter);

   
}
