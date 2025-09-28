//using DalApi;
using BO;
using DalApi;
using DO;
using System.Xml.Linq;
//using BIApi;
//using BIApi;


namespace Helpers;

internal static class CallManager
{
    private static DalApi.IDal s_dal = DalApi.Factory.Get;

    internal static ObserverManager Observers = new(); //stage 5 

   

    //MapDOToBOCall implementation
    public static BO.Call MapDOToBOCall(DO.Call call)
    {
        return new BO.Call()
        {
            IdCall = call.Id,
            Type = (BO.CallType)call.Type,
            description = call.description,
            FullAddress = call.FullAddress,
            CallStartTime = call.CallStartTime,
            MaxTimeForCall = call.MaxTimeForCall,
            Latitude = call.Latitude,
            Longitude = call.Longitude,
            CallState = GetCallState(call),
            callAssignInLists = CreatAssignInList(call.Id)
        };
    }
    //MapBOToDOCall implementation
    public static DO.Call MapBOToDOCall(BO.Call call)
    {
        return new DO.Call()
        {
            Id = call.IdCall,
            Type = (DO.CallType)call.Type,
            description = call.description,
            FullAddress = call.FullAddress,
            CallStartTime = call.CallStartTime,
            MaxTimeForCall = call.MaxTimeForCall,
            Latitude = call.Latitude,
            Longitude = call.Longitude
        };
    }

    //MPIdVolunteerToOpenCall implementation\
    public static BO.OpenCallInList MPIdVolunteerToOpenCallInList(BO.Call call, int idVolunteer)
    {
        return new BO.OpenCallInList()
        {
            IdCall = call.IdCall,
            Type = (BO.CallType)call.Type,
            description = call.description,
            FullAddress = call.FullAddress,
            CallStartTime = call.CallStartTime,
            MaxTimeForCall = call.MaxTimeForCall,
            DistanceFromVolunteer = VolunteerManager.GetDistanceInKm(call.Latitude, call.Longitude, s_dal.Volunteer.Read(idVolunteer).Latitude , s_dal.Volunteer.Read(idVolunteer).Longitud),
        };
    }

    //Check Call implementation
    public static void CheckCall(BO.Call boCall)
    {
        //check id
        if (boCall.IdCall < 1000 || boCall.IdCall > 1999)
            throw new BO.BlInvalidValueException($"Call id ={boCall.IdCall} not have corrent digits");

        //check time to complete task
        if (boCall.MaxTimeForCall < boCall.CallStartTime)
            throw new BO.NoTimeCompleteTaskException("Max time for boCall must be greater than boCall start time");

        //address check and update coordinates
        (boCall.Latitude, boCall.Longitude)  = VolunteerManager.GetCoordinatesFromAddress(boCall.FullAddress);
    }

    //CreatAssignInList implementation
    public static List<BO.CallAssignInList> CreatAssignInList(int callId)
    {
        if (s_dal.Assignment.ReadAll().Where(c => c.CallId == callId)==null) return null;
        
        List<BO.CallAssignInList> list = s_dal.Assignment.ReadAll().Where(a => a.CallId == callId).Select(a=> new BO.CallAssignInList
        {
            IdVolunteer = a.VolunteerId,
            FullName = s_dal.Volunteer.Read(a.VolunteerId).FullName,
            CallStartTime = a.StarCall,
            CompletionTime = a.CompletionTime,
            FinishType = (BO.CompletionType?)a.FinishType,
        }).ToList();

        return list;
    }

