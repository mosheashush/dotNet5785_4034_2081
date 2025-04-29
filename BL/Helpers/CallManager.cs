using DalApi;
using System.Data;

namespace Helpers;

internal static class CallManager
{
    private static IDal s_dal = Factory.Get; //stage 4

    public static BO.CallState GetStatus(int CallId)
    {
        //todo: implement logic to determine the status of the call based on its ID
        return new BO.CallState();
    }

    public static List<double> GetCoordinates(string address)
    {
        //todo: implement logic to determine the coordinates of the call based on its ID
        return new List<double>();
    }

    public static double GetDistance(string addressCall, string addressVolunteer)
    {
        //todo: implement logic to determine the distance between the call and the volunteer
        return 0;
    }

    public static void UpdateExpiredCall()
    {
        // todo: implement logic to update expired calls
    }

    public static void Simulator()
    {
        // todo: implement logic to simulate
    }

}
