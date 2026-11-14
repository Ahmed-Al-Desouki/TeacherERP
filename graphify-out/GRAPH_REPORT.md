# Graph Report - TeacherERP  (2026-11-14)

## Corpus Check
- cluster-only mode — file stats not available

## Summary
- 2367 nodes · 8051 edges · 118 communities (79 shown, 39 thin omitted)
- Extraction: 92% EXTRACTED · 8% INFERRED · 0% AMBIGUOUS · INFERRED: 651 edges (avg confidence: 0.85)
- Token cost: 0 input · 0 output

## Graph Freshness
- Built from commit: `56446b04`
- Run `git rev-parse HEAD` and compare to check if the graph is stale.
- Run `graphify update .` after code changes (no API cost).

## Community Hubs (Navigation)
- TeacherERP.Domain.Shared
- IActionResult
- Task
- TeacherERP.Application.Interfaces
- system
- TimeSpan
- system_text
- Guid
- CreateAssistantCommandHandler
- .AddApplicationServices
- Result
- Session
- List
- DateTime
- .AddInfrastructureServices
- Group
- ApplicationUser
- IGroupSessionRepository
- TeacherERP.API
- AttendanceRecord
- AppDbContext
- Center
- LoginSession
- .Handle
- http
- AttendanceController.cs
- .SeedAsync
- ICurrentTenantAccessor
- IUserAccountService
- Transaction
- GroupDto
- PasswordController.cs
- MembershipStatus
- OtpService
- .SaveChangesAsync
- TeacherERP.Application.Features.Groups
- GuardianRelation
- .GetByIdAsync
- ITenantRepository
- EnrollmentRequestStatus
- GroupEnrollment
- AbstractValidator
- TeacherERP.Application.Features.Users.Commands.Auth.Login
- fluentvalidation
- AttendanceResultDto
- TenantSettings
- .Handle
- AuthResponseDto
- Error
- AuditLog
- BaseEntity
- GroupSessionRepository
- SessionValidator
- .ResolveAsync
- ResetGrantService
- .HandleAsync
- .AddApiRateLimiting
- TenantRole
- IServiceCollection
- ICenterRepository
- .ValidateIdTokenAsync
- CompleteProfileValidator
- SessionManager
- TeacherERP.Application.Features.Team
- GetSessionsQueryHandler
- SetFinancialDelegationCommandHandler.cs
- RevokeSessionCommand
- CancelSessionCommandHandler.cs
- HangfireBasicAuthFilter
- GetMySessionsQueryHandler
- Centerdto.cs
- TeacherERP.Application.Features.Students
- LoggingBehavior.cs
- GeoDistance
- CachedRegistrationData
- .TryHandleAsync
- UpdateGroupValidator
- .RequiresProfileCompletion
- Updatecentervalidator.cs
- AddExtraSessionValidator.cs
- BulkCancelSessionsValidator.cs
- BulkShiftSessionsValidator.cs
- RescheduleSessionValidator.cs
- BlacklistStudentValidator.cs
- UnblacklistStudentValidator.cs
- CreateAssistantValidator.cs
- RemoveAssistantValidator.cs
- ResendWelcomeValidator.cs
- SetFinancialDelegationValidator.cs
- GoogleSignInValidator.cs
- ResetPasswordValidator.cs
- VerifyResetOtpValidator.cs
- CreateStudentValidator.cs
- VerifyOtpValidator.cs
- ProfileAuthorizationResultHandler.cs
- IdentityService.cs
- TeacherERP.Application.Features.Schedule.GetSessions
- UpdateGroupValidator
- ManualAttendanceValidator.cs
- Updatecentervalidator.cs
- RejectRequestValidator.cs
- RequestGroupValidator.cs
- GroupScanValidator.cs
- RequestGroupAdminChangeValidator.cs
- CancelSessionValidator.cs
- LogoutValidator.cs
- BlacklistStudentValidator.cs
- CreateAssistantValidator.cs
- RemoveAssistantValidator.cs
- ResendWelcomeValidator.cs
- Updatetenantprofilevalidator.cs
- ChangePasswordValidator.cs
- GoogleSignInValidator.cs
- RefreshTokenValidator.cs
- CreateStudentValidator.cs

