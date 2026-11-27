# Graph Report - TeacherERP  (2026-11-27)

## Corpus Check
- cluster-only mode — file stats not available

## Summary
- 4618 nodes · 17857 edges · 142 communities (113 shown, 29 thin omitted)
- Extraction: 90% EXTRACTED · 10% INFERRED · 0% AMBIGUOUS · INFERRED: 1847 edges (avg confidence: 0.85)
- Token cost: 0 input · 0 output

## Graph Freshness
- Built from commit: `c637c9a5`
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
- HangfireBasicAuthFilter
- IReadOnlyList
- .EnsurePaymentEventsAreAppendOnly
- SessionCleanupJob
- CurrentTenantAccessor
- CloudinarySettings
- JwtSettings

## God Nodes (most connected - your core abstractions)
1. `Result` - 460 edges
2. `TeacherERP.Application.Interfaces` - 321 edges
3. `TeacherERP.Domain.Shared` - 312 edges
4. `TeacherERP.Domain.Entities` - 257 edges
5. `AppDbContext` - 126 edges
6. `TeacherERP.Application.Common` - 120 edges
7. `PagedResult` - 106 edges
8. `ICurrentTenantAccessor` - 93 edges
9. `ApplicationUser` - 74 edges
10. `IUnitOfWork` - 72 edges

## Surprising Connections (you probably didn't know these)
- `🔒 1. Advanced Security & Session Control` --references--> `ApplicationUser`  [INFERRED]
  README.md → TeacherERP.Domain/Entities/Users/User.cs
- `🗄️ Database Schema` --references--> `ApplicationUser`  [INFERRED]
  README.md → TeacherERP.Domain/Entities/Users/User.cs
- `🗄️ Database Schema` --references--> `TenantSettings`  [INFERRED]
  README.md → TeacherERP.Domain/Entities/Tenants/TenantSettings.cs
- `PaymobErrors` --references--> `Error`  [EXTRACTED]
  TeacherERP.Application/Services/Finance/PaymobPaymentService.cs → TeacherERP.Domain/Shared/Error.cs
- `TeamErrors` --references--> `Error`  [EXTRACTED]
  TeacherERP.Application/Features/Team/TeamErrors.cs → TeacherERP.Domain/Shared/Error.cs

## Import Cycles
- None detected.

## Communities (142 total, 29 thin omitted)

### Community 0 - "TeacherERP.Domain.Shared"
Cohesion: 0.01
Nodes (73): TeacherERP.Application.Features.Attendance.RejectCompensation, TeacherERP.Application.Features.Finance.GetMyPaymentOptions, TeacherERP.Application.Features.Team.GetTeam, TeacherERP.Application.Features.GroupAdmins.GetMyGroupAdmin, TeacherERP.Application.Features.TeacherRequests.Review, TeacherERP.Application.Features.Finance.GetPaymentOptions, TeacherERP.Application.Features.Students.GetWeeklySchedule, TeacherERP.Application.Features.Centers.JoinRequests (+65 more)

### Community 1 - "IActionResult"
Cohesion: 0.03
Nodes (28): TeacherERP.Application.Enrollment, TeacherERP.Application.Features.Attendance, TeacherERP.Application.Features.GroupAdmins.GetGroupAdminRequests, TeacherERP.Application.Features.Students.GetStudentDetails, TeacherERP.Application.Attendance, TeacherERP.Application.Absence, TeacherERP.Application.Features.Attendance.ScanAttendance, TeacherERP.Application.Features.Enrollment.GetMyEnrollments (+20 more)

### Community 2 - "Task"
Cohesion: 0.04
Nodes (12): TeacherERP.API.Middlewares, TeacherERP.Infrastructure.Settings, TeacherERP.Infrastructure, TeacherERP.Tests.Integration.Sessions, TeacherERP.Application.Features.Groups.UpdateGroup, TeacherERP.Tests.Integration.Identity, TeacherERP.Domain.ValueObjects, TeacherERP.Application.Features.TeacherRequests (+4 more)

### Community 3 - "TeacherERP.Application.Scheduling"
Cohesion: 0.03
Nodes (20): TeacherERP.Application.Features.Users.Commands.Auth.SetPassword, TeacherERP.Application.Features.TeacherRequests.Submit, TeacherERP.Tests.Unit, TeacherERP.Application.Features.Finance.CreatePaymobIntent, TeacherERP.Application.Features.Enrollment.ApproveRequest, TeacherERP.Application.Features.Users.Commands.VerifyOtp, TeacherERP.Application.Features.Users.Commands.Auth.ChangePassword, TeacherERP.Application.Features.TeacherRequests.Verify (+12 more)

### Community 4 - "system"
Cohesion: 0.04
Nodes (24): CenterMembershipPolicy, TeacherRequestPolicy, SessionPolicy, InitialCreate, FixLoginSessionUserId, AddTenants, MoveCenterStateToMembership, AddCentersAndTeacherSubject (+16 more)

### Community 5 - "ApplicationUser"
Cohesion: 0.05
Nodes (48): AdminCenterDto, CenterTeacherLinkDto, CreateCenterCommand, CreateCenterCommandHandler, LinkTeacherToCenterCommand, LinkTeacherToCenterCommandHandler, LinkTeacherToCenterValidator, CenterDto (+40 more)

