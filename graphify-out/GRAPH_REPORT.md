# Graph Report - TeacherERP  (2026-11-27)

## Corpus Check
- cluster-only mode — file stats not available

## Summary
- 4579 nodes · 17687 edges · 147 communities (118 shown, 29 thin omitted)
- Extraction: 90% EXTRACTED · 10% INFERRED · 0% AMBIGUOUS · INFERRED: 1824 edges (avg confidence: 0.85)
- Token cost: 0 input · 0 output

## Graph Freshness
- Built from commit: `98d0364f`
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
- Exception
- SubmitInstaPayReceiptForm
- PaymentType
- CenterMembershipPolicy.cs
- SchedulePolicy.cs
- RoleNames.cs
- HangfireBasicAuthFilter
- IReadOnlyList
- SessionCleanupJob
- CurrentTenantAccessor
- CloudinarySettings
- JwtSettings
- AppDbContextFactory
- NotificationErrors.cs
- .RunAsync
- GroupAccessLevel
- ITransientAuthStateCleanupJob

## God Nodes (most connected - your core abstractions)
1. `Result` - 468 edges
2. `TeacherERP.Application.Interfaces` - 319 edges
3. `TeacherERP.Domain.Shared` - 317 edges
4. `TeacherERP.Domain.Entities` - 259 edges
5. `AppDbContext` - 126 edges
6. `TeacherERP.Application.Common` - 121 edges
7. `PagedResult` - 106 edges
8. `ICurrentTenantAccessor` - 95 edges
9. `ApplicationUser` - 75 edges
10. `IUnitOfWork` - 74 edges

## Surprising Connections (you probably didn't know these)
- `🔒 1. Advanced Security & Session Control` --references--> `ApplicationUser`  [INFERRED]
  README.md → TeacherERP.Domain/Entities/Users/User.cs
- `🗄️ Database Schema` --references--> `TenantSettings`  [INFERRED]
  README.md → TeacherERP.Domain/Entities/Tenants/TenantSettings.cs
- `🗄️ Database Schema` --references--> `ApplicationUser`  [INFERRED]
  README.md → TeacherERP.Domain/Entities/Users/User.cs
- `WalkInApprovalTests` --calls--> `TenantContext`  [INFERRED]
  TeacherERP.Tests/Unit/Attendance/WalkInLocationTests.cs → TeacherERP.Application/Interfaces/Tenants/ICurrentTenantAccessor.cs
- `BulkCancelSessionsCommandHandlerTests` --calls--> `TenantContext`  [INFERRED]
  TeacherERP.Tests/Unit/Sessions/CancelSessionHandlersTests.cs → TeacherERP.Application/Interfaces/Tenants/ICurrentTenantAccessor.cs

## Import Cycles
- None detected.

## Communities (147 total, 29 thin omitted)

### Community 0 - "TeacherERP.Domain.Shared"
Cohesion: 0.02
Nodes (67): TeacherERP.Application.Features.Attendance.RejectCompensation, TeacherERP.Application.Features.Attendance, TeacherERP.Application.Features.Finance.GetMyPaymentOptions, TeacherERP.Application.Features.Team.GetTeam, TeacherERP.Application.Features.GroupAdmins.GetMyGroupAdmin, TeacherERP.Application.Features.TeacherRequests.Review, TeacherERP.Application.Features.Students.GetStudentDetails, TeacherERP.Application.Features.Finance.GetPaymentOptions (+59 more)

### Community 1 - "IActionResult"
Cohesion: 0.05
Nodes (26): AdministrationController, AttendanceController, AuditController, AuthController, PasswordController, BaseApiController, CentersController, DashboardController (+18 more)

### Community 2 - "Task"
Cohesion: 0.03
Nodes (25): TeacherERP.Application.Enrollment, TeacherERP.Tests.Unit, TeacherERP.Application.Features.Finance.CreatePaymobIntent, TeacherERP.Application.Features.Enrollment.ApproveRequest, TeacherERP.Application.Attendance, TeacherERP.Application.Absence, TeacherERP.Application.Features.Attendance.ScanAttendance, TeacherERP.Application.Features.Centers.CreateCenter (+17 more)

### Community 3 - "TeacherERP.Application.Scheduling"
Cohesion: 0.03
Nodes (20): MoveStudentRequest, LinkTeacherToCenterRequest, UpdateGroupRequest, MoveSessionRequest, AttendanceSettingsInfo, RunningSessionInfo, AuditLogItem, EnrollmentRequestListItem (+12 more)

### Community 4 - "system"
Cohesion: 0.03
Nodes (93): ApproveCompensationCommand, DisplayCodeDto, GetDisplayCodeQuery, GetDisplayCodeQueryHandler, RejectCompensationCommand, RejectCenterJoinRequestCommand, RejectCenterJoinRequestValidator, CancelRequestCommand (+85 more)

### Community 5 - "ApplicationUser"
Cohesion: 0.03
Nodes (31): CenterMembershipPolicy, SessionPolicy, InitialCreate, FixLoginSessionUserId, MoveCenterStateToMembership, AddCentersAndTeacherSubject, AddGroupScheduleAndSessions, TestSeeder (+23 more)

