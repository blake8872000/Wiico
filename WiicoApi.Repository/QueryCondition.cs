using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiicoApi.Repository
{
    public class QueryCondition
    {
        public static string noticeKey { get { return "myNotice"; } }

        public static class ModuleType
        {
            // 訊息
            public static string Message { get { return "message"; } }

            // 點名
            public static string SignIn { get { return "signIn"; } }

            // 作業
            public static string Homework { get { return "upload"; } }
            // 教材
            public static string Material { get { return "material"; } }
            /// <summary>
            /// 分組
            /// </summary>
            public static string Group { get { return "group"; } }
            public static string Discussion { get { return "discussion"; } }
            public static string Leave { get { return "leave"; } }
            /// <summary>
            /// 給iCan5專用的工版活動
            /// </summary>
            public static string General { get { return "general"; } }
            /// <summary>
            /// 投票活動
            /// </summary>
            public static string Vote { get { return "vote"; } }

            /// <summary>
            /// 模組名稱字典列表
            /// 2017-01-11 add by sophiee 訊息紀錄會顯示新增哪一種活動，顯示的名稱在此設定
            /// </summary>
            private static Dictionary<string, string> _moduleNames = new Dictionary<string, string>()
            {
                { SignIn, "點名活動"},
                { Homework, "作業活動"},
                { Material, "檔案活動"},
                { Discussion, "主題討論"},
                { Leave, "請假單"},
                { Group,"分組活動"},
                { General,"公版活動"},
                 { Vote,"投票活動"}
            };

            /// <summary>
            /// 取得模組名稱
            /// </summary>
            /// <param name="type">模組代碼</param>
            /// <returns></returns>
            public static string GetModuleName(string type)
            {
                return _moduleNames[type];
            }
        }

        /// <summary>
        /// 出席狀態參數
        /// </summary>
        public static class AttendanceState
        {
            /// <summary>
            /// 不參與活動
            /// </summary>
            public static string NoNeed { get { return "-1"; } }

            /// <summary>
            /// 出席
            /// </summary>
            public static string Attend { get { return "1"; } }

            /// <summary>
            /// 缺席
            /// </summary>
            public static string Absence { get { return "2"; } }

            private static Dictionary<string, string> _Status;
            /// <summary>
            /// 狀態字典列表
            /// </summary>
            public static Dictionary<string, string> Status
            {
                get
                {
                    if (_Status == null)
                    {
                        _Status = new Dictionary<string, string>()
                    {
                        { NoNeed, "未開放您參加此活動"},
                        { Attend, "出席"},
                        { Absence, "缺席"},
                        { "3", "遲到"},
                        { "5", "請假"},
                        { "4", "早退"}
                    };
                    }
                    return _Status;
                }
            }

            /// <summary>
            /// 狀態字典列表(精簡)
            /// </summary>
            private static Dictionary<string, string> _shortStatus = new Dictionary<string, string>()
        {
            { Attend, "出"},
            { Absence, "缺"},
            { "3", "遲"},
            { "5", "假"},
            { "4", "退"}
        };

            /// <summary>
            /// 取得狀態名稱(精簡)
            /// </summary>
            /// <param name="state">狀態代碼</param>
            /// <returns></returns>
            public static string GetShortStateName(string state)
            {
                return _shortStatus[state];
            }

            /// <summary>
            /// 取得狀態名稱
            /// </summary>
            /// <param name="state">狀態代碼</param>
            /// <returns></returns>
            public static string GetStateName(string state)
            {
                return Status[state];
            }
        }

        /// <summary>
        /// 請假單狀態參數
        /// </summary>
        public static class LeaveState
        {
            /// <summary>
            /// 表單待審核
            /// </summary>
            public static string Review { get { return "20"; } }

            private static Dictionary<string, string> _Status;
            /// <summary>
            /// 狀態字典列表
            /// </summary>
            public static Dictionary<string, string> Status
            {
                get
                {
                    if (_Status == null)
                    {
                        _Status = new Dictionary<string, string>()
                    {
                        { "00", "已作廢"},
                        { "10", "已完成"},
                        { "20", "待審核"},
                        { "30", "已抽回"},
                        { "40", "已駁回"}
                    };
                    }
                    return _Status;
                }
            }

            /// <summary>
            /// 取得狀態名稱
            /// </summary>
            /// <param name="state">狀態代碼</param>
            /// <returns></returns>
            public static string GetStateName(string state)
            {
                return Status[state];
            }
        }

        /// <summary>
        /// 請假單申請假別參數
        /// </summary>
        public static class LeaveType
        {
            /// <summary>
            /// 病假
            /// </summary>
            public static string Sick { get { return "1"; } }

            /// <summary>
            /// 事假
            /// </summary>
            public static string Leave { get { return "2"; } }

            /// <summary>
            /// 公假
            /// </summary>
            public static string Common { get { return "3"; } }

            /// <summary>
            /// 其他假
            /// </summary>
            public static string Other { get { return "4"; } }
        }

        public static class SignInFunction
        {
            /// <summary>
            /// 新增點名
            /// </summary>
            public static string Admin { get { return "signInadmin"; } }

            /// <summary>
            /// 成員(被點名)
            /// </summary>
            public static string Member { get { return "signInmember"; } }
        }
    }
}
