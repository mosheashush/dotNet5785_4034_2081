using BIApi;
using BO;
using DalApi;
using DO;
using Helpers;
namespace BlImplementation;

internal class VolunteerImplementation// : IVolunteer
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;
    public void Create(BO.Volunteer boVolunteer)
    {
        VolunteerManager.CheckVolunteer(boVolunteer);

        DO.Volunteer doVolunteer = VolunteerManager.MapBOToDOVolunteer(boVolunteer);

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
        DO.Volunteer doVolunteer;
        try
        {
            doVolunteer = _dal.Volunteer.Read(id);
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException($"Volunteer with ID={id} does Not exist", ex);
        }

        if (doVolunteer.Active == true)
        {
            throw new BO.BlInMiddlePerformingTaskException($"Volunteer with ID={id} has an assignment and cannot be deleted");
        }

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
    public void Update(int userId, BO.Volunteer boVolunteer)
    {
        DO.Volunteer doVolunteer; //new not exists 
        try
        {
            doVolunteer = _dal.Volunteer.Read(boVolunteer.id);
        }
        catch (DO.DalAlreadyExistsException ex)
        {
            throw new BO.BlDoesNotExistException($"Volunteer with ID={boVolunteer.id} already exists", ex);
        }

        DO.Volunteer userVolunteer; //user exists
        try
        {
            userVolunteer = _dal.Volunteer.Read(userId);
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException($"Volunteer with ID={userId} does Not exist", ex);
        }

        if ((userId != boVolunteer.id && userVolunteer.CurrentPosition != DO.User.admin) //allow
            || (userVolunteer.CurrentPosition != DO.User.admin
            && doVolunteer.CurrentPosition == DO.User.volnteer
            && boVolunteer.CurrentPosition == BO.User.admin))
            throw new BO.BlNotAllowedMakeChangesException($"Volunteer with ID={userId} can not do this change");

        Delete(boVolunteer.id);
        Create(boVolunteer);
    }

    // implementation of Entrance
    public BO.User Entrance(string name, string password)
    {
        var volunteer = _dal.Volunteer.ReadAll().FirstOrDefault(v => v.FullName == name);
        if (volunteer == null)
            throw new BO.BlDoesNotExistException($"Volunteer with name={name} does Not exist");

        if (volunteer.Password != password)
            throw new BO.BlDoesNotExistException($"the password={name} does Not correctly");

        return (BO.User)volunteer.CurrentPosition;
    }

    public List<BO.VolunteerInList> listOfVolunteer(bool? isActive, VolunteerInListFields? field)
    {

        var volunteers = _dal.Volunteer.ReadAll();
        if (isActive != null)
        {
            volunteers = volunteers.Where(v => v.Active == isActive);
        }
        if (field != null)
        {
            switch (field)
            {
                case VolunteerInListFields.IdVolunteer:
                    volunteers = volunteers.OrderBy(v => v.id);
                    break;
                case VolunteerInListFields.FullName:
                    volunteers = volunteers.OrderBy(v => v.FullName);
                    break;
                case VolunteerInListFields.Active:
                    volunteers = volunteers.OrderBy(v => v.Active);
                    break;

                case VolunteerInListFields.IdCall:
                    if (isActive == false)
                        throw new BO.BlCanNotOrderNotExistsFieldException($"Volunteer with ID={isActive} can not do this change");
                    volunteers = volunteers.OrderBy(v => _dal.Assignment.ReadAll().FirstOrDefault(c => c.VolunteerId == v.id).CallId);
                    break;
                case VolunteerInListFields.Type:
                    volunteers = volunteers.OrderBy(v => _dal.Call.ReadAll().FirstOrDefault(c => c.Id == _dal.Assignment.ReadAll().FirstOrDefault(c => c.VolunteerId == v.id).CallId).Type);
                    break;
                case VolunteerInListFields.SumCallsCompleted:
                    volunteers = volunteers.OrderBy(v => VolunteerManager.CalculatSumCallsCompleted(v.id));
                    break;
                case VolunteerInListFields.SumCallsExpired:
                    volunteers = volunteers.OrderBy(v => VolunteerManager.CalculatSumCallsExpired(v.id));
                    break;
                case VolunteerInListFields.SumCallsConcluded:
                    volunteers = volunteers.OrderBy(v => VolunteerManager.CalculatSumCallsConcluded(v.id));
                    break;
            }
        }
        else
            volunteers = volunteers.OrderBy(v => v.id);

        return null;
        /*new List<VolunteerInList>
        {
            Capacity = volunteers.Count(),
            Count = volunteers.Count(),
            List = volunteers.Select(v => new VolunteerInList()
            {
                IdVolunteer = v.id,
                FullName = v.FullName,
                Active = v.Active,
                IdCall = _dal.Assignment.ReadAll().FirstOrDefault(c => c.VolunteerId == v.id).CallId,
                Type = (BO.CallType)_dal.Call.ReadAll().FirstOrDefault(c => c.Id == _dal.Assignment.ReadAll().FirstOrDefault(c => c.VolunteerId == v.id).CallId).Type,
                SumCallsCompleted = VolunteerManager.CalculatSumCallsCompleted(v.id),
                SumCallsExpired = VolunteerManager.CalculatSumCallsExpired(v.id),
                SumCallsConcluded = VolunteerManager.CalculatSumCallsConcluded(v.id),
            }).ToList()
        };*/
    }
}
