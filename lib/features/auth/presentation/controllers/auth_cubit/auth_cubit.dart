import 'package:bloc/bloc.dart';
import 'package:meta/meta.dart';
import 'package:smart_teacher_erp/features/auth/data/models/login_model.dart';
import 'package:smart_teacher_erp/features/auth/data/models/register_student_model.dart';
import 'package:smart_teacher_erp/features/auth/data/models/verify_otp_model.dart';
import 'package:smart_teacher_erp/features/auth/domain/repositories/auth_repository.dart';

part 'auth_state.dart';

class AuthCubit extends Cubit<AuthState> {
  final AuthRepository _authRepository;
  AuthCubit(this._authRepository) : super(AuthInitial());
  Future<void> registerStudent({
    required String fullName,
    required String email,
    required String phoneNumber,
    required String password,
  }) async {
    emit(RegisterLoading());

    final request = RegisterStudentRequestModel(
      fullName: fullName,
      email: email,
      phoneNumber: phoneNumber,
      password: password,
    );

    final result = await _authRepository.registerStudent(request);

    result.fold(
      (failure) => emit(RegisterFailure(failure.errmessage)),
      (response) => emit(RegisterSuccess(response)),
    );
  }

  Future<void> verifyOtp({
    required String email,
    required String otpCode,
  }) async {
    emit(VerifyOtpLoading());

    final request = VerifyOtpRequestModel(email: email, otpCode: otpCode);

    final result = await _authRepository.verifyOtp(request);

    result.fold(
      (failure) => emit(VerifyOtpFailure(failure.errmessage)),
      (response) => emit(VerifyOtpSuccess(response)),
    );
  }

  Future<void> login({
    required String email,
    required String password,
    String deviceInfo = 'Mobile App',
  }) async {
    emit(LoginLoading());

    final request = LoginRequestModel(
      email: email,
      password: password,
      deviceInfo: deviceInfo,
    );

    final result = await _authRepository.login(request);

    result.fold(
      (failure) => emit(LoginFailure(failure.errmessage)),
      (response) => emit(LoginSuccess(response)),
    );
  }
}