### Community 6 - "Result"
Cohesion: 0.05
Nodes (64): AttendanceSettingsDto, GetAttendanceSettingsQuery, GetAttendanceSettingsQueryHandler, RegenerateStaticCodeCommand, RegenerateStaticCodeCommandHandler, UpdateAttendanceSettingsCommand, UpdateAttendanceSettingsCommandHandler, UpdateAttendanceSettingsValidator (+56 more)

### Community 8 - "CreateAssistantCommandHandler"
Cohesion: 0.06
Nodes (27): RequestPlatformCommissionCollection, SettlePlatformCommissionRequest, MoveStudentRequest, LinkTeacherToCenterRequest, ApproveCompensationCommand, RejectCompensationCommand, MyEnrollmentDto, RejectGroupAdminRequestCommand (+19 more)

### Community 9 - ".SaveChangesAsync"
Cohesion: 0.05
Nodes (19): UserTime, SubmitTeacherRequestValidator, AssistantDto, CreateAssistantCommand, CreateAssistantCommandHandler, WelcomeEmailFailedException, CreateAssistantValidator, IEmailService (+11 more)

### Community 10 - "Guid"
Cohesion: 0.05
Nodes (41): CreatePaymobIntentBody, CreatePaymobIntentCommand, CreatePaymobIntentCommandValidator, CreatePaymobIntentCommandHandler, CreatePaymobPaymentRequest, IPaymobPaymentService, PaymobIntegrationKey, CaggOnlineNonShopify (+33 more)

### Community 11 - "Session"
Cohesion: 0.04
Nodes (28): IAbsenceRepository, Session, AbsencesProcessedAt, Attendances, BoardImageUrl, CancelReason, EndTime, Group (+20 more)

### Community 12 - "system_data"
Cohesion: 0.10
Nodes (9): SchoolGrade, CenterMembershipIntegrationTests, DashboardIntegrationTests, GroupPricingIntegrationTests, GroupGradeIntegrationTests, ScheduleOperationsIntegrationTests, SessionMoveIntegrationTests, StudentTransferIntegrationTests (+1 more)

### Community 13 - ".Handle"
Cohesion: 0.04
Nodes (32): GroupJoinCodeLookup, Group, Capacity, CenterId, DynamicJoinEnabled, DynamicJoinSecret, EducationStage, GradeLevel (+24 more)

### Community 14 - "DateTime"
Cohesion: 0.13
Nodes (61): AdminScanCommandHandler, ApproveCompensationCommandHandler, ManualAttendanceCommandHandler, RejectCompensationCommandHandler, ScanAttendanceCommandHandler, RejectCenterJoinRequestCommandHandler, ApproveRequestCommandHandler, CancelRequestCommandHandler (+53 more)

### Community 15 - "EnrollmentRequestStatus"
Cohesion: 0.03
Nodes (5): DependencyInjection, CloudinarySettings, ApiKey, ApiSecret, CloudName

### Community 16 - "IFinancialLedgerRepository"
Cohesion: 0.05
Nodes (33): GroupJoinInfo, EnrollmentRequest, DecidedAt, DecidedByUserId, DecisionNote, Group, GroupId, SourceLatitude (+25 more)

### Community 17 - "PagedResult"
Cohesion: 0.08
Nodes (10): AttendanceController, GroupAdminsController, GroupsController, UpdateGroupJoinCodesRequest, ScheduleController, SessionAttachmentsController, TeamController, RejectGroupAdminRequestRequest (+2 more)

### Community 18 - "IGroupSessionRepository"
Cohesion: 0.05
Nodes (27): ICredentialService, IExternalLoginService, ApplicationUser, Attendances, DateOfBirth, FullName, Gender, Guardian (+19 more)

### Community 19 - ".MarkAsUpdated"
Cohesion: 0.09
Nodes (12): ApproveInstaPayReceiptCommand, CollectCashPaymentCommand, CollectCashPaymentCommandHandler, CashPaymentRequest, CashPaymentResult, IFinancialLedgerService, PaymobAdjustmentResolution, ResolvePaymobAdjustmentRequest (+4 more)

### Community 20 - "AttendanceRecord"
Cohesion: 0.07
Nodes (21): AdminScanCommand, ManualAttendanceCommand, ScanAttendanceCommand, ApproveRequestCommand, GroupScanCommand, AttendanceOutcome, AlreadyRecorded, Corrected (+13 more)

### Community 22 - "Center"
Cohesion: 0.05
Nodes (30): AppDbContext, AttendanceRecords, AuditLogs, CenterInvitations, CenterJoinRequests, Centers, CenterTeachers, CurrentTenantId (+22 more)

### Community 23 - "Center"
Cohesion: 0.07
Nodes (12): AccessTokenResult, IJwtProvider, ISessionCleanupJob, ISessionRepository, SessionCleanupJob, SessionManager, TokenHasher, PasswordChangedNotifier (+4 more)

### Community 24 - ".Handle"
Cohesion: 0.06
Nodes (37): CompleteProfileCommand, CompleteProfileCommandHandler, CompleteProfileValidator, GetMyProfileQuery, GetMyProfileQueryHandler, GuardianDto, ProfileDto, GetStudentDetailsQuery (+29 more)

