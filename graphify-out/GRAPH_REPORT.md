# Graph Report - TeacherERP  (2026-11-21)

## Corpus Check
- cluster-only mode — file stats not available

## Summary
- 4273 nodes · 16155 edges · 152 communities (130 shown, 22 thin omitted)
- Extraction: 90% EXTRACTED · 10% INFERRED · 0% AMBIGUOUS · INFERRED: 1555 edges (avg confidence: 0.85)
- Token cost: 0 input · 0 output

## Graph Freshness
- Built from commit: `025f97ff`
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
- ProfileAuthorizationResultHandler.cs
- IdentityService.cs
- AttendanceOutcome
- MailSettings
- HangfireBasicAuthFilter
- ForwardedHeadersExtensions.cs
- GetNotificationWhatsAppShareQuery
- SessionCleanupJob
- CloudinarySettings
- JwtSettings
- .TokenValidated
- .Meters
- MailSettings
- TeacherERP.Application.Features.Students
- HangfireBasicAuthFilter
- .Resolve
- PreviewTeacherRequestQuery
- .ValidateIdTokenAsync
- GetMyEnrollmentsQuery
- GetMyNotificationsQuery
- ListTeacherRequestsQuery
- EgyptianPhoneNumber.cs
- GroupSlotInput
- AttendanceOutcome
- PaymobWebhookOutcome
- EducationStage
- LogoutCommand
- GetTeacherRequestStatusQuery
- PaymobCallbackVerifier
- SessionCleanupJob
- .BuildTargetModel
- JwtSettings
- .Meters
- ForgotPasswordCommandHandler

## God Nodes (most connected - your core abstractions)
1. `Result` - 412 edges
2. `TeacherERP.Application.Interfaces` - 291 edges
3. `TeacherERP.Domain.Shared` - 276 edges
4. `TeacherERP.Domain.Entities` - 234 edges
5. `AppDbContext` - 112 edges
6. `ApplicationUser` - 89 edges
7. `TeacherERP.Infrastructure.Data` - 60 edges
8. `TeacherERP.Infrastructure.Migrations` - 56 edges
9. `Session` - 52 edges
10. `Transaction` - 49 edges

## Surprising Connections (you probably didn't know these)
- `CancelSessionCommandHandlerTests` --calls--> `TenantContext`  [INFERRED]
  TeacherERP.Tests/Unit/CancelSessionHandlersTests.cs → TeacherERP.Application/Interfaces/Icurrenttenantaccessor.cs
- `CustomWebApplicationFactory` --calls--> `AppDbContext`  [INFERRED]
  TeacherERP.Tests/Integration/CustomWebApplicationFactory.cs → TeacherERP.Infrastructure/Data/AppDbContext.cs
- `BulkCancelSessionsCommandHandlerTests` --calls--> `TenantContext`  [INFERRED]
  TeacherERP.Tests/Unit/CancelSessionHandlersTests.cs → TeacherERP.Application/Interfaces/Icurrenttenantaccessor.cs
- `WalkInApprovalTests` --calls--> `TenantContext`  [INFERRED]
  TeacherERP.Tests/Unit/WalkInLocationTests.cs → TeacherERP.Application/Interfaces/Icurrenttenantaccessor.cs
- `SubmitTeacherRequestCommandHandlerTests` --calls--> `OtpIssueResult`  [INFERRED]
  TeacherERP.Tests/Unit/TeacherRequestTests.cs → TeacherERP.Application/Interfaces/IOtpService.cs

## Import Cycles
- None detected.

## Communities (152 total, 22 thin omitted)

### Community 0 - "TeacherERP.Domain.Shared"
Cohesion: 0.02
Nodes (53): TeacherERP.Application.Features.Attendance.RejectCompensation, TeacherERP.Application.Features.Users.Commands.Auth.SetPassword, TeacherERP.Application.Features.Finance.GetMyPaymentOptions, TeacherERP.Application.Features.Team.GetTeam, TeacherERP.Application.Features.TeacherRequests.Review, TeacherERP.Application.Features.Notifications, TeacherERP.Application.Features.Finance.GetPaymentOptions, TeacherERP.Application.Features.Centers.JoinRequests (+45 more)

### Community 1 - "IActionResult"
Cohesion: 0.03
Nodes (27): TeacherERP.Application.Features.Finance.CreatePaymobIntent, TeacherERP.Application.Scheduling, TeacherERP.Application.Features.Schedule.GetSessions, TeacherERP.Application.Absence, TeacherERP.Application.Centers, TeacherERP.Application.Features.Enrollment.GetMyEnrollments, TeacherERP.Application.Features.Centers.CreateCenter, TeacherERP.Application.Features.Groups.GetGroups (+19 more)

### Community 2 - "Task"
Cohesion: 0.03
Nodes (54): TeacherERP.Application.Features.Finance.UpdateTeacherPaymentSettings, TeacherERP.Application.Common, TeacherERP.Application.Features.GroupAdmins.GetGroupSessionAttendance, TeacherERP.Application.Features.Finance.CollectCashPayment, PagedRequestRules, ManualAttendanceValidator, AcceptCenterInvitationValidator, ApproveCenterJoinRequestValidator (+46 more)

### Community 3 - "TeacherERP.Application.Interfaces"
Cohesion: 0.03
Nodes (27): TeacherERP.API.Middlewares, TeacherERP.Application, TeacherERP.Infrastructure.Settings, TeacherERP.Infrastructure, TeacherERP.Application.Finance, TeacherERP.Application.Features.Profile, TeacherERP.Application.Sessions, TeacherERP.Application.Features.Users.Commands.Auth.ForgotPassword (+19 more)

### Community 4 - "system"
Cohesion: 0.04
Nodes (69): ApproveCompensationCommand, GetAttendanceSettingsQuery, RegenerateStaticCodeCommand, RejectCompensationCommand, ScanAttendanceCommand, CenterDto, CreateMyCenterCommand, CreateMyCenterCommandHandler (+61 more)

