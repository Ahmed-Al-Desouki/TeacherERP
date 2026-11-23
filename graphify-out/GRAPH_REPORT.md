# Graph Report - TeacherERP  (2026-11-23)

## Corpus Check
- cluster-only mode — file stats not available

## Summary
- 4404 nodes · 16523 edges · 137 communities (115 shown, 22 thin omitted)
- Extraction: 90% EXTRACTED · 10% INFERRED · 0% AMBIGUOUS · INFERRED: 1597 edges (avg confidence: 0.85)
- Token cost: 0 input · 0 output

## Graph Freshness
- Built from commit: `6f8cfce7`
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
- Center
- LoginSession
- .Handle
- http
- AttendanceController.cs
- .SeedAsync
- ICurrentTenantAccessor
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
- PaymobWebhookOutcome
- MailSettings
- TeacherERP.Application.Features.Students
- HangfireBasicAuthFilter
- .Resolve
- PreviewTeacherRequestQuery
- .ValidateIdTokenAsync
- GetMyEnrollmentsQuery
- GetMyNotificationsQuery

## God Nodes (most connected - your core abstractions)
1. `Result` - 387 edges
2. `TeacherERP.Application.Interfaces` - 297 edges
3. `TeacherERP.Domain.Shared` - 264 edges
4. `TeacherERP.Domain.Entities` - 236 edges
5. `AppDbContext` - 110 edges
6. `ApplicationUser` - 88 edges
7. `TeacherERP.Infrastructure.Data` - 58 edges
8. `TeacherERP.Infrastructure.Migrations` - 56 edges
9. `Session` - 49 edges
10. `Transaction` - 49 edges

## Surprising Connections (you probably didn't know these)
- `CustomWebApplicationFactory` --calls--> `AppDbContext`  [INFERRED]
  TeacherERP.Tests/Integration/CustomWebApplicationFactory.cs → TeacherERP.Infrastructure/Data/AppDbContext.cs
- `BulkCancelSessionsCommandHandlerTests` --calls--> `TenantContext`  [INFERRED]
  TeacherERP.Tests/Unit/CancelSessionHandlersTests.cs → TeacherERP.Application/Interfaces/Icurrenttenantaccessor.cs
- `CancelSessionCommandHandlerTests` --calls--> `TenantContext`  [INFERRED]
  TeacherERP.Tests/Unit/CancelSessionHandlersTests.cs → TeacherERP.Application/Interfaces/Icurrenttenantaccessor.cs
- `SubmitTeacherRequestCommandHandlerTests` --calls--> `OtpIssueResult`  [INFERRED]
  TeacherERP.Tests/Unit/TeacherRequestTests.cs → TeacherERP.Application/Interfaces/IOtpService.cs
- `SubmitInstaPayReceiptForm` --references--> `PaymentType`  [EXTRACTED]
  TeacherERP.API/Controllers/StudentPaymentsController.cs → TeacherERP.Domain/Entities/Transaction.cs

## Import Cycles
- None detected.

## Communities (137 total, 22 thin omitted)

### Community 0 - "TeacherERP.Domain.Shared"
Cohesion: 0.02
Nodes (38): TeacherERP.Application.Features.Team.GetTeam, TeacherERP.Application.Features.Finance.GetPaymentOptions, TeacherERP.Application.Features.Centers.JoinRequests, TeacherERP.Application.Features.Finance.UpdateTeacherPaymentSettings, TeacherERP.Application.Features.Finance.ApproveInstaPayReceipt, TeacherERP.Application.Features.Profile.GetMyProfile, TeacherERP.Application.Features.Groups.GetGroup, TeacherERP.Application.Features.Users.Commands.Auth.ChangePassword (+30 more)

### Community 1 - "IActionResult"
Cohesion: 0.05
Nodes (24): AdministrationController, AttendanceController, AuditController, AuthController, BaseApiController, CentersController, DirectoryController, EnrollmentController (+16 more)

### Community 2 - "Task"
Cohesion: 0.03
Nodes (20): MoveStudentRequest, LinkTeacherToCenterRequest, UpdateGroupRequest, MoveSessionRequest, AttendanceSettingsInfo, GroupJoinInfo, MyEnrollmentItem, MyRequestItem (+12 more)

### Community 3 - "TeacherERP.Application.Interfaces"
Cohesion: 0.03
Nodes (104): TeacherERP.Application.Features.Notifications.GetWhatsAppShare, PagedRequestRules, AdminScanValidator, ApproveCompensationCommand, GetSessionAttendanceValidator, RejectCompensationCommand, ScanAttendanceCommand, ScanAttendanceValidator (+96 more)

### Community 4 - "system"
Cohesion: 0.04
Nodes (13): TeacherERP.Infrastructure.Settings, TeacherERP.Application.Features.Users.Commands.VerifyOtp, TeacherERP.Infrastructure, TeacherERP.Application.Finance, TeacherERP.Application.Features.Users.Commands.Auth.ForgotPassword, TeacherERP.Application.Features.Users.Commands.CreateStudent, TeacherERP.Tests.Integration, TeacherERP.Infrastructure.Services (+5 more)

