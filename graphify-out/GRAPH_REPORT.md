# Graph Report - TeacherERP  (2026-11-23)

## Corpus Check
- cluster-only mode — file stats not available

## Summary
- 4372 nodes · 16415 edges · 147 communities (117 shown, 30 thin omitted)
- Extraction: 90% EXTRACTED · 10% INFERRED · 0% AMBIGUOUS · INFERRED: 1577 edges (avg confidence: 0.85)
- Token cost: 0 input · 0 output

## Graph Freshness
- Built from commit: `4c86b608`
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
- ApplicationUser
- IGroupSessionRepository
- TeacherERP.API
- AttendanceRecord
- AppDbContext
- Center
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
- SessionCleanupJob
- CloudinarySettings
- JwtSettings
- .TokenValidated
- .Meters
- PaymobWebhookOutcome
- HangfireBasicAuthFilter
- .Resolve
- PreviewTeacherRequestQuery
- .ValidateIdTokenAsync
- GetMyEnrollmentsQuery
- GetMyNotificationsQuery
- ListTeacherRequestsQuery
- .ValidatePayment
- EgyptianPhoneNumber.cs
- GroupSlotInput
- AttendanceOutcome
- PaymobWebhookOutcome
- StudentStatus
- StudentRepository

## God Nodes (most connected - your core abstractions)
1. `Result` - 412 edges
2. `TeacherERP.Application.Interfaces` - 297 edges
3. `TeacherERP.Domain.Shared` - 276 edges
4. `TeacherERP.Domain.Entities` - 230 edges
5. `AppDbContext` - 110 edges
6. `ApplicationUser` - 88 edges
7. `TeacherERP.Infrastructure.Data` - 58 edges
8. `TeacherERP.Infrastructure.Migrations` - 56 edges
9. `Session` - 52 edges
10. `Transaction` - 49 edges

## Surprising Connections (you probably didn't know these)
- `WalkInApprovalTests` --calls--> `TenantContext`  [INFERRED]
  TeacherERP.Tests/Unit/WalkInLocationTests.cs → TeacherERP.Application/Interfaces/Icurrenttenantaccessor.cs
- `StudentErrors` --references--> `Error`  [EXTRACTED]
  TeacherERP.Application/Features/Students/StudentErrors.cs → TeacherERP.Domain/Shared/Error.cs
- `TeamErrors` --references--> `Error`  [EXTRACTED]
  TeacherERP.Application/Features/Team/TeamErrors.cs → TeacherERP.Domain/Shared/Error.cs
- `BulkCancelSessionsCommandHandlerTests` --calls--> `TenantContext`  [INFERRED]
  TeacherERP.Tests/Unit/CancelSessionHandlersTests.cs → TeacherERP.Application/Interfaces/Icurrenttenantaccessor.cs
- `CancelSessionCommandHandlerTests` --calls--> `TenantContext`  [INFERRED]
  TeacherERP.Tests/Unit/CancelSessionHandlersTests.cs → TeacherERP.Application/Interfaces/Icurrenttenantaccessor.cs

## Import Cycles
- None detected.

## Communities (147 total, 30 thin omitted)

### Community 0 - "TeacherERP.Domain.Shared"
Cohesion: 0.03
Nodes (22): MoveStudentRequest, LinkTeacherToCenterRequest, RejectGroupAdminRequestCommand, UpdateGroupRequest, MoveSessionRequest, RunningSessionInfo, MyEnrollmentItem, MyRequestItem (+14 more)

### Community 1 - "IActionResult"
Cohesion: 0.02
Nodes (57): TeacherERP.Application.Features.TeacherRequests.Submit, TeacherERP.Application.Features.Team.GetTeam, TeacherERP.Application.Features.TeacherRequests.Review, TeacherERP.Application.Features.Notifications, TeacherERP.Application.Features.Finance.GetPaymentOptions, TeacherERP.Application.Features.Centers.JoinRequests, TeacherERP.Application.Features.Finance.UpdateTeacherPaymentSettings, TeacherERP.Application.Features.Finance.ApproveInstaPayReceipt (+49 more)

### Community 2 - "Task"
Cohesion: 0.05
Nodes (24): AdministrationController, AttendanceController, AuditController, AuthController, BaseApiController, CentersController, DirectoryController, EnrollmentController (+16 more)

### Community 3 - "TeacherERP.Application.Interfaces"
Cohesion: 0.04
Nodes (29): TeacherERP.Infrastructure.Data, TeacherERP.Infrastructure.Migrations, InitialCreate, ReplaceRefreshTokensWithLoginSessions, FixLoginSessionUserId, AddStudentProfile, AddTenants, MoveCenterStateToMembership (+21 more)

### Community 4 - "system"
Cohesion: 0.03
Nodes (25): TeacherERP.Application.Enrollment, TeacherERP.Application.Scheduling, TeacherERP.Application.Absence, TeacherERP.Application.Features.Enrollment.GetMyEnrollments, TeacherERP.Application.Features.Groups.GetGroups, TeacherERP.Application.Features.Audit, TeacherERP.Application.Features.Directory.SearchDirectory, TeacherERP.Application.Features.Centers (+17 more)

