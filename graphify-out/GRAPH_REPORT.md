# Graph Report - TeacherERP  (2026-11-23)

## Corpus Check
- cluster-only mode — file stats not available

## Summary
- 4543 nodes · 17060 edges · 139 communities (110 shown, 29 thin omitted)
- Extraction: 90% EXTRACTED · 10% INFERRED · 0% AMBIGUOUS · INFERRED: 1667 edges (avg confidence: 0.85)
- Token cost: 0 input · 0 output

## Graph Freshness
- Built from commit: `f372ba0a`
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
- PaymobWebhookOutcome
- MailSettings
- TeacherERP.Application.Features.Students
- HangfireBasicAuthFilter
- .Resolve
- PreviewTeacherRequestQuery
- .ValidateIdTokenAsync
- GetMyEnrollmentsQuery
- GetMySessionsQueryHandler
- PaymobCallbackVerifier

## God Nodes (most connected - your core abstractions)
1. `Result` - 387 edges
2. `TeacherERP.Application.Interfaces` - 309 edges
3. `TeacherERP.Domain.Shared` - 264 edges
4. `TeacherERP.Domain.Entities` - 240 edges
5. `AppDbContext` - 111 edges
6. `ApplicationUser` - 88 edges
7. `Group` - 61 edges
8. `TeacherERP.Infrastructure.Data` - 59 edges
9. `TeacherERP.Infrastructure.Migrations` - 58 edges
10. `Transaction` - 49 edges

## Surprising Connections (you probably didn't know these)
- `WalkInApprovalTests` --calls--> `TenantContext`  [INFERRED]
  TeacherERP.Tests/Unit/WalkInLocationTests.cs → TeacherERP.Application/Interfaces/Icurrenttenantaccessor.cs
- `BulkCancelSessionsCommandHandlerTests` --calls--> `TenantContext`  [INFERRED]
  TeacherERP.Tests/Unit/CancelSessionHandlersTests.cs → TeacherERP.Application/Interfaces/Icurrenttenantaccessor.cs
- `CancelSessionCommandHandlerTests` --calls--> `TenantContext`  [INFERRED]
  TeacherERP.Tests/Unit/CancelSessionHandlersTests.cs → TeacherERP.Application/Interfaces/Icurrenttenantaccessor.cs
- `TeamErrors` --references--> `Error`  [EXTRACTED]
  TeacherERP.Application/Features/Team/TeamErrors.cs → TeacherERP.Domain/Shared/Error.cs
- `CustomWebApplicationFactory` --calls--> `AppDbContext`  [INFERRED]
  TeacherERP.Tests/Integration/CustomWebApplicationFactory.cs → TeacherERP.Infrastructure/Data/AppDbContext.cs

## Import Cycles
- None detected.

## Communities (139 total, 29 thin omitted)

### Community 0 - "TeacherERP.Domain.Shared"
Cohesion: 0.03
Nodes (24): MoveStudentRequest, RequestPlatformCommissionCollection, SettlePlatformCommissionRequest, GroupAdminChangeResultDto, RequestGroupAdminChangeCommand, RequestGroupAdminChangeValidator, MoveSessionRequest, AttendanceSettingsInfo (+16 more)

### Community 1 - "IActionResult"
Cohesion: 0.04
Nodes (27): AdministrationController, AttendanceController, AuditController, AuthController, BaseApiController, CentersController, DashboardController, DirectoryController (+19 more)

### Community 2 - "Task"
Cohesion: 0.02
Nodes (43): TeacherERP.Application.Features.Users.Commands.Auth.GoogleSignIn, TeacherERP.Application.Features.TeacherRequests.Review, TeacherERP.Application.Features.Centers.JoinRequests, TeacherERP.Application.Features.Finance.UpdateTeacherPaymentSettings, TeacherERP.Application.Features.SessionContent.UploadSessionAttachment, TeacherERP.Application.Features.Profile.GetMyProfile, TeacherERP.Application.Features.Users.Commands.Auth.Login, TeacherERP.Application.Features.Students.UnblacklistStudent (+35 more)

### Community 3 - "TeacherERP.Application.Interfaces"
Cohesion: 0.03
Nodes (19): TeacherERP.Application.Enrollment, TeacherERP.API.Middlewares, TeacherERP.Application, TeacherERP.Infrastructure.Settings, TeacherERP.Application.Absence, TeacherERP.Infrastructure, TeacherERP.Application.Sessions, TeacherERP.Application.Features.Users.Commands.Auth.ForgotPassword (+11 more)

### Community 4 - "system"
Cohesion: 0.03
Nodes (39): CenterMembershipPolicy, MyEnrollmentDto, UpdateGroupScheduleRequest, SessionDto, RescheduleSessionRequest, SessionTimeDto, PlatformCommissionCollectionRequestResult, PlatformCommissionSettlementResult (+31 more)

### Community 5 - "TimeSpan"
Cohesion: 0.03
Nodes (18): TeacherERP.Application.Features.Schedule.GetSessions, TeacherERP.Application.Features.Enrollment.GetMyEnrollments, TeacherERP.Application.Features.Groups.GetGroups, TeacherERP.Application.Features.Audit, TeacherERP.Application.Features.Directory.SearchDirectory, TeacherERP.Application.Features.Finance.GetPendingInstaPayReceipts, TeacherERP.Application.Features.SessionContent.GetSessionAttachments, TeacherERP.Application.Features.Attendance.GetPendingCompensations (+10 more)