### Community 6 - "Result"
Cohesion: 0.04
Nodes (17): IAbsenceRepository, MyEnrollmentItem, FinancialTransactionInfo, PlatformCommissionSummary, GroupJoinCodeLookup, TeamMemberItem, TenantMembershipInfo, TenantStatus (+9 more)

### Community 7 - "TeacherERP.Application.Interfaces"
Cohesion: 0.03
Nodes (16): TeacherERP.API.Middlewares, TeacherERP.Infrastructure.Settings, TeacherERP.Application.Features.Users.Commands.VerifyOtp, TeacherERP.Application.Centers, TeacherERP.Infrastructure, TeacherERP.Tests.Integration.Sessions, TeacherERP.Application.Features.Users.Commands.Auth.VerifyResetOtp, TeacherERP.Application.Features.Team.CreateAssistant (+8 more)

### Community 8 - "CreateAssistantCommandHandler"
Cohesion: 0.04
Nodes (29): MonthlyCollectionReportsController, PaymobAdjustmentReviewsController, PlatformCommissionsController, RequestPlatformCommissionCollection, SettlePlatformCommissionRequest, TeacherMonthlyCollectionReportsController, TeacherPlatformCommissionsController, PageRequest (+21 more)

### Community 9 - ".SaveChangesAsync"
Cohesion: 0.03
Nodes (18): TeacherERP.Application.Features.TeacherRequests.Submit, TeacherERP.Application.Features.GroupAdmins.GetGroupAdminRequests, TeacherERP.Application.Features.Schedule.GetSessions, TeacherERP.Application.Features.Enrollment.GetMyEnrollments, TeacherERP.Application.Features.Groups.GetGroups, TeacherERP.Application.Features.Directory.SearchDirectory, TeacherERP.Application.Features.Finance.GetPendingInstaPayReceipts, TeacherERP.Application.Features.SessionContent.GetSessionAttachments (+10 more)

### Community 10 - "Guid"
Cohesion: 0.08
Nodes (11): IAbsenceProcessor, TenantContext, CenterMembershipIntegrationTests, TestData, DashboardIntegrationTests, FinanceAuthorizationIntegrationTests, GroupGradeIntegrationTests, SessionGenerationIntegrationTests (+3 more)

### Community 11 - "Session"
Cohesion: 0.06
Nodes (57): TeacherERP.Application.Features.Directory, IPagedRequest, Page, PageSize, PagedResult, HasNext, HasPrevious, TotalPages (+49 more)

### Community 12 - "system_data"
Cohesion: 0.06
Nodes (8): IExternalLoginService, IResetGrantService, ResetGrant, ITransientAuthStateStore, CenterMembershipRepository, TransientAuthStateStore, ExternalLoginService, ResetGrantService

### Community 14 - "DateTime"
Cohesion: 0.12
Nodes (62): ApproveCompensationCommandHandler, ManualAttendanceCommandHandler, RegenerateStaticCodeCommandHandler, RejectCompensationCommandHandler, CancelCenterJoinRequestCommandHandler, RejectCenterJoinRequestCommandHandler, ApproveRequestCommandHandler, CancelRequestCommandHandler (+54 more)

### Community 16 - "IFinancialLedgerRepository"
Cohesion: 0.05
Nodes (31): AppDbContext, AttendanceRecords, AuditLogs, CenterInvitations, CenterJoinRequests, Centers, CenterTeachers, CurrentTenantId (+23 more)

### Community 17 - "PagedResult"
Cohesion: 0.05
Nodes (37): Transaction, Amount, IdempotencyKey, PaymentContextSessionId, ProcessedByAdminId, Provider, ProviderClientSecret, ProviderFailureCode (+29 more)

### Community 18 - "IGroupSessionRepository"
Cohesion: 0.06
Nodes (32): AdminCenterDto, CreateCenterCommand, CreateCenterCommandHandler, CreateCenterValidator, LinkTeacherToCenterCommand, LinkTeacherToCenterValidator, CenterLocationPolicy, GeoDistance (+24 more)

### Community 19 - ".MarkAsUpdated"
Cohesion: 0.08
Nodes (19): ApproveInstaPayReceiptCommand, ApproveInstaPayReceiptCommandHandler, CollectCashPaymentCommand, CollectCashPaymentCommandValidator, CollectCashPaymentCommandHandler, GetSessionAttachmentsQuery, GetSessionAttachmentsQueryHandler, SessionAttachmentDto (+11 more)

### Community 21 - "IRequestHandler"
Cohesion: 0.11
Nodes (9): TeacherERP.Application.SessionContent, TeacherERP.API.Security, TeacherERP.API.Controllers, TeacherERP.API.Extensions, ResolvePaymobAdjustmentBody, UpdatePlatformCommissionSettingsRequest, UpdateGroupJoinCodesRequest, RateLimitPolicies (+1 more)

### Community 22 - "Center"
Cohesion: 0.09
Nodes (5): TeacherERP.Application.Features.Profile.CompleteProfile, TeacherERP.Application.Features.Groups.UpdateGroup, TeacherERP.Application.Features.Profile, TeacherERP.Domain.ValueObjects, TeacherERP.Tests.Integration

### Community 23 - "Center"
Cohesion: 0.10
Nodes (6): IAttendanceCodeService, AttendanceCodeService, AdministrationIntegrationTests, DynamicQrIntegrationTests, SeededTeacher, AttendanceCodeServiceTests

