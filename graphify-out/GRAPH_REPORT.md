# Graph Report - TeacherERP  (2026-10-30)

## Corpus Check
- cluster-only mode — file stats not available

## Summary
- 2004 nodes · 7052 edges · 97 communities (61 shown, 36 thin omitted)
- Extraction: 92% EXTRACTED · 8% INFERRED · 0% AMBIGUOUS · INFERRED: 557 edges (avg confidence: 0.85)
- Token cost: 0 input · 0 output

## Graph Freshness
- Built from commit: `501c35fe`
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
- IRequestHandler
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

## God Nodes (most connected - your core abstractions)
1. `Result` - 259 edges
2. `TeacherERP.Domain.Shared` - 173 edges
3. `TeacherERP.Application.Interfaces` - 165 edges
4. `TeacherERP.Domain.Entities` - 130 edges
5. `ApplicationUser` - 83 edges
6. `AppDbContext` - 73 edges
7. `ICurrentTenantAccessor` - 53 edges
8. `Session` - 49 edges
9. `TenantMember` - 43 edges
10. `Group` - 40 edges

## Surprising Connections (you probably didn't know these)
- `GroupErrors` --references--> `Error`  [EXTRACTED]
  TeacherERP.Application/Features/Groups/GroupErrors.cs → TeacherERP.Domain/Shared/Error.cs
- `CenterErrors` --references--> `Error`  [EXTRACTED]
  TeacherERP.Application/Features/Centers/Centererrors.cs → TeacherERP.Domain/Shared/Error.cs
- `StudentErrors` --references--> `Error`  [EXTRACTED]
  TeacherERP.Application/Features/Students/StudentErrors.cs → TeacherERP.Domain/Shared/Error.cs
- `TeamErrors` --references--> `Error`  [EXTRACTED]
  TeacherERP.Application/Features/Team/TeamErrors.cs → TeacherERP.Domain/Shared/Error.cs
- `GetRequestsQuery` --references--> `EnrollmentRequestStatus`  [EXTRACTED]
  TeacherERP.Application/Features/Enrollment/GetRequests/GetRequestsQuery.cs → TeacherERP.Domain/Entities/EnrollmentRequest.cs

## Import Cycles
- None detected.

## Communities (97 total, 36 thin omitted)

### Community 0 - "TeacherERP.Domain.Shared"
Cohesion: 0.03
Nodes (52): TeacherERP.Application.Features.Team.GetTeam, TeacherERP.Application.Features.Schedule.BulkCancelSessions, TeacherERP.Application.Features.Enrollment.ApproveRequest, TeacherERP.Application.Features.Profile.GetMyProfile, TeacherERP.Application.Features.Groups.GetGroup, TeacherERP.Application.Features.Schedule.RestoreSession, TeacherERP.Application.Features.Users.Commands.PromoteToSuperAdmin, TeacherERP.Application.Features.Users.Commands.Auth.ChangePassword (+44 more)

### Community 1 - "IActionResult"
Cohesion: 0.08
Nodes (19): AttendanceController, AuditController, AuthController, BaseApiController, CentersController, DirectoryController, EnrollmentController, GroupsController (+11 more)

### Community 2 - "Task"
Cohesion: 0.07
Nodes (5): IAbsenceRepository, AbsenceRepository, GroupRepository, TeamRepository, UserAccountService

### Community 3 - "TeacherERP.Application.Interfaces"
Cohesion: 0.06
Nodes (11): TeacherERP.Application.Scheduling, TeacherERP.Application.Attendance, TeacherERP.Application.Absence, TeacherERP.Application.Features.Audit, TeacherERP.Application.Features.Tenants, TeacherERP.Domain.Entities, TeacherERP.Application.Features.Enrollment, TeacherERP.Application.Interfaces (+3 more)

### Community 4 - "system"
Cohesion: 0.09
Nodes (3): TeacherERP.Infrastructure.Data, TeacherERP.Infrastructure.Migrations, TeacherERP.Infrastructure.Repositories

