
using DalApi;
using DO;

namespace Dal
{
    internal class ConfigImplementation : IConfig
    {
        public DateTime Clock
        {
            get => Config.Clock;
            set => Config.Clock = value;
        }
        public int NextCallAssignmentId
        {
            get => Config.NextAssignmentId;
            set => throw new NotImplementedException();
        }
        public int NextCallId
        {
            get => Config.NextCallId;
            set => throw new NotImplementedException();
        }
        public TimeSpan RiskRange
        {
            get => Config.RiskRange;
            set => throw new NotImplementedException();
        }

        public void Reset()
        {
            Config.Reset();
        }
    }
}

