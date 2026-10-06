part of 'auth_cubit.dart';

@immutable
sealed class AuthState {}

final class AuthInitial extends AuthState {}

// --- Register States ---
class RegisterLoading extends AuthState {}

class RegisterSuccess extends AuthState {
  final RegisterStudentResponseModel response;
  RegisterSuccess(this.response);
}

class RegisterFailure extends AuthState {
  final String errMessage;
  RegisterFailure(this.errMessage);
}

// --- Verify OTP States ---
class VerifyOtpLoading extends AuthState {}

class VerifyOtpSuccess extends AuthState {
  final VerifyOtpResponseModel response;
  VerifyOtpSuccess(this.response);
}

class VerifyOtpFailure extends AuthState {
  final String errMessage;
  VerifyOtpFailure(this.errMessage);
}

// --- Login States ---
class LoginLoading extends AuthState {}

class LoginSuccess extends AuthState {
  final LoginResponseModel response;
  LoginSuccess(this.response);
}

class LoginFailure extends AuthState {
  final String errMessage;
  LoginFailure(this.errMessage);
}
