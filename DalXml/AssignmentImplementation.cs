using Dal;
using DalApi;
using DO;
using System.Diagnostics;
using System.Xml.Linq;

namespace Dal
{
    internal class AssignmentImplementation : IAssignment
    {
        private XElement? createAssignmentElement(Assignment item)
        {
            return new XElement("Assignment",
                new XElement("Id", item.Id),
                new XElement("CallId", item.CallId),
                new XElement("VolunteerId", item.VolunteerId),
                new XElement("StarCall", item.StarCall),
                new XElement("CompletionTime", item.CompletionTime),
                new XElement("FinishType", item.FinishType)
                );
        }
        // getAssignment
        static Assignment getAssignment(XElement assignment)
        {
            XElement? elem(string name) =>
                assignment.Elements()
                          .FirstOrDefault(e => string.Equals(e.Name.LocalName, name, StringComparison.OrdinalIgnoreCase));

            int parseInt(string name)
            {
                var e = elem(name) ?? throw new ArgumentNullException($"{name} element is missing");
                if (!int.TryParse(e.Value, out var v)) throw new FormatException($"{name} is not a valid int: '{e.Value}'");
                return v;
            }

            DateTime parseDate(string name)
            {
                var e = elem(name) ?? throw new ArgumentNullException($"{name} element is missing");
                if (!DateTime.TryParse(e.Value, out var dt)) throw new FormatException($"{name} is not a valid DateTime: '{e.Value}'");
                return dt;
            }

            // parse enum case-insensitive
            CompletionType? finishType = null;
            var ftElem = elem("FinishType");
            if (ftElem != null && !string.IsNullOrWhiteSpace(ftElem.Value))
            {
                if (Enum.TryParse<CompletionType>(ftElem.Value, ignoreCase: true, out var parsed))
                    finishType = parsed;
                else
                    Debug.WriteLine($"Warning: unknown FinishType value '{ftElem.Value}'");
            }

            return new DO.Assignment()
            {
                Id = parseInt("Id"),
                CallId = parseInt("CallId"),
                VolunteerId = parseInt("VolunteerId"),
                StarCall = parseDate("StarCall"),
                CompletionTime = string.IsNullOrWhiteSpace(assignment.Element("CompletionTime")?.Value)
            ? null
            : DateTime.Parse(assignment.Element("CompletionTime")!.Value),

                FinishType = string.IsNullOrWhiteSpace(assignment.Element("FinishType")?.Value)
            ? null
            : Enum.Parse<CompletionType>(assignment.Element("FinishType")!.Value)
            };
        }

        public void Create(Assignment item)
        {
            if (item.Id == 0)
                item = item.WithId(Config.NextAssignmentId);

            XElement assignmentRootElem = XMLTools.LoadListFromXMLElement(Config.s_assignments_xml);

            XElement newAssignment = createAssignmentElement(item) ?? throw new InvalidOperationException("Failed to create assignment element");
            assignmentRootElem.Add(newAssignment);

            XMLTools.SaveListToXMLElement(assignmentRootElem, Config.s_assignments_xml);
        }

        public void Delete(int id)
        {
            // Implementation Delete assignment by XElement

            XElement assignmentRootElem = XMLTools.LoadListFromXMLElement(Config.s_assignments_xml);

            XElement? assignmentElem = assignmentRootElem.Elements().FirstOrDefault(a => (int?)a.Element("Id") == id);
            if (assignmentElem == null)
                throw new DO.DalDoesNotExistException($"Assignment with ID={id} does Not exist");

            assignmentElem.Remove();
            XMLTools.SaveListToXMLElement(assignmentRootElem, Config.s_assignments_xml);
        }

        public void DeleteAll()
        {
            // Implementation Delete assignment by XElement

            XElement assignmentRootElem = XMLTools.LoadListFromXMLElement(Config.s_assignments_xml);
            assignmentRootElem.RemoveAll();
            XMLTools.SaveListToXMLElement(assignmentRootElem, Config.s_assignments_xml);
        }

        public Assignment? Read(int id)
        {
            // Implementation Read assignment by XElement
            XElement assignmentRootElem = XMLTools.LoadListFromXMLElement(Config.s_assignments_xml);
            XElement? assignmentElem = XMLTools.LoadListFromXMLElement(Config.s_assignments_xml).Elements().
                                       FirstOrDefault(st => (int?)st.Element("Id") == id);
            if (assignmentElem == null)
                throw new DO.DalDoesNotExistException($"Assignment with ID={id} does Not exist");
            return getAssignment(assignmentElem);
        }

        public Assignment? Read(Func<Assignment, bool> filter)
        {
            return XMLTools.LoadListFromXMLElement(Config.s_assignments_xml).Elements().Select(a => getAssignment(a)).FirstOrDefault(filter);
        }

        // ReadAll
        public IEnumerable<Assignment> ReadAll(Func<Assignment, bool>? filter = null)
        {
            var root = XMLTools.LoadListFromXMLElement(Config.s_assignments_xml);
            if (root == null) return Enumerable.Empty<Assignment>();

            // select all Assignment elements, case-insensitive
            var elems = root.Elements()
                            .Where(e => string.Equals(e.Name.LocalName, "Assignment", StringComparison.OrdinalIgnoreCase))
                            .ToList();

            var result = new List<Assignment>();
            foreach (var el in elems)
            {
                try
                {
                    var a = getAssignment(el);
                    if (a != null && (filter == null || filter(a)))
                        result.Add(a);
                }
                catch (Exception ex)
                {
                    // if parsing of this element fails, log and skip it
                    Debug.WriteLine($"Failed parsing assignment element: {ex.Message}\nElement: {el}");
                }
            }

            return result;
        }
        public void Update(Assignment item)
        {
            XElement assignmentsRootElem = XMLTools.LoadListFromXMLElement(Config.s_assignments_xml);

            (assignmentsRootElem.Elements().FirstOrDefault(st => (int?)st.Element("Id") == item.Id)
            ?? throw new DO.DalDoesNotExistException($"Assignment with ID={item.Id} does Not exist"))
                    .Remove();

            assignmentsRootElem.Add(createAssignmentElement(item));

            XMLTools.SaveListToXMLElement(assignmentsRootElem, Config.s_assignments_xml);
        }
    }
}
