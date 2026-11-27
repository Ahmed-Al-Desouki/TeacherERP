# Graph Report - TeacherERP  (2026-11-27)

## Corpus Check
- cluster-only mode — file stats not available

## Summary
- 4535 nodes · 17558 edges · 135 communities (112 shown, 23 thin omitted)
- Extraction: 90% EXTRACTED · 10% INFERRED · 0% AMBIGUOUS · INFERRED: 1821 edges (avg confidence: 0.85)
- Token cost: 0 input · 0 output

## Graph Freshness
- Built from commit: `b4a85de6`
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
- Exception
- SubmitInstaPayReceiptForm
- PaymentType
- CenterMembershipPolicy.cs
- SchedulePolicy.cs
- RoleNames.cs

## God Nodes (most connected - your core abstractions)
1. `Result` - 468 edges
2. `TeacherERP.Application.Interfaces` - 321 edges
3. `TeacherERP.Domain.Shared` - 317 edges
4. `TeacherERP.Domain.Entities` - 260 edges
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

## Communities (135 total, 23 thin omitted)

### Community 0 - "TeacherERP.Domain.Shared"
Cohesion: 0.02
Nodes (47): TeacherERP.Application.Features.Attendance.RejectCompensation, TeacherERP.Application.Features.Team.GetTeam, TeacherERP.Application.Features.Schedule.BulkCancelSessions, TeacherERP.Application.Features.GroupAdmins.GetMyGroupAdmin, TeacherERP.Application.Features.Finance.GetPaymentOptions, TeacherERP.Application.Features.Students.GetWeeklySchedule, TeacherERP.Application.Features.Finance.UpdateTeacherPaymentSettings, TeacherERP.Application.Features.Finance.ApproveInstaPayReceipt (+39 more)

### Community 1 - "IActionResult"
Cohesion: 0.05
Nodes (26): AdministrationController, AttendanceController, AuditController, AuthController, PasswordController, BaseApiController, CentersController, DashboardController (+18 more)

### Community 2 - "Task"
Cohesion: 0.04
Nodes (13): SetAssistantSuspensionCommand, IAbsenceRepository, GroupAdminItem, GroupJoinCodeLookup, SessionAttachmentItem, TeamMemberItem, AbsenceRepository, CenterMembershipRepository (+5 more)

### Community 3 - "TeacherERP.Application.Scheduling"
Cohesion: 0.02
Nodes (46): TeacherERP.Application.Features.Finance.GetMyPaymentOptions, TeacherERP.Application.Features.GroupAdmins.GetGroupAdminRequests, TeacherERP.Application.Features.TeacherRequests.Review, TeacherERP.Application.Features.Centers.JoinRequests, TeacherERP.Application.Features.SessionContent.UploadSessionAttachment, TeacherERP.Application.Features.Attendance.UpdateAttendanceSettings, TeacherERP.Application.Features.Students.UnblacklistStudent, TeacherERP.Application.Features.Sessions.Logout (+38 more)

### Community 4 - "system"
Cohesion: 0.04
Nodes (21): MoveStudentRequest, LinkTeacherToCenterRequest, UpdateGroupRequest, MoveSessionRequest, AttendanceSettingsInfo, RunningSessionInfo, AuditLogItem, DirectoryCenterInfo (+13 more)

### Community 5 - "ApplicationUser"
Cohesion: 0.03
Nodes (13): TeacherERP.API.Middlewares, TeacherERP.Application, TeacherERP.Infrastructure.Settings, TeacherERP.Infrastructure, TeacherERP.Tests.Integration.Sessions, TeacherERP.Application.Services.Users, TeacherERP.Tests.Integration.Identity, TeacherERP.Application.Sessions (+5 more)

### Community 6 - "Result"
Cohesion: 0.08
Nodes (12): SchoolGrade, CenterMembershipIntegrationTests, TestData, DashboardIntegrationTests, FinanceAuthorizationIntegrationTests, GroupPricingIntegrationTests, GroupGradeIntegrationTests, ScheduleOperationsIntegrationTests (+4 more)

### Community 7 - "TeacherERP.Application.Interfaces"
Cohesion: 0.04
Nodes (24): TeacherRequestPolicy, InitialCreate, FixLoginSessionUserId, MoveCenterStateToMembership, AddCentersAndTeacherSubject, AddGroupScheduleAndSessions, TestSeeder, AddEnrollments (+16 more)

### Community 8 - "CreateAssistantCommandHandler"
Cohesion: 0.05
Nodes (11): TeacherERP.Application.Features.Students.GetStudentDetails, TeacherERP.Application.Absence, TeacherERP.Application.Features.Audit, TeacherERP.Application.Features.Profile.CompleteProfile, TeacherERP.Application.Features.Groups.UpdateGroup, TeacherERP.Domain.Entities, TeacherERP.Application.Features.Profile, TeacherERP.Domain.ValueObjects (+3 more)

