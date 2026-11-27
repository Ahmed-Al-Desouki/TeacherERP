# Graph Report - TeacherERP  (2026-11-27)

## Corpus Check
- cluster-only mode — file stats not available

## Summary
- 4264 nodes · 17310 edges · 123 communities (103 shown, 20 thin omitted)
- Extraction: 90% EXTRACTED · 10% INFERRED · 0% AMBIGUOUS · INFERRED: 1806 edges (avg confidence: 0.85)
- Token cost: 0 input · 0 output

## Graph Freshness
- Built from commit: `5254dbec`
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
- BaseApiController
- GoogleSignInValidator.cs
- ResetPasswordValidator.cs
- SessionAttachment
- .ResolveAsync
- ProfileAuthorizationResultHandler.cs
- CreatedTeacherDto
- .GetByIdAsync
- .AddInfrastructureServices
- MoveStudentCommandHandler.cs
- GroupScanValidator.cs
- .GetByIdAsync
- IAttendanceRepository
- LogoutValidator.cs
- IPagedRequest
- RequestGroupByCodeCommandHandler.cs
- PlatformCommissionSettings
- .VerifyAsync
- ForwardedHeadersExtensions.cs
- GetNotificationWhatsAppShareQuery
- IReadOnlyList
- CloudinarySettings
- .TokenValidated
- PaymobWebhookOutcome
- SubmitInstaPayReceiptForm
- PaymentType
- GetMyNotificationsQuery
- GetMySessionsQueryHandler
- EgyptianPhoneNumber.cs
- CentersController.cs
- PaymobWebhookOutcome
- IPasswordGenerator
- 20261016061231_AddCentersAndTeacherSubject.Designer.cs
- 20261028110215_AddAbsenceProcessing.Designer.cs
- HangfireBasicAuthFilter

## God Nodes (most connected - your core abstractions)
1. `Result` - 501 edges
2. `TeacherERP.Domain.Shared` - 329 edges
3. `TeacherERP.Application.Interfaces` - 316 edges
4. `TeacherERP.Domain.Entities` - 259 edges
5. `TeacherERP.Application.Common` - 122 edges
6. `AppDbContext` - 116 edges
7. `PagedResult` - 107 edges
8. `ICurrentTenantAccessor` - 97 edges
9. `ApplicationUser` - 97 edges
10. `IUnitOfWork` - 80 edges

## Surprising Connections (you probably didn't know these)
- `🔒 1. Advanced Security & Session Control` --references--> `ApplicationUser`  [INFERRED]
  README.md → TeacherERP.Domain/Entities/Users/User.cs
- `🗄️ Database Schema` --references--> `TenantSettings`  [INFERRED]
  README.md → TeacherERP.Domain/Entities/Tenants/TenantSettings.cs
- `🗄️ Database Schema` --references--> `ApplicationUser`  [INFERRED]
  README.md → TeacherERP.Domain/Entities/Users/User.cs
- `AdministrationErrors` --references--> `Error`  [EXTRACTED]
  TeacherERP.Application/Features/Administration/AdministrationErrors.cs → TeacherERP.Domain/Shared/Error.cs
- `GroupErrors` --references--> `Error`  [EXTRACTED]
  TeacherERP.Application/Features/Groups/GroupErrors.cs → TeacherERP.Domain/Shared/Error.cs

## Import Cycles
- None detected.

## Communities (123 total, 20 thin omitted)

### Community 0 - "TeacherERP.Domain.Shared"
Cohesion: 0.02
Nodes (75): TeacherERP.Application.Features.Attendance.RejectCompensation, TeacherERP.Application.Features.Users.Commands.Auth.SetPassword, TeacherERP.Application.Features.Team.GetTeam, TeacherERP.Application.Features.GroupAdmins.GetMyGroupAdmin, TeacherERP.Application.Features.Notifications, TeacherERP.Application.Features.Students.GetStudentDetails, TeacherERP.Application.Features.Finance.GetPaymentOptions, TeacherERP.Application.Features.Students.GetWeeklySchedule (+67 more)

### Community 1 - "IActionResult"
Cohesion: 0.02
Nodes (61): TeacherERP.Application.Features.TeacherRequests.Review, TeacherERP.Application.Features.Centers.Invitations, PagedRequestRules, CreateCenterValidator, GetPendingCompensationsValidator, GetSessionAttendanceValidator, GetAuditLogValidator, AcceptCenterInvitationValidator (+53 more)

### Community 2 - "Task"
Cohesion: 0.04
Nodes (11): TeacherERP.API.Middlewares, TeacherERP.Infrastructure.Settings, TeacherERP.Infrastructure, TeacherERP.Tests.Integration.Sessions, TeacherERP.Domain.ValueObjects, TeacherERP.Application.Sessions, TeacherERP.Application.Features.Users.Commands.Auth.ForgotPassword, TeacherERP.Tests.Integration (+3 more)