### Community 5 - "TimeSpan"
Cohesion: 0.03
Nodes (20): TeacherERP.API.Middlewares, TeacherERP.Application, TeacherERP.Infrastructure.Settings, TeacherERP.Application.Features.Users.Commands.VerifyOtp, TeacherERP.Application.Attendance, TeacherERP.Application.Centers, TeacherERP.Application.Features.Attendance.ScanAttendance, TeacherERP.Application.Files (+12 more)

### Community 6 - "system_text"
Cohesion: 0.06
Nodes (8): TeacherERP.Application.SessionContent, TeacherERP.API.Security, TeacherERP.API.Controllers, TeacherERP.API.Extensions, ResolvePaymobAdjustmentBody, UpdatePlatformCommissionSettingsRequest, RateLimitPolicies, AuthPolicies

### Community 7 - "Guid"
Cohesion: 0.04
Nodes (25): TeacherERP.Application.Features.Attendance, TeacherERP.Application.Features.GroupAdmins.GetGroupAdminRequests, TeacherERP.Application.Features.GroupAdmins.GetMyGroupAdmin, TeacherERP.Application.Features.Students.GetStudentDetails, TeacherERP.Application.Features.Students.GetWeeklySchedule, TeacherERP.Application.Features.Students.GetStudentCard, TeacherERP.Application.Features.Attendance.UpdateAttendanceSettings, TeacherERP.Application.Features.GroupAdmins.GroupScan (+17 more)

### Community 8 - "CreateAssistantCommandHandler"
Cohesion: 0.04
Nodes (64): PagedRequestRules, AdminScanValidator, ApproveCompensationCommand, GetAttendanceSettingsQuery, RegenerateStaticCodeCommand, RejectCompensationCommand, ScanAttendanceCommand, ScanAttendanceValidator (+56 more)

### Community 9 - ".AddApplicationServices"
Cohesion: 0.07
Nodes (12): CenterMembershipPolicy, MyEnrollmentDto, UpdateGroupScheduleRequest, SessionDto, RescheduleSessionRequest, TeacherRequestPolicy, IAttendanceCodeService, PlatformCommissionCollectionRequestResult (+4 more)

### Community 10 - "Result"
Cohesion: 0.10
Nodes (8): CenterMembershipIntegrationTests, FinanceAuthorizationIntegrationTests, GroupGradeIntegrationTests, SessionGenerationIntegrationTests, SessionMoveIntegrationTests, StudentExperienceIntegrationTests, StudentTransferIntegrationTests, TestData

### Community 11 - "Session"
Cohesion: 0.10
Nodes (56): TenantHandler, AbsenceService, CenterJoinRequestNotifier, CenterMembershipCleanupJob, CenterProvisioningService, ApproveCompensationCommandHandler, ManualAttendanceCommandHandler, RejectCompensationCommandHandler (+48 more)

### Community 12 - "IRequestHandler"
Cohesion: 0.05
Nodes (36): GeoDistance, AdminCenterDto, CenterTeacherLinkDto, CreateCenterCommand, CreateCenterCommandHandler, CreateCenterValidator, LinkTeacherToCenterCommand, LinkTeacherToCenterCommandHandler (+28 more)

### Community 13 - "List"
Cohesion: 0.05
Nodes (24): MonthlyCollectionReportsController, PlatformCommissionsController, RequestPlatformCommissionCollection, SettlePlatformCommissionRequest, TeacherMonthlyCollectionReportsController, TeacherPlatformCommissionsController, MonthlyCollectionReportService, AuditLogItem (+16 more)

### Community 14 - "DateTime"
Cohesion: 0.04
Nodes (14): TeacherERP.Application.Features.Users.Commands.Auth.GoogleSignIn, TeacherERP.Application.Features.Groups, TeacherERP.Application.Features.SessionContent.UploadSessionAttachment, TeacherERP.Application.Features.Users.Commands.Auth.Login, TeacherERP.Application.Features.Users.Commands.Auth.ActivateAccount, TeacherERP.Application.Features.Schedule.MoveSession, TeacherERP.Application.Features.SessionContent.DeleteSessionAttachment, TeacherERP.Application.Features.Groups.UpdateGroupSchedule (+6 more)

### Community 15 - ".AddInfrastructureServices"
Cohesion: 0.07
Nodes (42): GetPendingCompensationsQuery, GetPendingCompensationsQueryHandler, GetPendingCompensationsValidator, GetSessionAttendanceQuery, GetSessionAttendanceQueryHandler, GetSessionAttendanceValidator, GetAuditLogQuery, GetAuditLogQueryHandler (+34 more)

### Community 17 - "ApplicationUser"
Cohesion: 0.08
Nodes (4): TeacherERP.Application.Features.Groups.UpdateGroup, TeacherERP.Domain.ValueObjects, TeacherERP.Tests.Integration, DependencyInjection

### Community 18 - "IGroupSessionRepository"
Cohesion: 0.05
Nodes (18): TeacherERP.Application.Features.Attendance.RejectCompensation, TeacherERP.Application.Features.Finance.GetMyPaymentOptions, TeacherERP.Application.Features.Finance.CreatePaymobIntent, TeacherERP.Infrastructure, TeacherERP.Application.Features.SessionContent, TeacherERP.Application.Finance, TeacherERP.Application.Features.Centers.UpdateCenter, AdministrationErrors (+10 more)

