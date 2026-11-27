# Graph Report - TeacherERP  (2026-11-27)

## Corpus Check
- cluster-only mode — file stats not available

## Summary
- 4805 nodes · 18344 edges · 156 communities (119 shown, 37 thin omitted)
- Extraction: 90% EXTRACTED · 10% INFERRED · 0% AMBIGUOUS · INFERRED: 1883 edges (avg confidence: 0.85)
- Token cost: 0 input · 0 output

## Graph Freshness
- Built from commit: `fde3d466`
- Run `git rev-parse HEAD` and compare to check if the graph is stale.
- Run `graphify update .` after code changes (no API cost).

## Community Hubs (Navigation)
- TeacherERP.Domain.Shared
- IActionResult
- Task
- TeacherERP.Application.Scheduling
- system
- ApplicationUser
- Result
- TeacherERP.Application.Interfaces
- CreateAssistantCommandHandler
- .SaveChangesAsync
- Guid
- Session
- system_data
- .Handle
- DateTime
- EnrollmentRequestStatus
- IFinancialLedgerRepository
- PagedResult
- IGroupSessionRepository
- .MarkAsUpdated
- AttendanceRecord
- IRequestHandler
- Center
- Center
- .Handle
- IFinancialLedgerRepository.cs
- AttendanceController.cs
- Fact
- CustomWebApplicationFactory
- LoginSession
- MembershipStatus
- microsoft_aspnetcore_authorization
- PasswordController.cs
- IActionResult
- .Handle
- .SaveChangesAsync
- CreateAssistantCommandHandler
- GroupDto
- .AddApplicationServices
- IOtpService
- TeacherRequestReviewServiceTests
- GetTeacherDashboardQueryHandler
- AbstractValidator
- Session
- fluentvalidation
- CancellationToken
- 🏫 Smart Teacher ERP
- HttpGet
- AuthResponseDto
- .Upload
- EnrollmentRequest
- BaseEntity
- GroupSessionRepository
- .Create
- .ResolveAsync
- TeacherERP.Application.Features.Users.Commands.Auth.Login
- .GetByOwnerIdAsync
- SeededTeacher
- GroupAdminAttendanceIntegrationTests
- TenantContext
- ICenterRepository
- ApproveTeacherRequestCommand
- CompleteProfileValidator
- SessionManager
- TeacherERP.Application.Features.Team
- TeacherERP.Application/DependencyInjection.cs
- SetFinancialDelegationCommandHandler.cs
- RevokeSessionCommand
- CancelSessionCommandHandler.cs
- HangfireBasicAuthFilter
- GetMySessionsQueryHandler
- FinanceController
- LoggingBehavior
- GetAuditLogQuery
- GetSessionsQuery
- CachedRegistrationData
- GetMyNotificationsQuery
- UpdateGroupValidator
- .Handle
- SessionValidator
- RescheduleSessionRequest
- PageRequest
- Task
- RescheduleSessionValidator.cs
- Error
- .ApproveAsync
- CreateAssistantValidator.cs
- TeacherAccessRequestStatus
- AttendanceSettingsDto
- BaseApiController
- GoogleSignInValidator.cs
- ResetPasswordValidator.cs
- SessionAttachment
- TeacherProvisioningServiceTests
- .ResolveAsync
- ProfileAuthorizationResultHandler.cs
- TeacherERP.Application.Features.Users.Commands.Auth.Login
- CenterJoinRequestListItem
- CreatedTeacherDto
- .GetByIdAsync
- GroupScheduleSlot
- .AddInfrastructureServices
- MoveStudentCommandHandler.cs
- GroupScanValidator.cs
- .GetByIdAsync
- IAttendanceRepository
- LogoutValidator.cs
- .ConfigureWebHost
- IPagedRequest
- .AddApiRateLimiting
- CurrentUserService
- .Format
- Gender
- GetGroupJoinCodesQueryHandler
- TenantDto
- ControllerBase
- RequestGroupByCodeCommandHandler.cs
- IServiceCollection
- PlatformCommissionSettings
- .VerifyAsync
- TeacherERP.Application.Features.Team
- ForwardedHeadersExtensions.cs
- GetNotificationWhatsAppShareQuery
- IReadOnlyList
- CloudinarySettings
- GetSessionsQuery
- .TokenValidated
- PaymobWebhookOutcome
- PaymobWebhookOutcome
- SubmitInstaPayReceiptForm
- PaymentType
- CenterMembershipPolicy.cs
- SchedulePolicy.cs
- RoleNames.cs
- HangfireBasicAuthFilter
- IReadOnlyList
- .EnsurePaymentEventsAreAppendOnly
- SessionCleanupJob
- CurrentTenantAccessor
- CloudinarySettings
- JwtSettings
- AppDbContextFactory
- .EnsurePaymentEventsAreAppendOnly
- .Hash
- .Meters
- NotificationErrors.cs
- Updatecenterrequest.cs
- AppDbContextFactory
- 🛡️ Core Features
- GetNotificationWhatsAppShareQuery
- .GetAssistantsAsync
- GroupScanCommand.cs
- AppDbContextFactory.cs
- SessionContentPolicy.cs
- RoleNames.cs

## God Nodes (most connected - your core abstractions)
1. `Result` - 430 edges
2. `TeacherERP.Application.Interfaces` - 312 edges
3. `TeacherERP.Domain.Shared` - 292 edges
4. `TeacherERP.Domain.Entities` - 236 edges
5. `AppDbContext` - 110 edges
6. `TeacherERP.Application.Common` - 107 edges
7. `PagedResult` - 97 edges
8. `ApplicationUser` - 86 edges
9. `ICurrentTenantAccessor` - 78 edges
10. `ICurrentUserService` - 68 edges

## Surprising Connections (you probably didn't know these)
- `🔒 1. Advanced Security & Session Control` --references--> `ApplicationUser`  [INFERRED]
  README.md → TeacherERP.Domain/Entities/Users/User.cs