### Community 3 - "TeacherERP.Application.Scheduling"
Cohesion: 0.06
Nodes (13): TeacherERP.Application.Features.Schedule.BulkCancelSessions, TeacherERP.Application.Scheduling, TeacherERP.Application.Features.Schedule.GetSessions, TeacherERP.Application.Features.Schedule, TeacherERP.Application.Features.Groups.UpdateGroup, TeacherERP.Application.Features.Schedule.BulkShiftSessions, TeacherERP.Application.Features.Groups.UpdateGroupSchedule, TeacherERP.Application.Features.Schedule.RescheduleSession (+5 more)

### Community 4 - "system"
Cohesion: 0.03
Nodes (38): CenterMembershipPolicy, UpdateGroupScheduleRequest, MoveSessionRequest, TeacherRequestPolicy, PlatformCommissionCollectionRequestResult, PlatformCommissionSettlementResult, SessionTimeCalculator, SessionPolicy (+30 more)

### Community 5 - "ApplicationUser"
Cohesion: 0.08
Nodes (20): IExternalLoginService, ApplicationUser, Attendances, DateOfBirth, FullName, Gender, Guardian, IsProfileComplete (+12 more)

### Community 6 - "Result"
Cohesion: 0.05
Nodes (61): ApproveCompensationCommand, DisplayCodeDto, GetDisplayCodeQuery, GetDisplayCodeQueryHandler, RejectCompensationCommand, RejectCenterJoinRequestCommand, CancelRequestCommand, RejectRequestCommand (+53 more)

### Community 7 - "TeacherERP.Application.Interfaces"
Cohesion: 0.04
Nodes (29): TeacherERP.Application.Features.Attendance, TeacherERP.Application.Features.GroupAdmins.GetGroupAdminRequests, TeacherERP.Application.Attendance, TeacherERP.Application.Absence, TeacherERP.Application.Centers, TeacherERP.Application.Features.Attendance.ScanAttendance, TeacherERP.Application.Features.GroupAdmins.GroupScan, TeacherERP.Application.Features.Audit (+21 more)

### Community 8 - "CreateAssistantCommandHandler"
Cohesion: 0.11
Nodes (9): SchoolGrade, TestData, DashboardIntegrationTests, GroupPricingIntegrationTests, ScheduleOperationsIntegrationTests, SessionMoveIntegrationTests, StudentExperienceIntegrationTests, StudentTransferIntegrationTests (+1 more)

### Community 9 - ".SaveChangesAsync"
Cohesion: 0.09
Nodes (11): ApproveInstaPayReceiptCommand, ApproveInstaPayReceiptCommandHandler, CollectCashPaymentCommand, CollectCashPaymentCommandHandler, TeacherRequestQrDto, VerifyTeacherRequestCommandHandler, CashPaymentRequest, CashPaymentResult (+3 more)

### Community 10 - "Guid"
Cohesion: 0.06
Nodes (17): RequestPlatformCommissionCollection, SettlePlatformCommissionRequest, MoveStudentRequest, LinkTeacherToCenterRequest, MyEnrollmentDto, GetGroupAdminsQuery, GroupAdminDto, IAbsenceRepository (+9 more)

### Community 12 - "system_data"
Cohesion: 0.05
Nodes (11): TeacherERP.Application.Enrollment, TeacherERP.Application.Features.TeacherRequests.Submit, TeacherERP.Tests.Unit, TeacherERP.Application.Features.Enrollment.ApproveRequest, TeacherERP.Application.Features.TeacherRequests.Verify, TeacherERP.Application.Features.Centers.CreateCenter, TeacherERP.Tests.Unit.Sessions, TeacherERP.Application.Features.Administration.CreateTeacher (+3 more)

### Community 13 - ".Handle"
Cohesion: 0.16
Nodes (16): TeacherERP.Application.Features.Directory, DirectoryCenterDto, DirectoryCenterInfoDto, DirectoryGroupDto, DirectoryTeacherDto, TeacherProfileDto, GetTeacherProfileQuery, GetTeacherProfileQueryHandler (+8 more)

### Community 14 - "DateTime"
Cohesion: 0.06
Nodes (30): AppDbContext, AttendanceRecords, AuditLogs, CenterInvitations, CenterJoinRequests, Centers, CenterTeachers, CurrentTenantId (+22 more)

### Community 15 - "EnrollmentRequestStatus"
Cohesion: 0.16
Nodes (12): EnrollmentRequestListDto, GetRequestsQuery, EnrollmentRequestListItem, MyRequestItem, EnrollmentRequestStatus, Approved, Cancelled, Pending (+4 more)

### Community 16 - "IFinancialLedgerRepository"
Cohesion: 0.05
Nodes (24): FinancialSessionInfo, FinancialTransactionInfo, IFinancialLedgerRepository, FinancialLedgerService, FinancialObligation, Amount, GroupId, GroupName (+16 more)

### Community 17 - "PagedResult"
Cohesion: 0.17
Nodes (14): PagedResult, HasNext, HasPrevious, TotalPages, GetPendingCompensationsQuery, GetPendingCompensationsQueryHandler, GetPendingInstaPayReceiptsQuery, GetPendingInstaPayReceiptsValidator (+6 more)

