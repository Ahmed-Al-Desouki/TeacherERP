# Graph Report - TeacherERP  (2026-11-23)

## Corpus Check
- cluster-only mode — file stats not available

## Summary
- 4646 nodes · 16275 edges · 170 communities (122 shown, 48 thin omitted)
- Extraction: 92% EXTRACTED · 8% INFERRED · 0% AMBIGUOUS · INFERRED: 1306 edges (avg confidence: 0.85)
- Token cost: 0 input · 0 output

## Graph Freshness
- Built from commit: `f6c11617`
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
- RemoveAssistantValidator.cs
- Updatetenantprofilevalidator.cs
- ChangePasswordValidator.cs
- GoogleSignInValidator.cs
- RefreshTokenValidator.cs
- CreateStudentValidator.cs
- ProfileAuthorizationResultHandler.cs
- IdentityService.cs
- MailSettings
- ForwardedHeadersExtensions.cs
- GetNotificationWhatsAppShareQuery
- SessionCleanupJob
- CloudinarySettings
- JwtSettings
- .TokenValidated
- .Meters
- PaymobWebhookOutcome
- MailSettings
- TeacherERP.Application.Features.Students
- HangfireBasicAuthFilter
- .Resolve
- PreviewTeacherRequestQuery
- GetMyEnrollmentsQuery
- GetMySessionsQueryHandler
- PaymobCallbackVerifier
- GetNotificationWhatsAppShareQuery
- GetAdministratorDashboardQueryHandler
- EgyptianPhoneNumber.cs
- GetTeacherDashboardQueryHandler
- VerifyResetOtpCommand
- AttendanceOutcome
- PaymobWebhookOutcome
- IPasswordGenerator
- 20261010165019_AddTenants.Designer.cs
- 20261016061231_AddCentersAndTeacherSubject.Designer.cs
- 20261019104707_AddEnrollments.Designer.cs
- 20261023145617_AddNotificationANDAttendanceSystem.Designer.cs
- 20261025064044_AlignRolesWithTeam.Designer.cs
- 20261028110215_AddAbsenceProcessing.Designer.cs
- 20261030111655_AddWalkInSourceSession.Designer.cs
- 20261119120434_AddPaymobPaymentWorkflow.Designer.cs
- 20261119160000_AddPlatformCommissions.Designer.cs
- 20261119161712_ConfigurePlatformCommissionSettings.Designer.cs
- 20261119162425_AddPlatformCommissionCollectionRequests.Designer.cs
- MailSettings
- HangfireBasicAuthFilter
- GetMySessionsQuery
- GetTeamQuery
- CloudinarySettings
- JwtSettings
- SessionJwtBearerEvents
- GeoDistance
- GoogleAuthSettings

## God Nodes (most connected - your core abstractions)
1. `TeacherERP.Application.Interfaces` - 314 edges
2. `Result` - 302 edges
3. `TeacherERP.Domain.Entities` - 253 edges
4. `TeacherERP.Domain.Shared` - 221 edges
5. `AppDbContext` - 81 edges
6. `ApplicationUser` - 80 edges
7. `Group` - 59 edges
8. `TeacherERP.Infrastructure.Migrations` - 58 edges
9. `Session` - 57 edges
10. `Transaction` - 57 edges

## Surprising Connections (you probably didn't know these)
- `WalkInApprovalTests` --calls--> `TenantContext`  [INFERRED]
  TeacherERP.Tests/Unit/Attendance/WalkInLocationTests.cs → TeacherERP.Application/Interfaces/Tenants/ICurrentTenantAccessor.cs
- `BulkCancelSessionsCommandHandlerTests` --calls--> `TenantContext`  [INFERRED]
  TeacherERP.Tests/Unit/Sessions/CancelSessionHandlersTests.cs → TeacherERP.Application/Interfaces/Tenants/ICurrentTenantAccessor.cs
- `CancelSessionCommandHandlerTests` --calls--> `TenantContext`  [INFERRED]
  TeacherERP.Tests/Unit/Sessions/CancelSessionHandlersTests.cs → TeacherERP.Application/Interfaces/Tenants/ICurrentTenantAccessor.cs
- `UnsuspendStudentCommand` --references--> `Result`  [EXTRACTED]
  TeacherERP.Application/Features/Students/UnsuspendStudent/UnsuspendStudentCommand.cs → TeacherERP.Domain/Shared/Result.cs
- `GetPendingCompensationsQueryHandler` --references--> `Result`  [EXTRACTED]
  TeacherERP.Application/Features/Attendance/GetPendingCompensations/GetPendingCompensationsQueryHandler.cs → TeacherERP.Domain/Shared/Result.cs

## Import Cycles
- None detected.

## Communities (170 total, 48 thin omitted)

### Community 0 - "TeacherERP.Domain.Shared"
Cohesion: 0.02
Nodes (58): TeacherERP.Application.Features.Users.Commands.Auth.SetPassword, TeacherERP.Application.Features.TeacherRequests.Submit, TeacherERP.Application.Features.Team.GetTeam, TeacherERP.Application.Features.Users.Commands.Auth.GoogleSignIn, TeacherERP.Application.Features.Enrollment.ApproveRequest, TeacherERP.Application.Features.TeacherRequests.Review, TeacherERP.Application.Features.Finance.GetPaymentOptions, TeacherERP.Application.Features.Finance.UpdateTeacherPaymentSettings (+50 more)

