import 'dart:developer';
import 'package:jwt_decoder/jwt_decoder.dart';
import '../storage/secure_storage_helper.dart';

enum SessionStatus { valid, invalid }

class SessionManager {
  String? _cachedUserId;
  String? _cachedRole;
  String? _cachedEmail;

  String get userId => _cachedUserId ?? '';
  String get userRole => _cachedRole ?? '';
  String get userEmail => _cachedEmail ?? '';

  Future<SessionStatus> validateSession() async {
    try {
      final accessToken = await SecureStorageHelper.getAccessToken();

      if (accessToken == null) {
        return SessionStatus.invalid;
      }

      // فحص هل انتهت مدة التوكن باستخدام jwt_decoder
      final bool isExpired = JwtDecoder.isExpired(accessToken);

      if (!isExpired) {
        final payload = JwtDecoder.decode(accessToken);
        _extractClaims(payload);
        return SessionStatus.valid;
      }

      // لو انتهى، نحاول استخدام الـ refreshToken
      final refreshToken = await SecureStorageHelper.getRefreshToken();
      if (refreshToken != null) {
        return SessionStatus
            .valid; // سيقوم الـ ApiService بتجديده تلقائياً في أول Request
      }

      await SecureStorageHelper.clearAll();
      return SessionStatus.invalid;
    } catch (e) {
      log("❌ خطأ أثناء فحص الجلسة: $e");
      return SessionStatus.invalid;
    }
  }

  Future<void> saveUserFromToken({
    required String accessToken,
    required String refreshToken,
  }) async {
    final payload = JwtDecoder.decode(accessToken);

    final userId = (payload['nameid'] ?? payload['sub'] ?? '').toString();
    final email = (payload['unique_name'] ?? payload['email'] ?? '').toString();
    final role = (payload['role'] ?? payload['Role'] ?? 'Student').toString();

    _cachedUserId = userId;
    _cachedEmail = email;
    _cachedRole = role;

    await SecureStorageHelper.saveUserData(
      accessToken: accessToken,
      refreshToken: refreshToken,
      role: role,
      userId: userId,
      email: email,
    );
  }

  void _extractClaims(Map<String, dynamic> payload) {
    _cachedUserId = (payload['nameid'] ?? payload['sub'] ?? '').toString();
    _cachedEmail = (payload['unique_name'] ?? payload['email'] ?? '')
        .toString();
    _cachedRole = (payload['role'] ?? payload['Role'] ?? 'Student').toString();
  }
}