### Community 18 - "IGroupSessionRepository"
Cohesion: 0.05
Nodes (37): AuditAction, AttendanceCorrected, CashPaymentCollected, CompensationApproved, CompensationRejected, FinancialDelegationGranted, FinancialDelegationRevoked, GroupAdminAppointed (+29 more)

### Community 19 - ".MarkAsUpdated"
Cohesion: 0.03
Nodes (61): Transaction, Amount, IdempotencyKey, PaymentContextSessionId, ProcessedByAdminId, Provider, ProviderClientSecret, ProviderFailureCode (+53 more)

### Community 20 - "AttendanceRecord"
Cohesion: 0.12
Nodes (7): AdministrationController, AuthController, PasswordController, CentersController, TeacherRequestsController, UsersController, RejectTeacherRequestRequest

### Community 21 - "IRequestHandler"
Cohesion: 0.12
Nodes (68): LinkTeacherToCenterCommandHandler, ApproveCompensationCommandHandler, ManualAttendanceCommandHandler, RejectCompensationCommandHandler, AcceptCenterInvitationCommandHandler, CreateCenterInvitationCommandHandler, ApproveCenterJoinRequestCommandHandler, CancelCenterJoinRequestCommandHandler (+60 more)

### Community 22 - "Center"
Cohesion: 0.07
Nodes (27): GetMyPaymentOptionsQuery, GetMyPaymentOptionsQueryHandler, GetMyPaymentOptionsValidator, GetPaymentOptionsQuery, GetPaymentOptionsQueryHandler, GetTeacherPaymentSettingsQuery, GetTeacherPaymentSettingsQueryHandler, SubmitInstaPayReceiptCommand (+19 more)

### Community 23 - "Center"
Cohesion: 0.14
Nodes (8): Center, Address, GeofenceRadiusMeters, Latitude, Longitude, Name, OwnerUserId, CenterRepository

### Community 24 - ".Handle"
Cohesion: 0.06
Nodes (33): INotificationRepository, NotificationShareItem, NotificationService, Notification, IsRead, Message, RecipientUserId, RelatedId (+25 more)

### Community 25 - "IFinancialLedgerRepository.cs"
Cohesion: 0.33
Nodes (7): MonthlyCollectionItem, MonthlyCollectionReport, MonthlyCollectionTotals, MonthlyOutstandingDebtItem, MonthlyPaymentMethodSummary, IMonthlyCollectionReportService, MonthlyCollectionReportService

### Community 26 - "AttendanceController.cs"
Cohesion: 0.13
Nodes (3): IAttendanceCodeService, AttendanceCodeService, AttendanceCodeServiceTests

### Community 27 - "Fact"
Cohesion: 0.16
Nodes (3): CenterMembershipIntegrationTests, PasswordIntegrationTests, SchoolGradeTests

### Community 28 - "CustomWebApplicationFactory"
Cohesion: 0.11
Nodes (10): CompensationStaffIntegrationTests, CustomWebApplicationFactory, FileStorage, PaymobGateway, PaymobSettings, GroupGradeIntegrationTests, GroupJoinCodeIntegrationTests, RefreshTokenRotationIntegrationTests (+2 more)

### Community 29 - "LoginSession"
Cohesion: 0.06
Nodes (30): AccessTokenResult, IJwtProvider, ISessionValidator, ISessionRepository, SessionManager, TokenHasher, LoginSession, AbsoluteExpiresAt (+22 more)

### Community 30 - "MembershipStatus"
Cohesion: 0.06
Nodes (37): GetStudentDetailsQuery, GetStudentDetailsQueryHandler, StudentActiveGroupDto, StudentDetailsDto, GetStudentsQuery, GetStudentsQueryHandler, StudentListDto, GetTeamQuery (+29 more)

### Community 31 - "microsoft_aspnetcore_authorization"
Cohesion: 0.11
Nodes (7): TeacherERP.Application.SessionContent, TeacherERP.API.Security, TeacherERP.Application.Features.Attendance.ManualAttendance, TeacherERP.API.Controllers, TeacherERP.API.Extensions, RateLimitPolicies, AuthPolicies

### Community 32 - "PasswordController.cs"
Cohesion: 0.05
Nodes (44): CloudinaryDotNet (1.29.3), coverlet.collector (6.0.4), FluentValidation (12.1.1), FluentValidation.DependencyInjectionExtensions (12.1.1), Google.Apis.Auth (1.77.0), Hangfire.AspNetCore (1.8.25), Hangfire.Core (1.8.25), Hangfire.Dashboard.BasicAuthorization (1.0.2) (+36 more)

### Community 33 - "IActionResult"
Cohesion: 0.08
Nodes (10): AttendanceController, GroupAdminsController, GroupsController, UpdateGroupJoinCodesRequest, ScheduleController, TeachersController, TeamController, UpdateCenterRequest (+2 more)

### Community 34 - ".Handle"
Cohesion: 0.10
Nodes (12): AddExtraSessionCommand, AddExtraSessionValidator, MoveSessionCommand, MoveSessionValidator, RescheduleSessionCommand, RescheduleSessionValidator, RestoreSessionCommand, SessionTimeDto (+4 more)