### Community 9 - ".SaveChangesAsync"
Cohesion: 0.05
Nodes (65): ApproveCompensationCommand, CancelRequestCommand, CancelRequestCommandHandler, EnrollmentRequestDto, MyEnrollmentsDto, GetMyEnrollmentsQuery, GetMyEnrollmentsQueryHandler, MoveStudentCommand (+57 more)

### Community 11 - "Session"
Cohesion: 0.03
Nodes (19): TeacherERP.Application.Enrollment, TeacherERP.Application.Features.Users.Commands.Auth.SetPassword, TeacherERP.Tests.Unit, TeacherERP.Application.Features.Finance.CreatePaymobIntent, TeacherERP.Application.Features.Enrollment.ApproveRequest, TeacherERP.Application.Features.Administration, TeacherERP.Application.Features.Users.Commands.Auth.ChangePassword, TeacherERP.Application.Features.TeacherRequests.Verify (+11 more)

### Community 12 - "system_data"
Cohesion: 0.08
Nodes (42): ApproveCompensationCommandHandler, RejectCompensationCommand, RejectCompensationCommandHandler, ApproveRequestCommandHandler, RejectRequestCommand, RejectRequestCommandHandler, RequestGroupCommandHandler, RejectGroupAdminRequestCommand (+34 more)

### Community 13 - ".Handle"
Cohesion: 0.08
Nodes (37): CreateGroupCommand, CreateGroupCommandHandler, GetGroupQuery, GetGroupQueryHandler, GetGroupsQueryHandler, GroupDto, GroupSlotDto, IGroupPricing (+29 more)

### Community 14 - "DateTime"
Cohesion: 0.04
Nodes (34): IStudentTimelineRepository, AppDbContext, AttendanceRecords, AuditLogs, CenterInvitations, CenterJoinRequests, Centers, CenterTeachers (+26 more)

### Community 15 - "EnrollmentRequestStatus"
Cohesion: 0.03
Nodes (5): DependencyInjection, CloudinarySettings, ApiKey, ApiSecret, CloudName

### Community 16 - "IFinancialLedgerRepository"
Cohesion: 0.05
Nodes (36): AdminCenterDto, CenterTeacherLinkDto, CreateCenterCommand, CreateCenterCommandHandler, CreateCenterValidator, LinkTeacherToCenterCommand, LinkTeacherToCenterCommandHandler, LinkTeacherToCenterValidator (+28 more)

### Community 17 - "PagedResult"
Cohesion: 0.06
Nodes (23): MonthlyCollectionReportsController, PaymobAdjustmentReviewsController, TeacherMonthlyCollectionReportsController, TeacherPlatformCommissionsController, PageRequest, Skip, EnrollmentRequestListItem, MonthlyCollectionItem (+15 more)

### Community 18 - "IGroupSessionRepository"
Cohesion: 0.10
Nodes (10): TeacherERP.Application.Features.SessionContent, TeacherERP.Application.SessionContent, TeacherERP.API.Security, TeacherERP.API.Controllers, TeacherERP.API.Extensions, ResolvePaymobAdjustmentBody, UpdatePlatformCommissionSettingsRequest, UpdateGroupJoinCodesRequest (+2 more)

### Community 19 - ".MarkAsUpdated"
Cohesion: 0.04
Nodes (50): PagedRequestRules, GetSessionAttendanceValidator, UpdateAttendanceSettingsValidator, AcceptCenterInvitationValidator, CreateCenterInvitationValidator, ApproveCenterJoinRequestValidator, CancelCenterJoinRequestValidator, JoinCenterValidator (+42 more)

### Community 20 - "AttendanceRecord"
Cohesion: 0.09
Nodes (7): IAttendanceCodeService, AttendanceCodeService, AdministrationIntegrationTests, DynamicQrIntegrationTests, SeededTeacher, AttendanceCodeServiceTests, GroupPricingTests

### Community 22 - "Center"
Cohesion: 0.05
Nodes (20): CreateMyCenterValidator, MyEnrollmentDto, RescheduleSessionRequest, PlatformCommissionCollectionRequestResult, PlatformCommissionSettlementResult, CenterJoinRequest, CenterId, DecisionNote (+12 more)

### Community 23 - "Center"
Cohesion: 0.05
Nodes (26): Session, AbsencesProcessedAt, Attendances, BoardImageUrl, CancelReason, EndTime, Group, GroupId (+18 more)

### Community 24 - ".Handle"
Cohesion: 0.08
Nodes (38): GetPendingCompensationsQueryHandler, CenterDto, CenterInvitationDto, CreateMyCenterCommand, CreateMyCenterCommandHandler, GetMyCentersQuery, GetMyCentersQueryHandler, AcceptCenterInvitationCommand (+30 more)