## God Nodes (most connected - your core abstractions)
1. `Result` - 244 edges
2. `TeacherERP.Application.Interfaces` - 185 edges
3. `TeacherERP.Domain.Shared` - 162 edges
4. `TeacherERP.Domain.Entities` - 143 edges
5. `AppDbContext` - 78 edges
6. `ApplicationUser` - 76 edges
7. `ICurrentTenantAccessor` - 46 edges
8. `TenantMember` - 45 edges
9. `Session` - 42 edges
10. `TeacherERP.Infrastructure.Data` - 39 edges

## Surprising Connections (you probably didn't know these)
- `CenterErrors` --references--> `Error`  [EXTRACTED]
  TeacherERP.Application/Features/Centers/Centererrors.cs → TeacherERP.Domain/Shared/Error.cs
- `EnrollmentErrors` --references--> `Error`  [EXTRACTED]
  TeacherERP.Application/Features/Enrollment/EnrollmentErrors.cs → TeacherERP.Domain/Shared/Error.cs
- `GroupErrors` --references--> `Error`  [EXTRACTED]
  TeacherERP.Application/Features/Groups/GroupErrors.cs → TeacherERP.Domain/Shared/Error.cs
- `TeamErrors` --references--> `Error`  [EXTRACTED]
  TeacherERP.Application/Features/Team/TeamErrors.cs → TeacherERP.Domain/Shared/Error.cs
- `AppDbContext` --references--> `AttendanceRecord`  [EXTRACTED]
  TeacherERP.Infrastructure/Data/AppDbContext.cs → TeacherERP.Domain/Entities/AttendanceRecord.cs

## Import Cycles
- None detected.

## Communities (118 total, 39 thin omitted)

### Community 0 - "TeacherERP.Domain.Shared"
Cohesion: 0.03
Nodes (42): TeacherERP.Application.Features.Attendance.RejectCompensation, TeacherERP.Application.Features.Team.GetTeam, TeacherERP.Application.Features.Profile.GetMyProfile, TeacherERP.Application.Features.Groups.GetGroup, TeacherERP.Application.Features.Schedule.RestoreSession, TeacherERP.Application.Features.Users.Commands.PromoteToSuperAdmin, TeacherERP.Application.Features.Users.Commands.Auth.ChangePassword, TeacherERP.Application.Features.Students.UnblacklistStudent (+34 more)

### Community 1 - "IActionResult"
Cohesion: 0.07
Nodes (19): AttendanceController, AuditController, AuthController, BaseApiController, CentersController, DirectoryController, EnrollmentController, GroupAdminsController (+11 more)

### Community 2 - "Task"
Cohesion: 0.06
Nodes (9): ISessionRepository, AttendanceRepository, CenterRepository, GroupAdminRepository, GroupRepository, SessionRepository, TeamRepository, CredentialService (+1 more)

### Community 3 - "TeacherERP.Application.Interfaces"
Cohesion: 0.06
Nodes (20): NotificationService, UpdateGroupRequest, GetMyNotificationsQuery, GetMyNotificationsQueryHandler, RunningSessionInfo, AuditLogItem, EnrollmentRequestListItem, GroupJoinInfo (+12 more)

### Community 4 - "system"
Cohesion: 0.04
Nodes (10): TeacherERP.API.Middlewares, TeacherERP.Application, TeacherERP.Infrastructure.Settings, TeacherERP.Infrastructure, TeacherERP.Application.Sessions, TeacherERP.Application.Features.Users.Commands.Auth.ForgotPassword, TeacherERP.Infrastructure.Services, DependencyInjection (+2 more)

### Community 5 - "TimeSpan"
Cohesion: 0.06
Nodes (13): TeacherERP.Application.Enrollment, TeacherERP.Application.Absence, TeacherERP.Application.Features.Tenants, TeacherERP.Application.Features.Attendance.AdminScan, TeacherERP.Application.Features.Attendance.GetPendingCompensations, TeacherERP.Domain.Entities, TeacherERP.Application.Features.Enrollment.GetRequests, TeacherERP.Application.GroupAdmins (+5 more)

