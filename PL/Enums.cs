using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PL
{
    internal class VolunteerFilterCollection : IEnumerable
    {
        static readonly IEnumerable<BO.CallType> s_enums =
            (Enum.GetValues(typeof(BO.CallType)) as IEnumerable<BO.CallType>)!;

        public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
    }

    internal class UserCollection : IEnumerable
    {
        static readonly IEnumerable<BO.User> s_enums =
        (Enum.GetValues(typeof(BO.User)) as IEnumerable<BO.User>)!;

        public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
    }

    internal class DistanceCollection : IEnumerable
    {
        static readonly IEnumerable<BO.Distance> s_enums =
        (Enum.GetValues(typeof(BO.Distance)) as IEnumerable<BO.Distance>)!;

        public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
    }

    internal class CallStateCollection : IEnumerable
    {
        static readonly IEnumerable<BO.CallState> s_enums =
        (Enum.GetValues(typeof(BO.CallState)) as IEnumerable<BO.CallState>)!;

        public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
    }
}