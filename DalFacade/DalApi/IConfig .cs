
namespace DalApi
{
    public interface IConfig
    {
        DateTime Clock { get; set; }
        int NextAssignmentId { get; }
        int NextCallId { get;  }
        TimeSpan RiskRange { get; set; }

        void Reset();



    }
}
