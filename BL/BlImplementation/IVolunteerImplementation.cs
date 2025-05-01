using BIApi;
using BO;
using DO;
using Helpers;
namespace BlImplementation;

internal class IVolunteerImplementation// : IVolunteer
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;
    public void Create(BO.Volunteer boVolunteer)
    {
        if (boVolunteer == null)
            throw new blCanNotCreatArgumentNullException(nameof(boVolunteer));

        // Map BO.Volunteer to DO.Volunteer
        DO.Volunteer doVolunteer = new(
            boVolunteer.id,
            boVolunteer.FullName,
            boVolunteer.CallNumber,
            boVolunteer.EmailAddress,
            boVolunteer.Password,
            boVolunteer.FullCurrentAddress,
            boVolunteer.Latitude,
            boVolunteer.Longitude,
            (DO.User)boVolunteer.CurrentPosition,
            boVolunteer.Active,
            boVolunteer.MaxDistanceForCall,
            (DO.Distance)boVolunteer.TypeOfDistance
        );

        try
        {
            _dal.Volunteer.Create(doVolunteer);
        }
        catch (DO.DalAlreadyExistsException ex)
        {
            throw new BO.BlAlreadyExistsException($"Volunteer with ID={boVolunteer.id} already exists", ex);
        }
    }

    public void Delete(int id)
    {
        _dal.Volunteer.Delete(id);
    }
    public BO.Volunteer? Read(int id)
    {
        var doVolunteer = _dal.Volunteer.Read(id)
            ?? throw new BO.BlDoesNotExistException($"Volunteer with ID={id} does Not exist");

        // Map DO.Volunteer to BO.Volunteer
        return new BO.Volunteer()
        {
            id = id,
            FullName = doVolunteer.FullName,
            CallNumber = doVolunteer.CallNumber,
            EmailAddress = doVolunteer.EmailAddress,
            Password = doVolunteer.Password,
            FullCurrentAddress = doVolunteer.FullCurrentAddress,
            Latitude = doVolunteer.Latitude,
            Longitude = doVolunteer.Longitud,
            CurrentPosition = (BO.User)doVolunteer.CurrentPosition,
            Active = doVolunteer.Active,
            MaxDistanceForCall = doVolunteer.MaxDistanceForCall,
            TypeOfDistance = (BO.Distance)doVolunteer.TypeOfDistance,
            SumCallsCompleted = VolunteerManager.CalculatSumCallsCompleted(id),
            SumCallsExpired = VolunteerManager.CalculatSumCallsExpired(id),
            SumCallsConcluded = VolunteerManager.CalculatSumCallsConcluded(id),
            CallInProgress = VolunteerManager.IfCallInProgress(id),
        };
    }
    public void Update(int id, BO.Volunteer volunteer)
    {
        var existingVolunteer = _dal.Volunteer.Read(id);
        if (existingVolunteer == null)
            throw new BO.BlDoesNotExistException($"Volunteer with ID={id} does Not exist");

        Delete(id);
        Create(volunteer);
    }

    // implementation of Entrance
    public BO.User Entrance(string name, string password)
    {
        var volunteer = _dal.Volunteer.ReadAll().FirstOrDefault(v => v.FullName == name);// && v.Password == password);
        if (volunteer == null)
            throw new BO.BlDoesNotExistException($"Volunteer with name={name} does Not exist");

        if (volunteer.Password != password)
            throw new BO.BlDoesNotExistException($"the password={name} does Not correctly");

        return (BO.User)volunteer.CurrentPosition;
    }

    //public BO.VolunteerInList listOfVolunteer(bool isActive, VolunteerInListFields field){}
}
