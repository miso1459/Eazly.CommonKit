using Microsoft.Extensions.Logging;
using Radzen;
using Radzen.Blazor;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Eazly.CommonKit.Module.Template00.Shared.UI
{
	public class Template00UI
	{
		private string _userName = string.Empty;

		public string[] ignoreList = { "TId", "_rowState", "TenantId", "SiteId" };
		public string[] byList = { "CreatedBy", "ModifiedBy" };
		public string[] onList = { "CreatedOn", "ModifiedOn" };

		public Models.Contition _Condition;
		public Models.ContentsConifg _ContentsConifg;

		public DataTable _dataTable;
		public IList<ExpandoObject> _tableRows = new List<ExpandoObject>();
		private IList<ExpandoObject> _selectedRows;

		public RadzenDataGrid<ExpandoObject> _grid;
		private NotificationService _radzenNotificationService;
		public Template00UI(string userName, NotificationService radzenNotificationService)
		{
			_userName = userName;
			_radzenNotificationService = radzenNotificationService;
		}
		public IList<ExpandoObject> TableRows
		{
			get => _tableRows;
		}
		public void SetSelectedRow(ExpandoObject row, bool isAdd)
		{
			if (row == null) return;

			if (_selectedRows == null)
				_selectedRows = new List<ExpandoObject>();

			if (isAdd) 
				_selectedRows.Add(row);
			else
				_selectedRows.Remove(row);
		}
		public void SetSelectedRows(IList<ExpandoObject> rows)
		{
			if (_selectedRows != null)
				_selectedRows.Clear();

			SelectedRows = rows;
		}
		public IList<ExpandoObject> SelectedRows
		{
			get => _selectedRows;
			set
			{
				if (_selectedRows != null)
				{
					foreach (ExpandoObject row in _selectedRows)
					{
						if (_grid.IsRowInEditMode(row))
						{
							if (!value.Contains(row))
							{
								value.Add(row);
								_grid.SelectRow(row);
							}
						}
					}
				}

				_selectedRows = value;
			}
		}

		public bool IsHistoryTable()
		{
			if (_dataTable == null) return false;
			return _dataTable.Columns.IndexOf("DOCUMNET_DT") > -1;
		}
		public bool IsColumnPrimary(DataColumn dataColumn)
		{
			bool isPrimary = false;

			if (dataColumn != null)
			{
				if (dataColumn.ExtendedProperties.ContainsKey("IsPrimary"))
					isPrimary = dataColumn.ExtendedProperties["IsPrimary"]?.ToString().ToUpper() == "TRUE";
			}

			return isPrimary;
		}

		public string GetBorderClass(DataColumn dataColumn, ExpandoObject row)
		{
			if (dataColumn == null || row == null) return string.Empty;

			if (dataColumn.AllowDBNull) return "no-border";

			bool isValid = CustomValidateRow(dataColumn, row, false);

			return isValid ? "required-border" : "inValid-border";
		}

		public bool IsColumnVisible(DataColumn dataColumn)
		{
			bool isVisible = false;

			if (dataColumn != null)
			{
				if (dataColumn.ExtendedProperties.ContainsKey("IsVisible"))
					isVisible = dataColumn.ExtendedProperties["IsVisible"]?.ToString().ToUpper() == "TRUE";
			}

			return isVisible;
		}

		public string GetColumnWidth(DataColumn dataColumn)
		{
			int iWidth = 0;

			if (dataColumn != null)
			{
				if (dataColumn.ExtendedProperties.ContainsKey("Width"))
					iWidth = Convert.ToInt32(dataColumn.ExtendedProperties["Width"]);
			}

			if (iWidth == 0)
				iWidth = 120;

			return iWidth.ToString() + "px";
		}

		public string GetColumnFormat(DataColumn dataColumn)
		{ 
			string strFormat = string.Empty;
			if (dataColumn != null)
			{
				if (dataColumn.ExtendedProperties.ContainsKey("DataFormat"))
					strFormat = dataColumn.ExtendedProperties["DataFormat"].ToString();
			}
			return strFormat;
		}

		public string GetColumnFormatValue(DataColumn dataColumn, string strValue)
		{
			string strFormatValue = strValue;

			if (dataColumn != null)
			{
				string strDataFormat = GetColumnFormat(dataColumn);

				if (dataColumn.DataType == typeof(Boolean))
					strFormatValue = string.IsNullOrWhiteSpace(strValue)
								? string.Empty
								: (strValue == "True" ? "Y" : "N");

				if (!string.IsNullOrEmpty(strDataFormat))
				{
					if (dataColumn.DataType == typeof(DateTime))
						strFormatValue = string.IsNullOrWhiteSpace(strValue)
									? string.Empty
									: DateTime.Parse(strValue).ToString(strDataFormat);
					else if (dataColumn.DataType == typeof(int) || dataColumn.DataType == typeof(decimal) || dataColumn.DataType == typeof(double))
						strFormatValue = string.IsNullOrWhiteSpace(strValue)
									? string.Empty
									: Decimal.Parse(strValue).ToString(strDataFormat);
					else
						strFormatValue = string.Format(strDataFormat, strValue);
				}
			}

			return strFormatValue;
		}

		public bool CustomValidateRow(DataColumn dataColumn, ExpandoObject row, bool showMessage = true)
		{
			if (dataColumn == null || row == null) return false;
			if (dataColumn.AllowDBNull) return true;

			bool isValid = true;

			var dict = (IDictionary<string, object>)row;
			object value = dict.ContainsKey(dataColumn.ColumnName) ? dict[dataColumn.ColumnName] : null;

			if (dataColumn.DataType == typeof(string))
				isValid = !string.IsNullOrEmpty(value?.ToString());
			else if (dataColumn.DataType == typeof(int) || dataColumn.DataType == typeof(long) || dataColumn.DataType == typeof(decimal) || dataColumn.DataType == typeof(double) || dataColumn.DataType == typeof(float))
				isValid = value != null && !value.Equals(0);
			else if (dataColumn.DataType == typeof(DateTime))
				isValid = value != null && !value.Equals(DateTime.MinValue);

			if (!isValid && showMessage)
			{
				NotificationMessage message = new NotificationMessage
				{
					Severity = NotificationSeverity.Warning,
					Summary = "필수입력",
					Detail = $"{dataColumn.Caption} 은/는 필수 입력 항목입니다.",
					Duration = 3000
				};
				_radzenNotificationService.Notify(message);
			}

			return isValid;
		}

		private DataRow GetDataRowByTId(string TId)
		{
			if (_dataTable == null || string.IsNullOrEmpty(TId)) return null;

			DataRow[] dataRows = _dataTable.Select(string.Format("Tid = '{0}'", TId));
			if (dataRows.Length > 0)
				return dataRows[0];

			return null;
		}
		public void OnUpdateDataTable(IDictionary<string, object> dict)
		{
			DataRow dataRow = GetDataRowByTId(dict["TId"]?.ToString());

			if (dataRow != null)
			{
				foreach (DataColumn dataColumn in _dataTable.Columns)
				{
					if (dataColumn.ReadOnly)
						dataColumn.ReadOnly = false;

					if (dataColumn.DataType == typeof(Boolean))
						dataRow[dataColumn.ColumnName] = dict[dataColumn.ColumnName].ToString() == "1";
					else
						dataRow[dataColumn.ColumnName] = dict[dataColumn.ColumnName];
				}
			}
		}

		public async Task OnUpdateDataValue(DataColumn dataColumn, ExpandoObject row, object newValue)
		{
			if (row == null) return;

			var dict = (IDictionary<string, object>)row;
			dict[dataColumn.ColumnName] = newValue;

			if (dict["_rowState"].ToString() != "Insert")
				dict["_rowState"] = "Update";

			OnUpdateDataTable(dict);
		}

		public async Task UpdateRow()
		{
			if (_grid == null || !_grid.IsValid) return;

			if (_selectedRows == null) return;

			foreach (ExpandoObject row in _selectedRows)
			{
				await UpdateRow(row);
			}
		}

		private async Task UpdateRow(ExpandoObject row)
		{
			if (_grid == null || !_grid.IsValid) return;

			bool isValid = true;

			foreach (DataColumn dataColumn in _dataTable.Columns)
			{
				if (ignoreList.Contains(dataColumn.ColumnName) || byList.Contains(dataColumn.ColumnName) || onList.Contains(dataColumn.ColumnName)) continue;

				isValid = CustomValidateRow(dataColumn, row);
				if (!isValid)
					break;
			}

			if (isValid)
				await _grid.UpdateRow(row);
		}

		public async Task InsertRow()
		{
			if (string.IsNullOrEmpty(_userName)) return;

			if (_grid == null || !_grid.IsValid) return;
			DataRow newRow = _dataTable.NewRow();
			newRow["_rowState"] = "Insert";

			foreach (DataColumn dataColumn in _dataTable.Columns)
			{
				if (ignoreList.Contains(dataColumn.ColumnName)) continue;

				if (byList.Contains(dataColumn.ColumnName))
					newRow[dataColumn.ColumnName] = _userName;
				else if (onList.Contains(dataColumn.ColumnName))
					newRow[dataColumn.ColumnName] = DateTime.Now;
				else
				{
					if (dataColumn.DataType == typeof(string))
						newRow[dataColumn.ColumnName] = string.Empty;
					else if (dataColumn.DataType == typeof(int) || dataColumn.DataType == typeof(long) || dataColumn.DataType == typeof(decimal) || dataColumn.DataType == typeof(double) || dataColumn.DataType == typeof(float))
						newRow[dataColumn.ColumnName] = 0;
					else if (dataColumn.DataType == typeof(DateTime))
						newRow[dataColumn.ColumnName] = dataColumn.AllowDBNull ? null : DateTime.Today;
					else if (dataColumn.DataType == typeof(bool))
						newRow[dataColumn.ColumnName] = false;
					else if (dataColumn.DataType.IsValueType)
						newRow[dataColumn.ColumnName] = Activator.CreateInstance(dataColumn.DataType);
				}
			}

			_dataTable.Rows.InsertAt(newRow, 0);

			var expando = new ExpandoObject() as IDictionary<string, object>;
			foreach (DataColumn dataColumn in newRow.Table.Columns)
			{
				expando[dataColumn.ColumnName] = newRow[dataColumn];
			}

			_tableRows.Insert(0, (ExpandoObject)expando);
			SetSelectedRows(_tableRows.Take(1).ToList());

			await _grid.Reload();

			//await CancelRow();
			ExpandoObject row = _selectedRows[0];
			_grid.CancelEditRow(row);
			await EditRow();
		}

		public async Task EditRow()
		{
			if (_grid == null || !_grid.IsValid) return;

			if (_selectedRows == null) return;

			foreach (ExpandoObject row in _selectedRows)
			{
				await EditRow(row);

				break;
			}
		}

		private async Task EditRow(ExpandoObject row)
		{
			if (_grid == null || !_grid.IsValid || row == null) return;

			if (!_grid.IsRowInEditMode(row))
				await _grid.EditRow(row);
		}

		public async Task CancelRow()
		{
			if (_grid == null || !_grid.IsValid) return;

			foreach (ExpandoObject row in _selectedRows)
			{
				await CancelRow(row);
			}
		}

		private async Task CancelRow(ExpandoObject row)
		{
			if (_grid == null || !_grid.IsValid || row == null) return;

			var dict = (IDictionary<string, object>)row;
			string state = dict.ContainsKey("_rowState") && dict["_rowState"] != DBNull.Value ? dict["_rowState"].ToString() : string.Empty;

			_grid.CancelEditRow(row);

			if (state == "Insert")
			{
				_tableRows.Remove(row);

				DataRow dataRow = GetDataRowByTId(dict["TId"]?.ToString());
				if (dataRow != null)
					_dataTable.Rows.Remove(dataRow);

				_tableRows = _tableRows.ToList();
				_selectedRows = _tableRows.Take(1).ToList();
			}
			else
			{
				DataRow[] dataRows = _dataTable.Select(string.Format("Tid = '{0}'", dict["TId"]));
				if (dataRows.Length > 0)
				{
					DataRow dataRow = dataRows[0];

					dict["_rowState"] = string.Empty;

					if (dataRow.HasVersion(DataRowVersion.Original))
					{
						foreach (DataColumn dataColumn in _dataTable.Columns)
						{
							if (ignoreList.Contains(dataColumn.ColumnName) || byList.Contains(dataColumn.ColumnName) || onList.Contains(dataColumn.ColumnName)) continue;

							dict[dataColumn.ColumnName] = dataRow[dataColumn.ColumnName, DataRowVersion.Original];
						}
					}

					// await EditRow(row);
				}
			}

			OnUpdateDataTable(dict);
		}

		public async Task DeleteRow()
		{
			if (_grid == null || !_grid.IsValid) return;

			foreach (ExpandoObject row in _selectedRows)
			{
				await DeleteRow(row);
			}
		}

		private async Task DeleteRow(ExpandoObject row)
		{
			if (_grid == null || !_grid.IsValid || row == null) return;

			var dict = (IDictionary<string, object>)row;
			string state = dict.ContainsKey("_rowState") && dict["_rowState"] != DBNull.Value ? dict["_rowState"].ToString() : string.Empty;

			if (state == "Insert")
				await CancelRow(row);
			else
			{
				dict["_rowState"] = "Delete";
				OnUpdateDataTable(dict);
			}
		}

		public void OnLoadData(LoadDataArgs args)
		{
			_tableRows = _dataTable.AsEnumerable().Select(row =>
			{
				var obj = new ExpandoObject() as IDictionary<string, object>;
				foreach (DataColumn dataColumn in _dataTable.Columns)
				{
					obj.Add(dataColumn.ColumnName, row[dataColumn]);
				}
				return (ExpandoObject)obj;
			}).ToList();

			// 1. 원본 데이터에서 시작 (필터링 전 원본 보관 변수가 있다면 그것을 사용하세요)
			IEnumerable<dynamic> query = _tableRows;

			// 2. 필터링 로직 (추가된 부분)
			if (args.Filters != null && args.Filters.Any())
			{
				foreach (var filter in args.Filters)
				{
					var prop = filter.Property;
					var val = filter.FilterValue?.ToString().ToLower();
					if (string.IsNullOrEmpty(val)) continue;

					query = query.Where(x =>
					{
						var dict = (IDictionary<string, object>)x;
						if (dict.ContainsKey(prop) && dict[prop] != null)
						{
							string strData = dict[prop].ToString().ToLower();
							if (filter.Type == typeof(DateTime))
							{
								DateTime? dateData = string.IsNullOrEmpty(strData) ? (DateTime?)null : DateTime.Parse(strData);
								DateTime? dateVal = string.IsNullOrEmpty(val) ? (DateTime?)null : DateTime.Parse(val);

								return dateData == null || dateVal == null ? false : dateData.Value.Date == dateVal.Value.Date;
							}
							else
								return strData.Contains(val);
						}
						return false;
					});
				}
			}

			// 3. 정렬 로직 (기존 코드 유지)
			if (args.Sorts != null && args.Sorts.Any())
			{
				IOrderedEnumerable<dynamic> ordered = null;
				foreach (var sort in args.Sorts)
				{
					var prop = sort.Property;
					var isAsc = sort.SortOrder == SortOrder.Ascending;
					Func<dynamic, object> keySelector = x => ((IDictionary<string, object>)x).ContainsKey(prop) ? ((IDictionary<string, object>)x)[prop] : null;

					if (ordered == null)
						ordered = isAsc ? query.OrderBy(keySelector) : query.OrderByDescending(keySelector);
					else
						ordered = isAsc ? ordered.ThenBy(keySelector) : ordered.ThenByDescending(keySelector);
				}
				if (ordered != null) query = ordered;
			}

			// 4. 최종 결과 반영
			_tableRows = query.Select(x => (ExpandoObject)x).ToList();
			SetSelectedRows(_tableRows.Take(1).ToList());
		}

		public string GetJsonDataTable()
		{
			if (_grid == null || !_grid.IsValid) return string.Empty;

			string jsonDataTable = string.Empty;
			if (_tableRows != null && _tableRows.Count() > 0)
			{
				foreach (ExpandoObject row in _tableRows)
				{
					if (_grid.IsRowInEditMode(row))
					{
						bool isValid = true;
						foreach (DataColumn dataColumn in _dataTable.Columns)
						{
							if (ignoreList.Contains(dataColumn.ColumnName) || byList.Contains(dataColumn.ColumnName) || onList.Contains(dataColumn.ColumnName)) continue;

							isValid = CustomValidateRow(dataColumn, row);

							if (!isValid)
								break;
						}

						if (!isValid) return string.Empty;
					}
				}

				var jsonList = _tableRows.Where(row => !string.IsNullOrEmpty(((IDictionary<string, object>)row)["_rowState"]?.ToString())).ToList();
				jsonDataTable = JsonSerializer.Serialize(jsonList);
			}

			return jsonDataTable;
		}
	}
}