- `🗄️ Database Schema` --references--> `ApplicationUser`  [INFERRED]
  README.md → TeacherERP.Domain/Entities/Users/User.cs
- `🗄️ Database Schema` --references--> `TenantSettings`  [INFERRED]
  README.md → TeacherERP.Domain/Entities/Tenants/TenantSettings.cs
- `TeamErrors` --references--> `Error`  [EXTRACTED]
  TeacherERP.Application/Features/Team/TeamErrors.cs → TeacherERP.Domain/Shared/Error.cs
- `CancelSessionCommandHandlerTests` --calls--> `TenantContext`  [INFERRED]
  TeacherERP.Tests/Unit/Sessions/CancelSessionHandlersTests.cs → TeacherERP.Application/Interfaces/Tenants/ICurrentTenantAccessor.cs

## Import Cycles
- None detected.

## Communities (156 total, 37 thin omitted)

### Community 0 - "TeacherERP.Domain.Shared"
Cohesion: 0.02
Nodes (55): TeacherERP.Application.Features.Attendance.RejectCompensation, TeacherERP.Application.Features.Finance.GetMyPaymentOptions, TeacherERP.Application.Features.Team.GetTeam, TeacherERP.Application.Features.Finance.CreatePaymobIntent, TeacherERP.Application.Features.Enrollment.ApproveRequest, TeacherERP.Application.Features.GroupAdmins.GetMyGroupAdmin, TeacherERP.Application.Features.Finance.GetPaymentOptions, TeacherERP.Application.Features.Students.GetWeeklySchedule (+47 more)

### Community 1 - "IActionResult"
Cohesion: 0.04
Nodes (20): CollectCashPaymentCommand, CollectCashPaymentCommandHandler, GroupJoinInfo, FinancialSessionInfo, FinancialTransactionInfo, IFinancialLedgerRepository, CashPaymentRequest, CashPaymentResult (+12 more)

### Community 2 - "Task"
Cohesion: 0.03
Nodes (19): GroupJoinCodeLookup, ICredentialService, IResetGrantService, ResetGrant, ITransientAuthStateCleanupJob, ITransientAuthStateStore, ISessionCleanupJob, ITeacherAccessRequestRepository (+11 more)

### Community 3 - "TeacherERP.Application.Scheduling"
Cohesion: 0.05
Nodes (28): AdministrationController, AttendanceController, AuditController, AuthController, PasswordController, BaseApiController, CentersController, DashboardController (+20 more)

### Community 4 - "system"
Cohesion: 0.03
Nodes (24): TeacherERP.Application.Features.Users.Commands.Auth.SetPassword, TeacherERP.Application.Features.TeacherRequests.Submit, TeacherERP.Application.Features.Schedule.BulkCancelSessions, TeacherERP.Tests.Unit, TeacherERP.Application.Features.Users.Commands.VerifyOtp, TeacherERP.Application.Features.Users.Commands.Auth.ChangePassword, TeacherERP.Application.Features.TeacherRequests.Verify, TeacherERP.Application.Attendance (+16 more)

### Community 5 - "ApplicationUser"
Cohesion: 0.02
Nodes (26): TeacherERP.Application.Features.Students.GetStudentDetails, TeacherERP.Application.Features.Groups, TeacherERP.Application.Features.Attendance.UpdateAttendanceSettings, TeacherERP.Application.Absence, TeacherERP.Application.Features.Centers.CreateCenter, TeacherERP.Application.Features.Audit, TeacherERP.Application.Features.Centers, TeacherERP.Application.Features.Profile.CompleteProfile (+18 more)

### Community 6 - "Result"
Cohesion: 0.04
Nodes (85): AdminScanCommand, ApproveCompensationCommand, DisplayCodeDto, GetDisplayCodeQuery, GetDisplayCodeQueryHandler, RejectCompensationCommand, ScanAttendanceCommand, ApproveCenterJoinRequestCommand (+77 more)

### Community 7 - "TeacherERP.Application.Interfaces"
Cohesion: 0.02
Nodes (37): TeacherERP.Application.Features.Users.Commands.Auth.GoogleSignIn, TeacherERP.Application.Features.GroupAdmins.GetGroupAdminRequests, TeacherERP.Application.Features.TeacherRequests.Review, TeacherERP.Application.Features.SessionContent.UploadSessionAttachment, TeacherERP.Application.Features.Users.Commands.Auth.Login, TeacherERP.Application.Features.Students.UnblacklistStudent, TeacherERP.Application.Features.Sessions.Logout, TeacherERP.Application.Features.Enrollment.GetMyEnrollments (+29 more)

### Community 8 - "CreateAssistantCommandHandler"
Cohesion: 0.04
Nodes (17): TeacherERP.Application.Features.Schedule.RestoreSession, TeacherERP.Tests.Integration.Sessions, TeacherERP.Tests.Unit.Api, TeacherERP.Application.Features.Tenants.UpdateTenantSettings, TeacherERP.Application.Features.Tenants.GetTenantSettings, TeacherERP.API.Security, TeacherERP.Application.Sessions, TeacherERP.API.Controllers (+9 more)

### Community 9 - ".SaveChangesAsync"
Cohesion: 0.05
Nodes (57): IPagedRequest, Page, PageSize, PagedResult, HasNext, HasPrevious, TotalPages, PageRequest (+49 more)

### Community 10 - "Guid"
Cohesion: 0.07
Nodes (6): TeacherERP.Application.Features.DataRetention, TeacherERP.Infrastructure.Extensions, TeacherERP.Tests.Integration.Identity, TeacherERP.Tests.Integration.DataRetention, TeacherERP.Tests.Integration, TeacherERP.Infrastructure.Repositories

