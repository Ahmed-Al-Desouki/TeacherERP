// ignore: unused_import
import 'package:intl/intl.dart' as intl;
import 'app_localizations.dart';

// ignore_for_file: type=lint

/// The translations for Arabic (`ar`).
class AppLocalizationsAr extends AppLocalizations {
  AppLocalizationsAr([String locale = 'ar']) : super(locale);

  @override
  String get appName => 'Smart Teacher ERP';

  @override
  String get login => 'تسجيل الدخول';

  @override
  String get register => 'إنشاء حساب طالب';

  @override
  String get fullName => 'الاسم ثلاثي';

  @override
  String get email => 'البريد الإلكتروني';

  @override
  String get phoneNumber => 'رقم الهاتف';

  @override
  String get password => 'كلمة المرور';

  @override
  String get confirmPassword => 'تأكيد كلمة المرور';

  @override
  String get verifyOtp => 'تأكيد الرمز';

  @override
  String get enterOtpCode => 'أدخل رمز التحقق المرسل لبريدك';

  @override
  String get dontHaveAccount => 'ليس لديك حساب؟';

  @override
  String get alreadyHaveAccount => 'لديك حساب بالفعل؟';

  @override
  String get fieldRequired => 'هذا الحقل مطلوب';

  @override
  String get invalidEmail => 'البريد الإلكتروني غير صالح';

  @override
  String get invalidPhone => 'رقم الهاتف غير صالح';
}