### Community 5 - "TimeSpan"
Cohesion: 0.06
Nodes (16): UpdateGroupScheduleCommand, SessionPolicy, InitialCreate, ReplaceRefreshTokensWithLoginSessions, FixLoginSessionUserId, AddStudentProfile, AddTenants, MoveCenterStateToMembership (+8 more)

### Community 6 - "system_text"
Cohesion: 0.05
Nodes (11): TeacherERP.Application.Enrollment, TeacherERP.API.Middlewares, TeacherERP.Application, TeacherERP.Infrastructure.Settings, TeacherERP.Infrastructure, TeacherERP.Application.Common, TeacherERP.Application.Features.Profile, TeacherERP.Application.Sessions (+3 more)

### Community 7 - "Guid"
Cohesion: 0.06
Nodes (14): UpdateGroupRequest, AttendanceSettingsInfo, RunningSessionInfo, GroupJoinInfo, MyEnrollmentItem, MyRequestItem, TenantMembershipInfo, TenantStatus (+6 more)

### Community 8 - "CreateAssistantCommandHandler"
Cohesion: 0.08
Nodes (10): UserTime, AssistantDto, CreateAssistantCommand, CreateAssistantCommandHandler, WelcomeEmailFailedException, IEmailService, IPasswordGenerator, EmailService (+2 more)

### Community 9 - ".AddApplicationServices"
Cohesion: 0.06
Nodes (29): GlobalExceptionHandler, AbsenceProcessingJob, AbsenceProcessor, AbsenceService, NotificationService, LoggingBehavior, ValidationBehavior, DependencyInjection (+21 more)

### Community 10 - "Result"
Cohesion: 0.09
Nodes (32): CancelRequestCommand, CancelRequestCommandHandler, GetMyEnrollmentsQuery, GetRequestsQuery, MarkNotificationReadCommand, MarkNotificationReadCommandHandler, LogoutCommand, LogoutCommandHandler (+24 more)

### Community 11 - "Session"
Cohesion: 0.06
Nodes (25): Session, AbsencesProcessedAt, Attendances, BoardImageUrl, CancelReason, EndTime, Group, GroupId (+17 more)

### Community 12 - "IRequestHandler"
Cohesion: 0.11
Nodes (23): AttendanceRecorder, EnrollmentService, ApproveCompensationCommand, ApproveCompensationCommandHandler, ApproveRequestCommand, ApproveRequestCommandHandler, GetMyEnrollmentsQueryHandler, GetRequestsQueryHandler (+15 more)

### Community 13 - "List"
Cohesion: 0.10
Nodes (15): GetPendingCompensationsQuery, GetSessionAttendanceQuery, AuditLogDto, GetAuditLogQuery, GetAuditLogQueryHandler, GetMyNotificationsQuery, GetMyNotificationsQueryHandler, AttendanceListItem (+7 more)

### Community 14 - "DateTime"
Cohesion: 0.11
Nodes (14): AttendanceCodeService, RescheduleSessionRequest, IAttendanceCodeService, EnrollmentRequest, DecidedAt, DecidedByUserId, DecisionNote, Group (+6 more)

### Community 15 - ".AddInfrastructureServices"
Cohesion: 0.08
Nodes (25): IJwtProvider, ISessionRepository, IStudentRepository, IUserTimeZoneAccessor, TimeZoneId, DependencyInjection, SessionRepository, StudentRepository (+17 more)

### Community 16 - "Group"
Cohesion: 0.08
Nodes (18): Group, Capacity, CenterId, GradeLevel, IsActive, Name, Room, Sessions (+10 more)

### Community 17 - "ApplicationUser"
Cohesion: 0.08
Nodes (20): IExternalLoginService, AccessTokenResult, ApplicationUser, Attendances, DateOfBirth, FullName, Gender, Guardian (+12 more)

