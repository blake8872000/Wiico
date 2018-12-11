using WiicoApi.Infrastructure.ValueObject;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Infrastructure.ViewModel.ActivityFunction.Leave
{
    public class GetAbsenceFormListResponse
    {
        /// <summary>
        /// 查詢教室資料是否成功
        /// </summary>
        [JsonProperty("isQueryRoomSuccess")]
        public bool IsQueryRoomSuccess { get; set; }
        /// <summary>
        /// 查詢請假單資料是否成功
        /// </summary>
        [JsonProperty("isQueryFormSuccess")]
        public bool IsQueryFormSuccess { get; set; }
        /// <summary>
        /// 是否有點名權限
        /// </summary>
        [JsonProperty("isManager")]
        public bool IsManager { get; set; }
        /// <summary>
        /// 課程代碼
        /// </summary>
        [JsonProperty("classID")]
        public string ClassID { get; set; }
        /// <summary>
        /// 課程名稱
        /// </summary>
        [JsonProperty("className")]
        public string ClassName { get; set; }
        /// <summary>
        /// 請假單資訊
        /// </summary>
        [JsonProperty("forms")]
        public List<LeaveData>Forms { get; set; }
        /// <summary>
        /// 上課地點資訊
        /// </summary>
        [JsonProperty("rooms")]
        public List<RoomInfo> Rooms { get; set; }


        //請假單狀態{"00", "已作廢"},{ "10", "已完成"},{ "20", "待審核"},{ "30", "已抽回"},{ "40", "已駁回"}
        public static enumAbsenceFormStatus StatusStringToEnum(string status)
        {
            switch (status.ToUpper())
            {
                case "00":
                    return enumAbsenceFormStatus.Invalid;
                case "10":
                    return enumAbsenceFormStatus.Pass;
                case "20":
                    return enumAbsenceFormStatus.Wait;
                case "30":
                    return enumAbsenceFormStatus.Recall;
                case "40":
                    return enumAbsenceFormStatus.Reject;
                default:
                    return enumAbsenceFormStatus.Invalid;
            }
        }
    }
}