### Community 6 - "system_text"
Cohesion: 0.04
Nodes (31): TeacherERP.Application.Features.Attendance.RejectCompensation, TeacherERP.Application.Features.GroupAdmins.GetGroupAdminRequests, TeacherERP.Application.Features.GroupAdmins.GetMyGroupAdmin, TeacherERP.Application.Features.Students.GetStudentDetails, TeacherERP.Application.Features.Finance.GetPaymentOptions, TeacherERP.Application.Features.Students.GetWeeklySchedule, TeacherERP.Application.Features.Finance.ApproveInstaPayReceipt, TeacherERP.Application.Features.Students.GetStudentCard (+23 more)

### Community 7 - "Guid"
Cohesion: 0.05
Nodes (9): TeacherERP.Application.SessionContent, TeacherERP.API.Security, TeacherERP.API.Controllers, TeacherERP.API.Extensions, UpdateGroupJoinCodesRequest, ResolvePaymobAdjustmentBody, UpdatePlatformCommissionSettingsRequest, RateLimitPolicies (+1 more)

### Community 8 - "CreateAssistantCommandHandler"
Cohesion: 0.09
Nodes (8): CompensationStaffIntegrationTests, DashboardIntegrationTests, FinanceAuthorizationIntegrationTests, ScheduleOperationsIntegrationTests, SessionMoveIntegrationTests, StudentExperienceIntegrationTests, StudentTransferIntegrationTests, TestData

### Community 9 - ".AddApplicationServices"
Cohesion: 0.09
Nodes (60): AbsenceService, AttendanceRecorder, CenterProvisioningService, ApproveCompensationCommandHandler, GetPendingCompensationsQueryHandler, GetSessionAttendanceQueryHandler, ManualAttendanceCommandHandler, RejectCompensationCommandHandler (+52 more)

### Community 10 - "Result"
Cohesion: 0.04
Nodes (68): PagedRequestRules, ApproveCompensationCommand, GetAttendanceSettingsQuery, RegenerateStaticCodeCommand, RejectCompensationCommand, ScanAttendanceCommand, ApproveCenterJoinRequestCommand, ApproveCenterJoinRequestValidator (+60 more)

### Community 11 - "Session"
Cohesion: 0.06
Nodes (16): TeacherPlatformCommissionsController, CenterJoinRequestListItem, JoinCenterCommand, StudentTimelineAttachmentDto, StudentTimelineSessionDto, AuditLogItem, EnrollmentRequestListItem, PlatformCommissionItem (+8 more)

### Community 12 - "IRequestHandler"
Cohesion: 0.08
Nodes (6): ScheduleConflictItem, ScheduleSlotCandidate, SessionMoveBlockers, GroupSessionRepository, BulkCancelSessionsCommandHandlerTests, CancelSessionCommandHandlerTests

### Community 14 - "DateTime"
Cohesion: 0.04
Nodes (15): TeacherERP.Application.Features.TeacherRequests.Submit, TeacherERP.Application.Features.Users.Commands.VerifyOtp, TeacherERP.Application.Features.Administration, TeacherERP.Application.Features.TeacherRequests.Verify, TeacherERP.Application.Features.Administration.CreateCenter, TeacherERP.Application.Features.Administration.CreateTeacher, TeacherERP.Application.Features.Team.CreateAssistant, TeacherERP.Application.Features.TeacherRequests.Admin (+7 more)

### Community 16 - "Group"
Cohesion: 0.04
Nodes (32): IStudentTimelineRepository, AppDbContext, AttendanceRecords, AuditLogs, CenterInvitations, CenterJoinRequests, Centers, CenterTeachers (+24 more)

### Community 17 - "ApplicationUser"
Cohesion: 0.04
Nodes (26): TeacherERP.Application.Features.Attendance, TeacherERP.Application.Features.Finance.GetMyPaymentOptions, TeacherERP.Application.Features.Finance.CreatePaymobIntent, TeacherERP.Application.Features.Notifications, TeacherERP.Application.Files, TeacherERP.Application.Features.SessionContent, TeacherERP.Application.Finance, TeacherERP.Application.Features.Enrollment (+18 more)

### Community 18 - "IGroupSessionRepository"
Cohesion: 0.05
Nodes (34): TenantMember, CanManageFinance, ConsecutiveAbsences, CurrentBalance, HasFinancialDelegation, Role, Status, Tenant (+26 more)

### Community 20 - "AttendanceRecord"
Cohesion: 0.07
Nodes (34): RejectCenterJoinRequestCommand, RejectCenterJoinRequestValidator, CancelRequestCommand, CancelRequestCommandHandler, RejectInstaPayReceiptCommand, RejectInstaPayReceiptCommandHandler, GetNotificationWhatsAppShareQuery, NotificationWhatsAppShareDto (+26 more)

### Community 21 - "AppDbContext"
Cohesion: 0.06
Nodes (34): GetPendingCompensationsQuery, GetPendingCompensationsValidator, GetSessionAttendanceQuery, GetSessionAttendanceValidator, GetAuditLogQuery, GetAuditLogQueryHandler, GetAuditLogValidator, GetCenterJoinRequestsQuery (+26 more)