### Community 1 - "IActionResult"
Cohesion: 0.02
Nodes (34): TeacherERP.Application.Features.GroupAdmins.GetMyGroupAdmin, TeacherERP.Application.Features.Students.GetStudentDetails, TeacherERP.Application.Features.Students.GetWeeklySchedule, TeacherERP.Application.Features.SessionContent.UploadSessionAttachment, TeacherERP.Application.Features.Students.GetStudentCard, TeacherERP.Application.Features.Attendance.UpdateAttendanceSettings, TeacherERP.Application.Features.Dashboards, TeacherERP.Application.Features.Schedule (+26 more)

### Community 2 - "Task"
Cohesion: 0.03
Nodes (88): AdminScanCommand, AdminScanCommandHandler, AdminScanValidator, ApproveCompensationCommand, DisplayCodeDto, GetDisplayCodeQuery, GetDisplayCodeQueryHandler, RejectCompensationCommand (+80 more)

### Community 3 - "TeacherERP.Application.Interfaces"
Cohesion: 0.06
Nodes (10): TeacherERP.Application.Scheduling, TeacherERP.Application.Absence, TeacherERP.Application.Features.Audit, TeacherERP.Domain.Entities, TeacherERP.Application.Interfaces, TeacherERP.Tests.Integration, TeacherERP.Infrastructure.Repositories, IProfileCompletionChecker (+2 more)

### Community 4 - "system"
Cohesion: 0.04
Nodes (10): ICenterMembershipCleanupJob, ICenterMembershipRepository, FinancialSessionInfo, IFinancialLedgerRepository, IPlatformCommissionSettingsService, PlatformCommissionSettingsDto, CenterMembershipCleanupJob, PlatformCommissionSettingsService (+2 more)

### Community 5 - "TimeSpan"
Cohesion: 0.03
Nodes (16): TeacherERP.Application.Features.Centers.JoinRequests, TeacherERP.Application.Features.Groups, TeacherERP.Application.Features.Schedule.GetSessions, TeacherERP.Application.Features.Enrollment.GetMyEnrollments, TeacherERP.Application.Features.Groups.GetGroups, TeacherERP.Application.Features.Directory.SearchDirectory, TeacherERP.Application.Features.Finance.GetPendingInstaPayReceipts, TeacherERP.Application.Features.SessionContent.GetSessionAttachments (+8 more)

### Community 6 - "system_text"
Cohesion: 0.05
Nodes (18): GetGroupAdminsQuery, GetGroupAdminsQueryHandler, GroupAdminDto, IAbsenceRepository, IAbsenceService, FinancialTransactionInfo, PlatformCommissionSummary, INotificationRepository (+10 more)

### Community 7 - "Guid"
Cohesion: 0.09
Nodes (8): CenterMembershipIntegrationTests, TestData, FinanceAuthorizationIntegrationTests, GroupGradeIntegrationTests, ScheduleOperationsIntegrationTests, SessionMoveIntegrationTests, StudentExperienceIntegrationTests, StudentTransferIntegrationTests

### Community 8 - "CreateAssistantCommandHandler"
Cohesion: 0.06
Nodes (40): CenterTeacherLinkDto, LinkTeacherToCenterCommandHandler, ApproveCompensationCommandHandler, ManualAttendanceCommandHandler, RejectCompensationCommandHandler, ScanAttendanceCommandHandler, ScanAttendanceValidator, AcceptCenterInvitationCommandHandler (+32 more)

### Community 9 - ".AddApplicationServices"
Cohesion: 0.04
Nodes (11): TeacherERP.API.Middlewares, TeacherERP.Application, TeacherERP.Application.Features.TeacherRequests.Verify, TeacherERP.Application.Centers, TeacherERP.Infrastructure, TeacherERP.Application.Features.Users.Commands.Auth.ResetPassword, TeacherERP.Application.Sessions, TeacherERP.Application.Behaviors (+3 more)

### Community 10 - "Result"
Cohesion: 0.06
Nodes (53): TeacherERP.Application.Features.Directory, GetPendingCompensationsQuery, GetPendingCompensationsQueryHandler, GetPendingCompensationsValidator, GetSessionAttendanceQuery, GetSessionAttendanceQueryHandler, GetSessionAttendanceValidator, GetAuditLogQuery (+45 more)

### Community 11 - "Session"
Cohesion: 0.06
Nodes (15): ICenterJoinRequestNotifier, GroupAdminItem, GroupAdminRequestItem, IGroupAdminRepository, GroupJoinCodeLookup, IGroupRepository, ITeacherAccessRequestRepository, ITeamRepository (+7 more)

### Community 12 - "IRequestHandler"
Cohesion: 0.05
Nodes (23): ICredentialService, IExternalLoginService, IUserAccountService, IUserRepository, ApplicationUser, Attendances, DateOfBirth, FullName (+15 more)

### Community 13 - "List"
Cohesion: 0.04
Nodes (31): AppDbContext, AttendanceRecords, AuditLogs, CenterInvitations, CenterJoinRequests, Centers, CenterTeachers, CurrentTenantId (+23 more)

### Community 15 - ".AddInfrastructureServices"
Cohesion: 0.07
Nodes (12): TeacherERP.Infrastructure.Data, TeacherERP.Infrastructure.Migrations, ReplaceRefreshTokensWithLoginSessions, FixLoginSessionUserId, MoveCenterStateToMembership, TestSeeder, AddGroupAdminsAndSessionContent, AddGroupPricing (+4 more)

### Community 16 - "Group"
Cohesion: 0.06
Nodes (28): ProfileCompletionChecker, CachedTeacherRequestData, Email, FullName, Note, Subject, TimeZoneId, TeacherRequestCacheKey (+20 more)

