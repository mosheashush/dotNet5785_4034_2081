using DalApi;

namespace Helpers;

internal static class AssignmentManager
{
    private static IDal s_dal = Factory.Get; //stage 4

    //Get volunteer id and return the call id
    public static DO.Assignment? GetCallByVolunteerId(int id)
    {
        DO.Assignment assignment = s_dal.Assignment.ReadAll().FirstOrDefault(c => c.VolunteerId == id);
        if (assignment == null)
            return null;
        else
            return assignment;
    }
}