### Community 5 - "TimeSpan"
Cohesion: 0.04
Nodes (23): CenterMembershipPolicy, InitialCreate, AddStudentProfile, AddTenants, MoveCenterStateToMembership, AddCentersAndTeacherSubject, AddGroupScheduleAndSessions, TestSeeder (+15 more)

### Community 6 - "system_text"
Cohesion: 0.04
Nodes (15): CreatePaymobIntentBody, SubmitInstaPayReceiptCommand, SubmitInstaPayReceiptCommandHandler, FinancialTransactionInfo, IFinancialLedgerRepository, PaymobAdjustmentReviewItem, GroupAdminItem, SessionMoveBlockers (+7 more)

### Community 7 - "Guid"
Cohesion: 0.06
Nodes (7): TeacherERP.Application.SessionContent, TeacherERP.API.Security, TeacherERP.API.Controllers, TeacherERP.API.Extensions, RateLimitPolicies, AuthPolicies, SessionContentPolicy

### Community 8 - "CreateAssistantCommandHandler"
Cohesion: 0.05
Nodes (29): EnrollmentService, EnrollmentRequestDto, MoveStudentCommand, MoveStudentCommandHandler, RequestGroupCommand, RequestGroupCommandHandler, RunningSessionInfo, GroupJoinInfo (+21 more)

### Community 9 - ".AddApplicationServices"
Cohesion: 0.12
Nodes (8): SchoolGrade, CenterMembershipIntegrationTests, FinanceAuthorizationIntegrationTests, ScheduleOperationsIntegrationTests, SessionGenerationIntegrationTests, SessionMoveIntegrationTests, StudentTransferIntegrationTests, TestData

### Community 10 - "Result"
Cohesion: 0.10
Nodes (58): AbsenceProcessingJob, AbsenceService, AttendanceRecorder, CenterJoinRequestNotifier, CenterMembershipCleanupJob, CenterProvisioningService, ApproveCompensationCommandHandler, GetPendingCompensationsQueryHandler (+50 more)

### Community 12 - "IRequestHandler"
Cohesion: 0.09
Nodes (3): FinancialLedgerService, PlatformCommissionCollectionRequestResult, IStoredFileCleanupJob

### Community 13 - "List"
Cohesion: 0.08
Nodes (24): ApproveInstaPayReceiptCommand, ApproveInstaPayReceiptCommandHandler, CollectCashPaymentCommand, CollectCashPaymentCommandHandler, GetMyPaymentOptionsQuery, GetMyPaymentOptionsQueryHandler, GetPaymentOptionsQuery, GetPaymentOptionsQueryHandler (+16 more)

### Community 14 - "DateTime"
Cohesion: 0.06
Nodes (16): TeacherERP.Application.Features.GroupAdmins.GetGroupAdminRequests, TeacherERP.Application.Features.GroupAdmins.GetMyGroupAdmin, TeacherERP.Application.Features.Attendance.UpdateAttendanceSettings, TeacherERP.Application.Features.GroupAdmins.GroupScan, TeacherERP.Application.Features.Attendance.GetAttendanceSettings, TeacherERP.Application.Features.Tenants.UpdateTenantSettings, TeacherERP.Application.Features.GroupAdmins.ApproveGroupAdminRequest, TeacherERP.Application.Features.Attendance.RegenerateStaticCode (+8 more)

### Community 15 - ".AddInfrastructureServices"
Cohesion: 0.06
Nodes (26): Session, AbsencesProcessedAt, Attendances, BoardImageUrl, CancelReason, EndTime, Group, GroupId (+18 more)

### Community 16 - "Group"
Cohesion: 0.05
Nodes (30): AppDbContext, AttendanceRecords, AuditLogs, CenterInvitations, CenterJoinRequests, Centers, CenterTeachers, CurrentTenantId (+22 more)

### Community 17 - "ApplicationUser"
Cohesion: 0.05
Nodes (10): TeacherERP.Application.Features.TeacherRequests.Submit, TeacherERP.Tests.Unit, TeacherERP.Application.Features.Enrollment.ApproveRequest, TeacherERP.Application.Features.TeacherRequests.Verify, TeacherERP.Application.Features.Administration.CreateCenter, TeacherERP.Application.Features.Administration.CreateTeacher, TeacherERP.Application.Features.Administration.LinkTeacherToCenter, TeacherERP.Application.Features.Schedule.CancelSession (+2 more)

### Community 18 - "IGroupSessionRepository"
Cohesion: 0.09
Nodes (12): AuditController, BaseApiController, DirectoryController, FinanceController, GroupAdminsController, NotificationsController, ProfileController, SessionAttachmentsController (+4 more)

### Community 20 - "AttendanceRecord"
Cohesion: 0.09
Nodes (25): AttendanceSettingsDto, GetAttendanceSettingsQueryHandler, ManualAttendanceCommand, RegenerateStaticCodeCommandHandler, UpdateAttendanceSettingsCommand, UpdateAttendanceSettingsCommandHandler, UpdateAttendanceSettingsValidator, ApproveGroupAdminRequestCommand (+17 more)

### Community 21 - "AppDbContext"
Cohesion: 0.07
Nodes (25): NotificationService, GetNotificationWhatsAppShareQuery, NotificationWhatsAppShareDto, GetNotificationWhatsAppShareQueryHandler, INotificationRepository, NotificationShareItem, Notification, IsRead (+17 more)

### Community 23 - "LoginSession"
Cohesion: 0.09
Nodes (16): AdminScanCommand, AdminScanCommandHandler, AdminScanValidator, DisplayCodeDto, GetDisplayCodeQuery, GetDisplayCodeQueryHandler, ScanAttendanceCommandHandler, GroupScanCommand (+8 more)