### Community 6 - "system_text"
Cohesion: 0.06
Nodes (15): SessionPolicy, InitialCreate, FixLoginSessionUserId, AddStudentProfile, AddTenants, MoveCenterStateToMembership, AddCentersAndTeacherSubject, AddGroupScheduleAndSessions (+7 more)

### Community 7 - "Guid"
Cohesion: 0.05
Nodes (21): TeacherERP.Application.Features.Attendance, TeacherERP.Application.Features.GroupAdmins.GetGroupAdminRequests, TeacherERP.Application.Features.Enrollment.ApproveRequest, TeacherERP.Application.Features.GroupAdmins.GetMyGroupAdmin, TeacherERP.Application.Features.Attendance.UpdateAttendanceSettings, TeacherERP.Application.Attendance, TeacherERP.Application.Features.GroupAdmins.GroupScan, TeacherERP.Application.Features.Attendance.GetAttendanceSettings (+13 more)

### Community 8 - "CreateAssistantCommandHandler"
Cohesion: 0.12
Nodes (4): TeacherERP.Infrastructure.Data, TeacherERP.Infrastructure.Extensions, TeacherERP.Infrastructure.Migrations, TeacherERP.Infrastructure.Repositories

### Community 9 - ".AddApplicationServices"
Cohesion: 0.08
Nodes (44): DisplayCodeDto, GetAttendanceSettingsQuery, GetDisplayCodeQuery, GetDisplayCodeQueryHandler, GetSessionAttendanceQuery, GetSessionAttendanceQueryHandler, RegenerateStaticCodeCommand, GetAuditLogQuery (+36 more)

### Community 10 - "Result"
Cohesion: 0.07
Nodes (9): TeacherERP.Application.Features.Students, TeacherERP.Application.Features.Team.CreateAssistant, TeacherERP.API.Security, TeacherERP.Application.Features.Team.SetAssistantSuspension, TeacherERP.Application.Features.Groups.CreateGroup, TeacherERP.API.Controllers, TeacherERP.API.Extensions, RateLimitPolicies (+1 more)

### Community 11 - "Session"
Cohesion: 0.07
Nodes (13): UserTime, IEmailService, IResetGrantService, ResetGrant, EmailService, EmailTemplates, ResetGrantService, MailSettings (+5 more)

### Community 13 - "List"
Cohesion: 0.07
Nodes (21): AppDbContext, AttendanceRecords, AuditLogs, Centers, CenterTeachers, CurrentTenantId, EnrollmentRequests, GroupAdminRequests (+13 more)

### Community 14 - "DateTime"
Cohesion: 0.12
Nodes (22): AbsenceProcessor, AbsenceService, EnrollmentService, ApproveCompensationCommand, ApproveCompensationCommandHandler, RejectCompensationCommand, RejectCompensationCommandHandler, RequestGroupCommand (+14 more)

### Community 15 - ".AddInfrastructureServices"
Cohesion: 0.12
Nodes (21): IPagedRequest, Page, PageSize, PagedResult, HasNext, HasPrevious, TotalPages, PageRequest (+13 more)

### Community 16 - "Group"
Cohesion: 0.14
Nodes (17): ActivateAccountCommand, ActivateAccountCommandHandler, GoogleSignInCommand, GoogleSignInCommandHandler, AuthResponseDto, LoginCommand, LoginCommandHandler, RefreshTokenCommand (+9 more)

### Community 17 - "ApplicationUser"
Cohesion: 0.08
Nodes (11): TenantMember, CanManageFinance, ConsecutiveAbsences, CurrentBalance, HasFinancialDelegation, Role, Status, Tenant (+3 more)

### Community 19 - "TeacherERP.API"
Cohesion: 0.17
Nodes (16): ApproveGroupAdminRequestCommand, ApproveGroupAdminRequestCommandHandler, GetGroupAdminsQuery, GetGroupAdminsQueryHandler, GetMyGroupAdminQuery, GetMyGroupAdminQueryHandler, GroupAdminChangeResultDto, GroupAdminDto (+8 more)