### Community 25 - "IFinancialLedgerRepository.cs"
Cohesion: 0.05
Nodes (44): CloudinaryDotNet (1.29.3), coverlet.collector (6.0.4), FluentValidation (12.1.1), FluentValidation.DependencyInjectionExtensions (12.1.1), Google.Apis.Auth (1.77.0), Hangfire.AspNetCore (1.8.25), Hangfire.Core (1.8.25), Hangfire.Dashboard.BasicAuthorization (1.0.2) (+36 more)

### Community 26 - "AttendanceController.cs"
Cohesion: 0.08
Nodes (30): GetStudentDetailsQuery, GetStudentDetailsQueryHandler, StudentActiveGroupDto, StudentDetailsDto, StudentWeeklyGroupScheduleDto, StudentWeeklyScheduleDto, StudentWeeklyScheduleSlotDto, IStudentRepository (+22 more)

### Community 27 - "Fact"
Cohesion: 0.09
Nodes (11): TeacherRequestToken, VerifyTeacherRequestCommandHandler, VerifyTeacherRequestValidator, VerifyResetOtpCommandHandler, VerifyOtpCommandHandler, IResetGrantService, ResetGrant, ITransientAuthStateStore (+3 more)

### Community 28 - "CustomWebApplicationFactory"
Cohesion: 0.08
Nodes (27): ListTeacherRequestsQuery, ListTeacherRequestsQueryHandler, GetTeacherRequestStatusQuery, GetTeacherRequestStatusQueryHandler, GetTeacherRequestStatusValidator, TeacherRequestStatusDto, ITeacherAccessRequestRepository, TeacherAccessRequestListItem (+19 more)

### Community 29 - "LoginSession"
Cohesion: 0.10
Nodes (9): IEmailService, EmailService, EmailTemplates, MailSettings, DisplayName, Host, Mail, Password (+1 more)

### Community 30 - "MembershipStatus"
Cohesion: 0.05
Nodes (37): AuditAction, AttendanceCorrected, CashPaymentCollected, CompensationApproved, CompensationRejected, FinancialDelegationGranted, FinancialDelegationRevoked, GroupAdminAppointed (+29 more)

### Community 31 - "microsoft_aspnetcore_authorization"
Cohesion: 0.08
Nodes (20): PendingTeacherRequestData, Email, FullName, Note, Subject, TimeZoneId, SubmitTeacherRequestCommandHandler, CreateStudentCommandHandler (+12 more)

### Community 32 - "PasswordController.cs"
Cohesion: 0.09
Nodes (12): CompensationStaffIntegrationTests, ManualAttendanceIntegrationTests, CustomWebApplicationFactory, FileStorage, PaymobGateway, PaymobSettings, PasswordIntegrationTests, Scenario (+4 more)

### Community 33 - "IActionResult"
Cohesion: 0.15
Nodes (7): ForgotPasswordCommand, ForgotPasswordCommandHandler, ForgotPasswordResponseDto, ForgotPasswordValidator, IUserAccountService, JwtProvider, UserAccountService

### Community 34 - ".Handle"
Cohesion: 0.10
Nodes (12): IPaymobGateway, PaymobIntentCommandData, PaymobIntentData, PaymobGateway, TestPaymobGateway, CreateCount, FailNextWithCode, LastRequest (+4 more)

### Community 35 - ".SaveChangesAsync"
Cohesion: 0.11
Nodes (21): AssistantDto, CreateAssistantCommand, CreateAssistantCommandHandler, WelcomeEmailFailedException, CreateAssistantValidator, ResendWelcomeCommand, ResendWelcomeCommandHandler, WelcomeEmailFailedException (+13 more)

### Community 36 - "CreateAssistantCommandHandler"
Cohesion: 0.08
Nodes (20): Group, Capacity, CenterId, DynamicJoinEnabled, DynamicJoinSecret, EducationStage, GradeLevel, GradeNumber (+12 more)

### Community 37 - "GroupDto"
Cohesion: 0.14
Nodes (5): TransientAuthStateStore, OtpService, FrozenTimeProvider, StudentDraft, TransientAuthStateIntegrationTests

### Community 38 - ".AddApplicationServices"
Cohesion: 0.06
Nodes (22): Transaction, Amount, IdempotencyKey, PaymentContextSessionId, ProcessedByAdminId, Provider, ProviderClientSecret, ProviderFailureCode (+14 more)

### Community 39 - "IOtpService"
Cohesion: 0.10
Nodes (11): INotificationRepository, NotificationShareItem, NotificationService, Notification, IsRead, Message, RecipientUserId, RelatedId (+3 more)

### Community 40 - "TeacherRequestReviewServiceTests"
Cohesion: 0.10
Nodes (6): SessionJwtBearerEvents, ISessionValidator, ISessionRepository, SessionRepository, SessionValidator, RefreshTokenRotationTests

### Community 41 - "GetTeacherDashboardQueryHandler"
Cohesion: 0.07
Nodes (24): EnrollmentRequest, DecidedAt, DecidedByUserId, DecisionNote, Group, GroupId, SourceLatitude, SourceLongitude (+16 more)