### Community 17 - "ApplicationUser"
Cohesion: 0.07
Nodes (17): ICenterProvisioningService, ICenterRepository, ITeacherProvisioningService, ITenantRepository, CenterProvisioningService, TeacherProvisioningService, WelcomeEmailFailedException, Center (+9 more)

### Community 18 - "IGroupSessionRepository"
Cohesion: 0.08
Nodes (8): IAttendanceCodeService, AttendanceCodeService, AdministrationIntegrationTests, DynamicQrIntegrationTests, SeededTeacher, CreateTeacherValidatorTests, AttendanceCodeServiceTests, GroupPricingTests

### Community 19 - "TeacherERP.API"
Cohesion: 0.07
Nodes (15): AttendanceRequest, AttendanceResultDto, IAttendanceRecorder, EnrollmentRequestListItem, GroupJoinInfo, IEnrollmentRepository, MyEnrollmentItem, MyRequestItem (+7 more)

### Community 20 - "AttendanceRecord"
Cohesion: 0.10
Nodes (18): IAuditRepository, CashPaymentResult, IFinancialLedgerService, PaymobAdjustmentResolution, ResolvePaymobAdjustmentRequest, IPlatformCommissionService, PlatformCommissionCollectionRequestResult, PlatformCommissionSettlementResult (+10 more)

### Community 21 - "AppDbContext"
Cohesion: 0.07
Nodes (5): AuthController, PasswordController, CentersController, TeacherRequestsController, UsersController

### Community 22 - "Center"
Cohesion: 0.05
Nodes (29): AuditLogDto, MyEnrollmentDto, SessionDto, MoveSessionRequest, RescheduleSessionRequest, StudentListDto, AdministratorDashboardOperationalStats, DashboardUpcomingSession (+21 more)

### Community 23 - "LoginSession"
Cohesion: 0.09
Nodes (9): CancelSessionCommand, CancelSessionValidator, IGroupSessionRepository, ScheduleConflictItem, SessionListItem, SessionMoveBlockers, GroupSessionRepository, BulkCancelSessionsCommandHandlerTests (+1 more)

### Community 24 - ".Handle"
Cohesion: 0.05
Nodes (10): TeacherERP.Application.Features.Attendance.RejectCompensation, TeacherERP.Application.Enrollment, TeacherERP.Tests.Unit, TeacherERP.Application.Attendance, TeacherERP.Application.Features.Attendance.ScanAttendance, TeacherERP.Application.Features.Centers.UpdateCenter, TeacherERP.Application.Features.Enrollment.RequestGroupByCode, TeacherERP.Application.Teachers (+2 more)

### Community 26 - "AttendanceController.cs"
Cohesion: 0.06
Nodes (38): PagedRequestRules, AdminCenterDto, CreateCenterCommand, CreateCenterCommandHandler, CreateCenterValidator, ManualAttendanceCommand, ManualAttendanceValidator, EnrollmentRequestDto (+30 more)

### Community 27 - ".SeedAsync"
Cohesion: 0.06
Nodes (26): Session, AbsencesProcessedAt, Attendances, BoardImageUrl, CancelReason, EndTime, Group, GroupId (+18 more)

### Community 28 - "ICurrentTenantAccessor"
Cohesion: 0.09
Nodes (18): PaymobAdjustmentReviewsController, AuditLogItem, DirectoryCenterInfo, DirectoryRow, DirectorySearchPage, IDirectoryRepository, TeacherProfileData, MonthlyCollectionItem (+10 more)

### Community 29 - "IUserAccountService"
Cohesion: 0.09
Nodes (18): IFileStorage, StoredFile, IStoredFileCleanupJob, FinancialDebtItem, FinancialPaymentOptions, FinancialTransactionItem, StudentPaymentDetails, TeacherPaymentSettingsDto (+10 more)

### Community 30 - "Transaction"
Cohesion: 0.07
Nodes (28): StudentTimelineAttachmentDto, StudentTimelineSessionDto, GetStudentDetailsQuery, GetStudentDetailsQueryHandler, StudentActiveGroupDto, StudentDetailsDto, GetStudentWeeklyScheduleQuery, GetStudentWeeklyScheduleQueryHandler (+20 more)

### Community 31 - "GroupDto"
Cohesion: 0.05
Nodes (19): TeacherERP.Application.Features.Finance.GetMyPaymentOptions, TeacherERP.Application.Features.Finance.CreatePaymobIntent, TeacherERP.Application.Features.Notifications, TeacherERP.Application.Files, TeacherERP.Application.Features.SessionContent, TeacherERP.Application.Finance, TeacherERP.Application.Features.Finance.SubmitInstaPayReceipt, AdministrationErrors (+11 more)

### Community 32 - "PasswordController.cs"
Cohesion: 0.09
Nodes (7): PlatformCommissionsController, DashboardController, DirectoryController, TeacherPlatformCommissionsController, NotificationsController, StudentsController, TeachersController

### Community 33 - "MembershipStatus"
Cohesion: 0.09
Nodes (11): AccessTokenResult, IJwtProvider, ISessionValidator, ISessionManager, ISessionRepository, SessionManager, TokenHasher, AuditRepository (+3 more)

### Community 34 - "OtpService"
Cohesion: 0.09
Nodes (5): TeacherERP.API.Controllers, ResolvePaymobAdjustmentBody, RequestPlatformCommissionCollection, SettlePlatformCommissionRequest, UpdatePlatformCommissionSettingsRequest

### Community 35 - ".SaveChangesAsync"
Cohesion: 0.10
Nodes (5): UserTime, CreateTeacherValidator, IEmailService, EmailService, EmailTemplates