### Community 18 - "IGroupSessionRepository"
Cohesion: 0.17
Nodes (17): AddExtraSessionCommand, AddExtraSessionCommandHandler, BulkCancelSessionsCommand, BulkCancelSessionsCommandHandler, BulkShiftSessionsCommand, BulkShiftSessionsCommandHandler, CancelSessionCommand, CancelSessionCommandHandler (+9 more)

### Community 19 - "TeacherERP.API"
Cohesion: 0.09
Nodes (28): FluentValidation (12.1.1), FluentValidation.DependencyInjectionExtensions (12.1.1), Google.Apis.Auth (1.77.0), Hangfire.AspNetCore (1.8.25), Hangfire.Core (1.8.25), Hangfire.Dashboard.BasicAuthorization (1.0.2), Hangfire.SqlServer (1.8.25), MediatR (14.2.0) (+20 more)

### Community 20 - "AttendanceRecord"
Cohesion: 0.08
Nodes (22): AttendanceRecord, CompensatesSessionId, RecordedByUserId, ScannedLatitude, ScannedLongitude, ScannedVia, Session, SessionId (+14 more)

### Community 21 - "AppDbContext"
Cohesion: 0.08
Nodes (18): AppDbContext, AttendanceRecords, AuditLogs, Centers, CenterTeachers, CurrentTenantId, EnrollmentRequests, GroupEnrollments (+10 more)

### Community 22 - "Center"
Cohesion: 0.09
Nodes (21): Center, Address, GeofenceRadiusMeters, Latitude, Longitude, Name, OwnerUserId, CenterTeacher (+13 more)

### Community 23 - "LoginSession"
Cohesion: 0.08
Nodes (21): LoginSession, AbsoluteExpiresAt, DeviceId, DeviceName, LastRefreshedAt, PreviousRefreshTokenHash, RefreshExpiresAt, RefreshTokenHash (+13 more)

### Community 24 - ".Handle"
Cohesion: 0.17
Nodes (15): TeacherERP.Application.Features.Directory, DirectoryCenterDto, DirectoryCenterInfoDto, DirectoryGroupDto, DirectoryTeacherDto, TeacherProfileDto, GetTeacherProfileQuery, GetTeacherProfileQueryHandler (+7 more)

### Community 25 - "http"
Cohesion: 0.09
Nodes (23): ASPNETCORE_ENVIRONMENT, applicationUrl, commandName, dotnetRunMessages, environmentVariables, applicationUrl, commandName, dotnetRunMessages (+15 more)

### Community 26 - "AttendanceController.cs"
Cohesion: 0.09
Nodes (10): TeacherERP.Application.Features.Attendance.RejectCompensation, TeacherERP.Application.Features.Attendance.UpdateAttendanceSettings, TeacherERP.Application.Features.Attendance.ScanAttendance, TeacherERP.Application.Features.Attendance.ApproveCompensation, TeacherERP.Application.Features.Attendance.GetAttendanceSettings, TeacherERP.Application.Features.Attendance.GetDisplayCode, TeacherERP.Application.Features.Attendance.AdminScan, TeacherERP.Application.Features.Attendance.GetPendingCompensations (+2 more)

### Community 28 - "ICurrentTenantAccessor"
Cohesion: 0.19
Nodes (16): AttendanceSettingsDto, DisplayCodeDto, GetAttendanceSettingsQuery, GetAttendanceSettingsQueryHandler, GetDisplayCodeQuery, GetDisplayCodeQueryHandler, GetPendingCompensationsQueryHandler, GetSessionAttendanceQueryHandler (+8 more)

### Community 29 - "IUserAccountService"
Cohesion: 0.17
Nodes (12): ActivateAccountCommand, ActivateAccountCommandHandler, ForgotPasswordCommand, ForgotPasswordCommandHandler, ForgotPasswordResponseDto, ResetTokenDto, VerifyResetOtpCommand, VerifyResetOtpCommandHandler (+4 more)

### Community 30 - "Transaction"
Cohesion: 0.10
Nodes (19): PaymentType, DebtSettlement, MonthlySubscription, PerSession, Transaction, Amount, ProcessedByAdminId, ReceiptImageUrl (+11 more)

