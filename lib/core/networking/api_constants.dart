class ApiConstants {
  static const String baseUrl = 'https://smart-teachererp.runasp.net/api/';

  // Auth Endpoints
  static const String registerStudent = 'Users/students';
  static const String verifyOtp = 'Users/verify-otp';
  static const String login = 'Auth/login';

  // Secure Storage Keys
  static const String tokenKey = 'access_token';
  static const String refreshTokenKey = 'refresh_token';
  static const String userRoleKey = 'user_role';
}