### Community 36 - "TeacherERP.Application.Features.Groups"
Cohesion: 0.07
Nodes (37): CloudinaryDotNet (1.29.3), coverlet.collector (6.0.4), FluentValidation (12.1.1), FluentValidation.DependencyInjectionExtensions (12.1.1), Google.Apis.Auth (1.77.0), Hangfire.AspNetCore (1.8.25), Hangfire.Core (1.8.25), Hangfire.Dashboard.BasicAuthorization (1.0.2) (+29 more)

### Community 37 - "GuardianRelation"
Cohesion: 0.09
Nodes (3): EnrollmentController, GroupAdminsController, TeamController

### Community 38 - ".GetByIdAsync"
Cohesion: 0.08
Nodes (20): Group, Capacity, CenterId, DynamicJoinEnabled, DynamicJoinSecret, EducationStage, GradeLevel, GradeNumber (+12 more)

### Community 39 - "ITenantRepository"
Cohesion: 0.06
Nodes (36): AuditAction, AttendanceCorrected, CashPaymentCollected, CompensationApproved, CompensationRejected, FinancialDelegationGranted, FinancialDelegationRevoked, GroupAdminAppointed (+28 more)

### Community 40 - "EnrollmentRequestStatus"
Cohesion: 0.08
Nodes (16): CenterMembershipPolicy, GroupSlotDto, GroupSlotInput, IGroupSchedule, Slots, TeacherRequestPolicy, ScheduleSlotCandidate, SessionPolicy (+8 more)

### Community 41 - "GroupEnrollment"
Cohesion: 0.10
Nodes (18): SubmitTeacherRequestCommand, SubmitTeacherRequestCommandHandler, TeacherRequestOtpDto, AssistantDto, CreateAssistantCommand, CreateAssistantCommandHandler, WelcomeEmailFailedException, CreateAssistantValidator (+10 more)

### Community 42 - "AbstractValidator"
Cohesion: 0.07
Nodes (23): Tenant, AutoApproveGroupAdminChanges, AutoApproveJoinRequests, IsListedInSearch, Name, OwnerId, Status, Subject (+15 more)

### Community 43 - "TeacherERP.Application.Features.Users.Commands.Auth.Login"
Cohesion: 0.07
Nodes (22): Transaction, Amount, IdempotencyKey, PaymentContextSessionId, ProcessedByAdminId, Provider, ProviderClientSecret, ProviderFailureCode (+14 more)

### Community 44 - "fluentvalidation"
Cohesion: 0.11
Nodes (10): CompensationStaffIntegrationTests, CustomWebApplicationFactory, FileStorage, PaymobGateway, PaymobSettings, DashboardIntegrationTests, GroupPricingIntegrationTests, PasswordIntegrationTests (+2 more)

### Community 45 - "AttendanceResultDto"
Cohesion: 0.07
Nodes (29): Notification, IsRead, Message, RecipientUserId, RelatedId, TenantId, Type, NotificationType (+21 more)

### Community 47 - ".Handle"
Cohesion: 0.09
Nodes (5): AddWalkInLocationToEnrollmentRequests, AddTeacherAccessRequests, AddCenterMembershipRequests, AddInstaPayReceiptWorkflow, AllowPlatformCommissionPerPayment

### Community 48 - "AuthResponseDto"
Cohesion: 0.08
Nodes (19): TeacherAccessRequestListItem, TeacherAccessRequest, CreatedUserId, DecisionNote, Email, ExpiresAtUtc, FullName, Note (+11 more)

### Community 49 - "Error"
Cohesion: 0.11
Nodes (18): LinkTeacherToCenterCommand, LinkTeacherToCenterValidator, CenterTeacher, Center, CenterId, Origin, Status, TenantId (+10 more)

### Community 51 - "BaseEntity"
Cohesion: 0.25
Nodes (4): Scenario, WalkInLocationIntegrationTests, Scenario, TeacherPaymentsIntegrationTests

### Community 52 - "GroupSessionRepository"
Cohesion: 0.12
Nodes (14): IAbsenceProcessor, ISessionGenerator, ICurrentTenantAccessor, Current, TenantId, TenantContext, TenantRole, Assistant (+6 more)

### Community 53 - "SessionValidator"
Cohesion: 0.14
Nodes (13): CashPaymentRequest, FinancialObligation, Amount, GroupId, GroupName, IsSettled, OutstandingAmount, PaidAmount (+5 more)

### Community 54 - ".ResolveAsync"
Cohesion: 0.10
Nodes (21): ISessionAttachmentRepository, SessionAttachmentItem, SessionAttachment, Caption, ContentType, FileName, Kind, PublicId (+13 more)

### Community 55 - "ResetGrantService"
Cohesion: 0.08
Nodes (24): EnrollmentRequest, DecidedAt, DecidedByUserId, DecisionNote, Group, GroupId, SourceLatitude, SourceLongitude (+16 more)

### Community 56 - ".HandleAsync"
Cohesion: 0.08
Nodes (25): PaymentEvent, CurrentStatus, EventType, PayloadSha256, PreviousStatus, Provider, ProviderEventId, ProviderOccurredAtUtc (+17 more)

### Community 57 - ".AddApiRateLimiting"
Cohesion: 0.09
Nodes (12): TeacherERP.Application.Features.Team, TeacherERP.Application.Features.Students, ApiAuthorizationResultHandler, GroupErrors, StudentErrors, StudentReasonRequest, TeamErrors, TeamPolicy (+4 more)