### Community 31 - "GroupDto"
Cohesion: 0.20
Nodes (13): CreateGroupCommand, CreateGroupCommandHandler, GetGroupQuery, GetGroupQueryHandler, GetGroupsQuery, GetGroupsQueryHandler, GroupDto, GroupSlotDto (+5 more)

### Community 32 - "PasswordController.cs"
Cohesion: 0.12
Nodes (6): TeacherERP.Application.Features.Users.Commands.VerifyOtp, TeacherERP.Application.Features.Users.Commands.Auth.VerifyResetOtp, TeacherERP.Application.Features.Users.Commands.Auth.ResetPassword, TeacherERP.Application.Features.Users.Commands.CreateStudent, TeacherERP.API.Extensions, RateLimitPolicies

### Community 33 - "MembershipStatus"
Cohesion: 0.18
Nodes (12): GetStudentsQuery, GetStudentsQueryHandler, StudentListDto, GetTeamQuery, GetTeamQueryHandler, TeamMemberDto, StudentListItem, TeamMemberItem (+4 more)

### Community 34 - "OtpService"
Cohesion: 0.17
Nodes (13): OtpIssueResult, OtpPurpose, Activation, PasswordReset, Registration, IssueCounter, Count, OtpEntry (+5 more)

### Community 35 - ".SaveChangesAsync"
Cohesion: 0.18
Nodes (10): RejectCompensationCommand, RejectCompensationCommandHandler, RejectRequestCommand, RejectRequestCommandHandler, MarkAllNotificationsReadCommand, MarkAllNotificationsReadCommandHandler, RemoveAssistantCommand, RemoveAssistantCommandHandler (+2 more)

### Community 36 - "TeacherERP.Application.Features.Groups"
Cohesion: 0.12
Nodes (9): TeacherERP.Application.Features.Groups, CreateGroupValidator, GroupErrors, GroupSlotInput, GroupSlotsRules, IGroupSchedule, Slots, UpdateGroupScheduleRequest (+1 more)

### Community 37 - "GuardianRelation"
Cohesion: 0.12
Nodes (15): TeacherERP.Domain.ValueObjects, GuardianInfo, Name, PhoneNumber, Relation, GuardianRelation, Aunt, Brother (+7 more)

### Community 38 - ".GetByIdAsync"
Cohesion: 0.20
Nodes (11): CompleteProfileCommand, CompleteProfileCommandHandler, GetMyProfileQuery, GetMyProfileQueryHandler, GuardianDto, ProfileDto, IUserRepository, Gender (+3 more)

### Community 39 - "ITenantRepository"
Cohesion: 0.20
Nodes (12): GetCurrentTenantQuery, GetCurrentTenantQueryHandler, GetMyTenantsQuery, GetMyTenantsQueryHandler, TenantDto, UpdateTenantProfileCommand, UpdateTenantProfileCommandHandler, TenantSettingsDto (+4 more)

### Community 40 - "EnrollmentRequestStatus"
Cohesion: 0.15
Nodes (12): EnrollmentRequestDto, EnrollmentRequestListDto, MyEnrollmentDto, MyEnrollmentsDto, EnrollmentRequestStatus, Approved, Cancelled, Pending (+4 more)

### Community 41 - "GroupEnrollment"
Cohesion: 0.12
Nodes (13): EnrollmentStatus, Active, Ended, GroupEnrollment, ApprovedByUserId, EndedAt, EnrolledAt, Group (+5 more)

### Community 42 - "AbstractValidator"
Cohesion: 0.12
Nodes (8): AdminScanValidator, ScanAttendanceValidator, RejectRequestValidator, CancelSessionValidator, LogoutValidator, SetAssistantSuspensionValidator, ChangePasswordValidator, RefreshTokenValidator

