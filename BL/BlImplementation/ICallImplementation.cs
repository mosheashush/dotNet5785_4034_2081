using BIApi;
namespace BlImplementation;

internal class ICallImplementation : ICall
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;

}