### Community 22 - "Center"
Cohesion: 0.05
Nodes (14): TeacherERP.Application.Features.Users.Commands.Auth.SetPassword, TeacherERP.Application.Features.Schedule.BulkCancelSessions, TeacherERP.Tests.Unit, TeacherERP.Application.Features.Enrollment.ApproveRequest, TeacherERP.Application.Scheduling, TeacherERP.Application.Attendance, TeacherERP.Application.Features.Attendance.ScanAttendance, TeacherERP.Application.Features.Enrollment.MoveStudent (+6 more)

### Community 23 - "LoginSession"
Cohesion: 0.10
Nodes (5): AttendanceCodeService, CenterMembershipIntegrationTests, DynamicQrIntegrationTests, AttendanceCodeServiceTests, CenterMembershipTests

### Community 24 - ".Handle"
Cohesion: 0.07
Nodes (28): GlobalExceptionHandler, AbsenceProcessingJob, AbsenceProcessor, RequestGroupCommandHandler, BlacklistStudentCommandHandler, AssistantDto, CreateAssistantCommand, CreateAssistantCommandHandler (+20 more)

### Community 25 - "http"
Cohesion: 0.10
Nodes (27): AttendanceSettingsDto, GetAttendanceSettingsQueryHandler, RegenerateStaticCodeCommandHandler, UpdateAttendanceSettingsCommand, UpdateAttendanceSettingsCommandHandler, UpdateAttendanceSettingsValidator, GetGroupSessionAttendanceQueryHandler, GetMyGroupAdminQuery (+19 more)

### Community 27 - ".SeedAsync"
Cohesion: 0.07
Nodes (20): GoogleSignInCommand, GoogleSignInCommandHandler, GoogleSignInValidator, IExternalLoginService, IUserAccountService, ApplicationUser, Attendances, DateOfBirth (+12 more)

### Community 28 - "ICurrentTenantAccessor"
Cohesion: 0.06
Nodes (18): LoggingBehavior, ValidationBehavior, ProfileRequirement, Session, AbsencesProcessedAt, Attendances, BoardImageUrl, CancelReason (+10 more)

### Community 29 - "IUserAccountService"
Cohesion: 0.09
Nodes (11): ApproveGroupAdminRequestCommand, GetGroupAdminsQuery, GetGroupAdminsQueryHandler, GroupAdminDto, IAbsenceRepository, RunningSessionInfo, PlatformCommissionSummary, GroupAdminItem (+3 more)

### Community 30 - "Transaction"
Cohesion: 0.10
Nodes (4): UserTime, IEmailService, EmailService, EmailTemplates

### Community 31 - "GroupDto"
Cohesion: 0.08
Nodes (20): Group, Capacity, CenterId, DynamicJoinEnabled, DynamicJoinSecret, EducationStage, GradeLevel, GradeNumber (+12 more)

### Community 32 - "PasswordController.cs"
Cohesion: 0.05
Nodes (37): AuditLogDto, AuditAction, AttendanceCorrected, CashPaymentCollected, CompensationApproved, CompensationRejected, FinancialDelegationGranted, FinancialDelegationRevoked (+29 more)

### Community 33 - "MembershipStatus"
Cohesion: 0.07
Nodes (37): CloudinaryDotNet (1.29.3), coverlet.collector (6.0.4), FluentValidation (12.1.1), FluentValidation.DependencyInjectionExtensions (12.1.1), Google.Apis.Auth (1.77.0), Hangfire.AspNetCore (1.8.25), Hangfire.Core (1.8.25), Hangfire.Dashboard.BasicAuthorization (1.0.2) (+29 more)

### Community 35 - ".SaveChangesAsync"
Cohesion: 0.12
Nodes (3): AdministrationIntegrationTests, TeacherRequestsIntegrationTests, SeededTeacher

### Community 36 - "TeacherERP.Application.Features.Groups"
Cohesion: 0.11
Nodes (21): ApproveInstaPayReceiptCommand, ApproveInstaPayReceiptCommandHandler, CollectCashPaymentCommand, CollectCashPaymentCommandValidator, GetMyPaymentOptionsQuery, GetMyPaymentOptionsQueryHandler, GetMyPaymentOptionsValidator, GetPaymentOptionsQuery (+13 more)

### Community 37 - "GuardianRelation"
Cohesion: 0.09
Nodes (19): GetPendingInstaPayReceiptsQuery, GetPendingInstaPayReceiptsValidator, GetPendingInstaPayReceiptsQueryHandler, SubmitInstaPayReceiptCommand, SubmitInstaPayReceiptCommandHandler, SubmitInstaPayReceiptValidator, UploadSessionAttachmentCommand, UploadSessionAttachmentValidator (+11 more)

### Community 38 - ".GetByIdAsync"
Cohesion: 0.11
Nodes (20): CenterTeacherLinkDto, LinkTeacherToCenterCommand, LinkTeacherToCenterCommandHandler, LinkTeacherToCenterValidator, CenterTeacher, Center, CenterId, Origin (+12 more)

### Community 39 - "ITenantRepository"
Cohesion: 0.12
Nodes (14): SubmitTeacherRequestCommand, SubmitTeacherRequestCommandHandler, SubmitTeacherRequestValidator, TeacherRequestOtpDto, CachedRegistrationData, Email, FullName, Password (+6 more)

