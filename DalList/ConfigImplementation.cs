using DalApi;
using DO;

namespace Dal
{
    internal class ConfigImplementation : IConfig
    {
        /// <summary>
        /// The starting point for the call id
        /// <param name = "StartCallId"> The starting point for the call id</param>
        /// <param name="nextCallId"> The next call id</param>
        /// <param name="StartAssignmentId"> The starting point for the assignment id</param>
        /// <param name="nextAssignmentId"> The next assignment id</param>
        /// <param name="Clock"> The clock time</param>
        /// <param name="RiskRange"> The risk range</param>
        /// <param name="Reset"> The reset function</param>
        /// </summary>
        internal const int StartCallId = 1000;
        private static int nextCallId = StartCallId;
        internal static int NextCallId { get => nextCallId++; }

        internal const int StartAssignmentId = 2000;
        private static int nextAssignmentId = StartAssignmentId;

        public DateTime Clock { get; private set; }
        internal static int NextAssignmentId { get => nextAssignmentId++; }

        internal static TimeSpan RiskRange { get; set; } = TimeSpan.Zero;
        DateTime IConfig.Clock { get; set; } = DateTime.Now;
        int IConfig.NextCallId { get => NextCallId; }
        TimeSpan IConfig.RiskRange { get; set; } = TimeSpan.Zero;

        int IConfig.NextAssignmentId { get => NextAssignmentId; }

        void IConfig.Reset()
        {
            nextCallId = StartCallId;
            nextAssignmentId = StartAssignmentId;
            Clock = DateTime.Now;
            RiskRange = TimeSpan.Zero;
        }
    }
}