    //GetCallState implementation
    public static BO.CallState GetCallState(DO.Call call)
    {
        var assignments = s_dal.Assignment.ReadAll().Where(c => c.CallId == call.Id);

        if (assignments != null && assignments.Any(a=> a.FinishType == DO.CompletionType.completed))
            return BO.CallState.completed;
        else if (assignments != null && assignments.Any(a => a.FinishType == DO.CompletionType.expired))
            return BO.CallState.expired;

        if (call.MaxTimeForCall < AdminManager.Now)
        {
            if (assignments != null && assignments.Any(a => a.FinishType == null))
            {
                DO.Assignment assignment = assignments.FirstOrDefault(a => a.FinishType == null);
                DO.Assignment UpdateAssignment = assignment with
                         {
                            FinishType = DO.CompletionType.expired,
                            CompletionTime = AdminManager.Now
                          };
                s_dal.Assignment.Update(UpdateAssignment);
                Observers.NotifyItemUpdated(call.Id);  //stage 5
                Observers.NotifyListUpdated();  //stage 5}
                return BO.CallState.expired;
            }
            else
                return BO.CallState.expired;
        }
        else if (call.MaxTimeForCall - s_dal.Config.RiskRange <= AdminManager.Now)
        {
            if (assignments != null && assignments.Any(a => a.FinishType == null))
                return BO.CallState.ProcessedOnRisk;
            else
                return BO.CallState.openOnRisk;
        }
        else if (call.MaxTimeForCall - s_dal.Config.RiskRange > AdminManager.Now)
        {
            if (assignments != null && assignments.Any(a => a.FinishType == null))
                return BO.CallState.processed;
            else
                return BO.CallState.open;
        }
        throw new ArgumentException("Invalid boCall state");
    }

    public static CallInList MPAssignmentToCallInList(BO.Call call)
    {
        DO.Assignment assignment = s_dal.Assignment.ReadAll().FirstOrDefault(a => a.CallId == call.IdCall);
        return new BO.CallInList()
        {
            IdAssignment = assignment?.Id ?? null,
            IdCall = call.IdCall,
            Type = call.Type,
            CallStartTime = call.CallStartTime,
            TimeRemaining = call.MaxTimeForCall - AdminManager.Now,
            NameFinalVolunteer = s_dal.Assignment.ReadAll().Where(a => a.CallId == call.IdCall).OrderByDescending(a => a.CompletionTime)
                                                 .Join(s_dal.Volunteer.ReadAll(), a => a.VolunteerId, v => v.id, (a, v) => v.FullName)
                                                 .FirstOrDefault(),
            SumTimeProcess = assignment != null && assignment.FinishType == DO.CompletionType.completed ? AdminManager.Now - call.CallStartTime : null,
            CallState = call.CallState,
            SumOfAssignments = s_dal.Assignment.ReadAll().Count(c=> c.CallId == call.IdCall),
        };
    }

    /// <summary>
    /// Updates all expired calls that haven't been assigned to a volunteer
    /// by creating an assignments marked as 'expired'.
    /// </summary>
    public static void UpdateExpiredCall()
    {
        // Read all existing assignments once to improve performance
        var existingAssignments = s_dal.Assignment.ReadAll()
            .Select(a => a.CallId)
            .ToHashSet();

        // Filter calls whose MaxTimeForCall has passed and are not yet assigned
        var expiredCalls = s_dal.Call.ReadAll()
            .Where(c => c.MaxTimeForCall < AdminManager.Now &&
                        !existingAssignments.Contains(c.Id))
            .ToList();

        // Create an 'expired' assignments for each unassigned expired call
        expiredCalls.ForEach(c =>
            s_dal.Assignment.Create(new DO.Assignment
            {
                CallId = c.Id,
                VolunteerId = 0,
                StarCall = c.CallStartTime,
                FinishType = DO.CompletionType.expired,
                CompletionTime = AdminManager.Now
            })
        );

        var assignments = s_dal.Assignment.ReadAll()
     .Where(a => s_dal.Call.Read(a.CallId).MaxTimeForCall < AdminManager.Now)
     .Select(a => a with { FinishType = DO.CompletionType.expired, CompletionTime = AdminManager.Now })
     .ToList();

        assignments.ForEach(s_dal.Assignment.Update);
    }
}