### Community 24 - ".Handle"
Cohesion: 0.09
Nodes (30): CenterTeacherLinkDto, LinkTeacherToCenterCommandHandler, CenterDto, CenterInvitationToken, CenterInvitationDto, CreateMyCenterCommand, CreateMyCenterCommandHandler, GetMyCentersQuery (+22 more)

### Community 25 - "IFinancialLedgerRepository.cs"
Cohesion: 0.05
Nodes (44): CloudinaryDotNet (1.29.3), coverlet.collector (6.0.4), FluentValidation (12.1.1), FluentValidation.DependencyInjectionExtensions (12.1.1), Google.Apis.Auth (1.77.0), Hangfire.AspNetCore (1.8.25), Hangfire.Core (1.8.25), Hangfire.Dashboard.BasicAuthorization (1.0.2) (+36 more)

### Community 26 - "AttendanceController.cs"
Cohesion: 0.07
Nodes (38): PagedRequestRules, GetPendingCompensationsValidator, GetSessionAttendanceValidator, GetAuditLogValidator, AcceptCenterInvitationValidator, ApproveCenterJoinRequestValidator, CancelCenterJoinRequestCommand, CancelCenterJoinRequestValidator (+30 more)

### Community 27 - "Fact"
Cohesion: 0.09
Nodes (17): EnrollmentRequestDto, RequestGroupCommand, RequestGroupValidator, RequestGroupByCodeCommand, RequestGroupByCodeCommandHandler, RequestGroupByCodeValidator, IGroupJoinCodeService, GroupJoinCodeService (+9 more)

### Community 28 - "CustomWebApplicationFactory"
Cohesion: 0.09
Nodes (23): CreateGroupCommand, CreateGroupCommandHandler, GetGroupQuery, GetGroupQueryHandler, GroupDto, GroupSlotDto, IGroupPricing, MonthlyPrice (+15 more)

### Community 29 - "LoginSession"
Cohesion: 0.06
Nodes (29): TeacherERP.Tests.Unit.Common, AuditLog, Action, ActorUserId, Reason, TargetUserId, TenantId, MonthlySubscription (+21 more)

### Community 30 - "MembershipStatus"
Cohesion: 0.10
Nodes (9): ResetPasswordCommandHandler, AccessTokenResult, IJwtProvider, ISessionRepository, SessionManager, TokenHasher, SessionRepository, JwtProvider (+1 more)

### Community 31 - "microsoft_aspnetcore_authorization"
Cohesion: 0.06
Nodes (10): TeacherERP.Application.Features.Schedule.BulkCancelSessions, TeacherERP.Application.Scheduling, TeacherERP.Application.Features.Schedule, TeacherERP.Application.Features.Schedule.MoveSession, TeacherERP.Application.Features.Schedule.BulkShiftSessions, TeacherERP.Application.Features.SessionContent.DeleteSessionAttachment, TeacherERP.Application.Features.Schedule.CancelSession, TeacherERP.Application.Features.Groups.CreateGroup (+2 more)

### Community 32 - "PasswordController.cs"
Cohesion: 0.07
Nodes (11): MyEnrollmentDto, RescheduleSessionRequest, PlatformCommissionCollectionRequestResult, SessionListItem, SessionContentPolicy, ReplaceRefreshTokensWithLoginSessions, AddTenants, AddCashFinancialLedger (+3 more)

### Community 33 - "IActionResult"
Cohesion: 0.09
Nodes (16): PendingTeacherRequestData, Email, FullName, Note, Subject, TimeZoneId, TeacherRequestToken, VerifyTeacherRequestCommandHandler (+8 more)

### Community 34 - ".Handle"
Cohesion: 0.08
Nodes (20): Group, Capacity, CenterId, DynamicJoinEnabled, DynamicJoinSecret, EducationStage, GradeLevel, GradeNumber (+12 more)

### Community 35 - ".SaveChangesAsync"
Cohesion: 0.14
Nodes (14): GetGroupAdminsQuery, GetGroupAdminsQueryHandler, GroupAdminChangeResultDto, GroupAdminDto, RequestGroupAdminChangeCommand, RequestGroupAdminChangeValidator, AddExtraSessionCommand, BulkCancelSessionsCommand (+6 more)

### Community 36 - "CreateAssistantCommandHandler"
Cohesion: 0.10
Nodes (11): INotificationRepository, NotificationShareItem, NotificationService, Notification, IsRead, Message, RecipientUserId, RelatedId (+3 more)

### Community 37 - "GroupDto"
Cohesion: 0.08
Nodes (22): ScanMethod, AdminScan, ManualEntry, SelfScan, System, EnrollmentRequest, DecidedAt, DecidedByUserId (+14 more)

### Community 38 - ".AddApplicationServices"
Cohesion: 0.09
Nodes (20): GroupAdminRequestItem, GroupAdminRequest, DecidedAt, DecidedByUserId, DecisionNote, Group, GroupId, RequestedByUserId (+12 more)

### Community 39 - "IOtpService"
Cohesion: 0.13
Nodes (3): IEmailService, EmailService, EmailTemplates

