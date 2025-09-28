
using DalApi;

namespace Helpers;

internal static class AssignmentManager
{
    private static IDal s_dal = Factory.Get; //stage 4

    internal static ObserverManager Observers = new(); //stage 5 
                                                       //...


    //MapDOToBOAssignment implementation
    public static BO.Assignment MapDOToBOAssignment(DO.Assignment assignment)
    {
        return new BO.Assignment()
        {
            IdAssignment = assignment.Id,
            IdCall = assignment.CallId,
            VolunteerId = assignment.VolunteerId,
            StarCall = assignment.StarCall,
            CompletionTime = assignment.CompletionTime,
            FinishType = (BO.CompletionType?)assignment.FinishType
        };
    }

    //Get Assignment idAssignment and return public class ClosedCallInList implementation
    public static BO.ClosedCallInList MPIdAssignmentToClosedCall(int idAssignment)
    {
        return new BO.ClosedCallInList()
        {
            IdCall = s_dal.Assignment.Read(idAssignment).CallId,
            Type = (BO.CallType)s_dal.Call.Read(s_dal.Assignment.Read(idAssignment).CallId).Type,
            FullAddress = s_dal.Call.Read(s_dal.Assignment.Read(idAssignment).CallId).FullAddress,
            CallStartTime = s_dal.Call.Read(s_dal.Assignment.Read(idAssignment).CallId).CallStartTime,
            VolunteerTakeCall = s_dal.Assignment.Read(idAssignment).StarCall,
            CompletionTime = s_dal.Assignment.Read(idAssignment).CompletionTime,
            FinishType = (BO.CompletionType?)s_dal.Assignment.Read(idAssignment).FinishType,
        };
    }
}
