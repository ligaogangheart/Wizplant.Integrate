using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;
using Excel=Microsoft.Office.Interop.Excel;
using PR.WizPlant.Integrate.Sql;




namespace PR.WizPlant.DataImport
{
    public partial class Store : Form
    {
        public Store()
        {
            InitializeComponent();
        }

        private string sheetName = "";//获取要打开的sheet名


        /// <summary>
        /// 摄像头设备打开文件按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSearch_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileName = new OpenFileDialog(); //创建打开文件控件
            fileName.InitialDirectory = Application.StartupPath; //设置打开控件后，默认目录为exe运行文件所在的文件夹
            fileName.Filter = "All files (*.*)|*.*|xlsx files (*.xlsx)|*.xlsx|xls files (*.xls)|*.xls";//设置控件打开的文件类型(格式：文件后缀名 files (*.文件后缀名)|*.文件后缀名)
            fileName.FilterIndex = 2;//设置控件打开文件类型的显示顺序
            fileName.RestoreDirectory = true;//设置对话框是否记忆之前打开的目录
            if (fileName.ShowDialog() == DialogResult.OK)
            {
                string Path = fileName.FileName.ToString();//获取用户选择的文件的完整路径
                string Name = Path.Substring(Path.LastIndexOf("\\") + 1);//获取用户选择的不带路径的文件名
                this.lblPath.Text = Path;
                this.label4.Text = Name;
                //sheetName = this.tabPage1.Text;
                ExcelHelper eh = new ExcelHelper();
                DataTable exDt = eh.ImportData(Path);     //通过DataTable储存Excel读取的数据    
                if (exDt == null)
                {
                    MessageBox.Show("打开文件错误！请查看日志！");
                    return;
                }
                exDt = DealEmptyRow(exDt);
                //绑定数据源
                if (exDt.Columns.Count == 11 && exDt.Columns[0].ToString() == "Id" && exDt.Columns[1].ToString() == "Station" && exDt.Columns[2].ToString() == "MonitorIp" && exDt.Columns[3].ToString() == "MonitorPort" && exDt.Columns[4].ToString() == "UserName" && exDt.Columns[5].ToString() == "Password" && exDt.Columns[6].ToString() == "Name" && exDt.Columns[7].ToString() == "Context" && exDt.Columns[8].ToString() == "Revision" && exDt.Columns[9].ToString() == "ChannelNo" && exDt.Columns[10].ToString() == "ObjectId")
                {
                    this.dataGridView1.DataSource = exDt;
                }
                else
                {
                    MessageBox.Show("警告：所选文件表头或列数值与规定不符，请重新选择正确文件！");
                    this.dataGridView1.DataSource = "";
                }              
            }
        }

        /// <summary>
        /// SCADA打开文件按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click_1(object sender, EventArgs e)
        {
            OpenFileDialog fileName = new OpenFileDialog(); 
            fileName.InitialDirectory = Application.StartupPath; 
            fileName.Filter = "All files (*.*)|*.*|xlsx files (*.xlsx)|*.xlsx|xls files (*.xls)|*.xls";
            fileName.FilterIndex = 2;
            fileName.RestoreDirectory = true;
            if (fileName.ShowDialog() == DialogResult.OK)
            {
                string Path = fileName.FileName.ToString();
                string Name = Path.Substring(Path.LastIndexOf("\\") + 1);
                this.label5.Text = Path;
                this.label1.Text = Name;
                //sheetName = this.tabPage2.Text;
                ExcelHelper eh = new ExcelHelper();
                DataTable exDt = eh.ImportData(Path);
                if (exDt == null)
                {
                    MessageBox.Show("打开文件错误！请查看日志！");
                    return;
                }
                exDt = DealEmptyRow(exDt);
                if (exDt.Columns.Count == 12 && exDt.Columns[0].ToString() == "Id" && exDt.Columns[1].ToString() == "SiteName" && exDt.Columns[2].ToString() == "TagNo" && exDt.Columns[3].ToString() == "TagName" && exDt.Columns[4].ToString() == "TagType" && exDt.Columns[5].ToString() == "TagDesc" && exDt.Columns[6].ToString() == "Name" && exDt.Columns[7].ToString() == "Name2" && exDt.Columns[8].ToString() == "Context" && exDt.Columns[9].ToString() == "Revision" && exDt.Columns[10].ToString() == "MessureType" && exDt.Columns[11].ToString() == "ObjectId")
                {
                    this.dataGridView2.DataSource = exDt;
                }
                else
                {
                    MessageBox.Show("警告：所选文件表头或列数值与规定不符，请重新选择正确文件！");
                    this.dataGridView2.DataSource = "";
                }              
            }
        }