### Community 41 - "GetTeacherDashboardQueryHandler"
Cohesion: 0.11
Nodes (16): AssistantDto, CreateAssistantCommand, CreateAssistantCommandHandler, CreateAssistantValidator, GetTeamQuery, GetTeamQueryHandler, ResendWelcomeCommand, ResendWelcomeCommandHandler (+8 more)

### Community 42 - "AbstractValidator"
Cohesion: 0.07
Nodes (30): AuditAction, AttendanceCorrected, CashPaymentCollected, CompensationApproved, CompensationRejected, FinancialDelegationGranted, FinancialDelegationRevoked, GroupAdminAppointed (+22 more)

### Community 43 - "Session"
Cohesion: 0.20
Nodes (3): VerifyResetOtpCommandHandler, IUserAccountService, UserAccountService

### Community 45 - "CancellationToken"
Cohesion: 0.14
Nodes (13): CashPaymentRequest, FinancialObligation, Amount, GroupId, GroupName, IsSettled, OutstandingAmount, PaidAmount (+5 more)

### Community 46 - "🏫 Smart Teacher ERP"
Cohesion: 0.08
Nodes (25): PaymentEvent, CurrentStatus, EventType, PayloadSha256, PreviousStatus, Provider, ProviderEventId, ProviderOccurredAtUtc (+17 more)

### Community 47 - "HttpGet"
Cohesion: 0.08
Nodes (19): PlatformCommission, Amount, CollectionRequestedAtUtc, CollectionRequestedByUserId, GroupId, IsSettled, SessionId, SettledAtUtc (+11 more)

### Community 48 - "AuthResponseDto"
Cohesion: 0.12
Nodes (10): SubmitTeacherRequestCommandHandler, CreateStudentCommandHandler, CreateStudentValidator, PendingStudentRegistrationData, Email, FullName, PasswordHash, VerifyOtpCommandHandler (+2 more)

### Community 49 - ".Upload"
Cohesion: 0.25
Nodes (4): Scenario, WalkInLocationIntegrationTests, Scenario, TeacherPaymentsIntegrationTests

### Community 50 - "EnrollmentRequest"
Cohesion: 0.14
Nodes (21): CreatedTeacherDto, ApproveTeacherRequestByAdminCommand, ApproveTeacherRequestByAdminCommandHandler, ListTeacherRequestsQuery, ListTeacherRequestsQueryHandler, RejectTeacherRequestByAdminCommand, RejectTeacherRequestByAdminCommandHandler, RejectTeacherRequestByAdminValidator (+13 more)

### Community 51 - "BaseEntity"
Cohesion: 0.15
Nodes (8): EducationStage, Preparatory, Primary, Secondary, Unspecified, SchoolGrade, ScheduleOperationsIntegrationTests, GroupPricingTests

### Community 52 - "GroupSessionRepository"
Cohesion: 0.16
Nodes (18): AdminScanCommand, AdminScanCommandHandler, AdminScanValidator, ManualAttendanceCommand, ManualAttendanceValidator, ScanAttendanceCommand, ScanAttendanceCommandHandler, ScanAttendanceValidator (+10 more)

### Community 53 - ".Create"
Cohesion: 0.09
Nodes (20): Tenant, AutoApproveGroupAdminChanges, AutoApproveJoinRequests, IsListedInSearch, Name, OwnerId, Status, Subject (+12 more)

### Community 54 - ".ResolveAsync"
Cohesion: 0.08
Nodes (6): TeacherERP.Application.Features.Users.Commands.Auth.SetPassword, TeacherERP.Application.Features.Users.Commands.Auth.ChangePassword, TeacherERP.Application.Features.Administration.CreateCenter, TeacherERP.Tests.Unit.Sessions, TeacherERP.Application.Features.Administration.CreateTeacher, TeacherERP.Application.Features.Administration.LinkTeacherToCenter

### Community 55 - "TeacherERP.Application.Features.Users.Commands.Auth.Login"
Cohesion: 0.10
Nodes (9): LoggingBehavior, ValidationBehavior, GroupScheduleSlot, Day, DurationMinutes, Group, GroupId, StartTime (+1 more)

### Community 56 - ".GetByOwnerIdAsync"
Cohesion: 0.21
Nodes (4): CancelSessionCommand, CancelSessionValidator, BulkCancelSessionsCommandHandlerTests, CancelSessionCommandHandlerTests

### Community 57 - "SeededTeacher"
Cohesion: 0.13
Nodes (10): CompensationStaffIntegrationTests, ManualAttendanceIntegrationTests, CustomWebApplicationFactory, FileStorage, PaymobGateway, PaymobSettings, GroupPricingIntegrationTests, GroupJoinCodeIntegrationTests (+2 more)

### Community 58 - "GroupAdminAttendanceIntegrationTests"
Cohesion: 0.09
Nodes (19): TeacherRequestRules, TeacherAccessRequest, CreatedUserId, DecisionNote, Email, ExpiresAtUtc, FullName, Note (+11 more)

### Community 59 - "TenantContext"
Cohesion: 0.12
Nodes (6): TeacherERP.Tests.Integration.Common, SessionJwtBearerEvents, HealthChecksIntegrationTests, PasswordIntegrationTests, RefreshTokenRotationIntegrationTests, SessionJwtRevocationIntegrationTests