### Community 25 - "IFinancialLedgerRepository.cs"
Cohesion: 0.11
Nodes (7): TeacherERP.Application.SessionContent, TeacherERP.API.Security, TeacherERP.Application.Features.Attendance.ManualAttendance, TeacherERP.API.Controllers, TeacherERP.API.Extensions, RateLimitPolicies, AuthPolicies

### Community 26 - "AttendanceController.cs"
Cohesion: 0.10
Nodes (9): AdministrationController, ResolvePaymobAdjustmentBody, AuthController, PasswordController, CentersController, TeacherRequestsController, UsersController, UpdateCenterRequest (+1 more)

### Community 27 - "Fact"
Cohesion: 0.06
Nodes (33): INotificationRepository, NotificationShareItem, NotificationService, Notification, IsRead, Message, RecipientUserId, RelatedId (+25 more)

### Community 28 - "CustomWebApplicationFactory"
Cohesion: 0.09
Nodes (25): GetGroupAdminsQuery, GetGroupAdminsQueryHandler, GroupAdminDto, CreateGroupCommand, GetGroupQuery, GetGroupQueryHandler, GetGroupsQuery, GroupDto (+17 more)

### Community 29 - "LoginSession"
Cohesion: 0.05
Nodes (33): TeacherERP.Tests.Unit.Common, MonthlySubscription, Amount, GroupId, Month, PaymentTransactionId, StudentId, TenantId (+25 more)

### Community 30 - "MembershipStatus"
Cohesion: 0.05
Nodes (44): CloudinaryDotNet (1.29.3), coverlet.collector (6.0.4), FluentValidation (12.1.1), FluentValidation.DependencyInjectionExtensions (12.1.1), Google.Apis.Auth (1.77.0), Hangfire.AspNetCore (1.8.25), Hangfire.Core (1.8.25), Hangfire.Dashboard.BasicAuthorization (1.0.2) (+36 more)

### Community 31 - "microsoft_aspnetcore_authorization"
Cohesion: 0.06
Nodes (23): CreateMyCenterValidator, CenterInvitation, AcceptedAtUtc, AcceptedByTenantId, CenterId, CreatedByUserId, ExpiresAtUtc, Status (+15 more)

### Community 32 - "PasswordController.cs"
Cohesion: 0.07
Nodes (27): GetSessionAttachmentsQuery, GetSessionAttachmentsQueryHandler, SessionAttachmentDto, UploadSessionAttachmentCommand, UploadSessionAttachmentValidator, ISessionAttachmentRepository, SessionAttachmentItem, SessionContentPolicy (+19 more)

### Community 33 - "IActionResult"
Cohesion: 0.09
Nodes (23): CenterInvitationToken, CenterInvitationDto, CenterJoinRequestListItem, CreateCenterInvitationCommand, CreateCenterInvitationCommandHandler, CancelCenterJoinRequestCommand, CancelCenterJoinRequestCommandHandler, GetCenterJoinRequestsQuery (+15 more)

### Community 34 - ".Handle"
Cohesion: 0.10
Nodes (10): DisplayCodeDto, GetDisplayCodeQuery, GetDisplayCodeQueryHandler, IAttendanceCodeService, AttendanceCodeService, AttendanceMode, AdminOnly, DynamicScreenQR (+2 more)

### Community 35 - ".SaveChangesAsync"
Cohesion: 0.05
Nodes (38): CreateCenterValidator, AdminScanValidator, ManualAttendanceValidator, ScanAttendanceValidator, AcceptCenterInvitationValidator, CreateCenterInvitationValidator, ApproveCenterJoinRequestValidator, CancelCenterJoinRequestValidator (+30 more)

### Community 36 - "CreateAssistantCommandHandler"
Cohesion: 0.12
Nodes (6): IAbsenceProcessingJob, ICenterMembershipCleanupJob, ISessionGenerationJob, AbsenceProcessingJob, SessionGenerationJob, TeacherRequestsIntegrationTests

### Community 37 - "GroupDto"
Cohesion: 0.09
Nodes (12): PaymobAdjustmentReviewsController, AuditController, BaseApiController, DashboardController, DirectoryController, StudentPaymentsController, SessionsController, StudentsController (+4 more)

### Community 38 - ".AddApplicationServices"
Cohesion: 0.08
Nodes (13): TeacherRequestToken, VerifyTeacherRequestCommandHandler, VerifyTeacherRequestValidator, PendingStudentRegistrationData, Email, FullName, PasswordHash, VerifyOtpCommandHandler (+5 more)

### Community 39 - "IOtpService"
Cohesion: 0.11
Nodes (6): SessionJwtBearerEvents, AdministrationIntegrationTests, DynamicQrIntegrationTests, SeededTeacher, PasswordIntegrationTests, RefreshTokenRotationTests

### Community 40 - "TeacherRequestReviewServiceTests"
Cohesion: 0.09
Nodes (15): GetPaymentOptionsQuery, GetPaymentOptionsQueryHandler, FinancialDebtItem, FinancialPaymentOptions, FinancialTransactionItem, TeacherPaymentService, TeacherPaymentSettings, InstaPayLink (+7 more)

### Community 41 - "GetTeacherDashboardQueryHandler"
Cohesion: 0.08
Nodes (17): IFileStorage, StoredFile, IStoredFileCleanupJob, StoredFileCleanupJob, FileNames, DetectedFileType, Jpeg, Pdf (+9 more)