### Community 19 - "TeacherERP.API"
Cohesion: 0.07
Nodes (35): DisplayCodeDto, GetDisplayCodeQuery, GetDisplayCodeQueryHandler, CancelCenterJoinRequestCommand, CancelCenterJoinRequestValidator, RejectCenterJoinRequestCommand, RejectCenterJoinRequestValidator, RejectInstaPayReceiptCommand (+27 more)

### Community 20 - "AttendanceRecord"
Cohesion: 0.10
Nodes (5): ScheduleConflictItem, SessionMoveBlockers, GroupSessionRepository, BulkCancelSessionsCommandHandlerTests, CancelSessionCommandHandlerTests

### Community 21 - "AppDbContext"
Cohesion: 0.11
Nodes (3): FinancialLedgerService, IStoredFileCleanupJob, SessionContentPolicy

### Community 22 - "Center"
Cohesion: 0.07
Nodes (36): CancelRequestCommand, CancelRequestCommandHandler, RejectRequestCommand, RejectRequestCommandHandler, RejectRequestValidator, MarkAllNotificationsReadCommand, MarkAllNotificationsReadCommandHandler, MarkNotificationReadCommand (+28 more)

### Community 24 - ".Handle"
Cohesion: 0.06
Nodes (9): TeacherERP.Application.Features.Schedule.BulkCancelSessions, TeacherERP.Application.Features.Enrollment.ApproveRequest, TeacherERP.Application.Features.Schedule.GetSessions, TeacherERP.Application.Features.Attendance.ApproveCompensation, TeacherERP.Application.Features.Schedule, TeacherERP.Application.Features.Schedule.BulkShiftSessions, TeacherERP.Application.Features.Schedule.RescheduleSession, TeacherERP.Application.Features.Schedule.CancelSession (+1 more)

### Community 25 - "http"
Cohesion: 0.09
Nodes (22): StudentTimelineAttachmentDto, StudentTimelineSessionDto, GetStudentDetailsQuery, GetStudentDetailsQueryHandler, StudentActiveGroupDto, StudentDetailsDto, GetStudentWeeklyScheduleQuery, GetStudentWeeklyScheduleQueryHandler (+14 more)

### Community 26 - "AttendanceController.cs"
Cohesion: 0.13
Nodes (3): AttendanceCodeService, AdministrationIntegrationTests, AttendanceCodeServiceTests

### Community 27 - ".SeedAsync"
Cohesion: 0.07
Nodes (18): IExternalLoginService, IUserAccountService, ApplicationUser, Attendances, DateOfBirth, FullName, Gender, Guardian (+10 more)

### Community 28 - "ICurrentTenantAccessor"
Cohesion: 0.10
Nodes (4): UserTime, IEmailService, EmailService, EmailTemplates

### Community 29 - "IUserAccountService"
Cohesion: 0.05
Nodes (38): AuditLogDto, AuditAction, AttendanceCorrected, CashPaymentCollected, CompensationApproved, CompensationRejected, FinancialDelegationGranted, FinancialDelegationRevoked (+30 more)

### Community 30 - "Transaction"
Cohesion: 0.08
Nodes (25): TeacherERP.Infrastructure.Extensions, IPagedRequest, Page, PageSize, PagedResult, HasNext, HasPrevious, TotalPages (+17 more)

### Community 31 - "GroupDto"
Cohesion: 0.07
Nodes (37): CloudinaryDotNet (1.29.3), coverlet.collector (6.0.4), FluentValidation (12.1.1), FluentValidation.DependencyInjectionExtensions (12.1.1), Google.Apis.Auth (1.77.0), Hangfire.AspNetCore (1.8.25), Hangfire.Core (1.8.25), Hangfire.Dashboard.BasicAuthorization (1.0.2) (+29 more)

### Community 32 - "PasswordController.cs"
Cohesion: 0.06
Nodes (27): GroupAdminRequest, DecidedAt, DecidedByUserId, DecisionNote, Group, GroupId, RequestedByUserId, Status (+19 more)

### Community 33 - "MembershipStatus"
Cohesion: 0.06
Nodes (31): AppDbContext, AttendanceRecords, AuditLogs, CenterInvitations, CenterJoinRequests, Centers, CenterTeachers, CurrentTenantId (+23 more)

### Community 34 - "OtpService"
Cohesion: 0.07
Nodes (15): Session, AbsencesProcessedAt, Attendances, BoardImageUrl, CancelReason, EndTime, Group, GroupId (+7 more)

### Community 35 - ".SaveChangesAsync"
Cohesion: 0.09
Nodes (4): PaymobAdjustmentReviewService, Callback, PaymobErrors, PaymobPaymentService

### Community 36 - "TeacherERP.Application.Features.Groups"
Cohesion: 0.12
Nodes (8): GetGroupAdminsQuery, GetGroupAdminsQueryHandler, GroupAdminDto, IAbsenceRepository, PlatformCommissionSummary, GroupAdminItem, TeamMemberItem, AbsenceRepository

### Community 37 - "GuardianRelation"
Cohesion: 0.10
Nodes (9): EducationStage, Preparatory, Primary, Secondary, Unspecified, SchoolGrade, ScheduleOperationsIntegrationTests, GroupPricingTests (+1 more)

