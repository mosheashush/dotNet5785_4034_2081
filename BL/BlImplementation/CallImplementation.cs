using BIApi;
//using DalApi;
using Helpers;
using System.Collections.Generic;
using System.Linq;
namespace BlImplementation;

internal class CallImplementation : ICall
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
        if (CallManager.GetCallState(s_dal.Call.ReadAll().FirstOrDefault(c => c.Id == id)) != BO.CallState.open)
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
            throw new BO.BlDoesNotExistException("DAL is not initialized");

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
        allClosedCalls = allClosedCalls.OrderBy(orderProp.GetValue);

        return allClosedCalls;
    }

    //GetOpenCallsForVolunteer implementation
    public IEnumerable<BO.OpenCallInList> GetOpenCallsForVolunteer(int volunteerId, BO.CallType? filterType, BO.OpenCallFields? orderByField)
    {
        var allOpenCalls = s_dal.Call.ReadAll()
            .Select(CallManager.MapDOToBOCall)
            .Where(a => a.CallState == BO.CallState.open || a.CallState == BO.CallState.openOnRisk)
            .Select(a => CallManager.MPIdVolunteerToOpenCallInList(a, volunteerId));

        // filter:
        if (filterType != null)
            allOpenCalls = allOpenCalls.Where(c => c.Type == filterType);

        // sort:
        if (orderByField == null)
            orderByField = BO.OpenCallFields.IdCall;
        var orderProp = typeof(BO.OpenCallInList).GetProperty(orderByField.ToString());
        allOpenCalls = allOpenCalls.OrderBy(orderProp.GetValue);

        return allOpenCalls;
    }

    // Updated FinishTreatment method to use the 'With' expression for immutable record properties
    public void FinishTreatment(int volunteerId, int assignmentId)
    {
        var assignment = s_dal.Assignment.Read(assignmentId)
            ?? throw new BO.BlDoesNotExistException($"Assignment with ID={assignmentId} does Not exist");

        if (assignment.VolunteerId != volunteerId)
            throw new BO.BlNotAllowedMakeChangesException($"Assignment with ID={assignmentId} does not belong to Volunteer with ID={volunteerId}");

        if (assignment.FinishType != null)
            throw new BO.NoTimeCompleteTaskException($"Assignment with ID={assignmentId} already {assignment.FinishType}");

        // Use 'With' to create a new immutable record with updated properties
        var updatedAssignment = assignment with
        {
            FinishType = DO.CompletionType.completed,
            CompletionTime = DateTime.Now
        };

        s_dal.Assignment.Update(updatedAssignment);
    }


    public void CancelTreatment(int requesterId, int assignmentId)
    {
        var assignment = s_dal.Assignment.Read(assignmentId)
           ?? throw new BO.BlDoesNotExistException($"Assignment with ID={assignmentId} does Not exist");
        DO.Assignment updatedAssignment;

        if (assignment.VolunteerId != requesterId ||
            s_dal.Volunteer.Read(requesterId).CurrentPosition == DO.User.admin)
            throw new BO.BlNotAllowedMakeChangesException($"Assignment with ID={assignmentId} does not belong to Volunteer with ID={requesterId}");
        if (assignment.FinishType != null)
            throw new BO.NoTimeCompleteTaskException($"Assignment with ID={assignmentId} already {assignment.FinishType}");
        // Use 'With' to create a new immutable record with updated properties
        if (s_dal.Volunteer.Read(requesterId).CurrentPosition == DO.User.admin) 
        { 
            updatedAssignment = assignment with
            {

                FinishType = DO.CompletionType.canceledAdmin,
                CompletionTime = DateTime.Now
            };
        }

        else
        {
            updatedAssignment = assignment with
            {
                FinishType = DO.CompletionType.canceledVolunteer,
                CompletionTime = DateTime.Now
            };
        }


        s_dal.Assignment.Update(updatedAssignment);

    }

    public void ChooseCallForTreatment(int volunteerId, int callId){
        var volunteer = s_dal.Volunteer.Read(volunteerId)
            ?? throw new BO.BlDoesNotExistException($"Volunteer with ID={volunteerId} does Not exist");
        var call = s_dal.Call.Read(callId)
            ?? throw new BO.BlDoesNotExistException($"Call with ID={callId} does Not exist");
        // Check if the call is already assigned to another volunteer
        if (s_dal.Assignment.ReadAll().FirstOrDefault(a => a.CallId == callId &&
        // Check if the call is already canceled
        (a.FinishType != DO.CompletionType.canceledAdmin ||
        a.FinishType != DO.CompletionType.canceledVolunteer)) != null)
            throw new BO.BlInMiddlePerformingTaskException("Call already assigned to another volunteer");
        // Check if the call is already expired
        if ( CallManager.GetCallState( s_dal.Call.Read(callId))== BO.CallState.expired)
            throw new BO.NoTimeCompleteTaskException("Call already expired");

        // Use 'With' to create a new immutable record with updated properties
        var assignment = new DO.Assignment
        {
            VolunteerId = volunteerId,
            CallId = callId,
            StarCall = DateTime.Now,
        };

        // Create the assignment in the data layer
          s_dal.Assignment.Create(assignment);
    }
}

