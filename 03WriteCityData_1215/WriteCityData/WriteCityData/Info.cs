using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WriteCityData
{
    internal class Info
    {

    }

    public class DtDept
    {
        public string DEPTNO { get; set; }
        public string DEPTNAME { get; set; }
        public string INDEPTNO { get; set; }
        public string GRADE { get; set; }
        public string ADDR { get; set; }
        public string X { get; set; }
        public string Y { get; set; }
        public string LDATE { get; set; }
        public string LUSER { get; set; }
        public string DELETED { get; set; }
        public string SORTING { get; set; }
        public string ISSAVED { get; set; }
    }

    public class DtEmpl
    {
        public string USERID { get; set; }
        public string PWD { get; set; }
        public string NAME { get; set; }
        public string COUNTRY { get; set; }
        public string DEPTNO { get; set; }
        public string JOBTITLE { get; set; }
        public string JOBTYPE { get; set; }
        public string ONDUTY { get; set; }
        public string EMAIL { get; set; }
        public string PRETEL { get; set; }
        public string TEL { get; set; }
        public string LASTTEL { get; set; }
        public string CELLPHONE { get; set; }
        public string LDATE { get; set; }
        public string LUSER { get; set; }
        public string ROLEID { get; set; }
        public string DELETED { get; set; }
        public string NEWDEPTNO { get; set; }
        public string NEWROLEID { get; set; }
        public string CHECKROLE { get; set; }
        public string IDNO { get; set; }
        public string IDNOKEY { get; set; }
        public string MEMID { get; set; }
    }

    public class DtEmplMap
    {
        public string COUNTRY { get; set; }
        public string USERID { get; set; }
        public string NDPPC_UID { get; set; }
        public string LDATE { get; set; }
        public string LUSER { get; set; }
    }
}