### Community 38 - ".GetByIdAsync"
Cohesion: 0.06
Nodes (22): Transaction, Amount, IdempotencyKey, PaymentContextSessionId, ProcessedByAdminId, Provider, ProviderClientSecret, ProviderFailureCode (+14 more)

### Community 40 - "EnrollmentRequestStatus"
Cohesion: 0.17
Nodes (10): AttendanceRecorder, AdminScanCommand, AdminScanCommandHandler, ScanAttendanceCommandHandler, GroupScanCommand, GroupScanCommandHandler, AttendanceRequest, AttendanceResultDto (+2 more)

### Community 41 - "GroupEnrollment"
Cohesion: 0.09
Nodes (16): Group, Capacity, CenterId, EducationStage, GradeLevel, GradeNumber, IsActive, MonthlyPrice (+8 more)

### Community 42 - "AbstractValidator"
Cohesion: 0.15
Nodes (17): ManualAttendanceCommand, ManualAttendanceValidator, GetMyGroupAdminQuery, GetMyGroupAdminQueryHandler, GroupAdminChangeResultDto, GroupAdminMeDto, RequestGroupAdminChangeCommand, RequestGroupAdminChangeCommandHandler (+9 more)

### Community 43 - "TeacherERP.Application.Features.Users.Commands.Auth.Login"
Cohesion: 0.08
Nodes (19): TeacherAccessRequestListItem, TeacherAccessRequest, CreatedUserId, DecisionNote, Email, ExpiresAtUtc, FullName, Note (+11 more)

### Community 44 - "fluentvalidation"
Cohesion: 0.08
Nodes (18): MonthlySubscription, Amount, GroupId, Month, PaymentTransactionId, StudentId, TenantId, Year (+10 more)

### Community 45 - "AttendanceResultDto"
Cohesion: 0.09
Nodes (17): EnrollmentRequest, DecidedAt, DecidedByUserId, DecisionNote, Group, GroupId, SourceLatitude, SourceLongitude (+9 more)

### Community 46 - "TenantSettings"
Cohesion: 0.14
Nodes (14): FinancialSessionInfo, CashPaymentRequest, FinancialObligation, Amount, GroupId, GroupName, IsSettled, OutstandingAmount (+6 more)

### Community 47 - ".Handle"
Cohesion: 0.08
Nodes (26): FinancialTransactionInfo, PaymentEvent, CurrentStatus, EventType, PayloadSha256, PreviousStatus, Provider, ProviderEventId (+18 more)

### Community 48 - "AuthResponseDto"
Cohesion: 0.08
Nodes (19): PlatformCommission, Amount, CollectionRequestedAtUtc, CollectionRequestedByUserId, GroupId, IsSettled, SessionId, SettledAtUtc (+11 more)

### Community 49 - "Error"
Cohesion: 0.14
Nodes (14): ApproveCenterJoinRequestCommand, ApproveCenterJoinRequestValidator, CreateGroupCommand, GetGroupQuery, GetGroupQueryHandler, GroupDto, GroupSlotDto, IGroupPricing (+6 more)

### Community 50 - "AuditLog"
Cohesion: 0.15
Nodes (17): LogoutCommand, LogoutCommandHandler, LogoutValidator, ActivateAccountCommand, ActivateAccountCommandHandler, ActivateAccountValidator, GoogleSignInCommand, GoogleSignInCommandHandler (+9 more)

### Community 52 - "GroupSessionRepository"
Cohesion: 0.13
Nodes (4): NotificationService, INotificationRepository, NotificationShareItem, NotificationRepository

### Community 53 - "SessionValidator"
Cohesion: 0.09
Nodes (21): PaymobAdjustmentResolution, PaymobAdjustmentReview, CommissionReversalAmount, Currency, DecisionNote, ProviderAmount, ProviderEventId, ReviewedAtUtc (+13 more)

### Community 54 - ".ResolveAsync"
Cohesion: 0.09
Nodes (21): LoginSession, AbsoluteExpiresAt, DeviceId, DeviceName, LastRefreshedAt, PreviousRefreshTokenHash, RefreshExpiresAt, RefreshTokenHash (+13 more)

### Community 55 - "ResetGrantService"
Cohesion: 0.08
Nodes (22): AttendanceRecord, CompensatesSessionId, RecordedByUserId, ScannedLatitude, ScannedLongitude, ScannedVia, Session, SessionId (+14 more)

### Community 56 - ".HandleAsync"
Cohesion: 0.09
Nodes (16): GroupSlotInput, IGroupSchedule, Slots, BaseEntity, CreatedAt, Id, UpdatedAt, GroupScheduleSlot (+8 more)

### Community 57 - ".AddApiRateLimiting"
Cohesion: 0.11
Nodes (10): Program, CompensationStaffIntegrationTests, CustomWebApplicationFactory, FileStorage, PaymobGateway, PaymobSettings, GroupPricingIntegrationTests, ManualAttendanceIntegrationTests (+2 more)

### Community 58 - "TenantRole"
Cohesion: 0.12
Nodes (11): TenantMember, CanManageFinance, ConsecutiveAbsences, CurrentBalance, HasFinancialDelegation, Role, Status, Tenant (+3 more)

### Community 59 - "IServiceCollection"
Cohesion: 0.09
Nodes (23): ASPNETCORE_ENVIRONMENT, applicationUrl, commandName, dotnetRunMessages, environmentVariables, applicationUrl, commandName, dotnetRunMessages (+15 more)

