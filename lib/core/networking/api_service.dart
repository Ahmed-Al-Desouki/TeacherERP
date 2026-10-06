import 'dart:convert';
import 'dart:developer';
import 'package:dio/dio.dart';
import '../storage/secure_storage_helper.dart';
import 'api_constants.dart';

class ApiService {
  final Dio _dio;

  ApiService({String? baseUrl})
    : _dio = Dio(
        BaseOptions(
          baseUrl: baseUrl ?? ApiConstants.baseUrl,
          connectTimeout: const Duration(seconds: 30),
          receiveTimeout: const Duration(seconds: 30),
          headers: {'Content-Type': 'application/json'},
        ),
      ) {
    _setupInterceptors();
  }

  void _setupInterceptors() {
    _dio.interceptors.add(
      QueuedInterceptorsWrapper(
        onRequest: (options, handler) async {
          // استثناء الـ Endpoints العامة التي لا تحتاج إلى Bearer Token
          final isPublicEndpoint =
              options.path.contains(ApiConstants.login) ||
              options.path.contains(ApiConstants.registerStudent) ||
              options.path.contains(ApiConstants.verifyOtp);

          if (!isPublicEndpoint) {
            final token = await SecureStorageHelper.getAccessToken();
            if (token != null) {
              options.headers['Authorization'] = 'Bearer $token';
            }
          }

          log('➡️ [REQUEST] ${options.method} ${options.uri}');
          return handler.next(options);
        },

        onResponse: (response, handler) {
          log(
            '✅ [RESPONSE] ${response.statusCode} | ${response.requestOptions.path}',
          );
          return handler.next(response);
        },

        onError: (DioException e, handler) async {
          log('❌ [ERROR] ${e.response?.statusCode} | ${e.requestOptions.path}');

          // معالجة تجديد الجلسة عند انتهاء صلاحية التوكن (401)
          if (e.response?.statusCode == 401 &&
              !e.requestOptions.path.contains(ApiConstants.login)) {
            log(
              "⚠️ Access Token انتهى، جاري محاولة التجديد عبر Refresh Token...",
            );

            try {
              final refreshToken = await SecureStorageHelper.getRefreshToken();
              final accessToken = await SecureStorageHelper.getAccessToken();

              if (refreshToken == null || accessToken == null) {
                return _handleSessionExpired(handler, e);
              }

              final refreshDio = Dio(
                BaseOptions(baseUrl: _dio.options.baseUrl),
              );

              // استدعاء endpoint تجديد التوكن بالـ Flat JSON
              final response = await refreshDio.post(
                'Auth/refresh-token',
                data: {
                  "accessToken": accessToken,
                  "refreshToken": refreshToken,
                },
              );

              if (response.statusCode == 200 && response.data != null) {
                final newAccess = response.data['accessToken'] as String;
                final newRefresh = response.data['refreshToken'] as String;

                await SecureStorageHelper.updateTokens(
                  newAccessToken: newAccess,
                  newRefreshToken: newRefresh,
                );

                log("✅ تم تجديد الـ Token بنجاح وإعادة تنفيذ الطلب");

                e.requestOptions.headers['Authorization'] = 'Bearer $newAccess';
                final cloneReq = await _dio.fetch(e.requestOptions);
                return handler.resolve(cloneReq);
              } else {
                return _handleSessionExpired(handler, e);
              }
            } catch (_) {
              return _handleSessionExpired(handler, e);
            }
          }

          return handler.next(e);
        },
      ),
    );
  }

  Future<dynamic> _handleSessionExpired(
    ErrorInterceptorHandler handler,
    DioException e,
  ) async {
    log("⛔ انتهت الجلسة بالكامل. يتم تسجيل الخروج...");
    await SecureStorageHelper.clearAll();
    // سيتم استدعاء التنقل لصفحة الدخول هنا عبر AppRouter
    return handler.next(e);
  }

  // دوال الـ HTTP المشتركة
  Future<dynamic> get(
    String endpoint, {
    Map<String, dynamic>? queryParameters,
  }) async {
    final response = await _dio.get(endpoint, queryParameters: queryParameters);
    return response.data;
  }

  Future<dynamic> post(String endpoint, [dynamic body]) async {
    log(
      "📤 Sending Body: ${body is FormData ? 'Multipart' : jsonEncode(body)}",
    );
    final response = await _dio.post(endpoint, data: body);
    return response.data;
  }

  Future<dynamic> put(String endpoint, dynamic body) async {
    final response = await _dio.put(endpoint, data: body);
    return response.data;
  }

  Future<dynamic> delete(String endpoint, {dynamic body}) async {
    final response = await _dio.delete(endpoint, data: body);
    return response.data;
  }

  Future<dynamic> postMultipart(String endpoint, FormData formData) async {
    final response = await _dio.post(
      endpoint,
      data: formData,
      options: Options(contentType: 'multipart/form-data'),
    );
    return response.data;
  }
}
