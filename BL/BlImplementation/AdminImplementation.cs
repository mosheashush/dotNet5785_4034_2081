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
        DateTime newTime = AdminManager.Now;

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
        AdminManager.UpdateClock(newTime);
    }

    public DateTime GetClock()
    {
        return AdminManager.Now;
    }

    //GetRiskTimeSpan implementation
    public TimeSpan GetRiskTimeSpan()
    {
        return AdminManager.RiskRange;
    }

    public void InitializeDatabase()
    {
        Dal.Initialization.Do();
        AdminManager.UpdateClock(AdminManager.Now);
        AdminManager.RiskRange
            = AdminManager.RiskRange;
    } 

    public void ResetDatabase()
    {
        s_dal.ResetDB();
        AdminManager.UpdateClock(AdminManager.Now);
    }

    public void SetRiskTimeSpan(TimeSpan newRiskRange)
    {
        s_dal.Config.RiskRange = newRiskRange;
    }

    #region Stage 5
    public void AddClockObserver(Action clockObserver) =>
    AdminManager.ClockUpdatedObservers += clockObserver;
    public void RemoveClockObserver(Action clockObserver) =>
    AdminManager.ClockUpdatedObservers -= clockObserver;
    public void AddConfigObserver(Action configObserver) =>
   AdminManager.ConfigUpdatedObservers += configObserver;
    public void RemoveConfigObserver(Action configObserver) =>
    AdminManager.ConfigUpdatedObservers -= configObserver;
    #endregion Stage 5

}