### Community 40 - "EnrollmentRequestStatus"
Cohesion: 0.13
Nodes (6): GroupJoinCodeService, RequestGroupByCodeCommand, RequestGroupByCodeCommandHandler, RequestGroupByCodeValidator, IGroupJoinCodeService, GroupJoinCodeServiceTests

### Community 41 - "GroupEnrollment"
Cohesion: 0.08
Nodes (22): TeacherRequestRules, ReviewerKind, Administrator, Teacher, TeacherAccessRequest, CreatedUserId, DecisionNote, Email (+14 more)

### Community 42 - "AbstractValidator"
Cohesion: 0.11
Nodes (11): CustomWebApplicationFactory, FileStorage, PaymobGateway, PaymobSettings, GroupGradeIntegrationTests, GroupPricingIntegrationTests, ManualAttendanceIntegrationTests, PasswordIntegrationTests (+3 more)

### Community 43 - "TeacherERP.Application.Features.Users.Commands.Auth.Login"
Cohesion: 0.07
Nodes (29): Notification, IsRead, Message, RecipientUserId, RelatedId, TenantId, Type, NotificationType (+21 more)

### Community 44 - "fluentvalidation"
Cohesion: 0.11
Nodes (17): GetStudentDetailsQuery, GetStudentDetailsQueryHandler, StudentActiveGroupDto, StudentDetailsDto, StudentListDto, GetTeamQuery, GetTeamQueryHandler, TeamMemberDto (+9 more)

### Community 46 - "TenantSettings"
Cohesion: 0.11
Nodes (19): CompleteProfileCommand, CompleteProfileCommandHandler, CompleteProfileValidator, GetMyProfileQuery, GetMyProfileQueryHandler, GuardianDto, ProfileDto, GetMySessionsQuery (+11 more)

### Community 47 - ".Handle"
Cohesion: 0.08
Nodes (11): TeacherERP.Application.Features.Team.GetTeam, TeacherERP.Application.Features.Groups.GetGroup, TeacherERP.Application.Features.Tenants, TeacherERP.Application.Features.Team.RemoveAssistant, TeacherERP.Application.Features.Attendance.GetDisplayCode, TeacherERP.Application.Features.Team.SetAssistantSuspension, TeacherERP.Application.Features.Team.ResendWelcome, RemoveAssistantCommand (+3 more)

### Community 48 - "AuthResponseDto"
Cohesion: 0.13
Nodes (18): AdminScanCommand, AdminScanCommandHandler, AdminScanValidator, ManualAttendanceCommand, ManualAttendanceValidator, ScanAttendanceCommandHandler, ScanAttendanceValidator, GroupScanCommand (+10 more)

### Community 49 - "Error"
Cohesion: 0.14
Nodes (14): FinancialSessionInfo, CashPaymentRequest, FinancialObligation, Amount, GroupId, GroupName, IsSettled, OutstandingAmount (+6 more)

### Community 50 - "AuditLog"
Cohesion: 0.08
Nodes (21): LoginSession, AbsoluteExpiresAt, DeviceId, DeviceName, LastRefreshedAt, PreviousRefreshTokenHash, RefreshExpiresAt, RefreshTokenHash (+13 more)

### Community 51 - "BaseEntity"
Cohesion: 0.08
Nodes (24): EnrollmentRequest, DecidedAt, DecidedByUserId, DecisionNote, Group, GroupId, SourceLatitude, SourceLongitude (+16 more)

### Community 52 - "GroupSessionRepository"
Cohesion: 0.08
Nodes (22): AttendanceRecord, CompensatesSessionId, RecordedByUserId, ScannedLatitude, ScannedLongitude, ScannedVia, Session, SessionId (+14 more)

### Community 53 - "SessionValidator"
Cohesion: 0.09
Nodes (7): TeacherERP.Application.Features.Groups, TeacherERP.Application.Features.Profile.CompleteProfile, TeacherERP.Application.Features.Groups.UpdateGroup, TeacherERP.Application.Features.Profile, TeacherERP.Application.Features.Attendance.ManualAttendance, TeacherERP.Domain.ValueObjects, TeacherERP.Application.Features.Groups.CreateGroup

### Community 54 - ".ResolveAsync"
Cohesion: 0.09
Nodes (16): CenterInvitationToken, CenterInvitationDto, CreateCenterInvitationCommand, CreateCenterInvitationValidator, CenterInvitation, AcceptedAtUtc, AcceptedByTenantId, CenterId (+8 more)

### Community 55 - "ResetGrantService"
Cohesion: 0.09
Nodes (19): PlatformCommission, Amount, CollectionRequestedAtUtc, CollectionRequestedByUserId, GroupId, IsSettled, SessionId, SettledAtUtc (+11 more)

### Community 56 - ".HandleAsync"
Cohesion: 0.12
Nodes (8): Center, Address, GeofenceRadiusMeters, Latitude, Longitude, Name, OwnerUserId, CenterRepository

### Community 57 - ".AddApiRateLimiting"
Cohesion: 0.13
Nodes (12): ResetPasswordCommand, ResetPasswordCommandHandler, ResetPasswordValidator, ResetTokenDto, VerifyResetOtpCommand, VerifyResetOtpCommandHandler, VerifyResetOtpValidator, IResetGrantService (+4 more)