### Community 60 - "ICenterRepository"
Cohesion: 0.09
Nodes (20): PaymobAdjustmentReview, CommissionReversalAmount, Currency, DecisionNote, ProviderAmount, ProviderEventId, ReviewedAtUtc, ReviewedByUserId (+12 more)

### Community 61 - "ApproveTeacherRequestCommand"
Cohesion: 0.12
Nodes (4): UpdateGroupScheduleRequest, SessionTimeCalculator, AddStudentProfile, AddAbsenceProcessing

### Community 62 - "CompleteProfileValidator"
Cohesion: 0.14
Nodes (17): CreatePaymobIntentBody, CreatePaymobIntentCommand, CreatePaymobIntentCommandValidator, CreatePaymobIntentCommandHandler, CreatePaymobPaymentRequest, IPaymobPaymentService, PaymobIntegrationKey, CaggOnlineNonShopify (+9 more)

### Community 63 - "SessionManager"
Cohesion: 0.09
Nodes (23): ASPNETCORE_ENVIRONMENT, applicationUrl, commandName, dotnetRunMessages, environmentVariables, applicationUrl, commandName, dotnetRunMessages (+15 more)

### Community 64 - "TeacherERP.Application.Features.Team"
Cohesion: 0.11
Nodes (11): TenantMember, CanManageFinance, ConsecutiveAbsences, CurrentBalance, HasFinancialDelegation, Role, Status, Tenant (+3 more)

### Community 65 - "TeacherERP.Application/DependencyInjection.cs"
Cohesion: 0.09
Nodes (15): CreateMyCenterValidator, CenterJoinRequest, CenterId, DecisionNote, LastNotifiedAtUtc, ReviewedAtUtc, ReviewedByUserId, Status (+7 more)

### Community 66 - "SetFinancialDelegationCommandHandler.cs"
Cohesion: 0.09
Nodes (21): LoginSession, AbsoluteExpiresAt, DeviceId, DeviceName, LastRefreshedAt, PreviousRefreshTokenHash, RefreshExpiresAt, RefreshTokenHash (+13 more)

### Community 68 - "CancelSessionCommandHandler.cs"
Cohesion: 0.15
Nodes (9): IFileStorage, StoredFile, IStoredFileCleanupJob, StoredFileCleanupJob, TeacherPaymentService, CloudinaryFileStorage, TestFileStorage, DeletedPublicIds (+1 more)

### Community 69 - "HangfireBasicAuthFilter"
Cohesion: 0.15
Nodes (5): PaymobGateway, PaymobGatewayTests, RecordingLogger, Messages, StubHandler

### Community 70 - "GetMySessionsQueryHandler"
Cohesion: 0.16
Nodes (5): IPaymobCallbackVerifier, Callback, PaymobErrors, PaymobPaymentService, PaymobCallbackVerifier

### Community 71 - "FinanceController"
Cohesion: 0.09
Nodes (17): ApplicationUser, Attendances, DateOfBirth, FullName, Gender, Guardian, IsProfileComplete, LoginSessions (+9 more)

### Community 72 - "LoggingBehavior"
Cohesion: 0.10
Nodes (18): AdministrationErrors, AttendanceErrors, CenterErrors, EnrollmentErrors, GroupAdminErrors, GroupErrors, ScheduleErrors, SessionContentErrors (+10 more)

### Community 73 - "GetAuditLogQuery"
Cohesion: 0.09
Nodes (22): NotificationType, BulkSessionsCancelled, BulkSessionsRescheduled, CompensationApproved, CompensationRejected, CompensationRequested, EnrollmentApproved, EnrollmentRejected (+14 more)

### Community 75 - "CachedRegistrationData"
Cohesion: 0.18
Nodes (14): GetMyPaymentOptionsQuery, GetMyPaymentOptionsQueryHandler, GetMyPaymentOptionsValidator, GetTeacherPaymentSettingsQuery, GetTeacherPaymentSettingsQueryHandler, SubmitInstaPayReceiptCommand, SubmitInstaPayReceiptCommandHandler, SubmitInstaPayReceiptValidator (+6 more)

### Community 76 - "GetMyNotificationsQuery"
Cohesion: 0.17
Nodes (8): IPlatformCommissionSettingsService, PlatformCommissionSettingsDto, PlatformCommissionSettingsService, PlatformCommissionSettings, MonthlySubscriptionAmount, PerSessionAmount, UpdatedAtUtc, UpdatedByUserId

### Community 77 - "UpdateGroupValidator"
Cohesion: 0.20
Nodes (11): AdministratorDashboardDto, TeacherDashboardDto, GetAdministratorDashboardQuery, GetAdministratorDashboardQueryHandler, GetTeacherDashboardQuery, GetTeacherDashboardQueryHandler, AdministratorDashboardOperationalStats, DashboardUpcomingSession (+3 more)

### Community 78 - ".Handle"
Cohesion: 0.11
Nodes (15): EnrollmentStatus, Active, Ended, GroupEnrollment, ApprovedByUserId, EndedAt, EnrolledAt, Group (+7 more)

### Community 79 - "SessionValidator"
Cohesion: 0.11
Nodes (4): TeacherERP.Application.Features.TeacherRequests.Verify, TeacherERP.Application.Features.TeacherRequests, TeacherRequestErrors, TeacherRequestPolicy

