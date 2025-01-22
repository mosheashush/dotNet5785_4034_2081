
namespace DalApi;
using DO;
public interface IAssignment 
{
    void Create(Assignment item); // Create a new assignment
    void Update(Assignment item); // Update an existing assignment
    void Delete(int id);// Delete an assignment by its id
    Assignment Read(int id);// Read an assignment by its id
    List<Assignment> ReadAll();// Read all assignments
    void DeleteAll();// Delete all assignments



}