### Community 60 - "ICenterRepository"
Cohesion: 0.16
Nodes (16): ApproveInstaPayReceiptCommand, ApproveInstaPayReceiptCommandHandler, CollectCashPaymentCommand, CollectCashPaymentCommandValidator, CollectCashPaymentCommandHandler, GetMyPaymentOptionsQuery, GetMyPaymentOptionsQueryHandler, GetMyPaymentOptionsValidator (+8 more)

### Community 61 - ".ValidateIdTokenAsync"
Cohesion: 0.20
Nodes (12): CreatedTeacherDto, CreateTeacherCommand, CreateTeacherCommandHandler, CreateTeacherValidator, ApproveTeacherRequestByAdminCommand, ApproveTeacherRequestByAdminCommandHandler, ApproveTeacherRequestCommand, ApproveTeacherRequestCommandHandler (+4 more)

### Community 62 - "CompleteProfileValidator"
Cohesion: 0.19
Nodes (8): TeacherRequestQrDto, TeacherRequestToken, VerifyTeacherRequestCommand, VerifyTeacherRequestCommandHandler, VerifyTeacherRequestValidator, IOtpService, ITeacherAccessRequestRepository, VerifyTeacherRequestCommandHandlerTests

### Community 63 - "SessionManager"
Cohesion: 0.11
Nodes (10): GlobalExceptionHandler, AbsenceProcessingJob, AbsenceProcessor, LoggingBehavior, ValidationBehavior, IAbsenceProcessingJob, IAbsenceProcessor, IAbsenceService (+2 more)

### Community 64 - "TeacherERP.Application.Features.Team"
Cohesion: 0.21
Nodes (8): ChangePasswordCommand, ChangePasswordCommandHandler, ChangePasswordValidator, SetPasswordCommand, SetPasswordCommandHandler, SetPasswordValidator, ICredentialService, PasswordCommandTests

### Community 65 - "GetSessionsQueryHandler"
Cohesion: 0.16
Nodes (10): TenantContext, TenantRole, Assistant, Owner, Student, CurrentTenantAccessor, Current, TenantId (+2 more)

### Community 66 - "SetFinancialDelegationCommandHandler.cs"
Cohesion: 0.13
Nodes (4): ISessionRepository, ISessionValidator, SessionRepository, SessionValidator

### Community 67 - "RevokeSessionCommand"
Cohesion: 0.16
Nodes (12): CompleteProfileCommand, CompleteProfileCommandHandler, CompleteProfileValidator, GetMyProfileQuery, GetMyProfileQueryHandler, GuardianDto, ProfileDto, IUserRepository (+4 more)

### Community 68 - "CancelSessionCommandHandler.cs"
Cohesion: 0.16
Nodes (14): OtpIssueResult, OtpPurpose, Activation, PasswordReset, Registration, TeacherRequest, IssueCounter, Count (+6 more)

### Community 69 - "HangfireBasicAuthFilter"
Cohesion: 0.10
Nodes (13): CenterJoinRequest, CenterId, DecisionNote, LastNotifiedAtUtc, ReviewedAtUtc, ReviewedByUserId, Status, TenantId (+5 more)

### Community 70 - "GetMySessionsQueryHandler"
Cohesion: 0.12
Nodes (12): CenterInvitation, AcceptedAtUtc, AcceptedByTenantId, CenterId, CreatedByUserId, ExpiresAtUtc, Status, TokenHash (+4 more)

### Community 71 - "Centerdto.cs"
Cohesion: 0.11
Nodes (18): Notification, IsRead, Message, RecipientUserId, RelatedId, TenantId, Type, NotificationType (+10 more)

### Community 72 - "TeacherERP.Application.Features.Students"
Cohesion: 0.11
Nodes (4): TeacherERP.Application.Features.Users.Commands.Auth.SetPassword, TeacherERP.Tests.Unit, TeacherERP.Application.Features.Centers.CreateCenter, TeacherERP.Application.Features.Enrollment.MoveStudent

### Community 73 - "LoggingBehavior.cs"
Cohesion: 0.20
Nodes (8): IPaymobGateway, PaymobIntentCommandData, PaymobIntentData, PaymobGateway, TestPaymobGateway, CreateCount, FailNextWithCode, LastRequest

### Community 75 - "CachedRegistrationData"
Cohesion: 0.19
Nodes (4): VerifyOtpCommandHandler, TeacherProvisioningService, WelcomeEmailFailedException, TeacherProvisioningServiceTests

### Community 76 - ".TryHandleAsync"
Cohesion: 0.20
Nodes (7): SubmitTeacherRequestCommand, SubmitTeacherRequestCommandHandler, SubmitTeacherRequestValidator, TeacherRequestCacheKey, TeacherRequestOtpDto, SubmitTeacherRequestCommandHandlerTests, TeacherRequestBasicsTests

### Community 77 - "UpdateGroupValidator"
Cohesion: 0.19
Nodes (11): GetPaymentOptionsQuery, GetPaymentOptionsQueryHandler, TeacherPaymentService, FinancialDebtItem, FinancialPaymentOptions, FinancialTransactionItem, IFinancialLedgerService, PaymentType (+3 more)