### Community 42 - "AbstractValidator"
Cohesion: 0.12
Nodes (5): TestData, FinanceAuthorizationIntegrationTests, StudentExperienceIntegrationTests, Scenario, StudentLearningTimelineIntegrationTests

### Community 43 - "Session"
Cohesion: 0.13
Nodes (13): GroupAdminChangeResultDto, RequestGroupAdminChangeCommand, RequestGroupAdminChangeValidator, AddExtraSessionCommand, BulkCancelSessionsCommand, BulkShiftSessionsCommand, MoveSessionCommand, RescheduleSessionCommand (+5 more)

### Community 44 - "fluentvalidation"
Cohesion: 0.17
Nodes (5): VerifyResetOtpCommandHandler, CreateStudentCommandHandler, CreateStudentValidator, IUserAccountService, UserAccountService

### Community 45 - "CancellationToken"
Cohesion: 0.07
Nodes (9): MoveSessionRequest, RescheduleSessionRequest, ReplaceRefreshTokensWithLoginSessions, AddNotificationANDAttendanceSystem, AddCashFinancialLedger, AddPaymobPaymentWorkflow, AddPlatformCommissions, ConfigurePlatformCommissionSettings (+1 more)

### Community 46 - "🏫 Smart Teacher ERP"
Cohesion: 0.12
Nodes (9): EnrollmentRequestDto, RequestGroupCommand, RequestGroupValidator, RequestGroupByCodeCommand, RequestGroupByCodeCommandHandler, RequestGroupByCodeValidator, IGroupJoinCodeService, GroupJoinCodeService (+1 more)

### Community 47 - "HttpGet"
Cohesion: 0.06
Nodes (33): AuditAction, AssistantRemoved, AssistantSuspended, AssistantUnsuspended, AttendanceCorrected, CashPaymentCollected, CompensationApproved, CompensationRejected (+25 more)

### Community 48 - "AuthResponseDto"
Cohesion: 0.15
Nodes (6): ChangePasswordCommandHandler, ResetPasswordCommandHandler, SetPasswordCommandHandler, IPasswordChangedNotifier, PasswordCommandTests, ResetPasswordAlertTests

### Community 49 - ".Upload"
Cohesion: 0.16
Nodes (5): IAbsenceProcessor, TenantContext, AttendanceFlowIntegrationTests, ScanScenario, SessionGenerationIntegrationTests

### Community 50 - "EnrollmentRequest"
Cohesion: 0.08
Nodes (18): GroupAdminItem, IGroupAdminRepository, GroupAdminRequest, DecidedAt, DecidedByUserId, DecisionNote, Group, GroupId (+10 more)

### Community 51 - "BaseEntity"
Cohesion: 0.12
Nodes (20): ActivateAccountCommand, ActivateAccountCommandHandler, ActivateAccountValidator, GoogleSignInCommand, GoogleSignInCommandHandler, GoogleSignInValidator, AuthResponseDto, LoginCommand (+12 more)

### Community 52 - "GroupSessionRepository"
Cohesion: 0.07
Nodes (22): Transaction, Amount, IdempotencyKey, PaymentContextSessionId, ProcessedByAdminId, Provider, ProviderClientSecret, ProviderFailureCode (+14 more)

### Community 53 - ".Create"
Cohesion: 0.11
Nodes (4): EnrollmentController, FinanceController, NotificationsController, UserRepository

### Community 54 - ".ResolveAsync"
Cohesion: 0.16
Nodes (5): CancelSessionCommand, CancelSessionValidator, FixedTimeProvider, BulkCancelSessionsCommandHandlerTests, CancelSessionCommandHandlerTests

### Community 55 - "TeacherERP.Application.Features.Users.Commands.Auth.Login"
Cohesion: 0.08
Nodes (19): PlatformCommission, Amount, CollectionRequestedAtUtc, CollectionRequestedByUserId, GroupId, IsSettled, SessionId, SettledAtUtc (+11 more)

### Community 56 - ".GetByOwnerIdAsync"
Cohesion: 0.13
Nodes (16): ResendWelcomeCommand, ResendWelcomeCommandHandler, WelcomeEmailFailedException, GetCurrentTenantQuery, GetCurrentTenantQueryHandler, GetMyTenantsQuery, GetMyTenantsQueryHandler, GetTenantSettingsQuery (+8 more)

### Community 57 - "SeededTeacher"
Cohesion: 0.12
Nodes (12): FinancialObligation, Amount, GroupId, GroupName, IsSettled, OutstandingAmount, PaidAmount, SessionId (+4 more)

### Community 59 - "TenantContext"
Cohesion: 0.11
Nodes (18): PendingTeacherRequestData, Email, FullName, Note, Subject, TimeZoneId, SubmitTeacherRequestCommandHandler, ForgotPasswordCommand (+10 more)

### Community 60 - "ICenterRepository"
Cohesion: 0.08
Nodes (15): UpdateTeacherPaymentSettingsForm, InstaPayLink, QrImage, PaymobReturnController, PaymobWebhookController, SubmitInstaPayReceiptForm, Amount, Receipt (+7 more)