### Community 42 - "AbstractValidator"
Cohesion: 0.10
Nodes (11): PlatformCommissionsController, RequestPlatformCommissionCollection, SettlePlatformCommissionRequest, IPlatformCommissionSettingsService, PlatformCommissionSettingsDto, PlatformCommissionSettingsService, PlatformCommissionSettings, MonthlySubscriptionAmount (+3 more)

### Community 43 - "Session"
Cohesion: 0.13
Nodes (22): IPagedRequest, Page, PageSize, PagedResult, HasNext, HasPrevious, TotalPages, GetPendingCompensationsQuery (+14 more)

### Community 44 - "fluentvalidation"
Cohesion: 0.15
Nodes (21): AdminScanCommand, AdminScanCommandHandler, AdminScanValidator, ManualAttendanceCommand, ManualAttendanceCommandHandler, ManualAttendanceValidator, ScanAttendanceCommand, ScanAttendanceCommandHandler (+13 more)

### Community 45 - "CancellationToken"
Cohesion: 0.11
Nodes (20): CreatePaymobIntentBody, CreatePaymobIntentCommand, CreatePaymobIntentCommandValidator, CreatePaymobIntentCommandHandler, FinancialDebtItem, FinancialPaymentOptions, FinancialTransactionItem, CreatePaymobPaymentRequest (+12 more)

### Community 46 - "🏫 Smart Teacher ERP"
Cohesion: 0.08
Nodes (23): Tenant, AutoApproveGroupAdminChanges, AutoApproveJoinRequests, IsListedInSearch, Name, OwnerId, Status, Subject (+15 more)

### Community 47 - "HttpGet"
Cohesion: 0.08
Nodes (9): TeacherERP.Application.Features.Attendance, TeacherERP.Application.Attendance, TeacherERP.Application.Features.Attendance.ScanAttendance, TeacherERP.Application.Features.Students, TeacherERP.Application.Features.Attendance.ManualAttendance, TeacherERP.Application.Features.Enrollment.RequestGroup, TeacherERP.Application.Features.Enrollment, TeacherERP.Application.Features.Enrollment.RequestGroupByCode (+1 more)

### Community 48 - "AuthResponseDto"
Cohesion: 0.24
Nodes (4): Scenario, WalkInLocationIntegrationTests, Scenario, TeacherPaymentsIntegrationTests

### Community 49 - ".Upload"
Cohesion: 0.12
Nodes (18): GetGroupSessionAttendanceQueryHandler, GetMyGroupAdminQuery, GetMyGroupAdminQueryHandler, GroupAdminMeDto, GetSessionAttachmentsQuery, GetSessionAttachmentsQueryHandler, SessionAttachmentDto, UploadSessionAttachmentCommand (+10 more)

### Community 50 - "EnrollmentRequest"
Cohesion: 0.08
Nodes (25): PaymentEvent, CurrentStatus, EventType, PayloadSha256, PreviousStatus, Provider, ProviderEventId, ProviderOccurredAtUtc (+17 more)

### Community 51 - "BaseEntity"
Cohesion: 0.08
Nodes (19): PlatformCommission, Amount, CollectionRequestedAtUtc, CollectionRequestedByUserId, GroupId, IsSettled, SessionId, SettledAtUtc (+11 more)

### Community 52 - "GroupSessionRepository"
Cohesion: 0.08
Nodes (22): AttendanceRecord, CompensatesSessionId, RecordedByUserId, ScannedLatitude, ScannedLongitude, ScannedVia, Session, SessionId (+14 more)

### Community 53 - ".Create"
Cohesion: 0.13
Nodes (7): EducationStage, Preparatory, Primary, Secondary, Unspecified, GroupAdminAttendanceIntegrationTests, Scenario

### Community 54 - ".ResolveAsync"
Cohesion: 0.14
Nodes (19): CreatedTeacherDto, ApproveTeacherRequestByAdminCommand, ApproveTeacherRequestByAdminCommandHandler, RejectTeacherRequestByAdminCommand, RejectTeacherRequestByAdminCommandHandler, ApproveTeacherRequestCommand, ApproveTeacherRequestCommandHandler, ApproveTeacherRequestValidator (+11 more)

### Community 55 - "TeacherERP.Application.Features.Users.Commands.Auth.Login"
Cohesion: 0.14
Nodes (13): FinancialSessionInfo, FinancialObligation, Amount, GroupId, GroupName, IsSettled, OutstandingAmount, PaidAmount (+5 more)

### Community 56 - ".GetByOwnerIdAsync"
Cohesion: 0.08
Nodes (7): TeacherERP.Application.Features.TeacherRequests.Submit, TeacherERP.Application.Features.Users.Commands.VerifyOtp, TeacherERP.Application.Features.Team.CreateAssistant, TeacherERP.Application.Features.Users.Commands.Auth.ResetPassword, TeacherERP.Application.Features.Users.Commands.CreateStudent, VerifyOtpCommand, VerifyOtpValidator