### Community 5 - "TimeSpan"
Cohesion: 0.04
Nodes (20): TeacherERP.Application.Features.Users.Commands.Auth.SetPassword, TeacherERP.Application.Features.Finance.CreatePaymobIntent, TeacherERP.Application.Features.Enrollment.ApproveRequest, TeacherERP.Application.Attendance, TeacherERP.Application.Features.Attendance.ScanAttendance, TeacherERP.Application.Features.Centers.CreateCenter, TeacherERP.Application.Features.Administration.CreateCenter, TeacherERP.Application.Features.Audit (+12 more)

### Community 6 - "system_text"
Cohesion: 0.03
Nodes (30): TeacherERP.Application.Features.Attendance, TeacherERP.Application.Features.GroupAdmins.GetGroupAdminRequests, TeacherERP.Application.Features.GroupAdmins.GetMyGroupAdmin, TeacherERP.Application.Features.Students.GetStudentDetails, TeacherERP.Application.Features.Students.GetWeeklySchedule, TeacherERP.Application.Features.Groups, TeacherERP.Application.Features.SessionContent.UploadSessionAttachment, TeacherERP.Application.Features.Students.GetStudentCard (+22 more)

### Community 7 - "Guid"
Cohesion: 0.04
Nodes (23): InitialCreate, FixLoginSessionUserId, AddTenants, MoveCenterStateToMembership, AddCentersAndTeacherSubject, TestSeeder, AddNotificationANDAttendanceSystem, AlignRolesWithTeam (+15 more)

### Community 8 - "CreateAssistantCommandHandler"
Cohesion: 0.09
Nodes (8): CenterMembershipIntegrationTests, FinanceAuthorizationIntegrationTests, GroupPricingIntegrationTests, ScheduleOperationsIntegrationTests, SessionMoveIntegrationTests, StudentExperienceIntegrationTests, StudentTransferIntegrationTests, TestData

### Community 9 - ".AddApplicationServices"
Cohesion: 0.07
Nodes (39): AbsenceService, ApproveCompensationCommandHandler, RejectCompensationCommandHandler, RejectRequestCommandHandler, ApproveGroupAdminRequestCommand, ApproveGroupAdminRequestCommandHandler, GetGroupAdminsQuery, GetGroupAdminsQueryHandler (+31 more)

### Community 10 - "Result"
Cohesion: 0.03
Nodes (26): TeacherERP.Application.Features.Finance.GetMyPaymentOptions, TeacherERP.Application.Features.Users.Commands.Auth.GoogleSignIn, TeacherERP.Application.Features.TeacherRequests.Review, TeacherERP.Application.Features.Users.Commands.Auth.Login, TeacherERP.Application.Features.Students.UnblacklistStudent, TeacherERP.Application.Features.Schedule.GetSessions, TeacherERP.Application.Features.TeacherRequests.Status, TeacherERP.Application.Features.Tenants (+18 more)

### Community 11 - "Session"
Cohesion: 0.05
Nodes (11): TeacherERP.Application.SessionContent, TeacherERP.API.Security, TeacherERP.API.Controllers, TeacherERP.API.Extensions, ResolvePaymobAdjustmentBody, PaymobReturnController, PaymobWebhookController, UpdatePlatformCommissionSettingsRequest (+3 more)

### Community 12 - "IRequestHandler"
Cohesion: 0.05
Nodes (7): IExternalLoginService, AttendanceRepository, GroupRepository, TeacherAccessRequestRepository, CredentialService, ExternalLoginService, UserAccountService

### Community 13 - "List"
Cohesion: 0.03
Nodes (26): TeacherERP.Application.Features.Attendance.RejectCompensation, TeacherERP.Application.Enrollment, TeacherERP.Application.Features.TeacherRequests.Submit, TeacherERP.Tests.Unit, TeacherERP.Application.Features.Notifications, TeacherERP.Application.Features.Administration, TeacherERP.Application.Features.TeacherRequests.Verify, TeacherERP.Application.Features.Enrollment.MoveStudent (+18 more)

### Community 14 - "DateTime"
Cohesion: 0.07
Nodes (44): GetPendingCompensationsQuery, GetPendingCompensationsQueryHandler, GetPendingCompensationsValidator, GetSessionAttendanceQuery, GetSessionAttendanceQueryHandler, GetAuditLogQuery, GetAuditLogQueryHandler, GetAuditLogValidator (+36 more)

### Community 16 - "Group"
Cohesion: 0.04
Nodes (30): AppDbContext, AttendanceRecords, AuditLogs, CenterInvitations, CenterJoinRequests, Centers, CenterTeachers, CurrentTenantId (+22 more)

### Community 17 - "ApplicationUser"
Cohesion: 0.06
Nodes (34): AdminCenterDto, CenterTeacherLinkDto, CreateCenterCommand, CreateCenterCommandHandler, CreateCenterValidator, LinkTeacherToCenterCommand, LinkTeacherToCenterCommandHandler, LinkTeacherToCenterValidator (+26 more)

### Community 18 - "IGroupSessionRepository"
Cohesion: 0.05
Nodes (19): MonthlyCollectionReportsController, PaymobAdjustmentReviewsController, PlatformCommissionsController, RequestPlatformCommissionCollection, SettlePlatformCommissionRequest, TeacherMonthlyCollectionReportsController, TeacherPlatformCommissionsController, SessionDto (+11 more)