### Community 58 - "TenantRole"
Cohesion: 0.14
Nodes (10): TeacherERP.Infrastructure.Settings, TeacherERP.Application.Features.Users.Commands.Auth.ForgotPassword, CachedRegistrationData, Email, FullName, Password, StudentStatus, Active (+2 more)

### Community 59 - "IServiceCollection"
Cohesion: 0.13
Nodes (6): AttendanceListItem, AttendanceSettingsInfo, IAttendanceRepository, PendingCompensationItem, RunningSessionInfo, AttendanceRepository

### Community 60 - "ICenterRepository"
Cohesion: 0.13
Nodes (15): CenterDto, CenterInvitationToken, CenterInvitationDto, CreateMyCenterCommand, CreateMyCenterCommandHandler, GetMyCentersQuery, GetMyCentersQueryHandler, AcceptCenterInvitationCommand (+7 more)

### Community 61 - ".ValidateIdTokenAsync"
Cohesion: 0.08
Nodes (22): AttendanceRecord, CompensatesSessionId, RecordedByUserId, ScannedLatitude, ScannedLongitude, ScannedVia, Session, SessionId (+14 more)

### Community 62 - "CompleteProfileValidator"
Cohesion: 0.08
Nodes (21): LoginSession, AbsoluteExpiresAt, DeviceId, DeviceName, LastRefreshedAt, PreviousRefreshTokenHash, RefreshExpiresAt, RefreshTokenHash (+13 more)

### Community 63 - "SessionManager"
Cohesion: 0.09
Nodes (20): PaymobAdjustmentReview, CommissionReversalAmount, Currency, DecisionNote, ProviderAmount, ProviderEventId, ReviewedAtUtc, ReviewedByUserId (+12 more)

### Community 64 - "TeacherERP.Application.Features.Team"
Cohesion: 0.09
Nodes (19): GroupAdminRequest, DecidedAt, DecidedByUserId, DecisionNote, Group, GroupId, RequestedByUserId, Status (+11 more)

### Community 66 - "SetFinancialDelegationCommandHandler.cs"
Cohesion: 0.11
Nodes (9): MonthlyCollectionReportsController, AuditController, StudentPaymentsController, TeacherMonthlyCollectionReportsController, SessionsController, SessionAttachmentsController, StudentTimelineController, ProfileController (+1 more)

### Community 67 - "RevokeSessionCommand"
Cohesion: 0.09
Nodes (23): ASPNETCORE_ENVIRONMENT, applicationUrl, commandName, dotnetRunMessages, environmentVariables, applicationUrl, commandName, dotnetRunMessages (+15 more)

### Community 68 - "CancelSessionCommandHandler.cs"
Cohesion: 0.09
Nodes (15): MonthlySubscription, Amount, GroupId, Month, PaymentTransactionId, StudentId, TenantId, Year (+7 more)

### Community 69 - "HangfireBasicAuthFilter"
Cohesion: 0.17
Nodes (3): IGroupJoinCodeService, GroupJoinCodeService, GroupJoinCodeServiceTests

### Community 70 - "GetMySessionsQueryHandler"
Cohesion: 0.10
Nodes (10): IAbsenceProcessingJob, IGoogleAuthService, ISessionGenerationJob, ISessionCleanupJob, AbsenceProcessingJob, SessionGenerationJob, SessionCleanupJob, ProductionDbSeeder (+2 more)

### Community 71 - "Centerdto.cs"
Cohesion: 0.14
Nodes (12): GetSessionAttachmentsQuery, GetSessionAttachmentsQueryHandler, GetSessionAttachmentsValidator, SessionAttachmentDto, UploadSessionAttachmentCommand, UploadSessionAttachmentCommandHandler, UploadSessionAttachmentValidator, GetStudentCardQuery (+4 more)

### Community 72 - "TeacherERP.Application.Features.Students"
Cohesion: 0.10
Nodes (14): CreateMyCenterValidator, CenterInvitation, AcceptedAtUtc, AcceptedByTenantId, CenterId, CreatedByUserId, ExpiresAtUtc, Status (+6 more)

### Community 73 - "LoggingBehavior.cs"
Cohesion: 0.09
Nodes (19): PlatformCommission, Amount, CollectionRequestedAtUtc, CollectionRequestedByUserId, GroupId, IsSettled, SessionId, SettledAtUtc (+11 more)

### Community 74 - "GeoDistance"
Cohesion: 0.16
Nodes (4): CreatePaymobPaymentRequest, IPaymobPaymentService, Callback, PaymobPaymentService

### Community 75 - "CachedRegistrationData"
Cohesion: 0.13
Nodes (5): SnapshotAccessor, AddStructuredGroupGrades, SnapshotAccessor, AddSessionOriginalStartTime, SnapshotAccessor

### Community 76 - ".TryHandleAsync"
Cohesion: 0.14
Nodes (8): TeacherERP.API.Security, AuthPolicies, CompletedProfileHandler, CompletedProfileRequirement, TenantHandler, TenantRequirement, AllowedRoles, RequiresFinancialDelegation

### Community 77 - "UpdateGroupValidator"
Cohesion: 0.16
Nodes (12): TeacherERP.Application.Features.TeacherRequests, PreviewTeacherRequestQuery, PreviewTeacherRequestQueryHandler, GetTeacherRequestStatusQuery, GetTeacherRequestStatusQueryHandler, GetTeacherRequestStatusValidator, TeacherRequestPreviewDto, TeacherRequestQrDto (+4 more)

