using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NPOI.HSSF.UserModel;
using NPOI.SS.Formula.Functions;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using Excel = Microsoft.Office.Interop.Excel;

namespace PR.WizPlant.DataImport
{
    public class ExcelHelper
    {
        private static object lockObj = new object();
        private static ExcelHelper instance;
        /// <summary>
        /// 单例
        /// </summary>
        public static ExcelHelper Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (lockObj)
                    {
                        if (instance == null)
                        {
                            instance = new ExcelHelper();
                        }
                    }
                }
                return instance;
            }
        }

        Excel.Worksheet sheet = null;

        /// <summary>
        /// 将DataTable数据导入到excel中
        /// </summary>
        /// <param name="data">要导入的数据</param>
        /// <param name="isColumnWritten">DataTable的列名是否要导入</param>
        /// <param name="sheetName">要导入的excel的sheet的名称</param>
        /// <returns>导入数据行数(包含列名那一行)</returns>
        public string ExportData(DataTable data, string filePath, string sheetName)
        {
            IWorkbook workbook = null;          //定义表
            FileStream fs = null;               //定义操作功能对象
            string fileName = filePath;         //路径

            int i = 0;
            int j = 0;
            int count = 0;
            //string filePath = Path.GetDirectoryName(fileName);
            if (!Directory.Exists(Path.GetDirectoryName(fileName)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(fileName));
            }
            ISheet sheet = null;               //定义sheet页
            bool isColumnWritten = true;
            fs = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite);    //设置操作功能
            if (fileName.IndexOf(".xlsx") > 0) // 2007版本
                workbook = new XSSFWorkbook();
            else if (fileName.IndexOf(".xls") > 0) // 2003版本
                workbook = new HSSFWorkbook();
            
            try
            {
                if (workbook != null)
                {
                    sheet = workbook.CreateSheet(sheetName);                            
                }
                else
                {
                    return null;
                }

                if (isColumnWritten == true) //写入DataTable的列名
                {
                    IRow row = sheet.CreateRow(0);
                    for (j = 0; j < data.Columns.Count; ++j)  
                    {
                        row.CreateCell(j).SetCellValue(data.Columns[j].ColumnName);
                    }
                    count = 1;
                }
                else
                {
                    count = 0;
                }

                for (i = 0; i < data.Rows.Count; ++i)
                {
                    
                    IRow row = sheet.CreateRow(count);
                    for (j = 0; j < data.Columns.Count; ++j)
                    {
                        row.CreateCell(j).SetCellValue(data.Rows[i][j].ToString());
                    }
                    ++count;
                }
                workbook.Write(fs); //写入到excel

                return fileName;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
                return null;
            }
            finally
            {
                fs.Close();
            }
        }

        /// <summary>
        /// Sheet1 导出
        /// </summary>
        /// <param name="data"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public string ExportData(DataTable data, string filePath)
        {
            return ExportData(data, filePath, "Sheet1");
        }

        /// <summary>
        /// 将excel中的数据导入到DataTable中
        /// </summary>
        /// <param name="sheetName">excel工作薄sheet的名称</param>
        /// <param name="isFirstRowColumn">第一行是否是DataTable的列名</param>
        /// <returns>返回的DataTable</returns>
        public DataTable ImportData(string fileName, string sheetName)
        {
            IWorkbook workbook = null;
            FileStream fs = null;
            bool isFirstRowColumn = true;
            ISheet sheet = null;
            DataTable data = new DataTable();
            int startRow = 0;
            try
            {
                fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                if (fileName.IndexOf(".xlsx") > 0) // 2007版本
                    workbook = new XSSFWorkbook(fs);
                else if (fileName.IndexOf(".xls") > 0) // 2003版本
                    workbook = new HSSFWorkbook(fs);
                
                if (sheetName != null)
                {
                    sheet = workbook.GetSheet(sheetName);
                    if (sheet == null) //如果没有找到指定的sheetName对应的sheet，则尝试获取第一个sheet
                    {
                        sheet = workbook.GetSheetAt(0);                        
                    }
                }
                else
                {
                    sheet = workbook.GetSheetAt(0);
                }
                if (sheet != null)
                {
                    IRow firstRow = sheet.GetRow(0);
                    int cellCount = firstRow.LastCellNum; //一行最后一个cell的编号 即总的列数

                    if (isFirstRowColumn)
                    {
                        for (int i = firstRow.FirstCellNum; i < cellCount; ++i)
                        {
                            ICell cell = firstRow.GetCell(i);
                            if (cell != null)
                            {
                                string cellValue = cell.StringCellValue;
                                if (cellValue != null)
                                {
                                    DataColumn column = new DataColumn(cellValue);
                                    data.Columns.Add(column);
                                }
                            }
                        }
                        startRow = sheet.FirstRowNum + 1;
                    }
                    else
                    {
                        startRow = sheet.FirstRowNum;
                    }

                    //最后一列的标号
                    int rowCount = sheet.LastRowNum;
                    for (int i = startRow; i <= rowCount; ++i)
                    {
                        IRow row = sheet.GetRow(i);
                        if (row == null) continue; //没有数据的行默认是null　　　　　　　
                        
                        DataRow dataRow = data.NewRow();
                        for (int j = row.FirstCellNum; j < cellCount; ++j)
                        {
                            if (row.GetCell(j) != null) //同理，没有数据的单元格都默认是null

                                try
                                {
                                    switch (row.GetCell(j).CellType)   //判断当前单元格数据是什么数据类型
                                    {
                                        case CellType.Blank: //BLANK:
                                            dataRow[j] = "";
                                            break;
                                        case CellType.Boolean: //BOOLEAN:
                                            dataRow[j] = row.GetCell(j).ToString();
                                            break;
                                        case CellType.Numeric: //NUMERIC:
                                            dataRow[j] = row.GetCell(j).ToString();
                                            break;
                                        case CellType.String: //STRING:
                                            dataRow[j] = row.GetCell(j).ToString();
                                            break;
                                        case CellType.Formula: //FORMULA:  //如果当前单元格数据是经过公式计算得到的，需要用RichStringCellValue取得计算过后的值
                                            if (row.GetCell(j).CachedFormulaResultType == CellType.String)
                                            {
                                                dataRow[j] = row.GetCell(j).RichStringCellValue;
                                            }
                                            if (row.GetCell(j).CachedFormulaResultType == CellType.Numeric)
                                            {
                                                dataRow[j] = row.GetCell(j).NumericCellValue;
                                            }
                                            
                                            break;
                                        default:
                                            break;
                                    }
                                    
                                }
                                catch (Exception)
                                {
                                    dataRow[j] = "";                                    
                                }
                                
                                                               
                        }
                        data.Rows.Add(dataRow);
                    }
                }

                return data;
            }
            catch (Exception ex)
            {
                LogHelper.WriteErrorLog(ex);
                return null;
            }
            finally
            {
                fs.Close();
            }
        }

        /// <summary>
        /// Sheet1的数据
        /// </summary>
        /// <param name="fileName"></param>Class1.cs
        /// <returns></returns>
        public DataTable ImportData(string fileName)
        {
            return ImportData(fileName, "Sheet1");
        }
    }
}
