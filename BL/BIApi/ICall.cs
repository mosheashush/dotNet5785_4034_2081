namespace BIApi
{
    public interface ICall
    {
        int[] GetCallsAmountByStatus();
        IEnumerable<BO.CallInList> GetCallsList(BO.CallInListFields? filterField, object? filterValue, BO.CallInListFields? orderByField);
        BO.Call GetCallDetails(int callId);
        void UpdateCall(BO.Call call);
        void DeleteCall(int callId);
        void AddCall(BO.Call call);
        IEnumerable<BO.ClosedCallInList> GetClosedCallsByVolunteer(int volunteerId, BO.CallType? filterType, BO.ClosedCallFields? orderByField);
        IEnumerable<BO.OpenCallInList> GetOpenCallsForVolunteer(int volunteerId, BO.CallType? filterType, BO.OpenCallFields? orderByField);
        void FinishTreatment(int volunteerId, int assignmentId);
        void CancelTreatment(int requesterId, int assignmentId);
        void ChooseCallForTreatment(int volunteerId, int callId);

    }
}