### Community 20 - "AttendanceRecord"
Cohesion: 0.18
Nodes (13): AdminScanCommand, AdminScanCommandHandler, ManualAttendanceCommand, ManualAttendanceCommandHandler, ScanAttendanceCommand, ScanAttendanceCommandHandler, GroupScanCommand, GroupScanCommandHandler (+5 more)

### Community 21 - "AppDbContext"
Cohesion: 0.09
Nodes (28): FluentValidation (12.1.1), FluentValidation.DependencyInjectionExtensions (12.1.1), Google.Apis.Auth (1.77.0), Hangfire.AspNetCore (1.8.25), Hangfire.Core (1.8.25), Hangfire.Dashboard.BasicAuthorization (1.0.2), Hangfire.SqlServer (1.8.25), MediatR (14.2.0) (+20 more)

### Community 22 - "Center"
Cohesion: 0.08
Nodes (21): LoginSession, AbsoluteExpiresAt, DeviceId, DeviceName, LastRefreshedAt, PreviousRefreshTokenHash, RefreshExpiresAt, RefreshTokenHash (+13 more)

### Community 23 - "LoginSession"
Cohesion: 0.17
Nodes (19): CreateGroupCommand, CreateGroupCommandHandler, GetGroupQuery, GetGroupQueryHandler, GetGroupsQuery, GetGroupsQueryHandler, GroupDto, GroupSlotDto (+11 more)

### Community 24 - ".Handle"
Cohesion: 0.09
Nodes (20): EnrollmentRequest, DecidedAt, DecidedByUserId, DecisionNote, Group, GroupId, SourceSessionId, Status (+12 more)

### Community 25 - "http"
Cohesion: 0.19
Nodes (15): AddExtraSessionCommand, AddExtraSessionCommandHandler, BulkCancelSessionsCommand, BulkCancelSessionsCommandHandler, BulkShiftSessionsCommand, BulkShiftSessionsCommandHandler, CancelSessionCommand, CancelSessionCommandHandler (+7 more)

### Community 26 - "AttendanceController.cs"
Cohesion: 0.10
Nodes (15): IExternalLoginService, ApplicationUser, Attendances, DateOfBirth, FullName, Gender, Guardian, IsProfileComplete (+7 more)

### Community 27 - ".SeedAsync"
Cohesion: 0.12
Nodes (17): CachedRegistrationData, Email, FullName, Password, OtpIssueResult, OtpPurpose, Activation, PasswordReset (+9 more)

### Community 28 - "ICurrentTenantAccessor"
Cohesion: 0.08
Nodes (21): ITenantOwned, TenantId, PaymentType, DebtSettlement, MonthlySubscription, PerSession, Transaction, Amount (+13 more)

### Community 29 - "IUserAccountService"
Cohesion: 0.17
Nodes (15): TeacherERP.Application.Features.Directory, DirectoryCenterDto, DirectoryCenterInfoDto, DirectoryGroupDto, DirectoryTeacherDto, TeacherProfileDto, GetTeacherProfileQuery, GetTeacherProfileQueryHandler (+7 more)

### Community 30 - "Transaction"
Cohesion: 0.11
Nodes (14): Session, AbsencesProcessedAt, Attendances, BoardImageUrl, CancelReason, EndTime, Group, GroupId (+6 more)

### Community 31 - "GroupDto"
Cohesion: 0.09
Nodes (23): ASPNETCORE_ENVIRONMENT, applicationUrl, commandName, dotnetRunMessages, environmentVariables, applicationUrl, commandName, dotnetRunMessages (+15 more)

### Community 32 - "PasswordController.cs"
Cohesion: 0.17
Nodes (8): CancelRequestCommand, CancelRequestCommandHandler, RejectRequestCommand, RejectRequestCommandHandler, ISessionValidator, SessionManager, TokenHasher, SessionValidator

