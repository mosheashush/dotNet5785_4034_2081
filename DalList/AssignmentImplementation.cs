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
                throw new Exception($"Assignment already ID={item.Id} exists");
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
                throw new Exception($"Assignment does not ID={id} exist");
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
            else
            {
                return null;
            }
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
                throw new Exception($"Assignment does not ID={item.Id}  exist");
            }
        }
    }
}