### Community 43 - "TeacherERP.Application.Features.Users.Commands.Auth.Login"
Cohesion: 0.17
Nodes (5): TeacherERP.Application.Features.Users.Commands.Auth.GoogleSignIn, TeacherERP.Application.Features.Users.Commands.Auth.Login, TeacherERP.Application.Features.Sessions.Logout, TeacherERP.Application.Features.Users.Commands.Auth.ActivateAccount, TeacherERP.Application.Features.Users.Commands.Auth.RefreshToken

### Community 44 - "fluentvalidation"
Cohesion: 0.12
Nodes (7): UpdateAttendanceSettingsValidator, RequestGroupValidator, UnsuspendStudentValidator, UpdateTenantProfileValidator, ActivateAccountValidator, ForgotPasswordValidator, LoginValidator

### Community 45 - "AttendanceResultDto"
Cohesion: 0.25
Nodes (12): AdminScanCommand, AdminScanCommandHandler, ScanAttendanceCommand, ScanAttendanceCommandHandler, AttendanceOutcome, AlreadyRecorded, PendingApproval, PendingCompensation (+4 more)

### Community 46 - "TenantSettings"
Cohesion: 0.15
Nodes (12): ITenantOwned, TenantId, AttendanceMode, AdminOnly, DynamicScreenQR, StaticGlobalQR, TenantSettings, AutoRegisterOnScan (+4 more)

### Community 48 - "AuthResponseDto"
Cohesion: 0.30
Nodes (8): GoogleSignInCommand, GoogleSignInCommandHandler, AuthResponseDto, LoginCommand, LoginCommandHandler, RefreshTokenCommand, RefreshTokenCommandHandler, ISessionManager

### Community 49 - "Error"
Cohesion: 0.16
Nodes (9): TeacherERP.Application.Features.Attendance, TeacherERP.Application.Features.Schedule, AttendanceErrors, EnrollmentErrors, ScheduleErrors, TenantErrors, Error, Code (+1 more)

### Community 50 - "AuditLog"
Cohesion: 0.14
Nodes (13): AuditAction, StudentAutoSuspended, StudentBlacklisted, StudentReinstatedByCompensation, StudentUnblacklisted, StudentUnsuspended, AuditLog, Action (+5 more)

### Community 51 - "BaseEntity"
Cohesion: 0.14
Nodes (12): BaseEntity, CreatedAt, Id, UpdatedAt, Tenant, AutoApproveJoinRequests, IsListedInSearch, Name (+4 more)

### Community 53 - "SessionValidator"
Cohesion: 0.17
Nodes (5): SessionJwtBearerEvents, ProfileCompletionChecker, IProfileCompletionChecker, ISessionValidator, SessionValidator

### Community 54 - ".ResolveAsync"
Cohesion: 0.27
Nodes (5): CompletedProfileHandler, CompletedProfileRequirement, TenantHandler, TenantRequirement, AllowedRoles

### Community 55 - "ResetGrantService"
Cohesion: 0.33
Nodes (3): IResetGrantService, ResetGrant, ResetGrantService

### Community 58 - "TenantRole"
Cohesion: 0.24
Nodes (8): TenantContext, TenantRole, Assistant, Owner, Student, CurrentTenantAccessor, Current, TenantId

### Community 60 - "ICenterRepository"
Cohesion: 0.47
Nodes (6): CenterDto, GetMyCentersQuery, GetMyCentersQueryHandler, UpdateCenterCommand, UpdateCenterCommandHandler, ICenterRepository

### Community 61 - ".ValidateIdTokenAsync"
Cohesion: 0.31
Nodes (5): GoogleUserInfo, IGoogleAuthService, GoogleAuthService, GoogleAuthSettings, ClientIds

### Community 64 - "TeacherERP.Application.Features.Team"
Cohesion: 0.33
Nodes (3): TeacherERP.Application.Features.Team, TeamErrors, TeamPolicy

### Community 65 - "GetSessionsQueryHandler"
Cohesion: 0.60
Nodes (3): GetSessionsQuery, GetSessionsQueryHandler, SessionDto

### Community 67 - "RevokeSessionCommand"
Cohesion: 0.50
Nodes (3): TeacherERP.Application.Features.Sessions.RevokeSession, RevokeSessionCommand, RevokeSessionValidator

