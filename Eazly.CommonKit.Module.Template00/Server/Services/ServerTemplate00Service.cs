using Eazly.CommonKit.Module.Template00.Server.Services;
using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.VisualBasic;
using Newtonsoft.Json.Linq;
using Oqtane.Enums;
using Oqtane.Infrastructure;
using Oqtane.Models;
using Oqtane.Repository;
using Oqtane.Security;
using Oqtane.Shared;
using Org.BouncyCastle.Bcpg.OpenPgp;
using Radzen;
using Radzen.Blazor;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace Eazly.CommonKit.Module.Template00.Services
{
    public class ServerTemplate00Service : ITemplate00Service
    {
        private readonly IUserPermissions _userPermissions;
        private readonly ILogManager _logger;
        private readonly IHttpContextAccessor _accessor;
        private readonly Alias _alias;

        private readonly ITenantManager _tenantManager;
        private readonly ISqlRepository _sqlRepository;
        private readonly string _userID = string.Empty;
        private readonly int _tenantID;
        private readonly int _siteID;
        private string _EntityName = string.Empty;
        private string _TableName = string.Empty;

		private string[] ignoreList = { "_chk", "TId", "_rowState", "TenantId", "SiteId" };
		private string[] byList = { "CreatedBy", "ModifiedBy" };
		private string[] onList = { "CreatedOn", "ModifiedOn" };

		public ServerTemplate00Service(IUserPermissions userPermissions, ITenantManager tenantManager, ILogManager logger, IHttpContextAccessor accessor, ISqlRepository sqlRepository)
        {
            _userPermissions = userPermissions;
            _logger = logger;
            _accessor = accessor;
            _alias = tenantManager.GetAlias();

            _tenantManager = tenantManager;
            _sqlRepository = sqlRepository;

            if (_accessor != null && _accessor.HttpContext != null && _accessor.HttpContext.User != null)
                _userID = ((System.Security.Claims.ClaimsIdentity)_accessor.HttpContext.User.Identity).Name;
            _tenantID = _tenantManager.GetTenant().TenantId;
            _siteID = _alias.SiteId;
        }

        private string GetReplaceCreateQuery(string strQuery, int ModuleId, string queryID)
        {
            strQuery = strQuery.Replace("@EntityName", _EntityName);
            strQuery = strQuery.Replace("@TableName", queryID == "CreateTable" ? _TableName : '[' + _TableName + ']');
			strQuery = strQuery.Replace("@ModuleId", ModuleId.ToString());
			strQuery = strQuery.Replace("@QueryId", queryID);

            return strQuery;
        }

        private string GetReplaceExcuteQuery(string strQuery, int ModuleId, string queryID, string jsonParam = "")
        {
            strQuery = GetReplaceCreateQuery(strQuery, ModuleId, queryID);
            strQuery = strQuery.Replace("@TenantId", _tenantID.ToString());
            strQuery = strQuery.Replace("@SiteId", _siteID.ToString());
            strQuery = strQuery.Replace("@UserId", _userID);
            strQuery = strQuery.Replace("@JsonParam", jsonParam);

            return strQuery;
        }

        private void ExecuteScriptString(string strQuery)
        {
            try
            {
                _sqlRepository.ExecuteScript(_tenantManager.GetTenant(), strQuery);
            }
            catch (System.Exception)
            {
                _logger.Log(LogLevel.Error, this, LogFunction.Other, "ExecuteScriptString {strQuery}", strQuery);
            }

            return;
        }

        private bool ExecuteQueryString(string strQuery)
        {
            bool boolResult = false;
            int intReturn = -1;

            try
            {
                intReturn = _sqlRepository.ExecuteNonQuery(_tenantManager.GetTenant(), strQuery);
            }
            catch (System.Exception)
            {
                _logger.Log(LogLevel.Error, this, LogFunction.Other, "ExecuteQueryString {strQuery}", strQuery);
            }

            boolResult = intReturn > 0;

            return boolResult;
        }

        private DataTable GetSQLQueryDtaTable(string strQuery)
        {
            if (string.IsNullOrWhiteSpace(strQuery))
                return null;

            using (SqlDataReader reader = (SqlDataReader)_sqlRepository.ExecuteReader(_tenantManager.GetTenant(), strQuery))
            {
                DataTable dt = null;
                if (reader.VisibleFieldCount > 0)
                {
                    dt = new DataTable();
                    dt.Load(reader);
                }

                return dt;
            }
        }
        private string GetSQLQueryString(string strQuery)
        {
            string strResult = string.Empty;

            DataTable dt = GetSQLQueryDtaTable(strQuery);

            if (dt != null && dt.Rows.Count > 0 && dt.Columns.Count > 0)
                strResult = dt.Rows[0][0].ToString();

            return strResult;
        }
		private void CreateConfigTable(int ModuleId, string queryID)
		{
			if (!Debugger.IsAttached)
				return;

			string strCreateProcedure = ServerTemplateResources.TCreateConfigTable ?? string.Empty;

			strCreateProcedure = GetReplaceExcuteQuery(strCreateProcedure, ModuleId, queryID, string.Empty);

			ExecuteScriptString(strCreateProcedure);
		}

		private void CreateTableNProcedure(int ModuleId, string queryID)
        {
            if (string.IsNullOrEmpty(_EntityName))
            {
                DataTable dtSetting = GetSQLQueryDtaTable(string.Format("SELECT * FROM dbo.UFN_Setting({0}) A ", ModuleId.ToString()));

                if (dtSetting != null && dtSetting.Rows.Count > 0)
                {
                    _EntityName = dtSetting.Rows[0]["EntityName"].ToString();
                    _TableName = dtSetting.Rows[0]["TableName"].ToString();
                }
            }

            if (!Debugger.IsAttached)
                return;

            if (string.IsNullOrEmpty(_TableName)) return;

            string strCreateTable = ServerTemplateResources.TCreateTable ?? string.Empty;
            strCreateTable = GetReplaceCreateQuery(strCreateTable, ModuleId, "CreateTable");
            ExecuteScriptString(strCreateTable);

            string strCreateProcedure = string.Empty;
            if (queryID == "Condition")
                strCreateProcedure = ServerTemplateResources.TCreateProdecureCondition ?? string.Empty;
            else if (queryID == "Save")
            {
                strCreateProcedure = ServerTemplateResources.TCreateProdecureSaveContents ?? string.Empty;

                string strCRUD = string.Empty;
                if (!string.IsNullOrEmpty(_TableName))
                {
                    DataTable dtTable = GetSQLQueryDtaTable(string.Format("SELECT TOP 1 * FROM [{0}] A WHERE 1 <> 1", _TableName));

                    string strOpenjsonTable = string.Empty;
                    string strInsertColumnList = string.Empty;
                    string strUpdateList = string.Empty;

                    foreach (DataColumn dataColumn in dtTable.Columns)
                    {
                        string strDataType = string.Empty;
                        switch (dataColumn.DataType)
                        {
                            case System.Type _ when dataColumn.DataType == typeof(int):
                                strDataType = "INT";
                                break;
                            case System.Type _ when dataColumn.DataType == typeof(string):
                                strDataType = "NVARCHAR(MAX)";
                                break;
                            case System.Type _ when dataColumn.DataType == typeof(DateTime):
                                strDataType = "DATETIME2";
                                break;
                            case System.Type _ when dataColumn.DataType == typeof(bool):
                                strDataType = "BIT";
                                break;
                            case System.Type _ when dataColumn.DataType == typeof(decimal):
                                strDataType = "DECIMAL(18, 2)";
                                break;
                            default:
                                strDataType = "NVARCHARMAX)";
                                break;
                        }

                        if ("TenantId,SiteId,CreatedBy,CreatedOn,ModifiedBy,ModifiedOn,".Contains(dataColumn.ColumnName + ","))
                            continue;

                        strOpenjsonTable += string.Format("\t\t\t[{0}] {1},\r\n", dataColumn.ColumnName, strDataType);

                        if ("TId,".Contains(dataColumn.ColumnName + ","))
                            continue;

                        strInsertColumnList += string.Format(@"B.[{0}], ", dataColumn.ColumnName);
                        strUpdateList += string.Format("\r\n\t\t\t\t\t[{0}] = B.[{0}],", dataColumn.ColumnName);
                    }

                    strOpenjsonTable = string.Format(@"OPENJSON(@JsonParam)
	WITH (
{0}			_rowState NVARCHAR(50)
		)", strOpenjsonTable);

                    strInsertColumnList += "@TenantId, @SiteId, @UserId, GETDATE(), @userId, GETDATE()";

                    strUpdateList += "\r\n\t\t\t\t\tModifiedBy = @userId, ModifiedOn = GETDATE()";

                    strCRUD = string.Format(@"
	INSERT INTO [{0}]
	SELECT	{1}
	  FROM {2} B
	LEFT JOIN [{0}] A ON (B.TId = A.TId)
	WHERE B._rowState IN ('Insert', 'Update')
	  AND A.TId IS NULL

	UPDATE A SET {3}
	  FROM {2} B
	JOIN [{0}] A ON (B.TId = A.TId)
	WHERE B._rowState IN ('Insert', 'Update')

	DELETE A
	  FROM {2} B
	JOIN [{0}] A ON (B.TId = A.TId)
	WHERE B._rowState IN ('Delete')
", _TableName, strInsertColumnList, strOpenjsonTable, strUpdateList);
                }

                strCRUD = strCRUD.Replace("'", "''");
                strCreateProcedure = strCreateProcedure.Replace("-- @CRUD", strCRUD);
            }
            else
                strCreateProcedure = ServerTemplateResources.TCreateProdecureGetContents ?? string.Empty;

            strCreateProcedure = GetReplaceCreateQuery(strCreateProcedure, ModuleId, queryID);
            ExecuteScriptString(strCreateProcedure);
        }

        private void SetDefaultConfigContentColumns(int ModuleId, string queryID, DataTable dtColumns)
        {
		string strSQL = @"
INSERT INTO [Eazly.ConfigContentsColumns](ModuleId, QueryID, ColumnName, ColumnCaption, [IsPrimary], [IsEditable], [IsRequired], [IsVisible], [DefaultValue], [DataFormat], [Width], TenantId, SiteId, [CreatedBy], [CreatedOn], [ModifiedBy], [ModifiedOn])
SELECT ModuleId, QueryID, ColumnName, ColumnCaption, [IsPrimary], [IsEditable], [IsRequired], [IsVisible], [DefaultValue], [DataFormat], [Width], TenantId, SiteId, [CreatedBy], [CreatedOn], [ModifiedBy], [ModifiedOn] 
  FROM (" + Environment.NewLine;

            string strColumnList = @"SELECT @ModuleId ModuleId, '@QueryId' QueryID, '@ColumnName' ColumnName, '@ColumnCaption' ColumnCaption, 0 [IsPrimary], '@IsEditable' [IsEditable], '@IsRequired' [IsRequired], 1 [IsVisible], '' [DefaultValue], '@DataFormat' [DataFormat], 0 [Width], @TenantId TenantId, @SiteId SiteId, '@UserId' [CreatedBy], GETDATE() [CreatedOn], '@UserId' [ModifiedBy], GETDATE() [ModifiedOn]" + Environment.NewLine;

			string dateFormat = string.Empty;


            bool IsFirst = true;
			foreach (DataColumn dataColumn in dtColumns.Columns)
            {
                if (ignoreList.Contains(dataColumn.ColumnName)) continue;

				if (!onList.Contains(dataColumn.ColumnName) && dataColumn.DataType == typeof(DateTime))
                    dateFormat = "yyyy-MM-dd";
				else if (dataColumn.DataType == typeof(int) || dataColumn.DataType == typeof(decimal) || dataColumn.DataType == typeof(double))
					dateFormat = "#,##0";
                else
                    dateFormat = string.Empty;

				strSQL += (IsFirst ? " " : "UNION ALL" + Environment.NewLine)+
				GetReplaceExcuteQuery(strColumnList, ModuleId, queryID)
                    .Replace("@ColumnName", dataColumn.ColumnName)
                    .Replace("@ColumnCaption", dataColumn.Caption)
					.Replace("@IsEditable", byList.Contains(dataColumn.ColumnName) || onList.Contains(dataColumn.ColumnName) ? "0" : "1")
					.Replace("@IsRequired", byList.Contains(dataColumn.ColumnName) || onList.Contains(dataColumn.ColumnName) || dataColumn.AllowDBNull ? "0" : "1")
                    .Replace("@DataFormat", dateFormat);

				IsFirst = false;
			}

            strSQL += @"        ) A
WHERE NOT EXISTS(   SELECT 1 FROM [Eazly.ConfigContentsColumns] WITH(NOLOCK)
                    WHERE ModuleId = A.ModuleId AND QueryID = A.QueryID AND ColumnName = A.ColumnName
                )";   

            ExecuteScriptString(strSQL);
		}

		private void SetConfigContentColumns(int ModuleId, string queryID, DataTable dtColumns)
		{
			string strQuery = ServerTemplateResources.TGetConfigContentsColumns ?? string.Empty;
            strQuery = GetReplaceExcuteQuery(strQuery, ModuleId, queryID);
            DataTable dt = GetSQLQueryDtaTable(strQuery);

            DataRow[] dataRows = null;
            foreach (DataColumn dataColumn in dtColumns.Columns)
            {
				if (ignoreList.Contains(dataColumn.ColumnName)) continue;

				dataRows = dt.Select(string.Format("ColumnName = '{0}'", dataColumn.ColumnName));
                if (dataRows.Length == 0) continue;

                dataColumn.Caption = dataRows[0]["ColumnCaption"].ToString();
                dataColumn.ReadOnly = !dataRows[0]["IsEditable"].ToString().ToUpper().Equals("TRUE");
                dataColumn.AllowDBNull = !dataRows[0]["IsRequired"].ToString().ToUpper().Equals("TRUE") && !dataRows[0]["IsPrimary"].ToString().ToUpper().Equals("TRUE");
                switch (dataColumn.DataType)
                {
					case System.Type _ when dataColumn.DataType == typeof(string):
                        dataColumn.DefaultValue = dataRows[0]["DefaultValue"].ToString();
						break;
					case System.Type _ when dataColumn.DataType == typeof(decimal):
                        if (decimal.TryParse(dataRows[0]["DefaultValue"].ToString(), out decimal decValue))
                            dataColumn.DefaultValue = decValue;
                        else
                            dataColumn.DefaultValue = 0;
						break;
					case System.Type _ when dataColumn.DataType == typeof(DateTime):
						if (dataRows[0]["DefaultValue"].ToString().Equals("Today", StringComparison.InvariantCultureIgnoreCase))
							dataColumn.DefaultValue = DateTime.Today;
						else if (dataRows[0]["DefaultValue"].ToString().Equals("Now", StringComparison.InvariantCultureIgnoreCase))
                            dataColumn.DefaultValue = DateTime.Now;
						else if (DateTime.TryParse(dataRows[0]["DefaultValue"].ToString(), out DateTime dateValue))
                            dataColumn.DefaultValue = dateValue;
						else
							dataColumn.DefaultValue = null;
                        break;
					case System.Type _ when dataColumn.DataType == typeof(bool):
						dataColumn.DefaultValue = dataRows[0]["DefaultValue"].ToString().ToUpper().Equals("TRUE");
						break;
					default:
                        break;
                }
				dataColumn.ExtendedProperties["IsPrimary"] = dataRows[0]["IsPrimary"].ToString();
				dataColumn.ExtendedProperties["IsVisible"] = dataRows[0]["IsVisible"].ToString();
				dataColumn.ExtendedProperties["DataFormat"] = dataRows[0]["DataFormat"].ToString();
				dataColumn.ExtendedProperties["Width"] = dataRows[0]["Width"];
			}
        }

        public Task<DataTable> GetContentsIDAsync(int ModuleId, string queryID, string jsonParam)
        {
            if (!_userPermissions.IsAuthorized(_accessor.HttpContext.User, _alias.SiteId, EntityNames.Module, ModuleId, PermissionNames.View))
            {
                _logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized Get Attempt {ModuleId}", ModuleId);
                return null;
            }

            if (string.IsNullOrWhiteSpace(queryID))
                queryID = "Query";

            if (string.IsNullOrWhiteSpace(jsonParam))
				jsonParam = "[]";

            DataTable dt = null;

			if (!String.IsNullOrEmpty(_EntityName))
            {
                CreateTableNProcedure(ModuleId, queryID);

                string strQuery = ServerTemplateResources.TGetContentsID ?? string.Empty;
                strQuery = GetReplaceExcuteQuery(strQuery, ModuleId, queryID, jsonParam);
                dt = GetSQLQueryDtaTable(strQuery);
                DataColumn _chkCol = new DataColumn("_chk", typeof(bool));
                _chkCol.DefaultValue = false; // 모든 기존 행에 false가 들어감
                dt.Columns.Add(_chkCol);
                dt.Columns.Add("_rowState", typeof(string));

                dt.Columns.Remove("TenantId");
                dt.Columns.Remove("SiteId");

                SetDefaultConfigContentColumns(ModuleId, queryID, dt);
                SetConfigContentColumns(ModuleId, queryID, dt);

            }

			return Task.FromResult(dt);
		}

        public Task ExecuteQueryIDAsync(int ModuleId, string queryID, string jsonParam)
        {
            if (!_userPermissions.IsAuthorized(_accessor.HttpContext.User, _alias.SiteId, EntityNames.Module, ModuleId, PermissionNames.Edit))
            {
                _logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized Get Attempt {ModuleId}", ModuleId);
                return Task.CompletedTask;
            }

            if (string.IsNullOrWhiteSpace(queryID))
                queryID = "Save";
            if (string.IsNullOrWhiteSpace(jsonParam))
				jsonParam = "[]";

            CreateTableNProcedure(ModuleId, queryID);

            string strQuery = ServerTemplateResources.TExecuteQueryID ?? string.Empty;
            strQuery = GetReplaceExcuteQuery(strQuery, ModuleId, queryID, jsonParam);
            ExecuteQueryString(strQuery);   

            return Task.CompletedTask;
        }

        public Task<Models.Contition> GetConditionAsync(int ModuleId)
        {
            if (!_userPermissions.IsAuthorized(_accessor.HttpContext.User, _alias.SiteId, EntityNames.Module, ModuleId, PermissionNames.View))
            {
                _logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized Get Attempt {ModuleId}", ModuleId);
                return null;
            }

            CreateTableNProcedure(ModuleId, "Condition");

            Models.Contition condition = new()
            {
                DateFrom = System.DateTime.Now.AddDays(-7).Date,
                DateTo = System.DateTime.Now.Date,
                SearchValue = string.Empty
            };

            if (!String.IsNullOrEmpty(_EntityName))
            {
                string strQuery = ServerTemplateResources.TGetCondition ?? string.Empty;

                strQuery = strQuery.Replace("@EntityName", _EntityName);
                strQuery = strQuery.Replace("@ModuleId", ModuleId.ToString());

                DataTable dt = GetSQLQueryDtaTable(strQuery);

                if (dt != null && dt.Rows.Count > 0)
                {
                    condition.DateFrom = dt.Rows[0]["DateFrom"] != DBNull.Value ? Convert.ToDateTime(dt.Rows[0]["DateFrom"]) : condition.DateFrom;
                    condition.DateTo = dt.Rows[0]["DateTo"] != DBNull.Value ? Convert.ToDateTime(dt.Rows[0]["DateTo"]) : condition.DateTo;
                    condition.SearchValue = dt.Rows[0]["SearchValue"] != DBNull.Value ? dt.Rows[0]["SearchValue"].ToString() : condition.SearchValue;
                }
            }

            return Task.FromResult(condition);
        }

		public Task<Models.ContentsConifg> GetConfigContentsAsync(int ModuleId, string queryID)
		{
			if (!_userPermissions.IsAuthorized(_accessor.HttpContext.User, _alias.SiteId, EntityNames.Module, ModuleId, PermissionNames.View))
			{
				_logger.Log(LogLevel.Error, this, LogFunction.Security, "Unauthorized Get Attempt {ModuleId}", ModuleId);
				return null;
			}

			if (string.IsNullOrWhiteSpace(queryID))
				queryID = "Query";

			CreateConfigTable(ModuleId, queryID);

			Models.ContentsConifg contentsConifg = new()
			{
                IsDisSave = false,
                IsDisCreate = false,
                IsDisUpdate = false,
                IsDisDelete = false,
                IsDisExport = false,
				IsByOn = false
			};

			string strQuery = ServerTemplateResources.TGetConfigContents ?? string.Empty;
			strQuery = GetReplaceExcuteQuery(strQuery, ModuleId, queryID, "");
			DataTable dt = GetSQLQueryDtaTable(strQuery);

            if (dt != null && dt.Rows.Count > 0)
            {
                contentsConifg.IsDisCreate = dt.Rows[0]["IsCreate"] != DBNull.Value ? !(dt.Rows[0]["IsCreate"].ToString().ToUpper() == "TRUE") : contentsConifg.IsDisCreate;
                contentsConifg.IsDisUpdate = dt.Rows[0]["IsUpdate"] != DBNull.Value ? !(dt.Rows[0]["IsUpdate"].ToString().ToUpper() == "TRUE") : contentsConifg.IsDisUpdate;
                contentsConifg.IsDisDelete = dt.Rows[0]["IsDelete"] != DBNull.Value ? !(dt.Rows[0]["IsDelete"].ToString().ToUpper() == "TRUE") : contentsConifg.IsDisDelete;
                contentsConifg.IsDisExport = dt.Rows[0]["IsExport"] != DBNull.Value ? !(dt.Rows[0]["IsExport"].ToString().ToUpper() == "TRUE") : contentsConifg.IsDisExport;
				contentsConifg.IsByOn = dt.Rows[0]["IsByOn"] != DBNull.Value ? !(dt.Rows[0]["IsByOn"].ToString().ToUpper() == "TRUE") : contentsConifg.IsByOn;				

				contentsConifg.IsDisSave = contentsConifg.IsDisCreate && contentsConifg.IsDisUpdate && contentsConifg.IsDisDelete;
			}

			return Task.FromResult(contentsConifg);
		}
	}
}
 