### Community 78 - ".RequiresProfileCompletion"
Cohesion: 0.11
Nodes (18): SessionAttachment, Caption, ContentType, FileName, Kind, PublicId, Session, SessionId (+10 more)

### Community 80 - "AddExtraSessionValidator.cs"
Cohesion: 0.18
Nodes (10): EnrollmentService, EnrollmentRequestDto, MyEnrollmentsDto, GetMyEnrollmentsQuery, GetMyEnrollmentsQueryHandler, GetMyEnrollmentsValidator, RequestGroupCommand, RequestGroupCommandHandler (+2 more)

### Community 83 - "RescheduleSessionValidator.cs"
Cohesion: 0.13
Nodes (6): AddCenterMembershipRequests, SnapshotAccessor, SnapshotAccessor, AddSessionOriginalStartTime, SnapshotAccessor, AppDbContextModelSnapshot

### Community 84 - "BlacklistStudentValidator.cs"
Cohesion: 0.21
Nodes (7): ResetTokenDto, VerifyResetOtpCommand, VerifyResetOtpCommandHandler, VerifyResetOtpValidator, IResetGrantService, ResetGrant, ResetGrantService

### Community 85 - "UnblacklistStudentValidator.cs"
Cohesion: 0.24
Nodes (5): AccessTokenResult, IJwtProvider, SessionManager, TokenHasher, JwtProvider

### Community 86 - "CreateAssistantValidator.cs"
Cohesion: 0.12
Nodes (14): PaymobSettings, ApiKey, BaseUrl, Currency, HmacNullValue, HmacSecret, InitializationLeaseSeconds, IntegrationIds (+6 more)

### Community 87 - "RemoveAssistantValidator.cs"
Cohesion: 0.17
Nodes (9): FileNames, DetectedFileType, Jpeg, Pdf, Png, Unknown, Webp, FileSignature (+1 more)

### Community 89 - "SetFinancialDelegationValidator.cs"
Cohesion: 0.22
Nodes (13): CreatePaymobIntentBody, CreatePaymobIntentCommand, CreatePaymobIntentCommandValidator, CreatePaymobIntentCommandHandler, CreatePaymobPaymentRequest, IPaymobPaymentService, PaymobIntegrationKey, CaggOnlineNonShopify (+5 more)

### Community 90 - "GoogleSignInValidator.cs"
Cohesion: 0.12
Nodes (14): GuardianInfo, Name, PhoneNumber, Relation, GuardianRelation, Aunt, Brother, Father (+6 more)

### Community 91 - "ResetPasswordValidator.cs"
Cohesion: 0.21
Nodes (10): CenterDto, CreateMyCenterCommand, CreateMyCenterCommandHandler, CreateMyCenterValidator, GetMyCentersQuery, GetMyCentersQueryHandler, AcceptCenterInvitationCommand, AcceptCenterInvitationValidator (+2 more)

### Community 92 - "VerifyResetOtpValidator.cs"
Cohesion: 0.25
Nodes (12): TeacherERP.Application.Features.Directory, DirectoryCenterDto, DirectoryCenterInfoDto, DirectoryGroupDto, DirectoryTeacherDto, TeacherProfileDto, GetTeacherProfileQuery, GetTeacherProfileQueryHandler (+4 more)

### Community 93 - "CreateStudentValidator.cs"
Cohesion: 0.25
Nodes (4): DynamicQrIntegrationTests, Scenario, SessionContentNotificationsIntegrationTests, SeededTeacher

### Community 94 - "VerifyOtpValidator.cs"
Cohesion: 0.26
Nodes (3): PlatformCommissionSettingsService, IPlatformCommissionSettingsService, PlatformCommissionSettingsDto

### Community 95 - "ProfileAuthorizationResultHandler.cs"
Cohesion: 0.14
Nodes (12): Tenant, AutoApproveGroupAdminChanges, AutoApproveJoinRequests, IsListedInSearch, Name, OwnerId, Status, Subject (+4 more)

### Community 96 - "IdentityService.cs"
Cohesion: 0.16
Nodes (11): AttendanceMode, AdminOnly, DynamicScreenQR, StaticGlobalQR, TenantSettings, AutoApproveGroupAdminScans, AutoRegisterOnScan, Mode (+3 more)

### Community 97 - "TeacherERP.Application.Features.Schedule.GetSessions"
Cohesion: 0.23
Nodes (7): IFileStorage, StoredFile, StoredFileCleanupJob, CloudinaryFileStorage, TestFileStorage, DeletedPublicIds, UploadCount

### Community 98 - "UpdateGroupValidator"
Cohesion: 0.22
Nodes (4): PaymobGatewayTests, RecordingLogger, Messages, StubHandler

### Community 99 - "ManualAttendanceValidator.cs"
Cohesion: 0.29
Nodes (6): AttendanceSettingsDto, GetAttendanceSettingsQueryHandler, RegenerateStaticCodeCommandHandler, UpdateAttendanceSettingsCommand, UpdateAttendanceSettingsCommandHandler, UpdateAttendanceSettingsValidator

### Community 100 - "Updatecentervalidator.cs"
Cohesion: 0.15
Nodes (12): UpdateTeacherPaymentSettingsForm, InstaPayLink, QrImage, UploadSessionAttachmentForm, Caption, File, Kind, SubmitInstaPayReceiptForm (+4 more)

