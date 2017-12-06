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

        [WebMethod(Description = "注册账号")]
        public bool registerAccount(string Sno, string account, string pwd)
        {
            return sql.Reg(Sno, account, pwd);
        }

        [WebMethod(Description = "通过学号获取学生基本信息")]
        public string[] getBasicInfoBySno(string Sno) 
        {
            return sql.getBasicInfo(Sno).ToArray();
        }

        [WebMethod(Description = "通过学号获取报修基本信息")]
        public string[] getRepairBasicInfoBySno(string Sno)
        {
            return sql.getRepairBasicInfoBySno(Sno);
        }

        [WebMethod(Description = "通过学号获取离校登记基本信息")]
        public string[] getSLSBasicInfo(string Sno)
        {
            return sql.getSLSBasicInfo(Sno).ToArray();
        }

        //has not implements
        [WebMethod(Description = "通过学号获取夜归基本信息")]
        public string[] getReturnLatelyBasicInfo(string Sno)
        {
            return sql.getRLBasicInfo(Sno).ToArray();
        }

        [WebMethod(Description = "新的离校登记")]
        public bool newStudentLeavingSchool(string Sno,string SLSNo, string leaveDate, string backDate, string reason)
        {
            return sql.insertintoSLS(Sno, SLSNo, leaveDate, backDate, reason);
        }

        [WebMethod(Description = "新的报修")]
        public bool newRepairReport(string Sno, string repairNo, string repairArea, string repairPlace, string repairType, string detail, string contact, string photo,string reportDate)
        {
            return sql.inserttoRepair(Sno, repairNo, repairArea, repairPlace, repairType, detail, contact, photo, reportDate);
        }

        [WebMethod(Description = "新的夜归申请")]
        public bool newReturnLately(string Sno, string Rno, string returnTime, string reason)
        {
            return sql.insertintoReturnLately(Sno, Rno, returnTime, reason);
        }

        [WebMethod(Description = "更新头像")]
        public bool updateAvatar(string Sno, string photo)
        {
            return sql.updateAvatar(Sno, photo);
        }

        [WebMethod(Description = "获取账号对应的学号")]
        public string getSnobyAccount(string account,string pwd) 
        {
            return sql.getSnobyAccount(account,pwd);
        }

        [WebMethod(Description = "修改密码")]
        public bool ModifyPwd(string account, string pwd)
        {
            return sql.ModifyPwd(account, pwd);
        }

        [WebMethod(Description = "获取头像")]
        public string getAvatar(string Sno)
        {
            return sql.getAvatar(Sno);
        }

        /*
        SqlInfoInitializor initializor = new SqlInfoInitializor();
        
        [WebMethod(Description = "插入学生表初始信息")]
        public bool insertintoStudent() 
        {
            return sql.inserttoStudent();
        }

        [WebMethod(Description = "插入职工表初始信息")]
        public bool insertintoAdmin()
        {
            return sql.inserttoAdmin();
        }

        [WebMethod(Description = "插入财产表初始信息")]
        public bool inserttoAssets() 
        {
            return sql.inserttoAssets();
        }

        [WebMethod(Description = "插入宿舍表和宿舍所有财产表初始信息")]
        public bool inserttoDormitoryAndDA()
        {
            return sql.inserttoDormitoryAndDA();
        }

        [WebMethod(Description = "插入宿舍管理表初始信息")]
        public bool insertintoManagement()
        {
            return sql.insertintoManagement();
        }

        [WebMethod(Description = "插入学生住宿信息表初始信息")]
        public bool insertintoSD()
        {
            return initializor.insertintoSD();
        }*/
    }
}