### Community 33 - "MembershipStatus"
Cohesion: 0.19
Nodes (4): AttendanceCodeService, RescheduleSessionRequest, IAttendanceCodeService, ReplaceRefreshTokensWithLoginSessions

### Community 34 - "OtpService"
Cohesion: 0.10
Nodes (15): EnrollmentStatus, Active, Ended, GroupEnrollment, ApprovedByUserId, EndedAt, EnrolledAt, Group (+7 more)

### Community 35 - ".SaveChangesAsync"
Cohesion: 0.12
Nodes (8): TeacherERP.Application.Features.Schedule, ApiAuthorizationResultHandler, ScheduleErrors, StudentErrors, TenantErrors, Error, Code, Message

### Community 36 - "TeacherERP.Application.Features.Groups"
Cohesion: 0.10
Nodes (10): AdminScanValidator, ScanAttendanceValidator, ApproveGroupAdminRequestValidator, AddExtraSessionValidator, RescheduleSessionValidator, SetFinancialDelegationValidator, ActivateAccountValidator, ForgotPasswordValidator (+2 more)

### Community 38 - ".GetByIdAsync"
Cohesion: 0.11
Nodes (15): GroupAdminRequest, DecidedAt, DecidedByUserId, DecisionNote, Group, GroupId, RequestedByUserId, Status (+7 more)

### Community 39 - "ITenantRepository"
Cohesion: 0.13
Nodes (7): TeacherERP.Application.Features.Users.Commands.Auth.GoogleSignIn, TeacherERP.Application.Features.Users.Commands.Auth.Login, TeacherERP.Application.Features.Profile.CompleteProfile, TeacherERP.Application.Features.Users.Commands.Auth.ActivateAccount, TeacherERP.Application.Features.Profile, TeacherERP.Domain.ValueObjects, TeacherERP.Application.Features.Users.Commands.Auth.RefreshToken

### Community 40 - "EnrollmentRequestStatus"
Cohesion: 0.13
Nodes (9): AbsenceProcessingJob, IAbsenceProcessingJob, IAbsenceProcessor, TenantContext, ISessionGenerationJob, SessionGenerationJob, CurrentTenantAccessor, Current (+1 more)

### Community 41 - "GroupEnrollment"
Cohesion: 0.11
Nodes (18): SessionAttachment, Caption, ContentType, FileName, Kind, PublicId, Session, SessionId (+10 more)

### Community 42 - "AbstractValidator"
Cohesion: 0.11
Nodes (10): UpdateAttendanceSettingsValidator, RejectGroupAdminRequestValidator, BulkCancelSessionsValidator, BulkShiftSessionsValidator, RevokeSessionValidator, UnblacklistStudentValidator, UnsuspendStudentValidator, SetAssistantSuspensionValidator (+2 more)

### Community 43 - "TeacherERP.Application.Features.Users.Commands.Auth.Login"
Cohesion: 0.13
Nodes (11): BaseEntity, CreatedAt, Id, UpdatedAt, GroupScheduleSlot, Day, DurationMinutes, Group (+3 more)

### Community 44 - "fluentvalidation"
Cohesion: 0.12
Nodes (11): Group, Capacity, CenterId, GradeLevel, IsActive, Name, Room, Sessions (+3 more)

### Community 46 - "TenantSettings"
Cohesion: 0.19
Nodes (13): GetStudentsQuery, GetStudentsQueryHandler, StudentListDto, GetTeamQuery, GetTeamQueryHandler, TeamMemberDto, IStudentRepository, StudentListItem (+5 more)

### Community 47 - ".Handle"
Cohesion: 0.12
Nodes (9): TeacherERP.Application.Features.Groups, CreateGroupValidator, GroupErrors, GroupSlotInput, GroupSlotsRules, IGroupSchedule, Slots, UpdateGroupScheduleRequest (+1 more)

### Community 48 - "AuthResponseDto"
Cohesion: 0.13
Nodes (14): CenterTeacher, Center, CenterId, Origin, Status, TenantId, CenterTeacherOrigin, Founder (+6 more)

