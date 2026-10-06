import 'package:dartz/dartz.dart';
import '../../../../core/errors/failures.dart';
import '../../data/models/login_model.dart';
import '../../data/models/register_student_model.dart';
import '../../data/models/verify_otp_model.dart';

abstract class AuthRepository {
  Future<Either<Failure, RegisterStudentResponseModel>> registerStudent(
    RegisterStudentRequestModel request,
  );

  Future<Either<Failure, VerifyOtpResponseModel>> verifyOtp(
    VerifyOtpRequestModel request,
  );

  Future<Either<Failure, LoginResponseModel>> login(LoginRequestModel request);
}
