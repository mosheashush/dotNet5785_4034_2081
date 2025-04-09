
using DalApi;
using DO;
using System.Xml.Linq;

namespace Dal
{
    internal class VolunteerImplementation : IVolunteer
    {
        public void Create(Volunteer item)
        {
            List<Volunteer> volunteers = XMLTools.LoadListFromXMLSerializer<Volunteer>(Config.s_volunteers_xml);
            if (volunteers.FirstOrDefault(v => v.id == item.id) != null)
                throw new DalAlreadyExistsException($"Volunteer with the same ID={item.id} already exists...");
            else
            {
                volunteers.Add(item);
                XMLTools.SaveListToXMLSerializer<Volunteer>(volunteers, Config.s_volunteers_xml);
            }
        }

        public void Delete(int id)
        {
            List<Volunteer> volunteers = XMLTools.LoadListFromXMLSerializer<Volunteer>(Config.s_volunteers_xml);
            if (volunteers.RemoveAll(it => it.id == id) == 0)
                throw new DalDoesNotExistException($"Volunteer with ID={id} does Not exist");
            XMLTools.SaveListToXMLSerializer(volunteers, Config.s_volunteers_xml);
        }

        public void DeleteAll()
        {
            XMLTools.SaveListToXMLSerializer(new List<Volunteer>(), Config.s_volunteers_xml);
        }

        public Volunteer? Read(int id)
        {
            List<Volunteer> volunteers = XMLTools.LoadListFromXMLSerializer<Volunteer>(Config.s_volunteers_xml);

            Volunteer? volunteer = volunteers.FirstOrDefault(v => v.id == id);

            return volunteer;
        }

        public Volunteer? Read(Func<Volunteer, bool> filter)
        {
            List<Volunteer> volunteers = XMLTools.LoadListFromXMLSerializer<Volunteer>(Config.s_volunteers_xml);
            Volunteer? volunteer = volunteers.FirstOrDefault(filter);
            return volunteer;
        }

        public IEnumerable<Volunteer>? ReadAll(Func<Volunteer,bool>? filter = null)
        {
       
            List<Volunteer> volunteers = XMLTools.LoadListFromXMLSerializer<Volunteer>(Config.s_volunteers_xml);
            IEnumerable<Volunteer> volunteer_s = volunteers;
            return filter == null ? volunteer_s : volunteer_s.Where(filter);

        }

        public void Update(Volunteer item)
        {
            List<Volunteer> volunteers = XMLTools.LoadListFromXMLSerializer<Volunteer>(Config.s_volunteers_xml);
            if (volunteers.Remove(volunteers.FirstOrDefault(v => v.id == item.id)))
            {
                volunteers.Add(item);
                XMLTools.SaveListToXMLSerializer(volunteers, Config.s_volunteers_xml);
            }
            else
            {
                throw new DalDoesNotExistException($"Volunteer with ID={item.id} does Not exist");
            }
        }

        // private function to convert Volunteer to XElement
        static  Volunteer getVolunteer(XElement item)
        {
            return new DO.Volunteer()
            {
                id = item.ToIntNullable("id") ?? throw new FormatException("can't convert id"),
                FullName = (string?)item.Element("FullName") ?? "",
                CallNumber = (string?)item.Element("CallNumber") ?? "",
                EmailAddress = (string?)item.Element("EmailAddress") ?? "",
                Password = (string?)item.Element("Password") ?? "",
                FullCurrentAddress = (string?)item.Element("FullCurrentAddress") ?? "",
                Latitude = item.ToDoubleNullable("Latitude") ?? throw new FormatException("can't convert Latitude"),
                Longitud = item.ToDoubleNullable("Longitud") ?? throw new FormatException("can't convert Longitud"),
                CurrentPosition = (User)(item.ToIntNullable("CurrentPosition") ?? throw new FormatException("can't convert CurrentPosition")),
                Active = (bool?)item.Element("IsActive") ?? false,
                MaxDistanceForCall = item.ToDoubleNullable("MaxDistanceForCall") ?? throw new FormatException("can't convert MaxDistanceForCall"),
                TypeOfDistance = (Distance)(item.ToIntNullable("TypeOfDistance") ?? throw new FormatException("can't convert TypeOfDistance")),

            };
        }


    }
}
