import 'package:flutter_secure_storage/flutter_secure_storage.dart';
import '../networking/api_constants.dart';

class SecureStorageHelper {
  static const FlutterSecureStorage _storage = FlutterSecureStorage(
    aOptions: AndroidOptions(resetOnError: true),
  );

  static const String _userIdKey = 'user_id';
  static const String _userNameKey = 'user_name';
  static const String _userEmailKey = 'user_email';

  static Future<String?> getAccessToken() async =>
      await _storage.read(key: ApiConstants.tokenKey);

  static Future<String?> getRefreshToken() async =>
      await _storage.read(key: ApiConstants.refreshTokenKey);

  static Future<String?> getUserRole() async =>
      await _storage.read(key: ApiConstants.userRoleKey);

  static Future<String?> getUserId() async =>
      await _storage.read(key: _userIdKey);

  static Future<String?> getUserName() async =>
      await _storage.read(key: _userNameKey);

  static Future<String?> getUserEmail() async =>
      await _storage.read(key: _userEmailKey);

  static Future<void> saveUserData({
    required String accessToken,
    required String refreshToken,
    required String role,
    required String userId,
    required String email,
    String? name,
  }) async {
    await _storage.write(key: ApiConstants.tokenKey, value: accessToken);
    await _storage.write(
      key: ApiConstants.refreshTokenKey,
      value: refreshToken,
    );
    await _storage.write(key: ApiConstants.userRoleKey, value: role);
    await _storage.write(key: _userIdKey, value: userId);
    await _storage.write(key: _userEmailKey, value: email);
    if (name != null) {
      await _storage.write(key: _userNameKey, value: name);
    }
  }

  static Future<void> updateTokens({
    required String newAccessToken,
    required String newRefreshToken,
  }) async {
    await _storage.write(key: ApiConstants.tokenKey, value: newAccessToken);
    await _storage.write(
      key: ApiConstants.refreshTokenKey,
      value: newRefreshToken,
    );
  }

  static Future<void> clearAll() async => await _storage.deleteAll();
}
