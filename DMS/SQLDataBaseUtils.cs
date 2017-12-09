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
using System.Threading;


namespace DMS
{
    public class SQLDataBaseUtils
    {

        public static SqlConnection sqlCon;  //用于连接数据库 

        private static String rootDir = "C:\\dms_v6\\image";

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
                string sql = "Exec p_getStudentBasicInfo '" + Sno + "'";
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
            string path = rootDir + "\\repairPhoto";
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

        public bool updateRepair(string repairNo,string repairPlace, string repairType, string detail, string contact) 
        {
            string p = "";
            string t = "";
            string d = "";
            string c = "";
            if (repairPlace.Equals("RepairPlace"))
                p = repairPlace;
            else
                p = "'" + repairPlace + "'";
            if (repairType.Equals("RepairType"))
                t = repairType;
            else
                t = "'" + repairType + "'";
            if (detail.Equals("RepairDetail"))
                d = detail;
            else
                d = "'" + detail + "'";
            if (contact.Equals("Contact"))
                c = contact;
            else
                c = "'" + contact + "'";
            string sql = "Update Repair Set RepairPlace = " + p + ",RepairType = " + t + ",RepairDetail = " + d + ",Contact = " + c + " Where repairNo = '" + repairNo + "'";
            try
            {
                SqlCommand cmd = new SqlCommand(sql, sqlCon);
                cmd.ExecuteNonQuery();
                cmd.Dispose();
                return true;
            }
            catch (SqlException)
            {
                return false;
            }
        }

        public bool deleteRepair(string repairNo) 
        {
            try 
            {
                string sql = "Delete From Repair Where repairNo = '" + repairNo + "'";
                SqlCommand cmd = new SqlCommand(sql, sqlCon);
                cmd.ExecuteNonQuery();
                cmd.Dispose();
                return true;
            }
            catch(SqlException)
            {
                return false; 
            }
        }

        //获取报修基本信息
        public string[] getRepairBasicInfoBySno(string Sno)
        {
            List<string> list = new List<string>();
            try
            {
                string sql = "Exec p_getRepairBasicInfo '" + Sno + "'";
                SqlCommand cmd = new SqlCommand(sql, sqlCon);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(reader[0].ToString());
                    list.Add(reader[1].ToString());
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

        public string[] getRepairBasicInfoBmpBySno(string Sno)
        {
            List<string> list = new List<string>();
            try
            {
                string sql = "Exec p_getRepairBasicInfoBmp '" + Sno + "'";
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

        public string[] getRepairDetailsInfoBySno(string Rno)
        {
            List<string> list = new List<string>();
            try
            {
                string sql = "Exec p_getRepairDetailsInfo '" + Rno + "'";
                SqlCommand cmd = new SqlCommand(sql, sqlCon);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(reader[0].ToString());
                    list.Add(reader[1].ToString());
                    list.Add(reader[2].ToString());
                    list.Add(reader[3].ToString());
                    list.Add(reader[4].ToString());
                    list.Add(reader[5].ToString());
                    list.Add(reader[6].ToString());
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
                if (photo.Equals("null"))
                    return "123";
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

        public bool updateReturnLately(string Rno, string datetime, string reason)
        {
            string d = "";
            string r = "";
            if (datetime.Equals("returnTime"))
                d = "";
            else
                d = "returnTime = " + "'" + datetime + "',";
            if (reason.Equals("reason"))
                r = reason;
            else
                r = "'" + reason + "'";
            string sql = "Update ReturnLately Set " + d + "reason = reason Where Rno = '" + Rno + "'";
            try
            {
                SqlCommand cmd = new SqlCommand(sql, sqlCon);
                cmd.ExecuteNonQuery();
                cmd.Dispose();
                return true; 
            }
            catch (SqlException)
            {
                return false;
            }
        }

        public bool deleteReturnLately(string Rno)
        {
            try
            {
                string sql = "Delete From ReturnLately Where Rno = '" + Rno + "'";
                SqlCommand cmd = new SqlCommand(sql, sqlCon);
                cmd.ExecuteNonQuery();
                cmd.Dispose();
                return true; 
            }
            catch (SqlException)
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
                string dir = rootDir + "\\avatar" + "\\" + Sno;
                string sql = "Exec p_updateAvatar '" + Sno + "','" + dir + "'";
                //删除旧头像
                deleteOldPic(oldPhotoDir);
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

        public void savePic(string dir,string base64_str) 
        {
            isDirExists(rootDir + "\\avatar", dir);
            FileStream f = new FileStream(dir, FileMode.Open, FileAccess.Write, FileShare.ReadWrite);
            StreamWriter sw = new StreamWriter(f);
            sw.WriteLine(base64_str);
            sw.Flush();
            sw.Close();
            f.Close();
        }

        private void deleteOldPic(string dir)
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
                string sql = "Exec p_getSLSBasicInfo '" + Sno + "'";
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

        public string[] getSLSDetailsInfo(string Rno)
        {
            List<string> list = new List<string>();
            try
            {
                string sql = "Exec p_getSLSDetailsInfo '" + Rno + "'";
                SqlCommand cmd = new SqlCommand(sql, sqlCon);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(reader[0].ToString());
                    list.Add(reader[1].ToString());
                    list.Add(reader[2].ToString());
                    list.Add(reader[3].ToString());
                    list.Add(reader[4].ToString());
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

        //给定学号，获取夜归记录的基本信息
        //Sno 学号
        public List<string> getRLBasicInfo(string Sno)
        {
            try
            {
                List<string> list = new List<string>();
                string sql = "Exec p_getRLBasicInfoBySno '" + Sno + "'";
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

        public string[] getRLDetailsInfo(string Rno)
        {
            List<string> list = new List<string>();
            try
            {
                string sql = "Exec p_getRLDetailsInfoBySno '" + Rno + "'";
                SqlCommand cmd = new SqlCommand(sql, sqlCon);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(reader[0].ToString());
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
        public bool insertintoSLS(string Sno,string SLSNo, string leaveDate, string backDate, string reason)
        {
            try
            {
                string sql = "Exec p_insert_into_StudentLeavingSchool '" + Sno + "','" + SLSNo + "','" + leaveDate + "','" + backDate + "','" + reason + "'";
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

        public bool updateSLS(string SLSNo, string leaveDate, string backDate, string reason)
        {
            string l = "";
            string b = "";
            string r = "";
            if (leaveDate.Equals("LeaveDate"))
                l = "";
            else
                l = "LeaveDate = "+"'" + leaveDate + "',";
            if (backDate.Equals("BackDate"))
                b = "";
            else
                b = "BackDate = "+"'" + backDate + "',";
            if (reason.Equals("reason"))
                r = reason;
            else
                r = "'" + reason + "'";
            string sql = "Update StudentLeavingSchool Set " + l + b + "reason = reason Where SLSNo = '" + SLSNo + "'";
            try
            {
                SqlCommand cmd = new SqlCommand(sql, sqlCon);
                cmd.ExecuteNonQuery();
                cmd.Dispose();
                return true;
            }
            catch (SqlException)
            {
                return false;
            }
        }

        public bool deleteSLS(string SLSNo)
        {
            try
            {
                string sql = "Delete From StudentLeavingScholl Where SLSNo = '" + SLSNo + "'";
                SqlCommand cmd = new SqlCommand(sql, sqlCon);
                cmd.ExecuteNonQuery();
                cmd.Dispose();
                return true;
            }
            catch (SqlException)
            {
                return false;
            }
        }

    }
}