### Community 78 - ".RequiresProfileCompletion"
Cohesion: 0.10
Nodes (16): IPaymobCallbackVerifier, PaymobSettings, ApiKey, BaseUrl, Currency, HmacNullValue, HmacSecret, InitializationLeaseSeconds (+8 more)

### Community 80 - "AddExtraSessionValidator.cs"
Cohesion: 0.10
Nodes (4): TeacherERP.Application.Features.Attendance.ManualAttendance, TeacherERP.Application.Features.Enrollment, DependencyInjection, EnrollmentErrors

### Community 81 - "BulkCancelSessionsValidator.cs"
Cohesion: 0.18
Nodes (12): CreateGroupCommand, GetGroupQuery, GetGroupQueryHandler, GetGroupsQuery, GetGroupsQueryHandler, GetGroupsValidator, GroupDto, IGroupPricing (+4 more)

### Community 82 - "BulkShiftSessionsValidator.cs"
Cohesion: 0.19
Nodes (13): ActivateAccountCommand, ActivateAccountCommandHandler, ActivateAccountValidator, GoogleSignInCommand, GoogleSignInCommandHandler, GoogleSignInValidator, AuthResponseDto, LoginCommand (+5 more)

### Community 83 - "RescheduleSessionValidator.cs"
Cohesion: 0.11
Nodes (15): EnrollmentStatus, Active, Ended, GroupEnrollment, ApprovedByUserId, EndedAt, EnrolledAt, Group (+7 more)

### Community 84 - "BlacklistStudentValidator.cs"
Cohesion: 0.14
Nodes (3): UpdateGroupScheduleRequest, SessionTimeCalculator, AddStudentProfile

### Community 87 - "RemoveAssistantValidator.cs"
Cohesion: 0.12
Nodes (8): UpdateTeacherPaymentSettingsForm, InstaPayLink, QrImage, SubmitInstaPayReceiptForm, Amount, Receipt, SessionId, Type

### Community 89 - "SetFinancialDelegationValidator.cs"
Cohesion: 0.17
Nodes (9): FileNames, DetectedFileType, Jpeg, Pdf, Png, Unknown, Webp, FileSignature (+1 more)

### Community 90 - "GoogleSignInValidator.cs"
Cohesion: 0.12
Nodes (14): GuardianInfo, Name, PhoneNumber, Relation, GuardianRelation, Aunt, Brother, Father (+6 more)

### Community 91 - "ResetPasswordValidator.cs"
Cohesion: 0.16
Nodes (8): TeacherERP.Application.Features.Team.ResendWelcome, GlobalExceptionHandler, ResendWelcomeCommand, ResendWelcomeCommandHandler, WelcomeEmailFailedException, ResendWelcomeValidator, ProvisioningRejectedException, Error

### Community 92 - "VerifyResetOtpValidator.cs"
Cohesion: 0.16
Nodes (4): LoggingBehavior, ValidationBehavior, CenterLocationPolicy, ProfileRequirement

### Community 94 - "VerifyOtpValidator.cs"
Cohesion: 0.18
Nodes (10): IPagedRequest, Page, PageSize, PagedResult, HasNext, HasPrevious, TotalPages, GetGroupAdminRequestsQuery (+2 more)

### Community 96 - "IdentityService.cs"
Cohesion: 0.19
Nodes (5): TeacherERP.Application.Features.Attendance, TeacherERP.Application.Features.GroupAdmins.GetGroupAdminRequests, TeacherERP.Application.Common, TeacherERP.Application.Features.GroupAdmins.GetGroupSessionAttendance, AttendanceErrors

### Community 97 - "TeacherERP.Application.Features.Schedule.GetSessions"
Cohesion: 0.15
Nodes (3): TeacherERP.Application.Features.Centers, TeacherERP.Application.Features.Centers.Invitations, CenterErrors

### Community 98 - "UpdateGroupValidator"
Cohesion: 0.25
Nodes (8): AttendanceSettingsDto, GetAttendanceSettingsQuery, GetAttendanceSettingsQueryHandler, RegenerateStaticCodeCommand, RegenerateStaticCodeCommandHandler, UpdateAttendanceSettingsCommand, UpdateAttendanceSettingsCommandHandler, UpdateAttendanceSettingsValidator

### Community 99 - "ManualAttendanceValidator.cs"
Cohesion: 0.14
Nodes (10): ICurrentUserService, SessionId, UserId, IUserTimeZoneAccessor, TimeZoneId, CurrentUserService, SessionId, UserId (+2 more)

### Community 101 - "RejectRequestValidator.cs"
Cohesion: 0.21
Nodes (4): TeacherERP.Application.Features.Profile.CompleteProfile, TeacherERP.Application.Features.Groups.UpdateGroup, TeacherERP.Application.Features.Profile, TeacherERP.Domain.ValueObjects

### Community 102 - "RequestGroupValidator.cs"
Cohesion: 0.17
Nodes (8): UpdateGroupRequest, EducationStage, Preparatory, Primary, Secondary, Unspecified, SchoolGrade, SchoolGradeTests

### Community 103 - "GroupScanValidator.cs"
Cohesion: 0.22
Nodes (4): PaymobGatewayTests, RecordingLogger, Messages, StubHandler

### Community 104 - "RequestGroupAdminChangeValidator.cs"
Cohesion: 0.31
Nodes (8): CreatedTeacherDto, CreateTeacherCommand, CreateTeacherCommandHandler, ApproveTeacherRequestByAdminCommand, ApproveTeacherRequestByAdminCommandHandler, ApproveTeacherRequestCommand, ApproveTeacherRequestCommandHandler, CreateTeacherCommandHandlerTests