        /// <summary>
        /// CSGII打开文件按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button5_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileName = new OpenFileDialog(); 
            fileName.InitialDirectory = Application.StartupPath; 
            fileName.Filter = "All files (*.*)|*.*|xlsx files (*.xlsx)|*.xlsx|xls files (*.xls)|*.xls";
            fileName.FilterIndex = 2;
            fileName.RestoreDirectory = true;
            if (fileName.ShowDialog() == DialogResult.OK)
            {
                string Path = fileName.FileName.ToString();
                string Name = Path.Substring(Path.LastIndexOf("\\") + 1);
                this.label13.Text = Path;
                this.label11.Text = Name;
                //sheetName = this.tabPage4.Text;
                ExcelHelper eh = new ExcelHelper();
                DataTable exDt = eh.ImportData(Path);
                if (exDt == null)
                {
                    MessageBox.Show("打开文件错误！请查看日志！");
                    return;
                }
                exDt = DealEmptyRow(exDt);
                if (exDt.Columns.Count == 7 && exDt.Columns[0].ToString() == "Id" && exDt.Columns[1].ToString() == "Name" && exDt.Columns[2].ToString() == "Context" && exDt.Columns[3].ToString() == "Revision" && exDt.Columns[4].ToString() == "DeviceId" && exDt.Columns[5].ToString() == "DeviceName" && exDt.Columns[6].ToString() == "ObjectId")
                {
                    this.dataGridView4.DataSource = exDt;
                }
                else
                {
                    MessageBox.Show("警告：所选文件表头或列数值与规定不符，请重新选择正确文件！");
                    this.dataGridView4.DataSource = "";
                }
                              
            }
        }

        /// <summary>
        /// 处理DataTable中的空白行
        /// </summary>
        /// <param name="dt">需要处理的DataTable</param>
        /// <returns></returns>
        public DataTable DealEmptyRow(DataTable dt)
        {
            List<DataRow> RowDelList = new List<DataRow>();
            foreach (DataRow row in dt.Rows)
            {
                bool isDelete = true;
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    if(!string.IsNullOrEmpty(row[i].ToString()))
                    {
                        isDelete = false;
                    }
                }
                if (isDelete)
                {
                    RowDelList.Add(row);
                }
            }
            foreach (var rowDel in RowDelList)
            {
                dt.Rows.Remove(rowDel);
            }
            return dt;
        }

        /// <summary>
        /// 导入Excel到DataGridView中
        /// </summary>
        /// <param name="FilePath">Excel路劲名</param>
        /// <returns></returns>
        public DataSet ReadExcel_ds(string FilePath)
        {
            string subfile = FilePath.Substring(FilePath.LastIndexOf(".") + 1);   //获取后缀名
            DataSet ds = new DataSet();
            string strCon = "";
            if (subfile.ToUpper() == "XLS")//excel2003获取
            {
                strCon = "Provider=Microsoft.Jet.OLEDB.4.0;Extended Properties=Excel 8.0;data source=" + FilePath;
            }
            if (subfile.ToUpper() == "XLSX")//excel2007读取            
            {
                strCon = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + FilePath + ";Extended Properties=\"Excel 12.0;HDR=YES\"";   //Excel连接字符串
            }
            System.Data.OleDb.OleDbConnection Conn = new System.Data.OleDb.OleDbConnection(strCon);//创建Excel链接对象
            try
            {
                Conn.Open();
                System.Data.DataTable sTable = Conn.GetOleDbSchemaTable(System.Data.OleDb.OleDbSchemaGuid.Tables, null);//新添加 2010-10-14                
                string tableName = sTable.Rows[0][2].ToString().Trim();//新添加 2010-10-14  获取工作表名称                
                string strCom = "SELECT * FROM [" + tableName + "] ";
                System.Data.OleDb.OleDbDataAdapter myCommand = new System.Data.OleDb.OleDbDataAdapter(strCom, Conn);//创建适配器
                myCommand.Fill(ds, "[" + tableName + "]");//填充
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message);
            }
            finally
            {
                Conn.Close();
            }
            return ds;
        }

        static string connectionString = System.Configuration.ConfigurationManager.AppSettings["strConn"];

        /// <summary>
        /// 判断非查询操作成功与否
        /// </summary>
        /// <returns></returns>
        public bool NoQuery(string sql, SqlParameter[] sqlp)
        {
            if (SqlHelper.ExecuteNonQuery(connectionString, sql, sqlp) > -1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 执行多条SQL语句，实现数据库事务。
        /// </summary>
        /// <param name="SQLStringList">多条SQL语句</param>		
        public static int ExecuteSqlTran(List<string> alist)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = conn;
                SqlTransaction tx = conn.BeginTransaction();
                cmd.Transaction = tx;
                try
                {
                    for (int n = 0; n < alist.Count; n++)
                    {
                        string strsql = alist[n].ToString();
                        if (strsql.Trim().Length > 1)
                        {
                            cmd.CommandText = strsql;
                            cmd.ExecuteNonQuery();
                        }
                    }
                    tx.Commit();
                    return 1;
                }
                catch (System.Data.SqlClient.SqlException E)
                {
                    tx.Rollback();
                    return 0;
                    throw new System.Exception(E.Message);
                }
            }
        }

        /// <summary>
        /// 导入监控信息数据到数据库
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>                
        private void btnImport_Click_1(object sender, EventArgs e)
        {
            List<string> listDel = new List<string>();
            int cout = 0;
            DataSet dsW = new DataSet();//监控表所有数据，用来判断是否重复
            if (this.dataGridView1.Rows.Count - 1 == 0)
            {
                MessageBox.Show("无需要导入数据！");
                return;
            }
            for (int i = 0; i < this.dataGridView1.Rows.Count - 1; i++) //获取
            {                
                //获取DataGridView中的数据
                VideoMonitor v = new VideoMonitor();
                if (!string.IsNullOrEmpty(this.dataGridView1.Rows[i].Cells["VId"].Value.ToString()))
                {
                    v.Id = this.dataGridView1.Rows[i].Cells["VId"].Value.ToString();
                }
                else
                {
                    v.Id = "";
                }
                v.Station = this.dataGridView1.Rows[i].Cells["VStation"].Value.ToString();
                v.MonitorIp = this.dataGridView1.Rows[i].Cells["VMonitorIp"].Value.ToString();
                v.MonitorPort = this.dataGridView1.Rows[i].Cells["VMonitorPort"].Value.ToString();
                v.UserName = this.dataGridView1.Rows[i].Cells["VUserName"].Value.ToString();
                v.PassWord = this.dataGridView1.Rows[i].Cells["VPassWord"].Value.ToString();
                v.Name = this.dataGridView1.Rows[i].Cells["VName"].Value.ToString();
                v.Context = this.dataGridView1.Rows[i].Cells["VContext"].Value.ToString();
                v.Revision = this.dataGridView1.Rows[i].Cells["VRevision"].Value.ToString();
                v.ChannelNo = this.dataGridView1.Rows[i].Cells["VChannelNo"].Value.ToString();
                v.ObjectId = getObjId(v.Name, v.Context, v.Revision);//查询获取对应ObjectId

                

                //查询并获取当前匹配对象ID
                //string sqlId = "select Id from [PRW_Inte_VideoMonitor_CameraMap] where Station = '" + v.Station + "' and Name = '" + v.Name + "'";
                //DataSet ds = SqlHelper.ExecuteDataSet(connectionString,sqlId,null);
                //string id = ds.Tables[0].Rows[0][0].ToString();

                string id = "";
                id = v.Id;

                //如果存在对象ID进行修改操作
                if (!string.IsNullOrEmpty(id))
                {
                    //定义修改语句
                    string sqlUpd = "update [PRW_Inte_VideoMonitor_CameraMap] set Id ='" + id + "',";
                    
                    //查询当前对象
                    string sqlSel = "select * from [PRW_Inte_VideoMonitor_CameraMap] where Id = '" + id + "'";                   
                    DataSet dsSel = SqlHelper.ExecuteDataSet(connectionString, sqlSel, null);
                    string station = dsSel.Tables[0].Rows[0]["Station"].ToString();
                    string monitorip = dsSel.Tables[0].Rows[0]["MonitorIp"].ToString();
                    string monitorport = dsSel.Tables[0].Rows[0]["MonitorPort"].ToString();
                    string username = dsSel.Tables[0].Rows[0]["UserName"].ToString();
                    string password = dsSel.Tables[0].Rows[0]["PassWord"].ToString();
                    string name = dsSel.Tables[0].Rows[0]["Name"].ToString();
                    string context = dsSel.Tables[0].Rows[0]["Context"].ToString();
                    string revision = dsSel.Tables[0].Rows[0]["Revision"].ToString();
                    string channelNo = dsSel.Tables[0].Rows[0]["ChannelNo"].ToString();

                    bool isAdd = false;
                    //判断当前DataGridView中的数据是否与数据库一致，不一致的进行修改
                    if (v.Station != station)
                    {
                        sqlUpd = sqlUpd + " Station = '" + v.Station + "',";
                        isAdd = true;
                    }
                    if (v.MonitorIp != monitorip)
                    {
                        sqlUpd = sqlUpd + " MonitorIp = '" + v.MonitorIp + "',";
                        isAdd = true;
                    }
                    if (v.MonitorPort != monitorport)
                    {
                        sqlUpd = sqlUpd + " MonitorPort = '" + v.MonitorPort + "',";
                        isAdd = true;
                    }
                    if (v.UserName != username)
                    {
                        sqlUpd = sqlUpd + " UserName = '" + v.UserName + "',";
                        isAdd = true;
                    }
                    if (v.PassWord != password)
                    {
                        sqlUpd = sqlUpd + " PassWord = '" + v.PassWord + "',";
                        isAdd = true;
                    }
                    if (v.Name!=name)
                    {
                        sqlUpd = sqlUpd + " Name = '" + v.Name + "',";
                        isAdd = true;
                    }
                    if (v.Context != context)
                    {
                        sqlUpd = sqlUpd + " Context = '" + v.Context + "',";
                        isAdd = true;
                    }
                    if (v.Revision != revision)
                    {
                        sqlUpd = sqlUpd + " Revision = '" + v.Revision + "',";
                        isAdd = true;
                    }
                    if (v.ChannelNo != channelNo)
                    {
                        sqlUpd = sqlUpd + " ChannelNo = '" + v.ChannelNo + "',";
                        isAdd = true;
                    }
                    if (v.ObjectId != "" && v.ObjectId != dsSel.Tables[0].Rows[0]["ObjectId"].ToString())
                    {
                        isAdd = true;
                    }
                    if (string.IsNullOrEmpty(dsSel.Tables[0].Rows[0]["ObjectId"].ToString()))
                    {
                        sqlUpd = sqlUpd + " ObjectId = null where Id = '" + id + "'";
                    }
                    else
                    {
                        sqlUpd = sqlUpd + " ObjectId = '" + dsSel.Tables[0].Rows[0]["ObjectId"].ToString() + "' where Id = '" + id + "'";
                    }
                    if (isAdd)
                    {
                        listDel.Add(sqlUpd);
                    }
                    //if (v.Station != station && v.MonitorIp != monitorip && v.MonitorPort != monitorport && v.UserName != username && v.PassWord != password && v.Name!=name && v.Context != context && v.Revision != revision && v.ChannelNo != channelNo)
                    //{
                        
                    //}
                    
                 }
                else //如果不存在对象ID进行添加
                {
                    if (dsW.Tables.Count == 0)
                    {
                        string sqlW = "select * from [PRW_Inte_VideoMonitor_CameraMap]";
                        dsW = SqlHelper.ExecuteDataSet(connectionString, sqlW, null);
                    }
                    for (int j = 0; j < dsW.Tables[0].Rows.Count - 1; j++)
                    {
                        string station = dsW.Tables[0].Rows[j]["Station"].ToString();
                        string monitorip = dsW.Tables[0].Rows[j]["MonitorIp"].ToString();
                        string monitorport = dsW.Tables[0].Rows[j]["MonitorPort"].ToString();
                        string username = dsW.Tables[0].Rows[j]["UserName"].ToString();
                        string password = dsW.Tables[0].Rows[j]["PassWord"].ToString();
                        string name = dsW.Tables[0].Rows[j]["Name"].ToString();
                        string context = dsW.Tables[0].Rows[j]["Context"].ToString();
                        string revision = dsW.Tables[0].Rows[j]["Revision"].ToString();
                        string channelNo = dsW.Tables[0].Rows[j]["ChannelNo"].ToString();

                        //防止用户误删Excel表格中的Id导致添加数据与数据库重复
                        if (v.Station == station && v.MonitorIp == monitorip && v.MonitorPort == monitorport && v.UserName == username && v.PassWord == password && v.Name == name && v.Context == context && v.Revision == revision && v.ChannelNo == channelNo)
                        {
                            cout = cout + 1;
                            break;
                        }
                    }
                    if (cout == 0)
                    {
                        string sqlIn = "";
                        if (!string.IsNullOrEmpty(v.ObjectId))
                        {
                            sqlIn = "insert into PRW_Inte_VideoMonitor_CameraMap values(NEWID(),'" + v.Station + "','" + v.MonitorIp + "','" + v.MonitorPort + "','" + v.UserName + "','" + v.PassWord + "','" + v.Name + "','" + v.Context + "','" + v.Revision + "','" + v.ChannelNo + "','" + v.ObjectId + "')";
                        }
                        else
                        {
                            sqlIn = "insert into PRW_Inte_VideoMonitor_CameraMap values(NEWID(),'" + v.Station + "','" + v.MonitorIp + "','" + v.MonitorPort + "','" + v.UserName + "','" + v.PassWord + "','" + v.Name + "','" + v.Context + "','" + v.Revision + "','" + v.ChannelNo + "',null)";
                        }
                        listDel.Add(sqlIn);
                    }

                }                
            }
            if (ExecuteSqlTran(listDel) == 1)
            {
                string messg = "导入数据成功！";
                if (cout != 0)
                {
                    messg = messg + "其中存在"+ (cout + 1) +"条与数据库重复数据！请导入前不要误删数据，避免错误操作";
                }
                MessageBox.Show(messg);
                messg = "导入数据成功！";
                cout = 0;
            }
            else
            {
                MessageBox.Show("导入数据失败！");
            }
        
        }

        /// <summary>
        /// 根据name context revision获取ObjectId
        /// </summary>
        /// <param name="name"></param>
        /// <param name="context"></param>
        /// <param name="revision"></param>
        /// <returns></returns>
        public string getObjId(string name, string context, string revision) 
        {
            string connStr = ConfigurationManager.AppSettings["strConnA"];
            string sql = "select Id from PRW_Object where Name='" + name + "' and Context='" + context + "' and Revision='" + revision + "' and Status = 0";
            DataTable dt = new DataTable();
            string objId = "";
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                try
                {
                    SqlCommand comm = new SqlCommand(sql, conn);                                                     
                    SqlDataAdapter dapt = new SqlDataAdapter(comm);
                    dapt.Fill(dt);
                }
                catch (Exception ex)
                {

                    throw ex;
                }
                finally 
                {
                    conn.Close();
                }
            }
            if (dt.Rows.Count <= 0)
            {
                objId = "";
            }
            else
            {
                objId = dt.Rows[0]["Id"].ToString();
            }
            
            return objId;
        }

        /// <summary>
        /// CSGII映射表数据导入数据库
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button6_Click(object sender, EventArgs e)
        {
            //获取DataGridView中的数据
            //DataTable dt = (DataTable)this.dataGridView4.DataSource;

            DataSet ds = new DataSet();
            List<string> listDel = new List<string>();
            int cout = 0;
            if (this.dataGridView4.Rows.Count - 1 == 0)
            {
                MessageBox.Show("无需要导入数据！");
                return;
            }
            for (int i = 0; i < this.dataGridView4.Rows.Count - 1; i++) //获取
            {
                int b = 0;
                //获取DataGridView中的数据
                Device dModel = new Device();
                if (!string.IsNullOrEmpty(this.dataGridView4.Rows[i].Cells["CId"].Value.ToString()))
                {
                    dModel.Id = this.dataGridView4.Rows[i].Cells["CId"].Value.ToString();
                }
                else
                {
                    dModel.Id = "";
                }
                dModel.Name = this.dataGridView4.Rows[i].Cells["CName"].Value.ToString();
                dModel.Context = this.dataGridView4.Rows[i].Cells["CContext"].Value.ToString();
                dModel.Revision = this.dataGridView4.Rows[i].Cells["CRevision"].Value.ToString();
                dModel.DeviceId = this.dataGridView4.Rows[i].Cells["CDeviceId"].Value.ToString();
                dModel.DeviceName = this.dataGridView4.Rows[i].Cells["CDeviceName"].Value.ToString();
                dModel.ObjectId = getObjId(dModel.Name, dModel.Context, dModel.Revision);//查询获取对应ObjectId

                //查询并获取当前匹配对象ID
                //string sqlId = "select Id from [PRW_Inte_Csgii_DeviceMap] where Name = '" + dModel.Name + "' and Context = '" + dModel.Context + "' and DeviceName='" + dModel.DeviceName + "'";
                //DataSet ds = SqlHelper.ExecuteDataSet(connectionString, sqlId, null);
                string id = "";
                id = dModel.Id;
                //if (ds.Tables[0].Rows.Count>0)
                //{
                //    id = ds.Tables[0].Rows[0][0].ToString();
                //}
                

                //如果存在对象ID进行修改操作
                if (!string.IsNullOrEmpty(id))
                {
                    //定义修改语句
                    string sqlUpd = "update [PRW_Inte_Csgii_DeviceMap] set Id ='" + id + "',";

                    //查询当前对象
                    string sqlSel = "select * from [PRW_Inte_Csgii_DeviceMap] where Id = '" + id + "'";
                    DataSet dsSel = SqlHelper.ExecuteDataSet(connectionString, sqlSel, null);
                    string Name = dsSel.Tables[0].Rows[0]["Name"].ToString();
                    string Context = dsSel.Tables[0].Rows[0]["Context"].ToString();
                    string Revision = dsSel.Tables[0].Rows[0]["Revision"].ToString();
                    string DeviceId = dsSel.Tables[0].Rows[0]["DeviceId"].ToString();
                    string DeviceName = dsSel.Tables[0].Rows[0]["DeviceName"].ToString();

                    //判断当前DataGridView中的数据是否与数据库一致，不一致的进行修改
                    if (dModel.Name != Name)
                    {
                        sqlUpd = sqlUpd + " Name = '" + dModel.Name + "',";
                        b = b + 1;
                    }
                    else if (dModel.Context != Context)
                    {
                        sqlUpd = sqlUpd + " Context = '" + dModel.Context + "',";
                        b = b + 1;
                    }
                    else if (dModel.Revision != Revision)
                    {
                        sqlUpd = sqlUpd + " Revision = '" + dModel.Revision + "',";
                        b = b + 1;
                    }
                    else if (dModel.DeviceId != DeviceId)
                    {
                        sqlUpd = sqlUpd + " DeviceId = '" + dModel.DeviceId + "',";
                        b = b + 1;
                    }
                    else if (dModel.DeviceName != DeviceName)
                    {
                        sqlUpd = sqlUpd + " DeviceName = '" + dModel.DeviceName + "',";
                        b = b + 1;
                    }
                    if (dModel.ObjectId != "" && dModel.ObjectId != dsSel.Tables[0].Rows[0]["ObjectId"].ToString())
                    {
                        b = b + 1;
                    }
                    if (string.IsNullOrEmpty(dsSel.Tables[0].Rows[0]["ObjectId"].ToString()))
                    {
                        sqlUpd = sqlUpd + " ObjectId = null where Id = '" + id + "'";
                    }
                    else
                    {
                        sqlUpd = sqlUpd + " ObjectId = '" + dsSel.Tables[0].Rows[0]["ObjectId"].ToString() + "' where Id = '" + id + "'";
                    }

                    if (b > 0)
                    {
                        listDel.Add(sqlUpd);
                    }
                    
                  
                    //if (v.Station != station && v.MonitorIp != monitorip && v.MonitorPort != monitorport && v.UserName != username && v.PassWord != password && v.Name!=name && v.Context != context && v.Revision != revision && v.ChannelNo != channelNo)
                    //{

                    //}

                }
                else //如果不存在对象ID进行添加
                {
                    if(ds.Tables.Count == 0)
                    {
                        string sqlGH = "select * from [PRW_Inte_Csgii_DeviceMap]";
                        ds = SqlHelper.ExecuteDataSet(connectionString, sqlGH, null);
                    }
                    for (int j = 0; j < ds.Tables[0].Rows.Count; j++)
                    {
                        string Name = ds.Tables[0].Rows[0]["Name"].ToString();
                        string Context = ds.Tables[0].Rows[0]["Context"].ToString();
                        string Revision = ds.Tables[0].Rows[0]["Revision"].ToString();
                        string DeviceId = ds.Tables[0].Rows[0]["DeviceId"].ToString();
                        string DeviceName = ds.Tables[0].Rows[0]["DeviceName"].ToString();
                        if (dModel.Name == Name && dModel.Context == Context && dModel.Revision == Revision && dModel.DeviceId == DeviceId && dModel.DeviceName == DeviceName)
                        {
                            cout = cout + 1;
                            break;
                        }
                    }
                    if (cout == 0)
                    {
                        string sqlIn = "";
                        //如果对象在Object表中找不到对应的ObjectId就以ObjectId为NULL进行添加
                        if (string.IsNullOrEmpty(dModel.ObjectId))
                        {
                            sqlIn = "insert into [PRW_Inte_Csgii_DeviceMap] values(NEWID(),'" + dModel.Name + "','" + dModel.Context + "','" + dModel.Revision + "','" + dModel.DeviceId + "','" + dModel.DeviceName + "',null)";
                        }
                        else//存在就正常添加
                        {
                            sqlIn = "insert into [PRW_Inte_Csgii_DeviceMap] values(NEWID(),'" + dModel.Name + "','" + dModel.Context + "','" + dModel.Revision + "','" + dModel.DeviceId + "','" + dModel.DeviceName + "','" + dModel.ObjectId + "')";
                        }
                        listDel.Add(sqlIn);
                    }
                    
                }
            }
            if (ExecuteSqlTran(listDel) == 1)
            {
                MessageBox.Show("导入数据成功！");
            }
            else
            {
                MessageBox.Show("导入数据失败！");
            }
               
            
        }

        

        /// <summary>
        /// SCADA映射表数据导入数据库
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            DataSet ds = new DataSet();
            int cout = 0;
            List<string> listDel = new List<string>();
            if (this.dataGridView2.Rows.Count - 1 == 0)
            {
                MessageBox.Show("无需要导入数据！");
                return;
            }
            for (int i = 0; i <  this.dataGridView2.Rows.Count - 1; i++) //获取
            {
                //获取DataGridView中的数据
                SCADA sModel = new SCADA();
                if (!string.IsNullOrEmpty(this.dataGridView2.Rows[i].Cells["YXId"].Value.ToString()))
                {
                    sModel.Id = this.dataGridView2.Rows[i].Cells["YXId"].Value.ToString();
                }
                else
                {
                    sModel.Id = "";
                }
                sModel.SiteName = this.dataGridView2.Rows[i].Cells["YXSiteName"].Value.ToString();
                sModel.TagNo = this.dataGridView2.Rows[i].Cells["YXTagNo"].Value.ToString();
                sModel.TagName = this.dataGridView2.Rows[i].Cells["YXTagName"].Value.ToString();
                sModel.TagType = this.dataGridView2.Rows[i].Cells["YXTagType"].Value.ToString();
                sModel.TagDesc = this.dataGridView2.Rows[i].Cells["YXTagDesc"].Value.ToString();
                sModel.Name = this.dataGridView2.Rows[i].Cells["YXName"].Value.ToString();
                sModel.Name2 = this.dataGridView2.Rows[i].Cells["YXName2"].Value.ToString();
                sModel.Context = this.dataGridView2.Rows[i].Cells["YXContext"].Value.ToString();
                sModel.Revision = this.dataGridView2.Rows[i].Cells["YXRevision"].Value.ToString();
                sModel.MessureType = this.dataGridView2.Rows[i].Cells["YXMessureType"].Value.ToString();
                sModel.ObjectId = getObjId(sModel.Name, sModel.Context, sModel.Revision);//查询获取对应ObjectId

                //查询并获取当前匹配对象ID
                string sqlId = "";
                string id = "";
                
                id = sModel.Id;
                
                //如果存在对象ID进行修改操作
                if (!string.IsNullOrEmpty(id))
                {
                    //定义修改语句
                    string sqlUpd = "update [PRW_Inte_SCADA_Map] set Id ='" + id + "',";

                    //查询当前对象
                    string sqlSel = "select * from [PRW_Inte_SCADA_Map] where Id = '" + id + "'";
                    DataSet dsSel = SqlHelper.ExecuteDataSet(connectionString, sqlSel, null);
                    string SiteName = dsSel.Tables[0].Rows[0]["SiteName"].ToString();
                    string TagNo = dsSel.Tables[0].Rows[0]["TagNo"].ToString();
                    string TagName = dsSel.Tables[0].Rows[0]["TagName"].ToString();
                    string TagType = dsSel.Tables[0].Rows[0]["TagType"].ToString();
                    string TagDesc = dsSel.Tables[0].Rows[0]["TagDesc"].ToString();
                    string Name = dsSel.Tables[0].Rows[0]["Name"].ToString();
                    string Name2 = dsSel.Tables[0].Rows[0]["Name2"].ToString();
                    string Context = dsSel.Tables[0].Rows[0]["Context"].ToString();
                    string Revision = dsSel.Tables[0].Rows[0]["Revision"].ToString();
                    string MessureType = dsSel.Tables[0].Rows[0]["MessureType"].ToString();

                    bool isAdd = false;
                    //判断当前DataGridView中的数据是否与数据库一致，不一致的进行修改
                    if (sModel.SiteName != SiteName)
                    {
                        sqlUpd = sqlUpd + " SiteName = '" + sModel.SiteName + "',";
                        isAdd = true;
                    }
                    if (sModel.TagNo != TagNo)
                    {
                        sqlUpd = sqlUpd + " TagNo = '" + sModel.TagNo + "',";
                        isAdd = true;
                    }
                    if (sModel.TagName != TagName)
                    {
                        sqlUpd = sqlUpd + " TagName = '" + sModel.TagName + "',";
                        isAdd = true;
                    }
                    if (sModel.TagType != TagType)
                    {
                        sqlUpd = sqlUpd + " TagType = '" + sModel.TagType + "',";
                        isAdd = true;
                    }
                    if (sModel.TagDesc != TagDesc)
                    {
                        sqlUpd = sqlUpd + " TagDesc = '" + sModel.TagDesc + "',";
                        isAdd = true;
                    }
                    if (sModel.Name != Name)
                    {
                        sqlUpd = sqlUpd + " Name = '" + sModel.Name + "',";
                        isAdd = true;
                    }
                    if (sModel.Name2 != Name2)
                    {
                        sqlUpd = sqlUpd + " Name2 = '" + sModel.Name2 + "',";
                        isAdd = true;
                    }
                    if (sModel.Context != Context)
                    {
                        sqlUpd = sqlUpd + " Context = '" + sModel.Context + "',";
                        isAdd = true;
                    }
                    if (sModel.Revision != Revision)
                    {
                        sqlUpd = sqlUpd + " Revision = '" + sModel.Revision + "',";
                        isAdd = true;
                    }
                    if (sModel.MessureType != MessureType)
                    {
                        sqlUpd = sqlUpd + " MessureType = '" + sModel.MessureType + "',";
                        isAdd = true;
                    }
                    if (sModel.ObjectId != "" && sModel.ObjectId != dsSel.Tables[0].Rows[0]["ObjectId"].ToString())
                    {
                        isAdd = true;
                    }
                    if (!string.IsNullOrEmpty(dsSel.Tables[0].Rows[0]["ObjectId"].ToString()))
                    {
                        sqlUpd += " ObjectId = '" + dsSel.Tables[0].Rows[0]["ObjectId"].ToString() + "' where Id = '" + id + "'";
                    }
                    else
                    {
                        sqlUpd += " ObjectId = null where Id = '" + id + "'";
                    }
                    //sqlUpd = sqlUpd + " ObjectId = '" + dsSel.Tables[0].Rows[0]["ObjectId"].ToString() + "' where Id = '" + id + "'";
                    if (isAdd)
                    {
                        listDel.Add(sqlUpd);
                    }
                    //if (v.Station != station && v.MonitorIp != monitorip && v.MonitorPort != monitorport && v.UserName != username && v.PassWord != password && v.Name!=name && v.Context != context && v.Revision != revision && v.ChannelNo != channelNo)
                    //{

                    //}

                }
                else //如果不存在对象ID进行添加
                {
                    if (ds.Tables.Count == 0)
                    {
                        string sqlW = "select * from [PRW_Inte_SCADA_Map]";
                        ds = SqlHelper.ExecuteDataSet(connectionString, sqlW, null);
                    }
                    
                    for (int j = 0; j < ds.Tables[0].Rows.Count - 1; j++)
                    {
                        string SiteName = ds.Tables[0].Rows[j]["SiteName"].ToString();
                        string TagNo = ds.Tables[0].Rows[j]["TagNo"].ToString();
                        string TagName = ds.Tables[0].Rows[j]["TagName"].ToString();
                        string TagType = ds.Tables[0].Rows[j]["TagType"].ToString();
                        string TagDesc = ds.Tables[0].Rows[j]["TagDesc"].ToString();
                        string Name = ds.Tables[0].Rows[j]["Name"].ToString();
                        string Name2 = ds.Tables[0].Rows[j]["Name2"].ToString();
                        string Context = ds.Tables[0].Rows[j]["Context"].ToString();
                        string Revision = ds.Tables[0].Rows[j]["Revision"].ToString();
                        string MessureType = ds.Tables[0].Rows[j]["MessureType"].ToString();
                        string ObjectId = ds.Tables[0].Rows[j]["ObjectId"].ToString();

                        //进行匹配，避免进行重复操作
                        if (sModel.SiteName == SiteName && sModel.TagNo == TagNo && sModel.TagName == TagName && sModel.TagType == TagType && sModel.TagDesc == TagDesc && sModel.Name == Name && sModel.Name2 == Name2 && sModel.Context == Context && sModel.Revision == Revision && sModel.MessureType == MessureType)
                        {
                            cout = cout + 1;
                            break;
                        }
                    }
                    if (cout == 0)
                    {
                        string sqlIn = "";
                        //根据添加数据是否存在ObjectId进行不同操作
                        if (string.IsNullOrEmpty(sModel.ObjectId))
                        {
                            sqlIn = "insert into [PRW_Inte_SCADA_Map] values(NEWID(),'" + sModel.SiteName + "','" + sModel.TagNo + "','" + sModel.TagName + "','" + sModel.TagType + "','" + sModel.TagDesc + "','" + sModel.Name + "','" + sModel.Name2 + "','" + sModel.Context + "','" + sModel.Revision + "'," + sModel.MessureType + ",null)";
                        }
                        else
                        {
                            sqlIn = "insert into [PRW_Inte_SCADA_Map] values(NEWID(),'" + sModel.SiteName + "','" + sModel.TagNo + "','" + sModel.TagName + "','" + sModel.TagType + "','" + sModel.TagDesc + "','" + sModel.Name + "','" + sModel.Name2 + "','" + sModel.Context + "','" + sModel.Revision + "'," + sModel.MessureType + ",'" + sModel.ObjectId + "')";
                        }
                        listDel.Add(sqlIn);
                    }                    
                }
            }
           if (ExecuteSqlTran(listDel) == 1)
            {
                MessageBox.Show("导入数据成功！");
            }
            else
            {
                MessageBox.Show("导入数据失败！");
            }

            
        }

        /// <summary>
        /// 初始加载数据库内容
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabControl1_Click(object sender, EventArgs e)
        {
            string sql = "select * from [PRW_Inte_Csgii_DeviceMap]";
            DataSet ds = SqlHelper.ExecuteDataSet(connectionString,sql,null);
            this.dataGridView4.DataSource = ds.Tables[0] ;
        }

        /// <summary>
        /// CSGII导出按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button7_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileName = new OpenFileDialog();
            fileName.InitialDirectory = Application.StartupPath;
            fileName.Filter = "All files (*.*)|*.*|xlsx files (*.xlsx)|*.xlsx|xls files (*.xls)|*.xls";
            fileName.FilterIndex = 2;
            fileName.RestoreDirectory = true;
            if (fileName.ShowDialog() == DialogResult.OK)
            {
                string Path = fileName.FileName.ToString();
                string Name = Path.Substring(Path.LastIndexOf("\\") + 1);
                
                this.label13.Text = Path;
                this.label11.Text = Name;
                Name = "aaaa";
                sheetName = "Sheet3";
                ExcelHelper eh = new ExcelHelper();
                DataTable exDt = (DataTable)this.dataGridView4.DataSource;               
                
                if (exDt != null && exDt.Rows.Count>0)
                {
                    eh.ExportData(exDt, Path, sheetName);
                    MessageBox.Show("导出数据成功！");
                }
            }
        }

        
        private void Store_Load(object sender, EventArgs e)
        {
            string Asql = "select [Context] from PRW_Inte_VideoMonitor_CameraMap group by [Context]";
            this.comboBox1.DataSource = Conte(Asql);
            this.comboBox1.ValueMember = "val";
            this.comboBox1.DisplayMember = "dis";

            string Bsql = "select SiteName from PRW_Inte_SCADA_Map group by SiteName";
            this.comboBox2.DataSource = Conte(Bsql);
            this.comboBox2.ValueMember = "val";
            this.comboBox2.DisplayMember = "dis";

            string Csql = "select [TagType] from PRW_Inte_SCADA_Map group by [TagType]";
            this.comboBox3.DataSource = Conte(Csql);
            this.comboBox3.ValueMember = "val";
            this.comboBox3.DisplayMember = "dis";

            string Dsql = "select [Context] from PRW_Inte_Csgii_DeviceMap group by [Context]";
            this.comboBox4.DataSource = Conte(Dsql);
            this.comboBox4.ValueMember = "val";
            this.comboBox4.DisplayMember = "dis";
        }


        public static List<Conts> Conte(string sql) 
        {
            DataSet ds = SqlHelper.ExecuteDataSet(connectionString, sql, null);
            List<Conts> aaa = new List<Conts>();
            Conts c1 = new Conts();
            c1.dis = "全部";
            c1.val = "1";
            aaa.Add(c1);
            #region 站点类型判别
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                string name = "";
                if (ds.Tables[0].Rows[i][0].ToString() == "PE_SMK")
                {
                    name = "思茅变";                    
                }
                if (ds.Tables[0].Rows[i][0].ToString() == "PE_MJ")
                {
                    name = "墨江变";                    
                }
                if (ds.Tables[0].Rows[i][0].ToString() == "PE_MNH")
                {
                    name = "木乃河变";
                }
                if (ds.Tables[0].Rows[i][0].ToString() == "PE_DN")
                {
                    name = "东那变";
                }
                if (ds.Tables[0].Rows[i][0].ToString() == "PE_TS")
                {
                    name = "唐胜变";
                }
                if (ds.Tables[0].Rows[i][0].ToString() == "PE_WH")
                {
                    name = "文华变";
                }
                if (ds.Tables[0].Rows[i][0].ToString() == "PE_NE")
                {
                    name = "宁洱变";
                }
                if (ds.Tables[0].Rows[i][0].ToString() == "PE_XST")
                {
                    name = "细石头变";
                }
                if (ds.Tables[0].Rows[i][0].ToString() == "PE_PE")
                {
                    name = "普洱变";
                }
                if (ds.Tables[0].Rows[i][0].ToString() == "PE_SJC")
                {
                    name = "三家村变";
                }
                if (ds.Tables[0].Rows[i][0].ToString() == "PE_CN")
                {
                    name = "城南变";
                }
                if (ds.Tables[0].Rows[i][0].ToString() == "PE_CY")
                {
                    name = "茶园变";
                }
                if (ds.Tables[0].Rows[i][0].ToString() == "PE_XMH")
                {
                    name = "洗马河变";
                }
                if (ds.Tables[0].Rows[i][0].ToString() == "PE_LJZ")
                {
                    name = "李家寨变";
                }
                if (ds.Tables[0].Rows[i][0].ToString() == "PE_JC")
                {
                    name = "江城变";
                }
                if (ds.Tables[0].Rows[i][0].ToString() == "PE_MC")
                {
                    name = "马厂变";
                }
                if (ds.Tables[0].Rows[i][0].ToString() == "PE_MB")
                {
                    name = "民乐变";
                }
                if (ds.Tables[0].Rows[i][0].ToString() == "PE_QN")
                {
                    name = "迁糯变";
                }
                if (ds.Tables[0].Rows[i][0].ToString() == "PE_ZX")
                {
                    name = "正兴变";
                }
                if (ds.Tables[0].Rows[i][0].ToString() == "PE_LC")
                {
                    name = "澜沧变";
                }
                if (ds.Tables[0].Rows[i][0].ToString() == "PE_XM")
                {
                    name = "西盟变";
                }
                if (ds.Tables[0].Rows[i][0].ToString() == "PE_ML")
                {
                    name = "孟连变";
                }
                if (ds.Tables[0].Rows[i][0].ToString() == "PE_HM")
                {
                    name = "惠民变";
                }
                if (ds.Tables[0].Rows[i][0].ToString() == "PE_JD")
                {
                    name = "景东变";
                }
                if (ds.Tables[0].Rows[i][0].ToString() == "PE_DJ")
                {
                    name = "大街变";
                }
                if (ds.Tables[0].Rows[i][0].ToString() == "PE_EL")
                {
                    name = "恩乐变";
                }
                if (ds.Tables[0].Rows[i][0].ToString() == "PE_ZD")
                {
                    name = "者东变";
                }
                //if (ds.Tables[0].Rows[i][0].ToString() == "PE_QCT")
                //{
                //    name = "芹菜塘";
                //}
                //
                if (ds.Tables[0].Rows[i][0].ToString() == "SMB")
                {
                    name = "思茅变";
                }
                if (ds.Tables[0].Rows[i][0].ToString() == "MJB")
                {
                    name = "墨江变";
                }
                if (ds.Tables[0].Rows[i][0].ToString() == "MNH")
                {
                    name = "木乃河变";
                }
                if (ds.Tables[0].Rows[i][0].ToString() == "DNB")
                {
                    name = "东那变";
                }
                if (ds.Tables[0].Rows[i][0].ToString() == "TSB")
                {
                    name = "唐胜变";
                }
                if (ds.Tables[0].Rows[i][0].ToString() == "WHB")
                {
                    name = "文华变";
                }
                if (ds.Tables[0].Rows[i][0].ToString() == "NEB")
                {
                    name = "宁洱变";
                }
                if (ds.Tables[0].Rows[i][0].ToString() == "XST")
                {
                    name = "细石头变";
                }
                if (ds.Tables[0].Rows[i][0].ToString() == "PEB")
                {
                    name = "普洱变";
                }
                if (ds.Tables[0].Rows[i][0].ToString() == "SJC")
                {
                    name = "三家村变";
                }
                if (ds.Tables[0].Rows[i][0].ToString() == "CNB")
                {
                    name = "城南变";
                }
                if (ds.Tables[0].Rows[i][0].ToString() == "CYB")
                {
                    name = "茶园变";
                }
                if (ds.Tables[0].Rows[i][0].ToString() == "XMH")
                {
                    name = "洗马河变";
                }
                if (ds.Tables[0].Rows[i][0].ToString() == "LJZ")
                {
                    name = "李家寨变";
                }
                if (ds.Tables[0].Rows[i][0].ToString() == "JCB")
                {
                    name = "江城变";
                }
                if (ds.Tables[0].Rows[i][0].ToString() == "MCB")
                {
                    name = "马厂变";
                }
                if (ds.Tables[0].Rows[i][0].ToString() == "MLE")
                {
                    name = "民乐变";
                }
                if (ds.Tables[0].Rows[i][0].ToString() == "QNB")
                {
                    name = "迁糯变";
                }
                if (ds.Tables[0].Rows[i][0].ToString() == "ZXB")
                {
                    name = "正兴变";
                }
                if (ds.Tables[0].Rows[i][0].ToString() == "LCB")
                {
                    name = "澜沧变";
                }
                if (ds.Tables[0].Rows[i][0].ToString() == "XMB")
                {
                    name = "西盟变";
                }
                if (ds.Tables[0].Rows[i][0].ToString() == "MLB")
                {
                    name = "孟连变";
                }
                if (ds.Tables[0].Rows[i][0].ToString() == "HMB")
                {
                    name = "惠民变";
                }
                if (ds.Tables[0].Rows[i][0].ToString() == "JDB")
                {
                    name = "景东变";
                }
                if (ds.Tables[0].Rows[i][0].ToString() == "DJB")
                {
                    name = "大街变";
                }
                if (ds.Tables[0].Rows[i][0].ToString() == "ELB")
                {
                    name = "恩乐变";
                }
                if (ds.Tables[0].Rows[i][0].ToString() == "ZDB")
                {
                    name = "者东变";
                }
                //if (ds.Tables[0].Rows[i][0].ToString() == "QCT")
                //{
                //    name = "芹菜塘";
                //}
                if (ds.Tables[0].Rows[i][0].ToString() == "YC")
                {                    
                    name = "遥测";                    
                }
                if (ds.Tables[0].Rows[i][0].ToString() == "YX")
                {                    
                    name = "遥信";                    
                }
                Conts c = new Conts();
                c.dis = name;
                c.val = ds.Tables[0].Rows[i][0].ToString();
                aaa.Add(c);
            }
            #endregion
            return aaa;
        }

        
        public class Conts 
        {
            public string dis { get; set; }
            public string val { get; set; }
        }

        /// <summary>
        /// 监控设备查看按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button11_Click(object sender, EventArgs e)
        {
            string sql = "SELECT * FROM [PRW_Inte_VideoMonitor_CameraMap] WHERE 1=1 ";
            string sqlwhere = "";
            string Name = this.textBox1.Text.Trim();
            string Context = this.comboBox1.SelectedValue.ToString();
            if (!string.IsNullOrEmpty(Name))
            {
                sqlwhere = sqlwhere + " AND [Name] like '%" + Name + "%'";
            }
            if (Context == "1")
            {
                sqlwhere = sqlwhere + "";
            }
            else
            {
                sqlwhere = sqlwhere + " AND [Context] = '" + Context + "'";
            }
            sql = sql + sqlwhere;
            DataSet ds = SqlHelper.ExecuteDataSet(connectionString, sql, null);
            this.dataGridView1.DataSource = ds.Tables[0];
        }

        /// <summary>
        /// SCADA设备查看按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button12_Click(object sender, EventArgs e)
        {
            string sql = "SELECT * FROM PRW_Inte_SCADA_Map WHERE 1=1  ";
            string sqlwhere = "";
            string tagId = this.textBox2.Text.Trim();
            string tagName = this.textBox3.Text.Trim();
            string sitename = this.comboBox2.SelectedValue.ToString();
            string type = this.comboBox3.SelectedValue.ToString();
            if (!string.IsNullOrEmpty(tagId))
            {
                sqlwhere = sqlwhere + " AND [TagNo] LIKE '%"+tagId+"%'";
            }
            if (!string.IsNullOrEmpty(tagName))
            {
                sqlwhere = sqlwhere + " AND [TagDesc] LIKE '%" + tagName + "%'";
            }
            if (sitename == "1")
            {
                sqlwhere = sqlwhere + "";
            }
            else
            {
                sqlwhere = sqlwhere + " AND [SiteName] = '" + sitename + "'";
            }
            if (type == "1")
            {
                sqlwhere = sqlwhere + "";
            }
            else
            {
                sqlwhere = sqlwhere + " AND [TagType] = '" + type + "'";
            }
            sql = sql + sqlwhere;
            DataSet ds = SqlHelper.ExecuteDataSet(connectionString, sql, null);
            this.dataGridView2.DataSource = ds.Tables[0];
        }
        
        /// <summary>
        /// CSGII设备查看按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button13_Click(object sender, EventArgs e)
        {
            string sql = "SELECT * FROM PRW_Inte_Csgii_DeviceMap WHERE 1=1  ";
            string sqlwhere = "";
            string name = this.textBox4.Text.Trim();
            string context = this.comboBox4.SelectedValue.ToString();
            if (!string.IsNullOrEmpty(name))
            {
                sqlwhere = sqlwhere + " AND [DeviceName] LIKE '%" + name + "%'";
            }
            if (context == "1")
            {
                sqlwhere = sqlwhere + "";
            }
            else
            {
                sqlwhere = sqlwhere + " AND [Context] = '"+ context +"'";
            }
            sql = sql + sqlwhere;
            DataSet ds = SqlHelper.ExecuteDataSet(connectionString, sql, null);
            this.dataGridView4.DataSource = ds.Tables[0];
        }

        /// <summary>
        /// SCADA数据导出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button9_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileName = new OpenFileDialog();
            fileName.InitialDirectory = Application.StartupPath;
            fileName.Filter = "All files (*.*)|*.*|xlsx files (*.xlsx)|*.xlsx|xls files (*.xls)|*.xls";
            fileName.FilterIndex = 2;
            fileName.RestoreDirectory = true;
            if (fileName.ShowDialog() == DialogResult.OK)
            {
                string Path = fileName.FileName.ToString();
                string Name = Path.Substring(Path.LastIndexOf("\\") + 1);

                this.label5.Text = Path;
                this.label1.Text = Name;
                Name = "aaaa";
                sheetName = "Sheet3";
                ExcelHelper eh = new ExcelHelper();
                DataTable exDt = (DataTable)this.dataGridView2.DataSource;

                if (exDt != null && exDt.Rows.Count > 0)
                {
                    eh.ExportData(exDt, Path, sheetName);
                    MessageBox.Show("导出数据成功！");
                }
            }
        }

        /// <summary>
        /// 摄像头导出数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button10_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileName = new OpenFileDialog();
            fileName.InitialDirectory = Application.StartupPath;
            fileName.Filter = "All files (*.*)|*.*|xlsx files (*.xlsx)|*.xlsx|xls files (*.xls)|*.xls";
            fileName.FilterIndex = 2;
            fileName.RestoreDirectory = true;
            if (fileName.ShowDialog() == DialogResult.OK)
            {
                string Path = fileName.FileName.ToString();
                string Name = Path.Substring(Path.LastIndexOf("\\") + 1);

                this.lblPath.Text = Path;
                this.label4.Text = Name;
                
                sheetName = "Sheet1";
                ExcelHelper eh = new ExcelHelper();
                DataTable exDt = (DataTable)this.dataGridView1.DataSource;

                if (exDt != null && exDt.Rows.Count > 0)
                {
                    eh.ExportData(exDt, Path, sheetName);
                    MessageBox.Show("导出数据成功！");
                }
                else
                {
                    MessageBox.Show("当前没有可导出内容，请查询后导出！");
                }
            }
        }

        /// <summary>
        /// CSGII信息数据同步操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button8_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show("确定进行数据同步吗？", "提示", MessageBoxButtons.OKCancel);
            if (dr == DialogResult.OK)
            {
                string sql = "update [PRW_Inte_Csgii_DeviceMap] set ObjectId = null";
                List<string> list = new List<string>();
                if (NoQuery(sql, null))
                {
                    string sqlSel = "SELECT * FROM [PRW_Inte_Csgii_DeviceMap]";
                    DataSet ds = SqlHelper.ExecuteDataSet(connectionString, sqlSel, null);
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        Device dModel = new Device();
                        dModel.Id = ds.Tables[0].Rows[i]["Id"].ToString();
                        dModel.Name = ds.Tables[0].Rows[i]["Name"].ToString();
                        dModel.Context = ds.Tables[0].Rows[i]["Context"].ToString();
                        dModel.Revision = ds.Tables[0].Rows[i]["Revision"].ToString();
                        dModel.ObjectId = getObjId(dModel.Name, dModel.Context, dModel.Revision);//查询获取对应ObjectId

                        string sqlUp = "";
                        if (!string.IsNullOrEmpty(dModel.ObjectId))
                        {
                            sqlUp = "update [PRW_Inte_Csgii_DeviceMap] set ObjectId = '" + dModel.ObjectId + "' where Id = '" + dModel.Id + "'";
                            list.Add(sqlUp);
                        }

                    }
                    if (ExecuteSqlTran(list) == 1)
                    {
                        MessageBox.Show("同步数据成功！");
                    }
                    else
                    {
                        MessageBox.Show("同步数据失败！");
                    }
                }
            }
        }

        /// <summary>
        /// SCADA 信息数据同步操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button4_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show("确定进行数据同步吗？", "提示", MessageBoxButtons.OKCancel);
            if (dr == DialogResult.OK)
            {
                string sql = "update [PRW_Inte_SCADA_Map] set ObjectId = null";
                List<string> list = new List<string>();
                if (NoQuery(sql, null))
                {
                    string sqlSel = "SELECT * FROM [PRW_Inte_SCADA_Map]";
                    DataSet ds = SqlHelper.ExecuteDataSet(connectionString, sqlSel, null);
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        SCADA sModel = new SCADA();
                        sModel.Id = ds.Tables[0].Rows[i]["Id"].ToString();
                        sModel.Name = ds.Tables[0].Rows[i]["Name"].ToString();
                        sModel.Context = ds.Tables[0].Rows[i]["Context"].ToString();
                        sModel.Revision = ds.Tables[0].Rows[i]["Revision"].ToString();
                        sModel.ObjectId = getObjId(sModel.Name, sModel.Context, sModel.Revision);//查询获取对应ObjectId

                        string sqlUp = "";
                        if (!string.IsNullOrEmpty(sModel.ObjectId))
                        {
                            sqlUp = "update [PRW_Inte_SCADA_Map] set ObjectId = '" + sModel.ObjectId + "' where Id = '" + sModel.Id + "'";
                            list.Add(sqlUp);
                        }

                    }
                    if (ExecuteSqlTran(list) == 1)
                    {
                        MessageBox.Show("同步数据成功！");
                    }
                    else
                    {
                        MessageBox.Show("同步数据失败！");
                    }
                }
            }
        }

        /// <summary>
        /// 监控设备信息数据同步
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show("确定进行数据同步吗？", "提示", MessageBoxButtons.OKCancel);
            if (dr == DialogResult.OK)
            {
                string sql = "update [PRW_Inte_VideoMonitor_CameraMap] set ObjectId = null";
                List<string> list = new List<string>();
                if (NoQuery(sql, null))
                {
                    string sqlSel = "SELECT * FROM [PRW_Inte_VideoMonitor_CameraMap]";
                    DataSet ds = SqlHelper.ExecuteDataSet(connectionString, sqlSel, null);
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        VideoMonitor vModel = new VideoMonitor();
                        vModel.Id = ds.Tables[0].Rows[i]["Id"].ToString();
                        vModel.Name = ds.Tables[0].Rows[i]["Name"].ToString();
                        vModel.Context = ds.Tables[0].Rows[i]["Context"].ToString();
                        vModel.Revision = ds.Tables[0].Rows[i]["Revision"].ToString();
                        vModel.ObjectId = getObjId(vModel.Name, vModel.Context, vModel.Revision);//查询获取对应ObjectId

                        string sqlUp = "";
                        if (!string.IsNullOrEmpty(vModel.ObjectId))
                        {
                            sqlUp = "update [PRW_Inte_VideoMonitor_CameraMap] set ObjectId = '" + vModel.ObjectId + "' where Id = '" + vModel.Id + "'";
                            list.Add(sqlUp);
                        }

                    }
                    if (ExecuteSqlTran(list) == 1)
                    {
                        MessageBox.Show("同步数据成功！");
                    }
                    else
                    {
                        MessageBox.Show("同步数据失败！");
                    }
                }
            }
        }
     
    }
}
