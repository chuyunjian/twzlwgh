using Learun.Application.Base.SystemModule;
using Learun.Util;
using Learun.Util.Operat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.IO;
using System.Data;
using Learun.Application.TwoDevelopment.SYS_Code;
using System.Security.Cryptography;

namespace Learun.Application.Web.Controllers
{
    [HandlerLogin(FilterMode.Ignore)]
    public class apiMD5Controller : MvcControllerBase
    {

        #region 入口
        [ValidateInput(false)]
        /// <summary>
        /// 接口
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public JsonResult Index(string parameters)
        {
            JsonResult returnJson = new JsonResult();
            try
            {
                if (string.IsNullOrEmpty(parameters))
                {
                    throw new Exception("parameters为空");
                }
                Dictionary<string, string> param = AnalysisParam(parameters);
                string Function = param["Function"];
                switch (Function)
                {
                    default:
                        throw new Exception("不存在此方法");

                    case "UpdateVersion"://更新版本
                        returnJson = UpdateVersion(param);
                        break;

                    #region 附件相关
                    case "AccOperationList":     //根据相关编码ID获取该编码信息
                        returnJson = AccOperationList(param);
                        break;
                    case "UploadFile"://上传文件
                        returnJson = UploadFile(param);
                        break;
                    case "AccessoriesList":     //根据相关编码ID获取该编码所有附件
                        returnJson = AccessoriesList(param);
                        break;
                    case "UpdateAttachments"://更新业务相关附件信息
                        returnJson = UpdateAttachments(param);
                        break;
                    #endregion
                }
            }
            catch (Exception ex)
            {
                returnJson = Json(new { result = "0", message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
            finally
            {

                var logger = this.Logger;
                //记录日志
                LogEntity logEntity = new LogEntity();
                logEntity.F_CategoryId = 3;
                logEntity.F_OperateTypeId = ((int)OperationType.Exception).ToString();
                logEntity.F_OperateType = EnumAttribute.GetDescription(OperationType.Exception);
                logEntity.F_OperateAccount = "system";
                logEntity.F_OperateUserId = "system";
                logEntity.F_Module = "API";
                logEntity.F_SourceContentJson = Request.Url.ToString();
                logEntity.F_ExecuteResult = 1;
                logEntity.F_ExecuteResultJson = Newtonsoft.Json.JsonConvert.SerializeObject(returnJson.Data);
                LogBLL.WriteLog(logEntity);
            }
            return returnJson;
        }



        #endregion

        #region 其他私有方法
        private Dictionary<string, string> AnalysisParam(string parameters)
        {
            string[] Params = parameters.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
            Dictionary<string, string> ListParam = new Dictionary<string, string>();
            foreach (string item in Params)
            {
                string[] value = item.Split('=');
                if (item.IndexOf('=') > 0)
                {


                    if (!ListParam.ContainsKey(value[0]))
                    {
                        if (value.Length == 2)
                        {
                            ListParam.Add(value[0], value[1]);
                        }
                        else
                        {
                            //多个=号问题
                            ListParam.Add(value[0], item.Replace(value[0] + "=", ""));
                        }

                    }


                }
                else
                    throw new Exception("parameters error！");



            }
            //判断有没有Function参数
            if (!ListParam.Keys.Contains("Function"))
                throw new Exception("missing the specified parameters:Function");
            return ListParam;
        }

        private Dictionary<string, string> VerifyParam(Dictionary<string, string> AnalysisParam, string SpecifiedParam)
        {
            //删除Function参数
            AnalysisParam.Remove("Function");
            return AnalysisParam;
        }

        /// <summary>
        /// 与ASP兼容的MD5加密算法
        /// </summary>
        private string GetMD5(string s)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] t = md5.ComputeHash(Encoding.UTF8.GetBytes(s));
            StringBuilder sb = new StringBuilder(32);
            for (int i = 0; i < t.Length; i++)
            {
                sb.Append(t[i].ToString("x").PadLeft(2, '0'));
            }
            return sb.ToString();
        }

        /// <summary>
        /// 验证数据是否篡改
        /// </summary>
        /// <param name="Value">明文</param>
        /// <param name="MD">Hash签名</param>
        /// <param name="SMD">加密后数据</param>
        /// <returns></returns>
        private bool CheckData(string Value, string SMD)
        {
            bool flag = false;
            string MerKey = Config.GetValue("SecurityKey");//密钥
            if (MerKey == null)
            {
                MerKey = "";
            }
            if (string.IsNullOrEmpty(SMD))
                SMD = string.Empty;

            string outMD = string.Empty;
            outMD = GetMD5(Value + MerKey).ToUpper();
            if (SMD.ToUpper() == outMD)
                flag = true;
            else
                flag = false;
            return flag;
        }
        #endregion

        /*******************实现业务方法************************/
        #region 更新版本
        //更新版本
        private JsonResult UpdateVersion(Dictionary<string, string> parameters)
        {
            Dictionary<string, string> param = VerifyParam(parameters, "Type,MD5");
            //验证签名
            if (!CheckData(param["Type"], param["MD5"]))
            {
                return Json(new { result = "0", message = "非法访问" }, JsonRequestBehavior.AllowGet);
            }
            JsonResult retrunJson = new JsonResult();

            APPIBLL appIBLL = new APPBLL();
            APPEntity AppModel = appIBLL.GetLastVersion(param["Type"]);
            if (AppModel == null)
                return Json(new { result = "2", message = "temporarily not available for download" }, JsonRequestBehavior.AllowGet);
            //是否强制更新
            bool AppIsMustUpdate = Convert.ToBoolean(Config.GetValue("AppIsMustUpdate"));
            //获取上传的版本
            Sys_AccessoriesBLL BLL_Access = new Sys_AccessoriesBLL();
            List<Sys_AccessoriesEntity> ListAccess = BLL_Access.GetModelListByOperationCode("Version", AppModel.AGuid);
            if (ListAccess.Count > 0)
                AppModel.FileUrl = Learun.Util.Config.GetValue("AdminUrl") + ListAccess[0].SavePath + ListAccess[0].PhyFileName;
            else
                AppModel.FileUrl = string.Empty;

            retrunJson = Json(new { result = "1", message = "succeed", list = new[] { new { DownLoadURL = AppModel.FileUrl, UpadatContent = AppModel.ARemark, VersionCode = AppModel.Version, AppIsMustUpdate = AppIsMustUpdate } } }, JsonRequestBehavior.AllowGet);
            return retrunJson;
        }
        #endregion

        #region 通过编码获得附件编码信息
        private JsonResult AccOperationList(Dictionary<string, string> parameters)
        {
            Dictionary<string, string> param = VerifyParam(parameters, "UserID,OperationCode,MD5");
            //验证签名
            if (!CheckData(param["UserID"] + param["OperationCode"], param["MD5"]))
                return Json(new { result = "0", message = "非法访问" }, JsonRequestBehavior.AllowGet);
            try
            {
                string OperationCode = param["OperationCode"];
                int intTotalCount = 0;
                Sys_AccOperationBLL bllA = new Sys_AccOperationBLL();
                var accessories = bllA.GetListByID(OperationCode);
                intTotalCount = accessories.Count();
                var datas = accessories;
                return Json(new { result = "1", message = "成功", totalcount = intTotalCount, list = datas }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { result = "4", message = e.Message, list = DBNull.Value }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region 上传文件
        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        private JsonResult UploadFile(Dictionary<string, string> parameters)
        {
            try
            {
                if (Request.HttpMethod != "POST")
                {
                    return Json(new { result = "0", message = "只支持POST请求" }, JsonRequestBehavior.AllowGet);
                }
                Dictionary<string, string> param = VerifyParam(parameters, "UserID,OperationCode,MD5");
                //验证签名
                if (!CheckData(param["UserID"] + param["OperationCode"], param["MD5"]))
                {
                    return Json(new { result = "0", message = "非法访问" }, JsonRequestBehavior.AllowGet);
                }
                JsonResult retrunJson = new JsonResult();
                if (Request.Files.Count <= 0)
                {
                    return Json(new { result = "0", message = "没有文件上传" }, JsonRequestBehavior.AllowGet);
                }

                var file = Request.Files[0];
                string OperationCode = param["OperationCode"];
                //通过AttachmentType获取附近注册后的数据
                Sys_AccOperationBLL BLL_Acc = new Sys_AccOperationBLL();
                Sys_AccOperationEntity AccInfo = BLL_Acc.GetEntity(OperationCode);
                if (AccInfo == null)
                    return Json(new { result = "0", message = "MediaType无效" }, JsonRequestBehavior.AllowGet);

                //判断文件类型，与指定的文件类型是否一样
                string extension = System.IO.Path.GetExtension(file.FileName).ToUpper();
                if (string.IsNullOrEmpty(extension) || !AccInfo.FileType.ToUpper().Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Contains(extension.TrimStart('.')))
                    return Json(new { result = "0", message = "不支持的文件类型" }, JsonRequestBehavior.AllowGet);

                string newFile = Guid.NewGuid().ToString();
                Sys_AccessoriesBLL BLL_Accs = new Sys_AccessoriesBLL();
                Sys_AccessoriesEntity AccsInfo = new Sys_AccessoriesEntity();
                AccsInfo.ID = newFile;
                AccsInfo.PhyFileName = newFile + extension;
                AccsInfo.SysFileName = file.FileName;
                AccsInfo.OperationCode = param["OperationCode"];
                AccsInfo.OperationID = "";
                AccsInfo.FileSize = file.ContentLength;
                AccsInfo.FileType = extension.TrimStart('.');
                AccsInfo.SavePath = AccInfo.SavePath;
                AccsInfo.Remark = "1";
                AccsInfo.CreateTime = DateTime.Now;
                AccsInfo.CreatePerson = param["UserID"];
                string strFileFullPath = Server.MapPath(AccsInfo.SavePath);
                //检查是否有该路径没有就创建
                if (!Directory.Exists(strFileFullPath))
                    Directory.CreateDirectory(strFileFullPath);
                //保存文件 到Redis
                BLL_Accs.SaveChunkAnnexes(newFile, file.InputStream, AccsInfo);

                retrunJson = Json(new
                {
                    result = "1",
                    message = "上传成功",
                    list = new[] { new {
                     AttachmentID = newFile,
                        AccsInfo.PhyFileName,
                        AccsInfo.SysFileName,
                        AccsInfo.FileType,
                        AccsInfo.FilePath,
                        AccsInfo.FileSize,
                        HttpPath=AccsInfo.getHttpPath()
                } }
                }, JsonRequestBehavior.AllowGet);
                //解决跨域问题
                HttpContext.Response.AddHeader("Access-Control-Allow-Origin", "*");
                return retrunJson;
            }
            catch (Exception e)
            {
                return Json(new { result = "4", message = e.Message, list = DBNull.Value }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region 通过编码获得附件集合
        private JsonResult AccessoriesList(Dictionary<string, string> parameters)
        {
            Dictionary<string, string> param = VerifyParam(parameters, "UserID,OperationCode,OperationID,MD5");
            //验证签名
            if (!CheckData(param["UserID"] + param["OperationCode"] + param["OperationID"], param["MD5"]))
                return Json(new { result = "0", message = "非法访问" }, JsonRequestBehavior.AllowGet);
            try
            {
                string OperationCode = param["OperationCode"];
                string OperationID = param["OperationID"];
                int intTotalCount = 0;
                Sys_AccessoriesBLL bllA = new Sys_AccessoriesBLL();
                var accessories = bllA.GetList(new { OperationCode, OperationID }.ToJson());
                intTotalCount = accessories.Count();
                var datas = from t in accessories
                            select new
                            {
                                AttachmentID = t.ID,
                                t.PhyFileName,
                                t.SysFileName,
                                t.FileType,
                                t.FileSize,
                                t.FilePath,
                                HttpPath = t.getHttpPath()
                            };
                return Json(new { result = "1", message = "成功", totalcount = intTotalCount, list = datas }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { result = "4", message = e.Message, list = DBNull.Value }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region 将业务关联附件
        private JsonResult UpdateAttachments(Dictionary<string, string> parameters)
        {
            Dictionary<string, string> param = VerifyParam(parameters, "UserID,OperationCode,OperationID,AttachmentID,MD5");
            //验证签名
            if (!CheckData(param["UserID"] + param["OperationCode"] + param["OperationID"] + param["AttachmentID"], param["MD5"]))
                return Json(new { result = "0", message = "非法访问" }, JsonRequestBehavior.AllowGet);
            try
            {
                string OperationCode = param["OperationCode"];
                string OperationID = param["OperationID"];
                string AttachmentID = param["AttachmentID"];
                BindAttachment(param["UserID"], OperationCode, OperationID, AttachmentID);
                return Json(new { result = "1", message = "成功！" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { result = "4", message = e.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        /// <summary>
        /// 将业务关联附件
        /// </summary>
        /// <param name="OperationCode">附加编码</param>
        /// <param name="OperationID">业务主键</param>
        /// <param name="AttachmentID">附件ID列表，以逗号分隔</param>
        private void BindAttachment(string UserID, string OperationCode, string OperationID, string AttachmentID)
        {
            Sys_AccessoriesBLL bllA = new Sys_AccessoriesBLL();
            bllA.BindAttachment(UserID, OperationCode, OperationID, AttachmentID);
        }
        #endregion
    }
}