### Community 11 - "Session"
Cohesion: 0.08
Nodes (77): ApproveCompensationCommandHandler, AttendanceSettingsDto, GetAttendanceSettingsQuery, GetAttendanceSettingsQueryHandler, ManualAttendanceCommandHandler, RegenerateStaticCodeCommand, RegenerateStaticCodeCommandHandler, RejectCompensationCommandHandler (+69 more)

### Community 12 - "system_data"
Cohesion: 0.10
Nodes (7): CenterMembershipIntegrationTests, TestData, FinanceAuthorizationIntegrationTests, ScheduleOperationsIntegrationTests, SessionMoveIntegrationTests, StudentExperienceIntegrationTests, StudentTransferIntegrationTests

### Community 13 - ".Handle"
Cohesion: 0.04
Nodes (21): FixLoginSessionUserId, AddTenants, MoveCenterStateToMembership, AddCentersAndTeacherSubject, TestSeeder, AlignRolesWithTeam, AddAbsenceProcessing, AddWalkInSourceSession (+13 more)

### Community 14 - "DateTime"
Cohesion: 0.05
Nodes (17): UpdateGroupScheduleRequest, GetTeamQuery, GetTeamQueryHandler, TeamMemberDto, GetCurrentTenantQuery, GetMyTenantsQuery, TenantDto, IAbsenceRepository (+9 more)

### Community 16 - "IFinancialLedgerRepository"
Cohesion: 0.04
Nodes (31): AppDbContext, AttendanceRecords, AuditLogs, CenterInvitations, CenterJoinRequests, Centers, CenterTeachers, CurrentTenantId (+23 more)

### Community 17 - "PagedResult"
Cohesion: 0.07
Nodes (25): CreatePaymobIntentCommand, CreatePaymobIntentCommandValidator, CreatePaymobIntentCommandHandler, GetMyPaymentOptionsQuery, GetMyPaymentOptionsQueryHandler, GetPaymentOptionsQuery, GetPaymentOptionsQueryHandler, GetTeacherPaymentSettingsQuery (+17 more)

### Community 18 - "IGroupSessionRepository"
Cohesion: 0.04
Nodes (60): PagedRequestRules, AdminScanValidator, GetPendingCompensationsValidator, GetSessionAttendanceValidator, ManualAttendanceValidator, ScanAttendanceValidator, GetAuditLogValidator, CreateCenterInvitationValidator (+52 more)

### Community 19 - ".MarkAsUpdated"
Cohesion: 0.05
Nodes (9): TeacherERP.Application.Enrollment, TeacherERP.API.Middlewares, TeacherERP.Application, TeacherERP.Infrastructure.Settings, TeacherERP.Infrastructure, TeacherERP.Application.Features.TeacherRequests, TeacherERP.Application.Features.Users.Commands.Auth.ForgotPassword, TeacherERP.Infrastructure.Services (+1 more)

### Community 20 - "AttendanceRecord"
Cohesion: 0.03
Nodes (8): DependencyInjection, GoogleAuthService, CloudinarySettings, ApiKey, ApiSecret, CloudName, GoogleAuthSettings, ClientIds

### Community 21 - "IRequestHandler"
Cohesion: 0.07
Nodes (31): GetGroupAdminRequestsQuery, GetGroupAdminRequestsQueryHandler, GetGroupAdminsQuery, GetGroupAdminsQueryHandler, GroupAdminChangeResultDto, GroupAdminDto, GroupAdminRequestDto, RequestGroupAdminChangeCommand (+23 more)

### Community 22 - "Center"
Cohesion: 0.05
Nodes (26): Session, AbsencesProcessedAt, Attendances, BoardImageUrl, CancelReason, EndTime, Group, GroupId (+18 more)

### Community 23 - "Center"
Cohesion: 0.06
Nodes (28): UpdateGroupRequest, Group, Capacity, CenterId, DynamicJoinEnabled, DynamicJoinSecret, EducationStage, GradeLevel (+20 more)

### Community 24 - ".Handle"
Cohesion: 0.08
Nodes (15): FileStorageResource, FileStorageResourcePage, IFileStorage, StoredFile, IOrphanedFileCleanupJob, IRetentionCleanupRepository, OrphanedFileCleanupJob, QueryablePagingExtensions (+7 more)

### Community 25 - "IFinancialLedgerRepository.cs"
Cohesion: 0.07
Nodes (24): TransientAuthState, CodeExpiresAtUtc, Email, FailedAttempts, GrantExpiresAtUtc, GrantHash, Id, IsLocked (+16 more)

### Community 26 - "AttendanceController.cs"
Cohesion: 0.05
Nodes (3): CenterMembershipCleanupJob, SessionCleanupJob, TransientAuthStateCleanupJob

### Community 27 - "Fact"
Cohesion: 0.10
Nodes (4): AdministrationIntegrationTests, TeacherRequestsIntegrationTests, BaseApiControllerTests, TestController

### Community 28 - "CustomWebApplicationFactory"
Cohesion: 0.05
Nodes (44): CloudinaryDotNet (1.29.3), coverlet.collector (6.0.4), FluentValidation (12.1.1), FluentValidation.DependencyInjectionExtensions (12.1.1), Google.Apis.Auth (1.77.0), Hangfire.AspNetCore (1.8.25), Hangfire.Core (1.8.25), Hangfire.Dashboard.BasicAuthorization (1.0.2) (+36 more)

### Community 29 - "LoginSession"
Cohesion: 0.13
Nodes (5): IAttendanceCodeService, AttendanceCodeService, DynamicQrIntegrationTests, SeededTeacher, AttendanceCodeServiceTests

### Community 30 - "MembershipStatus"
Cohesion: 0.09
Nodes (9): CreateAssistantCommandHandler, WelcomeEmailFailedException, WelcomeEmailFailedException, IPasswordGenerator, TeacherProvisioningService, WelcomeEmailFailedException, PasswordGenerator, CreateCenterCommandHandlerTests (+1 more)