### Community 57 - "SeededTeacher"
Cohesion: 0.08
Nodes (21): TeacherERP.Application.Features.Notifications, AdministrationErrors, AttendanceErrors, CenterErrors, EnrollmentErrors, GroupAdminErrors, GroupErrors, NotificationErrors (+13 more)

### Community 59 - "TenantContext"
Cohesion: 0.09
Nodes (18): ApplicationUser, Attendances, DateOfBirth, FullName, Gender, Guardian, IsProfileComplete, LoginSessions (+10 more)

### Community 60 - "ICenterRepository"
Cohesion: 0.09
Nodes (20): PaymobAdjustmentReview, CommissionReversalAmount, Currency, DecisionNote, ProviderAmount, ProviderEventId, ReviewedAtUtc, ReviewedByUserId (+12 more)

### Community 62 - "CompleteProfileValidator"
Cohesion: 0.22
Nodes (4): CancelSessionCommand, CancelSessionValidator, BulkCancelSessionsCommandHandlerTests, CancelSessionCommandHandlerTests

### Community 63 - "SessionManager"
Cohesion: 0.17
Nodes (3): IGroupJoinCodeService, GroupJoinCodeService, GroupJoinCodeServiceTests

### Community 64 - "TeacherERP.Application.Features.Team"
Cohesion: 0.09
Nodes (19): GroupAdminRequest, DecidedAt, DecidedByUserId, DecisionNote, Group, GroupId, RequestedByUserId, Status (+11 more)

### Community 65 - "TeacherERP.Application/DependencyInjection.cs"
Cohesion: 0.15
Nodes (5): ProfileRequirement, AccessTokenResult, IJwtProvider, SessionManager, TokenHasher

### Community 66 - "SetFinancialDelegationCommandHandler.cs"
Cohesion: 0.15
Nodes (9): IFileStorage, StoredFile, IStoredFileCleanupJob, StoredFileCleanupJob, TeacherPaymentService, CloudinaryFileStorage, TestFileStorage, DeletedPublicIds (+1 more)

### Community 67 - "RevokeSessionCommand"
Cohesion: 0.09
Nodes (23): ASPNETCORE_ENVIRONMENT, applicationUrl, commandName, dotnetRunMessages, environmentVariables, applicationUrl, commandName, dotnetRunMessages (+15 more)

### Community 68 - "CancelSessionCommandHandler.cs"
Cohesion: 0.09
Nodes (21): LoginSession, AbsoluteExpiresAt, DeviceId, DeviceName, LastRefreshedAt, PreviousRefreshTokenHash, RefreshExpiresAt, RefreshTokenHash (+13 more)

### Community 69 - "HangfireBasicAuthFilter"
Cohesion: 0.12
Nodes (5): UpdateGroupScheduleRequest, SessionTimeCalculator, AddStudentProfile, AddTenants, AddPaymobAdjustmentReviews

### Community 71 - "FinanceController"
Cohesion: 0.09
Nodes (22): NotificationType, BulkSessionsCancelled, BulkSessionsRescheduled, CompensationApproved, CompensationRejected, CompensationRequested, EnrollmentApproved, EnrollmentRejected (+14 more)

### Community 72 - "LoggingBehavior"
Cohesion: 0.13
Nodes (12): CompletedProfileHandler, CompletedProfileRequirement, TenantHandler, TenantRequirement, AllowedRoles, RequiresFinancialDelegation, ProfileCompletionChecker, IProfileCompletionChecker (+4 more)

### Community 73 - "GetAuditLogQuery"
Cohesion: 0.19
Nodes (15): TeacherERP.Application.Features.Directory, DirectoryCenterDto, DirectoryCenterInfoDto, DirectoryGroupDto, DirectoryTeacherDto, TeacherProfileDto, GetTeacherProfileQuery, GetTeacherProfileQueryHandler (+7 more)

### Community 74 - "GetSessionsQuery"
Cohesion: 0.10
Nodes (16): IPaymobCallbackVerifier, PaymobSettings, ApiKey, BaseUrl, Currency, HmacNullValue, HmacSecret, InitializationLeaseSeconds (+8 more)

### Community 75 - "CachedRegistrationData"
Cohesion: 0.16
Nodes (13): CompleteProfileCommand, CompleteProfileCommandHandler, GetMyProfileQuery, GetMyProfileQueryHandler, GuardianDto, ProfileDto, GetStudentCardQuery, GetStudentCardQueryHandler (+5 more)

### Community 76 - "GetMyNotificationsQuery"
Cohesion: 0.24
Nodes (8): ApproveInstaPayReceiptCommand, ApproveInstaPayReceiptCommandHandler, CollectCashPaymentCommand, CollectCashPaymentCommandHandler, CashPaymentRequest, CashPaymentResult, IFinancialLedgerService, FinancialLedgerService

