using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WiicoApi.Infrastructure.Entity;
using WiicoApi.Infrastructure.Property;
using WiicoApi.Infrastructure.ViewModel;
using WiicoApi.Infrastructure.ViewModel.Backend;
using WiicoApi.Infrastructure.ViewModel.Login;
using WiicoApi.Repository;
using WiicoApi.Service.Utility;

namespace WiicoApi.Service.CommenService
{
    public class MemberService
    {
        private readonly GenericUnitOfWork _uow;
        private string appKey = System.Configuration.ConfigurationManager.AppSettings["AppLoginKey"].ToString();
        public MemberService()
        {
            _uow = new GenericUnitOfWork();
        }
        public MemberService(GenericUnitOfWork uow)
        {
            _uow = uow;
        }
        public MemberService(string connectionString = null)
        {
            _uow = new GenericUnitOfWork(connectionString);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="requestData"></param>
        /// <returns></returns>
        public Infrastructure.ViewModel.MemberManage.GetCourseMemberInfoResponse APPGetCourseMemberInfo(Infrastructure.ViewModel.MemberManage.GetCourseMemberInfoRequest requestData)
        {

            var memberService = new MemberService();
            var checkToken = memberService.TokenToMember(requestData.ICanToken).Result;
            if (checkToken == null)
                return null;

            var memberInfo = AccountToMember(requestData.QueryAccount, checkToken.OrgId);
            if (memberInfo == null)
                return null;

            var db = _uow.DbContext;
            var memberRoleInfo = (from cmr in db.CircleMemberRoleplay
                                  join lr in db.LearningRole on cmr.RoleId equals lr.Id
                                  join lc in db.LearningCircle on cmr.CircleId equals lc.Id
                                  where cmr.MemberId == memberInfo.Id && lc.LearningOuterKey == requestData.CircleKey
                                  select lr).ToList().FirstOrDefault();
            if (memberRoleInfo == null)
                return null;

            var response = new Infrastructure.ViewModel.MemberManage.GetCourseMemberInfoResponse()
            {
                Account = requestData.QueryAccount,
                Email = memberInfo.Email,
                Name = memberInfo.Name,
                Photo = memberInfo.Photo,
                IsShowEmail = memberInfo.IsShowEmail,
                GraduationStatus = memberInfo.GraduationStatus,
                Grade = memberInfo.Grade,
                SchoolRoll = memberInfo.SchoolRoll,
                RoleName = memberRoleInfo.Name
            };
            if (memberInfo.DeptId.HasValue)
            {
                var deptInfo = db.Depts.FirstOrDefault(t => t.Id == memberInfo.DeptId.Value);
                response.DeptId = deptInfo.Id;
                response.CollegeName = deptInfo.Name;
                response.CollegeBriefName = string.Format("{0} {1}年級", deptInfo.ShortName, memberInfo.Grade);
            }

            return response;

        }

        /// <summary>
        /// 取得所有使用者清單列表
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Infrastructure.ValueObject.AuthMember> GetAllMemberList()
        {
            var db = _uow.DbContext;
            var result = new List<Infrastructure.ValueObject.AuthMember>();

            //取得所有使用者清單
            var sqlMemberList = db.Members.ToList();
            foreach (var _item in sqlMemberList)
            {
                var resAuthMember = new Infrastructure.ValueObject.AuthMember();
                resAuthMember.AccountId = _item.Id;
                resAuthMember.AccountName = _item.Name;
                resAuthMember.Account = _item.Account;
                result.Add(resAuthMember);
            }
            return result;
        }

        public IEnumerable<Member> GetLearningCircleMembers(string circleKey, int? skipMemberId = null)
        {
            var db = _uow.DbContext;
            var members = skipMemberId.HasValue ? (from m in db.Members
                                                   join cm in db.CircleMemberRoleplay on m.Id equals cm.MemberId
                                                   join lc in db.LearningCircle on cm.CircleId equals lc.Id
                                                   where lc.LearningOuterKey == circleKey && m.Id != skipMemberId
                                                   select m).ToList() : (from m in db.Members
                                                                         join cm in db.CircleMemberRoleplay on m.Id equals cm.MemberId
                                                                         join lc in db.LearningCircle on cm.CircleId equals lc.Id
                                                                         where lc.LearningOuterKey == circleKey
                                                                         select m).ToList();
            if (members.Count() <= 0)
                return null;

            return members;
        }

        /// <summary>
        /// 取出學習圈學生列表
        /// </summary>
        /// <param name="circleKey"></param>
        /// <returns></returns>
        public IEnumerable<Member> GetStudentList(string circleKey)
        {
            return _uow.MembersRepo.GetLearningCircleMemberRoleList(circleKey, false);
        }

        /// <summary>
        /// 取出學習圈老師列表
        /// </summary>
        /// <param name="circleKey"></param>
        /// <returns></returns>
        public IEnumerable<Member> GetTeacherList(string circleKey)
        {
            return _uow.MembersRepo.GetLearningCircleMemberRoleList(circleKey, true);
        }

        public Member GetMemberInfo(int accountId)
        {
            //2016/9/12 mark by sophiee:為顯示舊ican個人大頭照，不讀取members table
            //iThinkDB db = new iThinkDB();
            //var sqlMemberInfo = db.Members.Find(accountId);
            //return sqlMemberInfo;

            var data = _uow.MembersRepo.MemberInfo(accountId);

            var sqlMemberInfo = new Member();
            sqlMemberInfo.Id = data.Id;
            sqlMemberInfo.Name = data.Name;
            sqlMemberInfo.OrgId = data.OrgId;
            sqlMemberInfo.Email = data.Email;
            sqlMemberInfo.Account = data.Account;
            sqlMemberInfo.Photo = data.Photo;

            return sqlMemberInfo;
        }

        public Member SetConnectionId(string connecitonId,int memberId) {
            var memberInfo = _uow.DbContext.Members.Find(memberId);
            memberInfo.ConnectionId = connecitonId;
            _uow.SaveChanges();
            return null;
        }

        /// <summary>
        /// Token轉MemberAccount
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<Member> TokenToMember(string token)
        {
            var _token = Guid.NewGuid();
            var checkToken = Guid.TryParse(token, out _token);
            if (checkToken)
                return await TokenToMember(_token);
            else
                return null;
        }

        /// <summary>
        /// Token轉MemberAccount
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<Member> TokenToMember(Guid token)
        {
            var tokenService = new TokenService();
            var userToken = await tokenService.GetTokenInfo(token.ToString());
            if (userToken != null)
            {
                //取得memberAccount
                int myId = userToken.MemberId;
                var me = _uow.EntityRepository<Member>().GetFirst(x => x.Id == myId);
                return me;
            }
            else
                return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Member UserIdToAccount(int id)
        {
            var me = _uow.DbContext.Members.FirstOrDefault(x => x.Id == id);
            return me != null ? me : null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public Member AccountToMember(string account, int? orgId = null)
        {
            var db = _uow.DbContext;
            var me = orgId.HasValue ?
                db.Members.FirstOrDefault(x => x.Account.ToLower() == account.ToLower() && x.OrgId == orgId.Value) : 
                (from m in db.Members
                 join O in db.Organizations on m.OrgId equals O.Id
                 where m.Account.ToLower() == account.ToLower() && O.OrgCode == "amateur"
                 select m).FirstOrDefault();
            return me != null ? me : null;
        }

        /// <summary>
        /// 取得使用者資料
        /// </summary>
        /// <param name="account">帳號</param>
        /// <param name="schoolName">學校代碼</param>
        /// <returns></returns>
        public Member GetMemberByAccountSchool(string account, string orgCode)
        {
            var db = _uow.DbContext;

            var me = (from m in db.Members
                      join o in db.Organizations on m.OrgId equals o.Id
                      where o.OrgCode.ToLower() == orgCode.ToLower() &&
                                  m.Account.ToLower() == account.ToLower()
                      select m).FirstOrDefault();

            return me;
        }
        /// <summary>
        /// 根據組織編號取得成員列表 - 人員管理專用
        /// </summary>
        /// <param name="orgId"></param>
        /// <param name="searchAccount">要查詢的人名或是帳號名[先保留彈性]</param>
        /// <returns></returns>
        public IEnumerable<UserGetResponse> GetBackendMemberListByOrgId(int? orgId, string searchAccount)
        {
            var db = _uow.DbContext;

            var list = (orgId.HasValue) ?
                      (from m in db.Members
                       join og in db.Organizations on m.OrgId equals og.Id
                       where og.Id == orgId.Value
                       select m) :
                    (db.Members);
            if (searchAccount != null && searchAccount != string.Empty)
                list.Where(t => t.Account.StartsWith(searchAccount));

            var responseDatas = (from m in list.ToList()
                                 select new UserGetResponse
                                 {
                                     Account = m.Account,
                                     Id = m.Id,
                                     ClassGrade = m.ClassGrade,
                                     ConnectionId = m.ConnectionId,
                                     DeptId = m.DeptId,
                                     Email = m.Email,
                                     Enable = m.Enable,
                                     ExternalRid = m.ExternalRid,
                                     ExtraInfo = new UserGetResponseExtraInfo
                                     {
                                         Grade = m.Grade,
                                         GraduationStatus = m.GraduationStatus,
                                         SchoolRoll = m.SchoolRoll,
                                         SemesterGrade = m.SemesterGradeId
                                     },
                                     IsOrgAdmin = m.IsOrgAdmin,
                                     Name = m.Name,
                                     IsShowEmail = m.IsShowEmail,
                                     OrganizationRoleId = m.OrganizationRoleId,
                                     OrgId = m.OrgId,
                                     Photo = m.Photo,
                                     Visibility = m.Visibility,
                                     PassWord = m.PassWord,
                                     RoleName = m.RoleName,
                                     Verified = m.Verified
                                 }).ToList();

            return responseDatas.OrderBy(t => t.Id).ToList();
        }

        /// <summary>
        /// 根據組織編號取得成員列表 - 人員管理專用
        /// </summary>
        /// <param name="orgId"></param>
        /// <returns></returns>
        public IEnumerable<Member> GetBackendMemberList(int orgId, string searchString, bool? status)
        {
            var db = _uow.DbContext;

            var list = from m in db.Members
                       join og in db.Organizations on m.OrgId equals og.Id
                       where m.OrgId == orgId
                       select m;
            if (searchString != null && searchString != string.Empty)
                list = list.Where(t => t.Name.Contains(searchString) || t.Account.Contains(searchString));

            if (status != null)
                list = list.Where(s => s.Enable == status.Value).OrderBy(o => o.Id);

            return list.OrderBy(t => t.Id).ToList();
        }

        /// <summary>
        /// 註冊帳號
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public UserPostResponse RegisterMember(RegisterRequest data, FileViewModel photo)
        {
            var memberService = new MemberService();
            var creatorInfo = memberService.TokenToMember(data.Token).Result;
            if (creatorInfo == null)
                return null;
            var encryptionService = new Encryption();

            var hostUrl = System.Configuration.ConfigurationManager.AppSettings["loginServer"].ToString();


            if (data.OrgCode == null || data.OrgCode == string.Empty)
                data.OrgCode = "amateur";

            var checkRegisted = GetMemberByAccountSchool(data.Account, data.OrgCode);
            //該帳號已經註冊過了
            if (checkRegisted != null)
                return null;
            var db = _uow.DbContext;
            var organizationInfo = db.Organizations.FirstOrDefault(t => t.OrgCode == data.OrgCode);
            try
            {
                var newMember = new Member()
                {
                    Account = data.Account,
                    Created = TimeData.Create(DateTime.UtcNow),
                    Deleted = TimeData.Create(null),
                    Updated = TimeData.Create(null),
                    Name = data.Name,
                    Email = data.Email,
                    Enable = true,
                    IsShowEmail = false,
                    Visibility = true,
                    IsOrgAdmin = Convert.ToInt32(data.RoleId) == 1 ? true : false,
                    OrgId = organizationInfo.Id,
                    PassWord = encryptionService.StringToSHA256(string.Format("{0}{1}", encryptionService.DecryptString(data.Pwd, appKey), data.Account)),
                    Photo = string.Format("{0}{1}", hostUrl, "images/img-user.png"),
                    RoleName = data.RoleId.ToString(),
                    Verified = true,
                    CreateUser = creatorInfo.Id,
                    ExternalRid = 0
                };
                db.Members.Add(newMember);
                //為了取得memberId
                db.SaveChanges();

                //上傳大頭照
                if (photo != null && photo.ContentLength > 0)
                {
                    var fileService = new FileService();
                    var maxImgWidth = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["maxImgWidth"].ToString());
                    var maxImgHeight = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["maxImgHeight"].ToString());
                    var drivePath = System.Configuration.ConfigurationManager.AppSettings["DrivePath"].ToString();
                    var fileInfo = fileService.UploadFile(newMember.Id, photo.FileName, photo.ContentType, photo.ContentLength, maxImgHeight, maxImgWidth);
                    var path = Path.Combine(drivePath, fileInfo.FileGuid.ToString("N"));
                    var stream = photo.InputStream;
                    /*stream轉bytes*/
                    var br = new BinaryReader(stream);
                    br.BaseStream.Seek(0, SeekOrigin.Begin);
                    var bytesInStream = br.ReadBytes((int)br.BaseStream.Length);
                    //實際檔案處理 
                    fileService.FileProxy(photo.ContentLength, path, stream, bytesInStream);
                    newMember.Photo = fileInfo.FileImageUrl;
                    //最終儲存
                    db.SaveChanges();
                }
                var responseData = new UserPostResponse()
                {
                    CreateAccount = creatorInfo.Account,
                    CreateTime = newMember.Created.Local.Value,
                    Enable = newMember.Enable,
                    ExternalRid = newMember.ExternalRid,
                    IsShowMail = newMember.IsShowEmail,
                    Photo = newMember.Photo,
                    Verified = newMember.Verified
                };
                return responseData;
            }
            catch (Exception ex)
            {
                return null;
                throw ex;
            }
        }

        /// <summary>
        /// 只更新照片
        /// </summary>
        /// <param name="token"></param>
        /// <param name="account"></param>
        /// <param name="photo"></param>
        /// <returns></returns>
        public bool UpdateMemberPhotoInfo(string token, string account, string photo)
        {

            try
            {
                var db = _uow.DbContext;
                var tokenService = new TokenService();
                //取得目前DB登入
                var checkUserToken = tokenService.GetTokenInfo(token).Result;
                //db.UserToken.FirstOrDefault(t => t.Token == token);
                if (checkUserToken == null)
                    return false;


                var memberInfo = db.Members.FirstOrDefault(t => t.Id == checkUserToken.MemberId);

                if (memberInfo == null)
                    return false;

                var fileService = new FileService();


                memberInfo.Updated = TimeData.Create(DateTime.UtcNow);
                memberInfo.UpdateUser = memberInfo.Id;
                db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// 只更新Email
        /// </summary>
        /// <param name="token"></param>
        /// <param name="account"></param>
        /// <param name="email"></param>
        /// <param name="isShowMail"></param>
        /// <returns></returns>
        public bool UpdateMemberEmailInfo(string token, string account, string email, bool? isShowMail = false)
        {

            try
            {
                var db = _uow.DbContext;
                var tokenService = new TokenService();
                //取得目前DB登入
                var checkUserToken = tokenService.GetTokenInfo(token).Result;
                //db.UserToken.FirstOrDefault(t => t.Token == token);
                if (checkUserToken == null)
                    return false;

                var memberInfo = db.Members.FirstOrDefault(t => t.Id == checkUserToken.MemberId);

                if (memberInfo == null)
                    return false;

                memberInfo.Email = email;
                memberInfo.Updated = TimeData.Create(DateTime.UtcNow);
                memberInfo.UpdateUser = memberInfo.Id;
                memberInfo.IsShowEmail = isShowMail.Value;
                db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        /// <summary>
        /// 更新成員資訊 - 管理者修改
        /// </summary>
        /// <param name="token">登入者代碼</param>
        /// <param name="name">欲修改姓名</param>
        /// <param name="email">欲修改信箱</param>
        /// <param name="roleName">欲修改角色</param>
        /// <param name="file">欲修改大頭照</param>
        /// <returns></returns>
        public Member UpdateMemberInfo(MemberManagePutRequest requestData)
        {
            try
            {
                var db = _uow.DbContext;
                var tokenService = new TokenService();
                //取得目前DB登入
                var checkUserToken = tokenService.GetTokenInfo(requestData.Token).Result;

                if (checkUserToken == null)
                    return null;

                var updaterInfo = db.Members.FirstOrDefault(t => t.Id == checkUserToken.MemberId);
                var memberInfo = db.Members.FirstOrDefault(t => t.OrgId == requestData.OrgId && t.Account.ToLower() == requestData.Account.ToLower());
                if (memberInfo == null)
                    return null;

                memberInfo.Email = requestData.Email;

                if (requestData.Name != null && requestData.Name != string.Empty)
                    memberInfo.Name = requestData.Name;

                if (requestData.RoleId.HasValue)
                    memberInfo.RoleName = requestData.RoleId.ToString();
                memberInfo.Updated = TimeData.Create(DateTime.UtcNow);
                memberInfo.UpdateUser = updaterInfo.Id;
                //if (requestData.Photo != string.Empty && requestData.Photo != null)
                //    memberInfo.Photo = requestData.Photo;
                memberInfo.IsShowEmail = requestData.IsShowEmail;
                db.SaveChanges();
                return memberInfo;
            }
            catch (Exception ex)
            {
                return null;
                throw ex;
            }
        }

        public bool UpdateMemberBySelf(PersonPutRequest requestData)
        {
            try
            {
                var db = _uow.DbContext;
                var tokenService = new TokenService();
                var checkUserToken = tokenService.GetTokenInfo(requestData.Token).Result;
                if (checkUserToken == null)
                    return false;
                var memberInfo = db.Members.FirstOrDefault(t => t.Id == checkUserToken.MemberId);
                if (memberInfo == null)
                    return false;

                memberInfo.Email = requestData.Email;

                if (requestData.Name != null && requestData.Name != string.Empty)
                    memberInfo.Name = requestData.Name;

                if (requestData.RoleId.HasValue)
                    memberInfo.RoleName = requestData.RoleId.ToString();
                memberInfo.Updated = TimeData.Create(DateTime.UtcNow);
                memberInfo.UpdateUser = memberInfo.Id;
                if (requestData.Photo != string.Empty && requestData.Photo != null)
                    memberInfo.Photo = requestData.Photo;
                memberInfo.IsShowEmail = requestData.IsShowEmail;


                db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// 更新成員資訊 - 人員管理專用
        /// </summary>
        /// <param name="data"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        public bool UpdateMemberInfo(RegisterRequest data, FileViewModel file)
        {
            try
            {
                var db = _uow.DbContext;
                var encryptionService = new Encryption();
                //取得目前DB人員資料
                var memberInfo = db.Members.FirstOrDefault(t => t.Id == data.Id);
                if (memberInfo == null)
                    return false;

                memberInfo.Account = data.Account;
                memberInfo.Name = data.Name;
                memberInfo.Email = data.Email;
                memberInfo.Updated = TimeData.Create(DateTime.UtcNow);
                memberInfo.UpdateUser = data.Id;
                memberInfo.Enable = data.Enable;
                if (data.Pwd != null)
                {
                    var deCodePwd = encryptionService.DecryptString(data.Pwd, appKey);
                    memberInfo.PassWord = encryptionService.StringToSHA256(deCodePwd);
                }
                if (file != null)
                {
                    if (file.ContentLength > 0)
                        memberInfo.Photo = memberPhotoProxy(data.Id, file);
                }
                db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// 處理成員照片資料
        /// </summary>
        /// <param name="memberId">使用者編號</param>
        /// <param name="file">照片</param>
        /// <returns></returns>
        private string memberPhotoProxy(int memberId, Infrastructure.ViewModel.FileViewModel file)
        {
            var fileService = new FileService();
            var maxImgWidth = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["maxImgWidth"].ToString());
            var maxImgHeight = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["maxImgHeight"].ToString());
            var drivePath = System.Configuration.ConfigurationManager.AppSettings["DrivePath"].ToString();
            var fileInfo = fileService.UploadFile(memberId, file.FileName, file.ContentType, file.ContentLength, maxImgHeight, maxImgWidth);
            var path = Path.Combine(drivePath, fileInfo.FileGuid.ToString("N"));
            var stream = file.InputStream;
            /*stream轉bytes*/
            System.IO.BinaryReader br = new System.IO.BinaryReader(stream);
            br.BaseStream.Seek(0, System.IO.SeekOrigin.Begin);
            var bytesInStream = br.ReadBytes((int)br.BaseStream.Length);
            //實際檔案處理 
            fileService.FileProxy(file.ContentLength, path, stream, bytesInStream);

            if (fileInfo == null)
                return null;
            else
                return fileInfo.FileUrl;
        }

        /// <summary>
        /// 刪除成員角色
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public bool DeleteMemberInfoByAccount(string token, string account)
        {
            try
            {
                var db = _uow.DbContext;
                var memberService = new MemberService();
                var checkToken = memberService.TokenToMember(token).Result;
                var memberInfo = AccountToMember(account, checkToken.OrgId);
                if (memberInfo == null)
                    return false;

                if (checkToken == null)
                    return false;
                var result = DeleteMemberInfo(memberInfo.Id, checkToken.Id);

                return result;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// 刪除成員角色
        /// </summary>
        /// <param name="memberId"></param>
        /// <param name="deleter">刪除者</param>
        /// <returns></returns>
        public bool DeleteMemberInfo(int memberId, int deleter)
        {
            try
            {
                var db = _uow.DbContext;
                var memberInfo = UserIdToAccount(memberId);
                if (memberInfo == null)
                    return false;
                else
                {
                    memberInfo.Enable = false;
                    memberInfo.Visibility = false;
                    memberInfo.Deleted = TimeData.Create(DateTime.UtcNow);
                    memberInfo.DeleteUser = deleter;
                    db.SaveChanges();
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
                throw ex;
            }
        }
        /// <summary>
        /// 刪除多筆成員
        /// </summary>
        /// <returns></returns>
        public bool DeleteMultipleMember(MemberManageDeleteRequest requestData)
        {
            var memberService = new MemberService();
            var deleter = memberService.TokenToMember(requestData.Token).Result;
            if (deleter == null)
                return false;
            try
            {
                var deleteTargets = (from m in _uow.DbContext.Members
                                     join r in requestData.Members on m.Id equals r
                                     select m);
                foreach (var target in deleteTargets)
                {
                    target.Enable = false;
                    target.Visibility = false;
                    target.Verified = false;
                }
                _uow.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
                throw ex;
            }
        }

        /// <summary>
        /// 批次修改人員狀態
        /// </summary>
        /// <param name="memberIds"></param>
        /// <returns></returns>
        public bool BatchProxyMemberStatus(string memberIds)
        {
            string[] ids = memberIds.Split(',');
            if (ids.Count() < 1)
                return false;
            try
            {
                var db = _uow.DbContext;
                foreach (string id in ids)
                {
                    var member = db.Members.Find(int.Parse(id));
                    member.Enable = !member.Enable;
                }
                db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
