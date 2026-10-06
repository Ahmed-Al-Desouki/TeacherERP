class VerifyOtpRequestModel {
  final String email;
  final String otpCode;

  VerifyOtpRequestModel({required this.email, required this.otpCode});

  Map<String, dynamic> toJson() => {'email': email, 'otpCode': otpCode};
}

class VerifyOtpResponseModel {
  final String message;

  VerifyOtpResponseModel({required this.message});

  factory VerifyOtpResponseModel.fromJson(Map<String, dynamic> json) {
    return VerifyOtpResponseModel(message: json['message'] as String? ?? '');
  }
}