### Community 35 - ".SaveChangesAsync"
Cohesion: 0.04
Nodes (33): Group, Capacity, CenterId, DynamicJoinEnabled, DynamicJoinSecret, EducationStage, GradeLevel, GradeNumber (+25 more)

### Community 36 - "CreateAssistantCommandHandler"
Cohesion: 0.14
Nodes (9): TeacherERP.Application.Features.Team, AssistantDto, CreateAssistantCommand, CreateAssistantCommandHandler, WelcomeEmailFailedException, CreateAssistantValidator, TeamPolicy, ITeamRepository (+1 more)

### Community 37 - "GroupDto"
Cohesion: 0.16
Nodes (9): GetGroupQuery, GetGroupQueryHandler, GetGroupsQuery, GroupDto, GroupSlotDto, GroupSlotInput, IGroupSchedule, Slots (+1 more)

### Community 38 - ".AddApplicationServices"
Cohesion: 0.08
Nodes (17): DependencyInjection, MoveStudentCommand, MoveStudentCommandHandler, IAbsenceProcessingJob, IAbsenceProcessor, IAbsenceService, ICenterJoinRequestNotifier, ICenterMembershipCleanupJob (+9 more)

### Community 39 - "IOtpService"
Cohesion: 0.08
Nodes (22): CachedTeacherRequestData, Email, FullName, Note, Subject, TimeZoneId, SubmitTeacherRequestCommand, SubmitTeacherRequestCommandHandler (+14 more)

### Community 41 - "GetTeacherDashboardQueryHandler"
Cohesion: 0.20
Nodes (11): AdministratorDashboardDto, TeacherDashboardDto, GetAdministratorDashboardQuery, GetAdministratorDashboardQueryHandler, GetTeacherDashboardQuery, GetTeacherDashboardQueryHandler, AdministratorDashboardOperationalStats, DashboardUpcomingSession (+3 more)

### Community 42 - "AbstractValidator"
Cohesion: 0.13
Nodes (3): IEmailService, EmailService, EmailTemplates

### Community 43 - "Session"
Cohesion: 0.07
Nodes (18): CancelSessionCommand, Session, AbsencesProcessedAt, Attendances, BoardImageUrl, CancelReason, EndTime, Group (+10 more)

### Community 44 - "fluentvalidation"
Cohesion: 0.11
Nodes (18): LinkTeacherToCenterCommand, LinkTeacherToCenterValidator, CenterTeacher, Center, CenterId, Origin, Status, TenantId (+10 more)

### Community 45 - "CancellationToken"
Cohesion: 0.11
Nodes (7): PlatformCommissionsController, UpdatePlatformCommissionSettingsRequest, EnrollmentController, TeacherPlatformCommissionsController, NotificationsController, RejectRequestRequest, IPlatformCommissionService

### Community 46 - "🏫 Smart Teacher ERP"
Cohesion: 0.12
Nodes (15): 🔒 1. Advanced Security & Session Control, 📍 2. Hybrid Anti-Cheat Attendance System, 💰 3. Strict Financial Ledger & Privacy, Ahmed Al-Desouki, 🏛️ Architecture, 👨‍💻 Author, Center Management System, 🛡️ Core Features (+7 more)

### Community 47 - "HttpGet"
Cohesion: 0.10
Nodes (8): AuditController, DashboardController, DirectoryController, StudentPaymentsController, StudentsController, StudentTimelineController, ProfileController, StudentReasonRequest

### Community 48 - "AuthResponseDto"
Cohesion: 0.23
Nodes (3): ManualAttendanceIntegrationTests, PaymobPaymentIntegrationTests, Scenario

### Community 50 - "EnrollmentRequest"
Cohesion: 0.05
Nodes (37): ApproveRequestCommand, RunningSessionInfo, AttendanceRecord, CompensatesSessionId, RecordedByUserId, ScannedLatitude, ScannedLongitude, ScannedVia (+29 more)

### Community 51 - "BaseEntity"
Cohesion: 0.08
Nodes (19): PlatformCommission, Amount, CollectionRequestedAtUtc, CollectionRequestedByUserId, GroupId, IsSettled, SessionId, SettledAtUtc (+11 more)

### Community 52 - "GroupSessionRepository"
Cohesion: 0.11
Nodes (15): ITeacherAccessRequestRepository, TeacherAccessRequest, CreatedUserId, DecisionNote, Email, ExpiresAtUtc, FullName, Note (+7 more)

### Community 54 - ".ResolveAsync"
Cohesion: 0.07
Nodes (25): PaymentEvent, CurrentStatus, EventType, PayloadSha256, PreviousStatus, Provider, ProviderEventId, ProviderOccurredAtUtc (+17 more)

### Community 55 - "TeacherERP.Application.Features.Users.Commands.Auth.Login"
Cohesion: 0.18
Nodes (4): TeacherERP.Application.Features.Users.Commands.Auth.GoogleSignIn, TeacherERP.Application.Features.Users.Commands.Auth.Login, TeacherERP.Application.Features.Users.Commands.Auth.ActivateAccount, TeacherERP.Application.Features.Users.Commands.Auth.RefreshToken