### Community 24 - ".Handle"
Cohesion: 0.07
Nodes (17): IExternalLoginService, ApplicationUser, Attendances, DateOfBirth, FullName, Gender, Guardian, IsProfileComplete (+9 more)

### Community 25 - "http"
Cohesion: 0.11
Nodes (6): GroupsController, ScheduleController, StudentsController, TeachersController, TeamController, SetFinancialDelegationRequest

### Community 26 - "AttendanceController.cs"
Cohesion: 0.12
Nodes (5): TeacherERP.Application.Features.Profile.CompleteProfile, TeacherERP.Application.Features.Groups.UpdateGroup, TeacherERP.Domain.ValueObjects, TeacherERP.Tests.Integration, UpdateGroupRequest

### Community 27 - ".SeedAsync"
Cohesion: 0.05
Nodes (39): TeacherERP.Application.Features.Audit, AuditLogDto, AuditAction, AttendanceCorrected, CashPaymentCollected, CompensationApproved, CompensationRejected, FinancialDelegationGranted (+31 more)

### Community 28 - "ICurrentTenantAccessor"
Cohesion: 0.12
Nodes (6): CreateTeacherCommand, CreateTeacherValidator, AdministrationIntegrationTests, TeacherRequestsIntegrationTests, CreateTeacherValidatorTests, GroupPricingTests

### Community 29 - "IUserAccountService"
Cohesion: 0.07
Nodes (17): Group, Capacity, CenterId, EducationStage, GradeLevel, GradeNumber, IsActive, MonthlyPrice (+9 more)

### Community 30 - "Transaction"
Cohesion: 0.11
Nodes (5): AuthController, CentersController, PasswordController, TeacherRequestsController, UsersController

### Community 31 - "GroupDto"
Cohesion: 0.07
Nodes (14): MoveSessionRequest, RescheduleSessionRequest, CenterJoinRequest, CenterId, DecisionNote, LastNotifiedAtUtc, ReviewedAtUtc, ReviewedByUserId (+6 more)

### Community 32 - "PasswordController.cs"
Cohesion: 0.08
Nodes (15): Program, CompensationStaffIntegrationTests, CustomWebApplicationFactory, FileStorage, PaymobGateway, PaymobSettings, DynamicQrIntegrationTests, GroupGradeIntegrationTests (+7 more)

### Community 33 - "MembershipStatus"
Cohesion: 0.13
Nodes (7): CreateTeacherCommandHandler, ICenterMembershipCleanupJob, ITeacherProvisioningService, UnitOfWork, PasswordIntegrationTests, CreateTeacherCommandHandlerTests, TeacherRequestReviewServiceTests

### Community 34 - "OtpService"
Cohesion: 0.07
Nodes (37): CloudinaryDotNet (1.29.3), coverlet.collector (6.0.4), FluentValidation (12.1.1), FluentValidation.DependencyInjectionExtensions (12.1.1), Google.Apis.Auth (1.77.0), Hangfire.AspNetCore (1.8.25), Hangfire.Core (1.8.25), Hangfire.Dashboard.BasicAuthorization (1.0.2) (+29 more)

### Community 35 - ".SaveChangesAsync"
Cohesion: 0.06
Nodes (14): TeacherERP.Application.Enrollment, TeacherERP.Application.Features.Attendance, TeacherERP.Application.Features.SessionContent, TeacherERP.Application.Features.Enrollment, AttendanceErrors, CenterErrors, EnrollmentErrors, ScheduleErrors (+6 more)

### Community 36 - "TeacherERP.Application.Features.Groups"
Cohesion: 0.06
Nodes (11): TeacherERP.Application.Features.Schedule.BulkCancelSessions, TeacherERP.Application.Features.Groups, TeacherERP.Application.Features.Schedule, TeacherERP.Application.Features.Schedule.MoveSession, TeacherERP.Application.Features.Schedule.BulkShiftSessions, TeacherERP.Application.Features.SessionContent.DeleteSessionAttachment, TeacherERP.Application.Features.Groups.UpdateGroupSchedule, TeacherERP.Application.Features.Schedule.AddExtraSession (+3 more)

### Community 37 - "GuardianRelation"
Cohesion: 0.06
Nodes (27): GroupAdminRequest, DecidedAt, DecidedByUserId, DecisionNote, Group, GroupId, RequestedByUserId, Status (+19 more)

### Community 38 - ".GetByIdAsync"
Cohesion: 0.09
Nodes (4): PaymobAdjustmentReviewService, Callback, PaymobErrors, PaymobPaymentService

### Community 39 - "ITenantRepository"
Cohesion: 0.13
Nodes (3): IEmailService, EmailService, EmailTemplates

### Community 40 - "EnrollmentRequestStatus"
Cohesion: 0.07
Nodes (7): TeacherERP.Application.Features.Users.Commands.Auth.GoogleSignIn, TeacherERP.Application.Features.SessionContent.UploadSessionAttachment, TeacherERP.Application.Features.Users.Commands.Auth.Login, TeacherERP.Application.Features.Users.Commands.Auth.ActivateAccount, TeacherERP.Application.Features.Team.CreateAssistant, TeacherERP.Application.Features.Users.Commands.Auth.ResetPassword, TeacherERP.Application.Features.Users.Commands.Auth.RefreshToken

### Community 41 - "GroupEnrollment"
Cohesion: 0.10
Nodes (19): TeacherERP.Infrastructure.Extensions, PagedResult, HasNext, HasPrevious, TotalPages, PageRequest, Skip, GetGroupAdminRequestsQuery (+11 more)

### Community 42 - "AbstractValidator"
Cohesion: 0.09
Nodes (23): CompleteProfileCommand, CompleteProfileCommandHandler, GetMyProfileQuery, GetMyProfileQueryHandler, GuardianDto, ProfileDto, Gender, Female (+15 more)

