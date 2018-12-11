using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Service.Utility
{
    /// <summary>
    /// 處理request參數資訊
    /// </summary>
    /// <typeparam name="T">檢驗model</typeparam>
    public class RequestDataHelper<T>
    {
        /// <summary>
        /// 判斷單一個key值是否為empty
        /// </summary>
        /// <param name="datas">欲檢驗的資料</param>
        /// <param name="columnKey">欲比對的欄位</param>
        /// <returns>回傳檢驗的值</returns>
        public string CheckColumnEmpty(T datas, string columnKey)
        {
            var propertyInfos = datas.GetType().GetProperties();
            var response = string.Empty;
            var checkKey = string.Empty;
            foreach (var info in propertyInfos)
            {
                #region 為了request為Post或Get時，只會傳進來KeyValuePair<string,object>而多的邏輯
                if (info.Name.ToLower() == "key")
                    checkKey = info.GetValue(datas, null).ToString();
                if (checkKey.ToLower() == columnKey &&
                    info.Name.ToLower() == "value")
                    response = info.GetValue(datas, null).ToString();
                else if (info.Name.ToLower() == "value")
                {
                    var obj = info.GetValue(datas, null);
                    //標記起來是想嘗試回傳物件
                    //  var objNameSpace = info.GetValue(datas, null).ToString();
                    // var objPath = objNameSpace.Split('.');
                    // var objUrl =string.Format("{0}.{1}", objPath[0], objPath[1]);
                    // var assembly = Assembly.Load(objUrl).CreateInstance(objNameSpace);
                    //var dynamicClass = assembly.CreateInstance(objNameSpace);
                    if (obj == null)
                        continue;
                    var entityPropertys = obj.GetType().GetProperties();
                    foreach (var entityProperty in entityPropertys)
                    {
                        if (entityProperty.Name.ToLower() == columnKey)
                        {
                            var _value = entityProperty.GetValue(obj, null);
                            response = _value != null ? _value.ToString() : null;
                            /*    assembly.GetType().GetProperty("Token").SetValue(assembly, token);
                                var type =assembly.GetType();*/
                        }
                    }
                }
                #endregion

                //data直接就是model，可以直接比對
                if (info.Name.ToLower() == columnKey)
                {
                    var value = info.GetValue(datas, null);
                    if (value != null)
                        response = info.GetValue(datas, null).ToString();
                }
            }
            if (response != string.Empty)
                return response;
            return null;
        }
        /// <summary>
        /// 判斷多個key值是否為empty
        /// </summary>
        /// <param name="datas">欲檢驗的資料</param>
        /// <param name="columnKeys">欲比對的欄位</param>
        /// <returns>回傳bool</returns>
        public bool CheckColumnEmpty(T datas, object[] columnKeys)
        {
            var propertyInfos = datas.GetType().GetProperties();
            var type = datas.GetType();
            var columnKey = SetColumnKeys(columnKeys);
            var response = false;
            var checkKey = string.Empty;
            foreach (var info in propertyInfos)
            {
                if (columnKey.FirstOrDefault(t => t.ToString().ToLower() == info.Name.ToLower()) != null)
                {
                    var value = info.GetValue(datas, null);
                    if (value != null)
                        response = true;
                    else //只要有值是null，就傳false
                        return false;
                }
            }
            return response;
        }

        /// <summary>
        /// 取出要比對的columnKeys
        /// </summary>
        /// <param name="columnKeys"></param>
        /// <returns></returns>
        private List<string> SetColumnKeys(object[] columnKeys)
        {
            var response = new List<string>();
            foreach (var columnKey in columnKeys)
            {
                var columnName = columnKey.ToString();
                response.Add(columnName);
            }
            return response;
        }


    }
}