### Community 31 - "microsoft_aspnetcore_authorization"
Cohesion: 0.07
Nodes (13): TeacherERP.API.HealthChecks, CoreConfigurationHealthCheck, DatabaseHealthCheck, HangfireWorkerHealthCheck, CloudinaryConfigurationHealthCheck, GoogleAuthConfigurationHealthCheck, PaymobConfigurationHealthCheck, SmtpEndpointHealthCheck (+5 more)

### Community 32 - "PasswordController.cs"
Cohesion: 0.07
Nodes (8): MonthlyCollectionReportsController, PaymobAdjustmentReviewsController, PlatformCommissionsController, PaymobReturnController, PaymobWebhookController, TeacherMonthlyCollectionReportsController, TeacherPlatformCommissionsController, RolesController

### Community 33 - "IActionResult"
Cohesion: 0.10
Nodes (9): IEmailService, EmailService, EmailTemplates, MailSettings, DisplayName, Host, Mail, Password (+1 more)

### Community 34 - ".Handle"
Cohesion: 0.07
Nodes (11): MoveSessionRequest, RescheduleSessionRequest, RunningSessionInfo, ReplaceRefreshTokensWithLoginSessions, AddEnrollments, AddNotificationANDAttendanceSystem, AddGroupAdminsAndSessionContent, AddTeacherAccessRequests (+3 more)

### Community 35 - ".SaveChangesAsync"
Cohesion: 0.07
Nodes (30): NotificationService, Notification, IsRead, Message, RecipientUserId, RelatedId, TenantId, Type (+22 more)

### Community 36 - "CreateAssistantCommandHandler"
Cohesion: 0.05
Nodes (37): AuditAction, AssistantRemoved, AssistantSuspended, AssistantUnsuspended, AttendanceCorrected, CashPaymentCollected, CompensationApproved, CompensationRejected (+29 more)

### Community 37 - "GroupDto"
Cohesion: 0.11
Nodes (16): SubmitTeacherRequestCommandHandler, ForgotPasswordCommand, ForgotPasswordCommandHandler, ForgotPasswordResponseDto, ForgotPasswordValidator, CreateStudentCommandHandler, CreateStudentValidator, PendingStudentRegistrationData (+8 more)

### Community 38 - ".AddApplicationServices"
Cohesion: 0.15
Nodes (11): BulkCancelSessionsCommandHandler, BulkShiftSessionsCommandHandler, CancelSessionCommandHandler, RestoreSessionCommandHandler, RemoveAssistantCommandHandler, SetAssistantSuspensionCommandHandler, TenantContext, CurrentTenantAccessor (+3 more)

### Community 39 - "IOtpService"
Cohesion: 0.12
Nodes (7): EnrollmentRequestDto, RequestGroupByCodeCommand, RequestGroupByCodeCommandHandler, RequestGroupByCodeValidator, IGroupJoinCodeService, GroupJoinCodeService, GroupJoinCodeServiceTests

### Community 40 - "TeacherRequestReviewServiceTests"
Cohesion: 0.12
Nodes (8): GetMySessionsQuery, GetMySessionsQueryHandler, SessionDto, AccessTokenResult, IJwtProvider, ISessionRepository, TokenHasher, RefreshTokenRotationTests

### Community 41 - "GetTeacherDashboardQueryHandler"
Cohesion: 0.12
Nodes (6): GlobalExceptionHandler, AttendanceScanLogging, AdminScanCommandHandler, ScanAttendanceCommandHandler, GroupScanCommandHandler, AttendanceSettingsInfo

### Community 42 - "AbstractValidator"
Cohesion: 0.10
Nodes (19): GetMyGroupAdminQuery, GetMyGroupAdminQueryHandler, GroupAdminMeDto, GetSessionAttachmentsQuery, GetSessionAttachmentsQueryHandler, SessionAttachmentDto, UploadSessionAttachmentCommand, GroupAccessLevel (+11 more)

### Community 43 - "Session"
Cohesion: 0.10
Nodes (14): PendingTeacherRequestData, Email, FullName, Note, Subject, TimeZoneId, TeacherRequestToken, VerifyTeacherRequestCommandHandler (+6 more)

### Community 44 - "fluentvalidation"
Cohesion: 0.12
Nodes (17): LogoutAllCommand, LogoutAllCommandHandler, GoogleSignInCommand, GoogleSignInCommandHandler, GoogleSignInValidator, AuthResponseDto, LoginCommand, LoginCommandHandler (+9 more)

### Community 45 - "CancellationToken"
Cohesion: 0.19
Nodes (5): ActivateAccountCommand, ActivateAccountCommandHandler, VerifyResetOtpCommandHandler, IUserAccountService, UserAccountService

### Community 46 - "🏫 Smart Teacher ERP"
Cohesion: 0.08
Nodes (19): ApplicationUser, Attendances, DateOfBirth, FullName, Gender, Guardian, IsProfileComplete, LoginSessions (+11 more)

### Community 47 - "HttpGet"
Cohesion: 0.16
Nodes (6): ChangePasswordCommandHandler, ResetPasswordCommandHandler, SetPasswordCommandHandler, IPasswordChangedNotifier, PasswordCommandTests, ResetPasswordAlertTests

### Community 48 - "AuthResponseDto"
Cohesion: 0.07
Nodes (22): Transaction, Amount, IdempotencyKey, PaymentContextSessionId, ProcessedByAdminId, Provider, ProviderClientSecret, ProviderFailureCode (+14 more)

### Community 49 - ".Upload"
Cohesion: 0.18
Nodes (4): ApproveRequestCommand, AttendanceRecorder, WalkInApprovalTests, WalkInLocationRecorderTests

### Community 50 - "EnrollmentRequest"
Cohesion: 0.09
Nodes (10): CenterMembershipPolicy, DataRetentionPolicy, GroupSlotDto, GroupSlotInput, IGroupSchedule, Slots, TeacherRequestPolicy, SessionPolicy (+2 more)

