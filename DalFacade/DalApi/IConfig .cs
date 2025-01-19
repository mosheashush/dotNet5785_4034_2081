
namespace DalApi
{
    public interface IConfig
    {
        DateTime Clock { get; set; }
        int NextCallAssignmentId { get; set; }
        int NextCallId { get; set; }
        TimeSpan RiskRange { get; set; }

        void Reset();



    }
}
