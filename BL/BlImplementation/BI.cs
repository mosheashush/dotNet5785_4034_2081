using BIApi;
namespace BlImplementation;

internal class BI : IBl
{
    public IAdmin Admin { get; }
    public ICall Call { get; }
    public IVolunteer Volunteer { get; }
}