### Community 58 - "TenantRole"
Cohesion: 0.09
Nodes (16): GroupSlotInput, IGroupSchedule, Slots, BaseEntity, CreatedAt, Id, UpdatedAt, GroupScheduleSlot (+8 more)

### Community 59 - "IServiceCollection"
Cohesion: 0.14
Nodes (5): IPasswordGenerator, TeacherProvisioningService, WelcomeEmailFailedException, PasswordGenerator, TeacherProvisioningServiceTests

### Community 60 - "ICenterRepository"
Cohesion: 0.15
Nodes (7): SessionJwtBearerEvents, AccessTokenResult, IJwtProvider, ISessionValidator, SessionManager, TokenHasher, JwtProvider

### Community 61 - ".ValidateIdTokenAsync"
Cohesion: 0.09
Nodes (21): PaymobAdjustmentResolution, PaymobAdjustmentReview, CommissionReversalAmount, Currency, DecisionNote, ProviderAmount, ProviderEventId, ReviewedAtUtc (+13 more)

### Community 62 - "CompleteProfileValidator"
Cohesion: 0.09
Nodes (13): MonthlySubscription, Amount, GroupId, Month, PaymentTransactionId, StudentId, TenantId, Year (+5 more)

### Community 63 - "SessionManager"
Cohesion: 0.18
Nodes (3): FinancialLedgerService, PaymobAdjustmentReviewService, IFinancialLedgerRepository

### Community 64 - "TeacherERP.Application.Features.Team"
Cohesion: 0.18
Nodes (7): TeacherRequestQrDto, TeacherRequestToken, VerifyTeacherRequestCommand, VerifyTeacherRequestCommandHandler, VerifyTeacherRequestValidator, TeacherRequestBasicsTests, VerifyTeacherRequestCommandHandlerTests

### Community 65 - "GetSessionsQueryHandler"
Cohesion: 0.12
Nodes (7): SnapshotAccessor, AddStructuredGroupGrades, SnapshotAccessor, AddSessionOriginalStartTime, SnapshotAccessor, AddGroupPricing, AppDbContextModelSnapshot

### Community 66 - "SetFinancialDelegationCommandHandler.cs"
Cohesion: 0.09
Nodes (23): ASPNETCORE_ENVIRONMENT, applicationUrl, commandName, dotnetRunMessages, environmentVariables, applicationUrl, commandName, dotnetRunMessages (+15 more)

### Community 67 - "RevokeSessionCommand"
Cohesion: 0.14
Nodes (4): NotificationService, INotificationRepository, NotificationShareItem, NotificationRepository

### Community 68 - "CancelSessionCommandHandler.cs"
Cohesion: 0.18
Nodes (9): TeacherERP.Application.Features.Users.Commands.Auth.ChangePassword, ChangePasswordCommand, ChangePasswordCommandHandler, ChangePasswordValidator, SetPasswordCommand, SetPasswordCommandHandler, SetPasswordValidator, ICredentialService (+1 more)

### Community 69 - "HangfireBasicAuthFilter"
Cohesion: 0.15
Nodes (8): PlatformCommissionSettingsService, IPlatformCommissionSettingsService, PlatformCommissionSettingsDto, PlatformCommissionSettings, MonthlySubscriptionAmount, PerSessionAmount, UpdatedAtUtc, UpdatedByUserId

### Community 70 - "GetMySessionsQueryHandler"
Cohesion: 0.17
Nodes (14): LogoutCommand, LogoutCommandHandler, LogoutValidator, ActivateAccountCommand, ActivateAccountCommandHandler, ActivateAccountValidator, AuthResponseDto, LoginCommand (+6 more)

### Community 71 - "Centerdto.cs"
Cohesion: 0.14
Nodes (15): AdminCenterDto, CreateCenterCommand, CreateCenterCommandHandler, CreateCenterValidator, CenterDto, CreateMyCenterCommand, CreateMyCenterCommandHandler, CreateMyCenterValidator (+7 more)

### Community 72 - "TeacherERP.Application.Features.Students"
Cohesion: 0.16
Nodes (10): TenantContext, TenantRole, Assistant, Owner, Student, CurrentTenantAccessor, Current, TenantId (+2 more)

### Community 73 - "LoggingBehavior.cs"
Cohesion: 0.15
Nodes (15): IPagedRequest, Page, PageSize, PagedResult, HasNext, HasPrevious, TotalPages, GetGroupAdminRequestsQuery (+7 more)

### Community 74 - "GeoDistance"
Cohesion: 0.12
Nodes (9): TeacherERP.Application.Features.Students, ApiAuthorizationResultHandler, GroupErrors, StudentErrors, StudentReasonRequest, TenantErrors, Error, Code (+1 more)

### Community 75 - "CachedRegistrationData"
Cohesion: 0.10
Nodes (16): PaymobSettings, ApiKey, BaseUrl, Currency, HmacNullValue, HmacSecret, InitializationLeaseSeconds, IntegrationIds (+8 more)

### Community 76 - ".TryHandleAsync"
Cohesion: 0.11
Nodes (15): GroupAdminRequest, DecidedAt, DecidedByUserId, DecisionNote, Group, GroupId, RequestedByUserId, Status (+7 more)

