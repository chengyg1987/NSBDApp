using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Web;
using System.Xml;
using System.Xml.Serialization;
using Genersoft.Platform.AppFramework.Service;
using Newtonsoft.Json;

namespace Genersoft.Drp.NSBDApp.Core
{
    public class MyHelperProvider
    {
        #region 记录日志
        public static void WriteLogToText(string fileName, string strLog)
        {
            //创建文件夹
            string strPath = @"D:\\LOG\\" + fileName + "\\";
            if (!Directory.Exists(strPath))
            {
                System.IO.Directory.CreateDirectory(strPath);
            }
            string curDate = DateTime.Now.ToString("yyyyMMdd");


            try
            {
                //创建正常文件
                string rightPath = strPath + "LogFile" + curDate + ".log";
                using (FileStream fs = new FileStream(rightPath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite))
                {
                    string curDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff");
                    StreamWriter sw = new StreamWriter(fs);
                    sw.BaseStream.Seek(0, SeekOrigin.End);
                    sw.WriteLine(curDateTime + " " + strLog);
                    sw.Flush();
                    sw.Close();
                    fs.Close();
                }
            }
            catch (Exception)
            {
                //创建异常文件
                string exPath = strPath + "LogFile" + curDate + "_" + Thread.CurrentThread.ManagedThreadId.ToString() + ".log";
                using (FileStream fs = new FileStream(exPath, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    string curDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff");
                    StreamWriter sw = new StreamWriter(fs);
                    sw.BaseStream.Seek(0, SeekOrigin.End);
                    sw.WriteLine(curDateTime + " " + strLog);
                    sw.Flush();
                    sw.Close();
                    fs.Close();
                }
            }
        }
        #endregion

        #region base64编码
        public static string EncodeBase64(string code_type, string code)
        {
            string encode = "";
            byte[] bytes = Encoding.GetEncoding(code_type).GetBytes(code);
            try
            {
                encode = Convert.ToBase64String(bytes);
            }
            catch
            {
                encode = code;
            }
            return encode;
        }
        #endregion

        #region base64解码
        public static string DecodeBase64(string code_type, string code)
        {
            string decode = "";
            byte[] bytes = Convert.FromBase64String(code);
            try
            {
                decode = Encoding.GetEncoding(code_type).GetString(bytes);
            }
            catch
            {
                decode = code;
            }
            return decode;
        }
        #endregion

        #region HttpGet--JSON
        public static string HttpGet(string Url)
        {
            string result;

            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(Url);
            httpWebRequest.Method = "GET";
            httpWebRequest.ContentType = "application/json;charset=UTF-8";
            HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            Stream responseStream = httpWebResponse.GetResponseStream();
            StreamReader streamReader = new StreamReader(responseStream, Encoding.UTF8);
            string text = streamReader.ReadToEnd();
            streamReader.Close();
            responseStream.Close();
            result = System.Web.HttpUtility.UrlDecode(text, Encoding.GetEncoding("UTF-8"));

            return result;
        }
        #endregion

        #region HttpPost--JSON
        public static string HttpPost(string Url, string JsonStr)
        {
            string strRes = string.Empty;
            HttpWebRequest hwRequ = (HttpWebRequest)WebRequest.Create(Url);
            hwRequ.Method = "POST";
            hwRequ.ContentType = "application/json;charset=UTF-8";
            byte[] btData = Encoding.UTF8.GetBytes(JsonStr);
            hwRequ.ContentLength = btData.Length;
            using (Stream reqStream = hwRequ.GetRequestStream())
            {
                reqStream.Write(btData, 0, btData.Length);
                reqStream.Close();
            }

            HttpWebResponse hwResp = (HttpWebResponse)hwRequ.GetResponse();
            Stream stream = hwResp.GetResponseStream();
            using (StreamReader sReader = new StreamReader(stream, Encoding.UTF8))
            {
                strRes = sReader.ReadToEnd();
                sReader.Close();
            }
            stream.Close();

            return strRes;

        }

        #endregion

        #region HttpPost--XML
        public static string HttpPostXML(string Url, string JsonStr)
        {
            string strRes = string.Empty;
            HttpWebRequest hwRequ = (HttpWebRequest)WebRequest.Create(Url);
            hwRequ.Method = "POST";
            hwRequ.ContentType = "application/xml;charset=UTF-8";
            byte[] btData = Encoding.UTF8.GetBytes(JsonStr);
            hwRequ.ContentLength = btData.Length;
            using (Stream reqStream = hwRequ.GetRequestStream())
            {
                reqStream.Write(btData, 0, btData.Length);
                reqStream.Close();
            }

            HttpWebResponse hwResp = (HttpWebResponse)hwRequ.GetResponse();
            Stream stream = hwResp.GetResponseStream();
            using (StreamReader sReader = new StreamReader(stream, Encoding.UTF8))
            {
                strRes = sReader.ReadToEnd();
                sReader.Close();
            }
            stream.Close();

            return strRes;

        }

        #endregion

        #region 获取URL短网址
        public static string GetShortURL(string longURL)
        {
            string strURL = "https://dwz.cn/admin/v2/create";
            string strData = string.Format(@"{0}""url"":""{1}""{2}", "{", longURL, "}");
            HttpWebRequest hwRequ = (HttpWebRequest)WebRequest.Create(strURL);
            hwRequ.Method = "POST";
            hwRequ.Headers.Add("Token", "3f6ff0fd27cb92aefadc680241fc7ead");
            hwRequ.ContentType = "application/json;charset=UTF-8";
            byte[] btData = Encoding.UTF8.GetBytes(strData);
            hwRequ.ContentLength = btData.Length;
            using (Stream reqStream = hwRequ.GetRequestStream())
            {
                reqStream.Write(btData, 0, btData.Length);
                reqStream.Close();
            }
            string strRes = string.Empty;
            HttpWebResponse hwResp = (HttpWebResponse)hwRequ.GetResponse();
            Stream stream = hwResp.GetResponseStream();
            using (StreamReader sReader = new StreamReader(stream, Encoding.UTF8))
            {
                strRes = sReader.ReadToEnd();
                sReader.Close();
            }
            return strRes;
        }
        #endregion

        #region XML序列化
        public static string XmlSerializer<T>(T obj)
        {
            using (StringWriter sw = new StringWriter())
            {
                Type t = obj.GetType();
                XmlSerializer serializer = new XmlSerializer(obj.GetType());
                serializer.Serialize(sw, obj);
                sw.Close();
                return sw.ToString();
            }
        }

        /// <summary>
        /// XML序列化
        /// </summary>
        /// <typeparam name="T">指定类型</typeparam>
        /// <param name="obj">实例对象</param>
        /// <param name="encoding">编码格式</param>
        /// <returns>格式化字符串</returns>
        public static string XmlSerializer<T>(T obj, Encoding encoding) where T : class
        {
            using (MemoryStream ms = new MemoryStream())
            {
                var setting = new XmlWriterSettings()
                {
                    Encoding = encoding,
                    Indent = true,
                    OmitXmlDeclaration = true,//控制<?xml version='1.0' encoding='GBK'?>是否显示
                };
                using (XmlWriter xw = XmlWriter.Create(ms, setting))
                {
                    XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                    ns.Add("", "");//控制<Person xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">是否显示

                    XmlSerializer xs = new XmlSerializer(obj.GetType());
                    xs.Serialize(xw, obj, ns);
                    return encoding.GetString(ms.ToArray());
                }
            }
        }

        #endregion

        #region XML反序列化
        public static T XmlDeserializer<T>(string xml)
        {
            T myObject;
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            StringReader reader = new StringReader(xml);
            myObject = (T)serializer.Deserialize(reader);
            reader.Close();
            return myObject;
        }

        /// <summary>
        /// XML反序列化
        /// </summary>
        /// <typeparam name="T">指定类型</typeparam>
        /// <param name="xml">XML字符串</param>
        /// <param name="encoding">编码格式</param>
        /// <returns>实例对象</returns>
        public static T XmlDeserializer<T>(string xml, Encoding encoding) where T : class, ISerializable
        {
            using (MemoryStream ms = new MemoryStream(encoding.GetBytes(xml)))
            {
                using (XmlReader xr = XmlReader.Create(ms))
                {
                    XmlSerializer xs = new XmlSerializer(typeof(T));
                    return (T)xs.Deserialize(xr);
                }
            }
        }
        #endregion

        #region 构造服务端Session

        public static void CreateSession(string appCode)
        {
            Hashtable table = new Hashtable();
            table.Add("AppInstanceID", appCode);
            table.Add("BizDate", DateTime.Now.ToString());
            table.Add("LoginDate", DateTime.Now.ToString());
            table.Add("UserCode", "mh");
            GSPState.IgnoreCheck();
            GSPState state = GSPState.CreateState(table, true);//重新生成ID
            GSPState.SetServerState(state);
        }

        public static void CreateSession(string appCode, string userCode, string userId)
        {
            Hashtable table = new Hashtable();
            table.Add("AppInstanceID", appCode);
            table.Add("AppCode", appCode);
            table.Add("BizDate", DateTime.Now.ToString());
            table.Add("LoginDate", DateTime.Now.ToString());
            table.Add("UserCode", userCode);
            table.Add("UserID", userId);

            GSPState.IgnoreCheck();
            GSPState state = GSPState.CreateState(table, true);//重新生成ID
            GSPState.SetServerState(state);
        }
        #endregion

        #region DataRow转化为实体类
        /// <summary>
        /// 此方法列名不区分大小写,但是数据库的类型要和类属性的类型对应上,否则就会返回NULl
        /// </summary>
        /// <param name="dr"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static object DataRowToObj(System.Data.DataRow dr, object obj)
        {
            try
            {
                if (dr != null && obj != null)
                {
                    Type t = obj.GetType();
                    PropertyInfo[] f = t.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
                    string tableName = t.Name;
                    foreach (PropertyInfo pi in f)
                    {
                        string field = pi.Name;

                        if (dr.Table.Columns.Contains(field))
                        {
                            string sourceValue = dr[field] == DBNull.Value ? "" : Convert.ToString(dr[field]);//如果数据库存放的是NULL转换为空字符串
                            pi.SetValue(obj, sourceValue, null);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                new Exception(ex.ToString());
                return null;
            }
            return obj;
        }
        #endregion

        #region 获取客户端IP
        public static string GetClientIp()
        {
            if (System.Web.HttpContext.Current.Request.ServerVariables["HTTP_VIA"] != null)
            {
                return System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"].Split(new char[] { ',' })[0];
            }
            else
            {
                return System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }
        }
        #endregion

        #region 过滤XSS脚本
        /// <summary>
        /// 
        /// </summary>
        /// <param name="html">传入字符串</param>
        /// <returns>过滤后的字符串</returns>
        public static string FilterXSS(string html)
        {
            if (string.IsNullOrEmpty(html)) return string.Empty;

            // CR(0a) ，LF(0b) ，TAB(9) 除外，过滤掉所有的不打印出来字符.    
            // 目的防止这样形式的入侵 ＜java\0script＞   
            // 注意：\n, \r,  \t 可能需要单独处理，因为可能会要用到   
            string ret = System.Text.RegularExpressions.Regex.Replace(
                html, "([\x00-\x08][\x0b-\x0c][\x0e-\x20])", string.Empty);

            //替换所有可能的16进制构建的恶意代码   
            //<IMG SRC=&#X40&#X61&#X76&#X61&#X73&#X63&#X72&#X69&#X70&#X74&#X3A&#X61&_#X6C&#X65&#X72&#X74&#X28&#X27&#X58&#X53&#X53&#X27&#X29>  
            string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%^&*()~`;:?+/={}[]-_|'\"\\";
            for (int i = 0; i < chars.Length; i++)
            {
                ret = System.Text.RegularExpressions.Regex.Replace(ret, string.Concat("(&#[x|X]0{0,}", Convert.ToString((int)chars[i], 16).ToLower(), ";?)"),
                    chars[i].ToString(), System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            }

            //过滤\t, \n, \r构建的恶意代码  
            string[] keywords = {"javascript", "vbscript", "expression", "applet", "meta", "xml", "blink", "link", "style", "script", "embed", "object", "iframe", "frame", "frameset", "ilayer", "layer", "bgsound", "title", "base"
        ,"onabort", "onactivate", "onafterprint", "onafterupdate", "onbeforeactivate", "onbeforecopy", "onbeforecut", "onbeforedeactivate", "onbeforeeditfocus", "onbeforepaste", "onbeforeprint", "onbeforeunload", "onbeforeupdate", "onblur", "onbounce", "oncellchange", "onchange", "onclick", "oncontextmenu", "oncontrolselect", "oncopy", "oncut", "ondataavailable", "ondatasetchanged", "ondatasetcomplete", "ondblclick", "ondeactivate", "ondrag", "ondragend", "ondragenter", "ondragleave", "ondragover", "ondragstart", "ondrop", "onerror", "onerrorupdate", "onfilterchange", "onfinish", "onfocus", "onfocusin", "onfocusout", "onhelp", "onkeydown", "onkeypress", "onkeyup", "onlayoutcomplete", "onload", "onlosecapture", "onmousedown", "onmouseenter", "onmouseleave", "onmousemove", "onmouseout", "onmouseover", "onmouseup", "onmousewheel", "onmove", "onmoveend", "onmovestart", "onpaste", "onpropertychange", "onreadystatechange", "onreset", "onresize", "onresizeend", "onresizestart", "onrowenter", "onrowexit", "onrowsdelete", "onrowsinserted", "onscroll", "onselect", "onselectionchange", "onselectstart", "onstart", "onstop", "onsubmit", "onunload"};

            bool found = true;
            while (found)
            {
                var retBefore = ret;
                for (int i = 0; i < keywords.Length; i++)
                {
                    string pattern = "/";
                    for (int j = 0; j < keywords[i].Length; j++)
                    {
                        if (j > 0)
                            pattern = string.Concat(pattern, '(', "(&#[x|X]0{0,8}([9][a][b]);?)?", "|(&#0{0,8}([9][10][13]);?)?",
                                ")?");
                        pattern = string.Concat(pattern, keywords[i][j]);
                    }
                    string replacement = string.Concat(keywords[i].Substring(0, 2), "＜x＞", keywords[i].Substring(2));
                    ret = System.Text.RegularExpressions.Regex.Replace(ret, pattern, replacement, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                    if (ret == retBefore)
                        found = false;
                }

            }

            return ret;
        }
        #endregion

        #region 获取配置文件
        /// <summary>
        /// <InvoiceType><Item key="1">1,2,11,12,51,14</Item><Item key="2">31,32,33</Item><Item key="3">21,22,52,55</Item></InvoiceType> 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetConfigByKey(string key)
        {

            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(System.Web.HttpContext.Current.Server.MapPath("/cwbase/JTGL/Image") + "/ImageConfig.xml");
                string result = string.Empty;

                XmlNodeList nodelist = doc.SelectNodes("//InvoiceType//Item");

                foreach (XmlNode node in nodelist)
                {
                    if (node.Attributes["key"].InnerText == key)
                    {
                        result = node.InnerText;
                    }
                }
                return result;

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        #endregion

        #region  DES加密
        /// <summary>
        /// DES加密
        /// </summary>
        /// <param name="str">要加密的字符串</param>
        /// <param name="key">密钥（8字节多余无效）</param>
        /// <param name="IV">偏移量</param>
        /// <returns>加密后Base64编码</returns>
        public static string DesEncrypt(string str, string key, string IV)
        {
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            byte[] inputByteArray = Encoding.UTF8.GetBytes(str);
            des.Key = UTF8Encoding.UTF8.GetBytes(key);// 密匙（只能8字节）
            des.IV = UTF8Encoding.UTF8.GetBytes(IV);//偏移量（ECB模式下不需要，其余和key相等即可）
            des.Mode = System.Security.Cryptography.CipherMode.CBC;//加密模式
            des.Padding = System.Security.Cryptography.PaddingMode.PKCS7;//填充模式（对应JAVA中的PKCS5）
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            ms.Dispose();
            cs.Dispose();
            var retB = Convert.ToBase64String(ms.ToArray());
            return retB;
        }
        #endregion

        #region DES解密
        /// <summary>
        /// DES解密
        /// </summary>
        /// <param name="str">需要解密的字符串</param>
        /// <param name="key">密钥</param>
        /// <returns>解密后明文字符串</returns>
        public static string DesDecrypt(string str, string key)
        {
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            byte[] inputByteArray = Convert.FromBase64String(str);
            des.Key = UTF8Encoding.UTF8.GetBytes(key);
            des.IV = UTF8Encoding.UTF8.GetBytes(key);
            des.Mode = System.Security.Cryptography.CipherMode.CBC;//加密模式
            des.Padding = System.Security.Cryptography.PaddingMode.PKCS7;//填充模式（对应JAVA中的PKCS5）
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            // 如果两次密匙不一样，这一步可能会引发异常
            cs.FlushFinalBlock();
            string reStr = System.Text.Encoding.Default.GetString(ms.ToArray());
            ms.Dispose();
            cs.Dispose();
            return reStr;
        }
        #endregion

        #region 16进制转换为字节数组
        public static byte[] strToToHexByte(string hexString)
        {
            hexString = hexString.Replace(" ", "");
            if ((hexString.Length % 2) != 0)
                hexString += " ";
            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            return returnBytes;
        }
        #endregion

        #region 格式化XML
        public static string formatXml(object xml)
        {
            XmlDocument xd;
            if (xml is XmlDocument)
            {
                xd = xml as XmlDocument;
            }
            else
            {
                xd = new XmlDocument();
                xd.LoadXml(xml as string);
            }
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            XmlTextWriter xtw = null;
            try
            {
                xtw = new XmlTextWriter(sw);
                xtw.Formatting = System.Xml.Formatting.Indented;
                xtw.Indentation = 1;
                xtw.IndentChar = '\t';
                xd.WriteTo(xtw);
            }
            finally
            {
                if (xtw != null)
                    xtw.Close();
            }
            return sb.ToString();
        }
        #endregion

        #region 格式化JSON
        public static string ConvertJsonString(string str)
        {
            //格式化json字符串
            JsonSerializer serializer = new JsonSerializer();
            TextReader tr = new StringReader(str);
            JsonTextReader jtr = new JsonTextReader(tr);
            object obj = serializer.Deserialize(jtr);
            if (obj != null)
            {
                StringWriter textWriter = new StringWriter();
                JsonTextWriter jsonWriter = new JsonTextWriter(textWriter)
                {
                    Formatting = Newtonsoft.Json.Formatting.Indented,
                    Indentation = 4,
                    IndentChar = ' '
                };
                serializer.Serialize(jsonWriter, obj);
                return textWriter.ToString();
            }
            else
            {
                return str;
            }
        }

        #endregion
    }
}