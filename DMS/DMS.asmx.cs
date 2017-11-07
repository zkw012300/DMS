using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;

namespace DMS
{
    /// <summary>
    /// DMS 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://zspirytus.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消注释以下行。 
    // [System.Web.Script.Services.ScriptService]
    public class DMS : System.Web.Services.WebService
    {
        SQLDataBaseUtils sql = new SQLDataBaseUtils();

        [WebMethod]
        public bool inserttoAssets()
        {
            return sql.inserttoAssets();
        }


        [WebMethod]
        public string[] getStudentBasicInfo() 
        {
            return sql.getBasicInfo().ToArray();
        }

        [WebMethod]
        public bool inserttoStudent()
        {
            return sql.inserttoStudent();
        }

        [WebMethod]
        public bool inserttoAdmin()
        {
            return sql.inserttoAdmin();
        }

        [WebMethod]
        public bool inserttoDormitory()
        {
            return sql.inserttoDormitory();
        }

        [WebMethod]
        public string[] getBasicInfoBySno(string Sno) 
        {
            return sql.getBasicInfoBySno(Sno).ToArray();
        }

        [WebMethod]
        public bool inserttoRepair(string sno, string repairNo, string repairArea, string repairPlace, string repairType, string detail, string contact, string photos)
        {
            return sql.inserttoRepair(sno,repairNo,repairArea,repairPlace,repairType,detail,contact,photos);
        }

        [WebMethod]
        public bool Reg(string sno, string account, string pwd) 
        {
            return sql.Reg(sno, account, pwd);
        }

        [WebMethod]
        public bool insertintoStudentLeaveSchool(string sno, string leaveDate, string backDate, string reason)
        {
            return sql.insertintoSLS(sno, leaveDate, backDate, reason);
        }
    }
}
