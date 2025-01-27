namespace Dal;
using DalApi;
using DO;
using System.Security.Cryptography;

public static class Initialization
{
    private static IVolunteer? s_dalVolunteer;
    private static IAssignment? s_dalAssignment;
    private static ICall? s_dalCall;
    private static IConfig? s_dalConfig;

    private static readonly Random s_rand = new();
    private static string[,] data = new string[20, 4]
        {
                { "David Cohen", "Jerusalem, Havad Haleumi 21", "31.777741", "35.203321" },
                { "Sarah Levy", "Tel Aviv, Ibn Gabirol 69", "32.085299", "34.781768" },
                { "Michael Mizrahi", "Haifa, HaAliya Hashniya 1", "32.818437", "34.988504" },
                { "Rachel Shimon", "Be'er Sheva, Yitzhak Nafha 25", "31.243870", "34.792345" },
                { "Daniel Kaplan", "Eilat, Tarshish 7", "29.558050", "34.948210" },
                { "Leah Abramov", "Ashdod, HaOrgim 2", "31.804381", "34.649288" },
                { "Jonathan Greenberg", "Netanya, David Hamelech 15", "32.331759", "34.852056" },
                { "Rebecca Rosen", "Petah Tikva, Haim Ozer 25", "32.087439", "34.887668" },
                { "Joshua Katz", "Holon, Mota Gur 6", "32.011540", "34.772210" },
                { "Miriam Weiss", "Bat Yam, HaKomemiyut 10", "32.023010", "34.750772" },
                { "Aaron Gold", "Herzliya, HaShunit 1", "32.166300", "34.843630" },
                { "Esther Rubin", "Rishon Lezion, Moshe Dayan 15", "31.964800", "34.802100" },
                { "Samuel Stern", "Kfar Saba, Weizmann 112", "32.175000", "34.906600" },
                { "Deborah Azulay", "Ra'anana, HaPark 5", "32.184800", "34.871300" },
                { "Jacob Friedman", "Modi'in, Emek Hachula 10", "31.899600", "35.013300" },
                { "Naomi Blum", "Ramat Gan, Abba Hillel 16", "32.068400", "34.824800" },
                { "Eli Shwartz", "Givatayim, Katznelson 3", "32.069800", "34.811700" },
                { "Hannah Berger", "Lod, Herzl 12", "31.951300", "34.895100" },
                { "Joseph Peretz", "Bnei Brak, Jabotinsky 20", "32.085000", "34.839000" },
                { "Tamar Halevi", "Ashkelon, Bar Kokhba 18", "31.668800", "34.574300" }
        };
    private static void createVolunteer()
    {

        // Create 20 volunteers with random data
        for (int i = 0; i < 20; i++)
        {
            int id, numberphone, password;
            do
            {
                id = s_rand.Next(20000000, 40000000); // 8 digits
                numberphone = s_rand.Next(500000000, 599999999); // 9 digits
                password = s_rand.Next(100000, 999999); // 6 digits
            }
            while (s_dalVolunteer!.Read(id) != null);

            string name = data[i, 0];
            bool even = id % 2 == 0;
            string email = name.Replace(" ", "").ToLower() + (even ? "1" : "0") + "@gmail.com";
            User po = (i % 9) == 0 ? User.volnteer : User.admin;
            Distance di;
            if (i % 3 == 0)
                di = Distance.air;
            else if (i % 3 == 1) di = Distance.walk;
            else
                di = Distance.drive;
            var volunteer = new Volunteer
            {
                id = id,
                FullName = name,
                CallNumber = "0" + numberphone,
                EmailAddress = email,
                Password = password.ToString(),
                FullCurrentAddress = data[i, 1],
                Latitude = double.Parse(data[i, 2]),
                Longitud = double.Parse(data[i, 3]),
                CurrentPosition = po,
                Active = (i + 1) % 9 == 0,
                MaxDistanceForCall = s_rand.Next(1, 10),
                TypeOfDistance = di,
            };

            s_dalVolunteer!.Create(volunteer);


        }
    }
    private static void createAssignment()
    {
        // Create 20 assignments with random data
        List<int> list = s_dalVolunteer!.ReadAll().Select(v => v.id).ToList();
        List<int> callList = s_dalCall!.ReadAll().Select(c => c.Id).ToList();
        for (int i = 1; i < 15; i++)
        {
            int idcall, id_volunteer, finishtype;
            CompletionType type = 0;
            finishtype = s_rand.Next(0, 4);
            idcall = callList[s_rand.Next(0, callList.Count)];
            id_volunteer = list[s_rand.Next(0, list.Count)];
            switch (finishtype)
            {
                case 0:
                    type = CompletionType.canceledAdmin;
                    break;
                case 1:
                    type = CompletionType.canceledVolunteer;
                    break;
                case 2:
                    type = CompletionType.completed;
                    break;
                case 3:
                    type = CompletionType.expired;
                    break;
            }
            Assignment assignment = new Assignment
            {
                CallId = idcall,
                VolunteerId = id_volunteer,
                StarCall = DateTime.Now,
                CompletionTime = DateTime.Now,
                FinishType = type,
            };
            s_dalAssignment!.Create(assignment);
            list.Remove(id_volunteer);
            callList.Remove(idcall);

        }
    }
    private static void createCall()
    {
        // Create 20 calls with random data
        for (int i = 1; i < 15; i++)
        {
            int type = s_rand.Next(0, 1);

            Call call = new Call
            {
                Type = (type == 0) ? CallType.makingfood : CallType.deliveringfood,
                description = null,
                FullAddress = data[i, 1],
                Latitude = double.Parse(data[i, 2]),
                Longitude = double.Parse(data[i, 3]),
                CallStartTime = DateTime.Now,
                MaxTimeForCall = null,
            };
            s_dalCall!.Create(call);
        }
    }



    public static void Do(IVolunteer? dalVolunteer, IAssignment? dalAssignment, ICall? dalCall, IConfig? dalConfig)
    {
        s_dalVolunteer = dalVolunteer ?? throw new NullReferenceException("DALvolunteer object can not be null! ");
        s_dalAssignment = dalAssignment ?? throw new NullReferenceException("DALassignment object can not be null! ");
        s_dalCall = dalCall ?? throw new NullReferenceException("DALcall object can not be null! ");
        s_dalConfig = dalConfig ?? throw new NullReferenceException("DALconfig object can not be null! ");

        

        Console.WriteLine("Reset Configuration values and List values...");

        s_dalVolunteer.DeleteAll();
        s_dalAssignment.DeleteAll();
        s_dalCall.DeleteAll();
        s_dalConfig.Reset();

        Console.WriteLine("Initializing all list ...");
        createVolunteer();
        createAssignment();
        createCall();

    }
}