### Community 49 - "Error"
Cohesion: 0.12
Nodes (15): TeacherERP.Application.Features.Audit, AuditLogDto, AuditAction, AttendanceCorrected, GroupAdminAppointed, GroupAdminAttendanceRecorded, GroupAdminRemoved, SessionAttachmentDeleted (+7 more)

### Community 50 - "AuditLog"
Cohesion: 0.20
Nodes (13): CompleteProfileCommand, CompleteProfileCommandHandler, GetMyProfileQuery, GetMyProfileQueryHandler, ProfileCompletionChecker, GuardianDto, ProfileDto, ResendWelcomeCommand (+5 more)

### Community 51 - "BaseEntity"
Cohesion: 0.22
Nodes (9): AssistantDto, CreateAssistantCommand, CreateAssistantCommandHandler, WelcomeEmailFailedException, ITeamRepository, TenantRole, Assistant, Owner (+1 more)

### Community 52 - "GroupSessionRepository"
Cohesion: 0.12
Nodes (14): GuardianInfo, Name, PhoneNumber, Relation, GuardianRelation, Aunt, Brother, Father (+6 more)

### Community 53 - "SessionValidator"
Cohesion: 0.18
Nodes (7): CompletedProfileHandler, CompletedProfileRequirement, TenantHandler, TenantRequirement, AllowedRoles, ProfileRequirement, IProfileCompletionChecker

### Community 54 - ".ResolveAsync"
Cohesion: 0.23
Nodes (3): UpdateGroupScheduleCommand, SessionTimeCalculator, AppDbContextModelSnapshot

### Community 55 - "ResetGrantService"
Cohesion: 0.22
Nodes (11): GetPendingCompensationsQuery, GetPendingCompensationsQueryHandler, GetCurrentTenantQuery, GetCurrentTenantQueryHandler, GetMyTenantsQuery, GetMyTenantsQueryHandler, TenantDto, PendingCompensationItem (+3 more)

### Community 56 - ".HandleAsync"
Cohesion: 0.12
Nodes (12): AttendanceRecord, CompensatesSessionId, RecordedByUserId, ScannedLatitude, ScannedLongitude, ScannedVia, Session, SessionId (+4 more)

### Community 57 - ".AddApiRateLimiting"
Cohesion: 0.16
Nodes (11): AttendanceMode, AdminOnly, DynamicScreenQR, StaticGlobalQR, TenantSettings, AutoApproveGroupAdminScans, AutoRegisterOnScan, Mode (+3 more)

### Community 58 - "TenantRole"
Cohesion: 0.14
Nodes (13): Notification, IsRead, Message, RecipientUserId, RelatedId, TenantId, Type, NotificationType (+5 more)

### Community 59 - "IServiceCollection"
Cohesion: 0.14
Nodes (12): Tenant, AutoApproveGroupAdminChanges, AutoApproveJoinRequests, IsListedInSearch, Name, OwnerId, Status, Subject (+4 more)

### Community 60 - "ICenterRepository"
Cohesion: 0.16
Nodes (5): TeacherERP.Application.Features.Schedule.BulkCancelSessions, TeacherERP.Application.Scheduling, TeacherERP.Application.Features.Schedule.BulkShiftSessions, TeacherERP.Application.Features.Schedule.RescheduleSession, SchedulePolicy

### Community 61 - ".ValidateIdTokenAsync"
Cohesion: 0.15
Nodes (7): AuditLog, Action, ActorUserId, Reason, TargetUserId, TenantId, AuditRepository

### Community 62 - "CompleteProfileValidator"
Cohesion: 0.15
Nodes (3): TeacherERP.Application.Features.Users.Commands.VerifyOtp, TeacherERP.Application.Features.Users.Commands.Auth.ResetPassword, TeacherERP.Application.Features.Users.Commands.CreateStudent

### Community 63 - "SessionManager"
Cohesion: 0.32
Nodes (5): AttendanceSettingsDto, GetAttendanceSettingsQueryHandler, RegenerateStaticCodeCommandHandler, UpdateAttendanceSettingsCommand, UpdateAttendanceSettingsCommandHandler