### Community 77 - "UpdateGroupValidator"
Cohesion: 0.23
Nodes (8): ChangePasswordCommand, ChangePasswordCommandHandler, ChangePasswordValidator, SetPasswordCommand, SetPasswordCommandHandler, SetPasswordValidator, ICredentialService, PasswordCommandTests

### Community 78 - ".Handle"
Cohesion: 0.11
Nodes (15): TeacherERP.Tests.Unit.Common, MonthlySubscription, Amount, GroupId, Month, PaymentTransactionId, StudentId, TenantId (+7 more)

### Community 79 - "SessionValidator"
Cohesion: 0.11
Nodes (4): TeacherERP.Application.Centers, TeacherERP.Application.Features.Centers.CreateCenter, TeacherERP.Application.Features.Centers, TeacherERP.Application.Features.Centers.Invitations

### Community 80 - "RescheduleSessionRequest"
Cohesion: 0.20
Nodes (11): AdministratorDashboardDto, TeacherDashboardDto, GetAdministratorDashboardQuery, GetAdministratorDashboardQueryHandler, GetTeacherDashboardQuery, GetTeacherDashboardQueryHandler, AdministratorDashboardOperationalStats, DashboardUpcomingSession (+3 more)

### Community 81 - "PageRequest"
Cohesion: 0.11
Nodes (15): EnrollmentStatus, Active, Ended, GroupEnrollment, ApprovedByUserId, EndedAt, EnrolledAt, Group (+7 more)

### Community 83 - "RescheduleSessionValidator.cs"
Cohesion: 0.11
Nodes (18): UpdateTeacherPaymentSettingsForm, InstaPayLink, QrImage, SubmitInstaPayReceiptForm, Amount, Receipt, SessionId, Type (+10 more)

### Community 84 - "Error"
Cohesion: 0.16
Nodes (5): SnapshotAccessor, SnapshotAccessor, AddSessionOriginalStartTime, SnapshotAccessor, AppDbContextModelSnapshot

### Community 85 - ".ApproveAsync"
Cohesion: 0.22
Nodes (12): ActivateAccountCommand, ActivateAccountCommandHandler, ActivateAccountValidator, AuthResponseDto, LoginCommand, LoginCommandHandler, LoginValidator, RefreshTokenCommand (+4 more)

### Community 87 - "TeacherAccessRequestStatus"
Cohesion: 0.12
Nodes (5): TeacherERP.Application.Features.Groups, TeacherERP.Application.GroupAdmins, TeacherERP.Application.Features.Groups.CreateGroup, TeacherERP.Application.Features.GroupAdmins, GroupAdminPolicy

### Community 88 - "AttendanceSettingsDto"
Cohesion: 0.24
Nodes (10): AttendanceSettingsDto, DisplayCodeDto, GetAttendanceSettingsQuery, GetAttendanceSettingsQueryHandler, GetDisplayCodeQuery, GetDisplayCodeQueryHandler, RegenerateStaticCodeCommand, RegenerateStaticCodeCommandHandler (+2 more)

### Community 89 - "BaseApiController"
Cohesion: 0.16
Nodes (10): GoogleSignInCommand, GoogleSignInCommandHandler, GoogleSignInValidator, IExternalLoginService, GoogleUserInfo, IGoogleAuthService, ExternalLoginService, GoogleAuthService (+2 more)

### Community 90 - "GoogleSignInValidator.cs"
Cohesion: 0.19
Nodes (3): Callback, PaymobErrors, PaymobPaymentService

### Community 91 - "ResetPasswordValidator.cs"
Cohesion: 0.12
Nodes (15): 🔒 1. Advanced Security & Session Control, 📍 2. Hybrid Anti-Cheat Attendance System, 💰 3. Strict Financial Ledger & Privacy, Ahmed Al-Desouki, 🏛️ Architecture, 👨‍💻 Author, Center Management System, 🛡️ Core Features (+7 more)

### Community 92 - "SessionAttachment"
Cohesion: 0.17
Nodes (9): FileNames, DetectedFileType, Jpeg, Pdf, Png, Unknown, Webp, FileSignature (+1 more)

### Community 93 - "TeacherProvisioningServiceTests"
Cohesion: 0.22
Nodes (5): IPasswordGenerator, TeacherProvisioningService, WelcomeEmailFailedException, PasswordGenerator, TeacherProvisioningServiceTests

### Community 94 - ".ResolveAsync"
Cohesion: 0.14
Nodes (12): SessionAttachment, Caption, ContentType, FileName, Kind, PublicId, Session, SessionId (+4 more)

### Community 95 - "ProfileAuthorizationResultHandler.cs"
Cohesion: 0.12
Nodes (15): TransientAuthState, CodeExpiresAtUtc, Email, FailedAttempts, GrantExpiresAtUtc, GrantHash, Id, IsLocked (+7 more)

### Community 96 - "TeacherERP.Application.Features.Users.Commands.Auth.Login"
Cohesion: 0.15
Nodes (4): TeacherERP.Application.Features.Users.Commands.Auth.GoogleSignIn, TeacherERP.Application.Features.Users.Commands.Auth.Login, TeacherERP.Application.Features.Users.Commands.Auth.ActivateAccount, TeacherERP.Application.Features.Users.Commands.Auth.RefreshToken

