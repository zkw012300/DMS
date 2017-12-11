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
        public bool registerAccount(string Sno, string account, string pwd,int type)
        {
            return sql.Reg(Sno, account, pwd, type);
        }

        [WebMethod(Description = "通过学号获取学生摘要信息")]
        public string[] getBasicInfoBySno(string Sno) 
        {
            return sql.getBasicInfo(Sno).ToArray();
        }

        [WebMethod(Description = "通过学号获取报修摘要信息")]
        public string[] getRepairBasicInfoBySno(string Sno)
        {
            return sql.getRepairBasicInfoBySno(Sno);
        }

        [WebMethod(Description = "通过学号获取离校登记摘要信息")]
        public string[] getSLSBasicInfo(string Sno)
        {
            return sql.getSLSBasicInfo(Sno).ToArray();
        }

        //has not implements
        [WebMethod(Description = "通过学号获取夜归摘要信息")]
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
        public string newReturnLately(string Sno, string Rno, string returnTime, string reason)
        {
            return sql.insertintoReturnLately(Sno, Rno, returnTime, reason);
        }

        [WebMethod(Description = "更新头像")]
        public bool updateAvatar(string Sno, string photo)
        {
            return sql.updateAvatar(Sno, photo);
        }

        [WebMethod(Description = "获取账号对应的学号")]
        public string getNo(string account, string pwd) 
        {
            return sql.getNo(account, pwd);
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

        [WebMethod(Description = "获取报修详细信息")]
        public string[] getRepairDetailsInfoBySno(string Rno)
        {
            return sql.getRepairDetailsInfoBySno(Rno);
        }

        [WebMethod(Description="获取报修照片")]
        public string[] getRepairBasicInfoBmpBySno(string Sno)
        {
            return sql.getRepairBasicInfoBmpBySno(Sno);
        }

        [WebMethod(Description = "获取离校登记详细信息")]
        public string[] getSLSDetailsInfo(string Rno)
        {
            return sql.getSLSDetailsInfo(Rno);
        }

        [WebMethod(Description = "获取夜归记录详细信息")]
        public string[] getRLDetailsInfo(string Rno)
        {
            return sql.getRLDetailsInfo(Rno);
        }

        [WebMethod(Description = "更新报修记录")]
        public bool updateRepair(string repairNo, string repairPlace, string repairType, string detail, string contact)
        {
            return sql.updateRepair(repairNo,repairPlace, repairType, detail, contact);
        }

        [WebMethod(Description="删除报修记录")]
        public bool deleteRepair(string deleteNo) 
        {
            return sql.deleteRepair(deleteNo);
        }

        [WebMethod(Description = "更新夜归记录")]
        public bool updateReturnLately(string Rno, string returnTime, string reason)
        {
            return sql.updateReturnLately(Rno, returnTime, reason);
        }

        [WebMethod(Description = "删除夜归记录")]
        public bool deleteReturnLately(string deleteNo)
        {
            return sql.deleteReturnLately(deleteNo);
        }

        [WebMethod(Description = "更新离校登记记录")]
        public bool updateSLS(string SLSNo, string leaveDate, string backDate, string reason)
        {
            return sql.updateSLS(SLSNo, leaveDate, backDate, reason);
        }

        [WebMethod(Description = "删除离校登记记录")]
        public bool deleteSLS(string deleteNo)
        {
            return sql.deleteSLS(deleteNo);
        }

        [WebMethod(Description = "宿舍管理员获取报修摘要信息")]
        public string[] getRepBasicInfoByManager(string Eno)
        {
            return sql.getRepBasicInfoByManager(Eno);
        }

        [WebMethod(Description = "宿舍管理员获取离校登记摘要信息")]
        public string[] getSLSBasicInfoByManager(string Eno)
        {
            return sql.getSLSBasicInfoByManager(Eno);
        }

        [WebMethod(Description = "宿舍管理员获取夜归记录摘要信息")]
        public string[] getRLBasicInfoByManager(string Eno) 
        {
            return sql.getRLBasicInfoByManager(Eno);
        }


        /*SqlInfoInitializor initializor = new SqlInfoInitializor();
                
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