### Community 103 - "GroupScanValidator.cs"
Cohesion: 0.24
Nodes (7): TeacherRequestRules, ReviewerKind, Administrator, Teacher, ProvisioningRejectedException, Error, TeacherRequestReviewService

### Community 104 - "RequestGroupAdminChangeValidator.cs"
Cohesion: 0.20
Nodes (6): CachedRegistrationData, Email, FullName, Password, CreateStudentCommandHandler, RegistrationCacheKey

### Community 109 - "RemoveAssistantValidator.cs"
Cohesion: 0.22
Nodes (5): CompletedProfileHandler, CompletedProfileRequirement, ProfileCompletionChecker, ProfileRequirement, IProfileCompletionChecker

### Community 110 - "ResendWelcomeValidator.cs"
Cohesion: 0.20
Nodes (7): IUserTimeZoneAccessor, TimeZoneId, CurrentUserService, SessionId, UserId, HttpUserTimeZoneAccessor, TimeZoneId

### Community 111 - "Updatetenantprofilevalidator.cs"
Cohesion: 0.47
Nodes (5): AssistantDto, CreateAssistantCommand, CreateAssistantCommandHandler, WelcomeEmailFailedException, ITeamRepository

### Community 113 - "GoogleSignInValidator.cs"
Cohesion: 0.31
Nodes (4): PaymobAdjustmentReviewsController, PaymobAdjustmentReviewItem, IPaymobAdjustmentReviewService, ResolvePaymobAdjustmentRequest

### Community 114 - "RefreshTokenValidator.cs"
Cohesion: 0.25
Nodes (5): CenterInvitationDto, CreateCenterInvitationCommand, CreateCenterInvitationValidator, JoinCenterCommand, JoinCenterValidator

### Community 115 - "CreateStudentValidator.cs"
Cohesion: 0.31
Nodes (5): GoogleUserInfo, IGoogleAuthService, GoogleAuthService, GoogleAuthSettings, ClientIds

### Community 116 - "ProfileAuthorizationResultHandler.cs"
Cohesion: 0.22
Nodes (8): EnrollmentRequestStatus, Approved, Cancelled, Pending, Rejected, EnrollmentRequestType, Join, Transfer

### Community 118 - "AttendanceOutcome"
Cohesion: 0.32
Nodes (5): GroupErrors, TenantErrors, Error, Code, Message

### Community 121 - "ForwardedHeadersExtensions.cs"
Cohesion: 0.36
Nodes (6): GetSessionAttachmentsQuery, GetSessionAttachmentsQueryHandler, GetSessionAttachmentsValidator, SessionAttachmentDto, UploadSessionAttachmentCommand, UploadSessionAttachmentValidator

### Community 123 - "SessionCleanupJob"
Cohesion: 0.33
Nodes (3): TenantRequirement, AllowedRoles, RequiresFinancialDelegation

### Community 124 - "CloudinarySettings"
Cohesion: 0.29
Nodes (3): PaymobReturnController, PaymobWebhookController, RolesController

### Community 125 - "JwtSettings"
Cohesion: 0.29
Nodes (6): CachedTeacherRequestData, Email, FullName, Note, Subject, TimeZoneId

### Community 127 - ".Meters"
Cohesion: 0.33
Nodes (3): TeacherERP.Application.Features.Team, TeamErrors, TeamPolicy

### Community 131 - "HangfireBasicAuthFilter"
Cohesion: 0.33
Nodes (5): StudentListDto, MembershipStatus, Active, Blacklisted, Suspended

### Community 132 - ".Resolve"
Cohesion: 0.53
Nodes (4): ForgotPasswordCommand, ForgotPasswordCommandHandler, ForgotPasswordResponseDto, ForgotPasswordValidator

### Community 133 - "PreviewTeacherRequestQuery"
Cohesion: 0.33
Nodes (6): AttendanceOutcome, AlreadyRecorded, Corrected, PendingApproval, PendingCompensation, Recorded

### Community 134 - ".ValidateIdTokenAsync"
Cohesion: 0.40
Nodes (4): GroupAccessLevel, GroupAdmin, Staff, IGroupAccessService

### Community 136 - "GetMyNotificationsQuery"
Cohesion: 0.33
Nodes (6): PaymobWebhookOutcome, Accepted, Duplicate, Ignored, InvalidPayload, InvalidSignature

### Community 137 - "ListTeacherRequestsQuery"
Cohesion: 0.33
Nodes (6): MailSettings, DisplayName, Host, Mail, Password, Port

### Community 138 - ".ValidatePayment"
Cohesion: 0.40
Nodes (3): TeacherERP.Application.Features.Students, StudentErrors, StudentReasonRequest

### Community 140 - "GroupSlotInput"
Cohesion: 0.40
Nodes (4): CloudinarySettings, ApiKey, ApiSecret, CloudName

### Community 141 - "AttendanceOutcome"
Cohesion: 0.40
Nodes (5): JwtSettings, Audience, ExpiryMinutes, Issuer, Secret

### Community 143 - "StudentStatus"
Cohesion: 0.50
Nodes (4): StudentStatus, Active, Blacklisted, Suspended

