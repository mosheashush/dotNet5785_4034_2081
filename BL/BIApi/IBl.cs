using BIApi;

namespace BIApi;

public interface IBl //: IObservable
{
    IAdmin Admin { get; }
    ICall Call { get; }
    IVolunteer Volunteer { get; }

}