### Community 65 - "GetSessionsQueryHandler"
Cohesion: 0.24
Nodes (5): GroupAccessService, GroupAccessLevel, GroupAdmin, Staff, IGroupAccessService

### Community 66 - "SetFinancialDelegationCommandHandler.cs"
Cohesion: 0.21
Nodes (8): AccessTokenResult, IJwtProvider, JwtProvider, JwtSettings, Audience, ExpiryMinutes, Issuer, Secret

### Community 67 - "RevokeSessionCommand"
Cohesion: 0.18
Nodes (7): IUserTimeZoneAccessor, TimeZoneId, CurrentUserService, SessionId, UserId, HttpUserTimeZoneAccessor, TimeZoneId

### Community 68 - "CancelSessionCommandHandler.cs"
Cohesion: 0.18
Nodes (10): AttendanceStatus, Absent, PendingCompensation, Present, Rejected, ScanMethod, AdminScan, ManualEntry (+2 more)

### Community 70 - "GetMySessionsQueryHandler"
Cohesion: 0.20
Nodes (4): GlobalExceptionHandler, WelcomeEmailFailedException, ISessionCleanupJob, SessionCleanupJob

### Community 71 - "Centerdto.cs"
Cohesion: 0.28
Nodes (5): TeacherERP.Application.Features.Enrollment, EnrollmentRequestDto, MyEnrollmentDto, MyEnrollmentsDto, EnrollmentErrors

### Community 72 - "TeacherERP.Application.Features.Students"
Cohesion: 0.31
Nodes (5): GoogleUserInfo, IGoogleAuthService, GoogleAuthService, GoogleAuthSettings, ClientIds

### Community 73 - "LoggingBehavior.cs"
Cohesion: 0.22
Nodes (7): Center, Address, GeofenceRadiusMeters, Latitude, Longitude, Name, OwnerUserId

### Community 74 - "GeoDistance"
Cohesion: 0.32
Nodes (4): TeacherERP.Application.Common, PagedRequestRules, GetGroupAdminRequestsValidator, GetGroupSessionAttendanceValidator

### Community 75 - "CachedRegistrationData"
Cohesion: 0.29
Nodes (3): TeacherERP.Application.Behaviors, LoggingBehavior, ValidationBehavior

### Community 76 - ".TryHandleAsync"
Cohesion: 0.50
Nodes (5): GetTenantSettingsQuery, GetTenantSettingsQueryHandler, TenantSettingsDto, UpdateTenantSettingsCommand, UpdateTenantSettingsCommandHandler

### Community 77 - "UpdateGroupValidator"
Cohesion: 0.54
Nodes (5): CenterDto, GetMyCentersQuery, GetMyCentersQueryHandler, UpdateCenterCommand, UpdateCenterCommandHandler

### Community 78 - ".RequiresProfileCompletion"
Cohesion: 0.48
Nodes (4): TeacherERP.Application.Features.Sessions.GetMySessions, GetMySessionsQuery, GetMySessionsQueryHandler, SessionDto

### Community 80 - "AddExtraSessionValidator.cs"
Cohesion: 0.33
Nodes (3): TeacherERP.Application.Features.Team, TeamErrors, TeamPolicy

### Community 83 - "RescheduleSessionValidator.cs"
Cohesion: 0.60
Nodes (3): GetSessionsQuery, GetSessionsQueryHandler, SessionDto

### Community 84 - "BlacklistStudentValidator.cs"
Cohesion: 0.33
Nodes (6): AttendanceOutcome, AlreadyRecorded, Corrected, PendingApproval, PendingCompensation, Recorded

### Community 90 - "GoogleSignInValidator.cs"
Cohesion: 0.70
Nodes (3): EnrollmentRequestListDto, GetRequestsQuery, GetRequestsQueryHandler

### Community 91 - "ResetPasswordValidator.cs"
Cohesion: 0.40
Nodes (3): Gender, Female, Male

### Community 96 - "IdentityService.cs"
Cohesion: 0.50
Nodes (4): StudentStatus, Active, Blacklisted, Suspended

