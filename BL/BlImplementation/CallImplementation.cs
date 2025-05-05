using BIApi;
using DO;
using Helpers;
using System.Collections.Generic;
using System.Linq;
namespace BlImplementation;


internal class CallImplementation //: ICall
{
    private readonly DalApi.IDal s_dal = DalApi.Factory.Get;

    //Creat implementation
    public void Create(BO.Call boCall)
    {
        CallManager.CheckCall(boCall); // Check the call before creating it

        try
        {
            s_dal.Call.Create(CallManager.MapBOToDOCall(boCall));
        }
        catch (DO.DalAlreadyExistsException ex)
        {
            throw new BO.BlAlreadyExistsException($"Call with ID={boCall.IdCall} already exists", ex);
        }
    }
    //Read implementation
    public BO.Call Read(int id)
    {
        var doCall = s_dal.Call.Read(id)
            ?? throw new BO.BlDoesNotExistException($"Call with ID={id} does Not exist");
        // Map DO.Call to BO.Call
        return CallManager.MapDOToBOCall(doCall);
    }

    // Update implementation
    public void Update(BO.Call boCall)
    {
        CallManager.CheckCall(boCall);

        try
        {
            s_dal.Call.Update(CallManager.MapBOToDOCall(boCall));
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException($"Call with ID={boCall.IdCall} does Not exist", ex);
        }
    }

    //Delete implementation
    public void Delete(int id)
    {
        if (CallManager.GetCallState(s_dal.Call.ReadAll().FirstOrDefault(c=> c.Id == id)) != BO.CallState.open)
        {
            throw new BO.BlInMiddlePerformingTaskException($"Call with ID={id} is in progress past or present and cannot be deleted");
        }

        try
        {
            s_dal.Call.Delete(id);
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException($"Call with ID={id} does Not exist", ex);
        }
    }

    public int[] GetCallsAmountByStatus()
    {
        if (s_dal.Call.ReadAll() == null)
            throw new DalDoesNotExistException("DAL is not initialized");

        var grouped = s_dal.Call.ReadAll()
            .Select(CallManager.MapDOToBOCall)
            .GroupBy(call => call.CallState)
            .ToDictionary(g => g.Key, g => g.Count());

        int[] callsAmountByStatus = Enum
            .GetValues(typeof(BO.CallState))
            .Cast<BO.CallState>()
            .Select(type => grouped.ContainsKey(type) ? grouped[type] : 0) // Use ContainsKey and indexer instead of GetValueOrDefault
            .ToArray();

        return callsAmountByStatus;
    }

    public IEnumerable<BO.CallInList> GetCallsList(BO.CallInListFields? filterField, object? filterValue, BO.CallInListFields? orderByField)
    {
        var allCallInList = s_dal.Volunteer.ReadAll()
            .Select(v => VolunteerManager.MPIdVolunteerToCallInProgress(v.id))
            .Where(c => c != null) // take just when CallInProgress not was null
            .Cast<BO.CallInList>();

        // filter:
        if (filterField != null && filterValue != null)
        {
            var prop = typeof(BO.CallInList).GetProperty(filterField.ToString());
            allCallInList = allCallInList.Where(c => object.Equals(prop.GetValue(c), filterValue));
        }

        // sort:
        if (orderByField == null)
            orderByField = BO.CallInListFields.IdCall;

        var orderProp = typeof(BO.CallInList).GetProperty(orderByField.ToString());
        allCallInList = allCallInList.OrderBy(c => orderProp.GetValue(c));

        return allCallInList;
    }

    //GetClosedCallsByVolunteer implementation
    public IEnumerable<BO.ClosedCallInList> GetClosedCallsByVolunteer(int volunteerId, BO.CallType? filterType, BO.ClosedCallFields? orderByField)
    {
        var allClosedCalls = s_dal.Assignment.ReadAll()
            .Where(a => a.VolunteerId == volunteerId)
            .Where(a => a.FinishType != null)
            .Select(a => AssignmentManager.MPIdAssignmentToClosedCall(a.Id));

        // filter:
        if (filterType != null)
            allClosedCalls = allClosedCalls.Where(c => c.Type == filterType);

        // sort:
        if (orderByField == null)
            orderByField = BO.ClosedCallFields.IdCall;

        var orderProp = typeof(BO.ClosedCallInList).GetProperty(orderByField.ToString());
        allClosedCalls = allClosedCalls.OrderBy(c => orderProp.GetValue(c));

        return allClosedCalls;
    }
}