### Community 19 - "TeacherERP.API"
Cohesion: 0.06
Nodes (35): CompleteProfileCommand, CompleteProfileCommandHandler, CompleteProfileValidator, GetMyProfileQuery, GetMyProfileQueryHandler, GuardianDto, ProfileDto, ChangePasswordCommand (+27 more)

### Community 20 - "AttendanceRecord"
Cohesion: 0.07
Nodes (11): UserTime, ResetTokenDto, VerifyResetOtpCommand, VerifyResetOtpCommandHandler, VerifyResetOtpValidator, IEmailService, IResetGrantService, ResetGrant (+3 more)

### Community 22 - "Center"
Cohesion: 0.10
Nodes (42): AttendanceRecorder, CenterJoinRequestNotifier, CenterMembershipCleanupJob, CenterProvisioningService, ManualAttendanceCommandHandler, AcceptCenterInvitationCommandHandler, CreateCenterInvitationCommandHandler, ApproveCenterJoinRequestCommandHandler (+34 more)

### Community 23 - "LoginSession"
Cohesion: 0.04
Nodes (13): TeacherERP.API.Middlewares, TeacherERP.Application, TeacherERP.Application.Absence, TeacherERP.Application.Centers, TeacherERP.Application.Features.Attendance.ApproveCompensation, TeacherERP.Application.Files, TeacherERP.Application.Features.Team.CreateAssistant, TeacherERP.Application.Features.Users.Commands.Auth.ResetPassword (+5 more)

### Community 24 - ".Handle"
Cohesion: 0.05
Nodes (34): TeacherERP.Infrastructure.Extensions, IPagedRequest, Page, PageSize, PagedResult, HasNext, HasPrevious, TotalPages (+26 more)

### Community 25 - "http"
Cohesion: 0.09
Nodes (26): ApproveInstaPayReceiptCommand, ApproveInstaPayReceiptCommandHandler, CollectCashPaymentCommand, CollectCashPaymentCommandValidator, CollectCashPaymentCommandHandler, GetMyPaymentOptionsQuery, GetMyPaymentOptionsQueryHandler, GetMyPaymentOptionsValidator (+18 more)

### Community 26 - "AttendanceController.cs"
Cohesion: 0.06
Nodes (20): MyEnrollmentDto, RescheduleSessionRequest, IAttendanceCodeService, PlatformCommissionCollectionRequestResult, PlatformCommissionSettlementResult, CenterJoinRequest, CenterId, DecisionNote (+12 more)

### Community 27 - ".SeedAsync"
Cohesion: 0.08
Nodes (13): AdministrationIntegrationTests, CompensationStaffIntegrationTests, CustomWebApplicationFactory, FileStorage, PaymobGateway, PaymobSettings, GroupGradeIntegrationTests, ManualAttendanceIntegrationTests (+5 more)

### Community 28 - "ICurrentTenantAccessor"
Cohesion: 0.09
Nodes (8): GetTeamQuery, GetTeamQueryHandler, TeamMemberDto, IAbsenceRepository, RunningSessionInfo, PlatformCommissionSummary, TeamMemberItem, AbsenceRepository

### Community 30 - "Transaction"
Cohesion: 0.05
Nodes (38): AuditLogDto, AuditAction, AttendanceCorrected, CashPaymentCollected, CompensationApproved, CompensationRejected, FinancialDelegationGranted, FinancialDelegationRevoked (+30 more)

### Community 31 - "GroupDto"
Cohesion: 0.09
Nodes (15): ApplicationUser, Attendances, DateOfBirth, FullName, Gender, Guardian, IsProfileComplete, LoginSessions (+7 more)

### Community 32 - "PasswordController.cs"
Cohesion: 0.07
Nodes (37): CloudinaryDotNet (1.29.3), coverlet.collector (6.0.4), FluentValidation (12.1.1), FluentValidation.DependencyInjectionExtensions (12.1.1), Google.Apis.Auth (1.77.0), Hangfire.AspNetCore (1.8.25), Hangfire.Core (1.8.25), Hangfire.Dashboard.BasicAuthorization (1.0.2) (+29 more)

### Community 33 - "MembershipStatus"
Cohesion: 0.14
Nodes (3): AttendanceCodeService, DynamicQrIntegrationTests, AttendanceCodeServiceTests

### Community 34 - "OtpService"
Cohesion: 0.07
Nodes (15): Session, AbsencesProcessedAt, Attendances, BoardImageUrl, CancelReason, EndTime, Group, GroupId (+7 more)

### Community 35 - ".SaveChangesAsync"
Cohesion: 0.13
Nodes (5): ScheduleConflictItem, SessionMoveBlockers, GroupSessionRepository, BulkCancelSessionsCommandHandlerTests, CancelSessionCommandHandlerTests

### Community 37 - "GuardianRelation"
Cohesion: 0.07
Nodes (15): CenterMembershipPolicy, GroupSlotDto, TeacherRequestPolicy, SessionPolicy, GroupScheduleSlot, Day, DurationMinutes, Group (+7 more)

### Community 39 - "ITenantRepository"
Cohesion: 0.14
Nodes (3): FinancialLedgerService, PaymobAdjustmentReviewService, IFinancialLedgerRepository

### Community 40 - "EnrollmentRequestStatus"
Cohesion: 0.06
Nodes (22): Transaction, Amount, IdempotencyKey, PaymentContextSessionId, ProcessedByAdminId, Provider, ProviderClientSecret, ProviderFailureCode (+14 more)

