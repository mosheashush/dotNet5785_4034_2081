using DalApi;

namespace Helpers;

internal static class AssignmentManager
{
    private static IDal s_dal = Factory.Get; //stage 4

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
            FinishType = (BO.CompletionType)assignment.FinishType
        };
    }

    //Get Assignment id and return public class ClosedCallInList implementation
    public static BO.ClosedCallInList MPIdAssignmentToClosedCall(int id)
    {
        return new BO.ClosedCallInList()
        {
            IdCall = s_dal.Assignment.Read(id).CallId,
            Type = (BO.CallType)s_dal.Call.Read(s_dal.Assignment.Read(id).CallId).Type,
            FullAddress = s_dal.Call.Read(s_dal.Assignment.Read(id).CallId).FullAddress,
            CallStartTime = s_dal.Call.Read(s_dal.Assignment.Read(id).CallId).CallStartTime,
            VolunteerTakeCall = s_dal.Assignment.Read(id).StarCall,
            CompletionTime = s_dal.Assignment.Read(id).CompletionTime,
            FinishType = (BO.CompletionType)s_dal.Assignment.Read(id).FinishType,
        };
    }
}
