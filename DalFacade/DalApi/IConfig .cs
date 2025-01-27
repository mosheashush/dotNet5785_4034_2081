
namespace DalApi
{
    public interface IConfig
    {
        DateTime Clock { get; set; }
        TimeSpan RiskRange { get; set; }
        int nextCallId { get; }
        int nextAsignmentId { get; }
        void Reset();



    }
}
