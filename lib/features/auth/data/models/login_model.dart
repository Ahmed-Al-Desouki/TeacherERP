class LoginRequestModel {
  final String email;
  final String password;
  final String deviceInfo;

  LoginRequestModel({
    required this.email,
    required this.password,
    this.deviceInfo = 'Mobile App',
  });

  Map<String, dynamic> toJson() => {
    'email': email,
    'password': password,
    'deviceInfo': deviceInfo,
  };
}

class LoginResponseModel {
  final String accessToken;
  final String refreshToken;
  final String expiryDate;

  LoginResponseModel({
    required this.accessToken,
    required this.refreshToken,
    required this.expiryDate,
  });

  factory LoginResponseModel.fromJson(Map<String, dynamic> json) {
    return LoginResponseModel(
      accessToken: json['accessToken'] as String? ?? '',
      refreshToken: json['refreshToken'] as String? ?? '',
      expiryDate: json['expiryDate'] as String? ?? '',
    );
  }
}
