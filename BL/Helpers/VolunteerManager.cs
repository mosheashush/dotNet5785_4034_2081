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

    public static BO.CallInProgress? IfCallInProgress(int id)
    {
        DO.Assignment assignment = s_dal.Assignment.ReadAll().Where(c => c.VolunteerId == id && c.FinishType == null).FirstOrDefault(); // volunteer can take only one call at a time
        if (assignment.FinishType  == null)
            return null;
        else
        {
            DO.Call call = s_dal.Call.ReadAll().Where(c => c.Id == assignment.CallId).FirstOrDefault();
            DO.Volunteer volunteer = s_dal.Volunteer.ReadAll().Where(c => c.id == id).FirstOrDefault();
            return new BO.CallInProgress()
            {
                IdCall = assignment.CallId,
                IdAssignment = assignment.Id,
                Type = (BO.CallType)call.Type,
                description = call.description,
                FullAddress = call.FullAddress,
                CallStartTime = call.CallStartTime,
                MaxTimeForCall = call.MaxTimeForCall,
                VolunteerTakeCall = assignment.StarCall,
                DistanceFromVolunteer = GetDistanceInKm(call.Latitude, call.Longitude, volunteer.Latitude, volunteer.Longitud),
                CollState = GetCallState(call),
            };
        }
    }
    public static double GetDistanceInKm(double? lat1, double? lon1, double? lat2, double? lon2)
    {
        if (lat1 == null || lat2 == null || lon1 == null || lon2 == null)
            throw new ArgumentNullException("Latitude and Longitude is null");
        else
        {
            double R = 6371; // radius of Earth in kilometers  
            double dLat = DegreesToRadians(lat2.Value - lat1.Value);
            double dLon = DegreesToRadians(lon2.Value - lon1.Value);

            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                       Math.Cos(DegreesToRadians(lat1.Value)) * Math.Cos(DegreesToRadians(lat2.Value)) *
                       Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c;
        }
    }

    private static double DegreesToRadians(double deg)
    {
        return deg * (Math.PI / 180);
    }

    public static BO.CallState GetCallState(DO.Call call)
    {
        if (call.MaxTimeForCall > ClockManager.Now)
                return BO.CallState.processed;
        else
            return BO.CallState.ProcessedOnRisk;
    }
}