### Community 56 - ".GetByOwnerIdAsync"
Cohesion: 0.06
Nodes (27): AdminCenterDto, CenterTeacherLinkDto, CreateCenterCommand, CreateCenterCommandHandler, CenterDto, CenterInvitationToken, CenterLocationPolicy, CenterInvitationDto (+19 more)

### Community 57 - "SeededTeacher"
Cohesion: 0.26
Nodes (4): DynamicQrIntegrationTests, SeededTeacher, Scenario, SessionContentNotificationsIntegrationTests

### Community 59 - "TenantContext"
Cohesion: 0.23
Nodes (3): ISessionGenerator, TenantContext, SessionGenerationIntegrationTests

### Community 60 - "ICenterRepository"
Cohesion: 0.09
Nodes (23): ASPNETCORE_ENVIRONMENT, applicationUrl, commandName, dotnetRunMessages, environmentVariables, applicationUrl, commandName, dotnetRunMessages (+15 more)

### Community 61 - "ApproveTeacherRequestCommand"
Cohesion: 0.29
Nodes (8): ApproveTeacherRequestCommand, ApproveTeacherRequestCommandHandler, ApproveTeacherRequestValidator, PreviewTeacherRequestQuery, PreviewTeacherRequestQueryHandler, PreviewTeacherRequestValidator, RejectTeacherRequestValidator, TeacherRequestPreviewDto

### Community 62 - "CompleteProfileValidator"
Cohesion: 0.08
Nodes (24): GetGroupAdminRequestsQuery, GetGroupAdminRequestsQueryHandler, GroupAdminRequestDto, GroupAdminRequestItem, GroupAdminRequest, DecidedAt, DecidedByUserId, DecisionNote (+16 more)

### Community 63 - "SessionManager"
Cohesion: 0.10
Nodes (10): EnrollmentRequestDto, RequestGroupCommand, RequestGroupValidator, RequestGroupByCodeCommand, RequestGroupByCodeCommandHandler, RequestGroupByCodeValidator, IGroupJoinCodeService, GroupJoinCodeLookup (+2 more)

### Community 64 - "TeacherERP.Application.Features.Team"
Cohesion: 0.10
Nodes (14): CreateMyCenterValidator, CenterInvitation, AcceptedAtUtc, AcceptedByTenantId, CenterId, CreatedByUserId, ExpiresAtUtc, Status (+6 more)

### Community 65 - "TeacherERP.Application/DependencyInjection.cs"
Cohesion: 0.05
Nodes (13): TeacherERP.Application, TeacherERP.Application.Features.SessionContent.UploadSessionAttachment, TeacherERP.Application.Files, TeacherERP.Application.Features.SessionContent.DeleteSessionAttachment, TeacherERP.Application.Features.Profile, TeacherERP.Application.Features.Team.CreateAssistant, TeacherERP.Application.Features.Users.Commands.Auth.ResetPassword, TeacherERP.Application.GroupAdmins (+5 more)

### Community 66 - "SetFinancialDelegationCommandHandler.cs"
Cohesion: 0.15
Nodes (10): ForgotPasswordCommand, ForgotPasswordCommandHandler, ForgotPasswordResponseDto, ForgotPasswordValidator, IPasswordGenerator, IUserAccountService, TeacherProvisioningService, WelcomeEmailFailedException (+2 more)

### Community 67 - "RevokeSessionCommand"
Cohesion: 0.12
Nodes (3): ProductionDbSeeder, TestDataSeeder, CreateCenterCommandHandlerTests

### Community 69 - "HangfireBasicAuthFilter"
Cohesion: 0.08
Nodes (14): CenterJoinRequest, CenterId, DecisionNote, LastNotifiedAtUtc, ReviewedAtUtc, ReviewedByUserId, Status, TenantId (+6 more)

### Community 70 - "GetMySessionsQueryHandler"
Cohesion: 0.09
Nodes (20): PaymobAdjustmentReview, CommissionReversalAmount, Currency, DecisionNote, ProviderAmount, ProviderEventId, ReviewedAtUtc, ReviewedByUserId (+12 more)

### Community 73 - "GetAuditLogQuery"
Cohesion: 0.43
Nodes (4): AuditLogDto, GetAuditLogQuery, GetAuditLogQueryHandler, AuditLogItem

### Community 74 - "GetSessionsQuery"
Cohesion: 0.43
Nodes (3): GetSessionsQuery, SessionDto, SessionListItem

### Community 75 - "CachedRegistrationData"
Cohesion: 0.17
Nodes (9): FileNames, DetectedFileType, Jpeg, Pdf, Png, Unknown, Webp, FileSignature (+1 more)

### Community 76 - "GetMyNotificationsQuery"
Cohesion: 0.67
Nodes (3): GetMyNotificationsQuery, GetMyNotificationsQueryHandler, NotificationItem

### Community 77 - "UpdateGroupValidator"
Cohesion: 0.06
Nodes (22): TeacherPaymentSettings, InstaPayLink, QrImagePublicId, QrImageUrl, TenantId, EnrollmentStatus, Active, Ended (+14 more)

