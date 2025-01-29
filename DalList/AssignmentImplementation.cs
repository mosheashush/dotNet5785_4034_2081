using DalApi;
using DO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dal
{
    public class AssignmentImplementation : IAssignment
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
                throw new InvalidOperationException("Assignment already exists");
            }
        }

        public void Delete(int id)
        {
            if (Read(id) != null)
            {
                DataSource.Assignments.Remove(Read(id));
            }
            else
            {
                throw new InvalidOperationException("Assignment does not exist");
            }
        }

        public void DeleteAll()
        {
            DataSource.Assignments.Clear();
        }

        public Assignment Read(int id)
        {
            Assignment? assignment = DataSource.Assignments.Find(a => a.Id == id);
            if (assignment != null)
            {
                return assignment;
            }
            return null;
        }

        public List<Assignment> ReadAll()
        {
            List<Assignment> assignments = DataSource.Assignments;
            return assignments;
        }

        public void Update(Assignment item)
        {
            if (Read(item.Id) != null)
            {
                DataSource.Assignments.Remove(Read(item.Id));
                DataSource.Assignments.Add(item);
            }
            else
            {
                throw new InvalidOperationException("Assignment does not exist");
            }
        }
    }
}