### Community 52 - "GroupSessionRepository"
Cohesion: 0.11
Nodes (12): CompensationStaffIntegrationTests, ManualAttendanceIntegrationTests, CustomWebApplicationFactory, FileStorage, PaymobGateway, PaymobSettings, DashboardIntegrationTests, GroupPricingIntegrationTests (+4 more)

### Community 53 - ".Create"
Cohesion: 0.09
Nodes (20): Tenant, AutoApproveGroupAdminChanges, AutoApproveJoinRequests, IsListedInSearch, Name, OwnerId, Status, Subject (+12 more)

### Community 54 - ".ResolveAsync"
Cohesion: 0.08
Nodes (22): TeacherERP.Tests.Unit.Common, MonthlySubscription, Amount, GroupId, Month, PaymentTransactionId, StudentId, TenantId (+14 more)

### Community 55 - "TeacherERP.Application.Features.Users.Commands.Auth.Login"
Cohesion: 0.08
Nodes (20): TeacherERP.Application.Features.Notifications, TeacherERP.Application.Features.SessionContent, AdministrationErrors, CenterErrors, EnrollmentErrors, GroupAdminErrors, GroupErrors, NotificationErrors (+12 more)

### Community 56 - ".GetByOwnerIdAsync"
Cohesion: 0.15
Nodes (12): FinancialObligation, Amount, GroupId, GroupName, IsSettled, OutstandingAmount, PaidAmount, SessionId (+4 more)

### Community 57 - "SeededTeacher"
Cohesion: 0.08
Nodes (25): PaymentEvent, CurrentStatus, EventType, PayloadSha256, PreviousStatus, Provider, ProviderEventId, ProviderOccurredAtUtc (+17 more)

### Community 58 - "GroupAdminAttendanceIntegrationTests"
Cohesion: 0.12
Nodes (7): AddCenterMembershipRequests, SnapshotAccessor, AddStructuredGroupGrades, SnapshotAccessor, AddSessionOriginalStartTime, SnapshotAccessor, AppDbContextModelSnapshot

### Community 59 - "TenantContext"
Cohesion: 0.15
Nodes (16): AdminCenterDto, CreatedTeacherDto, CreateCenterCommand, CreateCenterCommandHandler, CreateCenterValidator, ApproveTeacherRequestByAdminCommand, ApproveTeacherRequestByAdminCommandHandler, ApproveTeacherRequestCommand (+8 more)

### Community 60 - "ICenterRepository"
Cohesion: 0.09
Nodes (19): TeacherRequestRules, TeacherAccessRequest, CreatedUserId, DecisionNote, Email, ExpiresAtUtc, FullName, Note (+11 more)

### Community 61 - "ApproveTeacherRequestCommand"
Cohesion: 0.09
Nodes (20): PaymobAdjustmentReview, CommissionReversalAmount, Currency, DecisionNote, ProviderAmount, ProviderEventId, ReviewedAtUtc, ReviewedByUserId (+12 more)

### Community 62 - "CompleteProfileValidator"
Cohesion: 0.18
Nodes (6): GetPendingCompensationsQuery, GetPendingCompensationsQueryHandler, GetSessionAttendanceQuery, GetSessionAttendanceQueryHandler, AttendanceListItem, PendingCompensationItem

### Community 63 - "SessionManager"
Cohesion: 0.17
Nodes (16): TeacherERP.Application.Features.Directory, DirectoryCenterDto, DirectoryCenterInfoDto, DirectoryGroupDto, DirectoryTeacherDto, TeacherProfileDto, GetTeacherProfileQuery, GetTeacherProfileQueryHandler (+8 more)

### Community 64 - "TeacherERP.Application.Features.Team"
Cohesion: 0.09
Nodes (23): ASPNETCORE_ENVIRONMENT, applicationUrl, commandName, dotnetRunMessages, environmentVariables, applicationUrl, commandName, dotnetRunMessages (+15 more)

### Community 65 - "TeacherERP.Application/DependencyInjection.cs"
Cohesion: 0.09
Nodes (21): LoginSession, AbsoluteExpiresAt, DeviceId, DeviceName, LastRefreshedAt, PreviousRefreshTokenHash, RefreshExpiresAt, RefreshTokenHash (+13 more)

### Community 66 - "SetFinancialDelegationCommandHandler.cs"
Cohesion: 0.16
Nodes (8): IPlatformCommissionSettingsService, PlatformCommissionSettingsDto, PlatformCommissionSettingsService, PlatformCommissionSettings, MonthlySubscriptionAmount, PerSessionAmount, UpdatedAtUtc, UpdatedByUserId

### Community 67 - "RevokeSessionCommand"
Cohesion: 0.09
Nodes (19): PlatformCommission, Amount, CollectionRequestedAtUtc, CollectionRequestedByUserId, GroupId, IsSettled, SessionId, SettledAtUtc (+11 more)

### Community 68 - "CancelSessionCommandHandler.cs"
Cohesion: 0.23
Nodes (6): IUserTimeZoneAccessor, TimeZoneId, SessionManager, PasswordChangedNotifier, PasswordChangedNotifierTests, UserTimeZonePersistenceTests

### Community 69 - "HangfireBasicAuthFilter"
Cohesion: 0.15
Nodes (13): UpdateCenterCommand, UpdateCenterCommandHandler, UpdateCenterValidator, ICenterProvisioningService, ICenterRepository, CenterProvisioningService, Center, Address (+5 more)

### Community 72 - "LoggingBehavior"
Cohesion: 0.10
Nodes (17): AttendanceRecord, CompensatesSessionId, RecordedByUserId, ScannedLatitude, ScannedLongitude, ScannedVia, Session, SessionId (+9 more)

