using Learun.Application.TwoDevelopment.SYS_Code;
using Learun.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Learun.Application.Web.API
{
    /// <summary>
    /// 附件相关
    /// </summary>
    [RoutePrefix("rest/file")]
    [ApiAuthorize(FilterMode.Ignore)]//可以不登录
    public class FileController : BaseApi
    {
        /// <summary>
        /// 通过编码获得附件编码详情
        /// </summary>
        /// <returns></returns>
        [Route("accOperationDetail"), HttpGet]
        public ResParameter AccOperationList(string OperationCode = "")
        {
            try
            {
                if (OperationCode.IsEmpty())
                {
                    throw new Exception("参数非法");
                }
                int intTotalCount = 0;
                Sys_AccOperationBLL bllA = new Sys_AccOperationBLL();
                var accessories = bllA.GetListByID(OperationCode);
                intTotalCount = accessories.Count();
                var jsonData = new
                {
                    rows = accessories,
                    total = 1,
                    page = 1,
                    records = intTotalCount
                };
                return Success(jsonData);
            }
            catch (Exception ex)
            {
                return Fail(ex.Message);
            }
        }

        /// <summary>
        /// 业务附件
        /// </summary>
        /// <returns></returns>
        [Route("accessoriesList"), HttpGet]
        public ResParameter AccessoriesList(string OperationCode = "", string OperationID = "")
        {
            try
            {
                if (OperationCode.IsEmpty() || OperationID.IsEmpty())
                {
                    throw new Exception("参数必填");
                }
                int intTotalCount = 0;
                Sys_AccessoriesBLL bllA = new Sys_AccessoriesBLL();
                var accessories = bllA.GetList(new { OperationCode, OperationID }.ToJson());
                intTotalCount = accessories.Count();
                var jsonData = new
                {
                    rows = from t in accessories
                           select new
                           {
                               AttachmentID = t.ID,
                               t.PhyFileName,
                               t.SysFileName,
                               t.FileType,
                               t.FileSize,
                               t.FilePath,
                               HttpPath = t.getHttpPath()
                           },
                    total = 1,
                    page = 1,
                    records = intTotalCount
                };
                return Success(jsonData);
            }
            catch (Exception ex)
            {
                return Fail(ex.Message);
            }
        }

        /// <summary>
        /// 附件上传
        /// </summary>
        /// <returns></returns>
        [Route("upload"), HttpPost]
        public ResParameter Upload()
        {
            try
            {
                var user = LoginUser;
                var queryParam = requestParam;
                var file = httpContext.Request.Files[0];
                string OperationCode = queryParam["OperationCode"].ToString();
                if (OperationCode.IsEmpty())
                {
                    throw new Exception("缺少附件编码");
                }
                //通过AttachmentType获取附近注册后的数据
                Sys_AccOperationBLL BLL_Acc = new Sys_AccOperationBLL();
                Sys_AccOperationEntity AccInfo = BLL_Acc.GetEntity(OperationCode);
                if (AccInfo == null)
                {
                    throw new Exception("编码无效");
                }
                //判断文件类型，与指定的文件类型是否一样
                string extension = System.IO.Path.GetExtension(file.FileName).ToUpper();
                if (string.IsNullOrEmpty(extension) || !AccInfo.FileType.ToUpper().Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Contains(extension.TrimStart('.')))
                {
                    throw new Exception("不支持的文件类型");
                }
                string newFile = Guid.NewGuid().ToString();
                Sys_AccessoriesBLL BLL_Accs = new Sys_AccessoriesBLL();
                Sys_AccessoriesEntity AccsInfo = new Sys_AccessoriesEntity();
                AccsInfo.ID = newFile;
                AccsInfo.PhyFileName = newFile + extension;
                AccsInfo.SysFileName = file.FileName;
                AccsInfo.OperationCode = OperationCode;
                AccsInfo.OperationID = "";
                AccsInfo.FileSize = file.ContentLength;
                AccsInfo.FileType = extension.TrimStart('.');
                AccsInfo.SavePath = AccInfo.SavePath;
                AccsInfo.Remark = "1";
                AccsInfo.CreateTime = DateTime.Now;
                AccsInfo.CreatePerson = user == null ? "anonymous" : user.userId;
                string strFileFullPath = System.Web.Hosting.HostingEnvironment.MapPath(AccsInfo.SavePath);
                //检查是否有该路径没有就创建
                if (!Directory.Exists(strFileFullPath))
                    Directory.CreateDirectory(strFileFullPath);
                //保存文件 到Redis
                BLL_Accs.SaveChunkAnnexes(newFile, file.InputStream, AccsInfo);
                var jsonData = new[] {
                    new {
                     AttachmentID = newFile,
                        AccsInfo.PhyFileName,
                        AccsInfo.SysFileName,
                        AccsInfo.FileType,
                        AccsInfo.FilePath,
                        AccsInfo.FileSize,
                        HttpPath=AccsInfo.getHttpPath()
                    }
                };
                return Success(jsonData);
            }
            catch (Exception ex)
            {
                return Fail(ex.Message);
            }
        }

        /// <summary>
        /// 附件业务关联
        /// </summary>
        /// <returns></returns>
        [Route("updateAttachments"), HttpPost]
        public ResParameter UpdateAttachments(string OperationCode = "", string OperationID = "", string AttachmentID = "")
        {
            try
            {
                var user = LoginUser;
                if (OperationCode.IsEmpty() || OperationID.IsEmpty())
                {
                    throw new Exception("缺少参数");
                }
                Sys_AccessoriesBLL bllA = new Sys_AccessoriesBLL();
                bllA.BindAttachment(user == null ? "anonymous" : user.userId, OperationCode, OperationID, AttachmentID);
                return Success("成功");
            }
            catch (Exception ex)
            {
                return Fail(ex.Message);
            }
        }
    }
}