## Knowledge Gaps
- **594 isolated node(s):** `InstaPayLink`, `QrImage`, `Caption`, `File`, `Kind` (+589 more)
  These have ≤1 connection - possible missing edges. (Counts symbols only; 869 node(s) total have ≤1 connection when file, concept and rationale nodes are included.)
- **30 thin communities (<3 nodes) omitted from report** — run `graphify query` to explore isolated nodes.

## Suggested Questions
_Questions this graph is uniquely positioned to answer:_

- **Why does `Result` connect `TeacherERP.API` to `TeacherERP.Domain.Shared`, `.Resolve`, `TimeSpan`, `CreateAssistantCommandHandler`, `Session`, `IRequestHandler`, `List`, `.AddInfrastructureServices`, `IGroupSessionRepository`, `AttendanceRecord`, `AppDbContext`, `Center`, `.SeedAsync`, `EnrollmentRequestStatus`, `TenantSettings`, `Error`, `AuditLog`, `ICenterRepository`, `.ValidateIdTokenAsync`, `CompleteProfileValidator`, `TeacherERP.Application.Features.Team`, `SetFinancialDelegationCommandHandler.cs`, `RevokeSessionCommand`, `CancelSessionCommandHandler.cs`, `LoggingBehavior.cs`, `CachedRegistrationData`, `.TryHandleAsync`, `UpdateGroupValidator`, `AddExtraSessionValidator.cs`, `BulkShiftSessionsValidator.cs`, `BlacklistStudentValidator.cs`, `UnblacklistStudentValidator.cs`, `SetFinancialDelegationValidator.cs`, `ResetPasswordValidator.cs`, `VerifyResetOtpValidator.cs`, `VerifyOtpValidator.cs`, `TeacherERP.Application.Features.Schedule.GetSessions`, `RejectRequestValidator.cs`, `GroupScanValidator.cs`, `RequestGroupAdminChangeValidator.cs`, `Updatetenantprofilevalidator.cs`, `RefreshTokenValidator.cs`, `CreateStudentValidator.cs`, `HangfireBasicAuthFilter`, `ForwardedHeadersExtensions.cs`?**
  _High betweenness centrality (0.072) - this node is a cross-community bridge._
- **What connects `InstaPayLink`, `QrImage`, `Caption` to the rest of the system?**
  _594 weakly-connected nodes found - possible documentation gaps or missing edges._
- **Should `TeacherERP.Domain.Shared` be split into smaller, more focused modules?**
  _Cohesion score 0.02804499038034631 - nodes in this community are weakly interconnected._
- **Why does `TeacherERP.Domain.Entities` connect `system` to `PaymobWebhookOutcome`, `IActionResult`, `TimeSpan`, `system_text`, `Guid`, `CreateAssistantCommandHandler`, `IRequestHandler`, `List`, `DateTime`, `ApplicationUser`, `IGroupSessionRepository`, `.Handle`, `IUserAccountService`, `Transaction`, `PasswordController.cs`, `OtpService`, `EnrollmentRequestStatus`, `GroupEnrollment`, `TeacherERP.Application.Features.Users.Commands.Auth.Login`, `fluentvalidation`, `TenantSettings`, `.Handle`, `AuthResponseDto`, `Error`, `SessionValidator`, `.ResolveAsync`, `ResetGrantService`, `.HandleAsync`, `RevokeSessionCommand`, `HangfireBasicAuthFilter`, `GetMySessionsQueryHandler`, `Centerdto.cs`, `TeacherERP.Application.Features.Students`, `UpdateGroupValidator`, `.RequiresProfileCompletion`, `UnblacklistStudentValidator.cs`, `SetFinancialDelegationValidator.cs`, `ProfileAuthorizationResultHandler.cs`, `IdentityService.cs`, `RefreshTokenValidator.cs`, `ProfileAuthorizationResultHandler.cs`?**
  _High betweenness centrality (0.063) - this node is a cross-community bridge._
- **Should `IActionResult` be split into smaller, more focused modules?**
  _Cohesion score 0.01838931955211025 - nodes in this community are weakly interconnected._
- **Why does `TeacherERP.Application.Interfaces` connect `system` to `PaymobWebhookOutcome`, `IActionResult`, `TeacherERP.Application.Interfaces`, `TimeSpan`, `system_text`, `Guid`, `.ValidateIdTokenAsync`, `GetMyEnrollmentsQuery`, `Session`, `List`, `DateTime`, `ApplicationUser`, `IGroupSessionRepository`, `AppDbContext`, `Center`, `.Handle`, `http`, `ICurrentTenantAccessor`, `EnrollmentRequestStatus`, `Error`, `SessionManager`, `GetSessionsQueryHandler`, `SetFinancialDelegationCommandHandler.cs`, `TeacherERP.Application.Features.Students`, `UpdateGroupValidator`, `BlacklistStudentValidator.cs`, `UnblacklistStudentValidator.cs`, `SetFinancialDelegationValidator.cs`, `VerifyOtpValidator.cs`, `RemoveAssistantValidator.cs`, `ResendWelcomeValidator.cs`, `CreateStudentValidator.cs`?**
  _High betweenness centrality (0.054) - this node is a cross-community bridge._
- **Should `Task` be split into smaller, more focused modules?**
  _Cohesion score 0.045057471264367814 - nodes in this community are weakly interconnected._