using DalApi;

namespace Helpers;

internal static class VolunteerManager
{
    private static IDal s_dal = Factory.Get; //stage 4

    //CalculatSumCallsCompleted implementation
    public static int CalculatSumCallsCompleted(int id)
    {
        if (s_dal.Assignment.ReadAll()==null) return 0;

        return s_dal.Assignment.ReadAll().Where(c => c.VolunteerId == id && c.FinishType == DO.CompletionType.completed).Count();
    }

    //CalculatSumCallsExpired implementation:
    public static int CalculatSumCallsExpired(int id)
    {
        if (s_dal.Assignment.ReadAll() == null) return 0;

        return s_dal.Assignment.ReadAll().Where(c => c.VolunteerId == id && c.FinishType == DO.CompletionType.expired).Count();
    }

    //CalculatSumCallsConcluded implementation:
    public static int CalculatSumCallsConcluded(int id)
    {
        if (s_dal.Assignment.ReadAll() == null) return 0;

        return s_dal.Assignment.ReadAll().Where(c => c.VolunteerId == id && c.FinishType == DO.CompletionType.canceledVolunteer).Count();
    }

    //IfCallInProgress
    public static BO.CallInProgress? IfCallInProgress(int id)
    {
        var assignment = s_dal.Assignment.ReadAll().Where(c => c.VolunteerId == id).FirstOrDefault();
        if (assignment.FinishType  == null)
            return null;
        else
        {
            var callInProgress = s_dal.Assignment.ReadAll().Where(c => c.VolunteerId == id && c.FinishType == null).FirstOrDefault();
            return new BO.CallInProgress()
            {
                //IdCall = assignment.CallId,
                //IdAssignment = assignment.Id,
                //Type = (BO.CallType)s_dal.Call.ReadAll()?.FirstOrDefault(c => c.Id == assignment.CallId)?.Type,   // ?? BO.CallType.None,
                //description = s_dal.Call.ReadAll()?.FirstOrDefault(c => c.Id == assignment.CallId)?.description,
                //FullAddress = callInProgress.FinishType,
                //CallStartTime = assignment.StarCall,
                //MaxTimeForCall = assignment.CompletionTime,
                //VolunteerTakeCall = assignment.StarCall,
                //DistanceFromVolunteer = 0, // Placeholder for distance calculation
                //CollState = 
            };
        }
    }



}