### Community 80 - "RescheduleSessionRequest"
Cohesion: 0.15
Nodes (8): CompleteProfileCommand, CompleteProfileValidator, GuardianDto, ProfileDto, Gender, Female, Male, EgyptianPhoneNumber

### Community 81 - "PageRequest"
Cohesion: 0.25
Nodes (7): ChangePasswordCommand, ChangePasswordValidator, SetPasswordCommand, SetPasswordCommandHandler, SetPasswordValidator, ICredentialService, PasswordCommandTests

### Community 82 - "Task"
Cohesion: 0.20
Nodes (12): GetStudentDetailsQuery, GetStudentDetailsQueryHandler, StudentActiveGroupDto, StudentDetailsDto, IStudentRepository, StudentActiveGroupItem, StudentDetailsItem, MembershipStatus (+4 more)

### Community 83 - "RescheduleSessionValidator.cs"
Cohesion: 0.19
Nodes (5): IPasswordGenerator, TeacherProvisioningService, WelcomeEmailFailedException, PasswordGenerator, TeacherProvisioningServiceTests

### Community 84 - "Error"
Cohesion: 0.17
Nodes (6): CoreConfigurationHealthCheck, DatabaseHealthCheck, CloudinaryConfigurationHealthCheck, GoogleAuthConfigurationHealthCheck, PaymobConfigurationHealthCheck, SmtpEndpointHealthCheck

### Community 85 - ".ApproveAsync"
Cohesion: 0.14
Nodes (4): TeacherERP.Application.Features.Users.Commands.Auth.GoogleSignIn, TeacherERP.Application.Features.Users.Commands.Auth.Login, TeacherERP.Application.Features.Users.Commands.Auth.ActivateAccount, TeacherERP.Application.Features.Users.Commands.Auth.RefreshToken

### Community 86 - "CreateAssistantValidator.cs"
Cohesion: 0.14
Nodes (3): TeacherERP.API.HealthChecks, HangfireWorkerHealthCheck, Program

### Community 87 - "TeacherAccessRequestStatus"
Cohesion: 0.12
Nodes (12): AttendanceRecord, CompensatesSessionId, RecordedByUserId, ScannedLatitude, ScannedLongitude, ScannedVia, Session, SessionId (+4 more)

### Community 88 - "AttendanceSettingsDto"
Cohesion: 0.12
Nodes (14): GuardianInfo, Name, PhoneNumber, Relation, GuardianRelation, Aunt, Brother, Father (+6 more)

### Community 89 - "BaseApiController"
Cohesion: 0.12
Nodes (14): PaymobSettings, ApiKey, BaseUrl, Currency, HmacNullValue, HmacSecret, InitializationLeaseSeconds, IntegrationIds (+6 more)

### Community 90 - "GoogleSignInValidator.cs"
Cohesion: 0.12
Nodes (15): 🔒 1. Advanced Security & Session Control, 📍 2. Hybrid Anti-Cheat Attendance System, 💰 3. Strict Financial Ledger & Privacy, Ahmed Al-Desouki, 🏛️ Architecture, 👨‍💻 Author, Center Management System, 🛡️ Core Features (+7 more)

### Community 91 - "ResetPasswordValidator.cs"
Cohesion: 0.17
Nodes (9): FileNames, DetectedFileType, Jpeg, Pdf, Png, Unknown, Webp, FileSignature (+1 more)

### Community 92 - "SessionAttachment"
Cohesion: 0.17
Nodes (13): IPaymobGateway, PaymobIntentCommandData, PaymobIntentData, PaymobWebhookOutcome, Accepted, Duplicate, Ignored, InvalidPayload (+5 more)

### Community 93 - "TeacherProvisioningServiceTests"
Cohesion: 0.14
Nodes (12): SessionAttachment, Caption, ContentType, FileName, Kind, PublicId, Session, SessionId (+4 more)

### Community 94 - ".ResolveAsync"
Cohesion: 0.12
Nodes (15): TransientAuthState, CodeExpiresAtUtc, Email, FailedAttempts, GrantExpiresAtUtc, GrantHash, Id, IsLocked (+7 more)

### Community 95 - "ProfileAuthorizationResultHandler.cs"
Cohesion: 0.14
Nodes (12): CenterInvitation, AcceptedAtUtc, AcceptedByTenantId, CenterId, CreatedByUserId, ExpiresAtUtc, Status, TokenHash (+4 more)

### Community 96 - "TeacherERP.Application.Features.Users.Commands.Auth.Login"
Cohesion: 0.14
Nodes (3): TeacherERP.Application, TeacherERP.Application.Services.Users, DependencyInjection

### Community 98 - "CreatedTeacherDto"
Cohesion: 0.30
Nodes (6): CreateTeacherCommand, CreateTeacherCommandHandler, CreateTeacherValidator, ITeacherProvisioningService, CreateTeacherCommandHandlerTests, CreateTeacherValidatorTests

### Community 99 - ".GetByIdAsync"
Cohesion: 0.18
Nodes (5): CompletedProfileHandler, CompletedProfileRequirement, ProfileCompletionChecker, ProfileRequirement, IProfileCompletionChecker

### Community 100 - "GroupScheduleSlot"
Cohesion: 0.33
Nodes (3): FrozenTimeProvider, StudentDraft, TransientAuthStateIntegrationTests