### Community 73 - "GetAuditLogQuery"
Cohesion: 0.11
Nodes (13): CenterJoinRequest, CenterId, DecisionNote, LastNotifiedAtUtc, ReviewedAtUtc, ReviewedByUserId, Status, TenantId (+5 more)

### Community 74 - "GetSessionsQuery"
Cohesion: 0.14
Nodes (11): CompletedProfileHandler, CompletedProfileRequirement, TenantHandler, TenantRequirement, AllowedRoles, RequiresFinancialDelegation, IProfileCompletionChecker, TenantRole (+3 more)

### Community 75 - "CachedRegistrationData"
Cohesion: 0.16
Nodes (5): TeacherERP.Tests.Integration.Common, GroupAdminAttendanceIntegrationTests, Scenario, HealthChecksIntegrationTests, RefreshTokenRotationIntegrationTests

### Community 76 - "GetMyNotificationsQuery"
Cohesion: 0.20
Nodes (3): ScheduleSlotCandidate, SessionTimeCalculator, AddStudentProfile

### Community 78 - ".Handle"
Cohesion: 0.16
Nodes (3): INotificationRepository, NotificationShareItem, NotificationRepository

### Community 79 - "SessionValidator"
Cohesion: 0.13
Nodes (14): ISessionAttachmentRepository, SessionAttachment, Caption, ContentType, FileName, Kind, PublicId, Session (+6 more)

### Community 81 - "PageRequest"
Cohesion: 0.11
Nodes (16): EnrollmentRequest, DecidedAt, DecidedByUserId, DecisionNote, Group, GroupId, SourceLatitude, SourceLongitude (+8 more)

### Community 82 - "Task"
Cohesion: 0.11
Nodes (15): EnrollmentStatus, Active, Ended, GroupEnrollment, ApprovedByUserId, EndedAt, EnrolledAt, Group (+7 more)

### Community 83 - "RescheduleSessionValidator.cs"
Cohesion: 0.20
Nodes (12): CompleteProfileCommand, CompleteProfileCommandHandler, GetMyProfileQuery, GetMyProfileQueryHandler, GuardianDto, ProfileDto, ICenterJoinRequestNotifier, IUserRepository (+4 more)

### Community 84 - "Error"
Cohesion: 0.19
Nodes (3): Callback, PaymobErrors, PaymobPaymentService

### Community 85 - ".ApproveAsync"
Cohesion: 0.19
Nodes (4): UserTime, CreateTeacherCommand, CreateTeacherValidator, CreateTeacherValidatorTests

### Community 86 - "CreateAssistantValidator.cs"
Cohesion: 0.12
Nodes (14): PaymobSettings, ApiKey, BaseUrl, Currency, HmacNullValue, HmacSecret, InitializationLeaseSeconds, IntegrationIds (+6 more)

### Community 87 - "TeacherAccessRequestStatus"
Cohesion: 0.26
Nodes (5): CenterTeacherLinkDto, LinkTeacherToCenterCommand, LinkTeacherToCenterCommandHandler, LinkTeacherToCenterValidator, LinkTeacherToCenterCommandHandlerTests

### Community 88 - "AttendanceSettingsDto"
Cohesion: 0.12
Nodes (13): UpdateTeacherPaymentSettingsForm, InstaPayLink, QrImage, CreatePaymobIntentBody, SubmitInstaPayReceiptForm, Amount, Receipt, SessionId (+5 more)

### Community 89 - "BaseApiController"
Cohesion: 0.16
Nodes (5): SessionJwtBearerEvents, ProfileCompletionChecker, ISessionValidator, SessionValidator, SessionJwtRevocationIntegrationTests

### Community 90 - "GoogleSignInValidator.cs"
Cohesion: 0.12
Nodes (15): 🔒 1. Advanced Security & Session Control, 📍 2. Hybrid Anti-Cheat Attendance System, 💰 3. Strict Financial Ledger & Privacy, Ahmed Al-Desouki, 🏛️ Architecture, 👨‍💻 Author, Center Management System, 🛡️ Core Features (+7 more)

### Community 91 - "ResetPasswordValidator.cs"
Cohesion: 0.17
Nodes (9): FileNames, DetectedFileType, Jpeg, Pdf, Png, Unknown, Webp, FileSignature (+1 more)

### Community 92 - "SessionAttachment"
Cohesion: 0.25
Nodes (10): AdministratorDashboardDto, GetAdministratorDashboardQuery, GetAdministratorDashboardQueryHandler, MonthlyCollectionItem, MonthlyCollectionReport, MonthlyCollectionTotals, MonthlyOutstandingDebtItem, MonthlyPaymentMethodSummary (+2 more)

### Community 93 - "TeacherProvisioningServiceTests"
Cohesion: 0.25
Nodes (8): TeacherDashboardDto, GetTeacherDashboardQuery, GetTeacherDashboardQueryHandler, AdministratorDashboardOperationalStats, DashboardUpcomingSession, IDashboardRepository, TeacherDashboardOperationalStats, DashboardRepository

### Community 94 - ".ResolveAsync"
Cohesion: 0.13
Nodes (12): CenterInvitation, AcceptedAtUtc, AcceptedByTenantId, CenterId, CreatedByUserId, ExpiresAtUtc, Status, TokenHash (+4 more)

### Community 95 - "ProfileAuthorizationResultHandler.cs"
Cohesion: 0.12
Nodes (14): GuardianInfo, Name, PhoneNumber, Relation, GuardianRelation, Aunt, Brother, Father (+6 more)

### Community 96 - "TeacherERP.Application.Features.Users.Commands.Auth.Login"
Cohesion: 0.22
Nodes (4): AttendanceScanLoggingTests, CapturedLog, CapturingLogger, Entries

### Community 97 - "CenterJoinRequestListItem"
Cohesion: 0.27
Nodes (7): ScanMethod, AdminScan, ManualEntry, SelfScan, System, Scenario, WalkInLocationIntegrationTests

