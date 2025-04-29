//using BO;
using BO;

namespace BIApi;

public interface IVolunteer
{
    public BO.User Entrance(string name, string password);
    public BO.VolunteerInList listOfVolunteer(bool isActive, VolunteerInListFields field);
    public BO.Volunteer DetilsOfVolunteer(int IdVolunteer); //creat new

    public void Create(BO.Volunteer volunteer);
    public void Read (int IdVolunteer);
    public void Update(int IdVolunteer,BO.Volunteer volunteer);
    public void Delete(int IdVolunteer);
}