### Community 97 - "CenterJoinRequestListItem"
Cohesion: 0.22
Nodes (12): CenterJoinRequestListItem, GetCenterJoinRequestsQuery, GetCenterJoinRequestsQueryHandler, GetCenterJoinRequestsValidator, GetMyCenterJoinRequestsQuery, GetMyCenterJoinRequestsQueryHandler, GetMyCenterJoinRequestsValidator, CenterJoinRequestStatus (+4 more)

### Community 98 - "CreatedTeacherDto"
Cohesion: 0.23
Nodes (11): ApproveGroupAdminRequestCommand, ApproveGroupAdminRequestCommandHandler, GetGroupAdminsQuery, GetGroupAdminsQueryHandler, GroupAdminChangeResultDto, GroupAdminDto, RequestGroupAdminChangeCommand, RequestGroupAdminChangeCommandHandler (+3 more)

### Community 99 - ".GetByIdAsync"
Cohesion: 0.14
Nodes (12): CenterInvitation, AcceptedAtUtc, AcceptedByTenantId, CenterId, CreatedByUserId, ExpiresAtUtc, Status, TokenHash (+4 more)

### Community 100 - "GroupScheduleSlot"
Cohesion: 0.19
Nodes (7): GroupScheduleSlot, Day, DurationMinutes, Group, GroupId, StartTime, TenantId

### Community 102 - "MoveStudentCommandHandler.cs"
Cohesion: 0.30
Nodes (6): CreateTeacherCommand, CreateTeacherCommandHandler, CreateTeacherValidator, ITeacherProvisioningService, CreateTeacherCommandHandlerTests, CreateTeacherValidatorTests

### Community 103 - "GroupScanValidator.cs"
Cohesion: 0.23
Nodes (5): IAbsenceProcessor, TenantContext, CurrentTenantAccessor, Current, TenantId

### Community 104 - ".GetByIdAsync"
Cohesion: 0.18
Nodes (7): TeacherPaymentSettings, InstaPayLink, QrImagePublicId, QrImageUrl, TenantId, ITenantOwned, TenantId

### Community 110 - "CurrentUserService"
Cohesion: 0.20
Nodes (7): IUserTimeZoneAccessor, TimeZoneId, CurrentUserService, SessionId, UserId, HttpUserTimeZoneAccessor, TimeZoneId

### Community 112 - "Gender"
Cohesion: 0.20
Nodes (7): Gender, Female, Male, GuardianInfo, Name, PhoneNumber, Relation

### Community 113 - "GetGroupJoinCodesQueryHandler"
Cohesion: 0.50
Nodes (5): GetGroupJoinCodesQuery, GetGroupJoinCodesQueryHandler, GroupJoinCodesDto, UpdateGroupJoinCodesCommand, UpdateGroupJoinCodesCommandHandler

### Community 114 - "TenantDto"
Cohesion: 0.50
Nodes (5): GetCurrentTenantQuery, GetCurrentTenantQueryHandler, GetMyTenantsQuery, GetMyTenantsQueryHandler, TenantDto

### Community 115 - "ControllerBase"
Cohesion: 0.29
Nodes (3): PaymobReturnController, PaymobWebhookController, RolesController

### Community 116 - "RequestGroupByCodeCommandHandler.cs"
Cohesion: 0.48
Nodes (4): TeacherERP.Application.Features.Sessions.GetMySessions, GetMySessionsQuery, GetMySessionsQueryHandler, SessionDto

### Community 119 - ".VerifyAsync"
Cohesion: 0.48
Nodes (4): GetStudentLearningTimelineQuery, GetStudentLearningTimelineValidator, StudentTimelineAttachmentDto, StudentTimelineSessionDto

### Community 120 - "TeacherERP.Application.Features.Team"
Cohesion: 0.33
Nodes (3): TeacherERP.Application.Features.Team, TeamErrors, TeamPolicy

### Community 122 - "GetNotificationWhatsAppShareQuery"
Cohesion: 0.53
Nodes (4): AuditLogDto, GetAuditLogQuery, GetAuditLogQueryHandler, GetAuditLogValidator

### Community 123 - "IReadOnlyList"
Cohesion: 0.53
Nodes (4): EnrollmentRequestListDto, GetRequestsQuery, GetRequestsQueryHandler, GetRequestsValidator

### Community 124 - "CloudinarySettings"
Cohesion: 0.53
Nodes (4): GetGroupAdminRequestsQuery, GetGroupAdminRequestsQueryHandler, GetGroupAdminRequestsValidator, GroupAdminRequestDto

### Community 125 - "GetSessionsQuery"
Cohesion: 0.40
Nodes (4): GetSessionsQuery, GetSessionsValidator, SessionDto, SessionListItem