### Community 98 - "CreatedTeacherDto"
Cohesion: 0.24
Nodes (9): IStudentRepository, StudentActiveGroupItem, StudentDetailsItem, StudentListItem, MembershipStatus, Active, Blacklisted, Suspended (+1 more)

### Community 99 - ".GetByIdAsync"
Cohesion: 0.15
Nodes (11): CenterTeacher, Center, CenterId, Origin, Status, TenantId, CenterTeacherOrigin, Administrator (+3 more)

### Community 100 - "GroupScheduleSlot"
Cohesion: 0.26
Nodes (9): CenterDto, CreateMyCenterCommand, CreateMyCenterCommandHandler, CreateMyCenterValidator, GetMyCentersQuery, GetMyCentersQueryHandler, AcceptCenterInvitationCommand, AcceptCenterInvitationValidator (+1 more)

### Community 101 - ".AddInfrastructureServices"
Cohesion: 0.18
Nodes (5): IAbsenceProcessingJob, IAbsenceProcessor, ISessionGenerationJob, AbsenceProcessingJob, SessionGenerationJob

### Community 102 - "MoveStudentCommandHandler.cs"
Cohesion: 0.18
Nodes (3): LoggingBehavior, ValidationBehavior, ProfileRequirement

### Community 103 - "GroupScanValidator.cs"
Cohesion: 0.18
Nodes (7): CurrentUserService, SessionId, TimeZoneId, UserId, HttpUserTimeZoneAccessor, TimeZoneId, CurrentUserTimeZoneTests

### Community 104 - ".GetByIdAsync"
Cohesion: 0.18
Nodes (7): TeacherERP.Application.Features.Team, TeacherERP.Application.Features.Team.CreateAssistant, AssistantDto, CreateAssistantCommand, CreateAssistantValidator, TeamErrors, TeamPolicy

### Community 107 - ".ConfigureWebHost"
Cohesion: 0.27
Nodes (5): PaymobIntentCommandData, PaymobGatewayTests, RecordingLogger, Messages, StubHandler

### Community 108 - "IPagedRequest"
Cohesion: 0.18
Nodes (6): CreateGroupValidator, GroupPricingRules, GroupSlotsRules, UpdateGroupValidator, UpdateGroupScheduleValidator, SchoolGradeTests

### Community 109 - ".AddApiRateLimiting"
Cohesion: 0.20
Nodes (7): GroupScheduleSlot, Day, DurationMinutes, Group, GroupId, StartTime, TenantId

### Community 116 - "RequestGroupByCodeCommandHandler.cs"
Cohesion: 0.31
Nodes (5): MyEnrollmentDto, MyEnrollmentsDto, GetMyEnrollmentsQuery, GetMyEnrollmentsQueryHandler, MyEnrollmentItem

### Community 117 - "IServiceCollection"
Cohesion: 0.22
Nodes (7): SubmitTeacherRequestCommand, SubmitTeacherRequestValidator, TeacherRequestOtpDto, TeacherRequestQrDto, VerifyTeacherRequestCommand, VerifyTeacherRequestValidator, TeacherRequestBasicsTests

### Community 118 - "PlatformCommissionSettings"
Cohesion: 0.22
Nodes (8): IPaymobGateway, PaymobIntentData, PaymobWebhookOutcome, Accepted, Duplicate, Ignored, InvalidPayload, InvalidSignature

### Community 119 - ".VerifyAsync"
Cohesion: 0.25
Nodes (4): TeacherERP.Application.Files, FileStorageErrors, IStoredFileCleanupJob, StoredFileCleanupJob

### Community 121 - "ForwardedHeadersExtensions.cs"
Cohesion: 0.47
Nodes (5): GetStudentWeeklyScheduleQuery, GetStudentWeeklyScheduleQueryHandler, StudentWeeklyGroupScheduleDto, StudentWeeklyScheduleDto, StudentWeeklyScheduleSlotDto

### Community 122 - "GetNotificationWhatsAppShareQuery"
Cohesion: 0.36
Nodes (7): ApproveTeacherRequestValidator, PreviewTeacherRequestQuery, PreviewTeacherRequestQueryHandler, PreviewTeacherRequestValidator, RejectTeacherRequestCommand, RejectTeacherRequestValidator, TeacherRequestPreviewDto

### Community 124 - "CloudinarySettings"
Cohesion: 0.50
Nodes (5): IStudentWeeklyScheduleRepository, StudentWeeklyScheduleData, StudentWeeklyScheduleGroupItem, StudentWeeklyScheduleSlotItem, StudentWeeklyScheduleRepository

### Community 125 - "GetSessionsQuery"
Cohesion: 0.29
Nodes (6): AuditLog, Action, ActorUserId, Reason, TargetUserId, TenantId

### Community 126 - ".TokenValidated"
Cohesion: 0.57
Nodes (3): CreateTeacherCommandHandler, ITeacherProvisioningService, CreateTeacherCommandHandlerTests

### Community 127 - "PaymobWebhookOutcome"
Cohesion: 0.52
Nodes (4): ManualAttendanceCommand, AttendanceRequest, AttendanceResultDto, IAttendanceRecorder

### Community 128 - "PaymobWebhookOutcome"
Cohesion: 0.29
Nodes (6): PaymobIntegrationKey, CaggOnlineNonShopify, CashCashChannel, UigOnlineNewNonShopify, VpcOnlineNonShopify, VpcShopify

### Community 134 - "RoleNames.cs"
Cohesion: 0.67
Nodes (4): GetStudentDetailsQuery, GetStudentDetailsQueryHandler, StudentActiveGroupDto, StudentDetailsDto

### Community 135 - "HangfireBasicAuthFilter"
Cohesion: 0.33
Nodes (6): AttendanceOutcome, AlreadyRecorded, Corrected, PendingApproval, PendingCompensation, Recorded

