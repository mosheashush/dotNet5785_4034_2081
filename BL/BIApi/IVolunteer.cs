
using BO;
using BlApi;
using Helpers;

namespace BIApi;

public interface IVolunteer : IObservable
{
    public BO.User Entrance(string name, string password);
    public List<BO.VolunteerInList> listOfVolunteer(bool? isActive, BO.VolunteerInListFields? field);

    public void Create(BO.Volunteer volunteer);
    public BO.Volunteer? Read(int IdVolunteer);
    public void Update(int IdVolunteer, BO.Volunteer volunteer);
    public void Delete(int IdVolunteer);


}