## Knowledge Gaps
- **350 isolated node(s):** `DisplayName`, `Host`, `Mail`, `Password`, `Port` (+345 more)
  These have ≤1 connection - possible missing edges. (Counts symbols only; 515 node(s) total have ≤1 connection when file, concept and rationale nodes are included.)
- **39 thin communities (<3 nodes) omitted from report** — run `graphify query` to explore isolated nodes.

## Suggested Questions
_Questions this graph is uniquely positioned to answer:_

- **Why does `TeacherERP.Domain.Entities` connect `TimeSpan` to `TeacherERP.Domain.Shared`, `TeacherERP.Application.Interfaces`, `system`, `Guid`, `CreateAssistantCommandHandler`, `Result`, `AttendanceRecord`, `Center`, `LoginSession`, `.Handle`, `http`, `ICurrentTenantAccessor`, `IUserAccountService`, `Transaction`, `OtpService`, `.GetByIdAsync`, `ITenantRepository`, `GroupEnrollment`, `TeacherERP.Application.Features.Users.Commands.Auth.Login`, `fluentvalidation`, `AuthResponseDto`, `Error`, `.AddApiRateLimiting`, `TenantRole`, `IServiceCollection`, `.ValidateIdTokenAsync`, `CompleteProfileValidator`, `CancelSessionCommandHandler.cs`, `Centerdto.cs`, `LoggingBehavior.cs`, `ResetPasswordValidator.cs`, `VerifyResetOtpValidator.cs`?**
  _High betweenness centrality (0.067) - this node is a cross-community bridge._
- **What connects `DisplayName`, `Host`, `Mail` to the rest of the system?**
  _350 weakly-connected nodes found - possible documentation gaps or missing edges._
- **Should `TeacherERP.Domain.Shared` be split into smaller, more focused modules?**
  _Cohesion score 0.031982611395746 - nodes in this community are weakly interconnected._
- **Why does `TeacherERP.Application.Interfaces` connect `TimeSpan` to `TeacherERP.Domain.Shared`, `TeacherERP.Application.Interfaces`, `system`, `Guid`, `CreateAssistantCommandHandler`, `Result`, `Session`, `AttendanceRecord`, `LoginSession`, `IUserAccountService`, `PasswordController.cs`, `ITenantRepository`, `EnrollmentRequestStatus`, `Error`, `SessionValidator`, `ResetGrantService`, `ICenterRepository`, `CompleteProfileValidator`, `GetSessionsQueryHandler`, `RevokeSessionCommand`, `GetMySessionsQueryHandler`, `Centerdto.cs`, `TeacherERP.Application.Features.Students`, `UnblacklistStudentValidator.cs`, `RemoveAssistantValidator.cs`, `ResendWelcomeValidator.cs`, `TeacherERP.Application.Features.Schedule.GetSessions`?**
  _High betweenness centrality (0.064) - this node is a cross-community bridge._
- **Should `IActionResult` be split into smaller, more focused modules?**
  _Cohesion score 0.06885456885456885 - nodes in this community are weakly interconnected._
- **Why does `Result` connect `.AddApplicationServices` to `IActionResult`, `Task`, `TeacherERP.Application.Interfaces`, `system`, `Session`, `DateTime`, `Group`, `AttendanceRecord`, `LoginSession`, `http`, `AttendanceController.cs`, `.SeedAsync`, `IUserAccountService`, `PasswordController.cs`, `.SaveChangesAsync`, `TenantSettings`, `AuditLog`, `BaseEntity`, `.ResolveAsync`, `ResetGrantService`, `TeacherERP.Application.Features.Students`, `UpdateGroupValidator`, `.RequiresProfileCompletion`, `RescheduleSessionValidator.cs`, `GoogleSignInValidator.cs`, `ResetPasswordValidator.cs`?**
  _High betweenness centrality (0.055) - this node is a cross-community bridge._
- **Should `Task` be split into smaller, more focused modules?**
  _Cohesion score 0.05788149164950812 - nodes in this community are weakly interconnected._