### Community 78 - ".Handle"
Cohesion: 0.19
Nodes (11): GetStudentLearningTimelineQuery, StudentTimelineAttachmentDto, StudentTimelineSessionDto, IStudentTimelineRepository, StudentTimelineSessionItem, AttendanceStatus, Absent, PendingCompensation (+3 more)

### Community 81 - "PageRequest"
Cohesion: 0.19
Nodes (5): PageRequest, Skip, PaymobAdjustmentReviewItem, PlatformCommissionItem, QueryablePagingExtensions

### Community 83 - "RescheduleSessionValidator.cs"
Cohesion: 0.14
Nodes (6): AddCenterMembershipRequests, SnapshotAccessor, AddStructuredGroupGrades, SnapshotAccessor, SnapshotAccessor, AppDbContextModelSnapshot

### Community 84 - "Error"
Cohesion: 0.05
Nodes (19): TeacherERP.Application.Features.Finance.GetMyPaymentOptions, TeacherERP.Application.Features.Finance.CreatePaymobIntent, TeacherERP.Application.Features.SessionContent, TeacherERP.Application.Finance, TeacherERP.Application.Features.Finance.SubmitInstaPayReceipt, AttendanceErrors, CenterErrors, EnrollmentErrors (+11 more)

### Community 85 - ".ApproveAsync"
Cohesion: 0.20
Nodes (8): TeacherRequestRules, ITeacherRequestReviewService, ReviewerKind, Administrator, Teacher, ProvisioningRejectedException, Error, TeacherRequestReviewService

### Community 86 - "CreateAssistantValidator.cs"
Cohesion: 0.23
Nodes (4): Scenario, WalkInLocationIntegrationTests, Scenario, TeacherPaymentsIntegrationTests

### Community 87 - "TeacherAccessRequestStatus"
Cohesion: 0.13
Nodes (11): GetTeacherRequestStatusQuery, GetTeacherRequestStatusQueryHandler, GetTeacherRequestStatusValidator, TeacherRequestStatusDto, TeacherRequestToken, TeacherAccessRequestStatus, Approved, Expired (+3 more)

### Community 89 - "BaseApiController"
Cohesion: 0.11
Nodes (11): MonthlyCollectionReportsController, PaymobAdjustmentReviewsController, ResolvePaymobAdjustmentBody, BaseApiController, PaymobReturnController, PaymobWebhookController, TeacherMonthlyCollectionReportsController, RolesController (+3 more)

### Community 90 - "GoogleSignInValidator.cs"
Cohesion: 0.13
Nodes (14): PaymobSettings, ApiKey, BaseUrl, Currency, HmacNullValue, HmacSecret, InitializationLeaseSeconds, IntegrationIds (+6 more)

### Community 91 - "ResetPasswordValidator.cs"
Cohesion: 0.22
Nodes (4): PaymobGatewayTests, RecordingLogger, Messages, StubHandler

### Community 92 - "SessionAttachment"
Cohesion: 0.08
Nodes (23): GetSessionAttachmentsQuery, SessionAttachmentDto, ISessionAttachmentRepository, SessionAttachmentItem, SessionAttachment, Caption, ContentType, FileName (+15 more)

### Community 94 - ".ResolveAsync"
Cohesion: 0.05
Nodes (17): CorsExtensions, ForwardedHeadersExtensions, RateLimitingExtensions, GlobalExceptionHandler, ApiAuthorizationResultHandler, CompletedProfileHandler, CompletedProfileRequirement, TenantHandler (+9 more)

### Community 95 - "ProfileAuthorizationResultHandler.cs"
Cohesion: 0.19
Nodes (3): Callback, PaymobErrors, PaymobPaymentService

### Community 98 - "CreatedTeacherDto"
Cohesion: 0.25
Nodes (9): CreatedTeacherDto, CreateTeacherCommand, CreateTeacherCommandHandler, CreateTeacherValidator, ApproveTeacherRequestByAdminCommand, ApproveTeacherRequestByAdminCommandHandler, ITeacherProvisioningService, CreateTeacherCommandHandlerTests (+1 more)

### Community 99 - ".GetByIdAsync"
Cohesion: 0.07
Nodes (31): ProfileCompletionChecker, GetStudentCardQuery, GetStudentCardQueryHandler, StudentCardDto, ResendWelcomeCommand, ResendWelcomeCommandHandler, WelcomeEmailFailedException, ActivateAccountCommand (+23 more)

### Community 101 - ".AddInfrastructureServices"
Cohesion: 0.06
Nodes (31): GoogleUserInfo, IGoogleAuthService, IUserTimeZoneAccessor, TimeZoneId, DependencyInjection, GoogleAuthService, CurrentTenantAccessor, Current (+23 more)

### Community 102 - "MoveStudentCommandHandler.cs"
Cohesion: 0.29
Nodes (3): TeacherERP.Application.Features.Enrollment.MoveStudent, MoveStudentValidator, MoveStudentTests

### Community 103 - "GroupScanValidator.cs"
Cohesion: 0.17
Nodes (13): OtpPurpose, Activation, PasswordReset, Registration, TeacherRequest, IssueCounter, Count, OtpEntry (+5 more)

