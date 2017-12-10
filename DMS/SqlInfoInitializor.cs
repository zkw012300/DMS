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
    public class SqlInfoInitializor
    {
        public static SqlConnection sqlCon;  //用于连接数据库 
        private static int STUDENT_COUNTER = 500;
        private static int ADMIN_COUNTER = 10;
        private static int RACE = 56;
        private static int ASSETS_COUNTER = 5;
        private static string[] name = new string[STUDENT_COUNTER];
        private static string[] admin = new string[ADMIN_COUNTER];
        private static string[] race = new string[RACE];
        private static string[] assets = new string[2 * ASSETS_COUNTER];
        private static string[] politicsStatuss = new string[4] { "群众", "党员", "共青团员", "预备党员" };
        private static string[] phoneNumberHead = new string[26]
        {
            "134","135","136","137","138",
            "139","150","151","157","158",
            "159","182","187","188","130",
            "131","132","152","155","156",
            "185","186","133","153","180",
            "189",
        };
        private static string[] no = { "200953601", "200945421", "201605253", "200927172", "200415172" };
        private static string[] assetsName = { "实木双人电脑桌", "双层铁架床", "挂壁式空调", "太阳能热水器", "正门门锁" };
        private static int[] price = { 600, 400, 2300, 460, 90 };
        private static int[] qua = { 5920 * 2, 5920 * 2, 5920, 5920, 5920 };

        //连接字符串  
        private String ConServerStr = @"Data Source=39.108.113.13;Initial Catalog=DMS;Persist Security Info=True;User ID=sa;Password=Zkw012300";

        //默认构造函数  
        public SqlInfoInitializor()
        {
            if (sqlCon == null)
            {
                sqlCon = new SqlConnection();
                sqlCon.ConnectionString = ConServerStr;
                sqlCon.Open();
                init();
            }
        }

        private void init()
        {
            FileStream nameStream = new FileStream("h:\\name.txt", FileMode.Open, FileAccess.Read);
            FileStream adminStream = new FileStream("h:\\admin.txt", FileMode.Open, FileAccess.Read);
            FileStream raceStream = new FileStream("h:\\race.txt", FileMode.Open, FileAccess.Read);
            FileStream assetsStream = new FileStream("h:\\assets.txt", FileMode.Open, FileAccess.Read);
            StreamReader nameReader = new StreamReader(nameStream, System.Text.Encoding.Default);
            StreamReader adminReader = new StreamReader(adminStream, System.Text.Encoding.Default);
            StreamReader raceReader = new StreamReader(raceStream, System.Text.Encoding.Default);
            StreamReader assetsReader = new StreamReader(assetsStream, System.Text.Encoding.Default);
            string temp;
            for (int i = 0; (temp = nameReader.ReadLine()) != null;i++ )
                name[i] = temp;
            for (int i = 0; (temp = raceReader.ReadLine()) != null; i++)
                race[i] = temp;
            for (int i = 0; (temp = adminReader.ReadLine()) != null; i++)
                admin[i] = temp;
            for (int i = 0; (temp = assetsReader.ReadLine()) != null; i++)
                assets[i] = temp;
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

        // *插入所有学生的信息*
        // *OK*
        public bool inserttoStudent()
        {
            try
            {
                Random rd = new Random();
                int j = 0;
                int k = 0;
                string sql;
                string sex;
                int age;
                string college;
                string dept;
                int classes;
                int yearOfBirth;
                int mouthOfBirth;
                int dayOfBirth;
                int raceNum;
                string politicsStatus;
                string phoneHead;
                int phone1;
                int phone2;
                int goSchoolYear;
                for (int i = 0; i < name.Length; i++)
                {
                    sex = getSex(rd.Next(0, 2));
                    age = rd.Next(15, 26);
                    j = rd.Next(0, 4);
                    k = rd.Next(0, 4);
                    college = getCollege(j);
                    dept = getDept(j, k);
                    classes = rd.Next(1, 4);
                    yearOfBirth = rd.Next(1992, 2000);
                    mouthOfBirth = rd.Next(1, 13);
                    dayOfBirth = rd.Next(1, 31);
                    raceNum = rd.Next(0, 56);
                    politicsStatus = politicsStatuss[rd.Next(0, politicsStatuss.Length)];
                    phoneHead = phoneNumberHead[rd.Next(0, phoneNumberHead.Length)];
                    phone1 = rd.Next(1000, 9999);
                    phone2 = rd.Next(1000, 9999);
                    goSchoolYear = rd.Next(2013, 2017);
                    sql = "Exec p_insert_into_Student '15251123" + (i + 100) + "','" + name[i] + "','" + sex + "'," + age + ",'"
                        + college + "','" + dept + "'," + classes + ",'" + yearOfBirth + "-" + mouthOfBirth + "-" + dayOfBirth + "','" + race[raceNum]
                        + "','广东广州','" + politicsStatus + "','广东省广州市海珠区广东财经大学','" + phoneHead + phone1 + phone2
                        + "','" + goSchoolYear + "-09-01','" + (goSchoolYear + 4) + "-07-01'";
                    SqlCommand cmd = new SqlCommand(sql, sqlCon);
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();
                }
                return true;
            }
            catch (SqlException)
            {
                return false;
            }
        }

        // *插入所有宿舍管理员的信息*
        public bool inserttoAdmin()
        {
            try
            {
                Random rd = new Random();
                string sql;
                string sex;
                int age;
                int yearOfBirth;
                int mouthOfBirth;
                int dayOfBirth;
                int raceNum;
                string politicsStatus;
                string phoneHead;
                int phone1;
                int phone2;
                int goSchoolYear;
                int goSchoolMonth;
                int goSchoolDay;
                for (int i = 0; i < admin.Length; i++)
                {
                    sex = getSex(rd.Next(0, 2));
                    age = rd.Next(25, 55);
                    yearOfBirth = rd.Next(1962, 1992);
                    mouthOfBirth = rd.Next(1, 13);
                    dayOfBirth = rd.Next(1, 31);
                    raceNum = rd.Next(0, 56);
                    politicsStatus = politicsStatuss[rd.Next(0, 2)];
                    phoneHead = phoneNumberHead[rd.Next(0, phoneNumberHead.Length)];
                    phone1 = rd.Next(1000, 9999);
                    phone2 = rd.Next(1000, 9999);
                    goSchoolYear = rd.Next(2013, 2017);
                    goSchoolMonth = rd.Next(1, 13);
                    goSchoolDay = rd.Next(1, 31);
                    sql = "Exec p_insert_into_Administrator '52811234" + (i + 100) + "','" + admin[i] + "','" + sex + "'," + age
                        + ",'生活部','部员','宿舍管理员','" + yearOfBirth + "-" + mouthOfBirth + "-" + dayOfBirth + "','" + race[raceNum]
                        + "','广东广州','" + politicsStatus + "','广东省广州市海珠区广东财经大学','" + phoneHead + phone1 + phone2
                        + "','" + goSchoolYear + "-" + goSchoolMonth + "-" + goSchoolDay + "'";
                    SqlCommand cmd = new SqlCommand(sql, sqlCon);
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();
                    Thread.Sleep(100);
                }
                return true;
            }
            catch (SqlException)
            {
                return false;
            }
        }

        // *插入所有财产的信息*
        public bool inserttoAssets()
        {
            try
            {
                for (int i = 0; i < no.Length; i++)
                {
                    string sql = "Exec p_insert_into_assets '" + no[i] + "','" + assetsName[i] + "','" + price[i] + "','宿舍区','" + qua[i] + "'";
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
        public bool inserttoDormitoryAndDA()
        {
            try
            {
                for (int i = 0; i < 10; i++)
                {
                    string buildNo = (i + 1) + "";
                    if (i + 1 < 10)
                        buildNo = "0" + buildNo;
                    for (int j = 0; j < 5; j++)
                    {
                        string floorNo = (j + 1) + "";
                        if (j + 1 < 10)
                            floorNo = "0" + floorNo;
                        for (int k = 0; k < 10; k++)
                        {
                            string roomNo = (k + 1) + "";
                            if (k + 1 < 10)
                                roomNo = "0" + roomNo;
                            string sql = "Exec p_insert_into_dormitory '" + buildNo + floorNo + roomNo + "','" + buildNo + "','" + floorNo + "','" + roomNo + "'";
                            SqlCommand cmd = new SqlCommand(sql, sqlCon);
                            cmd.ExecuteNonQuery();
                            cmd.Dispose();
                            for (int l = 0; l < no.Length; l++)
                            {
                                string sqlDA = "Exec p_insert_into_DA '" + buildNo + floorNo + roomNo + "','" + no[l] + "'";
                                SqlCommand cmdForDA = new SqlCommand(sqlDA, sqlCon);
                                cmdForDA.ExecuteNonQuery();
                                cmdForDA.Dispose();
                            }
                        }
                    }
                }
                return true;
            }
            catch (SqlException)
            {
                return false;
            }
        }

        public bool insertintoManagement()
        {
            try
            {
                string sql;
                string buildNo;
                string floorNo;
                string roomNo;
                for (int i = 0; i < 10; i++)
                {
                    buildNo = "0" + (i + 1);
                    if (i + 1 == 10) 
                        buildNo = "10";
                    for (int j = 0; j < 5; j++)
                    {
                        floorNo = "0" + (j + 1);
                        for (int k = 0; k < 10; k++)
                        {
                            roomNo = "0" + (k + 1);
                            if (k + 1 == 10)
                                roomNo = "10";
                            sql = "Exec p_insert_into_management '5281123410"+i+"','"+buildNo+floorNo+roomNo+"'";
                            SqlCommand cmd = new SqlCommand(sql, sqlCon);
                            cmd.ExecuteNonQuery();
                            cmd.Dispose();
                        }
                    }
                }
                return true;
            }
            catch (SqlException)
            {
                return false;
            }
        }

        public bool insertintoSD()
        {
            try
            {
                int counter = 0;
                string str0;
                string str1;
                string str2;
                string sql;
                for (int i = 0; i < 10; i++)
                {
                    str0 = (i + 1) + "";
                    if (i + 1 < 10)
                        str0 = "0" + str0;
                    for (int j = 0; j < 5; j++)
                    {
                        str1 = (j + 1) + "";
                        if (j + 1 < 10)
                            str1 = "0" + str1;
                        for (int k = 0; k < 10; k++)
                        {
                            str2 = (k + 1) + "";
                            if (k + 1 < 10)
                                str2 = "0" + str2;
                            for (int x = 1; x < 5; x++)
                            {
                                if (counter >= 500)
                                    break;
                                sql = "Exec p_insert_into_SD '15251123" + (counter + 100) + "','" + str0 + str1 + str2 + "'," + x + "";
                                SqlCommand cmd = new SqlCommand(sql, sqlCon);
                                cmd.ExecuteNonQuery();
                                cmd.Dispose();
                                counter++;
                            }
                        }
                    }
                }
                return true;
            }
            catch (SqlException)
            {
                return false;
            }
        }

    }
}