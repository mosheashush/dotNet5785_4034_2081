using DalApi;
using DO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dal
{
    internal class AssignmentImplementation : IAssignment
    {
        public void Create(Assignment item)
        {

           if(Read(item.Id) == null)
            {
                item = item.WithId(Config.NextAssignmentId);
                DataSource.Assignments.Add(item);
            }
            else
            {
                throw new DalAlreadyExistsException($"Assignment already ID={item.Id} exists");
            }
        }

        public void Delete(int id)
        {
            Assignment? assignment = DataSource.Assignments.Find(a => a.Id == id);
            if (assignment != null)
            {
                DataSource.Assignments.Remove(assignment);
            }
            else
            {
                throw new DalDoesNotExistException($"Assignment does not ID={id} exist");
            }
        }

        public void DeleteAll()
        {
            DataSource.Assignments.Clear();
        }

        public Assignment? Read(int id)
        {
            //return DataSource.Assignment.Find(v => v.id == id); // stage 1

            return DataSource.Assignments.FirstOrDefault(item => item.Id == id); //stage 2
        }

        public Assignment? Read(Func<Assignment, bool> filter)
        {
            return DataSource.Assignments.FirstOrDefault(filter);
        }

        //public List<Assignment> ReadAll() // stage 1
        //{
        //    return DataSource.Assignment;
        //}

        public IEnumerable<Assignment> ReadAll(Func<Assignment, bool>? filter = null) // stage 2
            => filter == null
            ? DataSource.Assignments.Select(item => item)
            : DataSource.Assignments.Where(filter);

        public void Update(Assignment item)
        {
            if (Read(item.Id) != null)
            {
                DataSource.Assignments.Remove(Read(item.Id));
                DataSource.Assignments.Add(item);
            }
            else
            {
                throw new DalDoesNotExistException($"Assignment does not ID={item.Id}  exist");
            }
        }
    }
}