### Community 78 - ".RequiresProfileCompletion"
Cohesion: 0.20
Nodes (14): CreatedTeacherDto, ApproveTeacherRequestByAdminCommand, ApproveTeacherRequestByAdminCommandHandler, ApproveTeacherRequestCommand, ApproveTeacherRequestCommandHandler, ApproveTeacherRequestValidator, PreviewTeacherRequestQuery, PreviewTeacherRequestQueryHandler (+6 more)

### Community 79 - "Updatecentervalidator.cs"
Cohesion: 0.11
Nodes (13): CenterJoinRequest, CenterId, DecisionNote, LastNotifiedAtUtc, ReviewedAtUtc, ReviewedByUserId, Status, TenantId (+5 more)

### Community 80 - "AddExtraSessionValidator.cs"
Cohesion: 0.16
Nodes (8): CompletedProfileHandler, CompletedProfileRequirement, TenantHandler, TenantRequirement, AllowedRoles, RequiresFinancialDelegation, ProfileCompletionChecker, IProfileCompletionChecker

### Community 81 - "BulkCancelSessionsValidator.cs"
Cohesion: 0.12
Nodes (4): TeacherERP.Application.Centers, TeacherERP.Application.Features.Centers.CreateCenter, TeacherERP.Application.Features.Centers, TeacherERP.Application.Features.Centers.Invitations

### Community 82 - "BulkShiftSessionsValidator.cs"
Cohesion: 0.20
Nodes (8): IPaymobGateway, PaymobIntentCommandData, PaymobIntentData, PaymobGateway, TestPaymobGateway, CreateCount, FailNextWithCode, LastRequest

### Community 83 - "RescheduleSessionValidator.cs"
Cohesion: 0.17
Nodes (13): OtpPurpose, Activation, PasswordReset, Registration, TeacherRequest, IssueCounter, Count, OtpEntry (+5 more)

### Community 84 - "BlacklistStudentValidator.cs"
Cohesion: 0.11
Nodes (15): EnrollmentStatus, Active, Ended, GroupEnrollment, ApprovedByUserId, EndedAt, EnrolledAt, Group (+7 more)

### Community 85 - "UnblacklistStudentValidator.cs"
Cohesion: 0.11
Nodes (18): SessionAttachment, Caption, ContentType, FileName, Kind, PublicId, Session, SessionId (+10 more)

### Community 86 - "CreateAssistantValidator.cs"
Cohesion: 0.13
Nodes (13): TeacherERP.Application.Features.TeacherRequests, CachedTeacherRequestData, Email, FullName, Note, Subject, TimeZoneId, GetTeacherRequestStatusQuery (+5 more)

### Community 87 - "RemoveAssistantValidator.cs"
Cohesion: 0.16
Nodes (15): FinancialTransactionInfo, MonthlyCollectionItem, MonthlyCollectionReport, MonthlyCollectionTotals, MonthlyOutstandingDebtItem, MonthlyPaymentMethodSummary, TransactionStatus, Failed (+7 more)

### Community 90 - "GoogleSignInValidator.cs"
Cohesion: 0.12
Nodes (16): PaymentEvent, CurrentStatus, EventType, PayloadSha256, PreviousStatus, Provider, ProviderEventId, ProviderOccurredAtUtc (+8 more)

### Community 91 - "ResetPasswordValidator.cs"
Cohesion: 0.17
Nodes (9): UpdateGroupRequest, EducationStage, Preparatory, Primary, Secondary, Unspecified, SchoolGrade, GroupPricingTests (+1 more)

### Community 92 - "VerifyResetOtpValidator.cs"
Cohesion: 0.17
Nodes (9): FileNames, DetectedFileType, Jpeg, Pdf, Png, Unknown, Webp, FileSignature (+1 more)

### Community 93 - "CreateStudentValidator.cs"
Cohesion: 0.22
Nodes (13): CreatePaymobIntentBody, CreatePaymobIntentCommand, CreatePaymobIntentCommandValidator, CreatePaymobIntentCommandHandler, CreatePaymobPaymentRequest, IPaymobPaymentService, PaymobIntegrationKey, CaggOnlineNonShopify (+5 more)

### Community 94 - "VerifyOtpValidator.cs"
Cohesion: 0.27
Nodes (10): GetStudentWeeklyScheduleQuery, GetStudentWeeklyScheduleQueryHandler, StudentWeeklyGroupScheduleDto, StudentWeeklyScheduleDto, StudentWeeklyScheduleSlotDto, IStudentWeeklyScheduleRepository, StudentWeeklyScheduleData, StudentWeeklyScheduleGroupItem (+2 more)

### Community 95 - "ProfileAuthorizationResultHandler.cs"
Cohesion: 0.12
Nodes (14): GuardianInfo, Name, PhoneNumber, Relation, GuardianRelation, Aunt, Brother, Father (+6 more)

### Community 96 - "IdentityService.cs"
Cohesion: 0.25
Nodes (12): TeacherERP.Application.Features.Directory, DirectoryCenterDto, DirectoryCenterInfoDto, DirectoryGroupDto, DirectoryTeacherDto, TeacherProfileDto, GetTeacherProfileQuery, GetTeacherProfileQueryHandler (+4 more)

### Community 97 - "TeacherERP.Application.Features.Schedule.GetSessions"
Cohesion: 0.18
Nodes (4): PaymobGatewayTests, RecordingLogger, Messages, StubHandler

