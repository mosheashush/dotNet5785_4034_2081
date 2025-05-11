using BIApi;
using BO;
using Dal;
using Helpers;
using Microsoft.VisualBasic;

namespace BlImplementation;

internal class AdminImplementation : IAdmin
{
    private readonly DalApi.IDal s_dal = DalApi.Factory.Get;

    public void AdvanceClock(TimeUnit timeUnit)
    {
        DateTime newTime = ClockManager.Now;

        // Calculate the new time based on the selected time unit
        switch (timeUnit)
        {
            case TimeUnit.Minute:
                newTime = newTime.AddMinutes(1);
                break;
            case TimeUnit.Hour:
                newTime = newTime.AddHours(1);
                break;
            case TimeUnit.Day:
                newTime = newTime.AddDays(1);
                break;
            case TimeUnit.Month:
                newTime = newTime.AddMonths(1);
                break;
            case TimeUnit.Year:
                newTime = newTime.AddYears(1);
                break;
        }

        // Update the clock manager with the new time 
        ClockManager.UpdateClock(newTime);
    }

    public DateTime GetClock()
    {
        return ClockManager.Now;
    }

    //GetRiskTimeSpan implementation
    public TimeSpan GetRiskTimeSpan()
    {
        return s_dal.Config.RiskRange;
    }

    public void InitializeDatabase()
    {

        s_dal.ResetDB();
        Initialization.Do();
        ClockManager.UpdateClock(ClockManager.Now);
    }

    public void ResetDatabase()
    {
       Initialization.Do();
       ClockManager.UpdateClock(ClockManager.Now);
    }

    public void SetRiskTimeSpan(TimeSpan newRiskRange)
    {
        s_dal.Config.RiskRange = newRiskRange;
    }
}
