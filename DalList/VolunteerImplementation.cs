using DalApi;
using DO;

namespace Dal
{
    public class VolunteerImplementation : IVolunteer
    {
        public void Create(Volunteer item)
        {

            if (Read(item.id) == null)
            {
                DataSource.Volunteers.Add(item);
            }

            else
            {
                throw new Exception($"Volunteer with the same ID={item.id} already exists...");
            }
        }

        public void Delete(int id)
        {
            Volunteer? volunteer = DataSource.Volunteers.Find(v => v.id == id);
            if (volunteer == null)
            {
                throw new Exception($"Volunteer with the same ID={id} not found...");
            }
            DataSource.Volunteers.Remove(volunteer);
        }

        public void DeleteAll()
        {
           Dal.DataSource.Volunteers.Clear();
        }

        public Volunteer? Read(int id)
        {
            Volunteer? volunteer = DataSource.Volunteers.Find(v => v.id == id);
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
                throw new Exception($"Volunteer with the same ID={item.id} not found...");
            }
            Delete(item.id);
            Create(item);
        }
    }
}