### Community 70 - "GetMySessionsQueryHandler"
Cohesion: 0.70
Nodes (3): GetMySessionsQuery, GetMySessionsQueryHandler, SessionDto

### Community 75 - "CachedRegistrationData"
Cohesion: 0.50
Nodes (4): CachedRegistrationData, Email, FullName, Password

## Knowledge Gaps
- **294 isolated node(s):** `Error`, `IsFailure`, `IsSuccess`, `Value`, `AbsencesProcessedAt` (+289 more)
  These have ≤1 connection - possible missing edges. (Counts symbols only; 371 node(s) total have ≤1 connection when file, concept and rationale nodes are included.)
- **36 thin communities (<3 nodes) omitted from report** — run `graphify query` to explore isolated nodes.

## Suggested Questions
_Questions this graph is uniquely positioned to answer:_

- **Why does `Result` connect `Result` to `IActionResult`, `Task`, `TimeSpan`, `system_text`, `Guid`, `CreateAssistantCommandHandler`, `Session`, `IRequestHandler`, `List`, `ApplicationUser`, `IGroupSessionRepository`, `.Handle`, `ICurrentTenantAccessor`, `IUserAccountService`, `GroupDto`, `MembershipStatus`, `OtpService`, `.SaveChangesAsync`, `.GetByIdAsync`, `ITenantRepository`, `AttendanceResultDto`, `.Handle`, `AuthResponseDto`, `Error`, `ICenterRepository`, `.ValidateIdTokenAsync`, `SessionManager`, `GetSessionsQueryHandler`, `RevokeSessionCommand`, `GetMySessionsQueryHandler`?**
  _High betweenness centrality (0.085) - this node is a cross-community bridge._
- **What connects `Error`, `IsFailure`, `IsSuccess` to the rest of the system?**
  _294 weakly-connected nodes found - possible documentation gaps or missing edges._
- **Should `TeacherERP.Domain.Shared` be split into smaller, more focused modules?**
  _Cohesion score 0.027943635060902795 - nodes in this community are weakly interconnected._
- **Why does `TeacherERP.Application.Interfaces` connect `TeacherERP.Application.Interfaces` to `TeacherERP.Domain.Shared`, `system`, `system_text`, `Guid`, `CreateAssistantCommandHandler`, `.AddApplicationServices`, `IRequestHandler`, `List`, `.AddInfrastructureServices`, `.Handle`, `AttendanceController.cs`, `GroupDto`, `PasswordController.cs`, `EnrollmentRequestStatus`, `TeacherERP.Application.Features.Users.Commands.Auth.Login`, `AttendanceResultDto`, `SessionValidator`, `ResetGrantService`, `.ValidateIdTokenAsync`, `SetFinancialDelegationCommandHandler.cs`, `CancelSessionCommandHandler.cs`?**
  _High betweenness centrality (0.070) - this node is a cross-community bridge._
- **Should `IActionResult` be split into smaller, more focused modules?**
  _Cohesion score 0.07595959595959596 - nodes in this community are weakly interconnected._
- **Why does `AppDbContext` connect `AppDbContext` to `Task`, `system`, `TimeSpan`, `Guid`, `.AddApplicationServices`, `Session`, `List`, `DateTime`, `.AddInfrastructureServices`, `Group`, `ApplicationUser`, `AttendanceRecord`, `Center`, `LoginSession`, `.Handle`, `.SeedAsync`, `ICurrentTenantAccessor`, `Transaction`, `.SaveChangesAsync`, `.GetByIdAsync`, `GroupEnrollment`, `TenantSettings`, `AuditLog`, `BaseEntity`, `GroupSessionRepository`, `SessionValidator`?**
  _High betweenness centrality (0.062) - this node is a cross-community bridge._
- **Should `Task` be split into smaller, more focused modules?**
  _Cohesion score 0.0662451068955134 - nodes in this community are weakly interconnected._