### Community 41 - "GroupEnrollment"
Cohesion: 0.07
Nodes (29): Notification, IsRead, Message, RecipientUserId, RelatedId, TenantId, Type, NotificationType (+21 more)

### Community 42 - "AbstractValidator"
Cohesion: 0.11
Nodes (11): IAbsenceProcessor, TenantContext, TenantRole, Assistant, Owner, Student, CurrentTenantAccessor, Current (+3 more)

### Community 43 - "TeacherERP.Application.Features.Users.Commands.Auth.Login"
Cohesion: 0.10
Nodes (12): IFileStorage, StoredFile, IStoredFileCleanupJob, StoredFileCleanupJob, PaymentType, DebtSettlement, MonthlySubscription, PerSession (+4 more)

### Community 44 - "fluentvalidation"
Cohesion: 0.07
Nodes (25): BaseEntity, CreatedAt, Id, UpdatedAt, LoginSession, AbsoluteExpiresAt, DeviceId, DeviceName (+17 more)

### Community 45 - "AttendanceResultDto"
Cohesion: 0.24
Nodes (4): Scenario, TeacherPaymentsIntegrationTests, Scenario, WalkInLocationIntegrationTests

### Community 46 - "TenantSettings"
Cohesion: 0.08
Nodes (19): EnrollmentRequest, DecidedAt, DecidedByUserId, DecisionNote, Group, GroupId, SourceLatitude, SourceLongitude (+11 more)

### Community 47 - ".Handle"
Cohesion: 0.14
Nodes (14): FinancialSessionInfo, CashPaymentRequest, FinancialObligation, Amount, GroupId, GroupName, IsSettled, OutstandingAmount (+6 more)

### Community 48 - "AuthResponseDto"
Cohesion: 0.08
Nodes (19): PlatformCommission, Amount, CollectionRequestedAtUtc, CollectionRequestedByUserId, GroupId, IsSettled, SessionId, SettledAtUtc (+11 more)

### Community 49 - "Error"
Cohesion: 0.09
Nodes (12): TeacherERP.Application.Features.Team, TeacherERP.Application.Features.Students, ApiAuthorizationResultHandler, GroupErrors, StudentErrors, StudentReasonRequest, TeamErrors, TeamPolicy (+4 more)

### Community 50 - "AuditLog"
Cohesion: 0.17
Nodes (10): AdminScanCommand, AdminScanCommandHandler, ManualAttendanceCommand, ManualAttendanceValidator, ScanAttendanceCommandHandler, GroupScanCommand, GroupScanCommandHandler, AttendanceRequest (+2 more)

### Community 52 - "GroupSessionRepository"
Cohesion: 0.09
Nodes (21): PaymobAdjustmentResolution, PaymobAdjustmentReview, CommissionReversalAmount, Currency, DecisionNote, ProviderAmount, ProviderEventId, ReviewedAtUtc (+13 more)

### Community 53 - "SessionValidator"
Cohesion: 0.09
Nodes (18): TeacherAccessRequest, CreatedUserId, DecisionNote, Email, ExpiresAtUtc, FullName, Note, ReviewedAtUtc (+10 more)

### Community 54 - ".ResolveAsync"
Cohesion: 0.08
Nodes (22): AttendanceRecord, CompensatesSessionId, RecordedByUserId, ScannedLatitude, ScannedLongitude, ScannedVia, Session, SessionId (+14 more)

### Community 55 - "ResetGrantService"
Cohesion: 0.13
Nodes (4): NotificationService, INotificationRepository, NotificationShareItem, NotificationRepository

### Community 56 - ".HandleAsync"
Cohesion: 0.14
Nodes (12): AccessTokenResult, IJwtProvider, ISessionRepository, SessionManager, TokenHasher, SessionRepository, JwtProvider, JwtSettings (+4 more)

### Community 57 - ".AddApiRateLimiting"
Cohesion: 0.13
Nodes (4): CorsExtensions, ForwardedHeadersExtensions, RateLimitingExtensions, GlobalExceptionHandler

### Community 58 - "TenantRole"
Cohesion: 0.09
Nodes (23): ASPNETCORE_ENVIRONMENT, applicationUrl, commandName, dotnetRunMessages, environmentVariables, applicationUrl, commandName, dotnetRunMessages (+15 more)

### Community 59 - "IServiceCollection"
Cohesion: 0.10
Nodes (16): Group, Capacity, CenterId, EducationStage, GradeLevel, GradeNumber, IsActive, MonthlyPrice (+8 more)

### Community 60 - "ICenterRepository"
Cohesion: 0.15
Nodes (6): RegistrationCacheKey, VerifyOtpCommandHandler, IUserAccountService, TeacherProvisioningService, WelcomeEmailFailedException, TeacherProvisioningServiceTests

### Community 61 - ".ValidateIdTokenAsync"
Cohesion: 0.20
Nodes (12): CreatedTeacherDto, CreateTeacherCommand, CreateTeacherCommandHandler, CreateTeacherValidator, ApproveTeacherRequestByAdminCommand, ApproveTeacherRequestByAdminCommandHandler, ApproveTeacherRequestCommand, ApproveTeacherRequestCommandHandler (+4 more)