### Community 126 - ".TokenValidated"
Cohesion: 0.53
Nodes (4): GetStudentsQuery, GetStudentsQueryHandler, GetStudentsValidator, StudentListDto

### Community 127 - "PaymobWebhookOutcome"
Cohesion: 0.33
Nodes (6): PaymobWebhookOutcome, Accepted, Duplicate, Ignored, InvalidPayload, InvalidSignature

### Community 130 - "SubmitInstaPayReceiptForm"
Cohesion: 0.40
Nodes (5): JwtSettings, Audience, ExpiryMinutes, Issuer, Secret

## Knowledge Gaps
- **628 isolated node(s):** `Day`, `DurationMinutes`, `Group`, `GroupId`, `StartTime` (+623 more)
  These have ≤1 connection - possible missing edges or undocumented components. (Counts symbols only; 892 node(s) total have ≤1 connection when file, concept and rationale nodes are included.)
- **23 thin communities (<3 nodes) omitted from report** — run `graphify query` to explore isolated nodes.

## Suggested Questions
_Questions this graph is uniquely positioned to answer:_

- **Why does `AppDbContext` connect `DateTime` to `Task`, `system`, `Result`, `Guid`, `EnrollmentRequestStatus`, `Center`, `AttendanceController.cs`, `Fact`, `CustomWebApplicationFactory`, `MembershipStatus`, `PasswordController.cs`, `IActionResult`, `GroupDto`, `IOtpService`, `TeacherRequestReviewServiceTests`, `AuthResponseDto`, `ApproveTeacherRequestCommand`, `GetMySessionsQueryHandler`, `GetAuditLogQuery`, `RescheduleSessionRequest`, `Task`, `Error`, `ProfileAuthorizationResultHandler.cs`, `.ConfigureWebHost`, `PlatformCommissionSettings`?**
  _High betweenness centrality (0.062) - this node is a cross-community bridge._
- **What connects `Day`, `DurationMinutes`, `Group` to the rest of the system?**
  _628 weakly-connected nodes found - possible documentation gaps or missing edges._
- **Should `TeacherERP.Domain.Shared` be split into smaller, more focused modules?**
  _Cohesion score 0.02152621101526211 - nodes in this community are weakly interconnected._
- **Why does `TeacherERP.Application.Interfaces` connect `TeacherERP.Domain.Shared` to `TeacherERP.Application.Scheduling`, `ApplicationUser`, `CreateAssistantCommandHandler`, `Guid`, `Session`, `.Handle`, `PagedResult`, `IGroupSessionRepository`, `IRequestHandler`, `Center`, `.Handle`, `AttendanceController.cs`, `Fact`, `LoginSession`, `.Handle`, `.SaveChangesAsync`, `TeacherRequestReviewServiceTests`, `fluentvalidation`, `CancellationToken`, `HttpGet`, `.GetByOwnerIdAsync`, `SessionManager`, `TeacherERP.Application/DependencyInjection.cs`, `SetFinancialDelegationCommandHandler.cs`, `LoggingBehavior`, `GetAuditLogQuery`, `CachedRegistrationData`, `SessionValidator`, `RescheduleSessionRequest`, `TeacherAccessRequestStatus`, `TeacherProvisioningServiceTests`, `TeacherERP.Application.Features.Users.Commands.Auth.Login`, `GroupScanValidator.cs`, `CurrentUserService`?**
  _High betweenness centrality (0.060) - this node is a cross-community bridge._
- **Should `IActionResult` be split into smaller, more focused modules?**
  _Cohesion score 0.0499001996007984 - nodes in this community are weakly interconnected._
- **Why does `TeacherERP.Domain.Entities` connect `CreateAssistantCommandHandler` to `TeacherERP.Domain.Shared`, `TeacherERP.Application.Scheduling`, `ApplicationUser`, `.SaveChangesAsync`, `Guid`, `Session`, `.Handle`, `IFinancialLedgerRepository`, `PagedResult`, `IGroupSessionRepository`, `AttendanceController.cs`, `.Handle`, `GetTeacherDashboardQueryHandler`, `AbstractValidator`, `fluentvalidation`, `CancellationToken`, `HttpGet`, `EnrollmentRequest`, `BaseEntity`, `GroupSessionRepository`, `.GetByOwnerIdAsync`, `ICenterRepository`, `TeacherERP.Application.Features.Team`, `TeacherERP.Application/DependencyInjection.cs`, `GetAuditLogQuery`, `.Handle`, `SessionValidator`, `TeacherAccessRequestStatus`, `AttendanceSettingsDto`, `ProfileAuthorizationResultHandler.cs`, `TeacherERP.Application.Features.Users.Commands.Auth.Login`, `.GetByIdAsync`, `Gender`?**
  _High betweenness centrality (0.054) - this node is a cross-community bridge._
- **Should `Task` be split into smaller, more focused modules?**
  _Cohesion score 0.036163522012578615 - nodes in this community are weakly interconnected._