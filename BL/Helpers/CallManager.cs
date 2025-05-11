using DalApi;
using BIApi;
using Dal;
using System.Data;
using BlImplementation;
//using BO;
using DO;
using System.Numerics;
using System.Threading.Tasks;
using BO;

namespace Helpers;

internal static class CallManager
{
    private static IDal s_dal = Factory.Get; //stage 4

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
            FinishType = (BO.CompletionType)a.FinishType,
        }).ToList();

        return list;
    }

    //GetCallState implementation
    public static BO.CallState GetCallState(DO.Call call)
    {
        if (call.MaxTimeForCall < ClockManager.Now)
        {
            if (s_dal.Assignment.ReadAll().FirstOrDefault(c => c.CallId == call.Id).FinishType == DO.CompletionType.completed)
                return BO.CallState.completed;
            else
                return BO.CallState.expired;
        }
        if (call.MaxTimeForCall - s_dal.Config.RiskRange <= ClockManager.Now)
        {
            if (s_dal.Assignment.ReadAll().FirstOrDefault(c => c.CallId == call.Id) != null)
                return BO.CallState.ProcessedOnRisk;
            else
                return BO.CallState.openOnRisk;
        }
        else if (call.MaxTimeForCall - s_dal.Config.RiskRange > ClockManager.Now)
        {
            if (s_dal.Assignment.ReadAll().FirstOrDefault(c => c.CallId == call.Id) != null)
                return BO.CallState.processed;
            else
                return BO.CallState.open;
        }
        throw new ArgumentException("Invalid boCall state");
    }
} 