### Community 101 - ".AddInfrastructureServices"
Cohesion: 0.33
Nodes (7): AttendanceSettingsDto, GetAttendanceSettingsQuery, GetAttendanceSettingsQueryHandler, RegenerateStaticCodeCommand, UpdateAttendanceSettingsCommand, UpdateAttendanceSettingsCommandHandler, UpdateAttendanceSettingsValidator

### Community 102 - "MoveStudentCommandHandler.cs"
Cohesion: 0.20
Nodes (4): TeacherERP.Application.Features.SessionContent.UploadSessionAttachment, TeacherERP.Application.GroupAdmins, TeacherERP.Application.Features.GroupAdmins, GroupAdminPolicy

### Community 103 - "GroupScanValidator.cs"
Cohesion: 0.27
Nodes (8): TenantHandler, TenantRequirement, AllowedRoles, RequiresFinancialDelegation, TenantRole, Assistant, Owner, Student

### Community 105 - "IAttendanceRepository"
Cohesion: 0.36
Nodes (6): DirectoryCenterInfo, DirectoryRow, DirectorySearchPage, IDirectoryRepository, TeacherProfileData, DirectoryRepository

### Community 106 - "LogoutValidator.cs"
Cohesion: 0.31
Nodes (3): ISessionAttachmentRepository, SessionAttachmentItem, SessionAttachmentRepository

### Community 111 - ".Format"
Cohesion: 0.20
Nodes (7): IUserTimeZoneAccessor, TimeZoneId, CurrentUserService, SessionId, UserId, HttpUserTimeZoneAccessor, TimeZoneId

### Community 112 - "Gender"
Cohesion: 0.20
Nodes (10): UploadSessionAttachmentForm, Caption, File, Kind, SessionAttachmentKind, Board, Explanation, Homework (+2 more)

### Community 114 - "TenantDto"
Cohesion: 0.24
Nodes (8): IStudentTimelineRepository, StudentTimelineSessionItem, AttendanceStatus, Absent, PendingCompensation, Present, Rejected, StudentTimelineRepository

### Community 118 - "PlatformCommissionSettings"
Cohesion: 0.22
Nodes (8): UpdateTeacherPaymentSettingsForm, InstaPayLink, QrImage, SubmitInstaPayReceiptForm, Amount, Receipt, SessionId, Type

### Community 120 - "TeacherERP.Application.Features.Team"
Cohesion: 0.43
Nodes (5): GetPaymentOptionsQuery, GetPaymentOptionsQueryHandler, FinancialDebtItem, FinancialPaymentOptions, FinancialTransactionItem

### Community 121 - "ForwardedHeadersExtensions.cs"
Cohesion: 0.50
Nodes (5): GetCurrentTenantQuery, GetCurrentTenantQueryHandler, GetMyTenantsQuery, GetMyTenantsQueryHandler, TenantDto

### Community 122 - "GetNotificationWhatsAppShareQuery"
Cohesion: 0.32
Nodes (5): GoogleUserInfo, IGoogleAuthService, GoogleAuthService, GoogleAuthSettings, ClientIds

### Community 123 - "IReadOnlyList"
Cohesion: 0.50
Nodes (5): IStudentWeeklyScheduleRepository, StudentWeeklyScheduleData, StudentWeeklyScheduleGroupItem, StudentWeeklyScheduleSlotItem, StudentWeeklyScheduleRepository

### Community 125 - "GetSessionsQuery"
Cohesion: 0.29
Nodes (3): PaymobReturnController, PaymobWebhookController, RolesController

### Community 126 - ".TokenValidated"
Cohesion: 0.57
Nodes (5): GetTenantSettingsQuery, GetTenantSettingsQueryHandler, TenantSettingsDto, UpdateTenantSettingsCommand, UpdateTenantSettingsCommandHandler

### Community 127 - "PaymobWebhookOutcome"
Cohesion: 0.33
Nodes (3): TeacherERP.Application.Features.Team, TeamErrors, TeamPolicy

### Community 129 - "Exception"
Cohesion: 0.33
Nodes (3): TeacherERP.Application.Features.GroupAdmins.RejectGroupAdminRequest, RejectGroupAdminRequestRequest, RejectGroupAdminRequestValidator

### Community 131 - "PaymentType"
Cohesion: 0.67
Nodes (3): StudentWeeklyGroupScheduleDto, StudentWeeklyScheduleDto, StudentWeeklyScheduleSlotDto

### Community 132 - "CenterMembershipPolicy.cs"
Cohesion: 0.53
Nodes (4): ForgotPasswordCommand, ForgotPasswordCommandHandler, ForgotPasswordResponseDto, ForgotPasswordValidator

### Community 133 - "SchedulePolicy.cs"
Cohesion: 0.33
Nodes (6): AttendanceOutcome, AlreadyRecorded, Corrected, PendingApproval, PendingCompensation, Recorded

### Community 134 - "RoleNames.cs"
Cohesion: 0.33
Nodes (6): MailSettings, DisplayName, Host, Mail, Password, Port

### Community 139 - "CurrentTenantAccessor"
Cohesion: 0.40
Nodes (3): CurrentTenantAccessor, Current, TenantId

### Community 140 - "CloudinarySettings"
Cohesion: 0.40
Nodes (4): CloudinarySettings, ApiKey, ApiSecret, CloudName