### Community 105 - "CancelSessionValidator.cs"
Cohesion: 0.17
Nodes (9): PlatformCommissionSettings, MonthlySubscriptionAmount, PerSessionAmount, UpdatedAtUtc, UpdatedByUserId, BaseEntity, CreatedAt, Id (+1 more)

### Community 107 - "BlacklistStudentValidator.cs"
Cohesion: 0.18
Nodes (4): TeacherERP.API.Extensions, CorsExtensions, ForwardedHeadersExtensions, RateLimitPolicies

### Community 111 - "Updatetenantprofilevalidator.cs"
Cohesion: 0.35
Nodes (7): CompleteProfileCommand, CompleteProfileCommandHandler, CompleteProfileValidator, GetMyProfileQuery, GetMyProfileQueryHandler, GuardianDto, ProfileDto

### Community 112 - "ChangePasswordValidator.cs"
Cohesion: 0.27
Nodes (3): ManualAttendanceIntegrationTests, Scenario, StudentLearningTimelineIntegrationTests

### Community 116 - "ProfileAuthorizationResultHandler.cs"
Cohesion: 0.33
Nodes (7): IPaymobGateway, PaymobIntentCommandData, PaymobIntentData, TestPaymobGateway, CreateCount, FailNextWithCode, LastRequest

### Community 117 - "IdentityService.cs"
Cohesion: 0.25
Nodes (8): CreatePaymobIntentBody, PaymobIntegrationKey, CaggOnlineNonShopify, CashCashChannel, UigOnlineNewNonShopify, VpcOnlineNonShopify, VpcShopify, PaymobPaymentIntentResult

### Community 119 - "MailSettings"
Cohesion: 0.22
Nodes (4): UploadSessionAttachmentForm, Caption, File, Kind

### Community 121 - "ForwardedHeadersExtensions.cs"
Cohesion: 0.44
Nodes (6): GetGroupJoinCodesQuery, GetGroupJoinCodesQueryHandler, GroupJoinCodesDto, UpdateGroupJoinCodesCommand, UpdateGroupJoinCodesCommandHandler, UpdateGroupJoinCodesValidator

### Community 122 - "GetNotificationWhatsAppShareQuery"
Cohesion: 0.33
Nodes (5): GroupAccessLevel, GroupAdmin, Staff, IGroupAccessService, GroupAccessService

### Community 123 - "SessionCleanupJob"
Cohesion: 0.25
Nodes (4): BaseApiController, PaymobReturnController, PaymobWebhookController, RolesController

### Community 124 - "CloudinarySettings"
Cohesion: 0.29
Nodes (5): TeacherERP.Application.Features.Schedule.BulkCancelSessions, BulkCancelSessionsCommand, BulkCancelSessionsValidator, BulkResultDto, SessionTimeDto

### Community 125 - "JwtSettings"
Cohesion: 0.25
Nodes (4): TeacherERP.Infrastructure.Extensions, PageRequest, Skip, QueryablePagingExtensions

### Community 126 - ".TokenValidated"
Cohesion: 0.36
Nodes (5): TeacherERP.Application.Features.GroupAdmins, GetMyGroupAdminQuery, GetMyGroupAdminQueryHandler, GroupAdminMeDto, GroupAdminErrors

### Community 127 - ".Meters"
Cohesion: 0.50
Nodes (5): GetTenantSettingsQuery, GetTenantSettingsQueryHandler, TenantSettingsDto, UpdateTenantSettingsCommand, UpdateTenantSettingsCommandHandler

### Community 129 - "MailSettings"
Cohesion: 0.32
Nodes (4): MoveStudentCommand, MoveStudentCommandHandler, MoveStudentValidator, MoveStudentTests

### Community 130 - "TeacherERP.Application.Features.Students"
Cohesion: 0.50
Nodes (5): GetCurrentTenantQuery, GetCurrentTenantQueryHandler, GetMyTenantsQuery, GetMyTenantsQueryHandler, TenantDto

### Community 131 - "HangfireBasicAuthFilter"
Cohesion: 0.38
Nodes (4): TeacherERP.Application.Features.Team.RemoveAssistant, RemoveAssistantCommand, RemoveAssistantCommandHandler, RemoveAssistantValidator

### Community 132 - ".Resolve"
Cohesion: 0.52
Nodes (3): RequestGroupByCodeCommand, RequestGroupByCodeCommandHandler, RequestGroupByCodeValidator

### Community 139 - "GetNotificationWhatsAppShareQuery"
Cohesion: 0.60
Nodes (4): TeacherERP.Application.Features.Notifications.GetWhatsAppShare, GetNotificationWhatsAppShareQuery, NotificationWhatsAppShareDto, GetNotificationWhatsAppShareQueryHandler

### Community 140 - "GetAdministratorDashboardQueryHandler"
Cohesion: 0.53
Nodes (3): AdministratorDashboardDto, GetAdministratorDashboardQuery, GetAdministratorDashboardQueryHandler

### Community 144 - "GetTeacherDashboardQueryHandler"
Cohesion: 0.67
Nodes (3): TeacherDashboardDto, GetTeacherDashboardQuery, GetTeacherDashboardQueryHandler

### Community 145 - "VerifyResetOtpCommand"
Cohesion: 0.53
Nodes (4): ResetTokenDto, VerifyResetOtpCommand, VerifyResetOtpCommandHandler, VerifyResetOtpValidator

### Community 146 - "AttendanceOutcome"
Cohesion: 0.33
Nodes (6): AttendanceOutcome, AlreadyRecorded, Corrected, PendingApproval, PendingCompensation, Recorded