### Community 43 - "TeacherERP.Application.Features.Users.Commands.Auth.Login"
Cohesion: 0.12
Nodes (13): AssistantDto, CreateAssistantCommand, CreateAssistantCommandHandler, WelcomeEmailFailedException, CreateAssistantValidator, ResendWelcomeCommand, ResendWelcomeCommandHandler, WelcomeEmailFailedException (+5 more)

### Community 44 - "fluentvalidation"
Cohesion: 0.12
Nodes (3): AttendanceController, GroupAdminChangeResultDto, RequestGroupAdminChangeCommand

### Community 45 - "AttendanceResultDto"
Cohesion: 0.07
Nodes (7): TeacherERP.Application.Attendance, TeacherERP.Application.Features.Attendance.ScanAttendance, TeacherERP.Application.Features.Attendance.ApproveCompensation, TeacherERP.Application.Features.Attendance.ManualAttendance, AttendancePolicy, DependencyInjection, ScanAttendanceValidator

### Community 47 - ".Handle"
Cohesion: 0.11
Nodes (10): PlatformCommissionsController, RequestPlatformCommissionCollection, SettlePlatformCommissionRequest, UpdatePlatformCommissionSettingsRequest, TeacherPlatformCommissionsController, PlatformCommissionSettingsService, PlatformCommissionSummary, IPlatformCommissionService (+2 more)

### Community 48 - "AuthResponseDto"
Cohesion: 0.17
Nodes (3): ScheduleConflictItem, GroupSessionRepository, BulkCancelSessionsCommandHandlerTests

### Community 50 - "AuditLog"
Cohesion: 0.07
Nodes (22): Transaction, Amount, IdempotencyKey, PaymentContextSessionId, ProcessedByAdminId, Provider, ProviderClientSecret, ProviderFailureCode (+14 more)

### Community 51 - "BaseEntity"
Cohesion: 0.07
Nodes (3): IStudentTimelineRepository, DependencyInjection, StudentTimelineRepository

### Community 52 - "GroupSessionRepository"
Cohesion: 0.12
Nodes (4): CorsExtensions, ForwardedHeadersExtensions, ProductionDbSeeder, TestDataSeeder

### Community 53 - "SessionValidator"
Cohesion: 0.09
Nodes (12): TeacherERP.Application.Features.Team, TeacherERP.Application.Features.Students, ApiAuthorizationResultHandler, GroupErrors, StudentErrors, StudentReasonRequest, TeamErrors, TeamPolicy (+4 more)

### Community 54 - ".ResolveAsync"
Cohesion: 0.15
Nodes (18): TeacherERP.Application.Features.Directory, DirectoryCenterDto, DirectoryCenterInfoDto, DirectoryGroupDto, DirectoryTeacherDto, TeacherProfileDto, GetTeacherProfileQuery, GetTeacherProfileQueryHandler (+10 more)

### Community 55 - "ResetGrantService"
Cohesion: 0.16
Nodes (10): ChangePasswordCommand, ChangePasswordCommandHandler, ChangePasswordValidator, SetPasswordCommand, SetPasswordCommandHandler, SetPasswordValidator, ICredentialService, IUserRepository (+2 more)

### Community 56 - ".HandleAsync"
Cohesion: 0.09
Nodes (19): TeacherAccessRequest, CreatedUserId, DecisionNote, Email, ExpiresAtUtc, FullName, Note, ReviewedAtUtc (+11 more)

### Community 57 - ".AddApiRateLimiting"
Cohesion: 0.08
Nodes (25): PaymentEvent, CurrentStatus, EventType, PayloadSha256, PreviousStatus, Provider, ProviderEventId, ProviderOccurredAtUtc (+17 more)

### Community 58 - "TenantRole"
Cohesion: 0.16
Nodes (12): GetPendingCompensationsQuery, GetPendingCompensationsValidator, GetAuditLogQuery, GetAuditLogQueryHandler, GetAuditLogValidator, GetPendingInstaPayReceiptsQuery, GetPendingInstaPayReceiptsValidator, GetPendingInstaPayReceiptsQueryHandler (+4 more)

### Community 59 - "IServiceCollection"
Cohesion: 0.11
Nodes (7): FixLoginSessionUserId, AddCenterMembershipRequests, SnapshotAccessor, SnapshotAccessor, AddSessionOriginalStartTime, SnapshotAccessor, AppDbContextModelSnapshot

### Community 60 - "ICenterRepository"
Cohesion: 0.08
Nodes (22): AttendanceRecord, CompensatesSessionId, RecordedByUserId, ScannedLatitude, ScannedLongitude, ScannedVia, Session, SessionId (+14 more)

### Community 61 - ".ValidateIdTokenAsync"
Cohesion: 0.16
Nodes (10): GetMySessionsQuery, GetMySessionsQueryHandler, SessionDto, GetCurrentTenantQuery, GetCurrentTenantQueryHandler, GetMyTenantsQuery, GetMyTenantsQueryHandler, TenantDto (+2 more)

### Community 62 - "CompleteProfileValidator"
Cohesion: 0.10
Nodes (14): CreateMyCenterValidator, CenterInvitation, AcceptedAtUtc, AcceptedByTenantId, CenterId, CreatedByUserId, ExpiresAtUtc, Status (+6 more)

### Community 63 - "SessionManager"
Cohesion: 0.09
Nodes (21): PaymobAdjustmentResolution, PaymobAdjustmentReview, CommissionReversalAmount, Currency, DecisionNote, ProviderAmount, ProviderEventId, ReviewedAtUtc (+13 more)

### Community 64 - "TeacherERP.Application.Features.Team"
Cohesion: 0.11
Nodes (13): IAbsenceProcessingJob, ICurrentTenantAccessor, Current, TenantId, TenantContext, ISessionGenerationJob, TenantRole, Assistant (+5 more)