### Community 141 - "JwtSettings"
Cohesion: 0.40
Nodes (5): JwtSettings, Audience, ExpiryMinutes, Issuer, Secret

### Community 145 - "GroupAccessLevel"
Cohesion: 0.67
Nodes (3): GroupAccessLevel, GroupAdmin, Staff

## Knowledge Gaps
- **628 isolated node(s):** `AllowedRoles`, `RequiresFinancialDelegation`, `Assistant`, `Owner`, `Student` (+623 more)
  These have ≤1 connection - possible missing edges or undocumented components. (Counts symbols only; 895 node(s) total have ≤1 connection when file, concept and rationale nodes are included.)
- **29 thin communities (<3 nodes) omitted from report** — run `graphify query` to explore isolated nodes.

## Suggested Questions
_Questions this graph is uniquely positioned to answer:_

- **Why does `Result` connect `system` to `IActionResult`, `TeacherERP.Application.Scheduling`, `PaymentType`, `CenterMembershipPolicy.cs`, `Result`, `TeacherERP.Application.Interfaces`, `CreateAssistantCommandHandler`, `Guid`, `Session`, `system_data`, `DateTime`, `IGroupSessionRepository`, `.MarkAsUpdated`, `.Handle`, `AttendanceController.cs`, `Fact`, `CustomWebApplicationFactory`, `IActionResult`, `.SaveChangesAsync`, `.AddApplicationServices`, `GetTeacherDashboardQueryHandler`, `Session`, `CancellationToken`, `AuthResponseDto`, `EnrollmentRequest`, `GroupSessionRepository`, `.GetByOwnerIdAsync`, `CompleteProfileValidator`, `TeacherERP.Application.Features.Team`, `CancelSessionCommandHandler.cs`, `HangfireBasicAuthFilter`, `LoggingBehavior`, `CachedRegistrationData`, `GetMyNotificationsQuery`, `UpdateGroupValidator`, `RescheduleSessionRequest`, `PageRequest`, `Task`, `RescheduleSessionValidator.cs`, `AttendanceSettingsDto`, `SessionAttachment`, `CreatedTeacherDto`, `.AddInfrastructureServices`, `.GetByIdAsync`, `TeacherERP.Application.Features.Team`, `ForwardedHeadersExtensions.cs`, `GetNotificationWhatsAppShareQuery`, `.TokenValidated`?**
  _High betweenness centrality (0.082) - this node is a cross-community bridge._
- **What connects `AllowedRoles`, `RequiresFinancialDelegation`, `Assistant` to the rest of the system?**
  _628 weakly-connected nodes found - possible documentation gaps or missing edges._
- **Should `TeacherERP.Domain.Shared` be split into smaller, more focused modules?**
  _Cohesion score 0.016396479851783233 - nodes in this community are weakly interconnected._
- **Why does `AppDbContext` connect `IFinancialLedgerRepository` to `TeacherERP.Application.Scheduling`, `ApplicationUser`, `Result`, `CreateAssistantCommandHandler`, `.EnsurePaymentEventsAreAppendOnly`, `Guid`, `system_data`, `.Handle`, `AppDbContextFactory`, `EnrollmentRequestStatus`, `LoginSession`, `MembershipStatus`, `PasswordController.cs`, `IActionResult`, `CreateAssistantCommandHandler`, `Session`, `.Upload`, `TeacherERP.Application.Features.Users.Commands.Auth.Login`, `SeededTeacher`, `RevokeSessionCommand`, `GetSessionsQuery`, `UpdateGroupValidator`, `Task`, `GroupScheduleSlot`, `IAttendanceRepository`, `LogoutValidator.cs`, `CurrentUserService`, `TenantDto`, `RequestGroupByCodeCommandHandler.cs`, `IReadOnlyList`?**
  _High betweenness centrality (0.059) - this node is a cross-community bridge._
- **Should `IActionResult` be split into smaller, more focused modules?**
  _Cohesion score 0.049520991828684136 - nodes in this community are weakly interconnected._
- **Why does `TeacherERP.Domain.Entities` connect `Task` to `TeacherERP.Domain.Shared`, `Exception`, `system`, `TeacherERP.Application.Interfaces`, `CreateAssistantCommandHandler`, `.SaveChangesAsync`, `Session`, `.Handle`, `IRequestHandler`, `Center`, `.Handle`, `CustomWebApplicationFactory`, `LoginSession`, `MembershipStatus`, `microsoft_aspnetcore_authorization`, `.SaveChangesAsync`, `.AddApplicationServices`, `🏫 Smart Teacher ERP`, `HttpGet`, `GroupSessionRepository`, `.ResolveAsync`, `ICenterRepository`, `GetMyNotificationsQuery`, `SessionValidator`, `RescheduleSessionRequest`, `Task`, `.ApproveAsync`, `SessionAttachment`, `.ResolveAsync`, `MoveStudentCommandHandler.cs`, `IAttendanceRepository`, `.VerifyAsync`, `TeacherERP.Application.Features.Team`, `IReadOnlyList`?**
  _High betweenness centrality (0.051) - this node is a cross-community bridge._
- **Should `Task` be split into smaller, more focused modules?**
  _Cohesion score 0.028277404921700222 - nodes in this community are weakly interconnected._