### Community 138 - "SessionCleanupJob"
Cohesion: 0.70
Nodes (4): GetTeacherRequestStatusQuery, GetTeacherRequestStatusQueryHandler, GetTeacherRequestStatusValidator, TeacherRequestStatusDto

### Community 141 - "JwtSettings"
Cohesion: 0.50
Nodes (3): ResetTokenDto, VerifyResetOtpCommand, VerifyResetOtpValidator

### Community 142 - "AppDbContextFactory"
Cohesion: 0.50
Nodes (4): CenterTeacherStatus, Active, Pending, Removed

### Community 145 - ".Meters"
Cohesion: 0.67
Nodes (3): BulkCancelSessionsCommand, BulkCancelSessionsValidator, BulkResultDto

## Knowledge Gaps
- **640 isolated node(s):** `SessionId`, `TimeZoneId`, `UserId`, `TimeZoneId`, `Messages` (+635 more)
  These have ≤1 connection - possible missing edges or undocumented components. (Counts symbols only; 902 node(s) total have ≤1 connection when file, concept and rationale nodes are included.)
- **37 thin communities (<3 nodes) omitted from report** — run `graphify query` to explore isolated nodes.

## Suggested Questions
_Questions this graph is uniquely positioned to answer:_

- **Why does `TeacherERP.Application.Interfaces` connect `TeacherERP.Domain.Shared` to `IActionResult`, `Task`, `system`, `ApplicationUser`, `TeacherERP.Application.Interfaces`, `CreateAssistantCommandHandler`, `Guid`, `Session`, `EnrollmentRequestStatus`, `.MarkAsUpdated`, `Updatecenterrequest.cs`, `IRequestHandler`, `.Handle`, `MembershipStatus`, `IActionResult`, `IOtpService`, `TeacherRequestReviewServiceTests`, `AbstractValidator`, `SessionManager`, `SetFinancialDelegationCommandHandler.cs`, `CancelSessionCommandHandler.cs`, `GetSessionsQuery`, `RescheduleSessionValidator.cs`, `BaseApiController`, `SessionAttachment`, `TeacherProvisioningServiceTests`, `CreatedTeacherDto`, `.AddInfrastructureServices`, `LogoutValidator.cs`, `RequestGroupByCodeCommandHandler.cs`, `PlatformCommissionSettings`, `.VerifyAsync`, `ForwardedHeadersExtensions.cs`, `CloudinarySettings`, `PaymobWebhookOutcome`?**
  _High betweenness centrality (0.080) - this node is a cross-community bridge._
- **What connects `SessionId`, `TimeZoneId`, `UserId` to the rest of the system?**
  _640 weakly-connected nodes found - possible documentation gaps or missing edges._
- **Should `TeacherERP.Domain.Shared` be split into smaller, more focused modules?**
  _Cohesion score 0.019041248606466 - nodes in this community are weakly interconnected._
- **Why does `Result` connect `Result` to `IActionResult`, `Task`, `RoleNames.cs`, `.SaveChangesAsync`, `.EnsurePaymentEventsAreAppendOnly`, `Session`, `SessionCleanupJob`, `JwtSettings`, `DateTime`, `PagedResult`, `.Meters`, `.MarkAsUpdated`, `IRequestHandler`, `Center`, `PasswordController.cs`, `GroupDto`, `IOtpService`, `TeacherRequestReviewServiceTests`, `AbstractValidator`, `Session`, `fluentvalidation`, `CancellationToken`, `HttpGet`, `.Upload`, `TeacherERP.Application.Features.Users.Commands.Auth.Login`, `.GetByOwnerIdAsync`, `TenantContext`, `CompleteProfileValidator`, `SessionManager`, `SetFinancialDelegationCommandHandler.cs`, `HangfireBasicAuthFilter`, `GetMyNotificationsQuery`, `RescheduleSessionValidator.cs`, `.ApproveAsync`, `TeacherAccessRequestStatus`, `SessionAttachment`, `TeacherProvisioningServiceTests`, `GroupScheduleSlot`, `.GetByIdAsync`, `ControllerBase`, `RequestGroupByCodeCommandHandler.cs`, `IServiceCollection`, `PlatformCommissionSettings`, `ForwardedHeadersExtensions.cs`, `GetNotificationWhatsAppShareQuery`, `.TokenValidated`, `PaymobWebhookOutcome`?**
  _High betweenness centrality (0.053) - this node is a cross-community bridge._
- **Should `IActionResult` be split into smaller, more focused modules?**
  _Cohesion score 0.037467700258397935 - nodes in this community are weakly interconnected._
- **Why does `TeacherERP.Domain.Entities` connect `ApplicationUser` to `TeacherERP.Domain.Shared`, `IActionResult`, `system`, `TeacherERP.Application.Interfaces`, `CreateAssistantCommandHandler`, `.EnsurePaymentEventsAreAppendOnly`, `Guid`, `Session`, `CloudinarySettings`, `EnrollmentRequestStatus`, `.MarkAsUpdated`, `IRequestHandler`, `IFinancialLedgerRepository.cs`, `TeacherRequestReviewServiceTests`, `AbstractValidator`, `.ResolveAsync`, `SeededTeacher`, `ApproveTeacherRequestCommand`, `SessionManager`, `SetFinancialDelegationCommandHandler.cs`, `RevokeSessionCommand`, `RescheduleSessionValidator.cs`, `SessionAttachment`, `CreatedTeacherDto`, `RequestGroupByCodeCommandHandler.cs`, `IServiceCollection`, `PlatformCommissionSettings`, `CloudinarySettings`, `PaymobWebhookOutcome`?**
  _High betweenness centrality (0.046) - this node is a cross-community bridge._
- **Should `Task` be split into smaller, more focused modules?**
  _Cohesion score 0.03350441973196464 - nodes in this community are weakly interconnected._