### Community 65 - "GetSessionsQueryHandler"
Cohesion: 0.09
Nodes (23): ASPNETCORE_ENVIRONMENT, applicationUrl, commandName, dotnetRunMessages, environmentVariables, applicationUrl, commandName, dotnetRunMessages (+15 more)

### Community 66 - "SetFinancialDelegationCommandHandler.cs"
Cohesion: 0.11
Nodes (14): ISessionRepository, LoginSession, AbsoluteExpiresAt, DeviceId, DeviceName, LastRefreshedAt, PreviousRefreshTokenHash, RefreshExpiresAt (+6 more)

### Community 67 - "RevokeSessionCommand"
Cohesion: 0.17
Nodes (13): CreatedTeacherDto, ApproveTeacherRequestByAdminCommand, ApproveTeacherRequestByAdminCommandHandler, ApproveTeacherRequestCommand, ApproveTeacherRequestCommandHandler, TeacherRequestRules, ITeacherRequestReviewService, ReviewerKind (+5 more)

### Community 68 - "CancelSessionCommandHandler.cs"
Cohesion: 0.09
Nodes (19): PlatformCommission, Amount, CollectionRequestedAtUtc, CollectionRequestedByUserId, GroupId, IsSettled, SessionId, SettledAtUtc (+11 more)

### Community 69 - "HangfireBasicAuthFilter"
Cohesion: 0.16
Nodes (12): CreateGroupCommand, GetGroupQuery, GetGroupQueryHandler, GroupDto, GroupSlotDto, IGroupPricing, MonthlyPrice, PerSessionPrice (+4 more)

### Community 70 - "GetMySessionsQueryHandler"
Cohesion: 0.23
Nodes (3): Scenario, WalkInLocationIntegrationTests, WalkInApprovalTests

### Community 71 - "Centerdto.cs"
Cohesion: 0.15
Nodes (14): EnrollmentRequestListDto, GetRequestsQuery, GetRequestsQueryHandler, GetRequestsValidator, EnrollmentRequestListItem, MyRequestItem, EnrollmentRequestStatus, Approved (+6 more)

### Community 72 - "TeacherERP.Application.Features.Students"
Cohesion: 0.19
Nodes (14): ActivateAccountCommand, ActivateAccountCommandHandler, ActivateAccountValidator, GoogleSignInCommand, GoogleSignInCommandHandler, GoogleSignInValidator, AuthResponseDto, LoginCommand (+6 more)

### Community 73 - "LoggingBehavior.cs"
Cohesion: 0.14
Nodes (9): CompletedProfileHandler, CompletedProfileRequirement, TenantHandler, TenantRequirement, AllowedRoles, RequiresFinancialDelegation, ProfileCompletionChecker, ProfileRequirement (+1 more)

### Community 74 - "GeoDistance"
Cohesion: 0.16
Nodes (7): AdminCenterDto, CreateCenterCommand, CreateCenterCommandHandler, CreateCenterValidator, CenterLocationPolicy, ICenterProvisioningService, CreateCenterCommandHandlerTests

### Community 75 - "CachedRegistrationData"
Cohesion: 0.16
Nodes (14): OtpIssueResult, OtpPurpose, Activation, PasswordReset, Registration, TeacherRequest, IssueCounter, Count (+6 more)

### Community 76 - ".TryHandleAsync"
Cohesion: 0.18
Nodes (3): GroupAdminAttendanceIntegrationTests, Scenario, SchoolGradeTests

### Community 77 - "UpdateGroupValidator"
Cohesion: 0.18
Nodes (5): AbsenceProcessor, IAbsenceProcessor, IAbsenceRepository, IAbsenceService, AbsenceRepository

### Community 78 - ".RequiresProfileCompletion"
Cohesion: 0.12
Nodes (15): CenterTeacher, Center, CenterId, Origin, Status, TenantId, CenterTeacherOrigin, Administrator (+7 more)

### Community 79 - "Updatecentervalidator.cs"
Cohesion: 0.23
Nodes (3): FinancialSessionInfo, CashPaymentRequest, FinancialLedgerServiceTests

### Community 80 - "AddExtraSessionValidator.cs"
Cohesion: 0.22
Nodes (5): AccessTokenResult, IJwtProvider, SessionManager, TokenHasher, JwtProvider

### Community 81 - "BulkCancelSessionsValidator.cs"
Cohesion: 0.12
Nodes (13): BaseEntity, CreatedAt, Id, UpdatedAt, GroupScheduleSlot, Day, DurationMinutes, Group (+5 more)

### Community 82 - "BulkShiftSessionsValidator.cs"
Cohesion: 0.25
Nodes (7): TeacherRequestQrDto, VerifyTeacherRequestCommand, VerifyTeacherRequestCommandHandler, VerifyTeacherRequestValidator, IOtpService, ITeacherAccessRequestRepository, VerifyTeacherRequestCommandHandlerTests

### Community 83 - "RescheduleSessionValidator.cs"
Cohesion: 0.11
Nodes (18): SessionAttachment, Caption, ContentType, FileName, Kind, PublicId, Session, SessionId (+10 more)

### Community 84 - "BlacklistStudentValidator.cs"
Cohesion: 0.12
Nodes (13): MonthlySubscription, Amount, GroupId, Month, PaymentTransactionId, StudentId, TenantId, Year (+5 more)

### Community 86 - "CreateAssistantValidator.cs"
Cohesion: 0.20
Nodes (7): ResetTokenDto, VerifyResetOtpCommand, VerifyResetOtpCommandHandler, VerifyResetOtpValidator, IResetGrantService, ResetGrant, ResetGrantService

