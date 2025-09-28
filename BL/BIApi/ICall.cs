using BlApi;

namespace BIApi;

public interface ICall : IObservable
{
    int[] GetCallsAmountByStatus();
    IEnumerable<BO.CallInList> GetCallsList(BO.CallInListFields? filterField, object? filterValue, BO.CallInListFields? orderByField);
    BO.Call Read(int callId);
    void Update(BO.Call call);
    void Delete(int callId);
    void Create(BO.Call call);
    IEnumerable<BO.ClosedCallInList> GetClosedCallsByVolunteer(int volunteerId, BO.CallType? filterType, BO.ClosedCallFields? orderByField);
    IEnumerable<BO.OpenCallInList> GetOpenCallsForVolunteer(int volunteerId, BO.CallType? filterType, BO.OpenCallFields? orderByField);
    void FinishTreatment(int volunteerId, int assignmentId);
    void CancelTreatment(int requesterId, int assignmentId);
    void ChooseCallForTreatment(int volunteerId, int callId);
}
