using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using Genersoft.Platform.AppFramework.Service;
using Genersoft.Platform.Core.DataAccess;
using Genersoft.Platform.WebBillQuery.Common.Data;
using Genersoft.Platform.WebBillQuery.SPI;
using Genersoft.RO.MobileQueryIMGInterface.Core;
using Newtonsoft.Json;

namespace Genersoft.Drp.NSBDApp.Core
{
    /// <summary>
    /// 移动端审批子表联查
    /// </summary>
    public class MoApprovalQry : IQueryBizAssembly
    {

        private const string LogName = "移动审批联查子表";
        IGSPDatabase db = GSPContext.Current.Database;

        /// <summary>
        /// 根据提供的参数信息对原始的DataSet形式的业务数据进行组装整理，返回整理后格式化的展示数据
        /// </summary>
        /// <param name="funcID">可定义为功能ID，也可为业务上下文ID，无实际意义</param>
        /// <param name="bizParameter">业务参数键值对</param>
        /// <param name="bizData">原始业务数据</param>
        /// <returns>整理后可直接格式化展示的数据</returns>
        public QueryResultData GetBizFormatResult(string funcID, Dictionary<string, string> bizParameter, DataSet bizData)
        {
            MyHelperProvider.WriteLogToText(LogName, "--------------------移动审批联查子表开始--------------------");
            QueryResultData resultData = new QueryResultData();
            try
            {
                string ifQYYX = string.Empty;
                
                //预算                
                //string ywlx = "";
                string billID =  Convert.ToString(bizData.Tables[0].Rows[0]["ROBXDJ_NM"]);

                //各功能根据业务向LSMobileButtonConf表预制数据，调用下面方法时选择对应的功能ID即可
                DataSet ds = MobileQueryManager.GetConfSet(EnumClass.MyFuncEnum.MobileJFD.ToString(), "UM");

                string html = string.Empty;
                if (ds != null && ds.Tables != null && ds.Tables[0].Rows.Count > 1)
                {
                    //多个按钮时增加展开、隐藏组
                    setOnePlus(ref html, ds,billID);
                }
                else if (ds != null && ds.Tables != null && ds.Tables[0].Rows.Count == 1)
                {
                    //只有一个按钮时直接显示
                    string url = ds.Tables[0].Rows[0]["LSCONF_URL"].ToString();
                    string customText = ds.Tables[0].Rows[0]["LSCONF_BtnCustomText"].ToString();
                    string btnID = ds.Tables[0].Rows[0]["LSCONF_BtnKEY"].ToString();
                    string btnText = ds.Tables[0].Rows[0]["LSCONF_BtnText"].ToString();
                    if (!string.IsNullOrEmpty(customText))
                    {
                        btnText = customText;
                    }
                    setOne(ref html,url,btnText,billID);
                }
                else
                    html = string.Empty;//都没有时清空html


                resultData.FormatID = bizParameter["WebBillQuery_Runtime_FormatID"];//此处前期接口参数中没处理好，为避免影响已有实现，只得在此做了约定
                resultData.Data = JsonConvert.SerializeObject(bizData);
                MyHelperProvider.WriteLogToText(LogName, "移动审批影像接口html:" + html);
                resultData.Content = html;//此处是根据影像数据形成的Html片段
            }
            catch (Exception ex)
            {
                MyHelperProvider.WriteLogToText(LogName, "移动审批影像接口ex:" + ex);
                MyHelperProvider.WriteLogToText(LogName, "移动审批影像接口ds:" + bizData.GetXml());
            }
            finally
            {
                resultData.FormatID = bizParameter["WebBillQuery_Runtime_FormatID"];
                MyHelperProvider.WriteLogToText(LogName, "--------------------移动审批联查子表结束--------------------");
            }

            return resultData;
        }
        public string setOne(ref string html, string url,string name,string billID)
        {
            Random rd = new Random();
            int ird = rd.Next();
            html = string.Empty;
            html += "<html lang=\"zh\">";
            html += "<head>";
            html += "  <link rel=\"stylesheet\" type=\"text/css\" href=\"/cwbase/webapp/Docs/css/romobilestyles.css?v=" + ird + "\">";
            html += "  </head>";
            html += "  <body>";
            html += "  <section id=\"roMobileTouchMoveMain\" style=\"position: fixed;right: 0px;bottom: 25px \" class=\"model-4\">";
            html += "   <div class=\"float-btn-group\">";
            html += " <a href=\"#\" id =\"roMobileTouchMove\" class=\"btn-float blue\" onclick=\"window.top.gsp.rtf.func.openUrl({id:'viewcxxx',name:'" + name + "', url:'" + url + "'})\">" + name + "</a>";
            html += " </div>";
            html += "  </section>";
            html += "<script>";
            if (MobileQueryManager.GetConfValue("ROMOBILE_YDSPZCTD", "ROMOB"))//移动审批支持拖动
            {
                html += " document.getElementById('roMobileTouchMove').addEventListener('touchmove', function (evt) {  }, false); " +
                    "$('#roMobileTouchMove').on('touchmove', function (e) {" +
                        " debugger; " +
                        "e.preventDefault();" +
                        "if(!$('#pageAbstract')){return;}" +
                        "var pageWidth = $('#pageAbstract').width();var pageHeight = $('#pageAbstract').height()-112; " +
                        "if (e.originalEvent.targetTouches.length == 1) { " +
                            "var touch = e.originalEvent.targetTouches[0]; " +
                            "if(touch.pageX - 50 < 0 || touch.pageX > pageWidth){return;}" +
                            "if(touch.pageY - 46 < 0 || touch.pageY + 4 > pageHeight){return;}" +
                            "$('#roMobileTouchMoveMain').css({'bottom':(pageHeight-touch.pageY+40) + 'px', 'left':(touch.pageX+30) + 'px'});" +
                         "}" +
                     " });";
            }
            //下面一句是保证影像重扫在我的请求与我的已办中无法发起重扫的逻辑。
            html += "var locaHref=window.location.href.toLowerCase();if(locaHref.indexOf(\"myrequestphone\")>-1||locaHref.indexOf(\"ApprovalCenterComptTaskPhone\")>-1){var yxbtn=$('a.btn-float[onclick*=\"影像\"]');if(yxbtn.length){var yxonc=yxbtn.attr(\"onclick\");yxonc=yxonc.replace(\"&evaluation=1\",\"\");yxbtn.attr(\"attr\",yxonc)}};";
            html += "</script>";
            html += "  </body>";
            html += "  </html>";
            //MyHelperProvider.WriteLogToText(LogName, "移动审批影像接口html222:" + html);
            return html;
        }
        public string setOnePlus(ref string html, DataSet ds,string billID)
        {
            Random rd = new Random();
            int ird = rd.Next();
            html += "<html lang=\"zh\">";
            html += "<head>";
            html += "  <link rel=\"stylesheet\" type=\"text/css\" href=\"/cwbase/webapp/Docs/css/romobilestyles.css?v=" + ird + "\">";
            html += "  </head>";
            html += "  <body>";
            html += "  <section id=\"roMobileTouchMoveMain\" style=\"position: fixed;right: 0px;bottom: 25px \" class=\"model-4\">";
            html += "   <div class=\"float-btn-group\">";
            html += "<button  id =\"roMobileTouchMove\" class=\"btn-float btn-triger blue\"><i class=\"icon-bars\"></i></button>";
            html += "<div class=\"btn-list\">";
            foreach (DataRow drItem in ds.Tables[0].Rows)
            {
                //只组织可见的
                string customText = drItem["LSCONF_BtnCustomText"].ToString();
                string btnID = drItem["LSCONF_BtnKEY"].ToString();
                string btnUrl = drItem["LSCONF_URL"].ToString().Replace("@NM@",billID);
                string btnText = drItem["LSCONF_BtnText"].ToString();
                if (!string.IsNullOrEmpty(customText))
                {
                    btnText = customText;
                }
                html += string.Format("<a href=\"#\" class=\"btn-float blue\" onclick=\"window.top.gsp.rtf.func.openUrl({{id:'{0}',name:'{1}', url:'{2}'}})\">{3}</a>", btnID, btnText, btnUrl, btnText);
            }
            html += "</div>";
            html += " </div>";
            html += "  </section>";
            html += "  <script>";
            html += "  debugger;";
            html += "  $('.btn-triger').click(function () {";
            html += "      $(this).closest('.float-btn-group').toggleClass('open');";
            html += "      var isOpen = $('#roMobileTouchMove').parent().hasClass('open');";
            html += "      localStorage.setItem('btnIsOpen', isOpen);";
            html += "  });";
            //下面一句是保证影像重扫在我的请求与我的已办中无法发起重扫的逻辑。
            html += "var locaHref=window.location.href.toLowerCase();if(locaHref.indexOf(\"myrequestphone\")>-1||locaHref.indexOf(\"ApprovalCenterComptTaskPhone\")>-1){var yxbtn=$('a.btn-float[onclick*=\"影像\"]');if(yxbtn.length){var yxonc=yxbtn.attr(\"onclick\");yxonc=yxonc.replace(\"&evaluation=1\",\"\");yxbtn.attr(\"attr\",yxonc)}};";
            string roBtnOpenCtrl = "1";
            switch (roBtnOpenCtrl)
            {
                case "1":
                    html += "setTimeout(function(){$('.btn-triger').click();},500);";
                    break;
                case "2":
                    html += "var btnIsOpen = localStorage.getItem('btnIsOpen')=='true' ;";
                    html += "if(btnIsOpen){setTimeout(function(){$('.btn-triger').click();},500);};";
                    break;
            }
            if (MobileQueryManager.GetConfValue("ROMOBILE_YDSPZCTD", "ROMOB"))//移动审批支持拖动
            {
                html += " document.getElementById(\"roMobileTouchMove\").addEventListener(\"touchmove\", function (evt) {  }, false); " +
                    "$('#roMobileTouchMove').on('touchmove', function (e) {" +
                        " debugger; " +
                        "e.preventDefault();" +
                        "if(!$('#pageAbstract')){return;}" +
                        "var pageWidth = $('#pageAbstract').width();var pageHeight = $('#pageAbstract').height()-112; " +
                        "if (e.originalEvent.targetTouches.length == 1) { " +
                            "var touch = e.originalEvent.targetTouches[0]; " +
                            "if(touch.pageX - 50 < 0 || touch.pageX > pageWidth){return;}" +
                            "if(touch.pageY - 46 < 0 || touch.pageY+4 > pageHeight){return;}" +
                            "$('#roMobileTouchMoveMain').css({'bottom':(pageHeight-touch.pageY+25) + 'px', 'left':(touch.pageX-25) + 'px' });" +
                         "}" +
                     " });";
            }
            html += "  </script>";
            html += "  </body>";
            html += "  </html>";

            return html;
        }

        public DataSet GetBizResult(string funcID, Dictionary<string, string> bizParameter, DataSet bizData)
        {
            throw new NotImplementedException();
        }

        public DataSet GetBizResultWithFormatID(string funcID, Dictionary<string, string> bizParameter, DataSet bizData, ref string sysCode, ref string formatID)
        {
            throw new NotImplementedException();
        }
    }
}