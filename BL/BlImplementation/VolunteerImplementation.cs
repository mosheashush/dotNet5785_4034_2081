using BIApi;
//using DalApi;
using Helpers;
using System.Collections.Generic;
using System.Linq;
namespace BlImplementation;

internal class VolunteerImplementation : IVolunteer
{
    private readonly DalApi.IDal s_dal = DalApi.Factory.Get;

    #region Stage 5 
    public void AddObserver(Action listObserver) =>
    VolunteerManager.Observers.AddListObserver(listObserver); //stage 5 
    public void AddObserver(int id, Action observer) =>
    VolunteerManager.Observers.AddObserver(id, observer); //stage 5 
    public void RemoveObserver(Action listObserver) =>
    VolunteerManager.Observers.RemoveListObserver(listObserver); //stage 5 
    public void RemoveObserver(int id, Action observer) =>
    VolunteerManager.Observers.RemoveObserver(id, observer); //stage 5 
    #endregion Stage 5

    // public override string ToString() => this.ToStringProperty();

    public void Create(BO.Volunteer boVolunteer)
    {
        VolunteerManager.CheckVolunteer(boVolunteer);
        try
        {
            s_dal.Volunteer.Create(VolunteerManager.MapBOToDOVolunteer(boVolunteer));
            VolunteerManager.Observers.NotifyListUpdated(); //stage 5 
        }
        catch (DO.DalAlreadyExistsException ex)
        {
            throw new BO.BlAlreadyExistsException($"Volunteer with ID={boVolunteer.id} already exists", ex);
        }
    }

    public BO.Volunteer? Read(int id)
    {
        var doVolunteer = s_dal.Volunteer.Read(id)
            ?? throw new BO.BlDoesNotExistException($"Volunteer with ID={id} does Not exist");

        // Map DO.Volunteer to BO.Volunteer
        return VolunteerManager.MapDOToBOVolunteer(doVolunteer);
    }
    public void Update(int userId, BO.Volunteer boVolunteer)
    {
        DO.Volunteer doVolunteer; //new not exists 
        try
        {
            doVolunteer = s_dal.Volunteer.Read(boVolunteer.id);
        }
        catch (DO.DalAlreadyExistsException ex)
        {
            throw new BO.BlDoesNotExistException($"Volunteer with ID={boVolunteer.id} already exists", ex);
        }

        DO.Volunteer userVolunteer; //user exists
        try
        {
            userVolunteer = s_dal.Volunteer.Read(userId);
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

        VolunteerManager.CheckVolunteer(boVolunteer);

        try
        {
            s_dal.Volunteer.Update(VolunteerManager.MapBOToDOVolunteer(boVolunteer));
            VolunteerManager.Observers.NotifyItemUpdated(boVolunteer.id);  //stage 5 
            VolunteerManager.Observers.NotifyListUpdated();  //stage 5 
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException($"Volunteer with ID={boVolunteer.id} does Not exist", ex);
        }
    }
    public void Delete(int id)
    {
        if (s_dal.Volunteer.ReadAll().FirstOrDefault(v => v.id == id).Active == true)
            throw new BO.BlInMiddlePerformingTaskException($"Volunteer with ID={id} has an assignment and cannot be deleted");

        try
        {
            s_dal.Volunteer.Delete(id);
            VolunteerManager.Observers.NotifyListUpdated(); //stage 5
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException($"Volunteer with ID={id} does Not exist", ex);
        }
    }


    // implementation of Entrance
    public BO.User Entrance(string name, string password)
    {
        var volunteer = s_dal.Volunteer.ReadAll().FirstOrDefault(v => v.FullName == name);
        if (volunteer == null)
            throw new BO.BlDoesNotExistException($"Volunteer with name={name} does Not exist");

        if (volunteer.Password != password)
            throw new BO.BlDoesNotExistException($"the password={name} does Not correctly");

        return (BO.User)volunteer.CurrentPosition;
    }

    // get filters and sort for volunteers and return a list of volunteers
    public List<BO.VolunteerInList> listOfVolunteer(bool? isActive, BO.VolunteerInListFields? field)
    {

        var volunteers = s_dal.Volunteer.ReadAll();
        
        var assignments = s_dal.Assignment.ReadAll();
        if (isActive != null)
            volunteers = volunteers.Where(v => v.Active == isActive);

        if (field != null)
        {
            switch (field)
            {
                case BO.VolunteerInListFields.IdVolunteer:
                    volunteers = volunteers.OrderBy(v => v.id);
                    break;
                case BO.VolunteerInListFields.FullName:
                    volunteers = volunteers.OrderBy(v => v.FullName);
                    break;
                case BO.VolunteerInListFields.Active:
                    volunteers = volunteers.OrderBy(v => v.Active);
                    break;
                case BO.VolunteerInListFields.IdCall:
                    if (isActive == false)
                        throw new BO.BlCanNotOrderNotExistsFieldException($"Volunteer with ID={isActive} can not do this change");
                    volunteers = volunteers.OrderBy(v => assignments.FirstOrDefault(c => c.VolunteerId == v.id).CallId);
                    break;
                case BO.VolunteerInListFields.Type:
                    if (isActive == false)
                        throw new BO.BlCanNotOrderNotExistsFieldException($"Volunteer with ID={isActive} can not do this change");
                    volunteers = volunteers.OrderBy(v => s_dal.Call.ReadAll().FirstOrDefault(c => c.Id == assignments.FirstOrDefault(c => c.VolunteerId == v.id).CallId).Type);
                    break;
                case BO.VolunteerInListFields.SumCallsCompleted:
                    volunteers = volunteers.OrderBy(v => VolunteerManager.CalculatSumCallsCompleted(v.id));
                    break;
                case BO.VolunteerInListFields.SumCallsExpired:
                    volunteers = volunteers.OrderBy(v => VolunteerManager.CalculatSumCallsExpired(v.id));
                    break;
                case BO.VolunteerInListFields.SumCallsConcluded:
                    volunteers = volunteers.OrderBy(v => VolunteerManager.CalculatSumCallsConcluded(v.id));
                    break;
            }
        }
        else
            volunteers = volunteers.OrderBy(v => v.id);

        List<BO.VolunteerInList> converted = volunteers.Select(v => 
        new BO.VolunteerInList
        {
            IdVolunteer = v.id,
            FullName = v.FullName,
            Active = v.Active,
            IdCall = (assignments.FirstOrDefault(c => c.VolunteerId == v.id) != null && assignments.FirstOrDefault(c => c.VolunteerId == v.id).CompletionTime == null) ? assignments.FirstOrDefault(c => c.VolunteerId == v.id).CallId : null,
            Type = (assignments.FirstOrDefault(c => c.VolunteerId == v.id) != null && assignments.FirstOrDefault(c => c.VolunteerId == v.id).CompletionTime == null) ? (BO.CallType)(s_dal.Call.ReadAll().FirstOrDefault(c => c.Id == assignments.FirstOrDefault(a => a.VolunteerId == v.id).CallId).Type) : BO.CallType.None,
            SumCallsCompleted = VolunteerManager.CalculatSumCallsCompleted(v.id),
            SumCallsExpired = VolunteerManager.CalculatSumCallsExpired(v.id),
            SumCallsConcluded = VolunteerManager.CalculatSumCallsConcluded(v.id),
        }).ToList();

        return converted;
    }
}