### Community 87 - "RemoveAssistantValidator.cs"
Cohesion: 0.21
Nodes (4): IUserAccountService, TeacherProvisioningService, WelcomeEmailFailedException, TeacherProvisioningServiceTests

### Community 88 - "ResendWelcomeValidator.cs"
Cohesion: 0.24
Nodes (6): SubmitTeacherRequestCommand, SubmitTeacherRequestCommandHandler, SubmitTeacherRequestValidator, TeacherRequestOtpDto, SubmitTeacherRequestCommandHandlerTests, TeacherRequestBasicsTests

### Community 89 - "SetFinancialDelegationValidator.cs"
Cohesion: 0.18
Nodes (10): CenterJoinRequestListItem, GetCenterJoinRequestsQuery, GetCenterJoinRequestsValidator, GetMyCenterJoinRequestsQuery, GetMyCenterJoinRequestsValidator, CenterJoinRequestStatus, Approved, Cancelled (+2 more)

### Community 90 - "GoogleSignInValidator.cs"
Cohesion: 0.22
Nodes (6): GetStudentLearningTimelineQuery, GetStudentLearningTimelineValidator, StudentTimelineAttachmentDto, StudentTimelineSessionDto, SessionAttachmentItem, StudentTimelineSessionItem

### Community 91 - "ResetPasswordValidator.cs"
Cohesion: 0.17
Nodes (9): FileNames, DetectedFileType, Jpeg, Pdf, Png, Unknown, Webp, FileSignature (+1 more)

### Community 92 - "VerifyResetOtpValidator.cs"
Cohesion: 0.23
Nodes (5): GetSessionAttachmentsQuery, GetSessionAttachmentsQueryHandler, SessionAttachmentDto, ISessionAttachmentRepository, SessionAttachmentRepository

### Community 93 - "CreateStudentValidator.cs"
Cohesion: 0.29
Nodes (5): CenterTeacherLinkDto, LinkTeacherToCenterCommand, LinkTeacherToCenterCommandHandler, LinkTeacherToCenterValidator, LinkTeacherToCenterCommandHandlerTests

### Community 94 - "VerifyOtpValidator.cs"
Cohesion: 0.13
Nodes (12): Tenant, AutoApproveGroupAdminChanges, AutoApproveJoinRequests, IsListedInSearch, Name, OwnerId, Status, Subject (+4 more)

### Community 95 - "ProfileAuthorizationResultHandler.cs"
Cohesion: 0.16
Nodes (7): CachedRegistrationData, Email, FullName, Password, CreateStudentCommandHandler, RegistrationCacheKey, VerifyOtpCommandHandler

### Community 97 - "TeacherERP.Application.Features.Schedule.GetSessions"
Cohesion: 0.24
Nodes (3): ISessionGenerator, SessionGenerator, SessionTimeCalculator

### Community 98 - "UpdateGroupValidator"
Cohesion: 0.14
Nodes (11): LogoutAllCommand, LogoutAllCommandHandler, SessionRevokeReason, AdminAction, DeviceLimit, LogoutAll, PasswordChanged, PasswordReset (+3 more)

### Community 99 - "ManualAttendanceValidator.cs"
Cohesion: 0.13
Nodes (14): PaymobSettings, ApiKey, BaseUrl, Currency, HmacNullValue, HmacSecret, InitializationLeaseSeconds, IntegrationIds (+6 more)

### Community 100 - "Updatecentervalidator.cs"
Cohesion: 0.15
Nodes (8): Center, Address, GeofenceRadiusMeters, Latitude, Longitude, Name, OwnerUserId, CenterRepository

### Community 101 - "RejectRequestValidator.cs"
Cohesion: 0.16
Nodes (11): AttendanceMode, AdminOnly, DynamicScreenQR, StaticGlobalQR, TenantSettings, AutoApproveGroupAdminScans, AutoRegisterOnScan, Mode (+3 more)

### Community 102 - "RequestGroupValidator.cs"
Cohesion: 0.16
Nodes (3): PaymobReturnController, PaymobWebhookController, RolesController

### Community 103 - "GroupScanValidator.cs"
Cohesion: 0.23
Nodes (12): CreatePaymobIntentCommand, CreatePaymobIntentCommandValidator, CreatePaymobIntentCommandHandler, CreatePaymobPaymentRequest, IPaymobPaymentService, PaymobIntegrationKey, CaggOnlineNonShopify, CashCashChannel (+4 more)

### Community 104 - "RequestGroupAdminChangeValidator.cs"
Cohesion: 0.21
Nodes (9): StudentListDto, GetTeamQuery, GetTeamQueryHandler, TeamMemberDto, TeamMemberItem, MembershipStatus, Active, Blacklisted (+1 more)

### Community 105 - "CancelSessionValidator.cs"
Cohesion: 0.14
Nodes (11): FinancialObligation, Amount, GroupId, GroupName, IsSettled, OutstandingAmount, PaidAmount, SessionId (+3 more)

### Community 106 - "LogoutValidator.cs"
Cohesion: 0.19
Nodes (6): IFileStorage, StoredFileCleanupJob, CloudinaryFileStorage, TestFileStorage, DeletedPublicIds, UploadCount

### Community 107 - "BlacklistStudentValidator.cs"
Cohesion: 0.15
Nodes (9): TeacherERP.Application.Features.TeacherRequests, CachedTeacherRequestData, Email, FullName, Note, Subject, TimeZoneId, TeacherRequestCacheKey (+1 more)

### Community 108 - "CreateAssistantValidator.cs"
Cohesion: 0.22
Nodes (4): PaymobGatewayTests, RecordingLogger, Messages, StubHandler

### Community 109 - "RemoveAssistantValidator.cs"
Cohesion: 0.15
Nodes (12): UpdateTeacherPaymentSettingsForm, InstaPayLink, QrImage, UploadSessionAttachmentForm, Caption, File, Kind, SubmitInstaPayReceiptForm (+4 more)

