using BIApi;

namespace BlImplementation;

internal class IAdminImplementation : IAdmin
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;


}