### Community 98 - "UpdateGroupValidator"
Cohesion: 0.15
Nodes (5): CenterJoinRequestNotifier, TeacherPaymentService, ICenterJoinRequestNotifier, IStoredFileCleanupJob, StoredFileCleanupJob

### Community 99 - "ManualAttendanceValidator.cs"
Cohesion: 0.23
Nodes (8): PageRequest, Skip, AdministratorDashboardDto, TeacherDashboardDto, GetAdministratorDashboardQuery, GetAdministratorDashboardQueryHandler, GetTeacherDashboardQuery, GetTeacherDashboardQueryHandler

### Community 100 - "Updatecentervalidator.cs"
Cohesion: 0.14
Nodes (12): Tenant, AutoApproveGroupAdminChanges, AutoApproveJoinRequests, IsListedInSearch, Name, OwnerId, Status, Subject (+4 more)

### Community 101 - "RejectRequestValidator.cs"
Cohesion: 0.16
Nodes (11): AttendanceMode, AdminOnly, DynamicScreenQR, StaticGlobalQR, TenantSettings, AutoApproveGroupAdminScans, AutoRegisterOnScan, Mode (+3 more)

### Community 104 - "RequestGroupAdminChangeValidator.cs"
Cohesion: 0.30
Nodes (6): CreateTeacherCommand, CreateTeacherCommandHandler, CreateTeacherValidator, ITeacherProvisioningService, CreateTeacherCommandHandlerTests, CreateTeacherValidatorTests

### Community 105 - "CancelSessionValidator.cs"
Cohesion: 0.26
Nodes (6): AdministratorDashboardOperationalStats, DashboardUpcomingSession, IDashboardRepository, TeacherDashboardOperationalStats, DashboardRepository, StudentRepository

### Community 106 - "LogoutValidator.cs"
Cohesion: 0.15
Nodes (12): UpdateTeacherPaymentSettingsForm, InstaPayLink, QrImage, UploadSessionAttachmentForm, Caption, File, Kind, SubmitInstaPayReceiptForm (+4 more)

### Community 107 - "BlacklistStudentValidator.cs"
Cohesion: 0.26
Nodes (4): MonthlyCollectionReportsController, TeacherMonthlyCollectionReportsController, MonthlyCollectionReportService, IMonthlyCollectionReportService

### Community 109 - "RemoveAssistantValidator.cs"
Cohesion: 0.36
Nodes (6): DirectoryCenterInfo, DirectoryRow, DirectorySearchPage, IDirectoryRepository, TeacherProfileData, DirectoryRepository

### Community 113 - "GoogleSignInValidator.cs"
Cohesion: 0.31
Nodes (7): EnrollmentRequestDto, MyEnrollmentsDto, GetMyEnrollmentsQuery, GetMyEnrollmentsQueryHandler, GetMyEnrollmentsValidator, RequestGroupCommand, RequestGroupValidator

### Community 115 - "CreateStudentValidator.cs"
Cohesion: 0.36
Nodes (4): PaymobAdjustmentReviewsController, PaymobAdjustmentReviewItem, IPaymobAdjustmentReviewService, ResolvePaymobAdjustmentRequest

### Community 116 - "ProfileAuthorizationResultHandler.cs"
Cohesion: 0.43
Nodes (4): DisplayCodeDto, GetDisplayCodeQuery, GetDisplayCodeQueryHandler, IAttendanceCodeService

### Community 117 - "IdentityService.cs"
Cohesion: 0.32
Nodes (5): GoogleUserInfo, IGoogleAuthService, GoogleAuthService, GoogleAuthSettings, ClientIds

### Community 119 - "MailSettings"
Cohesion: 0.29
Nodes (3): PaymobReturnController, PaymobWebhookController, RolesController

### Community 122 - "GetNotificationWhatsAppShareQuery"
Cohesion: 0.29
Nodes (5): CurrentUserService, SessionId, UserId, HttpUserTimeZoneAccessor, TimeZoneId

### Community 124 - "CloudinarySettings"
Cohesion: 0.33
Nodes (3): TeacherERP.Application.Features.Team, TeamErrors, TeamPolicy

### Community 125 - "JwtSettings"
Cohesion: 0.53
Nodes (4): ForgotPasswordCommand, ForgotPasswordCommandHandler, ForgotPasswordResponseDto, ForgotPasswordValidator

### Community 126 - ".TokenValidated"
Cohesion: 0.33
Nodes (6): PaymobWebhookOutcome, Accepted, Duplicate, Ignored, InvalidPayload, InvalidSignature

### Community 127 - ".Meters"
Cohesion: 0.33
Nodes (6): MailSettings, DisplayName, Host, Mail, Password, Port

### Community 133 - "PreviewTeacherRequestQuery"
Cohesion: 0.40
Nodes (4): CloudinarySettings, ApiKey, ApiSecret, CloudName

### Community 134 - ".ValidateIdTokenAsync"
Cohesion: 0.40
Nodes (5): JwtSettings, Audience, ExpiryMinutes, Issuer, Secret

## Knowledge Gaps
- **609 isolated node(s):** `MonthlyPrice`, `PerSessionPrice`, `AutoApproveGroupAdminChanges`, `AutoApproveJoinRequests`, `IsListedInSearch` (+604 more)
  These have ≤1 connection - possible missing edges. (Counts symbols only; 889 node(s) total have ≤1 connection when file, concept and rationale nodes are included.)
