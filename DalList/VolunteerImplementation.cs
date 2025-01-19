using DalApi;
using DO;

namespace Dal
{
    internal class VolunteerImplementation : IVolunteer
    {
        public void Create(Volunteer item)
        {
            Volunteer volunteer = item;
            if (Read(volunteer.id) != null)
            {
                throw new Exception("Volunteer with the same id already exists...");
            }
            DataSource.Volunteers.Add(volunteer);
        }

        public void Delete(int id)
        {
            Volunteer volunteer = DataSource.Volunteers.Find(v => v.id == id);
            if (volunteer == null)
            {
                throw new Exception("Volunteer with the same id not found...");
            }
            DataSource.Volunteers.Remove(volunteer);
        }

        public void DeleteAll()
        {
           Dal.DataSource.Volunteers.Clear();
        }

        public Volunteer? Read(int id)
        {
            Volunteer volunteer = DataSource.Volunteers.Find(v => v.id == id);
            if ( volunteer == null)
            {
                return null;
            }
            return volunteer;
        }

        public List<Volunteer> ReadAll()
        {
            return DataSource.Volunteers;
        }

        public void Update(Volunteer item)
        {
            if (Read(item.id) == null)
            {
                throw new Exception("Volunteer with the same id not found...");
            }
            Delete(item.id);
            Create(item);
        }
    }
}
