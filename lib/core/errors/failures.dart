import 'package:dio/dio.dart';

abstract class Failure {
  final String errmessage;
  const Failure(this.errmessage);
}

class ServerFailure extends Failure {
  ServerFailure(super.errmessage);

  factory ServerFailure.fromDioException(DioException e) {
    switch (e.type) {
      case DioExceptionType.connectionTimeout:
        return ServerFailure('انتهت مهلة الاتصال بالخادم (Connection Timeout)');
      case DioExceptionType.sendTimeout:
        return ServerFailure('انتهت مهلة إرسال البيانات (Send Timeout)');
      case DioExceptionType.receiveTimeout:
        return ServerFailure(
          'انتهت مهلة استلام البيانات من الخادم (Receive Timeout)',
        );
      case DioExceptionType.badResponse:
        return ServerFailure.fromresponse(
          e.response?.statusCode,
          e.response?.data,
        );
      case DioExceptionType.connectionError:
        return ServerFailure('لا يوجد اتصال بالإنترنت، يرجى التحقق من الشبكة');
      case DioExceptionType.cancel:
        return ServerFailure('تم إلغاء الطلب');
      case DioExceptionType.unknown:
        if (e.message != null && e.message!.contains("SocketException")) {
          return ServerFailure('لا يوجد اتصال بالإنترنت');
        }
        return ServerFailure('حدث خطأ غير متوقع، يرجى المحاولة لاحقاً');
      default:
        return ServerFailure('حدث خطأ غير متوقع، يرجى المحاولة لاحقاً');
    }
  }

  factory ServerFailure.fromresponse(int? statusCode, dynamic response) {
    if (statusCode == 400 ||
        statusCode == 401 ||
        statusCode == 403 ||
        statusCode == 409) {
      if (response is Map) {
        // حالة خطأ الـ Business المخصص (مثل مشكلة كلمة المرور)
        if (response.containsKey('errorMessage') &&
            response['errorMessage'] != null) {
          return ServerFailure(response['errorMessage'].toString());
        }

        // حالة أخطاء الـ Validation (ProblemDetails)
        if (response.containsKey('errors') && response['errors'] is Map) {
          final errorsMap = response['errors'] as Map<String, dynamic>;
          if (errorsMap.isNotEmpty) {
            final firstKey = errorsMap.keys.first;
            final firstErrorList = errorsMap[firstKey];
            if (firstErrorList is List && firstErrorList.isNotEmpty) {
              return ServerFailure(firstErrorList.first.toString());
            }
          }
        }

        // حالة وجود رسالة عامة
        if (response.containsKey('message') && response['message'] != null) {
          return ServerFailure(response['message'].toString());
        }

        if (response.containsKey('detail') && response['detail'] != null) {
          return ServerFailure(response['detail'].toString());
        }
      }
      return ServerFailure('بيانات غير صحيحة أو طلب غير صالح.');
    } else if (statusCode == 404) {
      return ServerFailure('العنصر المطلوب غير موجود.');
    } else if (statusCode == 500) {
      return ServerFailure('خطأ داخلي في الخادم، يرجى المحاولة لاحقاً.');
    } else {
      return ServerFailure('حدث خطأ غير متوقع: $statusCode');
    }
  }
}

class CacheFailure extends Failure {
  CacheFailure(super.errmessage);
}

class OfflineFailure extends Failure {
  OfflineFailure(super.errmessage);
}
