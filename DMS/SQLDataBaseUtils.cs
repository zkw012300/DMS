using System;
using System.IO;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;


namespace DMS
{
    public class SQLDataBaseUtils
    {
        public static SqlConnection sqlCon;  //用于连接数据库  

        //将下面的引号之间的内容换成上面记录下的属性中的连接字符串  
        private String ConServerStr = @"Data Source=39.108.113.13;Initial Catalog=DMS;Persist Security Info=True;User ID=sa;Password=Zkw012300";

        //默认构造函数  
        public SQLDataBaseUtils()
        {
            if (sqlCon == null)
            {
                sqlCon = new SqlConnection();
                sqlCon.ConnectionString = ConServerStr;
                sqlCon.Open();
            }
        }

        //关闭/销毁函数，相当于Close()  
        public void Dispose()
        {
            if (sqlCon != null)
            {
                sqlCon.Close();
                sqlCon = null;
            }
        }

        //获取学生基本信息
        //姓名
        //学号
        //专业
        //班级
        public List<string> getBasicInfo()
        {
            List<string> list = new List<string>();

            try
            {
                string sql = "select * from view_student";
                SqlCommand cmd = new SqlCommand(sql, sqlCon);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    //将结果集信息添加到返回向量中  
                    list.Add(reader[0].ToString());
                    list.Add(reader[1].ToString());
                    list.Add(reader[2].ToString());
                    list.Add(reader[3].ToString());
                    list.Add(reader[4].ToString());
                    list.Add(reader[5].ToString());
                }

                reader.Close();
                cmd.Dispose();

            }
            catch (Exception)
            {

            }
            return list;
        }

        //插入报修信息
        //Sno 报修人学号

        public bool inserttoRepair(string Sno,string repairNo,string repairArea,string repairPlace,string repairType,string detail,string contact,string photos) 
        {
            string uuid = Guid.NewGuid().ToString();
            string dir = "C:\\dms\\image";
            string photodir = dir + "\\" + uuid + ".jpg";
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            System.Drawing.Image img = PhotoUtils.FromBase64String(photos);
            img.Save(photodir);
            try
            {
                string sql = "Exec p_insert_into_repair '"+Sno+"','"+repairNo+"',"+repairArea+",'"+repairPlace+"',"+repairType+",'"+detail+"','"+contact+"','"+photodir+"'";
                SqlCommand cmd = new SqlCommand(sql, sqlCon);
                cmd.ExecuteNonQuery();
                cmd.Dispose();
                return true;
            }
            catch (Exception) 
            {
                return false;
            }
        }