### Community 61 - "ApproveTeacherRequestCommand"
Cohesion: 0.08
Nodes (20): PaymobAdjustmentReview, CommissionReversalAmount, Currency, DecisionNote, ProviderAmount, ProviderEventId, ReviewedAtUtc, ReviewedByUserId (+12 more)

### Community 62 - "CompleteProfileValidator"
Cohesion: 0.10
Nodes (12): TeacherERP.Tests.Integration.Common, Program, CompensationStaffIntegrationTests, ManualAttendanceIntegrationTests, CustomWebApplicationFactory, FileStorage, PaymobGateway, PaymobSettings (+4 more)

### Community 63 - "SessionManager"
Cohesion: 0.12
Nodes (8): EducationStage, Preparatory, Primary, Secondary, Unspecified, GroupAdminAttendanceIntegrationTests, Scenario, SchoolGradeTests

### Community 64 - "TeacherERP.Application.Features.Team"
Cohesion: 0.11
Nodes (16): TeacherRequestRules, ITeacherAccessRequestRepository, TeacherAccessRequest, CreatedUserId, DecisionNote, Email, ExpiresAtUtc, FullName (+8 more)

### Community 65 - "TeacherERP.Application/DependencyInjection.cs"
Cohesion: 0.18
Nodes (10): RemoveAssistantCommandHandler, SetAssistantSuspensionCommandHandler, AuditLog, Action, ActorUserId, Reason, TargetUserId, TenantId (+2 more)

### Community 66 - "SetFinancialDelegationCommandHandler.cs"
Cohesion: 0.14
Nodes (6): CoreConfigurationHealthCheck, HangfireWorkerHealthCheck, CloudinaryConfigurationHealthCheck, GoogleAuthConfigurationHealthCheck, PaymobConfigurationHealthCheck, SmtpEndpointHealthCheck

### Community 67 - "RevokeSessionCommand"
Cohesion: 0.08
Nodes (21): TeacherERP.Application.Features.Notifications, AdministrationErrors, AttendanceErrors, CenterErrors, EnrollmentErrors, GroupAdminErrors, GroupErrors, NotificationErrors (+13 more)

### Community 68 - "CancelSessionCommandHandler.cs"
Cohesion: 0.17
Nodes (14): PagedResult, HasNext, HasPrevious, TotalPages, GetPendingCompensationsQuery, GetPendingCompensationsQueryHandler, GetPendingInstaPayReceiptsQuery, GetPendingInstaPayReceiptsValidator (+6 more)

### Community 69 - "HangfireBasicAuthFilter"
Cohesion: 0.16
Nodes (16): TeacherERP.Application.Features.Directory, DirectoryCenterDto, DirectoryCenterInfoDto, DirectoryGroupDto, DirectoryTeacherDto, TeacherProfileDto, GetTeacherProfileQuery, GetTeacherProfileQueryHandler (+8 more)

### Community 70 - "GetMySessionsQueryHandler"
Cohesion: 0.08
Nodes (21): LoginSession, AbsoluteExpiresAt, DeviceId, DeviceName, LastRefreshedAt, PreviousRefreshTokenHash, RefreshExpiresAt, RefreshTokenHash (+13 more)

### Community 71 - "FinanceController"
Cohesion: 0.09
Nodes (7): TeacherERP.Application.Features.SessionContent.UploadSessionAttachment, TeacherERP.Application.Features.Schedule, TeacherERP.Application.Features.SessionContent.DeleteSessionAttachment, TeacherERP.Application.GroupAdmins, TeacherERP.Application.Features.Groups.CreateGroup, TeacherERP.Application.Features.GroupAdmins, GroupAdminPolicy

### Community 72 - "LoggingBehavior"
Cohesion: 0.09
Nodes (23): ASPNETCORE_ENVIRONMENT, applicationUrl, commandName, dotnetRunMessages, environmentVariables, applicationUrl, commandName, dotnetRunMessages (+15 more)

### Community 73 - "GetAuditLogQuery"
Cohesion: 0.16
Nodes (16): CreatedTeacherDto, ApproveTeacherRequestByAdminCommand, ApproveTeacherRequestByAdminCommandHandler, RejectTeacherRequestByAdminCommand, RejectTeacherRequestByAdminCommandHandler, ApproveTeacherRequestCommand, ApproveTeacherRequestCommandHandler, ApproveTeacherRequestValidator (+8 more)

### Community 74 - "GetSessionsQuery"
Cohesion: 0.16
Nodes (8): IPlatformCommissionSettingsService, PlatformCommissionSettingsDto, PlatformCommissionSettingsService, PlatformCommissionSettings, MonthlySubscriptionAmount, PerSessionAmount, UpdatedAtUtc, UpdatedByUserId

### Community 75 - "CachedRegistrationData"
Cohesion: 0.18
Nodes (6): PageRequest, Skip, PaymobAdjustmentReviewItem, PlatformCommissionItem, IPaymobAdjustmentReviewService, QueryablePagingExtensions

### Community 76 - "GetMyNotificationsQuery"
Cohesion: 0.14
Nodes (6): AddCenterMembershipRequests, SnapshotAccessor, AddStructuredGroupGrades, SnapshotAccessor, SnapshotAccessor, AppDbContextModelSnapshot

