using DalApi;
using DO;

namespace Dal
{
    internal class VolunteerImplementation : IVolunteer
    {
        public void Create(Volunteer item)
        {

            if (Read(item.id) == null)
            {
                DataSource.Volunteers.Add(item);
            }

            else
            {
                throw new DalDoesNotExistException($"Volunteer with the same ID={item.id} already exists...");
            }
        }

        

        public void Delete(int id)
        {
            Volunteer? volunteer = DataSource.Volunteers.Find(v => v.id == id);
            if (volunteer == null)
            {
                throw new DalDoesNotExistException($"Volunteer with the same ID={id} not found...");
            }
            DataSource.Volunteers.Remove(volunteer);
        }

        public void DeleteAll()
        {
           DataSource.Volunteers.Clear();
        }

        public Volunteer? Read(int id)
        {
            //return DataSource.Volunteers.Find(v => v.id == id); // stage 1

            return DataSource.Volunteers.FirstOrDefault(item => item.id == id); //stage 2


        }

        public Volunteer? Read(Func<Volunteer, bool> filter)
        {
            return DataSource.Volunteers.FirstOrDefault(filter);
        }

        //public List<Volunteer> ReadAll() // stage 1
        //{
        //    return DataSource.Volunteers;
        //}

        public IEnumerable<Volunteer> ReadAll(Func<Volunteer, bool>? filter = null) // stage 2
        
            => filter == null
            ? DataSource.Volunteers.Select(item => item)
            : DataSource.Volunteers.Where(filter);


        

        public void Update(Volunteer item)
        {
            if (Read(item.id) == null)
            {
                throw new DalDoesNotExistException($"Volunteer with the same ID={item.id} not found...");
            }
            Delete(item.id);
            Create(item);
        }
    }
}