### Community 104 - ".GetByIdAsync"
Cohesion: 0.16
Nodes (9): ApproveGroupAdminRequestCommand, GroupAdminChangeResultDto, RequestGroupAdminChangeCommand, RequestGroupAdminChangeValidator, GetTenantSettingsQuery, GetTenantSettingsQueryHandler, UpdateTenantProfileCommand, TenantSettingsDto (+1 more)

### Community 105 - "IAttendanceRepository"
Cohesion: 0.10
Nodes (24): AdminScanCommand, AdminScanCommandHandler, AdminScanValidator, AttendanceSettingsDto, GetAttendanceSettingsQuery, GetAttendanceSettingsQueryHandler, ManualAttendanceCommand, ManualAttendanceValidator (+16 more)

### Community 106 - "LogoutValidator.cs"
Cohesion: 0.44
Nodes (3): VerifyTeacherRequestCommand, VerifyTeacherRequestValidator, VerifyTeacherRequestCommandHandlerTests

### Community 108 - "IPagedRequest"
Cohesion: 0.22
Nodes (10): IPagedRequest, Page, PageSize, GetSessionAttendanceQuery, GetSessionAttendanceQueryHandler, MyEnrollmentsDto, GetMyEnrollmentsQuery, GetMyEnrollmentsQueryHandler (+2 more)

### Community 118 - "PlatformCommissionSettings"
Cohesion: 0.16
Nodes (8): IPlatformCommissionSettingsService, PlatformCommissionSettingsDto, PlatformCommissionSettingsService, PlatformCommissionSettings, MonthlySubscriptionAmount, PerSessionAmount, UpdatedAtUtc, UpdatedByUserId

### Community 119 - ".VerifyAsync"
Cohesion: 0.15
Nodes (8): TeacherERP.Application.Features.Users.Commands.Auth.VerifyResetOtp, ResetTokenDto, VerifyResetOtpCommand, VerifyResetOtpCommandHandler, VerifyResetOtpValidator, IResetGrantService, ResetGrant, ResetGrantService

### Community 123 - "IReadOnlyList"
Cohesion: 0.24
Nodes (8): StudentWeeklyGroupScheduleDto, StudentWeeklyScheduleDto, StudentWeeklyScheduleSlotDto, IStudentWeeklyScheduleRepository, StudentWeeklyScheduleData, StudentWeeklyScheduleGroupItem, StudentWeeklyScheduleSlotItem, StudentWeeklyScheduleRepository

### Community 124 - "CloudinarySettings"
Cohesion: 0.20
Nodes (8): IPaymobGateway, PaymobIntentCommandData, PaymobIntentData, PaymobGateway, TestPaymobGateway, CreateCount, FailNextWithCode, LastRequest

### Community 126 - ".TokenValidated"
Cohesion: 0.42
Nodes (6): CompleteProfileCommand, CompleteProfileCommandHandler, GetMyProfileQuery, GetMyProfileQueryHandler, GuardianDto, ProfileDto

### Community 128 - "PaymobWebhookOutcome"
Cohesion: 0.26
Nodes (8): GetMyGroupAdminQuery, GetMyGroupAdminQueryHandler, GroupAdminMeDto, GroupAccessLevel, GroupAdmin, Staff, IGroupAccessService, GroupAccessService

### Community 130 - "SubmitInstaPayReceiptForm"
Cohesion: 0.15
Nodes (12): UpdateTeacherPaymentSettingsForm, InstaPayLink, QrImage, SubmitInstaPayReceiptForm, Amount, Receipt, SessionId, Type (+4 more)

### Community 131 - "PaymentType"
Cohesion: 0.12
Nodes (17): CreatePaymobIntentBody, CreatePaymobIntentCommand, CreatePaymobIntentCommandValidator, CreatePaymobIntentCommandHandler, CreatePaymobPaymentRequest, IPaymobPaymentService, PaymobIntegrationKey, CaggOnlineNonShopify (+9 more)

### Community 137 - "GetMySessionsQueryHandler"
Cohesion: 0.50
Nodes (5): GetCurrentTenantQuery, GetCurrentTenantQueryHandler, GetMyTenantsQuery, GetMyTenantsQueryHandler, TenantDto

### Community 147 - "PaymobWebhookOutcome"
Cohesion: 0.33
Nodes (6): AttendanceOutcome, AlreadyRecorded, Corrected, PendingApproval, PendingCompensation, Recorded

### Community 148 - "IPasswordGenerator"
Cohesion: 0.33
Nodes (6): PaymobWebhookOutcome, Accepted, Duplicate, Ignored, InvalidPayload, InvalidSignature

## Knowledge Gaps
- **618 isolated node(s):** `TimeZoneId`, `Current`, `TenantId`, `SessionId`, `UserId` (+613 more)
  These have ≤1 connection - possible missing edges or undocumented components. (Counts symbols only; 735 node(s) total have ≤1 connection when file, concept and rationale nodes are included.)
- **20 thin communities (<3 nodes) omitted from report** — run `graphify query` to explore isolated nodes.

## Suggested Questions
_Questions this graph is uniquely positioned to answer:_