### Community 79 - "SessionValidator"
Cohesion: 0.10
Nodes (13): AttendanceRecord, CompensatesSessionId, RecordedByUserId, ScannedLatitude, ScannedLongitude, ScannedVia, Session, SessionId (+5 more)

### Community 80 - "RescheduleSessionRequest"
Cohesion: 0.11
Nodes (4): TeacherERP.Application.Centers, TeacherERP.Application.Features.Centers.CreateCenter, TeacherERP.Application.Features.Centers, TeacherERP.Application.Features.Centers.Invitations

### Community 81 - "PageRequest"
Cohesion: 0.16
Nodes (13): EnrollmentRequestListDto, GetRequestsQuery, GetRequestsQueryHandler, EnrollmentRequestListItem, MyRequestItem, EnrollmentRequestStatus, Approved, Cancelled (+5 more)

### Community 83 - "RescheduleSessionValidator.cs"
Cohesion: 0.18
Nodes (4): ISessionGenerator, SessionGenerator, SessionTimeCalculator, AddStudentProfile

### Community 84 - "Error"
Cohesion: 0.11
Nodes (19): PagedRequestRules, GetPendingCompensationsValidator, GetSessionAttendanceValidator, GetAuditLogValidator, GetCenterJoinRequestsValidator, GetMyCenterJoinRequestsValidator, GetTeacherProfileValidator, SearchDirectoryValidator (+11 more)

### Community 85 - ".ApproveAsync"
Cohesion: 0.22
Nodes (11): GetMyPaymentOptionsQuery, GetMyPaymentOptionsQueryHandler, GetTeacherPaymentSettingsQuery, GetTeacherPaymentSettingsQueryHandler, RejectInstaPayReceiptCommand, RejectInstaPayReceiptCommandHandler, UpdateTeacherPaymentSettingsCommand, UpdateTeacherPaymentSettingsCommandHandler (+3 more)

### Community 87 - "TeacherAccessRequestStatus"
Cohesion: 0.13
Nodes (5): TeacherERP.Application.Features.Schedule.BulkCancelSessions, TeacherERP.Application.Scheduling, TeacherERP.Application.Features.Schedule.GetSessions, TeacherERP.Application.Features.Schedule.BulkShiftSessions, SchedulePolicy

### Community 88 - "AttendanceSettingsDto"
Cohesion: 0.21
Nodes (5): IPasswordGenerator, TeacherProvisioningService, WelcomeEmailFailedException, PasswordGenerator, TeacherProvisioningServiceTests

### Community 89 - "BaseApiController"
Cohesion: 0.15
Nodes (4): TeacherERP.Application.Features.Users.Commands.Auth.GoogleSignIn, TeacherERP.Application.Features.Users.Commands.Auth.Login, TeacherERP.Application.Features.Users.Commands.Auth.ActivateAccount, TeacherERP.Application.Features.Users.Commands.Auth.RefreshToken

### Community 90 - "GoogleSignInValidator.cs"
Cohesion: 0.12
Nodes (15): 🔒 1. Advanced Security & Session Control, 📍 2. Hybrid Anti-Cheat Attendance System, 💰 3. Strict Financial Ledger & Privacy, Ahmed Al-Desouki, 🏛️ Architecture, 👨‍💻 Author, Center Management System, 🛡️ Core Features (+7 more)

### Community 91 - "ResetPasswordValidator.cs"
Cohesion: 0.25
Nodes (8): TeacherDashboardDto, GetTeacherDashboardQuery, GetTeacherDashboardQueryHandler, AdministratorDashboardOperationalStats, DashboardUpcomingSession, IDashboardRepository, TeacherDashboardOperationalStats, DashboardRepository

### Community 92 - "SessionAttachment"
Cohesion: 0.22
Nodes (12): GetStudentLearningTimelineQuery, GetStudentLearningTimelineQueryHandler, StudentTimelineAttachmentDto, StudentTimelineSessionDto, IStudentTimelineRepository, StudentTimelineSessionItem, AttendanceStatus, Absent (+4 more)

### Community 93 - "TeacherProvisioningServiceTests"
Cohesion: 0.12
Nodes (15): TransientAuthState, CodeExpiresAtUtc, Email, FailedAttempts, GrantExpiresAtUtc, GrantHash, Id, IsLocked (+7 more)

### Community 94 - ".ResolveAsync"
Cohesion: 0.13
Nodes (14): PaymobSettings, ApiKey, BaseUrl, Currency, HmacNullValue, HmacSecret, InitializationLeaseSeconds, IntegrationIds (+6 more)

### Community 96 - "TeacherERP.Application.Features.Users.Commands.Auth.Login"
Cohesion: 0.27
Nodes (8): IPagedRequest, Page, PageSize, GetSessionAttendanceQuery, GetSessionAttendanceQueryHandler, GetGroupSessionAttendanceQuery, GetGroupSessionAttendanceQueryHandler, AttendanceListItem

### Community 97 - "CenterJoinRequestListItem"
Cohesion: 0.30
Nodes (6): CreateTeacherCommand, CreateTeacherCommandHandler, CreateTeacherValidator, ITeacherProvisioningService, CreateTeacherCommandHandlerTests, CreateTeacherValidatorTests