### Community 62 - "CompleteProfileValidator"
Cohesion: 0.10
Nodes (14): CreateMyCenterValidator, CenterInvitation, AcceptedAtUtc, AcceptedByTenantId, CenterId, CreatedByUserId, ExpiresAtUtc, Status (+6 more)

### Community 63 - "SessionManager"
Cohesion: 0.18
Nodes (14): ActivateAccountCommand, ActivateAccountCommandHandler, ActivateAccountValidator, GoogleSignInCommand, GoogleSignInCommandHandler, GoogleSignInValidator, AuthResponseDto, LoginCommand (+6 more)

### Community 64 - "TeacherERP.Application.Features.Team"
Cohesion: 0.12
Nodes (11): TenantMember, CanManageFinance, ConsecutiveAbsences, CurrentBalance, HasFinancialDelegation, Role, Status, Tenant (+3 more)

### Community 65 - "GetSessionsQueryHandler"
Cohesion: 0.13
Nodes (19): AbsenceProcessor, RemoveAssistantCommand, RemoveAssistantCommandHandler, RemoveAssistantValidator, ResendWelcomeCommand, ResendWelcomeCommandHandler, WelcomeEmailFailedException, ResendWelcomeValidator (+11 more)

### Community 66 - "SetFinancialDelegationCommandHandler.cs"
Cohesion: 0.14
Nodes (12): GetSessionAttachmentsQuery, GetSessionAttachmentsQueryHandler, GetSessionAttachmentsValidator, SessionAttachmentDto, UploadSessionAttachmentCommand, UploadSessionAttachmentValidator, GroupAccessService, GroupAccessLevel (+4 more)

### Community 67 - "RevokeSessionCommand"
Cohesion: 0.15
Nodes (14): OtpIssueResult, OtpPurpose, Activation, PasswordReset, Registration, TeacherRequest, IssueCounter, Count (+6 more)

### Community 68 - "CancelSessionCommandHandler.cs"
Cohesion: 0.20
Nodes (3): ScheduleSlotCandidate, SessionTimeCalculator, AddStudentProfile

### Community 69 - "HangfireBasicAuthFilter"
Cohesion: 0.11
Nodes (15): EnrollmentStatus, Active, Ended, GroupEnrollment, ApprovedByUserId, EndedAt, EnrolledAt, Group (+7 more)

### Community 70 - "GetMySessionsQueryHandler"
Cohesion: 0.17
Nodes (11): AttendanceSettingsDto, DisplayCodeDto, GetAttendanceSettingsQuery, GetAttendanceSettingsQueryHandler, GetDisplayCodeQuery, GetDisplayCodeQueryHandler, RegenerateStaticCodeCommand, RegenerateStaticCodeCommandHandler (+3 more)

### Community 71 - "Centerdto.cs"
Cohesion: 0.11
Nodes (12): CreateGroupCommand, CreateGroupValidator, IGroupPricing, MonthlyPrice, PerSessionPrice, GroupSlotInput, GroupSlotsRules, IGroupSchedule (+4 more)

### Community 73 - "LoggingBehavior.cs"
Cohesion: 0.15
Nodes (13): ApproveTeacherRequestValidator, PreviewTeacherRequestQuery, PreviewTeacherRequestQueryHandler, PreviewTeacherRequestValidator, RejectTeacherRequestValidator, GetTeacherRequestStatusQuery, GetTeacherRequestStatusQueryHandler, GetTeacherRequestStatusValidator (+5 more)

### Community 74 - "GeoDistance"
Cohesion: 0.15
Nodes (9): CompletedProfileHandler, CompletedProfileRequirement, TenantHandler, TenantRequirement, AllowedRoles, RequiresFinancialDelegation, ProfileCompletionChecker, ProfileRequirement (+1 more)

### Community 75 - "CachedRegistrationData"
Cohesion: 0.25
Nodes (7): TeacherRequestQrDto, VerifyTeacherRequestCommand, VerifyTeacherRequestCommandHandler, VerifyTeacherRequestValidator, IOtpService, ITeacherAccessRequestRepository, VerifyTeacherRequestCommandHandlerTests

### Community 76 - ".TryHandleAsync"
Cohesion: 0.11
Nodes (16): PaymentEvent, CurrentStatus, EventType, PayloadSha256, PreviousStatus, Provider, ProviderEventId, ProviderOccurredAtUtc (+8 more)

### Community 77 - "UpdateGroupValidator"
Cohesion: 0.11
Nodes (18): SessionAttachment, Caption, ContentType, FileName, Kind, PublicId, Session, SessionId (+10 more)

### Community 79 - "Updatecentervalidator.cs"
Cohesion: 0.14
Nodes (6): AddCenterMembershipRequests, SnapshotAccessor, SnapshotAccessor, AddSessionOriginalStartTime, SnapshotAccessor, AppDbContextModelSnapshot

### Community 80 - "AddExtraSessionValidator.cs"
Cohesion: 0.16
Nodes (15): FinancialTransactionInfo, MonthlyCollectionItem, MonthlyCollectionReport, MonthlyCollectionTotals, MonthlyOutstandingDebtItem, MonthlyPaymentMethodSummary, TransactionStatus, Failed (+7 more)

