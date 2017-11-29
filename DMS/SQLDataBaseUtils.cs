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
using System.Drawing;
using System.Text;


namespace DMS
{
    public class SQLDataBaseUtils
    {
        public static SqlConnection sqlCon;  //用于连接数据库  

        //连接字符串  
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
        //学号
        //姓名
        //院系
        //专业
        //班级
        public List<string> getBasicInfo(string Sno)
        {
            List<string> list = new List<string>();
            try
            {
                string sql = "select * from view_student where Sno = " + Sno;
                SqlCommand cmd = new SqlCommand(sql, sqlCon);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    //将结果集信息添加到返回向量中  
                    list.Add(reader[0].ToString());//学号
                    list.Add(reader[1].ToString());//姓名
                    list.Add(reader[2].ToString());//院系
                    list.Add(reader[3].ToString());//专业
                    list.Add(reader[4].ToString());//班级
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

        //插入报修信息
        //Sno 报修人学号
        //repairNo 报修编号
        //repairArea 报修区域
        //repairPlace 报修地点
        //repairType 报修类型
        //detail 描述
        //contact 联系方式
        // photos 传入照片的baos字符串

        public bool inserttoRepair(string Sno, string repairNo, string repairArea, string repairPlace, string repairType, string detail, string contact, string photo,string reportDate)
        {
            string path = "C:\\dms_v5\\image\\repairPhoto";
            string dir = path + "\\" + repairNo;
            try
            {
                //保存图片
                isDirExists(path,dir);
                savePic(dir, photo);
                //插入数据库
                string sql = "Exec p_insert_into_repair '" + Sno + "','" + repairNo + "'," + repairArea + ",'" + repairPlace + "'," + repairType + ",'" + detail + "','" + contact + "','" + dir + "','" + reportDate + "'";
                SqlCommand cmd = new SqlCommand(sql, sqlCon);
                cmd.ExecuteNonQuery();
                cmd.Dispose();
                return true;
            }
            catch (SqlException)
            {
                //数据库操作出错，回滚
                string rollback = "Exec p_insert_into_repair '" + Sno + "','" + repairNo + "'," + repairArea + ",'" + repairPlace + "'," + repairType + ",'" + detail + "','" + contact + "','" + "Null" + "','" + reportDate + "'";
                SqlCommand cmd = new SqlCommand(rollback, sqlCon);
                cmd.ExecuteNonQuery();
                cmd.Dispose();
                return false;
            }
            catch (IOException)
            {
                //保存文件失败，回滚
                if (File.Exists(dir))
                    File.Delete(dir);
                return false;
            }
        }

        //获取报修基本信息
        public string[] getRepairBasicInfoBySno(string Sno)
        {
            List<string> list = new List<string>();
            try
            {
                string sql = "Select PhotoDir,RepairDetail,ReportDate From view_getRepairBasicInfo Where Sno = '" + Sno + "'";
                SqlCommand cmd = new SqlCommand(sql, sqlCon);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    if (File.Exists(reader[0].ToString()))
                    {
                        string dir = getRepairPhoto(reader[0].ToString());
                        list.Add(dir);
                    }
                    else
                        list.Add("");
                    list.Add(reader[1].ToString());
                    list.Add(reader[2].ToString());
                }
                reader.Close();
                cmd.Dispose();
                return list.ToArray();
            }
            catch (SqlException)
            {
                return null;
            }
        }

        public string getRepairPhoto(string dir)
        {
            try
            {
                StreamReader sr = new StreamReader(dir, Encoding.Default);
                String line;
                String photo = "";
                while ((line = sr.ReadLine()) != null)
                {
                    photo = photo + line;
                }
                sr.Close();
                return photo;
            }
            catch (IOException)
            {
                return null;
            }
        }

        //插入夜归记录
        //学号
        //夜归记录编号
        //夜归时间
        //原因
        public bool insertintoReturnLately(string Sno, string Rno, string datetime, string reason)
        {
            try
            {
                string sql = "Exec p_insert_into_ReturnLately '" + Rno + "','" + Sno + "','" + datetime + "','" + reason + "'";
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

        //  更新头像
        // Sno 学号
        // photo string型图片
        public bool updateAvatar(string Sno,string photo)
        {
            try
            {
                string oldPhotoDir = getAvatarDir(Sno);
                string dir = "C:\\dms_v5\\image\\avatar" + "\\" + Sno;
                string sql = "Exec p_updateAvatar '" + Sno + "','" + dir + "'";
                //删除旧头像
                deleteOldAvatar(oldPhotoDir);
                //保存新头像
                savePic(dir, photo);
                //更新数据库
                SqlCommand cmd = new SqlCommand(sql, sqlCon);
                cmd.ExecuteNonQuery();
                cmd.Dispose();
                return true;
            }
            catch (SqlException)
            {
                //保存失败，回滚处理
                string rollbakck = "Exec p_updateAvatar '" + Sno + "',NULL";
                SqlCommand cmd = new SqlCommand(rollbakck, sqlCon);
                cmd.ExecuteNonQuery();
                cmd.Dispose();
                return false;
            }
            catch (IOException) 
            {
                return false;
            }
        }

        private void isDirExists(string path,string dir)
        {
            if (!Directory.Exists(path)) 
            {
                Directory.CreateDirectory(path);
            }
            if (!File.Exists(dir))
            {
                FileStream fs = File.Create(@"" + dir);
                fs.Close();
            }
        }

        private void savePic(string dir,string base64_str) 
        {
            isDirExists("C:\\dms_v5\\image\\avatar",dir);
            FileStream f = new FileStream(dir, FileMode.Open, FileAccess.Write, FileShare.ReadWrite);
            StreamWriter sw = new StreamWriter(f);
            sw.WriteLine(base64_str);
            sw.Flush();
            sw.Close();
            f.Close();
        }

        private void deleteOldAvatar(string dir)
        {
            if (dir == null || dir.Length == 0)
                return;
            if (File.Exists(dir))
            {
                File.Delete(dir);
            }
        }

        // 获取头像路径
        // Sno 学号
        private string getAvatarDir(string Sno)
        {
            try
            {
                string sql = "Exec p_getAvatarDirBySno '"+Sno+"'";
                SqlCommand cmd = new SqlCommand(sql, sqlCon);
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    string result = reader[0].ToString();
                    Console.WriteLine(result);
                    reader.Close();
                    cmd.Dispose();
                    return result;
                }
                else
                    return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        // 获取string型头像数据
        // dir 图片路径
        public string getAvatar(string Sno)
        {
            string dir = getAvatarDir(Sno);
            if (dir == null || dir.Length == 0)
                return null;
            try
            {
                StreamReader sr = new StreamReader(dir, Encoding.Default);
                String line;
                String photo = "";
                while ((line = sr.ReadLine()) != null) 
                {
                    photo = photo + line;
                }
                sr.Close();
                return photo;
            }
            catch (IOException)
            {
                return null;
            }
        }

        //给定学号，获取离校登记基本信息
        //Sno
        public List<string> getSLSBasicInfo(string Sno)
        {
            try
            {
                List<string> list = new List<string>();
                string sql = "Select * From view_getSLSBasicInfo Where Sno = '" + Sno + "'";
                SqlCommand cmd = new SqlCommand(sql, sqlCon);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    list.Add(reader[0].ToString());//Sno
                    list.Add(reader[1].ToString());//LeaveDate
                    list.Add(reader[2].ToString());//BackDate
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

        //给定学号，获取夜归记录的基本信息
        //Sno 学号
        public List<string> getRLBasicInfo(string Sno)
        {
            try
            {
                List<string> list = new List<string>();
                string sql = "Select * From view_getRLBasicInfoBySno Where Sno = " + Sno;
                SqlCommand cmd = new SqlCommand(sql, sqlCon);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    list.Add(reader[0].ToString());//Sno
                    list.Add(reader[1].ToString());//ReturnTime
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

        //注册账号
        // Sno 学号
        // account 账号
        // pwd 密码
        public bool Reg(string Sno, string account, string pwd)
        {
            try
            {
                string sql = "Exec p_insert_into_reg '" + Sno + "','" + account + "','" + pwd + "'";
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

        //验证账号密码是否正确，正确返回对应学号，错误返回-1
        // account 账号
        // pwd 密码
        public string getSnobyAccount(string account,string pwd)
        {
            try
            {
                string sql = "Exec p_getAccount_bySno '" + account + "','" + pwd + "'";
                SqlCommand cmd = new SqlCommand(sql, sqlCon);
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    string result = reader[0].ToString();
                    reader.Close();
                    cmd.Dispose();
                    return result;
                }
                else
                    return "-1";
            }
            catch (Exception)
            {
                return null;
            }
        }

        //修改密码
        //account 需要修改的账号
        //pwd 新密码
        public bool ModifyPwd(string account, string pwd)
        {
            try 
            {
                string sql = "Exec p_modifyPwd '" + account + "','" + pwd + "'";
                SqlCommand cmd = new SqlCommand(sql, sqlCon);
                cmd.ExecuteNonQuery();
                cmd.Dispose();
                return true;
            }
            catch(Exception)
            {
                return false;
            }
        }

        //离校登记
        // Sno 学号
        // leaveDate 离校时间
        // backDate 返校时间
        // reason 原因
        public bool insertintoSLS(string Sno, string leaveDate, string backDate, string reason)
        {
            try
            {
                string sql = "Exec p_insert_into_StudentLeavingSchool '" + Sno + "','" + leaveDate + "','" + backDate + "','" + reason + "'";
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

        // *获取性别*
        // i 性别代码
        private string getSex(int i)
        {
            if (i == 0)
                return "男";
            return "女";
        }

        // *获取学院*
        // i 学院代码
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

        // *获取专业*
        // i 学院代码
        // j 专业代码
        private string getDept(int i, int j)
        {
            if (i == 0)
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
            else if (i == 1)
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
            else if (i == 2)
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

        // *插入所有学号的信息*
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
                    sql = "Exec p_insert_into_Student '15251102" + i + "','" + s + "','" + getSex(rd.Next(0, 1)) + "','" + rd.Next(15, 25) + "','" + getCollege(j) + "','" + getDept(j, rd.Next(0, 3)) + "','" + rd.Next(1, 4) + "'";
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

        // *插入所有宿舍管理员的信息*
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
                    sql = "Exec p_insert_into_Administrator '20" + rd.Next(10, 17) + rd.Next(10, 12) + rd.Next(10, 30) + rd.Next(100, 999) + "','" + s + "','" + getSex(rd.Next(0, 1)) + "','" + rd.Next(20, 60) + "','宿舍管理员'";
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

        // *插入所有财产的信息*
        public bool inserttoAssets()
        {
            try
            {
                string[] no = { "200953601", "200945421", "201605253", "200927172", "200415172" };
                string[] name = { "实木双人电脑桌", "双层铁架床", "挂壁式空调", "太阳能热水器", "正门门锁" };
                int[] price = { 600, 400, 2300, 460, 90 };
                int[] qua = { 5920 * 2, 5920 * 2, 5920, 5920, 5920 };
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

        // *插入所有宿舍的信息*
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