using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using Genersoft.Platform.AppFramework.Service;
using Genersoft.Platform.Core.DataAccess;

namespace Genersoft.Drp.NSBDApp.Core
{
    /// <summary>
    /// 访问数据库工具类
    /// </summary>
    public class MobileQueryManager
    {
        /// <summary>
        /// 获取LSCONF配置值
        /// </summary>
        /// <param name="confKey">key</param>
        /// <param name="mkID">模块ID,默认UM</param>
        /// <returns></returns>
        public static bool GetConfValue(string confKey, string mkID = "UM")
        {
            IGSPDatabase arg_25_0 = GSPContext.Current.Database;
            string sqlStatement = string.Empty;
            bool result = false;
            sqlStatement = "select  LSCONF_VALUE from  LSCONF where  LSCONF_MKID='{0}' and LSCONF_KEY={1} ";
            DataSet dataSet = arg_25_0.ExecuteDataSet(sqlStatement, new object[]
			{
				mkID,confKey
			});
            if (dataSet != null && dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
            {
                result = (dataSet.Tables[0].Rows[0]["LSCONF_VALUE"].ToString() == "1");
            }
            return result;
        }

        /// <summary>
        /// 获取按键配置
        /// </summary>
        /// <param name="funcID">功能ID</param>
        /// <param name="mkID">模块ID,默认UM</param>
        /// <returns></returns>
        public static DataSet GetConfSet(string funcID, string mkID = "UM")
        {
            IGSPDatabase arg_25_0 = GSPContext.Current.Database;
            string sqlStatement = string.Empty;
            //DataSet result = new DataSet();
            sqlStatement = @"select  LSCONF_MKID,LSCONF_FuncID,LSCONF_BtnKEY,LSCONF_BtnText,
LSCONF_BtnCustomText,LSCONF_IsVisible,LSCONF_URL,LSCONF_NOTE from  LSMobileButtonConf where  LSCONF_MKID={0} and LSCONF_FuncID={1} and LSCONF_IsVisible='1'";
            DataSet dataSet = arg_25_0.ExecuteDataSet(sqlStatement, new object[]
			{
				mkID,funcID
			});
            if (dataSet != null && dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
            {
                return dataSet;
            }
            else
                return null;
            
        }
        /// <summary>
        /// 获取配置的备注
        /// </summary>
        /// <param name="confKey">key</param>
        /// <param name="mkID">模块ID,默认UM</param>
        /// <returns></returns>
        public static string GetConfNote(string confKey,string mkID="UM")
        {
            IGSPDatabase arg_29_0 = GSPContext.Current.Database;
            string sqlStatement = string.Empty;
            string result = string.Empty;
            sqlStatement = "select  LSCONF_NOTE from  LSCONF where  LSCONF_MKID={0} and LSCONF_KEY={1} ";
            DataSet dataSet = arg_29_0.ExecuteDataSet(sqlStatement, new object[]
			{
				mkID,confKey
			});
            if (dataSet != null && dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
            {
                result = dataSet.Tables[0].Rows[0]["LSCONF_NOTE"].ToString();
            }
            return result;
        }
    }
}