        public List<string> getRepairBasicInfoBySno(string Sno) 
        {
            try
            {
                List<string> list = new List<string>();
                string sql = "Select * From view_getRepairBasicInfo Where Sno = '" + Sno + "'";
                SqlCommand cmd = new SqlCommand(sql, sqlCon);
                SqlDataReader reader = cmd.ExecuteReader();

                while(reader.Read())
                {
                    list.Add(reader[0].ToString());
                    list.Add(reader[1].ToString());
                    list.Add(reader[2].ToString());
                    list.Add(reader[3].ToString());
                }
                reader.Close();
                cmd.Dispose();
                return list;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public List<string> getBasicInfoBySno(string Sno)
        {
            List<string> list = new List<string>();

            try
            {
                string sql = "select * from view_student Where Sno = "+Sno;
                SqlCommand cmd = new SqlCommand(sql, sqlCon);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    //将结果集信息添加到返回向量中  
                    list.Add(reader[0].ToString());
                    list.Add(reader[1].ToString());
                    list.Add(reader[2].ToString());
                    list.Add(reader[3].ToString());
                    list.Add(reader[4].ToString());
                    list.Add(reader[5].ToString());
                }

                reader.Close();
                cmd.Dispose();

            }
            catch (Exception)
            {

            }
            return list;
        }

        public bool Reg(string sno,string account,string pwd) 
        {
            try
            {
                string sql = "Exec p_insert_into_reg '"+sno+"','"+account+"','"+pwd+"'";
                SqlCommand cmd = new SqlCommand(sql, sqlCon);
                cmd.ExecuteNonQuery();
                cmd.Dispose();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool insertintoSLS(string sno, string leaveDate, string backDate, string reason)
        {
            try
            {
                string sql = "Exec p_insert_into_StudentLeavingSchool '" + sno + "','" + leaveDate + "','" + backDate + "','" + reason + "'";
                SqlCommand cmd = new SqlCommand(sql, sqlCon);
                cmd.ExecuteNonQuery();
                cmd.Dispose();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private string getSex(int i)
        {
            if (i == 0)
                return "男";
            return "女";
        }

        private string getCollege(int i)
        {
            switch (i)
            {
                case 0:
                    return "信息学院";
                case 1:
                    return "金融学院";
                case 2:
                    return "数学与统计学院";
                case 3:
                    return "外国语学院";
                default:
                    return null;
            }
        }

        private string getDept(int i, int j)
        {
            if(i==0)
                switch (j)
                {
                    case 0:
                        return "计算机科学与技术";
                    case 1:
                        return "软件工程";
                    case 2:
                        return "电子商务";
                    case 3:
                        return "信息管理系统";
                    default:
                        return null;
                }
            else if(i == 1)
                switch (j)
                {
                    case 0:
                        return "金融学";
                    case 1:
                        return "金融工程";
                    case 2:
                        return "会计学";
                    case 3:
                        return "投资学";
                    default:
                        return null;
                }
            else if(i == 2)
                switch (j)
                {
                    case 0:
                        return "应用统计学";
                    case 1:
                        return "数学与应用数学";
                    case 2:
                        return "信息与计算科学";
                    case 3:
                        return "统计学";
                    default:
                        return null;
                }
            else
                switch (j)
                {
                    case 0:
                        return "英语";
                    case 1:
                        return "日语";
                    case 2:
                        return "俄语";
                    case 3:
                        return "德语";
                    default:
                        return null;
                }
        }

        public bool inserttoStudent() 
        {
            try
            {
                FileStream fs = new FileStream("h:\\123.txt", FileMode.Open, FileAccess.Read);
                StreamReader sr = new StreamReader(fs, System.Text.Encoding.Default);
                string s;
                string sql;
                Random rd = new Random();
                int i = 100;
                while ((s = sr.ReadLine()) != null)
                {
                    i++;
                    int j = rd.Next(0, 3);
                    sql = "Exec p_insert_into_Student '15251102" + i + "','" + s + "','"+getSex(rd.Next(0,1))+"','"+rd.Next(15,25)+"','"+getCollege(j)+"','"+getDept(j,rd.Next(0,3))+"','"+rd.Next(1,4)+"'";
                    SqlCommand cmd = new SqlCommand(sql, sqlCon);
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();
                    for (int k = 0; k < 32767; k++) ;
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool inserttoAdmin()
        {
            try
            {
                FileStream fs = new FileStream("h:\\456.txt", FileMode.Open, FileAccess.Read);
                StreamReader sr = new StreamReader(fs, System.Text.Encoding.Default);
                string s;
                string sql;
                Random rd = new Random();
                while ((s = sr.ReadLine()) != null)
                {
                    int j = rd.Next(0, 3);
                    sql = "Exec p_insert_into_Administrator '20"+rd.Next(10,17)+rd.Next(10,12)+rd.Next(10,30)+rd.Next(100,999)+"','"+s+"','"+getSex(rd.Next(0,1))+"','"+rd.Next(20,60)+"','宿舍管理员'";
                    SqlCommand cmd = new SqlCommand(sql, sqlCon);
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();
                    //for (int k = 0; k < 32767; k++) ;
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool inserttoAssets() 
        {
            try
            {
                string[] no = { "200953601","200945421","201605253","200927172","200415172"};
                string[] name = { "实木双人电脑桌","双层铁架床","挂壁式空调","太阳能热水器","正门门锁" };
                int[] price = { 600,400,2300,460,90};
                int[] qua = { 5920*2,5920*2,5920,5920,5920};
                for (int i = 0; i < no.Length; i++)
                {
                    string sql = "Exec p_insert_into_assets '" + no[i] + "','" + name[i] + "','" + price[i] + "','宿舍区','" + qua[i] + "'";
                    SqlCommand cmd = new SqlCommand(sql, sqlCon);
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();
                }
                    return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool inserttoDormitory()
        {
            try
            {
                for (int i = 0; i < 37; i++)
                {
                    string buildNo = (i + 1) + "";
                    if (i + 1 < 10)
                        buildNo = "0" + buildNo;
                    for (int j = 0; j < 8; j++) 
                    {
                        string floorNo = (j + 1) + "";
                        if (j + 1 < 10)
                            floorNo = "0" + floorNo;
                        for (int k = 0; k < 20; k++)
                        {
                            string roomNo = (k + 1) + "";
                            if (k + 1 < 10)
                                roomNo = "0" + roomNo;
                            string sql = "Exec p_insert_into_dormitory '" + buildNo + floorNo + roomNo + "','" + buildNo + "','" + floorNo + "','" + roomNo + "'";
                            SqlCommand cmd = new SqlCommand(sql, sqlCon);
                            cmd.ExecuteNonQuery();
                            cmd.Dispose();
                        }
                    }
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}