### Community 81 - "BulkCancelSessionsValidator.cs"
Cohesion: 0.21
Nodes (8): AssistantDto, CreateAssistantCommand, CreateAssistantCommandHandler, WelcomeEmailFailedException, CreateAssistantValidator, IPasswordGenerator, ITeamRepository, PasswordGenerator

### Community 82 - "BulkShiftSessionsValidator.cs"
Cohesion: 0.12
Nodes (10): PlatformCommissionSettings, MonthlySubscriptionAmount, PerSessionAmount, UpdatedAtUtc, UpdatedByUserId, TeacherPaymentSettings, InstaPayLink, QrImagePublicId (+2 more)

### Community 84 - "BlacklistStudentValidator.cs"
Cohesion: 0.12
Nodes (6): TeacherERP.Application.Features.Schedule.BulkCancelSessions, TeacherERP.Application.Scheduling, TeacherERP.Application.Features.Schedule.BulkShiftSessions, TeacherERP.Application.Features.Schedule.CancelSession, CancelSessionRequest, SchedulePolicy

### Community 85 - "UnblacklistStudentValidator.cs"
Cohesion: 0.12
Nodes (14): PaymobSettings, ApiKey, BaseUrl, Currency, HmacNullValue, HmacSecret, InitializationLeaseSeconds, IntegrationIds (+6 more)

### Community 86 - "CreateAssistantValidator.cs"
Cohesion: 0.25
Nodes (5): SubmitTeacherRequestCommand, SubmitTeacherRequestCommandHandler, TeacherRequestCacheKey, TeacherRequestOtpDto, SubmitTeacherRequestCommandHandlerTests

### Community 87 - "RemoveAssistantValidator.cs"
Cohesion: 0.12
Nodes (8): MonthlySubscription, Amount, GroupId, Month, PaymentTransactionId, StudentId, TenantId, Year

### Community 88 - "ResendWelcomeValidator.cs"
Cohesion: 0.17
Nodes (9): FileNames, DetectedFileType, Jpeg, Pdf, Png, Unknown, Webp, FileSignature (+1 more)

### Community 89 - "SetFinancialDelegationValidator.cs"
Cohesion: 0.22
Nodes (13): CreatePaymobIntentBody, CreatePaymobIntentCommand, CreatePaymobIntentCommandValidator, CreatePaymobIntentCommandHandler, CreatePaymobPaymentRequest, IPaymobPaymentService, PaymobIntegrationKey, CaggOnlineNonShopify (+5 more)

### Community 90 - "GoogleSignInValidator.cs"
Cohesion: 0.17
Nodes (4): TeacherERP.Application.Features.Profile.CompleteProfile, TeacherERP.Application.Features.Groups.UpdateGroup, TeacherERP.Application.Features.Profile, TeacherERP.Domain.ValueObjects

### Community 91 - "ResetPasswordValidator.cs"
Cohesion: 0.18
Nodes (4): PaymobGatewayTests, RecordingLogger, Messages, StubHandler

### Community 92 - "VerifyResetOtpValidator.cs"
Cohesion: 0.26
Nodes (3): PlatformCommissionSettingsService, IPlatformCommissionSettingsService, PlatformCommissionSettingsDto

### Community 93 - "CreateStudentValidator.cs"
Cohesion: 0.23
Nodes (4): StudentTimelineAttachmentDto, ISessionAttachmentRepository, SessionAttachmentItem, SessionAttachmentRepository

### Community 94 - "VerifyOtpValidator.cs"
Cohesion: 0.14
Nodes (12): Tenant, AutoApproveGroupAdminChanges, AutoApproveJoinRequests, IsListedInSearch, Name, OwnerId, Status, Subject (+4 more)

### Community 96 - "IdentityService.cs"
Cohesion: 0.21
Nodes (9): CenterInvitationDto, CenterJoinRequestListItem, CreateCenterInvitationCommand, CreateCenterInvitationValidator, GetCenterJoinRequestsQuery, GetCenterJoinRequestsQueryHandler, GetCenterJoinRequestsValidator, JoinCenterCommand (+1 more)

### Community 97 - "TeacherERP.Application.Features.Schedule.GetSessions"
Cohesion: 0.21
Nodes (6): EducationStage, Preparatory, Primary, Secondary, Unspecified, SchoolGrade

### Community 98 - "UpdateGroupValidator"
Cohesion: 0.16
Nodes (11): AttendanceMode, AdminOnly, DynamicScreenQR, StaticGlobalQR, TenantSettings, AutoApproveGroupAdminScans, AutoRegisterOnScan, Mode (+3 more)

### Community 100 - "Updatecentervalidator.cs"
Cohesion: 0.29
Nodes (7): GetStudentDetailsQuery, GetStudentDetailsQueryHandler, StudentActiveGroupDto, StudentDetailsDto, IStudentRepository, StudentActiveGroupItem, StudentDetailsItem

### Community 101 - "RejectRequestValidator.cs"
Cohesion: 0.15
Nodes (12): UpdateTeacherPaymentSettingsForm, InstaPayLink, QrImage, UploadSessionAttachmentForm, Caption, File, Kind, SubmitInstaPayReceiptForm (+4 more)

