using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Centers
{
    public static class CenterErrors
    {
        // نفس الخطأ لسنتر غير موجود أو لا أملكه، حتى لا نكشف وجود سناتر
        public static readonly Error NotFound = new("Center.NotFound", "The center was not found.");
        public static readonly Error TeacherSpaceNotFound = new("Center.TeacherSpaceNotFound", "The teacher space was not found or is inactive.");
        public static readonly Error LocationRequired = new("Center.LocationRequired", "Latitude and longitude are required.");
        public static readonly Error LocationInvalid = new("Center.LocationInvalid", "Latitude or longitude is outside its valid range.");
        public static readonly Error GeofenceRadiusInvalid = new("Center.GeofenceRadiusInvalid", "Geofence radius must be between 10 and 500 meters.");
        public static readonly Error LocationAlreadyOwned = new("Center.LocationAlreadyOwned", "You already own a center at this location.");
        public static readonly Error AlreadyMember = new("Center.AlreadyMember", "You are already active in this center.");
        public static readonly Error JoinRequestPending = new("Center.JoinRequestPending", "A join request is already pending for this center.");
        public static readonly Error JoinRequestLimitReached = new("Center.JoinRequestLimitReached", "You can have pending requests for up to three different centers.");
        public static readonly Error JoinRequestNotFound = new("Center.JoinRequestNotFound", "The join request was not found.");
        public static readonly Error JoinRequestNotPending = new("Center.JoinRequestNotPending", "The join request is no longer pending.");
        public static readonly Error JoinRequestResendTooSoon = new("Center.JoinRequestResendTooSoon", "A reminder can be sent once every 24 hours for this center.");
        public static readonly Error InvitationNotFound = new("Center.InvitationNotFound", "The invitation was not found.");
        public static readonly Error InvitationExpired = new("Center.InvitationExpired", "The invitation has expired.");
        public static readonly Error InvitationAlreadyUsed = new("Center.InvitationAlreadyUsed", "The invitation has already been used.");
        public static readonly Error MissingOwner = new("Center.MissingOwner", "The center owner could not be found.");
    }
}