- **Why does `Result` connect `Result` to `PaymobWebhookOutcome`, `Task`, `PaymentType`, `ApplicationUser`, `.SaveChangesAsync`, `Guid`, `GetMySessionsQueryHandler`, `.Handle`, `EnrollmentRequestStatus`, `IFinancialLedgerRepository`, `PagedResult`, `AttendanceRecord`, `IRequestHandler`, `Center`, `IFinancialLedgerRepository.cs`, `LoginSession`, `MembershipStatus`, `.Handle`, `CreateAssistantCommandHandler`, `GroupDto`, `.AddApplicationServices`, `IOtpService`, `TeacherRequestReviewServiceTests`, `GetTeacherDashboardQueryHandler`, `Session`, `fluentvalidation`, `CancellationToken`, `EnrollmentRequest`, `.Create`, `.GetByOwnerIdAsync`, `ApproveTeacherRequestCommand`, `CompleteProfileValidator`, `SessionManager`, `SetFinancialDelegationCommandHandler.cs`, `GetAuditLogQuery`, `GetSessionsQuery`, `GetMyNotificationsQuery`, `.Handle`, `Error`, `.ApproveAsync`, `TeacherAccessRequestStatus`, `BaseApiController`, `SessionAttachment`, `CreatedTeacherDto`, `.GetByIdAsync`, `.AddInfrastructureServices`, `GroupScanValidator.cs`, `.GetByIdAsync`, `IAttendanceRepository`, `LogoutValidator.cs`, `IPagedRequest`, `PlatformCommissionSettings`, `.VerifyAsync`, `IReadOnlyList`, `CloudinarySettings`, `.TokenValidated`?**
  _High betweenness centrality (0.083) - this node is a cross-community bridge._
- **What connects `TimeZoneId`, `Current`, `TenantId` to the rest of the system?**
  _618 weakly-connected nodes found - possible documentation gaps or missing edges._
- **Should `TeacherERP.Domain.Shared` be split into smaller, more focused modules?**
  _Cohesion score 0.017033690946734424 - nodes in this community are weakly interconnected._
- **Why does `TeacherERP.Domain.Entities` connect `TeacherERP.Application.Interfaces` to `TeacherERP.Domain.Shared`, `IActionResult`, `Task`, `TeacherERP.Application.Scheduling`, `PaymentType`, `Session`, `system_data`, `EgyptianPhoneNumber.cs`, `.Handle`, `EnrollmentRequestStatus`, `IFinancialLedgerRepository`, `IGroupSessionRepository`, `.MarkAsUpdated`, `Center`, `Center`, `.Handle`, `IFinancialLedgerRepository.cs`, `LoginSession`, `MembershipStatus`, `microsoft_aspnetcore_authorization`, `.SaveChangesAsync`, `GroupDto`, `Session`, `fluentvalidation`, `BaseEntity`, `.ResolveAsync`, `TeacherERP.Application.Features.Users.Commands.Auth.Login`, `.GetByOwnerIdAsync`, `CompleteProfileValidator`, `TeacherERP.Application.Features.Team`, `TeacherERP.Application/DependencyInjection.cs`, `HangfireBasicAuthFilter`, `GetMySessionsQueryHandler`, `UpdateGroupValidator`, `.Handle`, `Error`, `TeacherAccessRequestStatus`, `BaseApiController`, `SessionAttachment`, `IAttendanceRepository`, `RequestGroupByCodeCommandHandler.cs`, `PlatformCommissionSettings`, `IReadOnlyList`, `.TokenValidated`?**
  _High betweenness centrality (0.050) - this node is a cross-community bridge._
- **Should `IActionResult` be split into smaller, more focused modules?**
  _Cohesion score 0.023991155334438918 - nodes in this community are weakly interconnected._
- **Why does `TeacherERP.Application.Interfaces` connect `TeacherERP.Application.Interfaces` to `TeacherERP.Domain.Shared`, `IActionResult`, `Task`, `TeacherERP.Application.Scheduling`, `PaymentType`, `Session`, `system_data`, `EgyptianPhoneNumber.cs`, `.Handle`, `IRequestHandler`, `Center`, `IFinancialLedgerRepository.cs`, `LoginSession`, `MembershipStatus`, `microsoft_aspnetcore_authorization`, `.AddApplicationServices`, `IOtpService`, `GetTeacherDashboardQueryHandler`, `AbstractValidator`, `TeacherERP.Application.Features.Users.Commands.Auth.Login`, `TenantContext`, `SessionManager`, `TeacherERP.Application/DependencyInjection.cs`, `SetFinancialDelegationCommandHandler.cs`, `Error`, `BaseApiController`, `.ResolveAsync`, `.AddInfrastructureServices`, `MoveStudentCommandHandler.cs`, `IAttendanceRepository`, `RequestGroupByCodeCommandHandler.cs`, `PlatformCommissionSettings`, `.VerifyAsync`, `IReadOnlyList`?**
  _High betweenness centrality (0.047) - this node is a cross-community bridge._
- **Should `Task` be split into smaller, more focused modules?**
  _Cohesion score 0.03994490358126722 - nodes in this community are weakly interconnected._