### Community 98 - "CreatedTeacherDto"
Cohesion: 0.24
Nodes (8): GetMyGroupAdminQuery, GetMyGroupAdminQueryHandler, GroupAdminMeDto, GroupAccessLevel, GroupAdmin, Staff, IGroupAccessService, GroupAccessService

### Community 99 - ".GetByIdAsync"
Cohesion: 0.26
Nodes (10): GetStudentsQuery, GetStudentsQueryHandler, StudentListDto, IStudentRepository, StudentListItem, MembershipStatus, Active, Blacklisted (+2 more)

### Community 100 - "GroupScheduleSlot"
Cohesion: 0.22
Nodes (8): TenantHandler, TenantRequirement, AllowedRoles, RequiresFinancialDelegation, TenantRole, Assistant, Owner, Student

### Community 101 - ".AddInfrastructureServices"
Cohesion: 0.22
Nodes (4): PaymobGatewayTests, RecordingLogger, Messages, StubHandler

### Community 103 - "GroupScanValidator.cs"
Cohesion: 0.18
Nodes (4): PlatformCommissionsController, UpdatePlatformCommissionSettingsRequest, TeacherPlatformCommissionsController, IPlatformCommissionService

### Community 104 - ".GetByIdAsync"
Cohesion: 0.22
Nodes (3): IResetGrantService, ResetGrant, ResetGrantService

### Community 105 - "IAttendanceRepository"
Cohesion: 0.21
Nodes (5): CompletedProfileHandler, CompletedProfileRequirement, ProfileCompletionChecker, ProfileRequirement, IProfileCompletionChecker

### Community 106 - "LogoutValidator.cs"
Cohesion: 0.30
Nodes (8): GetGroupAdminRequestsQuery, GetGroupAdminRequestsQueryHandler, GroupAdminRequestDto, GroupAdminRequestItem, GroupAdminRequestStatus, Approved, Pending, Rejected

### Community 108 - "IPagedRequest"
Cohesion: 0.33
Nodes (8): ListTeacherRequestsQuery, ListTeacherRequestsQueryHandler, TeacherAccessRequestListItem, TeacherAccessRequestStatus, Approved, Expired, Pending, Rejected

### Community 114 - "TenantDto"
Cohesion: 0.20
Nodes (7): IUserTimeZoneAccessor, TimeZoneId, CurrentUserService, SessionId, UserId, HttpUserTimeZoneAccessor, TimeZoneId

### Community 116 - "RequestGroupByCodeCommandHandler.cs"
Cohesion: 0.33
Nodes (7): IPaymobGateway, PaymobIntentCommandData, PaymobIntentData, TestPaymobGateway, CreateCount, FailNextWithCode, LastRequest

### Community 120 - "TeacherERP.Application.Features.Team"
Cohesion: 0.39
Nodes (3): MonthlyCollectionReportsController, TeacherMonthlyCollectionReportsController, IMonthlyCollectionReportService

### Community 121 - "ForwardedHeadersExtensions.cs"
Cohesion: 0.43
Nodes (4): AuditLogDto, GetAuditLogQuery, GetAuditLogQueryHandler, AuditLogItem

### Community 122 - "GetNotificationWhatsAppShareQuery"
Cohesion: 0.25
Nodes (5): CreateGroupValidator, GroupPricingRules, GroupSlotsRules, UpdateGroupValidator, UpdateGroupScheduleValidator

### Community 123 - "IReadOnlyList"
Cohesion: 0.38
Nodes (4): TeacherERP.Application.Features.Users.Commands.Auth.VerifyResetOtp, ResetTokenDto, VerifyResetOtpCommand, VerifyResetOtpValidator

### Community 124 - "CloudinarySettings"
Cohesion: 0.33
Nodes (4): TeacherERP.Application.Features.Enrollment.RejectRequest, RejectRequestCommand, RejectRequestRequest, RejectRequestValidator

### Community 125 - "GetSessionsQuery"
Cohesion: 0.29
Nodes (3): GlobalExceptionHandler, ProvisioningRejectedException, Error

### Community 127 - "PaymobWebhookOutcome"
Cohesion: 0.48
Nodes (4): GetSessionsQuery, GetSessionsQueryHandler, SessionDto, SessionListItem

### Community 130 - "SubmitInstaPayReceiptForm"
Cohesion: 0.33
Nodes (3): TeacherERP.Application.Features.Team, TeamErrors, TeamPolicy

### Community 131 - "PaymentType"
Cohesion: 0.33
Nodes (6): PaymobWebhookOutcome, Accepted, Duplicate, Ignored, InvalidPayload, InvalidSignature

### Community 132 - "CenterMembershipPolicy.cs"
Cohesion: 0.50
Nodes (3): TeacherERP.Application.Features.Students.BlacklistStudent, BlacklistStudentCommand, BlacklistStudentValidator

### Community 136 - "IReadOnlyList"
Cohesion: 0.40
Nodes (3): CurrentTenantAccessor, Current, TenantId

### Community 137 - ".EnsurePaymentEventsAreAppendOnly"
Cohesion: 0.40
Nodes (5): JwtSettings, Audience, ExpiryMinutes, Issuer, Secret

## Knowledge Gaps
- **631 isolated node(s):** `CaggOnlineNonShopify`, `CashCashChannel`, `UigOnlineNewNonShopify`, `VpcOnlineNonShopify`, `VpcShopify` (+626 more)
  These have ≤1 connection - possible missing edges or undocumented components. (Counts symbols only; 893 node(s) total have ≤1 connection when file, concept and rationale nodes are included.)
