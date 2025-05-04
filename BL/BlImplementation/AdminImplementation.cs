using BIApi;

namespace BlImplementation;

internal class AdminImplementation //: IAdmin
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;

    //GetRiskTimeSpan implementation
    public TimeSpan GetRiskTimeSpan()
    {
        return _dal.Config.RiskRange;
    }
}
