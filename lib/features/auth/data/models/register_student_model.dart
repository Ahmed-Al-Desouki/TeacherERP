class RegisterStudentRequestModel {
  final String fullName;
  final String email;
  final String phoneNumber;
  final String password;

  RegisterStudentRequestModel({
    required this.fullName,
    required this.email,
    required this.phoneNumber,
    required this.password,
  });

  Map<String, dynamic> toJson() => {
    'fullName': fullName,
    'email': email,
    'phoneNumber': phoneNumber,
    'password': password,
  };
}

class RegisterStudentResponseModel {
  final String email;
  final String message;

  RegisterStudentResponseModel({required this.email, required this.message});

  factory RegisterStudentResponseModel.fromJson(Map<String, dynamic> json) {
    return RegisterStudentResponseModel(
      email: json['email'] as String? ?? '',
      message: json['message'] as String? ?? '',
    );
  }
}
