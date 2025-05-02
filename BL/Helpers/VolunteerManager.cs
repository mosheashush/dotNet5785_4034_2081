using DalApi;
using System.Net;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.IO;



namespace Helpers;

internal static class VolunteerManager
{
    private static IDal s_dal = Factory.Get; //stage 4

    //CalculatSumCallsCompleted implementation
    public static int CalculatSumCallsCompleted(int id)
    {
        if (s_dal.Assignment.ReadAll() == null) return 0;

        return s_dal.Assignment.ReadAll().Where(c => c.VolunteerId == id && c.FinishType == DO.CompletionType.completed).Count();
    }

    //CalculatSumCallsExpired implementation:
    public static int CalculatSumCallsExpired(int id)
    {
        if (s_dal.Assignment.ReadAll() == null) return 0;

        return s_dal.Assignment.ReadAll().Where(c => c.VolunteerId == id && c.FinishType == DO.CompletionType.expired).Count();
    }

    //CalculatSumCallsConcluded implementation:
    public static int CalculatSumCallsConcluded(int id)
    {
        if (s_dal.Assignment.ReadAll() == null) return 0;

        return s_dal.Assignment.ReadAll().Where(c => c.VolunteerId == id && c.FinishType == DO.CompletionType.canceledVolunteer).Count();
    }

    public static BO.CallInProgress? IfCallInProgress(int id)
    {
        DO.Assignment assignment = s_dal.Assignment.ReadAll().FirstOrDefault(c => c.VolunteerId == id && c.FinishType == null); // volunteer can take only one call at a time
        if (assignment == null)
            return null;
        else
        {
            DO.Call call = s_dal.Call.ReadAll().Where(c => c.Id == assignment.CallId).FirstOrDefault();
            DO.Volunteer volunteer = s_dal.Volunteer.Read(id);
            return new BO.CallInProgress()
            {
                IdCall = assignment.CallId,
                IdAssignment = assignment.Id,
                Type = (BO.CallType)call.Type,
                description = call.description,
                FullAddress = call.FullAddress,
                CallStartTime = call.CallStartTime,
                MaxTimeForCall = call.MaxTimeForCall,
                VolunteerTakeCall = assignment.StarCall,
                DistanceFromVolunteer = GetDistanceInKm(call.Latitude, call.Longitude, volunteer.Latitude, volunteer.Longitud),
                CollState = GetCallState(call),
            };
        }
    }
    public static double GetDistanceInKm(double? lat1, double? lon1, double? lat2, double? lon2)
    {
        if (lat1 == null || lat2 == null || lon1 == null || lon2 == null)
            throw new ArgumentNullException("Latitude and Longitude is null");
        else
        {
            double R = 6371; // radius of Earth in kilometers  
            double dLat = DegreesToRadians(lat2.Value - lat1.Value);
            double dLon = DegreesToRadians(lon2.Value - lon1.Value);

            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                       Math.Cos(DegreesToRadians(lat1.Value)) * Math.Cos(DegreesToRadians(lat2.Value)) *
                       Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c;
        }
    }

    private static double DegreesToRadians(double deg)
    {
        return deg * (Math.PI / 180);
    }

    public static BO.CallState GetCallState(DO.Call call)
    {
        if (call.MaxTimeForCall > ClockManager.Now)
            return BO.CallState.processed;
        else
            return BO.CallState.ProcessedOnRisk;
    }

    public static bool IsValidIsraeliId(int id)
    {
        if (id <= 0 || id > 999999999)
            return false;

        string idStr = id.ToString().PadLeft(9, '0');

        int sum = 0;

        for (int i = 0; i < 9; i++)
        {
            int digit = idStr[i] - '0';
            int mult = (i % 2 == 0) ? 1 : 2;
            int product = digit * mult;

            if (product > 9)
                product -= 9;

            sum += product;
        }

        return sum % 10 == 0;
    }
    //IsValidEmail
    public static bool IsValidEmail(string email)
    {
        // Regular expression for validating an Email
        string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
        return Regex.IsMatch(email, pattern);
    }
    //internal static (double Latitude, double Longitude) GetCoordinatesFromAddress(string address)
    internal static bool GetCoordinatesFromAddress(BO.Volunteer boVolunteer)
    {
        if (string.IsNullOrWhiteSpace(boVolunteer.FullCurrentAddress))
            throw new ArgumentException("address is null and not valid");

        string url = $"https://geocode.maps.co/search?q={Uri.EscapeDataString(boVolunteer.FullCurrentAddress)}";

        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
        request.Method = "GET";

        using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
        using (Stream stream = response.GetResponseStream())
        using (StreamReader reader = new StreamReader(stream))
        {
            string json = reader.ReadToEnd();
            JsonDocument doc = JsonDocument.Parse(json);

            var results = doc.RootElement;
            if (results.GetArrayLength() == 0)
                return false;

            var firstResult = results[0];
            boVolunteer.Latitude = double.Parse(firstResult.GetProperty("lat").GetString()!);
            boVolunteer.Longitude = double.Parse(firstResult.GetProperty("lon").GetString()!);
            
            return true;
        }
    }
    //IsValidAddress
    public static bool IsValidAddress(string address)
    {
        // Regular expression for validating an address
        string pattern = @"^[a-zA-Z0-9\s,.'-]{5,}$";
        return Regex.IsMatch(address, pattern);
    }
    //CheckVolunteer
    public static bool CheckVolunteer(BO.Volunteer boVolunteer)
    {
        //id
        if (IsValidIsraeliId(boVolunteer.id))
            throw new BO.BlInvalidValueException($"Volunteer ID={boVolunteer.id} is not valid");

        //CallNumber
        if (boVolunteer.CallNumber.Length != 10 || boVolunteer.CallNumber[0] != 0 || boVolunteer.CallNumber[1] != 5)
            throw new BO.BlInvalidValueException($"Call number={boVolunteer.CallNumber} need to be 10 digits valid");

        //email
        if (IsValidEmail(boVolunteer.EmailAddress))
            throw new BO.BlInvalidValueException($"Email address={boVolunteer.EmailAddress} is not valid");

        //password
        if (boVolunteer.Password.Length < 8)
            throw new BO.BlInvalidValueException($"Password={boVolunteer.Password} need to be minimom 8 digits");

        //address
        if (IsValidAddress(boVolunteer.FullCurrentAddress))
            throw new BO.BlInvalidValueException($"Full address={boVolunteer.FullCurrentAddress} is not valid");

        //MaxDistanceForCall
        if (boVolunteer.MaxDistanceForCall < 0)
            throw new BO.BlInvalidValueException($"Max distance for call={boVolunteer.MaxDistanceForCall} is not valid");

        return true;
    }
    //MapBOToDOVolunteer
    public static DO.Volunteer MapBOToDOVolunteer(BO.Volunteer boVolunteer)
    {
        return new DO.Volunteer()
        {
            id = boVolunteer.id,
            FullName = boVolunteer.FullName,
            CallNumber = boVolunteer.CallNumber,
            EmailAddress = boVolunteer.EmailAddress,
            Password = boVolunteer.Password,
            FullCurrentAddress = boVolunteer.FullCurrentAddress,
            Latitude = boVolunteer.Latitude,
            Longitud = boVolunteer.Longitude,
            CurrentPosition = (DO.User)boVolunteer.CurrentPosition,
            Active = boVolunteer.Active,
            MaxDistanceForCall = boVolunteer.MaxDistanceForCall,
            TypeOfDistance = (DO.Distance)boVolunteer.TypeOfDistance,
        };
    }

}

