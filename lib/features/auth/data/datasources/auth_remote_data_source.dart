import '../../../../core/networking/api_constants.dart';
import '../../../../core/networking/api_service.dart';
import '../models/login_model.dart';
import '../models/register_student_model.dart';
import '../models/verify_otp_model.dart';

abstract class AuthRemoteDataSource {
  Future<RegisterStudentResponseModel> registerStudent(
    RegisterStudentRequestModel request,
  );
  Future<VerifyOtpResponseModel> verifyOtp(VerifyOtpRequestModel request);
  Future<LoginResponseModel> login(LoginRequestModel request);
}

class AuthRemoteDataSourceImpl implements AuthRemoteDataSource {
  final ApiService _apiService;

  AuthRemoteDataSourceImpl(this._apiService);

  @override
  Future<RegisterStudentResponseModel> registerStudent(
    RegisterStudentRequestModel request,
  ) async {
    final response = await _apiService.post(
      ApiConstants.registerStudent,
      request.toJson(),
    );
    return RegisterStudentResponseModel.fromJson(response);
  }

  @override
  Future<VerifyOtpResponseModel> verifyOtp(
    VerifyOtpRequestModel request,
  ) async {
    final response = await _apiService.post(
      ApiConstants.verifyOtp,
      request.toJson(),
    );
    return VerifyOtpResponseModel.fromJson(response);
  }

  @override
  Future<LoginResponseModel> login(LoginRequestModel request) async {
    final response = await _apiService.post(
      ApiConstants.login,
      request.toJson(),
    );
    return LoginResponseModel.fromJson(response);
  }
}