### Community 102 - "RequestGroupValidator.cs"
Cohesion: 0.31
Nodes (6): IStudentWeeklyScheduleRepository, StudentWeeklyScheduleData, StudentWeeklyScheduleGroupItem, StudentWeeklyScheduleSlotItem, StudentRepository, StudentWeeklyScheduleRepository

### Community 103 - "GroupScanValidator.cs"
Cohesion: 0.27
Nodes (7): TeacherRequestRules, ReviewerKind, Administrator, Teacher, ProvisioningRejectedException, Error, TeacherRequestReviewService

### Community 104 - "RequestGroupAdminChangeValidator.cs"
Cohesion: 0.36
Nodes (6): DirectoryCenterInfo, DirectoryRow, DirectorySearchPage, IDirectoryRepository, TeacherProfileData, DirectoryRepository

### Community 105 - "CancelSessionValidator.cs"
Cohesion: 0.29
Nodes (4): StudentTimelineSessionDto, IStudentTimelineRepository, StudentTimelineSessionItem, StudentTimelineRepository

### Community 108 - "CreateAssistantValidator.cs"
Cohesion: 0.20
Nodes (7): IUserTimeZoneAccessor, TimeZoneId, CurrentUserService, SessionId, UserId, HttpUserTimeZoneAccessor, TimeZoneId

### Community 109 - "RemoveAssistantValidator.cs"
Cohesion: 0.22
Nodes (5): AbsenceProcessingJob, IAbsenceProcessingJob, ISessionGenerationJob, ISessionGenerator, SessionGenerationJob

### Community 110 - "ResendWelcomeValidator.cs"
Cohesion: 0.29
Nodes (8): EnrollmentRequestDto, MyEnrollmentsDto, GetMyEnrollmentsQuery, GetMyEnrollmentsQueryHandler, GetMyEnrollmentsValidator, RequestGroupCommand, RequestGroupCommandHandler, RequestGroupValidator

### Community 111 - "Updatetenantprofilevalidator.cs"
Cohesion: 0.42
Nodes (7): TeacherERP.Application.Features.Directory, DirectoryCenterInfoDto, DirectoryGroupDto, TeacherProfileDto, GetTeacherProfileQuery, GetTeacherProfileQueryHandler, GetTeacherProfileValidator

### Community 112 - "ChangePasswordValidator.cs"
Cohesion: 0.39
Nodes (7): GetGroupQuery, GetGroupQueryHandler, GetGroupsQuery, GetGroupsQueryHandler, GetGroupsValidator, GroupDto, UpdateGroupScheduleCommand

### Community 113 - "GoogleSignInValidator.cs"
Cohesion: 0.33
Nodes (7): IPaymobGateway, PaymobIntentCommandData, PaymobIntentData, TestPaymobGateway, CreateCount, FailNextWithCode, LastRequest

### Community 115 - "CreateStudentValidator.cs"
Cohesion: 0.50
Nodes (5): GetCurrentTenantQuery, GetCurrentTenantQueryHandler, GetMyTenantsQuery, GetMyTenantsQueryHandler, TenantDto

### Community 116 - "ProfileAuthorizationResultHandler.cs"
Cohesion: 0.36
Nodes (6): EnrollmentRequestListItem, EnrollmentRequestStatus, Approved, Cancelled, Pending, Rejected

### Community 117 - "IdentityService.cs"
Cohesion: 0.29
Nodes (3): TeacherERP.Application.Behaviors, LoggingBehavior, ValidationBehavior

### Community 118 - "AttendanceOutcome"
Cohesion: 0.29
Nodes (6): CachedTeacherRequestData, Email, FullName, Note, Subject, TimeZoneId

### Community 120 - "HangfireBasicAuthFilter"
Cohesion: 0.67
Nodes (3): StudentWeeklyGroupScheduleDto, StudentWeeklyScheduleDto, StudentWeeklyScheduleSlotDto

### Community 121 - "ForwardedHeadersExtensions.cs"
Cohesion: 0.33
Nodes (5): StudentListDto, MembershipStatus, Active, Blacklisted, Suspended

### Community 122 - "GetNotificationWhatsAppShareQuery"
Cohesion: 0.33
Nodes (6): AttendanceOutcome, AlreadyRecorded, Corrected, PendingApproval, PendingCompensation, Recorded

### Community 123 - "SessionCleanupJob"
Cohesion: 0.53
Nodes (3): GoogleUserInfo, IGoogleAuthService, GoogleAuthService

### Community 124 - "CloudinarySettings"
Cohesion: 0.33
Nodes (6): PaymobWebhookOutcome, Accepted, Duplicate, Ignored, InvalidPayload, InvalidSignature

### Community 125 - "JwtSettings"
Cohesion: 0.33
Nodes (6): MailSettings, DisplayName, Host, Mail, Password, Port

### Community 129 - "MailSettings"
Cohesion: 0.40
Nodes (4): CloudinarySettings, ApiKey, ApiSecret, CloudName

### Community 133 - "PreviewTeacherRequestQuery"
Cohesion: 1.00
Nodes (3): ForgotPasswordCommand, ForgotPasswordCommandHandler, ForgotPasswordResponseDto

### Community 134 - ".ValidateIdTokenAsync"
Cohesion: 0.50
Nodes (4): CachedRegistrationData, Email, FullName, Password

### Community 135 - "GetMyEnrollmentsQuery"
Cohesion: 0.50
Nodes (4): StudentStatus, Active, Blacklisted, Suspended

