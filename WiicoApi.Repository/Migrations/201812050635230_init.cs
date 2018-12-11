namespace WiicoApi.Repository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ActBulletins",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(),
                        Content = c.String(),
                        Creator = c.Int(nullable: false),
                        CreateDate = c.DateTime(nullable: false),
                        FileId = c.Int(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ActDiscussions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        GoogleDriveUrl = c.String(),
                        Enable = c.Boolean(nullable: false),
                        Visibility = c.Boolean(nullable: false),
                        LearningId = c.Int(nullable: false),
                        EventId = c.Guid(nullable: false),
                        Description = c.String(),
                        FileCount = c.Int(nullable: false),
                        TagId = c.Int(),
                        Name = c.String(),
                        CreateUser = c.Int(),
                        UpdateUser = c.Int(),
                        DeleteUser = c.Int(),
                        Created_Utc = c.DateTime(),
                        Updated_Utc = c.DateTime(),
                        Deleted_Utc = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ActDiscussionMsgs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ActModuleMsgId = c.Int(nullable: false),
                        ActDiscussionId = c.Int(nullable: false),
                        OuterKey = c.Guid(nullable: false),
                        CreateUser = c.Int(),
                        UpdateUser = c.Int(),
                        DeleteUser = c.Int(),
                        Created_Utc = c.DateTime(),
                        Updated_Utc = c.DateTime(),
                        Deleted_Utc = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ActGenerals",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(),
                        Content = c.String(),
                        EventId = c.Guid(nullable: false),
                        LearningId = c.Int(nullable: false),
                        ActType = c.String(),
                        Publish_Utc = c.DateTime(),
                        Icon = c.String(),
                        Picture = c.String(),
                        IsWebView = c.Boolean(nullable: false),
                        Target_Type = c.Int(nullable: false),
                        Target_Url = c.String(),
                        Creator = c.Int(),
                        Updater = c.Int(),
                        Deleter = c.Int(),
                        CreateDate_Utc = c.DateTime(),
                        UpdateDate_Utc = c.DateTime(),
                        DeleteDate_Utc = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ActGroups",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CategoryId = c.Int(nullable: false),
                        Enable = c.Boolean(nullable: false),
                        Visibility = c.Boolean(nullable: false),
                        Sort = c.Int(nullable: false),
                        GroupId = c.String(),
                        Name = c.String(),
                        CreateUser = c.Int(),
                        UpdateUser = c.Int(),
                        DeleteUser = c.Int(),
                        Created_Utc = c.DateTime(),
                        Updated_Utc = c.DateTime(),
                        Deleted_Utc = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ActGroupCategories",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        LearningId = c.Int(nullable: false),
                        Name = c.String(),
                        EventId = c.Guid(nullable: false),
                        Enable = c.Boolean(nullable: false),
                        Content = c.String(),
                        Visibility = c.Boolean(nullable: false),
                        CreateUser = c.Int(),
                        UpdateUser = c.Int(),
                        DeleteUser = c.Int(),
                        Created_Utc = c.DateTime(),
                        Updated_Utc = c.DateTime(),
                        Deleted_Utc = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ActGroupMembers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        GroupId = c.Int(nullable: false),
                        MemberId = c.Int(nullable: false),
                        IsLeader = c.Boolean(nullable: false),
                        IsGroupMember = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ActHomeWorks",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        LearningId = c.Int(nullable: false),
                        EventId = c.Guid(nullable: false),
                        Name = c.String(maxLength: 100),
                        StartDate = c.DateTime(),
                        EndDate = c.DateTime(),
                        OverdueDate = c.DateTime(),
                        Description = c.String(),
                        GoogleDriveFolder = c.String(),
                        LectureCount = c.Int(nullable: false),
                        AllowRelease = c.Boolean(nullable: false),
                        Released = c.Boolean(nullable: false),
                        AllowOverDue = c.Boolean(nullable: false),
                        Visibility = c.Boolean(nullable: false),
                        CreateUser = c.Int(),
                        UpdateUser = c.Int(),
                        DeleteUser = c.Int(),
                        Created_Utc = c.DateTime(),
                        Updated_Utc = c.DateTime(),
                        Deleted_Utc = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ActHomeWorkLogs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        HomeWorkId = c.Int(nullable: false),
                        Description = c.String(),
                        Time = c.DateTime(nullable: false),
                        StudId = c.Int(nullable: false),
                        Status = c.Int(),
                        FileCount = c.Int(nullable: false),
                        TempGoogleDriveFolder = c.String(maxLength: 50),
                        AllowSend = c.Boolean(nullable: false),
                        Participated = c.Boolean(nullable: false),
                        CreateUser = c.Int(),
                        UpdateUser = c.Int(),
                        DeleteUser = c.Int(),
                        Created_Utc = c.DateTime(),
                        Updated_Utc = c.DateTime(),
                        Deleted_Utc = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ActHomeWorkScores",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Score = c.Int(),
                        HomeWorkLogId = c.Int(nullable: false),
                        SendScore = c.Int(),
                        BackScore = c.Int(),
                        CreateUser = c.Int(),
                        UpdateUser = c.Int(),
                        DeleteUser = c.Int(),
                        Created_Utc = c.DateTime(),
                        Updated_Utc = c.DateTime(),
                        Deleted_Utc = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Activitys",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ToRoomId = c.String(),
                        ModuleKey = c.String(maxLength: 100),
                        OuterKey = c.Guid(nullable: false),
                        StartDate = c.DateTime(),
                        ActivityDate = c.DateTime(),
                        Duration = c.Int(),
                        IsActivity = c.Boolean(nullable: false),
                        Publish_Utc = c.DateTime(),
                        CardisShow = c.Boolean(),
                        CreateUser = c.Int(),
                        UpdateUser = c.Int(),
                        DeleteUser = c.Int(),
                        Created_Utc = c.DateTime(),
                        Updated_Utc = c.DateTime(),
                        Deleted_Utc = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ActivitysNotices",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ToRoomId = c.String(),
                        MemberId = c.Int(nullable: false),
                        EventId = c.Guid(nullable: false),
                        NoticeContent = c.String(maxLength: 150),
                        HasClick = c.Boolean(nullable: false),
                        CreateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ActivitysReadMarks",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ToRoomId = c.String(),
                        memberId = c.Int(nullable: false),
                        LastReadActivityIdBegin = c.Int(nullable: false),
                        LastReadActivityIdEnd = c.Int(nullable: false),
                        Time = c.DateTime(),
                        Enabled = c.Boolean(),
                        UpdateDate_Utc = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ActivitySyllabus",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ActivityId = c.Int(nullable: false),
                        SyllabusId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ActMaterials",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        LearningId = c.Int(nullable: false),
                        EventId = c.Guid(nullable: false),
                        Name = c.String(),
                        GoogleDriveFileId = c.String(),
                        GoogleDriveFolder = c.String(),
                        FileType = c.String(),
                        FileLength = c.Int(nullable: false),
                        FileImgUrl = c.String(),
                        Visibility = c.Boolean(nullable: false),
                        CreateUser = c.Int(),
                        UpdateUser = c.Int(),
                        DeleteUser = c.Int(),
                        Created_Utc = c.DateTime(),
                        Updated_Utc = c.DateTime(),
                        Deleted_Utc = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ActMessages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        LearningId = c.Int(nullable: false),
                        EventId = c.Guid(nullable: false),
                        Type = c.String(maxLength: 20),
                        Content = c.String(),
                        Visibility = c.Boolean(nullable: false),
                        CreateUser = c.Int(),
                        UpdateUser = c.Int(),
                        DeleteUser = c.Int(),
                        Created_Utc = c.DateTime(),
                        Updated_Utc = c.DateTime(),
                        Deleted_Utc = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ActModuleMessages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ActivityId = c.Int(nullable: false),
                        MsgType = c.String(),
                        Content = c.String(),
                        ModuleType = c.String(),
                        OuterKey = c.Guid(nullable: false),
                        Visibility = c.Boolean(nullable: false),
                        Parent = c.Int(),
                        TagActModuleMessageId = c.Int(),
                        CreateUser = c.Int(),
                        UpdateUser = c.Int(),
                        DeleteUser = c.Int(),
                        Created_Utc = c.DateTime(),
                        Updated_Utc = c.DateTime(),
                        Deleted_Utc = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ActRollCalls",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        LearningId = c.Int(nullable: false),
                        EventId = c.Guid(nullable: false),
                        Name = c.String(maxLength: 100),
                        SignInKey = c.String(),
                        SignInPwd = c.String(maxLength: 50),
                        Visibility = c.Boolean(nullable: false),
                        CreateUser = c.Int(),
                        UpdateUser = c.Int(),
                        DeleteUser = c.Int(),
                        Created_Utc = c.DateTime(),
                        Updated_Utc = c.DateTime(),
                        Deleted_Utc = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ActRollCallLogs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        RollCallId = c.Int(nullable: false),
                        Time = c.DateTime(),
                        StudId = c.Int(nullable: false),
                        Status = c.String(maxLength: 2),
                        Memo = c.String(maxLength: 500),
                        CreateUser = c.Int(),
                        UpdateUser = c.Int(),
                        DeleteUser = c.Int(),
                        Created_Utc = c.DateTime(),
                        Updated_Utc = c.DateTime(),
                        Deleted_Utc = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ActVotes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        EventId = c.Guid(nullable: false),
                        Title = c.String(maxLength: 100),
                        Content = c.String(),
                        CreateDateUtc = c.DateTime(nullable: false),
                        Creator = c.Int(nullable: false),
                        IsStart = c.Boolean(nullable: false),
                        PresentCount = c.Int(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ActVoteItems",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Sort = c.Int(),
                        Content = c.String(),
                        Title = c.String(maxLength: 100),
                        ChooseRate = c.Double(nullable: false),
                        ChooseCount = c.Int(nullable: false),
                        ActVoteId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AppVersions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AppSystem = c.String(),
                        CreateUtcDate = c.DateTime(nullable: false),
                        UpdateUtcDate = c.DateTime(),
                        Version = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AttendanceLeaves",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        EventId = c.Guid(nullable: false),
                        LearningId = c.Int(nullable: false),
                        StudId = c.Int(nullable: false),
                        LeaveDate = c.DateTime(nullable: false),
                        LeaveType = c.Int(),
                        Subject = c.String(maxLength: 200),
                        Content = c.String(maxLength: 500),
                        Status = c.String(maxLength: 2),
                        Comment = c.String(maxLength: 200),
                        Creator = c.Int(nullable: false),
                        CreateDate = c.DateTime(nullable: false),
                        UpdateTime = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AttendanceLeaveLogs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        LeaveId = c.Int(nullable: false),
                        Creator = c.Int(nullable: false),
                        OldStatus = c.String(maxLength: 2),
                        NewStatus = c.String(maxLength: 2),
                        CreateDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AttendanceRecords",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        LearningId = c.Int(nullable: false),
                        EventId = c.Guid(nullable: false),
                        StudId = c.Int(nullable: false),
                        Status = c.String(maxLength: 2),
                        UpdateTime = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Calendars",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(),
                        Content = c.String(),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(nullable: false),
                        PublishDate = c.DateTime(nullable: false),
                        OrgId = c.Int(),
                        IsBigEvent = c.Boolean(nullable: false),
                        Code = c.String(),
                        FileId = c.Int(),
                        CreateDate = c.DateTime(),
                        Creator = c.Int(nullable: false),
                        UpdateDate = c.DateTime(),
                        Updater = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.CalendarDepts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CalendarId = c.Int(nullable: false),
                        DeptId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.CalendarOrganizationRoles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CalendarId = c.Int(nullable: false),
                        OrganizationRoleId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.CalendarSemesters",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CalendarId = c.Int(nullable: false),
                        SemesterId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.CircleMembers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CircleId = c.Int(nullable: false),
                        MemberId = c.Int(nullable: false),
                        DeptId = c.Int(),
                        MemberGrade = c.String(),
                        MemberGroup = c.String(),
                        ExternalRid = c.Int(),
                        Enabled = c.Boolean(nullable: false),
                        UpdateReason = c.String(),
                        MemberInfo = c.String(),
                        CreateUser = c.Int(),
                        UpdateUser = c.Int(),
                        DeleteUser = c.Int(),
                        Created_Utc = c.DateTime(),
                        Updated_Utc = c.DateTime(),
                        Deleted_Utc = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.CircleMemberRoleplays",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CircleId = c.Int(nullable: false),
                        MemberId = c.Int(nullable: false),
                        RoleId = c.Int(nullable: false),
                        Enable = c.Boolean(nullable: false),
                        ExternalRid = c.Int(),
                        ResType = c.Int(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Courses",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 100),
                        DeptId = c.Int(),
                        CourseOutline = c.String(),
                        CourseCode = c.String(maxLength: 100),
                        ExternalRid = c.Int(),
                        CreateUser = c.Int(),
                        UpdateUser = c.Int(),
                        DeleteUser = c.Int(),
                        Created_Utc = c.DateTime(),
                        Updated_Utc = c.DateTime(),
                        Deleted_Utc = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Depts", t => t.DeptId)
                .Index(t => t.DeptId);
            
            CreateTable(
                "dbo.Depts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 150),
                        ShortName = c.String(),
                        OrgId = c.Int(),
                        ParentId = c.Int(),
                        DeptCode = c.String(maxLength: 50),
                        Enable = c.Boolean(nullable: false),
                        Visibility = c.Boolean(nullable: false),
                        CreateUser = c.Int(),
                        UpdateUser = c.Int(),
                        DeleteUser = c.Int(),
                        Created_Utc = c.DateTime(),
                        Updated_Utc = c.DateTime(),
                        Deleted_Utc = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Depts", t => t.ParentId)
                .Index(t => t.ParentId);
            
            CreateTable(
                "dbo.DepartmentAdmins",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        MemberId = c.Int(nullable: false),
                        DeptId = c.Int(nullable: false),
                        Enable = c.Boolean(nullable: false),
                        Visibility = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DiscussionFiles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DiscussionId = c.Int(nullable: false),
                        MessageId = c.Int(),
                        FileId = c.Int(nullable: false),
                        CreateUtcDate = c.DateTime(nullable: false),
                        Creator = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ExtensionColumns",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 100),
                        DisplayName = c.String(),
                        OrgId = c.Int(nullable: false),
                        EditorMultiLine = c.Int(nullable: false),
                        DisplayMultiLine = c.Boolean(nullable: false),
                        EditorMaxLength = c.Int(nullable: false),
                        HelpLink = c.String(),
                        HelpText = c.String(),
                        Editable = c.Boolean(nullable: false),
                        Sort = c.Int(nullable: false),
                        Enable = c.Boolean(nullable: false),
                        Visibility = c.Boolean(nullable: false),
                        CreateUser = c.Int(),
                        UpdateUser = c.Int(),
                        DeleteUser = c.Int(),
                        Created_Utc = c.DateTime(),
                        Updated_Utc = c.DateTime(),
                        Deleted_Utc = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ExtensionColumnCustomizations",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DataId = c.Int(nullable: false),
                        ColumnId = c.Int(nullable: false),
                        Display = c.Boolean(nullable: false),
                        Sort = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ExtensionValues",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TextValue = c.String(),
                        ColumnId = c.Int(nullable: false),
                        DataId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ExternalResources",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        OrgId = c.Int(),
                        ExternalResTypeId = c.Int(),
                        Uri = c.String(),
                        UriPath = c.String(),
                        Enable = c.Boolean(nullable: false),
                        Status = c.Boolean(nullable: false),
                        LastModifyUtc = c.DateTime(),
                        CreateUser = c.Int(),
                        UpdateUser = c.Int(),
                        DeleteUser = c.Int(),
                        Created_Utc = c.DateTime(),
                        Updated_Utc = c.DateTime(),
                        Deleted_Utc = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ExternalResTypes", t => t.ExternalResTypeId)
                .Index(t => t.ExternalResTypeId);
            
            CreateTable(
                "dbo.ExternalResTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        AsyncTypeCode = c.String(),
                        Sort = c.Int(),
                        CreateUser = c.Int(),
                        UpdateUser = c.Int(),
                        DeleteUser = c.Int(),
                        Created_Utc = c.DateTime(),
                        Updated_Utc = c.DateTime(),
                        Deleted_Utc = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.FeedBacks",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Status = c.Int(nullable: false),
                        Note = c.String(),
                        ReContent = c.String(),
                        FeedBackType = c.String(),
                        Description = c.String(),
                        System = c.String(),
                        CreateTime = c.DateTime(nullable: false),
                        Email = c.String(),
                        Creator = c.Int(nullable: false),
                        OrgId = c.Int(nullable: false),
                        Enable = c.Boolean(nullable: false),
                        UpdateTime = c.DateTime(),
                        Updater = c.Int(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.FileStorages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        FileGuid = c.Guid(nullable: false),
                        FileSize = c.Int(nullable: false),
                        FileContentType = c.String(),
                        FileImageWidth = c.Int(),
                        FileImageHeight = c.Int(),
                        Creator = c.Int(nullable: false),
                        CreateUtcDate = c.DateTime(nullable: false),
                        FileUrl = c.String(),
                        DeleteUtcDate = c.DateTime(),
                        Deleter = c.Int(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.GoogleFiles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FileId = c.String(),
                        ParentFileId = c.String(),
                        Name = c.String(),
                        ImgUrl = c.String(),
                        WebViewUrl = c.String(),
                        DownLoadUrl = c.String(),
                        FileType = c.String(),
                        Size = c.Long(),
                        Create_User = c.Int(nullable: false),
                        Create_Utc = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.LCExtensionValues",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TextValue = c.String(),
                        ColumnId = c.Int(nullable: false),
                        DataId = c.Int(nullable: false),
                        ExternalRid = c.Int(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.LearningAuths",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        LearningRoleId = c.Int(nullable: false),
                        FunctionId = c.Int(nullable: false),
                        Enable = c.Boolean(nullable: false),
                        CreateUser = c.Int(),
                        UpdateUser = c.Int(),
                        DeleteUser = c.Int(),
                        Created_Utc = c.DateTime(),
                        Updated_Utc = c.DateTime(),
                        Deleted_Utc = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.LearningCircles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 50),
                        ModuleName = c.String(),
                        Section = c.String(maxLength: 10),
                        CourseId = c.Int(),
                        LCType = c.Int(nullable: false),
                        GoogleDriveFielUrl = c.String(),
                        LearningOuterKey = c.String(maxLength: 50),
                        Description = c.String(),
                        Enable = c.Boolean(nullable: false),
                        ExternalRid = c.Int(),
                        Visibility = c.Boolean(nullable: false),
                        StartDate = c.DateTime(),
                        EndDate = c.DateTime(),
                        OrgId = c.Int(),
                        Objective = c.String(),
                        ReMark = c.String(),
                        InviteEnable = c.Boolean(nullable: false),
                        AdminInviteEnable = c.Boolean(nullable: false),
                        CreateUser = c.Int(),
                        UpdateUser = c.Int(),
                        DeleteUser = c.Int(),
                        Created_Utc = c.DateTime(),
                        Updated_Utc = c.DateTime(),
                        Deleted_Utc = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Courses", t => t.CourseId)
                .Index(t => t.CourseId);
            
            CreateTable(
                "dbo.LearningCircleManagers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        MemberId = c.Int(nullable: false),
                        LearningCircleId = c.Int(nullable: false),
                        Enable = c.Boolean(nullable: false),
                        CreateUtcDate = c.DateTime(nullable: false),
                        Creator = c.Int(nullable: false),
                        ResType = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.LearningCircleModuleManages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CircleKey = c.String(),
                        ModuleKey = c.String(),
                        Enabled = c.Boolean(nullable: false),
                        Visibility = c.Boolean(nullable: false),
                        CreateUser = c.Int(),
                        UpdateUser = c.Int(),
                        DeleteUser = c.Int(),
                        Created_Utc = c.DateTime(),
                        Updated_Utc = c.DateTime(),
                        Deleted_Utc = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.LearningRoles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 50),
                        LearningId = c.Int(nullable: false),
                        Enable = c.Boolean(nullable: false),
                        RoleType = c.String(maxLength: 2),
                        IsFixed = c.Boolean(nullable: false),
                        IsAdminRole = c.Boolean(nullable: false),
                        Ican5Memo = c.String(),
                        ExternalRid = c.Int(),
                        Sort = c.Int(),
                        Level = c.Int(),
                        CreateUser = c.Int(),
                        UpdateUser = c.Int(),
                        DeleteUser = c.Int(),
                        Created_Utc = c.DateTime(),
                        Updated_Utc = c.DateTime(),
                        Deleted_Utc = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.LearningTemplateRoles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Level = c.Int(nullable: false),
                        OrgId = c.Int(nullable: false),
                        RoleKey = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.LeaveCategories",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.LeaveFiles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        LeaveId = c.Int(nullable: false),
                        GoogleId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.LikeLogs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        OuterKey = c.Guid(nullable: false),
                        MemberId = c.Int(nullable: false),
                        IsMsg = c.Boolean(nullable: false),
                        CreateUser = c.Int(),
                        UpdateUser = c.Int(),
                        DeleteUser = c.Int(),
                        Created_Utc = c.DateTime(),
                        Updated_Utc = c.DateTime(),
                        Deleted_Utc = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.MemberInvites",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Code = c.String(),
                        Type = c.Int(nullable: false),
                        CircleKey = c.String(),
                        CreateDate = c.DateTime(nullable: false),
                        InviteEmail = c.String(),
                        IsCourseCode = c.Boolean(nullable: false),
                        Enable = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Members",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 50),
                        OrgId = c.Int(nullable: false),
                        Account = c.String(maxLength: 100),
                        PassWord = c.Binary(),
                        Email = c.String(maxLength: 100),
                        Photo = c.String(maxLength: 500),
                        Enable = c.Boolean(nullable: false),
                        Visibility = c.Boolean(nullable: false),
                        ExternalRid = c.Int(),
                        IsOrgAdmin = c.Boolean(nullable: false),
                        IsShowEmail = c.Boolean(nullable: false),
                        ConnectionId = c.String(),
                        DeptId = c.Int(),
                        Grade = c.Int(),
                        SchoolRoll = c.String(),
                        GraduationStatus = c.Int(),
                        RoleName = c.String(),
                        SemesterGradeId = c.Int(),
                        ClassGrade = c.Int(),
                        IDMainDoma = c.String(),
                        NameMainDoma = c.String(),
                        OrganizationRoleId = c.Int(),
                        Verified = c.Boolean(nullable: false),
                        CreateUser = c.Int(),
                        UpdateUser = c.Int(),
                        DeleteUser = c.Int(),
                        Created_Utc = c.DateTime(),
                        Updated_Utc = c.DateTime(),
                        Deleted_Utc = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ModuleFuntions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 50),
                        ModulesId = c.Int(nullable: false),
                        Enable = c.Boolean(nullable: false),
                        OutSideKey = c.String(maxLength: 100),
                        IsAdminAuth = c.Boolean(nullable: false),
                        IsNormalAuth = c.Boolean(nullable: false),
                        CreateUser = c.Int(),
                        UpdateUser = c.Int(),
                        DeleteUser = c.Int(),
                        Created_Utc = c.DateTime(),
                        Updated_Utc = c.DateTime(),
                        Deleted_Utc = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ModuleGroupCategories",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        GCId = c.Int(nullable: false),
                        EventId = c.Guid(nullable: false),
                        IsActivity = c.Boolean(nullable: false),
                        Enable = c.Boolean(nullable: false),
                        Visibility = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Modules",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 50),
                        ShowOnAddPage = c.Boolean(nullable: false),
                        OnAddPageSort = c.Int(nullable: false),
                        Enable = c.Boolean(nullable: false),
                        OutSideKey = c.String(maxLength: 100),
                        CreateUser = c.Int(),
                        UpdateUser = c.Int(),
                        DeleteUser = c.Int(),
                        Created_Utc = c.DateTime(),
                        Updated_Utc = c.DateTime(),
                        Deleted_Utc = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.OrganizationLoginColumns",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ColumnKey = c.String(),
                        Method = c.String(),
                        Type = c.String(),
                        OrgId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.OrganizationRoles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        IsAdmin = c.Boolean(nullable: false),
                        RoleCode = c.String(),
                        OrgId = c.Int(nullable: false),
                        Level = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Organizations",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 300),
                        OrgCode = c.String(maxLength: 100),
                        ConnId = c.Int(nullable: false),
                        Visibility = c.Boolean(nullable: false),
                        APIKey = c.String(),
                        SemesterLength = c.Int(),
                        IsOrgRegister = c.Boolean(nullable: false),
                        CreateUser = c.Int(),
                        UpdateUser = c.Int(),
                        DeleteUser = c.Int(),
                        Created_Utc = c.DateTime(),
                        Updated_Utc = c.DateTime(),
                        Deleted_Utc = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.PushAccessTokens",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        token = c.String(),
                        Created_Utc = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.PushDatas",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(),
                        Content = c.String(),
                        SystemId = c.String(),
                        GaEvent = c.String(),
                        CircleKey = c.String(),
                        EventOuterKey = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.PushLogs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PushDataId = c.Int(nullable: false),
                        DeviceId = c.String(),
                        CreateDate = c.DateTime(nullable: false),
                        PublishDate = c.DateTime(),
                        Enable = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Rules",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        RuleKey = c.String(),
                        Customization = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.RuleScores",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        LearningId = c.Int(nullable: false),
                        RuleId = c.Int(nullable: false),
                        Score = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.SchoolRoles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        OrgId = c.Int(nullable: false),
                        RoleKey = c.String(),
                        Level = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Sections",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 10),
                        FullName = c.String(),
                        Serial = c.Int(),
                        Year = c.Int(nullable: false),
                        OrgId = c.Int(),
                        IsNowSeme = c.Boolean(nullable: false),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(nullable: false),
                        CreateUser = c.Int(),
                        UpdateUser = c.Int(),
                        DeleteUser = c.Int(),
                        Created_Utc = c.DateTime(),
                        Updated_Utc = c.DateTime(),
                        Deleted_Utc = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.SemesterGrades",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        GradeYears = c.Int(nullable: false),
                        Code = c.String(),
                        OrgId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.SmartTAJoinGroupLogs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SmartTAName = c.String(),
                        ConnectionId = c.String(),
                        CircleKey = c.String(),
                        CreateUtcDate = c.DateTime(nullable: false),
                        Enabled = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.SqlConnectionStrings",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 50),
                        DBName = c.String(),
                        DBUserName = c.String(),
                        DBPwd = c.String(),
                        CreateUser = c.Int(),
                        UpdateUser = c.Int(),
                        DeleteUser = c.Int(),
                        Created_Utc = c.DateTime(),
                        Updated_Utc = c.DateTime(),
                        Deleted_Utc = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Syllabus",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Note = c.String(),
                        Course_No = c.String(),
                        Enable = c.Boolean(nullable: false),
                        Sort = c.Int(nullable: false),
                        Syll_Date = c.DateTime(nullable: false),
                        Syll_Guid = c.Guid(nullable: false),
                        Visibility = c.Boolean(nullable: false),
                        ExternalRid = c.Int(),
                        CreateUser = c.Int(),
                        UpdateUser = c.Int(),
                        DeleteUser = c.Int(),
                        Created_Utc = c.DateTime(),
                        Updated_Utc = c.DateTime(),
                        Deleted_Utc = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.SyncCourseLogs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CourseCode = c.String(),
                        CreateUser = c.Int(),
                        UpdateUser = c.Int(),
                        DeleteUser = c.Int(),
                        Created_Utc = c.DateTime(),
                        Updated_Utc = c.DateTime(),
                        Deleted_Utc = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.SyncLogs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TableName = c.String(),
                        UpdateDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.SystemErrorLogs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ErrorType = c.Int(nullable: false),
                        Description = c.String(),
                        CreateUtcDate = c.DateTime(nullable: false),
                        IsFix = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.SystemErrorTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        OuterKey = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.SystemRoles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 50),
                        Enable = c.Boolean(nullable: false),
                        RoleType = c.String(),
                        IsSystemManager = c.Boolean(nullable: false),
                        CreateUser = c.Int(),
                        UpdateUser = c.Int(),
                        DeleteUser = c.Int(),
                        Created_Utc = c.DateTime(),
                        Updated_Utc = c.DateTime(),
                        Deleted_Utc = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.TimeTables",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Course_Id = c.Int(),
                        ClassRoomId = c.String(),
                        ClassRoom = c.String(),
                        ClassTime = c.String(),
                        Course_No = c.String(),
                        StartDate = c.DateTime(),
                        EndDate = c.DateTime(),
                        UpdateDate = c.DateTime(),
                        OriginClassRoomID = c.String(),
                        OriginClassRoomName = c.String(),
                        OriginStartTime = c.DateTime(),
                        OriginEndTime = c.DateTime(),
                        ChangeReason = c.Int(),
                        Remark = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.UserTokens",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        MemberId = c.Int(nullable: false),
                        Token = c.String(maxLength: 500),
                        TokenMark = c.Boolean(nullable: false),
                        Enable = c.Boolean(nullable: false),
                        OrgId = c.Int(nullable: false),
                        SimulationMember = c.Int(),
                        RequestSystem = c.String(maxLength: 100),
                        DeviceKey = c.String(maxLength: 128),
                        IsOrgAdmin = c.Boolean(nullable: false),
                        PushToken = c.String(maxLength: 500),
                        CreateUser = c.Int(),
                        UpdateUser = c.Int(),
                        DeleteUser = c.Int(),
                        Created_Utc = c.DateTime(),
                        Updated_Utc = c.DateTime(),
                        Deleted_Utc = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.WeekTables",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Week = c.String(),
                        StartPeriod = c.Int(),
                        EndPeriod = c.Int(),
                        StartTime = c.DateTime(nullable: false),
                        EndTime = c.DateTime(nullable: false),
                        Place = c.String(),
                        ClassRoomId = c.String(),
                        LearningCircleId = c.Int(nullable: false),
                        CreateUtcDate = c.DateTime(nullable: false),
                        Creator = c.Int(nullable: false),
                        UpdateUtcDate = c.DateTime(),
                        Updater = c.Int(),
                        ClassWeekType = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.LearningCircles", "CourseId", "dbo.Courses");
            DropForeignKey("dbo.ExternalResources", "ExternalResTypeId", "dbo.ExternalResTypes");
            DropForeignKey("dbo.Courses", "DeptId", "dbo.Depts");
            DropForeignKey("dbo.Depts", "ParentId", "dbo.Depts");
            DropIndex("dbo.LearningCircles", new[] { "CourseId" });
            DropIndex("dbo.ExternalResources", new[] { "ExternalResTypeId" });
            DropIndex("dbo.Depts", new[] { "ParentId" });
            DropIndex("dbo.Courses", new[] { "DeptId" });
            DropTable("dbo.WeekTables");
            DropTable("dbo.UserTokens");
            DropTable("dbo.TimeTables");
            DropTable("dbo.SystemRoles");
            DropTable("dbo.SystemErrorTypes");
            DropTable("dbo.SystemErrorLogs");
            DropTable("dbo.SyncLogs");
            DropTable("dbo.SyncCourseLogs");
            DropTable("dbo.Syllabus");
            DropTable("dbo.SqlConnectionStrings");
            DropTable("dbo.SmartTAJoinGroupLogs");
            DropTable("dbo.SemesterGrades");
            DropTable("dbo.Sections");
            DropTable("dbo.SchoolRoles");
            DropTable("dbo.RuleScores");
            DropTable("dbo.Rules");
            DropTable("dbo.PushLogs");
            DropTable("dbo.PushDatas");
            DropTable("dbo.PushAccessTokens");
            DropTable("dbo.Organizations");
            DropTable("dbo.OrganizationRoles");
            DropTable("dbo.OrganizationLoginColumns");
            DropTable("dbo.Modules");
            DropTable("dbo.ModuleGroupCategories");
            DropTable("dbo.ModuleFuntions");
            DropTable("dbo.Members");
            DropTable("dbo.MemberInvites");
            DropTable("dbo.LikeLogs");
            DropTable("dbo.LeaveFiles");
            DropTable("dbo.LeaveCategories");
            DropTable("dbo.LearningTemplateRoles");
            DropTable("dbo.LearningRoles");
            DropTable("dbo.LearningCircleModuleManages");
            DropTable("dbo.LearningCircleManagers");
            DropTable("dbo.LearningCircles");
            DropTable("dbo.LearningAuths");
            DropTable("dbo.LCExtensionValues");
            DropTable("dbo.GoogleFiles");
            DropTable("dbo.FileStorages");
            DropTable("dbo.FeedBacks");
            DropTable("dbo.ExternalResTypes");
            DropTable("dbo.ExternalResources");
            DropTable("dbo.ExtensionValues");
            DropTable("dbo.ExtensionColumnCustomizations");
            DropTable("dbo.ExtensionColumns");
            DropTable("dbo.DiscussionFiles");
            DropTable("dbo.DepartmentAdmins");
            DropTable("dbo.Depts");
            DropTable("dbo.Courses");
            DropTable("dbo.CircleMemberRoleplays");
            DropTable("dbo.CircleMembers");
            DropTable("dbo.CalendarSemesters");
            DropTable("dbo.CalendarOrganizationRoles");
            DropTable("dbo.CalendarDepts");
            DropTable("dbo.Calendars");
            DropTable("dbo.AttendanceRecords");
            DropTable("dbo.AttendanceLeaveLogs");
            DropTable("dbo.AttendanceLeaves");
            DropTable("dbo.AppVersions");
            DropTable("dbo.ActVoteItems");
            DropTable("dbo.ActVotes");
            DropTable("dbo.ActRollCallLogs");
            DropTable("dbo.ActRollCalls");
            DropTable("dbo.ActModuleMessages");
            DropTable("dbo.ActMessages");
            DropTable("dbo.ActMaterials");
            DropTable("dbo.ActivitySyllabus");
            DropTable("dbo.ActivitysReadMarks");
            DropTable("dbo.ActivitysNotices");
            DropTable("dbo.Activitys");
            DropTable("dbo.ActHomeWorkScores");
            DropTable("dbo.ActHomeWorkLogs");
            DropTable("dbo.ActHomeWorks");
            DropTable("dbo.ActGroupMembers");
            DropTable("dbo.ActGroupCategories");
            DropTable("dbo.ActGroups");
            DropTable("dbo.ActGenerals");
            DropTable("dbo.ActDiscussionMsgs");
            DropTable("dbo.ActDiscussions");
            DropTable("dbo.ActBulletins");
        }
    }
}