- **29 thin communities (<3 nodes) omitted from report** — run `graphify query` to explore isolated nodes.

## Suggested Questions
_Questions this graph is uniquely positioned to answer:_

- **Why does `TeacherERP.Application.Interfaces` connect `TimeSpan` to `TeacherERP.Domain.Shared`, `Task`, `TeacherERP.Application.Interfaces`, `HangfireBasicAuthFilter`, `.Resolve`, `system_text`, `Guid`, `List`, `DateTime`, `ApplicationUser`, `Center`, `.Handle`, `Transaction`, `TeacherERP.Application.Features.Groups`, `GuardianRelation`, `EnrollmentRequestStatus`, `fluentvalidation`, `TenantSettings`, `.Handle`, `AuthResponseDto`, `SessionValidator`, `.AddApiRateLimiting`, `IServiceCollection`, `ICenterRepository`, `HangfireBasicAuthFilter`, `AddExtraSessionValidator.cs`, `BulkCancelSessionsValidator.cs`, `RemoveAssistantValidator.cs`, `CreateStudentValidator.cs`, `VerifyOtpValidator.cs`, `UpdateGroupValidator`, `CancelSessionValidator.cs`, `RemoveAssistantValidator.cs`, `HangfireBasicAuthFilter`?**
  _High betweenness centrality (0.062) - this node is a cross-community bridge._
- **What connects `MonthlyPrice`, `PerSessionPrice`, `AutoApproveGroupAdminChanges` to the rest of the system?**
  _609 weakly-connected nodes found - possible documentation gaps or missing edges._
- **Should `TeacherERP.Domain.Shared` be split into smaller, more focused modules?**
  _Cohesion score 0.030028827674567583 - nodes in this community are weakly interconnected._
- **Why does `Result` connect `AttendanceRecord` to `TeacherERP.Domain.Shared`, `Task`, `TeacherERP.Application.Interfaces`, `.AddApplicationServices`, `Result`, `Session`, `IRequestHandler`, `DateTime`, `ApplicationUser`, `TeacherERP.API`, `AppDbContext`, `.Handle`, `.SeedAsync`, `TeacherERP.Application.Features.Groups`, `GuardianRelation`, `.GetByIdAsync`, `ITenantRepository`, `GroupEnrollment`, `fluentvalidation`, `TenantSettings`, `.Handle`, `AuthResponseDto`, `Error`, `.ResolveAsync`, `.HandleAsync`, `.AddApiRateLimiting`, `IServiceCollection`, `ICenterRepository`, `TeacherERP.Application.Features.Team`, `CancelSessionCommandHandler.cs`, `HangfireBasicAuthFilter`, `GetMySessionsQueryHandler`, `Centerdto.cs`, `.RequiresProfileCompletion`, `BulkShiftSessionsValidator.cs`, `RescheduleSessionValidator.cs`, `CreateAssistantValidator.cs`, `SetFinancialDelegationValidator.cs`, `CreateStudentValidator.cs`, `IdentityService.cs`, `UpdateGroupValidator`, `RequestGroupAdminChangeValidator.cs`, `BlacklistStudentValidator.cs`, `GoogleSignInValidator.cs`, `ProfileAuthorizationResultHandler.cs`, `IdentityService.cs`, `JwtSettings`?**
  _High betweenness centrality (0.055) - this node is a cross-community bridge._
- **Should `IActionResult` be split into smaller, more focused modules?**
  _Cohesion score 0.04008507853403141 - nodes in this community are weakly interconnected._
- **Why does `TeacherERP.Domain.Entities` connect `TimeSpan` to `TeacherERP.Domain.Shared`, `Task`, `TeacherERP.Application.Interfaces`, `system_text`, `Guid`, `List`, `DateTime`, `ApplicationUser`, `Center`, `ICurrentTenantAccessor`, `PasswordController.cs`, `.SaveChangesAsync`, `TeacherERP.Application.Features.Groups`, `.GetByIdAsync`, `GroupEnrollment`, `TeacherERP.Application.Features.Users.Commands.Auth.Login`, `TenantSettings`, `.Handle`, `AuthResponseDto`, `Error`, `AuditLog`, `BaseEntity`, `GroupSessionRepository`, `SessionValidator`, `.ResolveAsync`, `ResetGrantService`, `.HandleAsync`, `TenantRole`, `ICenterRepository`, `.ValidateIdTokenAsync`, `CompleteProfileValidator`, `HangfireBasicAuthFilter`, `.TryHandleAsync`, `Updatecentervalidator.cs`, `BulkCancelSessionsValidator.cs`, `BlacklistStudentValidator.cs`, `UnblacklistStudentValidator.cs`, `CreateAssistantValidator.cs`, `RemoveAssistantValidator.cs`, `GoogleSignInValidator.cs`, `CreateStudentValidator.cs`, `Updatecentervalidator.cs`, `RejectRequestValidator.cs`, `RemoveAssistantValidator.cs`, `HangfireBasicAuthFilter`?**
  _High betweenness centrality (0.047) - this node is a cross-community bridge._
- **Should `Task` be split into smaller, more focused modules?**
  _Cohesion score 0.01810790835181079 - nodes in this community are weakly interconnected._