### Community 147 - "PaymobWebhookOutcome"
Cohesion: 0.33
Nodes (6): PaymobWebhookOutcome, Accepted, Duplicate, Ignored, InvalidPayload, InvalidSignature

### Community 160 - "MailSettings"
Cohesion: 0.33
Nodes (6): MailSettings, DisplayName, Host, Mail, Password, Port

### Community 163 - "GetMySessionsQuery"
Cohesion: 0.70
Nodes (3): GetMySessionsQuery, GetMySessionsQueryHandler, SessionDto

### Community 164 - "GetTeamQuery"
Cohesion: 0.70
Nodes (3): GetTeamQuery, GetTeamQueryHandler, TeamMemberDto

### Community 165 - "CloudinarySettings"
Cohesion: 0.40
Nodes (4): CloudinarySettings, ApiKey, ApiSecret, CloudName

### Community 166 - "JwtSettings"
Cohesion: 0.40
Nodes (5): JwtSettings, Audience, ExpiryMinutes, Issuer, Secret

## Knowledge Gaps
- **609 isolated node(s):** `Preparatory`, `Primary`, `Secondary`, `Unspecified`, `Messages` (+604 more)
  These have ≤1 connection - possible missing edges. (Counts symbols only; 1017 node(s) total have ≤1 connection when file, concept and rationale nodes are included.)
- **48 thin communities (<3 nodes) omitted from report** — run `graphify query` to explore isolated nodes.

## Suggested Questions
_Questions this graph is uniquely positioned to answer:_

- **Why does `TeacherERP.Domain.Entities` connect `TeacherERP.Application.Interfaces` to `TeacherERP.Domain.Shared`, `IActionResult`, `TimeSpan`, `.AddApplicationServices`, `PaymobCallbackVerifier`, `GetMySessionsQueryHandler`, `ApplicationUser`, `TeacherERP.API`, `Center`, `LoginSession`, `.Handle`, `ICurrentTenantAccessor`, `IUserAccountService`, `Transaction`, `GroupDto`, `ITenantRepository`, `EnrollmentRequestStatus`, `AbstractValidator`, `AttendanceResultDto`, `TenantSettings`, `AuthResponseDto`, `Error`, `GroupSessionRepository`, `SessionValidator`, `.ResolveAsync`, `ResetGrantService`, `.HandleAsync`, `TenantRole`, `IServiceCollection`, `ICenterRepository`, `.ValidateIdTokenAsync`, `CompleteProfileValidator`, `SessionManager`, `TeacherERP.Application.Features.Team`, `CancelSessionCommandHandler.cs`, `TeacherERP.Application.Features.Students`, `LoggingBehavior.cs`, `.TryHandleAsync`, `UpdateGroupValidator`, `AddExtraSessionValidator.cs`, `RescheduleSessionValidator.cs`, `IdentityService.cs`, `TeacherERP.Application.Features.Schedule.GetSessions`, `RejectRequestValidator.cs`, `CancelSessionValidator.cs`, `ResendWelcomeValidator.cs`, `ProfileAuthorizationResultHandler.cs`, `AttendanceOutcome`, `MailSettings`, `HangfireBasicAuthFilter`, `CloudinarySettings`, `.TokenValidated`?**
  _High betweenness centrality (0.079) - this node is a cross-community bridge._
- **What connects `Preparatory`, `Primary`, `Secondary` to the rest of the system?**
  _609 weakly-connected nodes found - possible documentation gaps or missing edges._
- **Should `TeacherERP.Domain.Shared` be split into smaller, more focused modules?**
  _Cohesion score 0.018651362984218076 - nodes in this community are weakly interconnected._
- **Why does `TeacherERP.Application.Interfaces` connect `TeacherERP.Application.Interfaces` to `TeacherERP.Domain.Shared`, `IActionResult`, `system`, `TimeSpan`, `.AddApplicationServices`, `PaymobCallbackVerifier`, `GetMySessionsQueryHandler`, `Group`, `TeacherERP.API`, `IPasswordGenerator`, `Center`, `LoginSession`, `.Handle`, `ICurrentTenantAccessor`, `IUserAccountService`, `Transaction`, `GroupDto`, `MembershipStatus`, `OtpService`, `.SaveChangesAsync`, `GroupSessionRepository`, `TenantRole`, `IServiceCollection`, `HangfireBasicAuthFilter`, `GetMySessionsQueryHandler`, `.TryHandleAsync`, `AddExtraSessionValidator.cs`, `IdentityService.cs`, `TeacherERP.Application.Features.Schedule.GetSessions`, `ManualAttendanceValidator.cs`, `RejectRequestValidator.cs`, `ResendWelcomeValidator.cs`, `ProfileAuthorizationResultHandler.cs`, `.TokenValidated`?**
  _High betweenness centrality (0.064) - this node is a cross-community bridge._
- **Should `IActionResult` be split into smaller, more focused modules?**
  _Cohesion score 0.02312992125984252 - nodes in this community are weakly interconnected._
- **Why does `AppDbContext` connect `List` to `TeacherERP.Application.Interfaces`, `system_text`, `Centerdto.cs`, `CreateAssistantCommandHandler`, `Guid`, `GetMySessionsQueryHandler`, `CachedRegistrationData`, `CreateAssistantValidator.cs`, `Center`?**
  _High betweenness centrality (0.044) - this node is a cross-community bridge._
- **Should `Task` be split into smaller, more focused modules?**
  _Cohesion score 0.032758866258367755 - nodes in this community are weakly interconnected._