### Community 110 - "ResendWelcomeValidator.cs"
Cohesion: 0.15
Nodes (5): TeacherPaymentSettings, InstaPayLink, QrImagePublicId, QrImageUrl, TenantId

### Community 112 - "ChangePasswordValidator.cs"
Cohesion: 0.24
Nodes (7): IPagedRequest, Page, PageSize, GetSessionAttendanceQuery, GetSessionAttendanceValidator, GetGroupSessionAttendanceQuery, AttendanceListItem

### Community 114 - "RefreshTokenValidator.cs"
Cohesion: 0.24
Nodes (3): SessionJwtBearerEvents, ISessionValidator, SessionValidator

### Community 115 - "CreateStudentValidator.cs"
Cohesion: 0.35
Nodes (6): GetStudentsQuery, GetStudentsQueryHandler, GetStudentsValidator, IStudentRepository, StudentListItem, StudentRepository

### Community 116 - "ProfileAuthorizationResultHandler.cs"
Cohesion: 0.35
Nodes (6): MonthlyCollectionReportService, MonthlyCollectionItem, MonthlyCollectionReport, MonthlyCollectionTotals, MonthlyOutstandingDebtItem, MonthlyPaymentMethodSummary

### Community 119 - "MailSettings"
Cohesion: 0.22
Nodes (3): TeacherERP.Application.Behaviors, LoggingBehavior, ValidationBehavior

### Community 121 - "ForwardedHeadersExtensions.cs"
Cohesion: 0.20
Nodes (7): IUserTimeZoneAccessor, TimeZoneId, CurrentUserService, SessionId, UserId, HttpUserTimeZoneAccessor, TimeZoneId

### Community 123 - "SessionCleanupJob"
Cohesion: 0.20
Nodes (3): AdministrationController, LinkTeacherToCenterRequest, RejectTeacherRequestRequest

### Community 124 - "CloudinarySettings"
Cohesion: 0.33
Nodes (7): IPaymobGateway, PaymobIntentCommandData, PaymobIntentData, TestPaymobGateway, CreateCount, FailNextWithCode, LastRequest

### Community 125 - "JwtSettings"
Cohesion: 0.31
Nodes (4): PaymobAdjustmentReviewsController, ResolvePaymobAdjustmentBody, IPaymobAdjustmentReviewService, ResolvePaymobAdjustmentRequest

### Community 126 - ".TokenValidated"
Cohesion: 0.28
Nodes (4): GetSessionsQuery, GetSessionsValidator, SessionDto, SessionListItem

### Community 127 - ".Meters"
Cohesion: 0.28
Nodes (4): CenterInvitationToken, CenterInvitationDto, CreateCenterInvitationCommand, CreateCenterInvitationValidator

### Community 130 - "TeacherERP.Application.Features.Students"
Cohesion: 0.29
Nodes (3): TeacherERP.Application.Features.Enrollment.MoveStudent, MoveStudentValidator, MoveStudentTests

### Community 131 - "HangfireBasicAuthFilter"
Cohesion: 0.39
Nodes (3): MonthlyCollectionReportsController, TeacherMonthlyCollectionReportsController, IMonthlyCollectionReportService

### Community 133 - "PreviewTeacherRequestQuery"
Cohesion: 0.39
Nodes (4): PreviewTeacherRequestQuery, PreviewTeacherRequestQueryHandler, TeacherRequestPreviewDto, TeacherRequestToken

### Community 134 - ".ValidateIdTokenAsync"
Cohesion: 0.32
Nodes (5): GoogleUserInfo, IGoogleAuthService, GoogleAuthService, GoogleAuthSettings, ClientIds

### Community 135 - "GetMyEnrollmentsQuery"
Cohesion: 0.43
Nodes (5): MyEnrollmentDto, MyEnrollmentsDto, GetMyEnrollmentsQuery, GetMyEnrollmentsQueryHandler, GetMyEnrollmentsValidator

### Community 136 - "GetMyNotificationsQuery"
Cohesion: 0.52
Nodes (4): GetMyNotificationsQuery, GetMyNotificationsQueryHandler, GetMyNotificationsValidator, NotificationItem

### Community 137 - "ListTeacherRequestsQuery"
Cohesion: 0.52
Nodes (4): ListTeacherRequestsQuery, ListTeacherRequestsQueryHandler, ListTeacherRequestsValidator, TeacherAccessRequestListItem

### Community 140 - "GroupSlotInput"
Cohesion: 0.33
Nodes (3): GroupSlotInput, IGroupSchedule, Slots

### Community 141 - "AttendanceOutcome"
Cohesion: 0.33
Nodes (6): AttendanceOutcome, AlreadyRecorded, Corrected, PendingApproval, PendingCompensation, Recorded

### Community 142 - "PaymobWebhookOutcome"
Cohesion: 0.33
Nodes (6): PaymobWebhookOutcome, Accepted, Duplicate, Ignored, InvalidPayload, InvalidSignature

### Community 143 - "EducationStage"
Cohesion: 0.33
Nodes (5): EducationStage, Preparatory, Primary, Secondary, Unspecified

### Community 144 - "LogoutCommand"
Cohesion: 0.50
Nodes (3): LogoutCommand, LogoutCommandHandler, LogoutValidator

### Community 145 - "GetTeacherRequestStatusQuery"
Cohesion: 0.70
Nodes (4): GetTeacherRequestStatusQuery, GetTeacherRequestStatusQueryHandler, GetTeacherRequestStatusValidator, TeacherRequestStatusDto

### Community 149 - "JwtSettings"
Cohesion: 0.40
Nodes (5): JwtSettings, Audience, ExpiryMinutes, Issuer, Secret

### Community 151 - "ForgotPasswordCommandHandler"
Cohesion: 1.00
Nodes (3): ForgotPasswordCommand, ForgotPasswordCommandHandler, ForgotPasswordResponseDto

