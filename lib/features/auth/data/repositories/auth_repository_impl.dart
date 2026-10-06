import 'package:dartz/dartz.dart';
import 'package:dio/dio.dart';
import '../../../../core/errors/failures.dart';
import '../../../../core/utils/session_manager.dart';
import '../../domain/repositories/auth_repository.dart';
import '../datasources/auth_remote_data_source.dart';
import '../models/login_model.dart';
import '../models/register_student_model.dart';
import '../models/verify_otp_model.dart';

class AuthRepositoryImpl implements AuthRepository {
  final AuthRemoteDataSource _remoteDataSource;
  final SessionManager _sessionManager;

  AuthRepositoryImpl(this._remoteDataSource, this._sessionManager);

  @override
  Future<Either<Failure, RegisterStudentResponseModel>> registerStudent(
    RegisterStudentRequestModel request,
  ) async {
    try {
      final response = await _remoteDataSource.registerStudent(request);
      return Right(response);
    } on DioException catch (e) {
      return Left(ServerFailure.fromDioException(e));
    } catch (e) {
      return Left(ServerFailure(e.toString()));
    }
  }

  @override
  Future<Either<Failure, VerifyOtpResponseModel>> verifyOtp(
    VerifyOtpRequestModel request,
  ) async {
    try {
      final response = await _remoteDataSource.verifyOtp(request);
      return Right(response);
    } on DioException catch (e) {
      return Left(ServerFailure.fromDioException(e));
    } catch (e) {
      return Left(ServerFailure(e.toString()));
    }
  }

  @override
  Future<Either<Failure, LoginResponseModel>> login(
    LoginRequestModel request,
  ) async {
    try {
      final response = await _remoteDataSource.login(request);

      // حفظ التوكن واستخراج الـ Role والـ Claims مباشرة
      await _sessionManager.saveUserFromToken(
        accessToken: response.accessToken,
        refreshToken: response.refreshToken,
      );

      return Right(response);
    } on DioException catch (e) {
      return Left(ServerFailure.fromDioException(e));
    } catch (e) {
      return Left(ServerFailure(e.toString()));
    }
  }
}