## Knowledge Gaps
- **605 isolated node(s):** `InstaPayLink`, `QrImage`, `Caption`, `File`, `Kind` (+600 more)
  These have ≤1 connection - possible missing edges. (Counts symbols only; 875 node(s) total have ≤1 connection when file, concept and rationale nodes are included.)
- **22 thin communities (<3 nodes) omitted from report** — run `graphify query` to explore isolated nodes.

## Suggested Questions
_Questions this graph is uniquely positioned to answer:_

- **Why does `Result` connect `TeacherERP.Application.Interfaces` to `Task`, `system`, `PreviewTeacherRequestQuery`, `.AddApplicationServices`, `IRequestHandler`, `List`, `DateTime`, `ApplicationUser`, `IGroupSessionRepository`, `TeacherERP.API`, `AttendanceRecord`, `Center`, `http`, `ICurrentTenantAccessor`, `GroupDto`, `TeacherERP.Application.Features.Groups`, `TeacherERP.Application.Features.Users.Commands.Auth.Login`, `.Handle`, `AuditLog`, `.HandleAsync`, `ICenterRepository`, `.ValidateIdTokenAsync`, `SessionManager`, `GetSessionsQueryHandler`, `SetFinancialDelegationCommandHandler.cs`, `RevokeSessionCommand`, `CancelSessionCommandHandler.cs`, `GetMySessionsQueryHandler`, `Centerdto.cs`, `LoggingBehavior.cs`, `CachedRegistrationData`, `BulkCancelSessionsValidator.cs`, `RescheduleSessionValidator.cs`, `CreateAssistantValidator.cs`, `SetFinancialDelegationValidator.cs`, `VerifyResetOtpValidator.cs`, `IdentityService.cs`, `GroupScanValidator.cs`, `CancelSessionValidator.cs`, `BlacklistStudentValidator.cs`, `ResendWelcomeValidator.cs`, `Updatetenantprofilevalidator.cs`, `ChangePasswordValidator.cs`, `GoogleSignInValidator.cs`, `CreateStudentValidator.cs`, `SessionCleanupJob`?**
  _High betweenness centrality (0.066) - this node is a cross-community bridge._
- **What connects `InstaPayLink`, `QrImage`, `Caption` to the rest of the system?**
  _605 weakly-connected nodes found - possible documentation gaps or missing edges._
- **Should `TeacherERP.Domain.Shared` be split into smaller, more focused modules?**
  _Cohesion score 0.021377331420373027 - nodes in this community are weakly interconnected._
- **Why does `TeacherERP.Domain.Entities` connect `TimeSpan` to `TeacherERP.Domain.Shared`, `TeacherERP.Application.Interfaces`, `system`, `system_text`, `Result`, `Session`, `List`, `DateTime`, `ApplicationUser`, `TeacherERP.API`, `LoginSession`, `.Handle`, `http`, `AttendanceController.cs`, `.SeedAsync`, `Transaction`, `OtpService`, `GuardianRelation`, `GroupEnrollment`, `fluentvalidation`, `TenantSettings`, `.Handle`, `AuthResponseDto`, `GroupSessionRepository`, `SessionValidator`, `.ResolveAsync`, `CompleteProfileValidator`, `HangfireBasicAuthFilter`, `GetMySessionsQueryHandler`, `.TryHandleAsync`, `UpdateGroupValidator`, `AddExtraSessionValidator.cs`, `BulkShiftSessionsValidator.cs`, `BlacklistStudentValidator.cs`, `RemoveAssistantValidator.cs`, `SetFinancialDelegationValidator.cs`, `GoogleSignInValidator.cs`, `VerifyOtpValidator.cs`, `IdentityService.cs`, `RequestGroupAdminChangeValidator.cs`?**
  _High betweenness centrality (0.051) - this node is a cross-community bridge._
- **Should `IActionResult` be split into smaller, more focused modules?**
  _Cohesion score 0.045312603813700086 - nodes in this community are weakly interconnected._
- **Why does `TeacherERP.Application.Interfaces` connect `TimeSpan` to `TeacherERP.Domain.Shared`, `Task`, `TeacherERP.Application.Interfaces`, `system`, `system_text`, `Result`, `Session`, `List`, `DateTime`, `.AddInfrastructureServices`, `AttendanceRecord`, `Center`, `LoginSession`, `http`, `AbstractValidator`, `TeacherERP.Application.Features.Users.Commands.Auth.Login`, `RevokeSessionCommand`, `GeoDistance`, `AddExtraSessionValidator.cs`, `BulkCancelSessionsValidator.cs`, `BlacklistStudentValidator.cs`, `SetFinancialDelegationValidator.cs`, `GoogleSignInValidator.cs`, `VerifyResetOtpValidator.cs`, `Updatecentervalidator.cs`, `RequestGroupValidator.cs`, `RequestGroupAdminChangeValidator.cs`, `CreateAssistantValidator.cs`, `RemoveAssistantValidator.cs`, `HangfireBasicAuthFilter`, `SessionCleanupJob`?**
  _High betweenness centrality (0.051) - this node is a cross-community bridge._
- **Should `Task` be split into smaller, more focused modules?**
  _Cohesion score 0.03126385809312639 - nodes in this community are weakly interconnected._