## Knowledge Gaps
- **594 isolated node(s):** `Address`, `GeofenceRadiusMeters`, `Latitude`, `Longitude`, `Name` (+589 more)
  These have ≤1 connection - possible missing edges. (Counts symbols only; 830 node(s) total have ≤1 connection when file, concept and rationale nodes are included.)
- **22 thin communities (<3 nodes) omitted from report** — run `graphify query` to explore isolated nodes.

## Suggested Questions
_Questions this graph is uniquely positioned to answer:_

- **Why does `Result` connect `system` to `HangfireBasicAuthFilter`, `TeacherERP.Application.Interfaces`, `PreviewTeacherRequestQuery`, `system_text`, `GetMyEnrollmentsQuery`, `CreateAssistantCommandHandler`, `GetMyNotificationsQuery`, `Result`, `ListTeacherRequestsQuery`, `IRequestHandler`, `List`, `.ValidateIdTokenAsync`, `LogoutCommand`, `GetTeacherRequestStatusQuery`, `AppDbContext`, `LoginSession`, `ForgotPasswordCommandHandler`, `.Handle`, `ICurrentTenantAccessor`, `MembershipStatus`, `.SaveChangesAsync`, `AbstractValidator`, `TeacherERP.Application.Features.Users.Commands.Auth.Login`, `.Handle`, `AuthResponseDto`, `.ResolveAsync`, `ResetGrantService`, `TenantRole`, `.ValidateIdTokenAsync`, `RevokeSessionCommand`, `HangfireBasicAuthFilter`, `Centerdto.cs`, `TeacherERP.Application.Features.Students`, `GeoDistance`, `CachedRegistrationData`, `Updatecentervalidator.cs`, `AddExtraSessionValidator.cs`, `BulkShiftSessionsValidator.cs`, `CreateAssistantValidator.cs`, `RemoveAssistantValidator.cs`, `ResendWelcomeValidator.cs`, `SetFinancialDelegationValidator.cs`, `GoogleSignInValidator.cs`, `VerifyResetOtpValidator.cs`, `CreateStudentValidator.cs`, `ProfileAuthorizationResultHandler.cs`, `TeacherERP.Application.Features.Schedule.GetSessions`, `UpdateGroupValidator`, `GroupScanValidator.cs`, `RequestGroupAdminChangeValidator.cs`, `LogoutValidator.cs`, `ChangePasswordValidator.cs`, `CreateStudentValidator.cs`, `ProfileAuthorizationResultHandler.cs`, `HangfireBasicAuthFilter`, `CloudinarySettings`, `.TokenValidated`, `.Meters`?**
  _High betweenness centrality (0.072) - this node is a cross-community bridge._
- **What connects `Address`, `GeofenceRadiusMeters`, `Latitude` to the rest of the system?**
  _594 weakly-connected nodes found - possible documentation gaps or missing edges._
- **Should `TeacherERP.Domain.Shared` be split into smaller, more focused modules?**
  _Cohesion score 0.022338317420284633 - nodes in this community are weakly interconnected._
- **Why does `AppDbContext` connect `Group` to `PaymobWebhookOutcome`, `system_text`, `CreateAssistantCommandHandler`, `.AddApplicationServices`, `Result`, `Session`, `TeacherERP.API`, `AppDbContext`, `LoginSession`, `.SeedAsync`, `IUserAccountService`, `GroupDto`, `PasswordController.cs`, `MembershipStatus`, `GuardianRelation`, `TeacherERP.Application.Features.Users.Commands.Auth.Login`, `AuthResponseDto`, `BaseEntity`, `GroupSessionRepository`, `.ResolveAsync`, `ResetGrantService`, `.HandleAsync`, `IServiceCollection`, `.ValidateIdTokenAsync`, `SetFinancialDelegationCommandHandler.cs`, `UpdateGroupValidator`, `Updatecentervalidator.cs`, `VerifyResetOtpValidator.cs`, `Updatecentervalidator.cs`, `ResendWelcomeValidator.cs`, `RefreshTokenValidator.cs`, `CreateStudentValidator.cs`, `AttendanceOutcome`?**
  _High betweenness centrality (0.051) - this node is a cross-community bridge._
- **Should `IActionResult` be split into smaller, more focused modules?**
  _Cohesion score 0.03074712643678161 - nodes in this community are weakly interconnected._
- **Why does `TeacherERP.Application.Interfaces` connect `IActionResult` to `TeacherERP.Domain.Shared`, `TeacherERP.Application.Features.Students`, `TeacherERP.Application.Interfaces`, `Task`, `system`, `Guid`, `CreateAssistantCommandHandler`, `Result`, `Session`, `IRequestHandler`, `List`, `DateTime`, `ApplicationUser`, `SessionCleanupJob`, `LoginSession`, `AttendanceController.cs`, `.SeedAsync`, `MembershipStatus`, `.SaveChangesAsync`, `TeacherERP.Application.Features.Groups`, `ITenantRepository`, `EnrollmentRequestStatus`, `TeacherERP.Application.Features.Users.Commands.Auth.Login`, `AttendanceResultDto`, `.Handle`, `.ResolveAsync`, `TeacherERP.Application.Features.Team`, `LoggingBehavior.cs`, `UpdateGroupValidator`, `CreateAssistantValidator.cs`, `TeacherERP.Application.Features.Schedule.GetSessions`, `RefreshTokenValidator.cs`, `ProfileAuthorizationResultHandler.cs`, `ForwardedHeadersExtensions.cs`, `CloudinarySettings`?**
  _High betweenness centrality (0.047) - this node is a cross-community bridge._
- **Should `Task` be split into smaller, more focused modules?**
  _Cohesion score 0.025494943740207946 - nodes in this community are weakly interconnected._