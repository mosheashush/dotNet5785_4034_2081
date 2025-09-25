using Dal;
using DalApi;
using DO;
using Microsoft.VisualBasic;
using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Threading.Channels;

namespace DalTest;
public class Program
{

    //private static IVolunteer? s_dalVolunteer = new VolunteerImplementation(); // stage 1
    //private static IAssignment? s_dalAssignment = new AssignmentImplementation(); // stage 1
    //private static ICall? s_dalCall = new CallImplementation(); // stage 1
    //private static IConfig? s_dalConfig = new ConfigImplementation(); // stage 1

    //static readonly IDal? s_dal = new DalList(); //stage 2


    //static readonly IDal s_dal = new DalXml(); //stage 3

    static readonly IDal s_dal = Factory.Get; //stage 4

    static void Main(string[] args)
    {

        try
        {
            //Initialization.Do(s_dal); //stage 2
            Initialization.Do(); //stage 4


            int userInput = menu();
            int id;

            while (userInput != 0)
            {
                switch (userInput)
                {
                    case 1:
                        Console.WriteLine("Volunteer");
                        int op = options();
                        switch (op)
                        {
                            case 1:
                                creatVolunteer();
                                break;
                            case 2:
                                Console.WriteLine("Enter Volunteer ID");
                                id = int.Parse(Console.ReadLine()!);
                                PrintTheReadFunctionOfVolunteer(s_dal.Volunteer!.Read(id));
                                break;
                            case 3:
                                readAllVolunteers();
                                break;
                            case 4:
                                s_dal.Volunteer!.Update(updateVolunteer());
                                break;
                            case 5:
                                Console.WriteLine("Enter Volunteer ID");
                                id = int.Parse(Console.ReadLine()!);
                                s_dal.Volunteer!.Delete(id);
                                break;
                            case 6:
                                s_dal.Volunteer!.DeleteAll();
                                break;
                        }
                        break;
                    case 2:
                        Console.WriteLine("Assignment");
                        int op1 = options();
                        switch (op1)
                        {
                            case 1:
                                creatAssignment();
                                break;
                            case 2:
                                Console.WriteLine("Enter Assignment ID");
                                id = int.Parse(Console.ReadLine()!);
                                PrintTheReadFunctionOfAssignment(s_dal.Assignment!.Read(id));
                                break;
                            case 3:
                                readAllAssignments();
                                break;
                            case 4:
                                s_dal.Assignment!.Update(updateAssignment());
                                break;
                            case 5:
                                Console.WriteLine("Enter Assignment ID");
                                id = int.Parse(Console.ReadLine()!);
                                s_dal.Assignment!.Delete(id);
                                break;
                            case 6:
                                s_dal.Assignment!.DeleteAll();
                                break;
                        }
                        break;
                    case 3:
                        Console.WriteLine("Call");
                        int op2 = options();
                        switch (op2)
                        {
                            case 1:
                                creatCall();
                                break;
                            case 2:
                                Console.WriteLine("Enter Call ID");
                                id = int.Parse(Console.ReadLine()!);
                                PrintTheReadFunctionOfCall(s_dal.Call!.Read(id));
                                break;
                            case 3:
                                readAllCalls();
                                break;
                            case 4:
                                s_dal.Call!.Update(updateCall());
                                break;
                            case 5:
                                Console.WriteLine("Enter Call ID");
                                id = int.Parse(Console.ReadLine()!);
                                s_dal.Call!.Delete(id);
                                break;
                            case 6:
                                s_dal.Call!.DeleteAll();
                                break;
                        }
                        break;
                }
                userInput = menu();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    // Menu function to display the main menu and capture user's choice
    public static int menu()
    {
        Console.WriteLine("choose from next list");
        Console.WriteLine("1 - Choose Volunteer");
        Console.WriteLine("2 - Choose Assignment");
        Console.WriteLine("3 - Choose Call");
        Console.WriteLine("0 - exit");
        int firstmenu = int.Parse(Console.ReadLine()!);
        // Validating user input
        while (firstmenu < 0 || firstmenu > 3)
        {
            Console.WriteLine("ERROR\nEnter number again please");
            firstmenu = int.Parse(Console.ReadLine()!);
        }
        return firstmenu;
    }
    // Function to display a submenu for CRUD operations and capture user's choice
    public static int options()
    {
        // Displaying options for CRUD operations
        Console.WriteLine("Your Options");
        Console.WriteLine("1 - Create");
        Console.WriteLine("2 - Read");
        Console.WriteLine("3 - Read All");
        Console.WriteLine("4 - Update");
        Console.WriteLine("5 - Delete");
        Console.WriteLine("6 - DeleteAll");
        Console.WriteLine("0 - Back");

        int op = int.Parse(Console.ReadLine()!);
        while (op < 0 || op > 6)
        {
            Console.WriteLine("ERROR\nEnter number again please");
            op = int.Parse(Console.ReadLine()!);
        }
        return op;
    }

    /// Create a new Volunteer
    public static void creatVolunteer()
    {
        Console.WriteLine("Enter Volunteer ID");
        int id = int.Parse(Console.ReadLine()!);
        Console.WriteLine("Enter Volunteer Name");
        string FullName = Console.ReadLine()!;
        Console.WriteLine("Enter Volunteer Phone");
        string CallphoneNumber = Console.ReadLine()!;
        Console.WriteLine("Enter Volunteer Email");
        string Email = Console.ReadLine()!;
        Console.WriteLine("Enter Volunteer Password");
        int password = int.Parse(Console.ReadLine()!);
        Console.WriteLine("Enter Volunteer Address");
        string FullCurrentAddress = Console.ReadLine()!;
        Console.WriteLine("Enter Volunteer Latitude");
        double Latitude = double.Parse(Console.ReadLine()!);
        Console.WriteLine("Enter Volunteer Longitude");
        double Longitude = double.Parse(Console.ReadLine()!);
        Console.WriteLine("Enter Volunteer Current Position");
        User CurrentPosition = (User)Enum.Parse(typeof(User), Console.ReadLine()!);
        Console.WriteLine("Enter Volunteer Active");
        bool Active = bool.Parse(Console.ReadLine()!);
        Console.WriteLine("Enter Volunteer Max Distance For Call");
        double MaxDistanceForCall = double.Parse(Console.ReadLine()!);
        Console.WriteLine("Enter Volunteer Type Of Distance");
        Distance TypeOfDistance = (Distance)Enum.Parse(typeof(Distance), Console.ReadLine()!);

        DO.Volunteer volunteer = new DO.Volunteer(id, FullName, CallphoneNumber, Email
            , password.ToString(), FullCurrentAddress, Latitude, Longitude, CurrentPosition, Active, MaxDistanceForCall, TypeOfDistance);

        s_dal.Volunteer!.Create(volunteer);
    }

    // fuction to print the read function of Volunteer
    public static void PrintTheReadFunctionOfVolunteer(DO.Volunteer? Toprint)
    {
        if (Toprint == null)
        {
            Console.WriteLine("Volunteer not found");
            return;
        }
        else
        {
            Console.WriteLine("Volunteer ID: " + Toprint.id);
            Console.WriteLine("Volunteer Name: " + Toprint.FullName);
            Console.WriteLine("Volunteer Phone: " + Toprint.CallNumber);
            Console.WriteLine("Volunteer Email: " + Toprint.EmailAddress);
            Console.WriteLine("Volunteer Password: " + Toprint.Password);
            Console.WriteLine("Volunteer Address: " + Toprint.FullCurrentAddress);
            Console.WriteLine("Volunteer Latitude: " + Toprint.Latitude);
            Console.WriteLine("Volunteer Longitude: " + Toprint.Longitud);
            Console.WriteLine("Volunteer Current Position: " + Toprint.CurrentPosition);
            Console.WriteLine("Volunteer Active: " + Toprint.Active);
            Console.WriteLine("Volunteer Max Distance For Call: " + Toprint.MaxDistanceForCall);
            Console.WriteLine("Volunteer Type Of Distance: " + Toprint.TypeOfDistance);
        }

    }

    // Function to read all volunteers
    public static void readAllVolunteers()
    {
        var volunteers = s_dal.Volunteer!.ReadAll();
        foreach (var volunteer in volunteers)
        {
            PrintTheReadFunctionOfVolunteer(volunteer);
        }

    }
    // Function to update a volunteer
    public static Volunteer updateVolunteer()
    {

        Console.WriteLine("Enter Volunteer ID");
        int id = int.Parse(Console.ReadLine()!);
        Console.WriteLine("Enter Volunteer Name");
        string FullName = Console.ReadLine()!;
        Console.WriteLine("Enter Volunteer Phone");
        string CallphoneNumber = Console.ReadLine()!;
        Console.WriteLine("Enter Volunteer Email");
        string Email = Console.ReadLine()!;
        Console.WriteLine("Enter Volunteer Password");
        int password = int.Parse(Console.ReadLine()!);
        Console.WriteLine("Enter Volunteer Address");
        string FullCurrentAddress = Console.ReadLine()!;
        Console.WriteLine("Enter Volunteer Latitude");
        double Latitude = double.Parse(Console.ReadLine()!);
        Console.WriteLine("Enter Volunteer Longitude");
        double Longitude = double.Parse(Console.ReadLine()!);
        Console.WriteLine("Enter Volunteer Current Position");
        User CurrentPosition = (User)Enum.Parse(typeof(User), Console.ReadLine()!);
        Console.WriteLine("Enter Volunteer Active");
        bool Active = bool.Parse(Console.ReadLine()!);
        Console.WriteLine("Enter Volunteer Max Distance For Call");
        double MaxDistanceForCall = double.Parse(Console.ReadLine()!);
        Console.WriteLine("Enter Volunteer Type Of Distance");
        Distance TypeOfDistance = (Distance)Enum.Parse(typeof(Distance), Console.ReadLine()!);

        Volunteer temp = new Volunteer(id, FullName, CallphoneNumber, Email
            ,password.ToString(), FullCurrentAddress, Latitude, Longitude, CurrentPosition, Active, MaxDistanceForCall, TypeOfDistance);

        return temp;
    }



    // Function create a new Assignment
    public static void creatAssignment()
    {
        Console.WriteLine("Enter Assignment Call ID");
        int CallId = int.Parse(Console.ReadLine()!);
        Console.WriteLine("Enter Assignment Volunteer ID");
        int VolunteerId = int.Parse(Console.ReadLine()!);
        Console.WriteLine("Enter Assignment Entry Time");
        DateTime EntryTime = DateTime.Parse(Console.ReadLine()!);
        Console.WriteLine("Enter Assignment Completion Time");
        DateTime CompletionTime = DateTime.Parse(Console.ReadLine()!);


        Console.WriteLine(" Enter FinishType:");
        Console.WriteLine("choose from next list");
        Console.WriteLine("0 - Choose canceled of admin");
        Console.WriteLine("1 - Choose canceled of volunteer");
        Console.WriteLine("2 - Choose completed");
        Console.WriteLine("3 - Choose expired");
        Console.WriteLine("4 - Choose not finish (null)");
        int l = int.Parse(Console.ReadLine()!);
        while (l > 4 || l < 0)
        {
            Console.WriteLine("ERROR\nEnter number again please");
            l = int.Parse(Console.ReadLine()!);
        }

        DO.CompletionType? completionType = null;
        if (l != 4)
            completionType = (DO.CompletionType)l;

            DO.Assignment assignment = new Assignment(0, CallId, VolunteerId, EntryTime, CompletionTime, completionType);

        s_dal.Assignment!.Create(assignment);
    }

    // Function to print the read function of Volunteer
    public static void PrintTheReadFunctionOfAssignment(DO.Assignment? Toprint)
    {
        if (Toprint == null)
        {
            Console.WriteLine("Assignment not found");
            return;
        }
        else
        {
            Console.WriteLine("Assignment ID: " + Toprint.Id);
            Console.WriteLine("Assignment Call ID: " + Toprint.CallId);
            Console.WriteLine("Assignment Volunteer ID: " + Toprint.VolunteerId);
            Console.WriteLine("Assignment Entry Time: " + Toprint.StarCall);
            Console.WriteLine("Assignment Completion Time: " + Toprint.CompletionTime);
            Console.WriteLine("Assignment Finish Type: " + Toprint.FinishType);
        }
    }

    // Function to read all assignments
    public static void readAllAssignments()
    {
        var assignments = s_dal.Assignment!.ReadAll();
        foreach (var assignment in assignments)
        {
            PrintTheReadFunctionOfAssignment(assignment);
        }
    }


    // Function to update an assignment
    public static Assignment updateAssignment()
    {
        Console.WriteLine("Enter Assignment ID");
        int id = int.Parse(Console.ReadLine()!);
        Console.WriteLine("Enter Assignment Call ID");
        int CallId = int.Parse(Console.ReadLine()!);
        Console.WriteLine("Enter Assignment Volunteer ID");
        int VolunteerId = int.Parse(Console.ReadLine()!);
        Console.WriteLine("Enter Assignment Entry Time");
        DateTime EntryTime = DateTime.Parse(Console.ReadLine()!);
        Console.WriteLine("Enter Assignment Completion Time");
        DateTime CompletionTime = DateTime.Parse(Console.ReadLine()!);

        Assignment assignment = new Assignment(id, CallId, VolunteerId, EntryTime, CompletionTime, 0);


        return assignment;
    }



    // Function to create a new Call
    public static void creatCall()
    {
        Console.WriteLine();
        Console.WriteLine("Enter Call Type");
        Console.WriteLine("choose from next list");
        Console.WriteLine("0 - Choose MakingFood");
        Console.WriteLine("1 - Choose DeliverFood");
        int l = int.Parse(Console.ReadLine()!);
        if (l > 1 || l < 0)
        {
            Console.WriteLine("ERROR\nEnter number again please");
            l = int.Parse(Console.ReadLine()!);
        }
        DO.CallType CallType = (DO.CallType)l;

        Console.WriteLine("Enter Call Description");
        string description = Console.ReadLine()!;
        Console.WriteLine("Enter Call Full Address");
        string fullAddress = Console.ReadLine()!;
        Console.WriteLine("Enter Call Latitude");
        double latitude = double.Parse(Console.ReadLine()!);
        Console.WriteLine("Enter Call Longitude");
        double longitude = double.Parse(Console.ReadLine()!);
        Console.WriteLine("Enter Call Max Time For Call");
        DateTime maxTimeForCall = DateTime.Parse(Console.ReadLine()!);

        Call call = new Call(0, CallType, fullAddress, latitude, longitude, s_dal.Config.Clock, description, maxTimeForCall);

        s_dal.Call!.Create(call);
    }

    // Function to print the read function of Call
    public static void PrintTheReadFunctionOfCall(DO.Call? Toprint)
    {
        if (Toprint == null)
        {
            Console.WriteLine("Call not found");
            return;
        }
        else
        {
            Console.WriteLine("Call ID: " + Toprint.Id);
            Console.WriteLine("Call Type: " + Toprint.Type);
            Console.WriteLine("Call Description: " + Toprint.description);
            Console.WriteLine("Call Full Address: " + Toprint.FullAddress);
            Console.WriteLine("Call Latitude: " + Toprint.Latitude);
            Console.WriteLine("Call Longitude: " + Toprint.Longitude);
            Console.WriteLine("Call Start Time: " + Toprint.CallStartTime);
            Console.WriteLine("Call Max Time For Call: " + Toprint.MaxTimeForCall);
        }
    }

    // Function to read all calls
    public static void readAllCalls()
    {
        var calls = s_dal.Call!.ReadAll();
        foreach (var call in calls)
        {
            PrintTheReadFunctionOfCall(call);
        }
    }

    // Function to update a call
    public static Call updateCall()
    {
        Console.WriteLine("Enter Call ID");
        int id = int.Parse(Console.ReadLine()!);
        Console.WriteLine("Enter Call Type");
        Console.WriteLine("choose from next list");
        Console.WriteLine("0 - Choose MakingFood");
        Console.WriteLine("1 - Choose DeliverFood");
        int l = int.Parse(Console.ReadLine()!);
        while (l != 1 && l != 0)
        {
            Console.WriteLine("ERROR\nEnter number again please");
            l = int.Parse(Console.ReadLine()!);
        }
        DO.CallType callType = (DO.CallType)l;
        Console.WriteLine("Enter Call Description");
        string description = Console.ReadLine()!;
        Console.WriteLine("Enter Call Full Address");
        string fullAddress = Console.ReadLine()!;
        Console.WriteLine("Enter Call Latitude");
        double latitude = double.Parse(Console.ReadLine()!);
        Console.WriteLine("Enter Call Longitude");
        double longitude = double.Parse(Console.ReadLine()!);
        Console.WriteLine("Enter Call Max Time For Call");
        DateTime maxTimeForCall = DateTime.Parse(Console.ReadLine()!);

        Call call = new Call(id, callType, fullAddress, latitude, longitude, s_dal.Config.Clock, description, maxTimeForCall);

        s_dal.Call!.Update(call);

        return call;

    }

}