- **29 thin communities (<3 nodes) omitted from report** — run `graphify query` to explore isolated nodes.

## Suggested Questions
_Questions this graph is uniquely positioned to answer:_

- **Why does `Result` connect `Result` to `Task`, `CenterMembershipPolicy.cs`, `ApplicationUser`, `CreateAssistantCommandHandler`, `.SaveChangesAsync`, `Guid`, `DateTime`, `IFinancialLedgerRepository`, `IGroupSessionRepository`, `.MarkAsUpdated`, `AttendanceRecord`, `.Handle`, `AttendanceController.cs`, `CustomWebApplicationFactory`, `PasswordController.cs`, `IActionResult`, `.Handle`, `TeacherRequestReviewServiceTests`, `GetTeacherDashboardQueryHandler`, `AbstractValidator`, `Session`, `🏫 Smart Teacher ERP`, `AuthResponseDto`, `BaseEntity`, `.ResolveAsync`, `.GetByOwnerIdAsync`, `SeededTeacher`, `TenantContext`, `RevokeSessionCommand`, `CancelSessionCommandHandler.cs`, `HangfireBasicAuthFilter`, `GetAuditLogQuery`, `GetSessionsQuery`, `PageRequest`, `.ApproveAsync`, `AttendanceSettingsDto`, `ResetPasswordValidator.cs`, `SessionAttachment`, `TeacherERP.Application.Features.Users.Commands.Auth.Login`, `CenterJoinRequestListItem`, `CreatedTeacherDto`, `.GetByIdAsync`, `LogoutValidator.cs`, `IPagedRequest`, `Gender`, `RequestGroupByCodeCommandHandler.cs`, `TeacherERP.Application.Features.Team`, `ForwardedHeadersExtensions.cs`, `IReadOnlyList`, `CloudinarySettings`, `PaymobWebhookOutcome`?**
  _High betweenness centrality (0.073) - this node is a cross-community bridge._
- **What connects `CaggOnlineNonShopify`, `CashCashChannel`, `UigOnlineNewNonShopify` to the rest of the system?**
  _631 weakly-connected nodes found - possible documentation gaps or missing edges._
- **Should `TeacherERP.Domain.Shared` be split into smaller, more focused modules?**
  _Cohesion score 0.014869048426095407 - nodes in this community are weakly interconnected._
- **Why does `TeacherERP.Application.Interfaces` connect `IActionResult` to `TeacherERP.Domain.Shared`, `PaymobWebhookOutcome`, `Task`, `TeacherERP.Application.Scheduling`, `TeacherERP.Application.Interfaces`, `CreateAssistantCommandHandler`, `.SaveChangesAsync`, `DateTime`, `.MarkAsUpdated`, `AttendanceRecord`, `Center`, `.Handle`, `IFinancialLedgerRepository.cs`, `IActionResult`, `CreateAssistantCommandHandler`, `.AddApplicationServices`, `GetTeacherDashboardQueryHandler`, `Session`, `🏫 Smart Teacher ERP`, `.Upload`, `.GetByOwnerIdAsync`, `HangfireBasicAuthFilter`, `FinanceController`, `GetSessionsQuery`, `RescheduleSessionRequest`, `RescheduleSessionValidator.cs`, `TeacherAccessRequestStatus`, `AttendanceSettingsDto`, `BaseApiController`, `ResetPasswordValidator.cs`, `ProfileAuthorizationResultHandler.cs`, `CreatedTeacherDto`, `.GetByIdAsync`, `IAttendanceRepository`, `TenantDto`, `RequestGroupByCodeCommandHandler.cs`, `PlatformCommissionSettings`?**
  _High betweenness centrality (0.065) - this node is a cross-community bridge._
- **Should `IActionResult` be split into smaller, more focused modules?**
  _Cohesion score 0.026965408805031447 - nodes in this community are weakly interconnected._
- **Why does `TeacherERP.Domain.Entities` connect `IActionResult` to `TeacherERP.Domain.Shared`, `Task`, `TeacherERP.Application.Scheduling`, `ApplicationUser`, `Result`, `TeacherERP.Application.Interfaces`, `CreateAssistantCommandHandler`, `Guid`, `.MarkAsUpdated`, `AttendanceRecord`, `Center`, `.Handle`, `IFinancialLedgerRepository.cs`, `LoginSession`, `IActionResult`, `TeacherRequestReviewServiceTests`, `Session`, `TeacherERP.Application.Features.Users.Commands.Auth.Login`, `.GetByOwnerIdAsync`, `ApproveTeacherRequestCommand`, `HangfireBasicAuthFilter`, `FinanceController`, `GetSessionsQuery`, `RescheduleSessionRequest`, `TeacherAccessRequestStatus`, `BaseApiController`, `TeacherProvisioningServiceTests`, `RequestGroupByCodeCommandHandler.cs`, `PlatformCommissionSettings`?**
  _High betweenness centrality (0.063) - this node is a cross-community bridge._
- **Should `Task` be split into smaller, more focused modules?**
  _Cohesion score 0.03796825396825397 - nodes in this community are weakly interconnected._